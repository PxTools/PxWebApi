﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using Px.Abstractions.Interfaces;

using PxWeb.Code.Api2.SavedQueryBackend.DatabaseBackend;
using PxWeb.Code.Api2.SavedQueryBackend.FileBackend;

namespace PxWeb.Code.Api2.SavedQueryBackend
{
    public static class SavedQueryBackendServiceCollectionExtensions
    {
        public static void AddSavedQuery(this IServiceCollection services, WebApplicationBuilder builder)
        {
            var backend = builder.Configuration.GetSection("SavedQuery:Backend").Value ?? "File";

            if (backend.Equals("Database", StringComparison.OrdinalIgnoreCase))
            {
                builder.Services.Configure<SavedQueryDatabaseStorageOptions>(builder.Configuration.GetSection("SavedQuery:" + SavedQueryDatabaseStorageOptions.SectionName));
                builder.Services.AddTransient<ISavedQueryStorageBackend, SavedQueryDatabaseStorageBackend>();
            }
            else
            {
                // File storage backend is also the fallback
                builder.Services.Configure<SavedQueryFileStorageOptions>(builder.Configuration.GetSection("SavedQuery:" + SavedQueryFileStorageOptions.SectionName));
                builder.Services.AddTransient<ISavedQueryStorageBackend, SaveQueryFileStorgeBackend>();
            }

        }
    }
}
