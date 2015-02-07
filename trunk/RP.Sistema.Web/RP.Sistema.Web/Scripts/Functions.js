var Functions = new function () {
    // exibe overlay com mensagem de carregando
    this.bodyLoading = function (show) {
        if (show) {
            $(document.documentElement).addClass('loading');
            $('body').prepend('<div id="loading"/>');
        }
        else {
            $(document.documentElement).removeClass("loading");
        }
    };

    // retorna o valor atributo informado como objeto json
    this.getValueAttribute = function (el, attributeName) {
        var node = el.attr(attributeName);

        if (typeof node == 'string') {
            return eval('(' + node + ')');
        }
        else if (typeof node == 'object') {
            if (node.getNamedItem(attributeName)) {
                return eval('(' + node.getNamedItem(attributeName)['value'] + ')');
            }
        }
    };

    this.openTab = function (id, title, url) {
        $('#content-tab').append($($.trim(tmpl("tmpl-content-tab", { id: id, src: url }))));
        Painel.tabs.append($($.trim(tmpl("tmpl-header-tab", { text: title, id: id }))));
        Painel.tabs.find('li.active').removeClass('active');
        Painel.tabs.find('a:last').tab('show');
    };

    // contem os handlers de elementos de entrada (a, input, textarea, select...)
    this.handlers = new function () {

        var ModalReport = function (el) {
            $this = el;

            var $template = "",
                $originForm = ($this[0] && $this[0].tagName.toLowerCase() == 'form') ? $this : null,
                $form = null,
                params = "",
                modalId = "",
                href = $this.attr('href'),
                formData = {};

            // define o valor 'modal-id' se ainda estiver definido
            if (!$this.data('modal-id')) {
                $this.data('modal-id', new Date().getTime());
            }

            // obtem o valor 'modal-id'
            modalId = $this.data('modal-id');

            //target = "_blank"
            //'<a href="{1}" target="_blank" class="btn btn-primary" rel="tooltip" data-placement="bottom" data-container="#{0}" title="Abre o relatório para visualização"><i class="icon-eye-open icon-white"></i> Abrir</a> ' +
            //(href.indexOf('?') > 0 ? (href + location.search.replace('?', '&')) : href + location.search)

            // se ainda nao existir uma estrutura no DOM para a janela
            if (!$this.data('has-modal')) {

                // define o template
                $template =
                    '<div id="{0}" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" data-keyboard="false" style="width:590px; margin-left:-295px">' +
                        '<div class="modal-header">' +
                            '<button type="button" title="Fechar" class="close" data-dismiss="modal" aria-hidden="true">×</button>' +
                            '<h3 id="myModalLabel">Opções do relatório</h3>' +
                        '</div>' +
                        '<div class="modal-body">' +
                            '<form action="{1}" class="nomargin" target="_blank">{2}' +
                                '<div class="alert hide"></div>' +
                                '<div class="control-group">' +
                                    '<textarea class="input-block-level" name="Descricao" placeholder="Insira uma descrição para o relatório... (apenas para salvar ou agendar)"></textarea>' +
                                '</div>' +
                                '<div style="margin-bottom:5px; height:12px;" class="progress progress-striped active hide">' +
                                    '<div class="bar" style="width: 100%;"></div>' +
                                '</div>' +
                                '<button type="submit" name="Acao" value="1" class="btn btn-primary" rel="tooltip" data-placement="bottom" data-container="#{0}" title="Abre o relatório para visualização"><i class="icon-eye-open icon-white"></i> Abrir</button> ' +
                                '<button type="submit" name="Acao" value="2" class="btn btn-success" rel="tooltip" data-placement="bottom" data-container="#{0}" title="Salva o relatório no repositório"><i class="icon-hdd icon-white"></i> Salvar</button> ' +
                                '<button type="submit" name="Acao" value="9" class="btn btn-inverse" rel="tooltip" data-placement="bottom" data-container="#{0}" title="Abre e salva relatório no repositório""><i class="icon-eye-open icon-white"></i> Abrir e Salvar <i class="icon-hdd icon-white"></i></button> ' +
                                '<div class="control-group input-append pull-right nomargin">' +
                                    '<input type="text" name="Data" class="input-medium input-date" placeholder="dd/mm/aaaa hh:mm" mask="99/99/9999 99:99" title="Data e hora para o agendamento" style="width:125px" />' +
                                    '<button type="submit" name="Acao" value="3" class="btn" rel="tooltip" data-placement="bottom" data-container="#{0}" title="Agenda a geração do relatório">Agendar <i class="icon-calendar"></i></button>' +
                                '</div>' +
                            '</form>' +
                        '</div>' +
                    '</div>';

                // percorre todos elementos da querystring e adiciona no formulario como campo hidden
                for (var i in $.getQueryString()) {
                    params += $.format('<input type="hidden" name="{0}" value="{1}"/>', i, $.getQueryString(i));
                };

                // cria objeto jquery com template
                $template = $($.format($template, modalId, href, params));

                // obtem o formulario
                $form = $template.find('form');

                // busca os botoes submit dentro do form e ouve o evento click
                $form.find('[type=submit]').click(function (e) {
                    // seta o valor da acao no formulario
                    $form.data('acao', e.target.value);
                });

                // ouve o evento submit do formulario
                $form.submit(function (e) {
                    var acao = $(this).data('acao');
                    var sendForm = true;
                    var $alert = $form.find('.alert');

                    $alert.addClass('hide');
                    $form.find('.control-group').removeClass('error');

                    // se a acao for "Abrir"
                    if (acao == 1) {
                        // se houver o form de origem
                        if ($originForm) {
                            // previne o envio do form da modal
                            e.preventDefault();

                            // submete o form de origem
                            $originForm.data('forcesubmit', true).submit().data('forcesubmit', false);
                        }
                    }
                    // se a acao for "Salvar" ou "Abrir e Salvar"
                    else if ((acao == 2 || acao == 9) && Functions.isNullOrEmpty($form.find('textarea').val())) {
                        // previne o envio do form da modal
                        e.preventDefault();

                        sendForm = false;
                        $form.find('textarea').focus().parent('div').addClass('error');
                        $alert.html('<strong>Atenção...</strong> insira uma descrição para o relatório!').addClass('alert-error').removeClass('hide');
                    }
                    // se a acao for "Agendar"
                    else if (acao == 3) {
                        if (Functions.isNullOrEmpty($form.find('textarea').val())) {
                            // previne o envio do form da modal
                            e.preventDefault();

                            sendForm = false;
                            $form.find('textarea').focus().parent('div').addClass('error');
                            $alert.html('<strong>Atenção...</strong> insira uma descrição para o agendamento!').addClass('alert-error').removeClass('hide');
                        }
                        else if (Functions.isNullOrEmpty($form.find('.input-date').val())) {
                            // previne o envio do form da modal
                            e.preventDefault();

                            sendForm = false;
                            $form.find('.input-date').focus().parent('div').addClass('error');
                            $alert.html('<strong>Atenção...</strong> informe uma data para o agendamento!').addClass('alert-error').removeClass('hide');
                        }
                    }

                    // se o formulario da modal for valido e a Acao for "Salvar", "Agendar" ou "Abrir e Salvar"
                    if (sendForm && (acao == 2 || acao == 3 || acao == 9)) {

                        // se a Acao for "Salvar" ou "Agendar"
                        if (acao == 2 || acao == 3) {
                            // previne o envio do form da modal
                            e.preventDefault();
                        }

                        // serializa formulario do modal
                        formData = $form.serializeArray();

                        // adiciona a Acao selecionada nos dados serializados
                        formData.push({ name: 'Acao', value: (acao == 9 ? 2 : acao) });

                        // busca os elementos no DOM
                        var $buttons = $form.find('[type=submit]');
                        var $progress = $form.find('.progress');

                        // desabilita botoes e remove mensagem
                        $buttons.addClass('disabled').attr('disabled');
                        $progress.removeClass("hide");

                        // faz requisicao ajax
                        $.ajax({
                            type: $originForm ? $originForm.attr('method').toUpperCase() : 'GET',
                            dataType: 'json',
                            url: href,
                            data: $originForm ? formData.concat($originForm.serializeArray()) : formData,
                            success: function (data) {
                                $buttons.removeClass('disabled').removeAttr('disabled');
                                $progress.addClass("hide");
                                $alert.text(data.Message).removeClass('alert-success alert-error').addClass(data.Success ? 'alert-success' : 'alert-error').removeClass('hide');
                            }
                        });

                        // se houver o form de origem e a Acao for "Abrir e Salvar"
                        if ($originForm && acao == 9) {
                            // previne o envio do form da modal
                            e.preventDefault();

                            // submete o formulario original
                            $originForm.data('forcesubmit', true).submit().data('forcesubmit', false);
                        }
                    }
                });

                // adiciona template ao body
                $('body').append($template);

                // renderiza tooltip
                Functions.handlers.tooltip();

                // renderiza mascara
                Functions.handlers.mask();
                Functions.handlers.filter();

                // seta que ja existe estrutura da janela
                $this.data('has-modal', true);
            }

            // exibe modal
            $('#' + modalId).modal('show');
        };

        // todos elementos com o atributo [data-modal-report=true] ao ser clicado abrirao uma janela com opcoes para relatorio
        this.modalReport = function () {
            $('body').on({
                click: function (e) {
                    e.preventDefault();
                    ModalReport($(this));
                }
            }, 'a[data-modal-report=true]');

            $('body').on({
                submit: function (e) {
                    var $this = $(this);

                    if (!$this.data('forcesubmit')) {
                        e.preventDefault();
                    }
                    if ($.validator) {
                        if ($this.valid()) {
                            ModalReport($this);
                        }
                    }
                }
            }, 'form[data-modal-report=true]');
        };

        // exibe tooltip para todos elementos com o atributo [rel=tooltip]
        this.tooltip = function () {
            $('[rel=tooltip]').tooltip();
        };

        // fix para placeholder em navegadores sem suporte
        this.placeholder = function () {
            if (!Modernizr.input.placeholder) {
                Placeholders.init();
            }
        };

        // fix para autofocus para navegadores sem suporte
        this.autofocus = function () {
            if (!Modernizr.input.autofocus) {
                // seta o foco no primeiro elemento com o atributo [autofocus]
                $('[autofocus]')[0].setFocus();
            }
        };

        this.autoselect = function () {
            //$('[data-select-onfocus]').setFocus({
            //    callback: function () {
            //        $(this).select();
            //    }
            //});
        };

        // todos elementos com a classe "datepicker" exibirao um calendario
        this.datepicker = function () {
            $('.datepicker').each(function () {
                var _attrs = {
                    changeMonth: true,
                    changeYear: true,
                    showOtherMonths: true,
                    selectOtherMonths: true,
                    showButtonPanel: true,
                    yearRange: 'c-15:c+15'
                };

                var attrs = Functions.getValueAttribute($(this), 'datepicker');
                attrs = $.extend(_attrs, attrs);

                $(this).attr('autocomplete', 'off').datepicker(attrs);
            });
        };

        // todos elementos com a classe "spinner" ou com o atributo [spinner] exibirao a funcionalidade
        this.spinner = function () {
            $('.spinner, [spinner]').each(function () {
                var _attrs = {
                    culture: 'pt-BR'
                };

                var attrs = Functions.getValueAttribute($(this), 'spinner');
                attrs = $.extend(_attrs, attrs);

                $(this).spinner(attrs)
                    .addClass('ui-spinner-box')
                .parent()
                .find(".ui-spinner-button")
                .replaceWith(function () {
                    return $("<button>", {
                        type: 'button',
                        'class': this.className,
                        tabindex: -1
                    });
                });

                if (attrs) {
                    if (attrs.start) {
                        $(this).spinner("value", attrs.start);
                    }
                }
            });
        };

        // exibe funcionalidade de contador de caracteres a todos elementos textarea com o atributo [maxlength]
        this.counterTextarea = function () {
            $('textarea[maxlength]').each(function () {
                var defaults = {};
                defaults.goal = $(this).attr('maxlength');
                defaults.text = ($(this).attr('data-textarea-label') || true).toString().toLowerCase() === 'true';
                defaults.type = $(this).attr('data-textarea-type');
                defaults = $.extend({ type: 'char' }, defaults);

                $(this).counter(defaults);
            });
        };

        // adiciona a funcionalidade de mascara a todos inputs com atributo [mask]
        this.mask = function () {
            $('input[mask]').each(function () {
                $(this).mask($(this).attr('mask'));
            });
        };

        // remove a funcionalidade mascara para o elemento informado
        this.unmask = function ($input) {
            if ($input instanceof jQuery) {
                $input.val($input.val().replace(/[\s\-\.\/\(\)]/g, ''));
            }
            else {
                return $input.replace(/[\s\-\.\/\(\)]/g, '');
            }
        };

        // adiciona a funcionalidade filter para todos elementos com o atributo [filter]
        // opcoes de filter: 'alphanumeric', 'numeric', 'alpha', 'floatnumber'
        this.filter = function () {
            $('input[filter]').each(function () {
                var filters = $(this).attr('filter').toString().split('|');
                var re = new RegExp(/\((.+?|s?)\)/g);

                for (var i = 0; i < filters.length; i++) {
                    var filter = $.trim(filters[i]).replace(re, '');
                    var param = $.trim(filters[i]).replace(filter, '').replace(/\(\)/, '');
                    if (param) {
                        param = eval(param);
                    }

                    switch (filter) {
                        case 'alphanumeric':
                            $(this).alphanumeric(param);
                            break;
                        case 'numeric':
                            $(this).numeric(param);
                            break;
                        case 'alpha':
                            $(this).alpha(param);
                            break;
                        case 'floatnumber':
                            $(this).numeric({ allow: (param.separator ? param.separator : ',') });
                            $(this).floatnumber(param);
                            break;
                    }
                }
            });
        };

        // captura todos os dados da tela query e envia para a janela pai
        this.selectModalItem = function (data) {
            $('[prefix="' + $('#context-prefix').val() + '"][from]', parent.window.document).each(function () {
                $(this).val(data[$(this).attr('from')]);
            });
            window.parent.$(window.parent.document).trigger($('#context-prefix').val() + '_modal-close');
        };

        // ao clicar no elemento com o atributo [data-form-type=submit], submete o formulario pai
        this.actionSubmitForm = function () {
            $('[data-form-type=submit]').click(function (e) {
                e.preventDefault();

                if ($(this).attr('data-form-element')) {
                    $($(this).attr('data-form-element')).submit();
                }
                else {
                    $(this).parents('form').submit();
                }
            });
        };

        // ao clicar no elemento com o atributo [data-form-type=submit], reseta o formulario pai
        this.actionResetForm = function () {
            $('[data-form-type=reset]').click(function (e) {
                e.preventDefault();
                $(this).parents('form')[0].reset();
            });
        };

        // previne que o formulario seja submetido mais de uma vez
        this.preventManySubmitForm = function () {
            $('form').submit(function (e) {
                $(this).submit(function () {
                    e.preventDefault();
                });
            });
        };

        // previne que o formulario seja submetido ao pressionar a tecla ENTER
        this.preventSubmitFormOnEnter = function () {
            // busca todos elementos que nao tenham a classe "enter" ou o id filter
            $('input:not(.enter, #filter), select:not(.enter)').keypress(function (e) {
                if (e.which == 13) {
                    e.preventDefault();
                }
            });
        };

        // captura todos elementos que contenham o seletor informado e faz redirecionamento
        this.redirectPageSize = function (selector) {
            $(selector ? selector : '.datatable-pagesize').bind('change', function () {
                Functions.handlers.redirectPageSize($(this).val());
            });
        };

        // captura links que contenham a classe open-tab/open-tab-sistema e abre uma nova aba no sistema
        this.opentab = function () {
            $('body')
                .on({
                    click: function (e) {
                        e.preventDefault();
                        var self = $(this);
                        Functions.openTab(new Date().getTime(), $.trim(self.attr('title') ? self.attr('title') : self.text()), self.prop('href'));
                    }
                }, 'a.open-tab')
                .on({
                    click: function (e) {
                        var self = $(this);
                        var tmpl_header_tab = window.parent.$("#tmpl-header-tab").html();
                        var tmpl_content_tab = window.parent.$("#tmpl-content-tab").html();

                        if (IsNullOrEmpty(tmpl_header_tab) || IsNullOrEmpty(tmpl_content_tab)) {
                            self.attr('target', '_blank');
                        }
                        else {
                            e.preventDefault();
                            var id_tab = new Date().getTime();
                            var header_tab = window.parent.$('#header-tab');
                            var content_tab = window.parent.$('#content-tab');

                            header_tab.find('li.active').removeClass('active');
                            $($.trim(tmpl(tmpl_header_tab, { text: $.trim(self.attr('title') ? self.attr('title') : self.text()), id: id_tab }))).appendTo(header_tab);

                            content_tab.find('div.active').removeClass('active');
                            $($.trim(tmpl(tmpl_content_tab, { id: id_tab, src: self.prop('href') }))).appendTo(content_tab);
                        }
                    }
                }, 'a.open-tab-sistema');
        };
    };

    // detecta a versao do internet explorer e adiciona a versao no elemento html
    this.detectIeVersion = function () {
        if (BrowserDetect.browser == 'Explorer') {
            $("html").addClass("ie").addClass("ie" + parseInt(BrowserDetect.version));
        }
    };

    // Privado - Corrige o tamanho dos elementos com a classe .container-nav-list
    var _fixNavList = function () {
        var container = $('.container-nav-list.affix');
        container.width(container.parent('.span2').width()
            - parseInt(container.css('border-left-width')) - parseInt(container.css('padding-left'))
            - parseInt(container.css('border-right-width')) - parseInt(container.css('padding-right'))
        );
    };

    // Corrige o tamanho dos elementos com a classe .container-nav-list
    this.fixNavList = function () {
        _fixNavList();

        $(window).bind('resize', function () {
            _fixNavList();
        });
    };

    // remove tabindex dos campos readonly
    this.tabIndexNone = function () {
        $('input[readonly]').attr('tabindex', '-1');
    };

    // redireciona para com requisicao de paginacao
    this.redirectPageSize = function () {
        var url = '';
        if (window.location.href.match(/pagesize=(\d*)/g)) {
            url = window.location.href
                .replace(/pagesize=(\d*)/g, 'pagesize=' + value)
                .replace(/page=(\d*)/g, 'page=1');
        }
        else {
            url = window.location.href + '&pagesize=' + value;
        }
        window.location.href = url;
    };

    // retorna true se o valor for um numero
    this.isNumber = function (value) {
        return !isNaN(parseFloat(value)) && isFinite(value);
    };

    // retorna true se o valor for nulo ou vazio
    this.isNullOrEmpty = function (value) {
        return (!value || $.trim(value) == '' || value == null);
    };

    // executa acao de acordo com a requisicao informada
    this.checkRequest = function (request) {
        if (request.status == 401) {
            window.location.href = $('#_url_auth_login').val();
        }
        else {
            if (request.status) {
                alert(request.responseText);
            }
        }
    };

    // retorna o valor JSON do elemento informado
    this.getJsonData = function (element, remove) {
        var result = [];
        remove = remove == undefined ? true : remove;

        if ($(element).get(0)) {
            result = eval($(element).val());
            if (remove) {
                $(element).remove();
            }
        }

        return result;
    };

    // captura texto selecionado no DOM
    this.getSelectedText = function () {
        //TODO: verificar a possibilidade de buscar o texto selecionado de um elemento especifico (DIV)
        var text = "";
        if (window.getSelection) {
            text = window.getSelection().toString();
        } else if (document.selection && document.selection.type != "Control") {
            text = document.selection.createRange().text;
        }
        return text;
    };

    // encode html
    this.htmlEncode = function (value) {
        return $('<div/>').text(value).html();
    };

    // decode html
    this.htmlDecode = function (value) {
        return $('<div/>').html(value).text();
    };

    // flashmessage
    this.flashMessage = new function () {
        // adiciona mensagem
        this.add = function (element, message) {
            var _alert;
            if ($.data(element[0], 'modal_message')) {
                _alert = $.data(element[0], 'modal_message');
            }
            else {
                _alert = $('<div class="alert" style="margin:10px"/>');
            }
            _alert.text(message);
            $.data(element[0], 'modal_message', _alert);
            _alert.insertAfter(element);
        };

        // remove mensagem
        this.remove = function (element) {
            var _alert = $.data(element[0], 'modal_message');
            if (_alert) {
                _alert.remove();
            }
        };
    };

    // adiciona input hidden em formulario informado
    this.addHiddenInput = function (form, id, attributes) {
        var input, _exists = false;
        var _input = $('#' + id, form);

        if (_input.length) {
            input = _input;
            _exists = true;
        }
        else {
            input = $('<input type="hidden" id="' + id + '"/>');
        }

        $.each(attributes, function (att) {
            if (att != 'type' && att != 'id') {
                input.attr(att, attributes[att]);
            }
        });

        if (!_exists) {
            $(form).prepend(input);
        }

        return input;
    };

    // progressbar
    this.progressbar = new function () {
        // atuliza progressbar
        this.update = function (progress, index, total) {
            var perc = (5 * total) / 100;
            if (Math.floor((index + 1) % perc) == 0 || Math.floor((index + 1) % perc) == .5) {
                progress.update(((index + 1) / total) * 100);
            }
        }
    };
}

