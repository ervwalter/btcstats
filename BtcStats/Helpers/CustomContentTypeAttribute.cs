using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BtcStats
{
    public class CustomContentTypeAttribute : ActionFilterAttribute
    {
        public string ContentType { get; set; }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {

            filterContext.HttpContext.Response.ContentType = ContentType;

        }
    }
}