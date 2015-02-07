$(function () {
    'use strict';

    $('#dv-msg').hide();

    $('#btn-msg').click(function () {
        $('#dv-msg').hide();
    });

    $('#btn-gerar-sprite').click(function () {
        $('#dv-msg').show();
    });
    // Initialize the jQuery File Upload widget:
    var upload = $('#fileupload').fileupload({
        acceptFileTypes: /(\.|\/)(jpe?g|png)$/i
    });
    var upload_in_progress = false;

    upload.bind('fileuploadstart', function (e, data) {
        upload_in_progress = true;
    });

    upload.bind('fileuploadstop', function (e, data) {
        upload_in_progress = false;
    });

    // Load existing files:
    $.getJSON($('#fileupload').attr('data-url'), function (files) {
        var fu = $('#fileupload').data('fileupload');
        fu._adjustMaxNumberOfFiles(-files.length);
        fu._renderDownload(files)
            .appendTo($('#fileupload .files'))
            .fadeIn(function () {
                $(this).show().addClass('in');
            });
    });
});
