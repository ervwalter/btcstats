using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using Newtonsoft.Json.Linq;
using System.Web.UI;
using System.Security.Cryptography;

namespace BtcStats.Controllers
{
    public class SigController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetKey(string pool, string apiKey)
        {
            using (BtcStatsDb db = new BtcStatsDb())
            {
                pool = pool.Trim();
                apiKey = apiKey.Trim();
                string existingKey = (from s in db.Keys
                                      where s.Pool == pool && s.ApiKey == apiKey
                                      select s.StatsKey).FirstOrDefault();

                if (existingKey != null)
                {
                    return Content(existingKey);
                }

                string statsKey;
                while (true)
                {
                    RandomNumberGenerator rnd = RNGCryptoServiceProvider.Create();
                    byte[] keyBytes = new byte[4];
                    rnd.GetNonZeroBytes(keyBytes);
                    statsKey = HttpServerUtility.UrlTokenEncode(keyBytes).Replace('_', 'a').Replace("-", "x");
                    Key duplicateKey = (from s in db.Keys
                                        where s.StatsKey == statsKey
                                        select s).FirstOrDefault();
                    if (duplicateKey == null)
                    {
                        Key newEntry = new Key();
                        newEntry.StatsKey = statsKey;
                        newEntry.Pool = pool;
                        newEntry.ApiKey = apiKey;
                        db.Keys.Add(newEntry);
                        db.SaveChanges();
                        break;
                    }
                }

                return Content(statsKey);
            }
        }

        [OutputCache(Location = OutputCacheLocation.ServerAndClient, Duration = 900)]
        [CustomContentType(ContentType = "image/png", Order = 2)]
        public ActionResult Generic(string key, ImageType mode)
        {
            using (BtcStatsDb db = new BtcStatsDb())
            {
                Key keyDetails = (from s in db.Keys
                                  where s.StatsKey == key
                                  select s).FirstOrDefault();

                if (keyDetails != null)
                {
                    switch (keyDetails.Pool)
                    {
                        case "eligius":
                            return Eligius(keyDetails.ApiKey, mode);
                        case "ozcoin":
                            return Ozcoin(keyDetails.ApiKey, mode);
                        case "ars":
                            return Ars(keyDetails.ApiKey, mode);
                        case "mmc":
                            return MMC(keyDetails.ApiKey, mode);
                        case "yourbtc":
                            return YourBtc(keyDetails.ApiKey, mode);
                        case "abc":
                            return ABC(keyDetails.ApiKey, mode);
                        case "emc":
                            return EMC(keyDetails.ApiKey, mode);
                        case "p2pool":
                            return P2Pool(keyDetails.ApiKey, mode);
                        case "slush":
                            return Slush(keyDetails.ApiKey, mode);
                        case "mtred":
                            return MtRed(keyDetails.ApiKey, mode);
                    }
                }
            }
            if (mode == ImageType.Sig)
            {
                return new FilePathResult(Server.MapPath("~/Content/KeyNotFound.png"), "image/png");
            }
            else
            {
                return new FilePathResult(Server.MapPath("~/Content/KeyNotFoundAvatar.png"), "image/png");
            }
        }

        [OutputCache(Location = OutputCacheLocation.ServerAndClient, Duration = 900)]
        [CustomContentType(ContentType = "image/png", Order = 2)]
        public ActionResult Ars(string key, ImageType mode)
        {
            decimal hashRate = 0;

            try
            {
                string url = string.Format("http://arsbitcoin.com/api.php?api_key={0}", key);
                BtcStatsWebClient client = new BtcStatsWebClient();
                client.RequestTimeout = 30000;
                string response = client.DownloadString(url);
                JObject data = JObject.Parse(response);
                hashRate = decimal.Parse((string)data["hashrate"]) * 1000000;
            }
            catch
            {
                //absorb
            }


            return GenerateImage(hashRate, "Ars", mode);
        }

        [OutputCache(Location = OutputCacheLocation.ServerAndClient, Duration = 900)]
        [CustomContentType(ContentType = "image/png", Order = 2)]
        public ActionResult YourBtc(string key, ImageType mode)
        {
            decimal hashRate = 0;

            try
            {
                string url = string.Format("http://www.yourbtc.net/api.php?api_key={0}", key);
                BtcStatsWebClient client = new BtcStatsWebClient();
                client.RequestTimeout = 30000;
                string response = client.DownloadString(url);
                JObject data = JObject.Parse(response);
                string rawHashrate = (string)data["user"]["hashrate"];
                hashRate = ParseHashRate(rawHashrate);
            }
            catch
            {
                //absorb
            }

            return GenerateImage(hashRate, "YourBtc", mode);
        }

