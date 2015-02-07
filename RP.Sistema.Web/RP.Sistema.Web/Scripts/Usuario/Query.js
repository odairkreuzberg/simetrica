$(function () {
    $('#grid').tGrid({
        selectable: true,
        select: function (data) {
            Functions.handlers.selectModalItem(data);
            window.parent.$(window.parent.document).trigger('AfterSelect_' + $('#context-prefix').val(), { idUsuario: data.idUsuario });
        }
    });
});
