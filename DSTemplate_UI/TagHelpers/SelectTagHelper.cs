using DSTemplate_UI.Services;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSTemplate_UI.TagHelpers
{
    [HtmlTargetElement("select", Attributes = "ds-model-type,ds-controller,ds-name")]
    public class SelectTagHelper : TagHelper
    {
        private ViewRendererService _viewRendererService;

        public SelectTagHelper(ViewRendererService viewRendererService)
        {
            _viewRendererService = viewRendererService;
        }

        public Type DsModelType { get; set; }
        public string DsController { get; set; }
        public string DsName { get; set; }
        public string PlaceHolder { get; set; }
        public ModelExpression AspFor { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public bool AllowNewValue { get; set; } = false;
        public IEnumerable<KeyValuePair<string, object>> DSRouteValues { get; set; } = new List<KeyValuePair<string, object>>();
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            try
            {
                await base.ProcessAsync(context, output);

                if (AspFor != null)
                {
                    Id = AspFor.Name;
                    Name = AspFor.Name;
                }

                output.TagName = "div";
                output.TagMode = TagMode.StartTagAndEndTag;
                output.Attributes.Add("class", "ds-select");
                output.Attributes.Add("id", DsName);
                output.Attributes.SetAttribute("Id", DsName); // making sure asp-for doesn't set the id
                output.Attributes.RemoveAll("name"); // remove the name attribute that asp-for might have set
                output.Attributes.Add("data-ds-name", DsName);
                output.Attributes.Add("data-ds-allow-new-value", AllowNewValue);
                output.Attributes.Add("data-ds-data-url", $"/{DsController}/DSGetSelectData");

                if (DSRouteValues.Any())
                {
                    output.Attributes.Add("data-ds-route-values", JsonConvert.SerializeObject(DSRouteValues));
                }

                output.Content.SetHtmlContent(await _viewRendererService.RenderViewToStringAsync("DSSelect/Select", DsModelType,
                    [
                        new KeyValuePair<string, object>("selectName", DsName),
                        new KeyValuePair<string, object>("placeHolder", PlaceHolder),
                        new KeyValuePair<string, object>("Id", Id),
                        new KeyValuePair<string, object>("Name", Name),
                    ]));
            }
            catch (Exception e)
            {
                throw new Exception($"{e.Message}\nStack Trace:\n{e.StackTrace}", e.InnerException);
            }
        }
    }
}
