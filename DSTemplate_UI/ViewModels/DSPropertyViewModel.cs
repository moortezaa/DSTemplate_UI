using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DSTemplate_UI.ViewModels
{
    public class DSPropertyViewModel
    {
        public PropertyInfo PropertyInfo { get; set; }
        public Type PropertyType { get; set; }
        public string Navigation { get; set; }
        public IStringLocalizer Localizer { get; set; }
    }
}
