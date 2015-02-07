$(function () {
    $('#filter').focus();
    $('[rel=popover]').popover({ html: true });
    $('.btn-create').click(function (e) {
        var url = e.currentTarget.href;
        var ano = $('#Consulta_ano').val();
        var mes = $('#Consulta_mes').val();
        url = url.replace("ano=0", "ano=" + ano);
        url = url.replace("mes=0", "mes=" + mes);
        e.currentTarget.href = url;
    });
    $('#btn-report-mes').click(function (e) {
        var url = e.currentTarget.href;
        var ano = $('#Consulta_ano').val();
        var mes = $('#Consulta_mes').val();
        url = url.replace("ano=0", "ano=" + ano);
        url = url.replace("mes=0", "mes=" + mes);
        e.currentTarget.href = url;
    });
    $('#btn-report').click(function (e) {
        var url = e.currentTarget.href;
        var ano = $('#Consulta_ano').val();
        var mes = $('#Consulta_mes').val();
        url = url.replace("ano=0", "ano=" + ano);
        url = url.replace("mes=0", "mes=" + mes);
        e.currentTarget.href = url;
    });
});