﻿(function ($) {

    yepnope({
        load: [
            'Media/Scripts/Lib/taffy-min.js',
            'Media/Scripts/Lib/underscore-min.js',
            'Media/Scripts/Lib/mustache.js',
            'Media/Scripts/Lib/jquery.ui.widget.js',
            'Media/Scripts/Lib/jquery.iframe-transport.js',
            'Media/Scripts/Lib/jquery.fileupload.js',
            'Media/Scripts/Utils/templateLoader.js',
            'Media/Scripts/Model/server.js',
            'Media/Scripts/Model/bindingDirective.js'
          ],

        complete: function () {
            var templates = [
                'Templates/directivesListTemplate.htm'
            ];

            $(function () {
                templateLoader.loadTemplates(templates, loadList);
            });
        }
    });

    /// <summary>
    /// Carga la lista de ensamblados y la muestra.
    /// </summary>
    function loadList() {
        bindingDirective.loadAll(
            function success() {
                unsetModelHandlers();
                showApplyChanges(false);
                showList();
                setModelHandlers();
                removePleaseWait();
            },
            function failure() {
                alert('No se ha podido cargar la lista de ensamblados.');
            }
        );
    }

    /// <summary>
    /// Quita el mensaje de por favor, espere.
    /// </summary>
    function removePleaseWait() {
        $('#firstWait').fadeOut('slow');
    }

    /// <summary>
    /// Fija los manejadores de eventos del modelo.
    /// </summary>
    function setModelHandlers() {
        $(document).on(
            [
                'assembliesInstalled',
                'assemblyDeleted',
                'redirectionCreated',
                'redirectionDeleted'
            ].join(" "),
            function () {
                showList();
                showApplyChanges(true);
            }
        );
    }

    /// <summary>
    /// Muestra u oculta el botón de aplicar cambios.
    /// </summary>
    /// <param name="show">True para mostrar el
    /// botón. False para ocultarlo.</param>
    function showApplyChanges(show) {
        var applyChanges = $('.applyChanges');

        if (show) {
            applyChanges
                .css('opacity', '0')
                .css('visibility', 'visible')
                .animate({
                    opacity: 1
                }, 500);
        } else {
            applyChanges
                .css('opacity', '1')
                .animate({
                    opacity: 0
                }, 500, function () {
                    applyChanges.css('visibility', 'hidden');
                });
        }
    }

    /// <summary>
    /// Elimina los manejadores de eventos del modelo.
    /// </summary>
    function unsetModelHandlers() {
        $(document).off(
            [
                'assembliesInstalled',
                'assemblyDeleted',
                'redirectionCreated',
                'redirectionDeleted'
            ].join(" "));
    }

    /// <summary>
    /// Muestra la lista de ensamblados.
    /// </summary>
    function showList() {
        var assembliesListHtml,
        templateHtml = $('#directivesListTemplate').html(),
        directivesList = bindingDirective.getList()().get(),
        data = { Data: directivesList };

        eraseInterfaceEventHandlers();

        assembliesListHtml = Mustache.render(templateHtml, data);
        $('#assembliesListContainer')
            .empty()
            .html(assembliesListHtml);

        addInterfaceEventHandlers();
    }

    /// <summary>
    /// Establece los manejadores de eventos de la
    /// lista de ensamblados.
    /// </summary>
    function addInterfaceEventHandlers() {

        //Eventos para los controles de versión.
        $('.versionControl').on('click', function setVersionControlClick() {
            var versionInput = $('<input type="text" class="versionControl">'),
            currentCell = $(this),
            oldVersion = currentCell.html();

            currentCell.off('click');
            versionInput.val(oldVersion);
            currentCell.html('');
            currentCell.append(versionInput);
            versionInput.focus();

            versionInput.on('blur', function () {

                var redirectionId = currentCell.attr('data-id'),
                newVersion = versionInput.val(),
                assemblyName = currentCell.attr('data-assembly-name');

                if (oldVersion != newVersion) {

                    try {

                        if (currentCell.hasClass('lowerBound')) {
                            bindingDirective.updateLowerBound(assemblyName, redirectionId, newVersion);
                        } else if (currentCell.hasClass('upperBound')) {
                            bindingDirective.updateUpperBound(assemblyName, redirectionId, newVersion);
                        } else {
                            bindingDirective.updateTargetVersion(assemblyName, redirectionId, newVersion);
                        }

                        currentCell.html(newVersion);

                        currentCell
                            .closest('.redirections')
                            .prev('.bindingDirective')
                            .addClass('Changed');

                    } catch (err) {
                        versionInput.addClass('versionError');
                        alert(err.message);
                        currentCell.html(oldVersion);
                    } finally {
                        versionInput.remove();
                        currentCell.on('click', setVersionControlClick);
                    }

                } else {
                    versionInput.remove();
                    currentCell.html(oldVersion);
                    currentCell.on('click', setVersionControlClick);
                }

            });
        });

        //Eventos para el botón de borrado de redirecciones.
        $('.deleteButton').on('click', function () {
            var deleteButton = $(this),
            assemblyName = deleteButton.attr('data-assembly-name'),
            redirectionId = deleteButton.attr('data-id');

            bindingDirective.deleteRedirection(assemblyName, redirectionId);
        });

        //Eventos para el botón de adición de redirecciones.
        $('.addButton').on('click', function () {
            var assemblyName = $(this).attr('data-assembly-name');
            bindingDirective.createRedirection(assemblyName);
        });

        //Eventos para el botón de eliminación de ensamblado.
        $('.deleteAssemblyButton').on('click', function () {
            var deleteAssemblyButton = $(this),
                assemblyName = deleteAssemblyButton.attr('data-assembly-name'),
                version = deleteAssemblyButton
                    .closest('.bindingDirective')
                    .find('.installedVersion:selected')
                    .val();

            bindingDirective.deleteAssembly(assemblyName, version);
        });

        //Eventos para el botón de descartar cambios.
        $('.discardChanges').on('click', function () {
            loadList();
        });

        //Eventos para el botón de aplicar cambios en el servidor.
        $('.applyChanges').on('click', function () {
            showApplyChanges(false);

            bindingDirective.applyChanges(function success(result) {
                if (result.Success) {
                    alert('Cambios aplicados con éxito.');
                    loadList();
                } else {
                    alert(result.Data);
                }
            },
            function failure() {
                alert('Fallo al aplicar los cambios en el servidor.');
            });
        });

        //Soporte drag&drop.
        var dragdropTarget = getDragDropTarget();
        dragdropTarget.on('dragenter', function (e) {
            dragdropTarget.addClass('dragdrop');
        });

        dragdropTarget.on('dragover', function (e) {
            if (!dragdropTarget.hasClass('dragdrop')) {
                dragdropTarget.addClass('dragdrop');
            }
        });

        dragdropTarget.on('dragleave', function (e) {
            dragdropTarget.removeClass('dragdrop');
        });

        dragdropTarget.on('drop', function (e) {
            dragdropTarget.removeClass('dragdrop');
            $('#fileupload').trigger('fileupload');
        });

        //Subida de archivos asíncrona.
        $('#fileupload').fileupload({
            dataType: 'json',
            done: function (e, data) {
                var result = data.result;

                if (result.Success) {

                    try {

                        bindingDirective.installAssemblies(result);
                        showList();

                    } catch (err) {
                        alert(err.message);
                    }

                } else {
                    alert(result.Data);
                }
            }
        });
    }

    /// <summary>
    /// Obtiene el destino de los elementos
    /// drag&drop.
    /// </summary>
    function getDragDropTarget() {
        return $('body');
    }

    /// <summary>
    /// Elimina los manejadores de eventos de la
    /// lista de ensamblados.
    /// </summary>
    function eraseInterfaceEventHandlers() {
        $('.versionControl').off('click');
        $('.addButton').off('click');
        $('.deleteButton').off('click');
        $('.discardChanges').off('click');
        $('.applyChanges').off('click');

        var dragdropTarget = getDragDropTarget();
        dragdropTarget.off('dragenter');
        dragdropTarget.off('dragleave');
        dragdropTarget.off('drop');
    }

})(jQuery);
