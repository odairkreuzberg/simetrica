/*!
* Jquery Grid
* requires jQuery 1.6+
*/
(function ($) {
    $.extend($.fn, {
        tGrid: function (options) {
            if (this[0]) {
                var grid = $.data(this[0], 'tgrid');
                if (grid) {
                    return grid;
                }

                grid = new $.tGrid(options, this[0]);
                $.data(this[0], 'tgrid', grid);

                if (typeof (grid.settings.onCreate) == 'function') {
                    grid.settings.onCreate(grid);
                }

                return grid;
            }
        }
    });
})(jQuery);

$.tGrid = function (options, table) {
    this.settings = $.extend(true, {}, $.tGrid.defaults, options);
    this.currentTable = table;
    this.init();
};

$.tGrid.escapeRegex = function (value) {
    return value.replace(/[-[\]{}()*+?.,\\^$|#\s]/g, "\\$&");
};

$.tGrid.customPaginator = function (grid, totalRows) {
    var p = new $.tGrid.paginator(grid);
    p.init();
    grid.header.hide();
    p.setTotalRows(totalRows);
    return p;
};

$.tGrid.paginator = function (grid) {
    var $_footer = grid.footer,
        $_column_left = null,
        $_column_rigth = null,
        $_label = null,
        $_total = 0,
        $_pages_display = 6;

    var _init = function () {
        _createFooter();
    };

    var _createFooter = function () {
        if ($_footer.children().length) {
            $_column_left = $($_footer.children('.datatable-column-left'));
            $_column_rigth = $($_footer.children('.datatable-column-right'));
            $_label = $_column_left.children('.datatable-info');
        }
        else {
            $_column_left = $('<div class="span6 datatable-column-left"/>');
            $_column_rigth = $('<div class="span6 datatable-column-right"/>');

            $_label = $('<div class="datatable-info"/>');
            $_column_left.append($_label);

            $_footer
                .append($_column_left)
                .append($_column_rigth);
        }
    };

    var _clickAction = function (e, rel) {
        grid.settings.pager.currentPage = rel;
        grid.paginate();
        grid.buttonHandler(grid.settings, $(grid.currentTable));
        e.preventDefault();
    };

    var _createNavigators = function (pageCount, callback) {
        // se a quantidade de registro for maior que o tamanho minimo para paginacao
        if ($_total > grid.settings.pager.size)
        {
            var currentPage = parseInt(grid.settings.pager.currentPage);
            var a, li;
            var html = $('<div class="pagination"><ul/></div>');
            var prev = $('<li><a href="#" title="Página anterior">Anterior</a></li>');
            var next = $('<li><a href="#" title="Próxima página">Próximo</a></li>');
            var navigators = new Array();

            // prev
            if (currentPage == 1) {
                html.find('ul').append(prev.addClass('disabled'));
            }
            else {
                a = prev.find('a')
                    .attr('rel', currentPage - 1)
                    .click(function (e) { _clickAction(e, $(this).attr('rel')) });
                navigators.push(a);

                html.find('ul').append(a.parent());
            }

            var start = 1;
            var end = pageCount;

            if (pageCount > $_pages_display) {
                var middle = Math.ceil($_pages_display / 2) - 1;
                var below = currentPage - middle;
                var above = currentPage + middle;

                if (below < 4) {
                    above = $_pages_display;
                    below = 1;
                }
                else if (above > (pageCount - 4)) {
                    above = pageCount;
                    below = pageCount - $_pages_display;
                }

                start = below;
                end = above;
            }

            // first pages
            if (start > 2) {
                a = $('<a/>').attr('href', '#')
                        .attr('rel', 1)
                        .text(1)
                        .click(function (e) { _clickAction(e, $(this).attr('rel')) });
                navigators.push(a);

                html.find('ul')
                    .append($('<li/>')
                    .append(a)
                );

                a = $('<a/>')
                        .attr('href', '#')
                        .attr('rel', 2)
                        .text(2)
                        .click(function (e) { _clickAction(e, $(this).attr('rel')) });
                navigators.push(a);

                html.find('ul')
                    .append($('<li/>')
                    .append(a)
                );

                html.find('ul').append('<li class="disabled"><a>...</a></li>');
            }

            // middle pages
            for (var i = start; i <= end; i++) {
                a = $('<a/>').text(i);
                li = $('<li/>');

                if (i == currentPage || (currentPage <= 0 && i == 0)) {
                    li.addClass('active').append(a);
                }
                else {
                    a.attr('href', '#').attr('rel', i);
                    a.click(function (e) { _clickAction(e, $(this).attr('rel')) });
                    li.append(a);
                    navigators.push(a);
                }

                html.find('ul').append(li);
            }

            // last pages
            if (end < (pageCount - 3)) {
                html.find('ul').append('<li class="disabled"><a>...</a></li>');

                a = $('<a/>')
                        .attr('href', '#')
                        .attr('rel', pageCount - 1)
                        .text(pageCount - 1)
                        .click(function (e) { _clickAction(e, $(this).attr('rel')) });
                navigators.push(a);

                html.find('ul')
                    .append($('<li/>')
                    .append(a)
                );

                a = $('<a/>')
                        .attr('href', '#')
                        .attr('rel', pageCount)
                        .text(pageCount)
                        .click(function (e) { _clickAction(e, $(this).attr('rel')) });
                navigators.push(a);

                html.find('ul')
                    .append($('<li/>')
                    .append(a)
                );
            }

            // next
            if (currentPage == pageCount) {
                html.find('ul').append(next.addClass('disabled'));
            }
            else {
                a = next.find('a')
                    .attr('rel', currentPage + 1)
                    .click(function (e) { _clickAction(e, $(this).attr('rel')) })
                navigators.push(a);

                html.find('ul').append(a.parent());
            }

            $_column_rigth.empty();
            $_column_rigth.append(html);

            if (typeof callback == 'function') {
                callback(navigators);
            }
        }
        else {
            if (!grid.hasHeaderButtons) {
                if (!grid.settings.searchable) {
                    grid.header.hide();
                }
            }
            else
            {
                grid.header.show();
                if (grid.settings.searchable) {
                    grid.header.children('.datatable-column-right').show();
                }
                else {
                    grid.header.children('.datatable-column-right').hide();
                }
            }

            // limpa a coluna que contem a paginacao
            $_column_rigth.empty();
        }
    };

    return {
        init: function () {
            _init();
        },

        setTotalRows: function (total) {
            $_total = total;
            $_label.text(total + ' registro(s) encontrado(s)');
        },

        createNavigators: function (pageCount, callback) {
            _createNavigators(pageCount, callback);
        },

        paginate: function () {
            var rows = grid.getRows(),
                _rows = [],
                iterateRows;
            var pageCounter = 1;

            if (grid.hasSearch) {
                rows.each(function (index, row) {
                    var cells = $(row).find('td');
                    if (cells.length > 0) {
                        var found = false;
                        cells.each(function (index, td) {
                            var regExp = new RegExp(grid.input_search.val(), 'i');
                            if (regExp.test($.removeAccents($(td).text()))) {
                                _rows.push($(row));
                                return false;
                            }
                        });
                    }
                });
            }

            rows.hide();
            iterateRows = ($.isArray(_rows) && _rows.length > 0) ? _rows : rows;
            $.each(iterateRows, function (i) {
                if (!(i < pageCounter * grid.settings.pager.size && i >= (pageCounter - 1) * grid.settings.pager.size)) {
                    pageCounter++;
                }
                if (pageCounter == grid.settings.pager.currentPage) {
                    $(this).show();
                }
            });

            this.setTotalRows(iterateRows.length);

            this.createNavigators(pageCounter);

            grid.header.children('.datatable-column-right').show();
            grid.header.show();
        }
    }
};

$.extend($.tGrid, {
    defaults: {
        // classe adicionada na tabela
        rootClass: 'tgrid table table-bordered table-condensed table-striped table-hover',
        // define qual posicao sera inserida uma nova linha na tabela usando o metodo addRow
        // append, insere no fim
        // prepend, insere no começo
        addMode: 'prepend',
        // configurações para paginação dos resultados
        // type normal, basicamente não faz nada além de estilizar os botões
        // type client, faz a paginação no DOM do html        
        pager: {
            enabled: false,
            size: 10,
            sizeOptions: [5, 10, 30, 50, 100],
            currentPage: 1,
            showTotalLabel: true,
            position: 'bottom'  // opcoes: bottom, top, both
        },
        // se possivel a selecao de linhas
        selectable: false,
        // se possivel a pesquisa de resultados no lado cliente
        searchable: false,
        // metodo chamado ao clicar na linha 
        // retorna os dados da linha setados como "item" 
        // (function(data){})
        select: null,
        // linha que contem dados
        rowData: 'tbody tr.data',
        // linha que contem a mensagem
        rowMessage: 'tbody tr.message',
        // metodo chamado ao clicar no botao "tgrid-detail"
        // retorna a referencia do proprio botao e um objeto com campos setados como Key
        // (function (button, keys) { })
        onClickDetailButton: null,
        // metodo chamado ao clicar no botao "tgrid-edit"
        // retorna a referencia do proprio botao e um objeto com campos setados como Key
        // (function (button, keys) { })
        onClickEditButton: null,
        // metodo chamado ao clicar no botao "tgrid-delete"
        // retorna a referencia do proprio botao e um objeto com campos setados como Key
        // (function (button, keys) { })
        onClickDeleteButton: null,
        // metodo chamado ao fazer qualquer alteração na grid (addRow, removeRow, removeAllRows)
        // retorna a referencia da propria grid
        // (function (grid) { })
        onUpdate: null,
        // metodo chamado ao terminar de renderizar a grid
        // retorna a referencia da propria grid
        // (function (grid) { })
        onCreate: null,
        // mensagens
        messages: {},
        // add 
        headerButtons: null,
    },

    currentTable: null,

    // funcao chamada ao ser disparado evento de click na linha da grid
    onClickRow: function (row, settings) {
        var data = [];
        // percorre todas tag TD da linha(TR) e adiciona ao array data os valores
        // os valores sao adicionados tanto com array numerico quanto array posicional
        // para array posicional, é esperado que a linha da grid contenha um atributo chamado item
        row.find('td').each(function (i, e) {
            data[i] = $(e).text();
            if ($(e).attr('value-name')) {
                data[$(e).attr('value-name')] = $(e).text();
            }
        });

        if (settings.selectable == true && typeof (settings.select) == 'function') {
            settings.select(data);
        }
    },

    triggerUpdate: function (update, grid) {
        if (typeof (update) == 'function') {
            update(grid.tGrid());
        }
    },

    redirectPageSize: function (value) {
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
    },

    check: function (table, rowData, rowMessage) {
        if (table.find(rowData).length == 0) {
            table.find(rowMessage).show();
        }
        else {
            table.find(rowMessage).hide();
        }
    },

    // retorna um array com os valores que todos campos setados como "unique"
    getUniqueValues: function (table, rowData) {
        var $find = null;
        var uniqueValues = [];

        table.find(rowData).each(function () {
            $find = $(this).find('input[type=hidden][unique=true]');
            if ($find.length) {
                uniqueValues.push($find.val());
            }
        });

        return uniqueValues;
    },

    getAllKeys: function (table, rowData) {
        var keys = {};
        table.find(rowData).each(function () {
            $(this).find('input[type=hidden][key]').each(function () {
                keys[$(this).attr('name')] = $(this).val();
            });
        });
        return keys;
    },

    getRowKeys: function (row) {
        var keys = {};
        row.find('input[type=hidden][key]').each(function () {
            keys[$(this).attr('name')] = $(this).val();
        });
        return keys;
    },

    getRowData: function (row) {
        var data = {}, name, value;
        row.find('input[name], select[name], textarea[name]').each(function (i) {
            value = $(this).val();
            data[i] = value;
            if ($(this).is('[data-name]')) {
                data[$(this).attr('data-name')] = value;
            }
            else {
                name = ($(this).attr('name')).substring($(this).attr('name').indexOf('.') + 1, $(this).attr('name').length);
                data[name] = value;
            }
        });
        return data;
    },

    checkTemplate: function (object) {
        if (!(object instanceof jQuery && object.length)) {
            if (!(typeof object == 'string' && object != '')) {
                throw 'O template deve ser definido como uma String válida ou um Objeto jQuery existente.';
            }
        }
    },

    prototype: {
        /// <summary>
        /// Construtor - Cria as estruturas e adiciona estilos
        /// </summary>
        init: function () {
            var _this = this;
            var $table = $(_this.currentTable);

            $table.addClass(_this.settings.rootClass);
            //$table.find('thead tr td').addClass('ui-state-default');

            // se a opcao selectable for true
            if (_this.settings.selectable) {
                // adiciona estilos ao elemento e adiciona handler para quando clicar na linha da grid
                $table.find('tbody tr td')
                    .css('cursor', 'pointer')
                    .css('padding', '6px 4px')
                    .parent('tr')
                    .click(function (e) {
                        // ao clicar na linha, chama metodo onClickRow, passando a propria referencia como parametro
                        $.tGrid.onClickRow($(this), _this.settings);
                        e.preventDefault();
                    });
            }

            // cria a tabela a ser inserida em footer
            //$tbfoot = $('<table width="100%"/>');

            // insere classe a primeira td de tfoot
            //$table.find('tfoot').find('td').first().addClass('ui-widget-content');

            // insere tfoot na tabela
            //$tbfoot.append($table.find('tfoot'));

            // cria a div wrapper
            var $wrapper = $('<div class="datatable"/>');

            // cria a div header
            if (_this.settings.pager.position != 'none') {
                var $header = $('<div class="row-fluid"><div class="span6 datatable-column-left"></div><div class="span6 datatable-column-right coluna-consulta"></div></div>');

                // seta variavel que informa que NAO existe botoes na header
                _this.hasHeaderButtons = false;

                if (_this.settings.headerButtons) {
                    // seta variavel que informa que existe botoes na header
                    _this.hasHeaderButtons = true;

                    // se o valor de headerButtons for um array
                    // Deve ser um array contendo referencia de elementos ja criado no html. Estes serao realocados para a grid.
                    if (_this.settings.headerButtons instanceof Array) {
                        for (var b = 0; b < _this.settings.headerButtons.length; b++) {
                            $header.find('.datatable-column-left').append($(_this.settings.headerButtons[b]));
                        }
                    }
                    // se for um objeto
                    else {
                        var btn;
                        for (var b in _this.settings.headerButtons) {
                            if (_this.settings.headerButtons.hasOwnProperty(b)) {
                                btn = $('<button type="button" class="btn" title="' + b + '">' + b + '</button>');

                                if (typeof (_this.settings.headerButtons[b]) == 'function') {
                                    btn.click(_this.settings.headerButtons[b]).button();
                                }
                                else if (typeof (_this.settings.headerButtons[b]) == 'object') {
                                    var object = _this.settings.headerButtons[b];

                                    if (typeof object["callback"] == 'function') {
                                        btn.click(object["callback"]);
                                    }

                                    if (object["icon"]) {
                                        btn.prepend('<i class="' + object["icon"] + '"></i> ');
                                    }
                                    else {
                                        btn.button();
                                    }
                                }

                                $header.find('.datatable-column-left').append(btn);
                            }
                        }
                    }
                }
                
                if (_this.settings.searchable) {
                    _this.input_search = $('<input type="search" placeholder="Buscar..." class="input-search" />');
                    var _rows = [],
                        current;

                    _this.input_search.bind('keydown', function (e) {
                        var keyCode = $.ui.keyCode;
                        switch (e.keyCode) {
                            case keyCode.ENTER:
                            case keyCode.NUMPAD_ENTER:
                                e.preventDefault();
                                break;
                            case keyCode.TAB:
                                break;
                            case keyCode.ESCAPE:
                                $(this)[0].value = '';
                            default:
                                var self = $(this);

                                if (e.which) {
                                    clearTimeout(_this.search);
                                    _this.search = setTimeout(function () {
                                        if (IsNullOrEmpty(self.val())) {
                                            //_this.searchedRows = null;
                                            _this.hasSearch = false;
                                            _this.paginate();
                                        }
                                        else {
                                            var rows = _this.getRows(),
                                                _rows = [];
                                            var pageCounter = 1;

                                            _this.settings.pager.currentPage = 1;
                                            _this.hasSearch = true;

                                            rows.each(function (index, row) {
                                                var cells = $(row).find('td');
                                                if (cells.length > 0) {
                                                    var found = false;
                                                    cells.each(function (index, td) {
                                                        var regExp = new RegExp($.removeAccents(self.val()), 'i');
                                                        if (regExp.test($.removeAccents($(td).text()))) {
                                                            _rows.push($(row));
                                                            return false;
                                                        }
                                                    });
                                                }
                                            });

                                            rows.hide();
                                            $.each(_rows, function (i) {
                                                if (!(i < pageCounter * _this.settings.pager.size && i >= (pageCounter - 1) * _this.settings.pager.size)) {
                                                    pageCounter++;
                                                }
                                                if (pageCounter == _this.settings.pager.currentPage) {
                                                    $(this).show();
                                                }
                                            });

                                            _this.paginator.setTotalRows(_rows.length);

                                            _this.paginator.createNavigators(pageCounter);
                                        }

                                        _this.buttonHandler(_this.settings, $(_this.currentTable));
                                    }, 50);
                                }
                        }
                    });

                    $header.find('.coluna-consulta').append(_this.input_search);

                    // TODO: verificar este script de placeholder (versao para plugin jquery)
                    Placeholders.refresh();
                }

                // cria a div do footer
                var $footer = $('<div class="row-fluid footer"/>');

                // se as colunas da esquerda e direita do header estiverem vazias
                if ($header.find('.datatable-column-left').is(':empty') && $header.find('.datatable-column-right').is(':empty'))
                {
                    // esconde o header
                    $header.hide();
                }

                _this.header = $header;
                _this.footer = $footer;

                // insere conteudo dentro de footer
                //$footer.append($tbfoot);
            }

            // cria div da tabela
            var $content = $('<div class="tgrid-content"/>');

            // armazena conteudos
            _this.content = $content;
            _this.wrapper = $wrapper;


            // insere wrapper depois do elemento table
            $wrapper.insertAfter($table);

            // insere table dentro de content
            $content.append($table);

            if (_this.settings.pager.position != 'none') {
                // insere header dentro de wrapper
                $wrapper.append($header);
            }
            // insere content dentro de wrapper
            $wrapper.append($content);


            if (_this.settings.pager.position != 'none') {
                // insere footer dentro de wrapper
                $wrapper.append($footer);

                // estiliza botoes da paginacao
                var links = $footer.find('a');

                links.button();
                //.removeClass('ui-corner-all')
                //.addClass('noborder');

                links.first().button({
                    icons: { primary: 'ui-icon-arrow-1-w' },
                    text: false
                })
                .addClass('first')
                .addClass('ui-corner-left');

                links.last().button({
                    icons: { primary: 'ui-icon-arrow-1-e' },
                    text: false
                })
                .addClass('last')
                .addClass('ui-corner-right');

                $footer.find('a.disabled').button({
                    disabled: true
                });

                $footer.find('a.active').button({
                    disabled: true,
                }).addClass('ui-state-hover ui-state-focus');

                $footer.find('select').bind('change', function () {
                    $.tGrid.redirectPageSize($(this).val());
                });

                if (_this.totalCount() > 0) {
                    _this.buttonHandler(_this.settings, $table);
                }

                _this.paginate();
            }
        },

        paginate: function () {
            if (this.settings.pager) {
                if (this.settings.pager.enabled) {
                    if (!this.paginator) {
                        this.paginator = new $.tGrid.paginator(this);
                    }
                    this.paginator.init();
                    this.paginator.paginate();
                }
            }

            return this;
        },

        /// <summary>
        /// Desabilita os botães de uma linha da grid
        /// </summary>
        /// <param name="row">Linha que contém os botões</param>
        /// <param name="enable">Flag que define se os botões serão habilitados/desabilitados</param>
        disableButtons: function (row, disable) {
            disable = disable == undefined ? true : disable;
            var command = disable == true ? 'disable' : 'enable';
            $(this.currentTable).find(row).find('.tgrid-detail').button(command);
            $(this.currentTable).find(row).find('.tgrid-edit').button(command);
            $(this.currentTable).find(row).find('.tgrid-delete').button(command);
        },

        /// <summary>
        /// Desabilita todos botões da grid
        /// </summary>
        /// <param name="enable">Flag que define se os botões serão habilitados/desabilitados</param>
        disableAllButtons: function (disable) {
            disable = disable == undefined ? true : disable;
            var command = disable == true ? 'disable' : 'enable';
            $(this.currentTable).find('.tgrid-detail').button(command);
            $(this.currentTable).find('.tgrid-edit').button(command);
            $(this.currentTable).find('.tgrid-delete').button(command);
        },

        /// <summary>
        /// Adiciona handler aos botoes da grid de acordo com a classe css
        /// </summary>
        /// <param name="settings">Configuraçães padrão da grid</param>
        /// <param name="update">Estrutura que contém os botões (pode ser TABLE ou TR)</param>
        buttonHandler: function (settings, struct) {
            if (typeof (settings.onClickDetailButton) == 'function') {
                struct.find('.tgrid-detail').click(function (e) {
                    if (!($(this).button("option", "disabled") == true)) {
                        settings.onClickDetailButton($(this), $.tGrid.getRowData($(this).parents('tr.data')));
                    }
                    e.preventDefault();
                });
            }

            if (typeof (settings.onClickEditButton) == 'function') {
                struct.find('.tgrid-edit').click(function (e) {
                    if (!($(this).button("option", "disabled") == true)) {
                        settings.onClickEditButton($(this), $.tGrid.getRowData($(this).parents('tr.data')));
                    }
                    e.preventDefault();
                });
            }

            if (typeof (settings.onClickDeleteButton) == 'function') {
                struct.find('.tgrid-delete').click(function (e) {
                    if (!($(this).button("option", "disabled") == true)) {
                        settings.onClickDeleteButton($(this), $.tGrid.getRowData($(this).parents('tr.data')));
                    }
                    e.preventDefault();
                });
            }
        },

        /// <summary>
        /// Atualiza os indices das linhas e mantém sempre iniciando de zero
        /// </summary>
        /// <param name="update">Flag que define se seré atualizado os índices</param>
        updateIndex: function () {
            var $table = $(this.currentTable);
            // percorre todas linhas que estao setadas como "data"
            $table.find(this.settings.rowData).each(function (i) {
                // percorre todos campos hidden da linha e atualiza o indice
                $(this).find('input[type], select[name], textarea[name]').each(function () {
                    $(this).attr('name', $(this).attr('name').replace(/(\d+)/g, i));
                });
            });

            return this;
        },

        /// <summary>
        /// Adiciona uma linha a grid
        /// </summary>
        /// <param name="tmpl">Template da linha a ser inserida</param>
        /// <param name="data">Objeto com os dados do template</param>
        /// <param name="updateIndex">Define se é preciso atualizar os indíces após inserir linha (Padrão False)</param>
        /// <param name="paginate">Define se é preciso fazer paginação (Padrão False)</param>
        addRow: function (tmpl, data) {
            $.tGrid.checkTemplate(tmpl);
            var $table = $(this.currentTable);
            var $row = $.tmpl(tmpl, data);

            if (this.settings.addMode == 'prepend') {
                $row.prependTo($table);
            }
            else {
                $row.appendTo($table);
            }

            this.buttonHandler(this.settings, $row);
            $.tGrid.check($table, this.settings.rowData, this.settings.rowMessage);
            $.tGrid.triggerUpdate(this.settings.onUpdate, $(this.currentTable));

            return this;
        },

        /// <summary>
        /// Atualiza os dados de uma linha
        /// </summary>
        /// <param name="row">Linha atual a ser alterada</param>
        /// <param name="tmpl">Template da linha a ser inserida</param>
        /// <param name="data">Objeto com os dados do template</param>
        updateRow: function (row, tmpl, data) {
            $.tGrid.checkTemplate(tmpl);
            $row = $.tmpl(tmpl, data);
            this.buttonHandler(this.settings, $row);
            row.after($row);
            row.remove();
            $.tGrid.triggerUpdate(this.settings.onUpdate, $(this.currentTable));

            return this;
        },

        /// <summary>
        /// Remove uma linha
        /// </summary>
        /// <param name="row">Linha(TR) a ser removida</param>
        removeRow: function (el) {
            el.parents('tr.data').remove();
            var $table = $(this.currentTable);
            $.tGrid.check($table, this.settings.rowData, this.settings.rowMessage);
            $.tGrid.triggerUpdate(this.settings.onUpdate, $(this.currentTable));

            return this;
        },

        /// <summary>
        /// Remove todas linhas da grid
        /// </summary>
        removeAllRows: function () {
            var $table = $(this.currentTable);
            $table.find(this.settings.rowData).remove();
            $.tGrid.check($table, this.settings.rowData, this.settings.rowMessage);
            $.tGrid.triggerUpdate(this.settings.onUpdate, $(this.currentTable));
            this.footer.hide();
            return this;
        },

        /// <summary>
        /// Retorna a quantidade de linhas setadas como "data"
        /// </summary>
        count: function () {
            return this.getRows().length;
        },

        /// <summary>
        /// Retorna todas linhas setadas como "data"
        /// </summary>
        getRows: function () {
            return $(this.currentTable).find(this.settings.rowData);
        },

        /// <summary>
        /// Retorna a quantidade total de linhas da grid
        /// </summary>
        totalCount: function () {
            return $(this.currentTable).find('tbody tr').length;
        },

        /// <summary>
        /// Verifica se já existe uma linha "unique" com o valor informado
        /// </summary>
        /// <param name="value">Valor a ser verificado</param>
        hasUniqueValue: function (value) {
            var uniqueValues = $.tGrid.getUniqueValues($(this.currentTable), this.settings.rowData);
            return ($.inArray(value.toString(), uniqueValues) >= 0);
        },

        /// <summary>
        /// Recupera todos os valores dos campos-chaves (key="true") da linha informada
        /// </summary>
        /// <param name="row">Linha a ser verificada</param>
        getRowKeys: function (row) {
            return $.tGrid.getRowKeys(row);
        },

        /// <summary>
        /// Recupera todos os valores (campos hidden) da linha informada
        /// </summary>
        /// <param name="row">Linha a ser verificada</param>
        getRowData: function (row) {
            return $.tGrid.getRowData(row);
        },

        /// <summary>
        /// Recupera todos os valores dos campos-chaves (key="true") de todas linhas da tabela
        /// </summary>
        getAllKeys: function () {
            var $table = $(this.currentTable);
            return $.tGrid.getAllKeys($table, this.settings.rowData);
        },

        /// <summary>
        /// Desenha barra de progresso que esconde grid. A grid é exibida apenas ao ser invocado .dispose()
        /// </summary>
        progressbar: function () {
            var _this = this;

            if (this.loading == undefined || this.loading == false) {
                this.loading = true;

                this.header.hide();
                this.content.hide();
                this.footer.hide();

                //this.bar = $('<div class="progress progress-striped active" style="position:relative"/>');
                //this.bar.append('<div class="bar"><p class="label-bar"/></div>');

                this.bar = $('<div/>');
                this.bar.append('<div class="progress progress-striped active"><div class="bar"/></div>');
                this.wrapper.prepend(this.bar);
            }

            return {
                update: function (percent) {
                    //_this.bar.find('.label-bar').text(Math.floor(percent) + '%');
                    //_this.bar.find('.bar').css('width', percent + '%');

                    _this.bar.find('.bar').css('width', percent + '%').text(Math.floor(percent) + '%');
                },

                dispose: function () {
                    _this.loading == false;
                    _this.bar.animate({
                        opacity: 0,
                        height: 'toggle'
                    }, 300, function () {
                        if (_this.getRows().length > _this.settings.pager.size) {
                            _this.header.show();
                        }
                        _this.content.show();
                        _this.footer.show();
                    }).find('.progress').removeClass('active');
                }
            }
        }
    }
});