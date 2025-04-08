using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSTemplate_UI.Interfaces
{

    public interface IDSTableController
    {
        Task<JsonResult> DSGetTableData(string tableName, string sortPropertyName, bool? sortDesending, string filters, int page = 1, int rowsPerPage = 10);
        Task<JsonResult> DSGetTableDataCount(string tableName, string filters);
    }
}
