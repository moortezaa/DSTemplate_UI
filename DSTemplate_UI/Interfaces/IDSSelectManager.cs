using DSTemplate_UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DSTemplate_UI.Interfaces
{
    public interface IDSSelectManager
    {
        Task<JsonResult> Json<T>(string selectName, IEnumerable<T> options, string valueField, string textField);
    }
}