var IsNullOrEmpty = function (value) {
    return Functions.isNullOrEmpty(value);
};

/**************************************************************************
* http://blog.shiguenori.com/2009/03/21/number_format-em-javascript/
***************************************************************************/
var NumberFormat = function (number, decimals, dec_point, thousands_sep) {
    // %   nota 1: Para 1000.55 retorna com precisão 1 no FF/Opera é 1,000.5, mas no IE é 1,000.6
    // *     exemplo 1: number_format(1234.56);
    // *     retorno 1: '1,235'
    // *     exemplo 2: number_format(1234.56, 2, ',', ' ');
    // *     retorno 2: '1 234,56'
    // *     exemplo 3: number_format(1234.5678, 2, '.', '');
    // *     retorno 3: '1234.57'
    // *     exemplo 4: number_format(67, 2, ',', '.');
    // *     retorno 4: '67,00'
    // *     exemplo 5: number_format(1000);
    // *     retorno 5: '1,000'
    // *     exemplo 6: number_format(67.311, 2);
    // *     retorno 6: '67.31'

    var n = number, prec = (decimals) ? decimals : 2;
    n = !isFinite(+n) ? 0 : +n;
    prec = !isFinite(+prec) ? 0 : Math.abs(prec);
    var sep = (typeof thousands_sep == 'undefined') ? '.' : thousands_sep;
    var dec = (typeof dec_point == 'undefined') ? ',' : dec_point;

    var s = (prec > 0) ? n.toFixed(prec) : Math.round(n).toFixed(prec); //fix for IE parseFloat(0.55).toFixed(0) = 0;

    var abs = Math.abs(n).toFixed(prec);
    var _, i;

    if (abs >= 1000) {
        _ = abs.split(/\D/);
        i = _[0].length % 3 || 3;

        _[0] = s.slice(0, i + (n < 0)) +
              _[0].slice(i).replace(/(\d{3})/g, sep + '$1');

        s = _.join(dec);
    } else {
        s = s.replace('.', dec);
    }

    return s;
}

