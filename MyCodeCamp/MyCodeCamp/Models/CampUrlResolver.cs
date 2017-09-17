using AutoMapper;
using MyCodeCamp.Data.Entities;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyCodeCamp.Controllers;

namespace MyCodeCamp.Models
{
    public class CampUrlResolver : IValueResolver<Camp, CampModel, string>
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CampUrlResolver(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public string Resolve(Camp source, CampModel destination, string destMember, ResolutionContext context)
        {
            var url = (IUrlHelper) httpContextAccessor.HttpContext.Items[BaseController.UrlHelper];

            return url.Link("CampGet", new { moniker = source.Moniker });
        }
    }
}
