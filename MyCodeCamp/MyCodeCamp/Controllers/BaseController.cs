using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MyCodeCamp.Controllers
{
    public abstract class BaseController : Controller
    {
        public const string UrlHelper = "URLHELPER";
        
        // Move the logger here?

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            context.HttpContext.Items[UrlHelper] = Url;
        }
    }
}
