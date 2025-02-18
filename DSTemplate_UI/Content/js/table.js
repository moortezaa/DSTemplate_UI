$(document).ready(() => {
    var tables = $('.ds-table');
    for (var i = 0; i < tables.length; i++) {
        var table = $(tables[i]);
        table.data('ds-page', 1);
        var rowsPerPage = table.data('ds-rows-per-page');
        if (rowsPerPage == undefined) {
            table.data('rows-per-page', 10);
        }
        DSGetTableData(table);
        var count = table.data('ds-count');
        if (count == undefined) {
            table.data('ds-count', 0);
        }
    }
    var dateFilters = $('.date-time-filter');
    for (var i = 0; i < dateFilters.length; i++) {
        var dateFilter = dateFilters[i];
        var tableName = $(dateFilter).data('table-name');
        var propName = $(dateFilter).data('prop-name');
        var spanstartId = tableName + '-span-start' + propName;
        var textstartId = tableName + '-input-start' + propName;
        var datestartId = tableName + '-filter-start' + propName;
        var spanendId = tableName + '-span-end' + propName;
        var textendId = tableName + '-input-end' + propName;
        var dateendId = tableName + '-filter-end' + propName;
        new mds.MdsPersianDateTimePicker(document.getElementById(spanstartId), {
            targetTextSelector: '#' + textstartId,
            targetDateSelector: '#' + datestartId,
            enableTimePicker: true,
            dateFormat: "yyyy-MM-ddTHH:mm:ss.000",
        });
        new mds.MdsPersianDateTimePicker(document.getElementById(spanendId), {
            targetTextSelector: '#' + textendId,
            targetDateSelector: '#' + dateendId,
            enableTimePicker: true,
            dateFormat: "yyyy-MM-ddTHH:mm:ss.000",
        });
    }
})
function DSSortClick(propertyName, button) {
    table = $(button).parentsUntil('table').parent();
    var desending = table.data('ds-sort-desending');
    if (desending) {
        desending = false;
    } else {
        desending = true;
    }

    table.data('ds-sort-prop-name', propertyName);
    table.data('ds-sort-desending', desending);

    DSGetTableData(table);
}

function DSFilterClick(propertyName, button) {
    table = $(button).parentsUntil('table').parent();
    table.data('ds-page', 1);
    var filters = table.data('ds-filters');
    if (filters == undefined) {
        filters = [];
    } else {
        for (var i = 0; i < filters.length; i++) {
            var filter = filters[i];
            if (filter.propertyName == propertyName) {
                filters.splice(i, 1);
                break
            }
        }
    }
    var inputs = $(button).parent().find('input');
    if (inputs.length > 1) {
        filters.push({
            propertyName: propertyName,
            filterTerms: {
                from: $(inputs[0]).val(),
                to: $(inputs[1]).val(),
            },
        })
    } else if (inputs.length == 1) {
        if (inputs.attr('type') == 'checkbox') {
            filters.push({
                propertyName: propertyName,
                filterTerm: inputs[0].checked
            })
        } else {
            filters.push({
                propertyName: propertyName,
                filterTerm: inputs.val()
            })
        }
    } else if (inputs.length == 0) {
        var selects = $(button).parent().find('select');
        if (selects.length == 1) {
            filters.push({
                propertyName: propertyName,
                filterTerm: selects.val()
            })
        }
    }
    table.data('ds-filters', filters);
    DSGetTableData(table);
    $(button).parent().collapse('hide');
}

function DSClearFilterClick(propertyName, button) {
    table = $(button).parentsUntil('table').parent();
    table.data('ds-page', 1);
    var filters = table.data('ds-filters');
    if (filters != undefined) {
        for (var i = 0; i < filters.length; i++) {
            var filter = filters[i];
            if (filter.propertyName == propertyName) {
                filters.splice(i, 1);
                break
            }
        }
    }
    var inputs = $(button).parent().find('input');
    inputs.val('');
    var selects = $(button).parent().find('select');
    selects.val('');
    DSGetTableData(table);
    $(button).parent().collapse('hide');
}

