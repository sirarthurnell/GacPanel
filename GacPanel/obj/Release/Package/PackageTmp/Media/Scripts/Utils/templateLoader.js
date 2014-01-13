/// <summary>
/// Módulo que gestiona la carga de plantillas.
/// </summary>
var templateLoader = (function ($) {

    /// <summary>
    /// Carga la plantilla de lista de ensamblados a renderizar.
    /// Después, inicia el renderizado.
    /// </summary>
    /// <param name="templates">Array de URLs de las plantillas
    /// a cargar.</param>
    /// <param name="callback">Función que será llamada una vez
    /// se hayan cargado todas las plantillas.</param>
    function loadTemplates(templates, callback) {
        var downloadPromises = [],
        allPromise;

        $.each(templates, function (index, template) {
            var templatePromise = $.get(template);
            downloadPromises.push(templatePromise);
        });

        allPromise = $.when.apply(null, downloadPromises);
        allPromise.done(function () {
            var args = Array.prototype.slice.call(arguments, 0);

            if (!_.isArray(args[0])) {
                args = [args];
            }

            $.each(args, function (index, value) {
                var templateHtml = value[0];
                $('body').append(templateHtml);
            });

            callback();
        });
    }

    return {
        loadTemplates: loadTemplates
    };

})(jQuery);