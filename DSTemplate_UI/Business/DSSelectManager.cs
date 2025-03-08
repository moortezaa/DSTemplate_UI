using DSTemplate_UI.Interfaces;
using DSTemplate_UI.Services;
using DSTemplate_UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DSTemplate_UI.Business
{
    internal class DSSelectManager : IDSSelectManager
    {

        public async Task<JsonResult> Json<T>(string selectName, IEnumerable<T> options, string valueField, string textField)
        {
            var dsOptions = new List<DSSelectOption>();
            var valueProp = typeof(T).GetProperty(valueField);
            var textProp = typeof(T).GetProperty(textField);
            foreach (var option in options)
            {
                dsOptions.Add(new DSSelectOption()
                {
                    Value = valueProp.GetValue(option),
                    Text = textProp.GetValue(option).ToString()
                });

            }
            return new JsonResult(new { options = dsOptions, selectName });
        }
    }
}
