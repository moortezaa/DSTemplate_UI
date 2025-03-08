using DSTemplate_UI.Business;
using DSTemplate_UI.Interfaces;
using Microsoft.Extensions.DependencyInjection;

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
