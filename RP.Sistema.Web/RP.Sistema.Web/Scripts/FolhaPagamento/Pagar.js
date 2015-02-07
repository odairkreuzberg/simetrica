$(function () {

    $('form').submit(function (e) {
        var list = tbl.getRows();
        for (var i = 0; i < list.length; i++) {
            var data = tbl.getRowData($(list[i]));
            data.comissao = data.comissao.replaceAll(".", "");
            data.vale = data.vale.replaceAll(".", "");
            tbl.updateRow($(list[i]), Movimento.estrutura, data);
        }
        tbl.updateIndex();
        tbl.paginate();
        $('#horaExtra').val($('#horaExtra').val().replaceAll(".", ""));
        $('#bonificacao').val($('#bonificacao').val().replaceAll(".", ""));
        $('#outrosDescontos').val($('#outrosDescontos').val().replaceAll(".", ""));
        $('#inss').val($('#inss').val().replaceAll(".", ""));
        $('#totalVencimento').val($('#totalVencimento').val().replaceAll(".", ""));
        $('#totalDesconto').val($('#totalDesconto').val().replaceAll(".", ""));
        $('#totalReceber').val($('#totalReceber').val().replaceAll(".", ""));

        if ($('#salario').val()) {
            $('#salario').val($('#salario').val().replaceAll(".", ""));
        }
    });

    var tbl = $('#tblMovimento').tGrid({
        addMode: "prepend",
        onCreate: function (grid) {
            var data = window.Functions.getJsonData('#tblMovimento_data');
            if (data.length) {
                grid.progressbar();
                var work = new window.ThreadLoop(data);
                work.action = function (item, index, total) {
                    grid.addRow(Movimento.estrutura, Movimento.formpatData(item));
                    Functions.progressbar.update(grid.progressbar(), index, total);
                };
                work.finish = function (thread) {
                    grid.updateIndex();
                    grid.paginate();
                    grid.progressbar().dispose();
                    window.Functions.handlers.mask();
                    Movimento.addValorTotal(grid);
                };
                work.start();
            }

        },
        onUpdate: function (grid) {
            $('#tblMovimento_count').val(grid.count());
        }
    });

    $('#tblMovimento').on({
        click: function ()
        { Movimento.addProximo(this); return false; }
    }, 'a[data-event=add-proximo]');

    $('.data-movimento').on({
        change: function () {
            Movimento.addValorTotal(tbl);
            return false;
        }
    });
    Movimento.addValorTotal($('#tblMovimento').tGrid());
});

var Movimento = {

    addValorTotal: function (tbl) {
        var list = tbl.getRows();
        var totalVencimento = 0;
        var totalDesconto = 0;
        for (var i = 0; i < list.length; i++) {
            var data = tbl.getRowData($(list[i]));
            var comissao = data.tipo == "Vale" ? 0 : data.comissao.replaceAll(".", "").replace(",", ".");
            var vale = data.tipo != "Vale" ? 0 : data.vale.replaceAll(".", "").replace(",", ".");

            totalVencimento = parseFloat(totalVencimento) + parseFloat(comissao);
            totalDesconto = parseFloat(totalDesconto) + parseFloat(vale);
        }
        //vencimentos
        
        if ($('#salario').val()) {
            var salario = $('#salario').val().replaceAll(".", "").replace(",", ".");
            $('#salario').val(window.NumberFormat(salario, 2, ',', '.'));
            totalVencimento = parseFloat(totalVencimento) + parseFloat(salario == "" ? 0 : salario);
        }

        var horaExtra = $('#horaExtra').val().replaceAll(".", "").replace(",", ".");
        $('#horaExtra').val(window.NumberFormat(horaExtra, 2, ',', '.'));

        var bonificacao = $('#bonificacao').val().replaceAll(".", "").replace(",", ".");
        $('#bonificacao').val(window.NumberFormat(bonificacao, 2, ',', '.'));

        totalVencimento = parseFloat(totalVencimento) + parseFloat(bonificacao == "" ? 0 : bonificacao);
        totalVencimento = parseFloat(totalVencimento) + parseFloat(horaExtra == "" ? 0 : horaExtra);

        //descontos
        var outrosDescontos = $('#outrosDescontos').val().replaceAll(".", "").replace(",", ".");
        $('#outrosDescontos').val(window.NumberFormat(outrosDescontos, 2, ',', '.'));

        var inss = $('#inss').val().replaceAll(".", "").replace(",", ".");
        $('#inss').val(window.NumberFormat(inss, 2, ',', '.'));

        totalDesconto = parseFloat(totalDesconto) + parseFloat(inss == "" ? 0 : inss);
        totalDesconto = parseFloat(totalDesconto) + parseFloat(outrosDescontos == "" ? 0 : outrosDescontos);

        var totalReceber = totalVencimento - totalDesconto;

        $('#totalVencimento').val(window.NumberFormat(totalVencimento, 2, ',', '.'));
        $('#totalDesconto').val(window.NumberFormat(totalDesconto, 2, ',', '.'));
        $('#totalReceber').val(window.NumberFormat(totalReceber, 2, ',', '.'));
    },

    addProximo: function (el) {
        var tblMovimento = $('#tblMovimento').tGrid();

        var row = $(el).parents('tr.data');
        var data = tblMovimento.getRowData(row);

        tblProximo.addRow(Proximo.estrutura, data);
        tblProximo.updateIndex();
        tblProximo.paginate();

        tblMovimento.removeRow($(el));
        tblMovimento.updateIndex();
        tblMovimento.paginate();
        $('#tabContainer a[href="#tab-proximo"]').tab('show');
        ShowMessage("Comissão adicionada para o próximo mês com sucesso!", "sucesso");
    },
    formpatData: function (item) {
        var data = {
            vale: item.tipo == "Vale" ? window.NumberFormat(parseFloat(item.vale), 2, ',', '.') : "",
            comissao: item.tipo != "Vale" ? window.NumberFormat(parseFloat(item.comissao), 2, ',', '.') : "",
            descricao: item.descricao,
            tipo: item.tipo,
            idMovimento: item.idMovimento
        }
        return data;
    },

    estrutura:
        '<tr class="data">' +
            '<td >' +
                '<input type="hidden" name="Movimentos[0].idMovimento" value="${idMovimento}"/>' +
                '<input type="hidden" name="Movimentos[0].tipo" value="${tipo}"/>' +
                '<input type="hidden" name="Movimentos[0].descricao" value="${descricao}"/>' +
                '${descricao}</td>' +
            '<td>${tipo}</td>' +
            '<td><input type="text" style="text-align: right" value="${comissao}" readonly="readonly" name="Movimentos[0].comissao" class="span12" tabindex="-1"></td>' +
            '<td><input type="text" style="text-align: right" value="${vale}" readonly="readonly" name="Movimentos[0].vale" class="span12" tabindex="-1"></td>' +
         '</tr>'
};