/**************************************************************************
* Jquery regex
* http://james.padolsey.com/javascript/regex-selector-for-jquery/
**************************************************************************/
jQuery.expr[':'].regex = function (elem, index, match) {
    var matchParams = match[3].split(','),
        validLabels = /^(data|css):/,
        attr = {
            method: matchParams[0].match(validLabels) ?
                        matchParams[0].split(':')[0] : 'attr',
            property: matchParams.shift().replace(validLabels, '')
        },
        regexFlags = 'ig',
        regex = new RegExp(matchParams.join('').replace(/^\s+|\s+$/g, ''), regexFlags);
    return regex.test(jQuery(elem)[attr.method](attr.property));
}

/*******************************************************************************
* Converte uma data de um JsonResult
* Permite formatar caso seja passado um formato para a data. ex: dd/MM/yyyy
* Requer a bibliteca Date.js
*******************************************************************************/
String.prototype.toDateFromJson = function (format) {
    if (this) {
        var dte = eval("new " + this.replace(/\//g, '') + ";");
        dte.setMinutes(dte.getMinutes());
        return dte.toString(format ? format : null);
    }

    return null;
};

/*******************************************************************************
* lpad
* Preenche à esqueda uma string para um certo tamanho com outra string
*******************************************************************************/
String.prototype.lpad = function (length, string) {
    var fill = string;
    for (var i = 0; i < length; i++) { fill += string; }

    return (fill + this).slice(length * -1);
};

/*******************************************************************************
* startsWith
* Verifica se a palavra começa com o termo informado
*******************************************************************************/
if (typeof String.prototype.startsWith != 'function') {
    // see below for better implementation!
    String.prototype.startsWith = function (str) {
        return this.indexOf(str) == 0;
    };
}

/*******************************************************************************
* pushUnique
* Adiciona um valor em um array se o mesmo ainda não foi adicionado
*******************************************************************************/
Array.prototype.pushUnique = function (item) {
    //if (this.indexOf(item) == -1) {
    if (jQuery.inArray(item, this) == -1) {
        this.push(item);
        return true;
    }
    return false;
}

/*******************************************************************************
* filter
* busca por valores de acordo com a condicao
*******************************************************************************/
if (typeof Array.prototype.filter != 'function') {
    Array.prototype.find = function (fun) {
        var len = this.length;
        if (typeof fun != "function")
            throw new TypeError();

        var res = new Array();
        var thisp = arguments[1];
        for (var i = 0; i < len; i++) {
            if (i in this) {
                var val = this[i]; // in case fun mutates this
                if (fun.call(thisp, val, i, this))
                    res.push(val);
            }
        }
        return res;
    };
}

/*******************************************************************************
* trim
* Remove espaços em branco do início e do fim de uma string
*******************************************************************************/
String.prototype.trim = function () {
    return this.replace(/^\s+|\s+$/g, '');
};

/*******************************************************************************
* ltrim
* Remove espaços em branco do início de uma string
*******************************************************************************/
String.prototype.ltrim = function () {
    return this.replace(/^\s+/, '');
};

/*******************************************************************************
* ltrim
* Remove espaços em branco do fim de uma string
*******************************************************************************/
String.prototype.rtrim = function () {
    return this.replace(/\s+$/, '');
};

/*******************************************************************************
* ltrim
* Remove todos espaços em branco de uma string
*******************************************************************************/
String.prototype.fulltrim = function () {
    return this.replace(/(?:(?:^|\n)\s+|\s+(?:$|\n))/g, '').replace(/\s+/g, ' ');
};

/*******************************************************************************
* Object size
* retorna a quantidade de elementos de um objeto
*******************************************************************************/
Object.size = function(obj) {
    var size = 0, key;
    for (key in obj) {
        if (obj.hasOwnProperty(key)) size++;
    }
    return size;
};

/*******************************************************************************
* Simulate thread
*******************************************************************************/
//loops through an array in segments
var ThreadLoop = function (array) {
    var self = this;

    //holds the threaded work
    var thread = {
        work: null,
        wait: null,
        index: 0,
        total: array.length,
        finished: false
    };

    //set the properties for the class
    this.collection = array;
    this.finish = function () { };
    this.action = function () { throw "You must provide the action to do for each element"; };
    this.interval = 1;

    //set this to public so it can be changed
    var chunk = parseInt(thread.total * .005);
    this.chunk = (chunk == NaN || chunk == 0) ? thread.total : chunk;

    //end the thread interval
    thread.clear = function () {
        window.clearInterval(thread.work);
        window.clearTimeout(thread.wait);
        thread.work = null;
        thread.wait = null;
    };

    //checks to run the finish method
    thread.end = function () {
        if (thread.finished) { return; }
        self.finish(thread);
        thread.finished = true;
    };

    //set the function that handles the work
    thread.process = function () {
        //if (thread.index >= thread.total) { return false; }

        //thread, do a chunk of the work
        if (thread.work) {
            var part = Math.min((thread.index + self.chunk), thread.total);
            while (thread.index < part) {
                self.action(self.collection[thread.index], thread.index, thread.total);
                thread.index++;
            }
        }
        else {

            //no thread, just finish the work
            while (thread.index++ < thread.total) {
                self.action(self.collection[thread.index], thread.index, thread.total);
            }
        }

        //check for the end of the thread
        if (thread.index >= thread.total) {
            thread.clear();
            thread.end();
        }

        //return the process took place
        return true;

    };

    //set the working process
    self.start = function () {
        thread.finished = false;
        thread.index = 0;
        thread.work = window.setInterval(thread.process, self.interval);
    };

    //stop threading and finish the work
    self.wait = function (timeout) {

        //create the waiting function
        var complete = function () {
            thread.clear();
            thread.process();
            thread.end();
        };

        //if there is no time, just run it now
        if (!timeout) {
            complete();
        }
        else {
            thread.wait = window.setTimeout(complete, timeout);
        }
    };

};

// Plugins
//////////////////////////////////////////////////////////////////////
; (function ($) {

    /*****************************************************************
    * plugin que retorna a query string da url
    *****************************************************************/
    $.extend({
        getQueryString: function (name) {
            function parseParams() {
                var params = {},
                    e,
                    a = /\+/g,  // Regex for replacing addition symbol with a space
                    r = /([^&=]+)=?([^&]*)/g,
                    d = function (s) { return decodeURIComponent(s.replace(a, " ")); },
                    q = window.location.search.substring(1);

                while (e = r.exec(q))
                    params[d(e[1])] = d(e[2]);

                return params;
            }

            if (!this.queryStringParams) {
                this.queryStringParams = parseParams();
            }

            if (name) {
                return this.queryStringParams[name];
            }
            else {
                return this.queryStringParams;
            }
        },

        alert: function (message) {
            $('<div/>')
                .attr('title', 'Alerta')
                .append('<span class="ui-icon ui-icon-alert" style="float:left; margin:0 7px 50px 0;"></span>')
                .append('<p>' + message + '</p>')
                .dialog({
                    modal: true,
                    buttons: {
                        OK: function () {
                            $(this).remove();
                        }
                    },
                    close: function () {
                        $(this).remove();
                    }
                });
        },

        loadingBox: function (show) {
            if (show === 'show')
                $('#loading-box-message').show();
            else
                $('#loading-box-message').hide();
        },

        removeAccents: function (value) {
            var rs = value;
            rs = rs.replace(new RegExp("\\s", 'g'), "");
            rs = rs.replace(new RegExp("[àáâãäå]", 'g'), "a");
            rs = rs.replace(new RegExp("æ", 'g'), "ae");
            rs = rs.replace(new RegExp("ç", 'g'), "c");
            rs = rs.replace(new RegExp("[èéêë]", 'g'), "e");
            rs = rs.replace(new RegExp("[ìíîï]", 'g'), "i");
            rs = rs.replace(new RegExp("ñ", 'g'), "n");
            rs = rs.replace(new RegExp("[òóôõö]", 'g'), "o");
            rs = rs.replace(new RegExp("œ", 'g'), "oe");
            rs = rs.replace(new RegExp("[ùúûü]", 'g'), "u");
            rs = rs.replace(new RegExp("[ýÿ]", 'g'), "y");
            rs = rs.replace(new RegExp("\\W", 'g'), "");
            return rs;
        },

        format: function (source, params) {
            if (arguments.length == 1)
                return function () {
                    var args = $.makeArray(arguments);
                    args.unshift(source);
                    return $.format.apply(this, args);
                };
            if (arguments.length > 2 && params.constructor != Array) {
                params = $.makeArray(arguments).slice(1);
            }
            if (params.constructor != Array) {
                params = [params];
            }
            $.each(params, function (i, n) {
                source = source.replace(new RegExp("\\{" + i + "\\}", "g"), n);
            });
            return source;
        }
    });

    $.fn.passwordStrength = function (options) {
        var defaults = {
            bar: '',
            width: 200,
            showLabel: true
        };

        options = $.extend(defaults, options);
        var wrapper, label, progress, bar, self;

        var strength = function (text) {
            var label = new Array();
            label[0] = "Muito curta";
            label[1] = "Muito fraca";
            label[2] = "Fraca";
            label[3] = "Boa";
            label[4] = "Forte";
            label[5] = "Muito forte";

            var score = 0;

            if (text.length > 0) {
                //if password bigger than 6 give 1 point
                if (text.length >= 6) score++;

                //if password has uppercase characters give 1 point	
                if (text.match(/[A-Z]/g)) score++;

                //if password has lowercase characters give 1 point	
                if (text.match(/[a-z]/g)) score++;

                //if password has at least one number give 1 point
                if (text.match(/\d+/)) score++;

                //if password has at least one special caracther give 1 point
                if (text.match(/.[!,@,#,$,%,^,&,*,?,_,~,-,(,)]/)) score++;

                //TODO: verificar se existe caractere duplicado. Se houver diminui um score
            }

            return {
                score: score == 0 ? 0 : score * 20,
                label: label[score],
                color: score <= 1 ? 'progress-danger' : (score <= 3 ? 'progress-warning' : 'progress-success')
            };
        }

        var _text;

        return this.each(function () {
            self = $(this);

            if ($('html').hasClass('ie')) {
                _text = self.val() == self.attr('placeholder') ? '' : self.val();
            }
            else {
                _text = self.val();
            }

            wrapper = $(options.bar).css('width', options.width);
            if (options.showLabel) {
                label = $('<span/>').css('font-size', '11px');
            }
            progress = $('<div class="progress"/>').css('height', 14);
            bar = $('<div class="bar"/>');
            progress.append(bar);
            if (options.showLabel) {
                wrapper.append(label);
            }
            wrapper.append(progress);

            var o = strength(_text);
            progress.removeClass('progress-success').removeClass('progress-warning').removeClass('progress-danger');
            progress.addClass(o.color);
            if (options.showLabel) {
                label.text(o.label);
            }
            bar.width(o.score + '%');

            self.bind('keyup', function () {
                if ($('html').hasClass('ie')) {
                    _text = self.val() == self.attr('placeholder') ? '' : self.val();
                }
                else {
                    _text = self.val();
                }

                o = strength(_text);
                progress.removeClass('progress-success').removeClass('progress-warning').removeClass('progress-danger');
                progress.addClass(o.color);
                if (options.showLabel) {
                    label.text(o.label);
                }
                bar.width(o.score + '%');
            });
        });

    };

    $.fn.dialog = function (options) {
        var defaults = {
            width: 600,
            height: null
        };

        options = $.extend(defaults, options);
        var _this = this;
        var _body = _this.children('.modal-body');

        _this.addClass('modal hide fade')
            .attr('tabindex', '-1')
            .attr('role', 'dialog')
            .attr('aria-hidden', 'true')
            .attr('aria-labelledby', 'lbl-' + _this.attr('id'))
            .attr('data-keyboard', 'false')
            .css('width', options.width)
            //.css('top', -(_this.height() / 2))
            .css('margin-left', -(options.width / 2));

        // se existir a tag para adicionar iframe no conteudo
        if (_body.is('[data-iframe-url]')) {
            _body.empty();
            _body.css({ padding: 0, overflow: 'hidden' });

            var iframe = $('<iframe frameborder="0"/>').attr('src', _body.attr('data-iframe-url')).css({ width: '100%', height: '100%' }).appendTo(_body);

            $(iframe).load(function () {
                if (_body.is('[data-iframe-prefix]')) {
                    // verifica se nao existe um campo #context_prefix no dom do iframe
                    if ($(this).contents().find('#context-prefix').length == 0) {
                        // cria/adiciona o campo #context_prefix com o valor do atributo
                        $(this).contents().find('body').append($('<input type="hidden" id="context-prefix" />').val(_body.attr('data-iframe-prefix')));
                    }
                }
            });

            _body.removeAttr('data-iframe-url');
        }

        // se a altura for definida
        if (options.height != null) {
            _this.on('shown', function () {
                _body.height(
                    options.height
                    - parseInt(_this.find('.modal-header').outerHeight())
                    - parseInt(_this.find('.modal-footer').outerHeight())
                    - parseInt(_body.css('padding-top'))
                    - parseInt(_body.css('padding-bottom'))
                );
            });

            _this.css('height', options.height);
        }

        _this.find('.modal-header h3').attr('id', 'lbl-' + _this.attr('id'));

        return _this;
    };

    /*****************************************************************
    * plugin que faz o input piscar com uma cor diferente
    *****************************************************************/
    $.fn.blink = function (options) {
        var defaults = {
            cssClass: 'input-validation-error',
            microseconds: 3000,
            value: null
        };
        options = $.extend(defaults, options);
        var self;
        var time = null;

        return this.each(function () {
            self = $(this);
            self.addClass(options.cssClass);

            if ($.data(self[0], 'blink_time') != null) {
                clearTimeout($.data(self[0], 'blink_time'));
            }

            $.data(self[0], 'blink_time', setTimeout(function () {
                self.removeClass(options.cssClass);
            }, options.microseconds));

            if (options.value !== null) {
                self.val(options.value);
            }
        });
    };

    //TODO: remover
    $.fn.Report = function (options) {
        var href;
        return this.each(function () {
            href = $(this).attr('href');
            $(this)
                .attr('target', '_blank')
                .attr('href', href.indexOf('?') > 0 ? (href + location.search.replace('?', '&')) : href + location.search);
        });
    };

    $.fn.setFocus = function (options) {
        var self = $(this);
        var timeout, callback = {};

        if (Functions.isNumber(options)) {
            timeout = options;
        }
        else {
            var defaults = {
                timeout: 100
            };
            options = $.extend(defaults, options);

            callback = options.callback;
            timeout = options.timeout;
        }
        setTimeout(function () {
            self.focus(callback);
        }, timeout);
        return this;
    };

    $.fn.renderIcons = function (options) {
        var $these = this;

        if (typeof options == "string") {
            if (options == "reload") {
                return $these.each(function () {
                    $(this).next().removeClass($(this).prop('checked') ? 'icon-check-empty' : 'icon-check').addClass($(this).prop('checked') ? 'icon-check' : 'icon-check-empty');
                });
            }
            return;
        }

        var defaults = {
            change: null
        };

        options = $.extend(defaults, options);

        var $this;
        return $these.each(function () {
            $this = $(this);
            $this.hide().after('<i class="' + ($this.prop('checked') ? 'icon-check' : 'icon-check-empty') + '"/> ');
            $this
            .change(function () {
                var values = [];

                if ($this.prop('type') == 'checkbox') {
                    $(this).next().removeClass($(this).prop('checked') ? 'icon-check-empty' : 'icon-check').addClass($(this).prop('checked') ? 'icon-check' : 'icon-check-empty');
                    
                    $these.each(function (i, e) {
                        if ($(e).is(':checked')) {
                            values.push($(this).val());
                        }
                    });
                }
                else if ($this.prop('type') == 'radio') {
                    $these.each(function (i, e) {
                        $(e).next().removeClass('icon-check').addClass('icon-check-empty');
                    });
                    $(this).next().removeClass('icon-check-empty').addClass('icon-check');

                    values.push($(this).val());
                }

                if (typeof options.change == "function") {
                    options.change(values);
                }
            })
            .parents('li').click(function (e) { e.stopPropagation(); });
        });
    };
})(jQuery);