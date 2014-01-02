yepnope({
    load: [
            'Media/Styles/directivesList.css',
            'Media/Scripts/Lib/taffy-min.js',
            'Media/Scripts/Lib/underscore-min.js',
            'Media/Scripts/Lib/mustache.js',
            'Media/Scripts/Lib/jquery-1.10.2.min.js',
            'Media/Scripts/Utils/templateLoader.js',
            'Media/Scripts/Model/server.js',
            'Media/Scripts/Model/bindingDirective.js'
          ],

    complete: function () {
        var templates = [
            'Templates/directivesListTemplate.htm'
        ];

        $(function () {
            templateLoader.loadTemplates(templates, showList);
        });
    }
});

/// <summary>
/// Muestra la lista de ensamblados.
/// </summary>
function showList() {
    var templateHtml = $('#directivesListTemplate').html();

    Mustache.parse(templateHtml);

    bindingDirective.loadAll(
        function success(data) {
            var assembliesListHtml;
            assembliesListHtml = Mustache.render(templateHtml, data);
            $('#assembliesListContainer').html(assembliesListHtml);

            addEventHandlers();
        },
        function failure() {
            alert('No se ha podido cargar la lista de ensamblados.');
        }
    );
}

/// <summary>
/// Establece los manejadores de eventos de la
/// lista de ensamblados.
/// </summary>
function addEventHandlers() {

    ///Eventos para los controles de versión.
    $('.versionControl').on('click', function setVersionControlClick() {
        var versionInput = $('<input type="text" class="versionControl">'),
            currentCell = $(this);

        currentCell.off('click');
        versionInput.val(currentCell.html());
        currentCell.html('');
        currentCell.append(versionInput);
        versionInput.focus();

        versionInput.on('blur', function () {

            var redirectionId = currentCell.attr('data-id'),
                newVersion = versionInput.val(),
                assemblyName = currentCell.attr('data-assembly-name');

            if (currentCell.hasClass('lowerBound')) {
                bindingDirective.updateLowerBound(assemblyName, redirectionId, newVersion);
            } else {
                bindingDirective.updateUpperBound(assemblyName, redirectionId, newVersion);
            }            

            currentCell.html(newVersion);
            versionInput.remove();
            currentCell.on('click', setVersionControlClick);
        });
    });

    ///Eventos para el botón de borrado de redirecciones.
    $('.deleteButton').on('click', function () {
        var assemblyName = $(this).attr('data-assembly-name');

        $('[data-assembly-name="' + assemblyName + '"]:checked').each(function () {
            var checkbox = $(this),
                redirectionId = checkbox.attr('data-id'),
                redirectionTr = checkbox.closest('tr');

            bindingDirective.deleteRedirection(assemblyName, redirectionId);
            redirectionTr.remove();
        });
    });
}
