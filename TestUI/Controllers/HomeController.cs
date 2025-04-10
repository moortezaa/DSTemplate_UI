using DSTemplate_UI.Interfaces;
using DSTemplate_UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TestUI.Models;

namespace TestUI.Controllers
{
    public class HomeController : Controller, IDSTableController, IDSSelectController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDSTableManager _dSTableManager;
        private readonly IDSSelectManager _dSSelectManager;

        public HomeController(ILogger<HomeController> logger, IDSTableManager dSTableManager, IDSSelectManager dSSelectManager)
        {
            _logger = logger;
            _dSTableManager = dSTableManager;
            _dSSelectManager = dSSelectManager;
        }

        public IActionResult Index()
        {
            return View(new Class1() { Id = new Guid(), title = "some title" });
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

        public async Task<JsonResult> DSGetTableData(string tableName, string sortPropertyName, bool? sortDesending, string filters, int page = 1, int rowsPerPage = 10)
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
                rows.Add(await _dSTableManager.RenderRow(row, customRowView: "Home/IndexRow"));
            }
            return await _dSTableManager.Json(rows, tableName);
        }

        public Task<JsonResult> DSGetTableDataCount(string tableName, string filters)
        {
            return _dSTableManager.Json(2, tableName);
        }

        public async Task<JsonResult> DSGetSelectData(string selectName, string filter, string? selectedKey = null)
        {
            Class1[] entities = [
                new Class1(){Id=new Guid(),title="some title"},
                new Class1(){Id=new Guid(),title="title 1"},
                new Class1(){Id=new Guid(),title="no"},
                new Class1(){Id=new Guid(),title="here is a title"},
                ];
            if (selectName == "theselect")
            {
                if (selectedKey != null)
                {
                    var key = new Guid(selectedKey);
                    //instead of filtering we want the query to return only a sigle element in the list and that element is the selected one
                    return await _dSSelectManager.Json(selectName, entities.Where(e => e.Id == key), nameof(Class1.Id), nameof(Class1.title));
                }
                var filtered = entities.Where(e => e.title.Contains(filter ?? ""));
                return await _dSSelectManager.Json(selectName, filtered, nameof(Class1.Id), nameof(Class1.title));
            }
            return Json("Select not found.");
        }
    }
}
