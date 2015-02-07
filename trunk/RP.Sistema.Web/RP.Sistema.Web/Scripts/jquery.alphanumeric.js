/*!
* alphanumeric, numeric, alpha - http://www.itgroup.com.ph/alphanumeric/
* floatnumber - http://www.tuliofaria.net/jquery-floatnumber/
*/


(function ($) {

    //$('.sample1').alphanumeric();
    //$('.sample2').alphanumeric({ allow: "., " });
    //$('.sample6').alphanumeric({ ichars: '.1a' });
    $.fn.alphanumeric = function (p) {

        p = $.extend({
            ichars: "!@#$%^&*()+=[]\\\';,/{}|\":<>?~`.- ",
            nchars: "",
            allow: "",
            padding: '0',
            length: 0,
            contextmenuoff: false,
            ctrlpasteoff: false
        }, p);

        return this.each
			(
				function () {

				    if (p.nocaps) p.nchars += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
				    if (p.allcaps) p.nchars += "abcdefghijklmnopqrstuvwxyz";

				    s = p.allow.split('');
				    for (i = 0; i < s.length; i++) if (p.ichars.indexOf(s[i]) != -1) s[i] = "\\" + s[i];
				    p.allow = s.join('|');

				    var reg = new RegExp(p.allow, 'gi');
				    var ch = p.ichars + p.nchars;
				    ch = ch.replace(reg, '');

				    $(this).keypress(function (e) {
				        if (!e.charCode) k = String.fromCharCode(e.which);
				        else k = String.fromCharCode(e.charCode);

				        if (ch.indexOf(k) != -1) e.preventDefault();

				        if (p.ctrlpasteoff) {
				            if ((e.ctrlKey && k == 'v')) e.preventDefault();
				        }
				    });

				    if (p.length > 0) {
				        $(this).change(function () {
				            if ($(this).val().length > 0 && $(this).val().length < p.length) {
				                $(this).val(Array(p.length - $(this).val().length + 1).join(p.padding) + $(this).val());
				            }
				        });
				    }

				    if (p.contextmenuoff) {
				        $(this).bind('contextmenu', function () { return false });
				    }
				}
			);

    };

    //$('.sample4').numeric();
    //$('.sample5').numeric({ allow: "." });
    //$('.sample5').numeric({ length: 3, padding: "0" })
    $.fn.numeric = function (p) {

        var az = "abcdefghijklmnopqrstuvwxyz";
        az += az.toUpperCase();

        p = $.extend({
            nchars: az,
            padding: '0',
            length: 0
        }, p);

        return this.each(function () {
            $(this).alphanumeric(p);
        });

    };

    //$('.sample3').alpha({nocaps:true});
    $.fn.alpha = function (p) {

        var nm = "1234567890";

        p = $.extend({
            nchars: nm
        }, p);

        return this.each(function () {
            $(this).alphanumeric(p);
        }
		);

    };


    //$("#divida_vlComposicao").floatnumber({separator: ",", precision: 2});
    $.fn.floatnumber = function (options) {
        var defaults = {
            separator: ',',
            precision: 2
        };
        options = $.extend(defaults, options);

        return this.each(function () {
            var input = $(this);

            function blur() {
                var re = new RegExp(",", "g");
                s = input.val();
                s = s.replace(re, ".");

                if (s == "") {
                    s = "0";
                }

                if (!isNaN(s)) {
                    n = parseFloat(s);
                    s = n.toFixed(options.precision);

                    input.attr('real-value', s);
                    re2 = new RegExp("\\.", "g");
                    s = s.replace(re2, options.separator);
                    input.val(s);
                }
            }

            input.bind('change blur', blur);
        });
    };

})(jQuery);