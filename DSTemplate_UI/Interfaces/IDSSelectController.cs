using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSTemplate_UI.Interfaces
{

    public interface IDSSelectController
    {
        Task<JsonResult> DSGetSelectData(string selectName, string filter,object? selectedKey = null);
    }
}