        [OutputCache(Location = OutputCacheLocation.ServerAndClient, Duration = 900)]
        [CustomContentType(ContentType = "image/png", Order = 2)]
        public ActionResult MMC(string key, ImageType mode)
        {
            decimal hashRate = 0;

            try
            {
                string url = string.Format("http://mining.mainframe.nl/api?api_key={0}", key);
                BtcStatsWebClient client = new BtcStatsWebClient();
                client.RequestTimeout = 30000;
                string response = client.DownloadString(url);
                JObject data = JObject.Parse(response);
                hashRate = decimal.Parse((string)data["total_hashrate"]) * 1000000;
            }
            catch
            {
                //absorb
            }

            return GenerateImage(hashRate, "MMC", mode);
        }

        [OutputCache(Location = OutputCacheLocation.ServerAndClient, Duration = 900)]
        [CustomContentType(ContentType = "image/png", Order = 2)]
        public ActionResult Ozcoin(string key, ImageType mode)
        {
            decimal hashRate = 0;

            try
            {
                string url = string.Format("http://ozco.in/api.php?api_key={0}", key);
                BtcStatsWebClient client = new BtcStatsWebClient();
                client.RequestTimeout = 30000;
                string response = client.DownloadString(url);
                JObject data = JObject.Parse(response);
                hashRate = decimal.Parse((string)data["user"]["hashrate_raw"]) * 1000000;
            }
            catch
            {
                //absorb
            }

            return GenerateImage(hashRate, "Ozcoin", mode);
        }

        [OutputCache(Location = OutputCacheLocation.ServerAndClient, Duration = 900)]
        [CustomContentType(ContentType = "image/png", Order = 2)]
        public ActionResult ABC(string key, ImageType mode)
        {
            decimal hashRate = 0;

            try
            {
                string url = string.Format("http://www.abcpool.co/api.php?api_key={0}", key);
                BtcStatsWebClient client = new BtcStatsWebClient();
                client.RequestTimeout = 30000;
                string response = client.DownloadString(url);
                JObject data = JObject.Parse(response);
                hashRate = (decimal)data["hashrate"] * 1000000;
            }
            catch
            {
                //absorb
            }

            return GenerateImage(hashRate, "ABC", mode);
        }


        [OutputCache(Location = OutputCacheLocation.ServerAndClient, Duration = 900)]
        [CustomContentType(ContentType = "image/png", Order = 2)]
        public ActionResult Eligius(string key, ImageType mode)
        {
            decimal hashRate = 0;

            if (key == "1GEJfZRPrK2BLSSx3r6gwtuFxCUvq3QytN")
            {
                key = "16kNKa7WUg8QAPFy8dJRv7USSu2fAG2pkW";
            }

            try
            {
                string url = string.Format("http://eligius.st/~twmz/hashrate.php?addr={0}", key);
                BtcStatsWebClient client = new BtcStatsWebClient();
                client.RequestTimeout = 30000;
                string response = client.DownloadString(url);
                JObject data = JObject.Parse(response);
                hashRate = (decimal)data["hashrate"];
            }
            catch
            {
                //absorb
            }

            return GenerateImage(hashRate, "Eligius", mode);
        }

        //https://eclipsemc.com/api.php?key=2ca08a5062dc40d2151b93b0170631&action=userstats
        [OutputCache(Location = OutputCacheLocation.ServerAndClient, Duration = 900)]
        [CustomContentType(ContentType = "image/png", Order = 2)]
        public ActionResult EMC(string key, ImageType mode)
        {
            decimal hashRate = 0;

            try
            {
                string url = string.Format("https://eclipsemc.com/api.php?key={0}&action=userstats", key);
                BtcStatsWebClient client = new BtcStatsWebClient();
                client.RequestTimeout = 30000;
                string response = client.DownloadString(url);
                JObject data = JObject.Parse(response);
                hashRate = (from w in data["workers"].Children() select w).Sum(w => ParseHashRate(w["hash_rate"]));
            }
            catch
            {
                //absorb
            }

            return GenerateImage(hashRate, "Emc", mode);
        }

        [OutputCache(Location = OutputCacheLocation.ServerAndClient, Duration = 900)]
        [CustomContentType(ContentType = "image/png", Order = 2)]
        public ActionResult MtRed(string key, ImageType mode)
        {
            decimal hashRate = 0;

            try
            {
                string url = string.Format("https://mtred.com/api/user/key/{0}", key);
                BtcStatsWebClient client = new BtcStatsWebClient();
                client.RequestTimeout = 30000;
                string response = client.DownloadString(url);
                JObject data = JObject.Parse(response);
                hashRate = (from w in data["workers"].Children() select w).Sum(w => (decimal)((JProperty)w).Value["mhash"] * 1000000);
            }
            catch
            {
                //absorb
            }

            return GenerateImage(hashRate, "MtRed", mode);
        }

