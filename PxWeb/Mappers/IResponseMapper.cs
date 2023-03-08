﻿using Microsoft.AspNetCore.Http;
using PCAxis.Menu;
using PxWeb.Api2.Server.Models;

namespace PxWeb.Mappers
{
    public interface IResponseMapper
    {
        public Folder GetFolder(PxMenuItem currentItem, HttpContext httpContext);
    }
}
