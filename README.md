# DSTemplate_UI
a fast and easy way to make tables in your razor pages

# Quick Start Guide
as the razor pages mechanic work, you need to copy 2 default .cshtml files to your Views directory.
consider downloading them from the [repository](https://github.com/moortezaa/DSTemplate_UI/tree/master/DSTemplate_UI/Views/DSTable).

do this steps to create a table:

 1. use the `IDSTableController` interface to implement your controller:
 and implement the two functions `DSGetTableData` and `DSGetTableDataCount`
```
public class ModelController : IDSTableController{
    public async Task<JsonResult> DSGetTableData(string tableName, string sortPropertyName, bool? sortDesending, string filters, int page = 1, int rowsPerPage = 10, string routeValues = null)
{
    if (tableName == "custom-table")
    {
        var query = _repository.Query; //must be an IQueryable<Model>
        //get the route values you set in the tag helper
        var routeValuesParsed = JsonConvert.DeserializeObject<List<KeyValuePair<string, object>>>(routeValues);
        var userId = (string?)routeValuesParsed?.Where(x=>x.Key == "userId").FirstOrDefault().Value;
        if (userId == null)
        {
            return Json("user most not be null!");
        }
        //add any filters you want to the queryable
        query = query.Where(g=>g.UserId == new Guid(userId));

        //You MUST use the DoSFP (do sorting filtering and pagination) at the end of any changes to the query
        query = await _dsTableManager.DoSFP(query, sortPropertyName, sortDesending, filters, page, rowsPerPage);

        //Render the rows of the table
        var rows = new List<string>();
        var row = 0;
        foreach (var model in query)
        {
            row++;
            //you can pass view data to the row razor page for custom rows
            ViewData["Row"] = row;
            //use the RenderRow function to render the row.cshtml
            //if you leave the customRowView argument empty it will render the default row.cshtml
            rows.Add(await _dsTableManager.RenderRow(model, ViewData, customRowView: "Gateway/IndexRow"));
        }
        //return the rendered rows with Json function
        return await _dsTableManager.Json(rows, tableName);
    }
    //what ever json you return here will be rendered as the table body
    return Json("invalid table name");
}

public async Task<JsonResult> DSGetTableDataCount(string tableName, string filters, string routeValues = null)
{
    //write the same code from the DSGetTableData function to here exept the DoSFP function
    if (tableName == "custom-table")
    {
        var query = _gatewayAccountManager.GatewayAccountQuery;
        var routeValuesParsed = JsonConvert.DeserializeObject<List<KeyValuePair<string, object>>>(routeValues);
        var userId = (string?)routeValuesParsed?.Where(x => x.Key == "userId").FirstOrDefault().Value;
        if (userId == null)
        {
            return Json("user most not be null!");
        }
        query = query.Where(g => g.UserId == new Guid(userId));
        
        //Instead of DoSFP use the CountData function
        var count = await _dsTableManager.CountData(query, filters);
        return await _dsTableManager.Json(count, tableName);
    }
    return await _dsTableManager.Json(0, tableName);
}
}
```

2. use the `<table></table>` tag like this to make your table:
```
@{
    var modelProperties = new List<string>
    {
        nameof(ModelType.Name),
        nameof(ModelType.Type),
        nameof(ModelType.MerchantId),
    };
}
<table ds-model-type="typeof(ModelType)"
       ds-controller="@nameof(ModelController).Replace("Controller","")"
       ds-name="custom-table"
       ds-rows-per-page="10"
       ds-pre-columns="1" //extra collumns to render at header and footer's start
       ds-post-columns="1" //extra collumns to render at header and footer's end
       ds-properties-to-show="modelProperties" //this will generate the header too
       ds-route-values="@(new []{new KeyValuePair<string,object>("userId",Model.Id)})"></table>
```
