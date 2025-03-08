$(document).ready(() => {
    var selects = $('.ds-select');
    for (var i = 0; i < selects.length; i++) {
        var select = $(selects[i]);
        select.find('input').on('input', () => {
            DSGetSelectData(select);
        })
    }
})

function DSGetSelectData(select) {
    var name = select.attr('name');
    var dataUrl = select.data('ds-data-url');
    var filter = select.find('input').val();
    var routeValues = select.data('ds-route-values');

    var data = new FormData();
    data.append('selectName', name);
    data.append('filter', filter);

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
            var select = $('#' + data.selectName);
            var dropDown = select.find('.ds-select-drop-down');
            dropDown.html('');
            for (var i = 0; i < data.options.length; i++) {
                var option = data.options[i];
                var optionEl = $('<div value="'+option.value+'">'+option.text+'</div>')
                dropDown.append(optionEl);
            }
        },
        error: function (xhr, desc, err) {
            console.error(desc, err);
        }
    });
}