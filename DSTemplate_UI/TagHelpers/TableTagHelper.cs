using DSTemplate_UI.Services;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSTemplate_UI.TagHelpers
{
    [HtmlTargetElement("table", Attributes = "ds-model-type,ds-controller,ds-name")]
    public class TableTagHelper : TagHelper
    {
        private ViewRendererService _viewRendererService;

        public TableTagHelper(ViewRendererService viewRendererService)
        {
            _viewRendererService = viewRendererService;
        }

        public Type DsModelType { get; set; }
        public string DsController { get; set; }
        public string DsName { get; set; }
        public int? DSRowsPerPage { get; set; }
        public int? DSPreColumns { get; set; }
        public int? DSPostColumns { get; set; }
        public IEnumerable<string> DSPropertiesToShow { get; set; } = new List<string>();
        public IEnumerable<string> DSDisableFilters { get; set; } = new List<string>();
        public IEnumerable<KeyValuePair<string, object>> DSRouteValues { get; set; } = new List<KeyValuePair<string, object>>();
        public IEnumerable<KeyValuePair<string, Type>> DSPropertiesTypes { get; set; } = new List<KeyValuePair<string, Type>>();
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            try
            {
                await base.ProcessAsync(context, output);

                var propIndex = 0;
                foreach (var propName in DSPropertiesToShow)
                {
                    if (!DSPropertiesTypes.Any(t => t.Key == propName))
                    {
                        var propType = DsModelType.GetProperty(propName);
                        if (propName.Contains("."))
                        {
                            var props = propName.Split('.');
                            propType = DsModelType.GetProperty(props[0]);
                            props = props.Skip(1).ToArray();
                            foreach (var prop in props)
                            {
                                //check if it is an enumerable or a drived class of IEnumerable
                                if (propType.PropertyType.GetInterface("IEnumerable") != null)
                                {
                                    propType = propType.PropertyType.GetGenericArguments()[0].GetProperty(prop);
                                }
                                else
                                {
                                    propType = propType.PropertyType.GetProperty(prop);
                                }
                            }
                        }
                        ((List<KeyValuePair<string, Type>>)DSPropertiesTypes).Insert(propIndex, new KeyValuePair<string, Type>(propName, propType.PropertyType));
                    }
                    propIndex++;
                }

                output.TagName = "table";
                output.TagMode = TagMode.StartTagAndEndTag;
                output.Attributes.Add("class", "table ds-table");
                output.Attributes.Add("name", DsName);
                output.Attributes.Add("id", DsName);
                output.Attributes.Add("data-ds-data-url", $"/{DsController}/DSGetTableData");
                output.Attributes.Add("data-ds-count-url", $"/{DsController}/DSGetTableDataCount");
                if (DSRowsPerPage != null)
                {
                    output.Attributes.Add("data-ds-rows-per-page", DSRowsPerPage);
                }
                if (DSRouteValues.Any())
                {
                    output.Attributes.Add("data-ds-route-values", JsonConvert.SerializeObject(DSRouteValues));
                }
                output.Content.SetHtmlContent(await _viewRendererService.RenderViewToStringAsync("DSTable/Header", DsModelType,
                    new[] {
                    new KeyValuePair<string, object>("tableName", DsName),
                    new KeyValuePair<string, object>("preColumns", DSPreColumns),
                    new KeyValuePair<string, object>("postColumns", DSPostColumns),
                    new KeyValuePair<string, object>("propertiesTypes", DSPropertiesTypes),
                    new KeyValuePair<string, object>("disableFilters", DSDisableFilters),
                    }));
                output.Content.AppendHtml("<tbody></tbody>");
                output.Content.AppendHtml(await _viewRendererService.RenderViewToStringAsync("DSTable/Footer", DsModelType));
            }
            catch (Exception e)
            {
                throw new Exception($"{e.Message}\nStack Trace:\n{e.StackTrace}", e.InnerException);
            }
        }
    }
}
