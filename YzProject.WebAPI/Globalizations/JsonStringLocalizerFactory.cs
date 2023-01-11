﻿using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using System;

namespace YzProject.WebAPI.Globalizations
{
    public class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IDistributedCache _cache;
        public JsonStringLocalizerFactory(IDistributedCache cache)
        {
            _cache = cache;
        }
        public IStringLocalizer Create(Type resourceSource) =>
            new JsonStringLocalizer(_cache);
        public IStringLocalizer Create(string baseName, string location) =>
            new JsonStringLocalizer(_cache);
    }
}
