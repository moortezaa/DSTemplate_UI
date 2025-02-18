using DSTemplate_UI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TestUI.Models;

namespace TestUI.Controllers
{
    public class HomeController : Controller, IDSTableController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDSTableManager _dSTableManager;

        public HomeController(ILogger<HomeController> logger, IDSTableManager dSTableManager)
        {
            _logger = logger;
            _dSTableManager = dSTableManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<JsonResult> DSGetTableData(string tableName, string sortPropertyName, bool? sortDesending, string filters, int page = 1, int rowsPerPage = 10, string routeValues = null)
        {
            var class1s = new List<Class1>
            {
                new Class1
                {
                    title = "Title 1",
                    Class2 = new Class2()
                    {
                        Class3 = new Class3
                        {
                            Class4s =
                            [
                                new Class4() {
                                    description = "Description 1"
                                },
                                new Class4() {
                                    description = "Description 2"
                                }
                            ]
                        }
                    }
                },
                new Class1
                {
                    title = "Title 2",
                    Class2 = new Class2()
                    {
                        Class3 = new Class3
                        {
                            Class4s =
                            [
                                new Class4() {
                                    description = "Description 3"
                                },
                                new Class4() {
                                    description = "Description 4"
                                }
                            ]
                        }
                    }
                }
            };

            var query = class1s.AsQueryable();
            query = await _dSTableManager.DoSFP(query, sortPropertyName, sortDesending, filters, page, rowsPerPage);

            var rows = new List<string>();
            foreach (var row in query)
            {
                rows.Add(await _dSTableManager.RenderRow(row));
            }
            return await _dSTableManager.Json(rows, tableName);
        }

        public Task<JsonResult> DSGetTableDataCount(string tableName, string filters, string routeValues = null)
        {
            return _dSTableManager.Json(2, tableName);
        }
    }
}
