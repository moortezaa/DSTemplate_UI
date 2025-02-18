using DSTemplate_UI.Services;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DSTemplate_UI.TagHelpers
{
    [HtmlTargetElement("form", Attributes = "ds-model-type")]
    public class FormTagHelper : TagHelper
    {
        private ViewRendererService _viewRendererService;
        public FormTagHelper(ViewRendererService viewRendererService)
        {
            _viewRendererService = viewRendererService;
        }

        public Type DsModelType { get; set; }
        public object DsFor { get; set; }
        public IEnumerable<string> DSPropertiesToShow { get; set; } = new List<string>();
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.Add("class", "ds-form");
            output.Content.SetHtmlContent(await _viewRendererService.RenderViewToStringAsync("DSForm/Form", DsModelType,
            new[] {
                    new KeyValuePair<string, object>("DSFor", DsFor),
                    new KeyValuePair<string, object>("DSPropertiesToShow", DSPropertiesToShow),
                }));
        }
    }
}
