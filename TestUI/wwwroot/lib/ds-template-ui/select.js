$(document).ready(() => {
    var selects = $('.ds-select');
    for (var i = 0; i < selects.length; i++) {
        var select = $(selects[i]);
        select.find('.ds-select-filter').on('input', (e) => {
            DSGetSelectData($(e.currentTarget).parent());
        })
        select.find('.ds-select-filter').on('blur', (e) => {
            var select = $(e.currentTarget).parent();
            var dropDown = select.find('.ds-select-drop-down');
            var option = dropDown.children('[selected=selected]');
            dropDown.collapse('hide');
            var selectInput = dropDown.parent().find('.ds-select-input');
            selectInput.val(option.attr('value'));
            select.find('.ds-select-filter').val(option.text());
        })
        select.find('.ds-select-filter').on('keydown', (e) => {
            if (e.code == "ArrowDown" || e.code == "ArrowUp") {
                var select = $(e.currentTarget).parent();
                var dropDown = select.find('.ds-select-drop-down');
                var option = dropDown.children('[selected=selected]');
                var newOption = undefined;
                if (option.length > 0) {
                    if (e.code == "ArrowDown") {
                        var newOption = option.next();
                    } else if (e.code == "ArrowUp") {
                        var newOption = option.prev();
                    }
                }
                if (newOption != undefined && newOption.length != 0) {
                    option.removeAttr('selected');
                    newOption.attr('selected', 1);
                }
            }
        })
    }
})

function DSGetSelectData(select) {
    var name = select.data('ds-name');
    var dataUrl = select.data('ds-data-url');
    var filter = select.find('.ds-select-filter').val();
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
                var optionEl = $('<div class="ds-select-option" value="' + option.value + '">' + option.text + '</div>');
                if (i == 0) {
                    optionEl.attr('selected', 1);
                }
                optionEl.on('mouseover', (e) => {
                    var option = $(e.currentTarget);
                    dropDown = option.parent();
                    dropDown.find('div').removeAttr('selected');
                    option.attr('selected', 1);
                })
                dropDown.append(optionEl);
            }
            dropDown.collapse('show');
        },
        error: function (xhr, desc, err) {
            console.error(desc, err);
        }
    });
}