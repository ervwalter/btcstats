using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace BtcStats
{
	public class Key
	{
		public int Id { get; set; }
		public string StatsKey { get; set; }
		public string Pool { get; set; }
		public string ApiKey { get; set; }
	}

	public class BtcStatsDb : DbContext
	{
		public DbSet<Key> Keys { get; set; }
	}
}