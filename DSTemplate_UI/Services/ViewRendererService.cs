using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

namespace DSTemplate_UI.Services
{
    public class ViewRendererService
    {

        private IRazorViewEngine _razorViewEngine;
        private ITempDataProvider _tempDataProvider;
        private IServiceProvider _serviceProvider;

        public ViewRendererService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _tempDataProvider = serviceProvider.GetRequiredService<ITempDataProvider>();
            _razorViewEngine = serviceProvider.GetRequiredService<IRazorViewEngine>();
        }

        public async Task<string> RenderViewToStringAsync(string viewName, object model, IEnumerable<KeyValuePair<string, object>> viewData)
        {
            var vD = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };
            foreach (var item in viewData)
            {
                vD[item.Key] = item.Value;
            }
            return await RenderViewToStringAsync(viewName, model, vD);
        }
        public async Task<string> RenderViewToStringAsync(string viewName, object model, ViewDataDictionary viewData = null)
        {
            var httpContext = new DefaultHttpContext()
            {
                RequestServices = _serviceProvider
            };

            var endpointFeature = new EndpointFeature
            {
                Endpoint = new((c) => Task.CompletedTask, new EndpointMetadataCollection(), "")
            };
            httpContext.Features.Set<IEndpointFeature>(endpointFeature);
            var routeData = new RouteData();
            var actionDescriptor = new ActionDescriptor();
            var actionContext = new ActionContext(httpContext, routeData, actionDescriptor);
            using var sw = new StringWriter();
            var viewResult = _razorViewEngine.FindView(actionContext, viewName, false);

            if (viewResult.View == null)
            {
                throw new Exception($"view for {viewName} not found." +
                    $"\n searched locations:" +
                    $"\n{string.Join('\n',viewResult.SearchedLocations)}");
            }
            if (viewData != null)
            {
                viewData = new ViewDataDictionary(viewData)
                {
                    Model = model,
                };
            }
            else
            {
                viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                };
            }

            var viewContext = new ViewContext(
                actionContext,
                viewResult.View,
                viewData,
                new TempDataDictionary(httpContext, _tempDataProvider),
                sw,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext);

            return sw.ToString();
        }

        private class EndpointFeature : IEndpointFeature
        {
            public Endpoint Endpoint { get; set; }
        }
    }
}
