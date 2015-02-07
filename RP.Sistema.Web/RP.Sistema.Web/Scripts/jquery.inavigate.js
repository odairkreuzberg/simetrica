(function ($) {
    $.extend($.fn, {
        iNavigate: function (options) {
            if (this[0]) {
                var input = $.data(this[0], 'iNavigate');
                if (input) {
                    return input;
                }

                input = new $.iNavigate(options, this[0]);
                $.data(this[0], 'iNavigate', input);

                return input;
            }
        }
    });
})(jQuery);

$.iNavigate = function (options, input) {
    this.settings = $.extend(true, {}, $.iNavigate.defaults, options);
    this.current = input;
    this.init();
};

$.iNavigate.escapeRegex = function (value) {
    return value.replace(/[-[\]{}()*+?.,\\^$|#\s]/g, "\\$&");
};

$.iNavigate.filter = function (array, term) {
    var matcher = new RegExp($.iNavigate.escapeRegex(term), "i");
    return $.grep(array, function (value) {
        for (var field in value) {
            if (matcher.test(value[field])) {
                return value[field];
            }
        }
    });
};

$.extend($.iNavigate, {
    defaults: {
        appendTo: 'body',
        cssClass: 'inavigate',
        focusClass: 'warning',
        delay: 300,
        position: {
            my: 'left top',
            at: 'left bottom',
            offset: '1px 2px',
            collision: 'none'
        },
        width: 400,
        height: 100,
        // estrutura esperada para coluna
        // name: Define o nome da coluna que será exibido no header da tabela
        // width: Define o tamanho da coluna do header
        // bind: Define qual campo do resultado será atachado a coluna
        // dateFormat: Define o formato para valor do tipo Date
        // align: Define o alinhamento do elemento dentro da célula
        columns: [],
        showHeader: true,
        source: null,
        minLength: 2,
        paginate: {
            size: 10,
            message: 'Exibindo {start} até {end} de {count}'
        },
        select: null,
        change: null,
        onsearch: null,
        notFoundMessage: "Nenhum registro encontrado"
    },

    current: null,

    direction: {
        UP: 1,
        DOWN: 2
    },

    prototype: {
        init: function () {
            var self = this;
            var $input = $(self.current);

            $input
            .attr('autocomplete', 'off')
			.bind('keydown', function (event) {
			    var keyCode = $.ui.keyCode;
			    switch (event.keyCode) {
			        case keyCode.PAGE_UP:
			            break;
			        case keyCode.PAGE_DOWN:
			            break;
			        case keyCode.UP:
			            self._move($.iNavigate.direction.UP);
			            event.preventDefault();
			            break;
			        case keyCode.DOWN:
			            self._move($.iNavigate.direction.DOWN);
			            event.preventDefault();
			            break;
			        case keyCode.ENTER:
			        case keyCode.NUMPAD_ENTER:
			            if (self.active) {
			                self._setSelectDataInput($input, event);
			                self.close();
			                event.preventDefault();
			            }
			        case keyCode.TAB:
			            if (!self.active) {
			                return;
			            }
			            self._setSelectDataInput($input, event);
			            break;
			        case keyCode.ESCAPE:
			            $input.val(self.term);
			            self.close();
			            break;
			        default:
			            var paginate = false;

			            self.page = self.page == null ? 1 : self.page;

			            if (event.ctrlKey) {
			                if (event.keyCode == $.ui.keyCode.LEFT) {
			                    if (self.page > 1) {
			                        self.page--;
			                        paginate = true;
			                    }
			                    else {
			                        paginate = false;
			                    }
			                }
			                else if (event.keyCode == $.ui.keyCode.RIGHT) {
			                    if (self.page < self.pageCount) {
			                        self.page++;
			                        paginate = true;
			                    }
			                    else {
			                        paginate = false;
			                    }
			                }
			            }
			            else {
			                self.page = 1;
			            }

			            clearTimeout(self.searching);
			            self.searching = setTimeout(function () {
			                if ($.iNavigate.term != $input.val()) {
			                    self.term = $input.val();
			                    self.search($input.val(), self.page, self.settings.paginate.size);
			                }
			                else if (paginate === true) {
			                    self.search($input.val(), self.page, self.settings.paginate.size);
			                    event.preventDefault();
			                }
			            }, self.settings.delay);
			            break;
			    }
			})
			.bind('focus', function () {
			    $(this).select();
			})
            .bind('change', function () {
                if ($input.val() == '') {
                    self.selectedItem = null;
                }
                self._change();
            })
			.bind('blur', function () {
			    if (self.uncloseable === true) {
			        return;
			    }

			    clearTimeout(self.searching);
			    self.closing = setTimeout(function () {
			        self.close();
			    }, 150);
			});

            self._initSource();

            self.response = function () {
                return self._response.apply(self, arguments);
            };

            self.navigator = $('<div/>')
				.addClass(self.settings.cssClass)
				.appendTo($(self.settings.appendTo || "body", $input[0].ownerDocument)[0])
				.zIndex($input.zIndex() + 1)
				.css({ top: 0, left: 0 })
				.width(self.settings.width)
				.hide();

            self.pending = 0;
        },

        _initSource: function () {
            if ($.isArray(this.settings.source)) {
                this.source = function (request, response) {
                    response($.iNavigate.filter(this.settings.source, request.term));
                };
            }
            else if (typeof this.settings.source === "string") {
                var self = this,
					url = this.settings.source;

                this.source = function (request, response) {

                    if (self.xhr) {
                        self.xhr.abort();
                    }
                    self.xhr = $.ajax({
                        url: url,
                        data: request,
                        dataType: "json",
                        success: function (data, status, xhr) {
                            if (xhr === self.xhr) {
                                response(data.result);
                                self.pageCount = Math.ceil(data.count / request.pagesize);
                                self._setFooterValues(((request.page * request.pagesize) - request.pagesize + 1), (request.page * request.pagesize > data.count ? data.count : request.page * request.pagesize), data.count);
                                self._move($.iNavigate.direction.DOWN, false);
                                if (data.count == 0) {
                                    self.selectedItem = null;
                                    self.addRowMessage(self.settings.notFoundMessage);
                                }
                                else if (data.message) {
                                    self.selectedItem = null;
                                    self.addRowMessage(data.message);
                                }
                            }
                            self.xhr = null;
                        },
                        error: function (xhr) {
                            if (xhr === self.xhr) {
                                response([]);
                            }
                            self.xhr = null;
                            Functions.checkRequest(xhr);
                        }
                    });
                };
            }
            else {
                this.source = this.settings.source;
            }
        },

        _search: function (params) {
            this.pending++;
            $.loadingBox('show');
            this.source(params, this.response);
        },

        _response: function (content) {
            if (!this.settings.disabled && content && content.length) {
                this._suggest(content);
            }
            else {
                this.close();
            }

            this.pending--;
            if (!this.pending) {
                $.loadingBox('hide');
            }
        },

        _suggest: function (items) {
            this.rows = items;

            var navigator = this.navigator
				.empty()
				.zIndex($(this.current).zIndex() + 1);

            this._renderTable(navigator, items);
            this._open();

            var position = jQuery.extend(true, {}, this.settings.position);

            if (this.navigator.outerHeight() < $(document).height() &&
							($(this.current).offset().top + $(this.current).outerHeight() + this.navigator.outerHeight()) > $(document).height()) {
                position.my = "left bottom";
                position.at = "left top";
            }

            if (($(this.current).offset().left + this.navigator.outerWidth()) > $(document).width()) {
                if (($(this.current).offset().top + $(this.current).outerHeight() + this.navigator.outerHeight()) > $(document).height()) {
                    position.my = "right bottom";
                    position.at = "right top";
                }
                else {
                    position.my = "right top";
                    position.at = "right bottom";
                }
            }

            navigator.position($.extend({
                of: $(this.current)
            }, position));

            //TODO: verificar se esta chamada esta em lugar correto
            if ($.isArray(this.settings.source)) {
                this._setFooterValues(0, 0, items.length);
            }
        },

        _renderTable: function (container, items) {
            var $table = $('<table/>').appendTo(container).addClass('table table-bordered table-condensed table-striped table-hover');
            var self = this;
            var _columns = null;

            if (!this.hideHeaderAndFooter === true) {
                if (self.settings.showHeader == true && self.settings.columns.length > 0) {
                    self._renderTableHeader($table, self.settings.columns);
                }
            }

            if (items.message) {
                self._renderTableMessage($table, items.text);
            }
            else {
                $.each(items, function (index, item) {
                    self._renderTableItem($table, item, self.settings.columns);
                });
            }

            if (!this.hideHeaderAndFooter === true) {
                self._renderTableFooter($table, self.settings.columns.length);
            }

            this.hideHeaderAndFooter = false;

            this.showHeader = self.settings.showHeader;
            this.showFooter = true;

            $table.find('tbody tr')
			.bind('mouseover', function () {
			    $table.find('tr.' + self.settings.focusClass).removeClass(self.settings.focusClass);
			    $(this).addClass(self.settings.focusClass);
			})
			.bind('mouseleave', function () {
			    $(this).removeClass(self.settings.focusClass);
			})
			.bind('mousedown', function () {
			    self.uncloseable = true;
			})
			.bind('mouseup', function (e) {
			    self._setSelectDataInput($(self.current), e);
			    self.uncloseable = false;
			    self.close();
			});

            $table.find('tfoot')
			.bind('mousedown', function () {
			    self.uncloseable = true;
			});
        },

        _renderTableHeader: function (table, columns) {
            var $thead = $('<thead/>');
            var $tr = $('<tr/>');
            var $td;

            for (var i =0; i< columns.length; i++) {
                if (typeof columns[i] == 'object') {
                    $td = $('<th/>');
                    $td.text(columns[i].name);

                    if (columns[i].width) {
                        $td.width(columns[i].width);
                    }
                    $td.appendTo($tr);
                }
            }

            $tr.appendTo($thead);
            $thead.appendTo(table);
        },

        _renderTableItem: function (node, item, columns) {
            var $tr = $('<tr/>');
            var td;
            var index = 0;
            var value, text;

            for (var pos in item) {
                if (columns[index]) {
                    value = item[columns[index].bind];

                    if (value == null) {
                        text = '';
                    }
                    else {
                        if (columns[index].dateFormat) {
                            text = value.toDateFromJson(columns[index].dateFormat);
                        }
                        else {
                            text = value;
                        }
                    }

                    td = $('<td/>');

                    if (columns[index].align) {
                        td.css('text-align', columns[index].align);
                    }

                    td.text(text)
                        .appendTo($tr);
                }
                index++;
            }

            $tr.appendTo(node);
        },

        _renderTableMessage: function (node, message) {
            node.append('<tr><td>' + message + '</td></tr>');
        },

        _renderTableFooter: function (table, colspan) {
            var ul = '<div style="margin:0" class="pagination pagination-small">' +
                        '<ul>' +
                            '<li><a href="#">«</a></li>' +
                            '<li><a href="#">»</a></li>' +
                        '</ul>' +
                     '</div>';

            var $tfoot = $('<tfoot/>');
            var $tr = $('<tr/>')
            .append(
                $('<td class="row-fluid"/>').attr('colspan', colspan)
                    .append($('<div class="span7 message"/>').text(this.settings.paginate.message))
                    .append($('<div class="span5 navigator pagination-right"/>').html(ul))
             );

            $tr.appendTo($tfoot);
            $tfoot.appendTo(table);
        },

        _setFooterValues: function (start, end, count) {
            var self = this;
            self.page = self.page == null ? 1 : self.page;

            var $messageContainer = $(this.navigator).find('table tfoot .message');
            var $navContainer = $(this.navigator).find('table tfoot .navigator');

            var message = this.settings.paginate.message;
            message = message
			    .replace('{start}', start)
			    .replace('{end}', end)
			    .replace('{count}', count);
            $messageContainer.text(message);
            
            var prev = $navContainer.find('ul li:first-child').addClass('disabled');
            var next = $navContainer.find('ul li:last-child').addClass('disabled');

            if (self.pageCount > 1) {
                if (self.page == 1) {
                    prev.addClass('disabled');
                    next.removeClass('disabled');
                }
                else if (self.page == self.pageCount) {
                    prev.removeClass('disabled');
                    next.addClass('disabled');
                }
                else {
                    prev.removeClass('disabled');
                    next.removeClass('disabled');
                }
            }

            prev.click(function (e) {
                $(self.current).focus();
                if (!$(this).hasClass('disabled')) {
                    self.search($(self.current).val(), --self.page, self.settings.paginate.size);
                }
                e.preventDefault();
            });

            next.click(function (e) {
                $(self.current).focus();
                if (!$(this).hasClass('disabled')) {
                    self.search($(self.current).val(), ++self.page, self.settings.paginate.size);
                }
                e.preventDefault();
            });
        },

        _move: function (direction, search) {
            if (!$(this.navigator).is(":visible")) {
                if (search === undefined || search === true) {
                    this.search(null);
                }
                return;
            }

            if (direction === $.iNavigate.direction.UP) {
                var $table = $(this.navigator).find('table');
                if (this.active && $table.length) {
                    var rows = $table.find('tbody').find('tr');
                    var index = $table.find('tr.' + this.settings.focusClass).index();
                    var newIndex = (index > 0) ? (index - 1) : 0;
                    $table.find('tr.' + this.settings.focusClass).removeClass(this.settings.focusClass);
                    rows.eq(newIndex).addClass(this.settings.focusClass);
                }
            }
            else if (direction === $.iNavigate.direction.DOWN) {
                var $table = $(this.navigator).find('table');
                if (this.active && $table.length) {
                    var rows = $table.find('tbody').find('tr');
                    var index = $table.find('tr.' + this.settings.focusClass).index();
                    var newIndex = (index < (rows.length - 1)) ? (index + 1) : (rows.length - 1);
                    $table.find('tr.' + this.settings.focusClass).removeClass(this.settings.focusClass);
                    rows.eq(newIndex).addClass(this.settings.focusClass);
                }
            }
        },

        _open: function () {
            var self = this;
            this.navigator.show(0, function () {
                self.active = true;
            });
        },

        _change: function () {
            if (typeof this.settings.change == 'function') {
                this.settings.change(this.selectedItem);
            }
        },

        _onsearch: function () {
            if (typeof this.settings.onsearch == 'function') {
                this.settings.onsearch();
            }
        },

        _setSelectDataInput: function (input, event) {
            this.selectedItem = this.getSelectedRow();
            if (typeof this.settings.select == 'function') {
                this.settings.select(this.selectedItem, event);
            }
            else {
                var data = this.selectedItem;
                if (data) {
                    for (var index in data) break;
                    input.val(data[index]);
                }
            }
        },

        _hasScroll: function () {
            var element = $(this.navigator).find('.body-container')
            return element.outerHeight() < element.attr("scrollHeight");
        },

        getSelectedRow: function () {
            var $table = $(this.navigator).find('table');
            if ($table.length) {
                var index = $table.find('tr.' + this.settings.focusClass).index();
                return this.rows[index];
            }
            return null;
        },

        search: function (value, page, pagesize) {
            value = value != null ? value : $(this.current).val();

            $.iNavigate.term = $(this.current).val();

            if (value.length < this.settings.minLength) {
                return this.close();
            }

            clearTimeout(this.closing);

            this._onsearch();
            $(this.current).trigger('onsearch');

            var params = { filter: value };
            params.page = page ? page : 1;
            params.pagesize = pagesize ? pagesize : this.settings.paginate.size;

            if (this.extraParams) {
                for (var p in this.extraParams) {
                    params[p] = this.extraParams[p];
                }
            }

            if (!this.cancelCurrentRequest === true) {
                this._search(params);
            }

            this.extraParams = null;
            this.cancelCurrentRequest = false;
        },

        close: function () {
            var self = this;
            clearTimeout(self.closing);
            if (self.navigator.is(':visible')) {
                self.navigator.hide(0, function () {
                    self.active = false;
                });
            }
        },

        addParams: function (params) {
            this.extraParams = params;
        },

        cancelRequest: function () {
            this.cancelCurrentRequest = true;
        },

        addRowMessage: function (message) {
            this.hideHeaderAndFooter = true;
            this._suggest({ message: true, text: message });
        }
    }
});