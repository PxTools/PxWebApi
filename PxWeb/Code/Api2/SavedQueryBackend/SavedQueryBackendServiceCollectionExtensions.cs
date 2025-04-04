using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using Px.Abstractions.Interfaces;

using PxWeb.Code.Api2.SavedQueryBackend.FileBackend;

namespace PxWeb.Code.Api2.SavedQueryBackend
{
    public static class SavedQueryBackendServiceCollectionExtensions
    {
        public static void AddSavedQuery(this IServiceCollection services, WebApplicationBuilder builder)
        {
            var backend = builder.Configuration.GetSection("SavedQuery:Backend");

            if (backend.Value != null && backend.Value.ToUpper() == "DATABASE")
            {
                // Configure database backend
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
