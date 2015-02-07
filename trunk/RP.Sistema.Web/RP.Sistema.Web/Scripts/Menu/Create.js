$(function () {
    var picker = $('.color').colorpicker();
    $('#dsCor').change(function () {
        picker.colorpicker('setValue', $(this).val());
    });
});
