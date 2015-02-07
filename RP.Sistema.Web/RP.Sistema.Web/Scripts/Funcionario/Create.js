$(function () {
    $('#nome').focus();

    if ($('#flMensalista').val() == 'Sim') {
        $('#fl-comissao').hide();

    } else {
        $('#fl-comissao').show();
        $('#comissao').val('')
    }

    $('#flMensalista').change(function () {
        if ($('#flMensalista').val() == 'Sim') {
            $('#fl-comissao').hide();

        } else {
            $('#fl-comissao').show();
            $('#comissao').val('')
        }
    });
});