        [OutputCache(Location = OutputCacheLocation.ServerAndClient, Duration = 3600)]
        [CustomContentType(ContentType = "image/png", Order = 2)]
        public ActionResult Slush(string key, ImageType mode)
        {
            decimal hashRate = 0;
            string image = "Slush";
            try
            {
                string url = string.Format("http://mining.bitcoin.cz/accounts/profile/json/{0}", key);
                BtcStatsWebClient client = new BtcStatsWebClient();
                client.RequestTimeout = 30000;
                string response = client.DownloadString(url);
                JObject data = JObject.Parse(response);
                hashRate = decimal.Parse((string)data["hashrate"]) * 1000000;
                if ((string)data["rating"] == "trusted")
                {
                    image = "SlushTrusted";
                }
                else if ((string)data["rating"] == "vip")
                {
                    image = "SlushVIP";
                }
            }
            catch (Exception ex)
            {
                //absorb
            }

            return GenerateImage(hashRate, image, mode);
        }

        [OutputCache(Location = OutputCacheLocation.ServerAndClient, Duration = 900)]
        [CustomContentType(ContentType = "image/png", Order = 2)]
        public ActionResult P2Pool(string key, ImageType mode)
        {
            decimal hashRate = 0;

            try
            {
                string url = "http://p2pool.info/users";
                BtcStatsWebClient client = new BtcStatsWebClient();
                client.RequestTimeout = 30000;
                string response = client.DownloadString(url);
                JArray data = JArray.Parse(response);
                string hrText = (from u in data
                                 where (string)u["Address"] == key
                                 select (string)u["Hashrate"]).FirstOrDefault();
                if (hrText != null)
                {
                    hashRate = decimal.Parse(hrText.Substring(0, hrText.Length - 5)) * 1000000;
                }
            }
            catch
            {
                //absorb
            }

            return GenerateImage(hashRate, "p2p", mode);
        }


        private ActionResult GenerateImage(string hashRate, string backgroundPath, ImageType mode)
        {
            byte[] output = null;

            string filename;
            float fontSize;
            if (mode == ImageType.Sig)
            {
                fontSize = 14f;
                filename = backgroundPath + "Background.png";
            }
            else
            {
                fontSize = 18f;
                filename = backgroundPath + "Avatar.png";
            }
            using (Image background = Image.FromFile(Server.MapPath("~/Content/" + filename)))
            {
                using (Bitmap bar = new Bitmap(background))
                {
                    using (Font font = new Font("Arial", fontSize, FontStyle.Bold, GraphicsUnit.Pixel))
                    {
                        using (Graphics g = Graphics.FromImage(bar))
                        {
                            StringFormat format = new StringFormat();
                            format.Alignment = StringAlignment.Far;
                            if (mode == ImageType.Sig)
                            {
                                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                                string message = string.Format("I'm mining at {0}", hashRate);
                                format.LineAlignment = StringAlignment.Center;
                                g.DrawString(message, font, Brushes.White, new RectangleF(-5, 0, background.Width, background.Height), format);
                            }
                            else
                            {
                                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                                string message = string.Format("{0}", hashRate);
                                format.LineAlignment = StringAlignment.Far;
                                format.Alignment = StringAlignment.Center;
                                g.DrawString(message, font, Brushes.White, new RectangleF(0, -13, background.Width, background.Height), format);
                            }
                        }
                    }

                    using (MemoryStream stream = new MemoryStream())
                    {
                        bar.Save(stream, ImageFormat.Png);
                        output = stream.ToArray();
                    }
                }
            }

            return new FileContentResult(output, "image/png");
        }

        private ActionResult GenerateImage(decimal hashRate, string backgroundPath, ImageType mode)
        {
            string units;
            string normalizedRate;

            if (hashRate > 1000000000)
            {
                normalizedRate = Math.Round(hashRate / 1000000000, 2).ToString("#,0.00");
                units = "G";
            }
            else if (hashRate > 1000000 || hashRate == 0)
            {
                normalizedRate = Math.Round(hashRate / 1000000, 0).ToString();
                units = "M";
            }
            else
            {
                normalizedRate = Math.Round(hashRate / 1000, 0).ToString();
                units = "k";
            }

            string hashRateString = string.Format("{0} {1}H/s", normalizedRate, units);

            return GenerateImage(hashRateString, backgroundPath, mode);
        }

        private decimal ParseHashRate(JToken rawHashrate)
        {
            try
            {
                return ParseHashRate((string)rawHashrate);
            }
            catch
            {
                return 0;
            }
        }

        private decimal ParseHashRate(string rawHashrate)
        {
            decimal hashRate = 0;
            try
            {
                string[] pieces = rawHashrate.ToLower().Split(' ');
                if (pieces.Length == 2)
                {
                    hashRate = decimal.Parse(pieces[0].Trim());
                    if (pieces[1].Contains("mh"))
                    {
                        hashRate *= 1000000;
                    }
                    else if (pieces[1].Contains("gh"))
                    {
                        hashRate *= 1000000000;
                    }
                    else
                    {
                        hashRate = 0;
                    }
                }
            }
            catch
            {
                //absorb 
            }
            return hashRate;
        }


    }

    public enum ImageType
    {
        Sig,
        Avatar
    }
}
