using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSTemplate_UI.Interfaces
{
    public interface IDSTableManager
    {
        Task<IQueryable<T>> DoSFP<T>(IQueryable<T> datas, string sortPropertyName, bool? sortDesending, string filters, int page = 1, int rowsPerPage = 10);
        Task<string> RenderRow<T>(T data, IEnumerable<KeyValuePair<string, object>> viewData, string customRowView = null);
        Task<string> RenderRow<T>(T data, ViewDataDictionary viewData = null, string customRowView = null);
        Task<JsonResult> Json(IEnumerable<string> rows, string tableName);
        Task<int> CountData<T>(IQueryable<T> datas, string filters);
        Task<JsonResult> Json(int dataCount, string tableName);
    }
}
