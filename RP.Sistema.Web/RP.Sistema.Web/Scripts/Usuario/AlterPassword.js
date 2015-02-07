//var NovaSenha = new function () {
//    this.timeout = null;

//    this.content = function () {
//        return $($.trim(tmpl("tmpl-senha-descricao", {})));
//    };

//    this.title = function (el) {
//        var $this = $(el);
//        var er = new RegExp($this.attr('data-val-regex-pattern'));
//        if (er.test($this.val())) {
//            return '<strong>Critérios da senha</strong>' +
//                       '<span class="label label-success pull-right"><i class="icon-ok icon-white"></i> <span>Atende</span>' +
//                   '</span>';
//        }
//        else {
//            return '<strong>Critérios da senha</strong>' +
//                       '<span class="label label-important pull-right"><i class="icon-remove icon-white"></i> <span>Não atende</span>' +
//                   '</span>';
//        }
//    };

//    this.check = function (el) {
//        var $this = $(el);
//        var er = new RegExp($this.attr('data-val-regex-pattern'));
//        if (er.test($this.val())) {
//            $this.nextAll('.popover').find('.popover-title')
//                .find('.label').removeClass('label-important').addClass('label-success')
//                .find('i').removeClass('icon-remove').addClass('icon-ok')
//                .parent().find('span').text('Atende');
//        }
//        else {
//            $this.nextAll('.popover').find('.popover-title')
//                .find('.label').removeClass('label-success').addClass('label-important')
//                .find('i').removeClass('icon-ok').addClass('icon-remove')
//                .parent().find('span').text('Não atende');
//        }
//    };
//};

//$(function () {
//    $('#NovaSenha').passwordStrength({
//        bar: '#progress-bar',
//        width: 220
//    });

//    $('#NovaSenha')
//        .popover({
//            trigger: 'focus',
//            html: true,
//            title: function () {
//                return NovaSenha.title(this);
//            },
//            content: function () {
//                return NovaSenha.content();
//            }
//        })
//        .keydown(function (e, i) {
//            clearTimeout(NovaSenha.timeout);
//            NovaSenha.timeout = setTimeout(function () {
//                NovaSenha.check(e.target);
//            }, 50);
//        });
//});
