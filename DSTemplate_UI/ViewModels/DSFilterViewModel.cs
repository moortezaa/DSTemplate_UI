using DSTemplate_UI.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSTemplate_UI.ViewModels
{
    public class DSFilterViewModel
    {
        public string PropertyName { get; set; }
        public string FilterTerm { get; set; }
        public DSFilterTermViewModel FilterTerms { get; set; }
    }

    public class DSFilterTermViewModel
    {
        public string From { get; set; }
        public string To { get; set; }
    }
}
