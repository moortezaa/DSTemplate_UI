using DSTemplate_UI.Business;
using DSTemplate_UI.Interfaces;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace DSTemplate_UI.Services
{
    public static class IServiceCollectionExtention
    {
        public static void AddDSTemplateUI(this IServiceCollection services)
        {
            services.AddScoped<ViewRendererService>();

            services.AddScoped<IDSTableManager, DSTableManager>();
        }
    }
}
