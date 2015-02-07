$(function () {
    $('#listaIcone').autocomplete({
        source: eval($('#lista-icones').val()),
        minLength: 0,
        focus: function (event, ui) {
            $('#listaIcone').val(ui.item.label);
            $("#dsIcone").val('atalho-' + ui.item.label);
            return false;
        },
        select: function (event, ui) {
            $("#preview-atalho").removeAttr('class').addClass('atalho-' + ui.item.label);
            return false;
        },
        change: function (event, ui) {
            if (!ui.item || !ui.item.label) {
                $("#preview-atalho").removeAttr('class');
                $("#dsIcone").val('');
                $('#listaIcone').val('');
            }
        }
    })
    .data("autocomplete")._renderItem = function (ul, item) {
        return $("<li></li>")
            .data("item.autocomplete", item)
            .append("<a class='clearfix'><span class='atalho-" + item.label + "'></span><span style='margin-top:10px; display:inline-block'>" + item.label + "</span></a>")
            .appendTo(ul);
    };
});