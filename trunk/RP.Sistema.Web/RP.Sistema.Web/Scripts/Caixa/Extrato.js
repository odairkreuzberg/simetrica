$(function () {
    $('#tblItem').tGrid({
        addMode: "append",
    });
    $('#btnLocalizar').click(function () {
        Caixa.localizar();
        return false;
    });

});

var Caixa = {
    localizar: function () {

        var tbl = $('#tblItem').tGrid();
        tbl.removeAllRows();


        $.ajax({
            dataType: 'json',
            type: 'GET',
            url: $('#getExtrato').val(),
            data: { dia: $('#dia').val() },

            success: function (data) {
                if (data && data.length > 0) {
                    for (var i = 0; i < data.length; i++) {
                        var item = data[i];
                        tbl.addRow(Caixa.estrutura, {
                            dtlancamento: item.dtlancamento,
                            tipo: item.tipo,
                            descricao: item.descricao,
                            saldo: window.NumberFormat(parseFloat(item.saldo), 2, ",", "."),
                            valorpago: window.NumberFormat(parseFloat(item.valorpago), 2, ",", ".")
                        });
                    }
                }
            },

            error: function (request) {
                Functions.checkRequest(request);
            }
        });
    },

    estrutura:
'<tr class="data">' +
    '<td>${dtlancamento}</td>' +
    '<td>${tipo}</td>' +
    '<td>${descricao}</td>' +
    '<td style="text-align: right">{{if parseFloat(valorpago) >= 0}} <strong style="color:green">${valorpago}</strong>{{else}}<strong style="color:red">${valorpago}</strong> {{/if}}</td>' +
    '<td style="text-align: right">{{if parseFloat(saldo) >= 0}} <strong style="color:green">${saldo}</strong>{{else}}<strong style="color:red">${saldo}</strong> {{/if}}</td>' +
 '</tr>'
};