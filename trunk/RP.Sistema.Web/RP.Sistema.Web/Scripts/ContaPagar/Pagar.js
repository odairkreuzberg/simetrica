$(function () {
    $('#valorPago').focus();
    Conta.validarConta();

    $('#valorPago').on({
        change: function () {
            Conta.validarConta();
        }
    });
});

var Conta = {

    validarConta: function () {
        var valorPago = $('#valorPago').val().replaceAll(".", "").replace(",", ".");
        var valorConta = $('#valorConta').val().replaceAll(".", "").replace(",", ".");
        if (parseFloat(valorConta) > parseFloat(valorPago)) {

            var diferenca = parseFloat(valorConta) - parseFloat(valorPago);
            $('#vlDiferenca').val(diferenca.toString().replace(".", ","));
            $('#diferenca').show();
        } else {
            $('#flDiferenca').val('Não');
            $('#vlDiferenca').val('');
            $('#diferenca').hide();
        }
    }
};
