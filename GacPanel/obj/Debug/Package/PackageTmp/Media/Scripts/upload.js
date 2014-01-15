$(function () {

    //Subida de archivos asíncrona.
    $('#fileupload').fileupload({
        dataType: 'json',
        done: function (e, data) {

            var result = data.result;

            if (result.Success) {
                alert(result.Data);
            }

            /*$.each(data.result.files, function (index, file) {
            $('<p/>').text(file.name).appendTo(document.body);
            });*/
        }
    });

    //Soporte drag&drop.
    $(document).on('dragenter', function (e) {

    });

    $(document).on('dragover', function (e) {

    });

    $(document).on('drop', function (e) {
        $('#fileupload').trigger('fileupload');
    });
});