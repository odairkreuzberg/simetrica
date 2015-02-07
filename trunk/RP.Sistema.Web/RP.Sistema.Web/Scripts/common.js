DEFAULT_PAGESIZE = 10;

var shared_function = [];

// adiciona class loading no documento e insere div que exibe barra
Functions.bodyLoading(true);

$(function () {

    Functions.detectIeVersion();

    Functions.handlers.modalReport();

    Functions.handlers.tooltip();

    Functions.handlers.autofocus();

    Functions.handlers.autoselect();

    Functions.handlers.placeholder();

    Functions.handlers.datepicker();

    Functions.handlers.spinner();

    Functions.handlers.mask();

    Functions.handlers.filter();

    Functions.handlers.counterTextarea();

    Functions.handlers.actionSubmitForm();

    Functions.handlers.actionResetForm();

    Functions.handlers.preventManySubmitForm();

    Functions.handlers.preventSubmitFormOnEnter();

    Functions.handlers.redirectPageSize('.datatable-pagesize');

    Functions.tabIndexNone();

    Functions.fixNavList();

    $().UItoTop({ text: 'Voltar para o topo', min: 100, inDelay: 200, outDelay: 200, scrollSpeed: 500 });

    Functions.handlers.opentab();

    // remove class loading do documento e remove barra (deixa sempre por ultimo)
    //setTimeout(function () { Functions.bodyLoading(false); }, 300);
    Functions.bodyLoading(false);

    $(".msg-aviso").click(function () {
        $(".msg-aviso").hide();
        return false;
    });
});

function ShowMessage(msg, tipo) {
    if (tipo == "erro") {
        $(".msg-aviso").removeClass('msg-sucesso').addClass('msg-erro');
    } else {
        $(".msg-aviso").removeClass('msg-erro').addClass('msg-sucesso');
    }
    $(".msg").text(msg);
    $(".msg-aviso").stop(true, true).fadeOut();
    $(".msg-aviso").show();
    $(".msg-aviso").fadeOut(8000);
}

function validarCampo(span, input, msg) {
    if (input.val() == "") {
        span.text(msg);
        span.addClass('field-validation-error');
        span.removeClass('field-validation-valid');
        input.addClass('input-validation-error');
    } else {
        span.text("");
        span.addClass('field-validation-valid');
        span.removeClass('field-validation-error');
        input.removeClass('input-validation-error');
    }
}
String.prototype.replaceAll = function (de, para) {
    var str = this;
    var pos = str.indexOf(de);
    while (pos > -1) {
        str = str.replace(de, para);
        pos = str.indexOf(de);
    }
    return (str);
}