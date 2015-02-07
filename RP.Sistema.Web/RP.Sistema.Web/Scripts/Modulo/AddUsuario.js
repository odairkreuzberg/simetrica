$(function () {
    var tbUsuario = $('#tblUsuario').tGrid({
        headerButtons: ['#btn-open-usuario'],
        pager: {
            enabled: true,
            size: 10
        },
        onCreate: function (grid) {
            var list = Functions.getJsonData('#tblUsuario_data');
            if (list) {
                for (var i = 0; i < list.length; i++) {
                    grid.addRow(Usuario.estrutura, { idUsuario: list[i].idUsuario, nmUsuario: list[i].nmUsuario });
                }
                grid.updateIndex();
                grid.paginate();
            }
        },
        onUpdate: function (grid) {
            $('#tblUsuario_count').val(grid.count());
        }
    });

    $('#dlg-usuario').dialog({
        width: 900
    })
    .on('shown', function () {
        $('#Usuario_idUsuario').focus();
    });

    $("#btn-add-usuario").click(function () {
        if (Usuario.validarCampos()) {
            if (!tbUsuario.hasUniqueValue($("#Usuario_idUsuario").val())) {
                tbUsuario.addRow(Usuario.estrutura, { idUsuario: $("#Usuario_idUsuario").val(), nmUsuario: $("#Usuario_nmUsuario").val() });
                tbUsuario.updateIndex();
                tbUsuario.paginate();
                Usuario.limparCampos();
            }
            else {
                alert('Este usuário já foi adicionado ao módulo');
            }
        }
        else {
            alert('Informe um usuário');
        }

        $("#Usuario_idUsuario").focus();
        return false;
    });
    
    $('#btn-open-usuario').click(function () {
        //$('#Usuario_idUsuario').focus();
    });
});

var Usuario = {
    validarCampos: function () {
        return (!IsNullOrEmpty($("#Usuario_idUsuario").val()) && !IsNullOrEmpty($("#Usuario_nmUsuario").val()));
    },

    limparCampos: function () {
        $("#Usuario_idUsuario").val('');
        $("#Usuario_nmUsuario").val('');
    },

    remover: function (row) {
        var tbUsuario = $('#tblUsuario').tGrid();
        tbUsuario.removeRow($(row));
        tbUsuario.updateIndex();
        tbUsuario.paginate();
    },

    estrutura:
        '<tr class="data">' +
            '<td class="actions b1">' +
                '<a href="#" class="tgrid-delete btn btn-danger" title="Excluir" onclick="Usuario.remover(this); return false"><i class="icon-trash icon-white"></i></a>' +
                '<input type="hidden" name="Usuarios[0].idUsuario" unique="true" value="${idUsuario}"/>' +
                '<input type="hidden" name="Usuarios[0].nmUsuario" value="${nmUsuario}"/>' +
            '</td>' +
            '<td>${idUsuario}</td>' +
            '<td>${nmUsuario}</td>' +
        '</tr>'
}