function DSGetTableData(table) {
    var name = table.attr('name');
    var dataUrl = table.data('ds-data-url');
    var countUrl = table.data('ds-count-url');
    var desending = table.data('ds-sort-desending');
    var sortProp = table.data('ds-sort-prop-name');
    var filters = table.data('ds-filters');
    var page = table.data('ds-page');
    var rowsPerPage = table.data('ds-rows-per-page');
    var routeValues = table.data('ds-route-values');

    var data = new FormData();
    data.append('tableName', name);
    if (filters != undefined) {
        data.append('filters', JSON.stringify(filters));
    }

    if (routeValues != undefined) {
        for (let i = 0; i < routeValues.length; i++) {
            var routeValue = routeValues[i];
            data.append(routeValue.Key, routeValue.Value);
        }
    }

    $.ajax({
        url: countUrl,
        type: "post",
        dataType: "json",
        data: data,
        processData: false,
        contentType: false,
        success: function (data, status) {
            var table = $('#' + data.tableName);
            table.data('ds-count', data.dataCount);
            RenderPagination(table);
        },
        error: function (xhr, desc, err) {
            console.error(desc, err);
        }
    });

    var data = new FormData();
    data.append('tableName', name);
    if (filters != undefined) {
        data.append('filters', JSON.stringify(filters));
    }
    data.append('sortPropertyName', sortProp);
    data.append('sortDesending', desending);
    data.append('page', page);
    data.append('rowsPerPage', rowsPerPage);

    if (routeValues != undefined) {
        for (let i = 0; i < routeValues.length; i++) {
            var routeValue = routeValues[i];
            data.append(routeValue.Key, routeValue.Value);
        }
    }

    $.ajax({
        url: dataUrl,
        type: "post",
        dataType: "json",
        data: data,
        processData: false,
        contentType: false,
        success: function (data, status) {
            var table = $('#' + data.tableName);
            var tbody = table.find('tbody');
            tbody.html('');
            for (var i = 0; i < data.rows.length; i++) {
                var row = data.rows[i];
                tbody.append(row);
            }
        },
        error: function (xhr, desc, err) {
            console.error(desc, err);
        }
    });
}

function DSTablePageNext(page, button) {
    table = $(button).parentsUntil('table').parent();
    var currentPage = table.data('ds-page');
    var lastPage = DSGetLastPage(table);
    var nextPage = currentPage + page;
    if (nextPage > lastPage) {
        nextPage = lastPage
    }
    table.data('ds-page', nextPage);
    DSGetTableData(table);
}

function DSGetLastPage(table) {
    var rowsPerPage = table.data('ds-rows-per-page');
    var count = table.data('ds-count');
    var lastPage = count / rowsPerPage;
    if (lastPage % 1 != 0) {
        lastPage += 1 - lastPage % 1;
    }
    return lastPage;
}

function DSTablePageBack(page, button) {
    table = $(button).parentsUntil('table').parent();
    var currentPage = table.data('ds-page');
    var nextPage = currentPage - page;
    if (nextPage < 1) {
        nextPage = 1
    }
    table.data('ds-page', nextPage);
    DSGetTableData(table);
}

function DSTablePageGoto(page, button) {
    table = $(button).parentsUntil('table').parent();
    var lastPage = DSGetLastPage(table);
    if (page == -1) {
        page = parseInt(table.find('.ds-pagination-input-goto').val());
    }
    var nextPage = page;
    if (nextPage < 1) {
        nextPage = 1
    }
    if (nextPage > lastPage) {
        nextPage = lastPage
    }
    table.data('ds-page', nextPage);
    DSGetTableData(table);
}

function RenderPagination(table) {
    var currentPage = table.data('ds-page');
    var lastPage = DSGetLastPage(table);
    var nextButton = table.find('.ds-pagination-btn-next');
    var nexterButton = table.find('.ds-pagination-btn-nexter');
    var backButton = table.find('.ds-pagination-btn-back');
    var backerButton = table.find('.ds-pagination-btn-backer');
    nextButton.removeClass('ds-hidden');
    nexterButton.removeClass('ds-hidden');
    backButton.removeClass('ds-hidden');
    backerButton.removeClass('ds-hidden');
    if (currentPage == 1) {
        backButton.addClass('ds-hidden');
        backerButton.addClass('ds-hidden');
    } else if (currentPage - 11 <= 1) {
        backerButton.addClass('ds-hidden');
    }
    if (currentPage == lastPage) {
        nextButton.addClass('ds-hidden');
        nexterButton.addClass('ds-hidden');
    } else if (currentPage + 11 >= lastPage) {
        nexterButton.addClass('ds-hidden');
    }

    var input = table.find('.ds-pagination-input-goto');
    input.attr('min', 1);
    input.attr('max', lastPage);
    input.val(currentPage);

    var lastPageContainer = table.find('.ds-pagination-last');
    lastPageContainer.text(' /' + lastPage)

    var previousPagesContainer = table.find('.ds-pagination-pages-previous')
    previousPagesContainer.html('');
    var currentPageContainer = table.find('.ds-pagination-pages-current')
    currentPageContainer.html('');
    var nextPagesContainer = table.find('.ds-pagination-pages-next')
    nextPagesContainer.html('');
    var renderPage = currentPage - 5;
    if (renderPage < 1) {
        renderPage = 1;
    }
    while (renderPage < currentPage) {
        var btn = DSGenerateTablePageButton(renderPage);
        previousPagesContainer.append(btn);
        renderPage++;
    }
    renderPage = currentPage + 1;
    var nextRenderCount = 0;
    while (renderPage <= lastPage && nextRenderCount < 5) {
        var btn = DSGenerateTablePageButton(renderPage);
        nextPagesContainer.append(btn);
        renderPage++;
        nextRenderCount++;
    }
    var btn = DSGenerateTablePageButton(currentPage);
    currentPageContainer.append(btn);
}

function DSGenerateTablePageButton(renderPage) {
    var btn = $('<button></button>');
    btn.addClass('ds-pagination-page-button');
    btn.on('click', () => {
        DSTablePageGoto(renderPage, btn);
    });
    btn.text(renderPage);
    return btn;
}
