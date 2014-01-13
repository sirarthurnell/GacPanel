/// <summary>
/// Modelo que representa las directivas de enlace
/// en el cliente.
/// </summary>
bindingDirective = (function ($) {

    var versionMixing = {
        VersionAsString: function () {
            return this.Parts.join('.');
        },
        setVersionAsString: function (newVersion) {
            var newPartsAsString = newVersion.split('.'),
                newParts = [];

            function isNumber(n) {
                return !isNaN(parseFloat(n)) && isFinite(n);
            }

            $.each(newPartsAsString, function (index, part) {
                if (isNumber(part)) {
                    newParts.push(parseInt(part));
                } else {
                    throw new Error('La cadena ' + newVersion + ' no es un número de versión válido.');
                }
            });

            this.Parts = newParts;
        }
    };

    var list = null,
        redirectionId = 0;

    /// <summary>
    /// Obtiene la lista de directivas de enlace.
    /// </summary>
    function getList() {
        return list;
    }

    /// <summary>
    /// Agrega los ensamblados propuestos para la
    /// instalación.
    /// </summary>
    /// <param name="directivesList">Lista de directivas
    /// de enlace propuestas para la instalación.</param>
    function installAssemblies(data) {
        adaptModel(data);
        $.each(data.Data, function (index, directive) {

            var directivesWithName,
                existentDirective,
                existentVersion,
                newVersion = directive.Redirections[0].TargetVersion,
                i, j, installedVersionsCopy;

            directivesWithName = list({ 'Name': directive.Name }).get();

            if (directivesWithName.length > 0) {

                existentDirective = directivesWithName[0];
                installedVersionsCopy = existentDirective.InstalledVersions.slice();
                for (i = 0; i < installedVersionsCopy.length; i++) {
                    existentVersion = installedVersionsCopy[i];

                    if (existentVersion.VersionAsString() != newVersion.VersionAsString()) {
                        existentDirective.InstalledVersions.push(newVersion);
                        existentDirective.Redirections.push(directive.Redirections[0]);
                        existentDirective.State = 'NewInstall';
                    } else {
                        throw new Error('No se permite más de una instalación de la misma versión de un ensamblado. Escoja otra versión del ensamblado ' + existentDirective.Name + '.');
                    }
                }

            } else {
                list.insert(directive);
                directive.State = 'NewInstall';
            }

        });

        triggerEvent('assembliesInstalled');
    }

    /// <summary>
    /// Aplica los cambios realizados en el servidor.
    /// </summary>
    /// <param name="successCallback">Función a llamar
    /// cuando se aplican los cambios con éxito.</param>
    /// <param name="failureCallback">Función a llamar
    /// cuando falla la conexión con el servidor.</param>
    function applyChanges(successCallback, failureCallback) {
        var data = list().get();
        server.applyChanges(data, successCallback, failureCallback);
    }

    /// <summary>
    /// Carga todas las directivas de enlace existentes
    /// en la GAC.
    /// </summary>
    /// <param name="successCallback">Función a ejecutar
    /// cuando la carga de directivas tiene éxito.</param>
    /// <param name="failureCallback">Función a llamar
    /// cuando la carga de directivas falla.</param>
    function loadAll(successCallback, failureCallback) {

        server.recoverDirectives(

            function success(data) {
                var shortedList;

                adaptModel(data);
                list = TAFFY(data.Data);
                shortedList = list().order('Name').get();
                successCallback({ Data: shortedList });
            },

            failureCallback

        );

    }

    /// <summary>
    /// Elimina el ensamblado indicado.
    /// </summary>
    /// <param name="assemblyName">Nombre del ensamblado
    /// que se quiere eliminar.</param>
    /// <param name="selectedVersion">Versión a eliminar
    /// seleccionada.</param>
    function deleteAssembly(assemblyName, selectedVersion) {
        var directive = list({ 'Name': assemblyName }).get()[0];

        $.each(directive.InstalledVersions, function (index, version) {
            if (version.VersionAsString() == selectedVersion) {
                version.Selected = true;
            }
        });

        directive.State = "Removed";

        triggerEvent('assemblyDeleted');
    }

    /// <summary>
    /// Crea una nueva redirección para la directiva de
    /// enlace indicada.
    /// </summary>
    /// <param name="assemblyName">Nombre del ensamblado
    /// al que se le quiere crear una redirección.</param>
    function createRedirection(assemblyName) {
        var directive = list({ 'Name': assemblyName }).get()[0],
            newRedirection = {
                "Range": {
                    "LowerBound": {
                        "Parts": []
                    },
                    "UpperBound": {
                        "Parts": []
                    }
                },
                "TargetVersion": {
                    "Parts": []
                }
            };

        extendRedirection(newRedirection);
        newRedirection.Range.LowerBound.setVersionAsString('0.0.0.0');
        newRedirection.Range.UpperBound.setVersionAsString('0.0.0.0');
        newRedirection.TargetVersion.setVersionAsString('0.0.0.0');
        directive.Redirections.push(newRedirection);
        directive.State = "Changed";

        triggerEvent('redirectionCreated', directive, newRedirection);
    }



    /// <summary>
    /// Actualiza la versión de destino de la redirección.
    /// </summary>
    /// <param name="assemblyName">Nombre del ensamblado.</param>
    /// <param name="redirectionId">Id de la redirección.</param>
    /// <param name="newVersion">Nueva versión de destino.</param>
    function updateTargetVersion(assemblyName, redirectionId, newVersion) {
        var combo = findRedirection(assemblyName, redirectionId);
        combo.redirection.TargetVersion.setVersionAsString(newVersion);
        combo.directive.State = "Changed";

        triggerEvent('targetVersionUpdated', combo.directive, combo.redirection);
    }

    /// <summary>
    /// Actualiza el límite inferior de la redirección.
    /// </summary>
    /// <param name="assemblyName">Nombre del ensamblado.</param>
    /// <param name="redirectionId">Id de la redirección.</param>
    /// <param name="newVersion">Nueva versión del límite superior.</param>
    function updateLowerBound(assemblyName, redirectionId, newVersion) {
        var combo = findRedirection(assemblyName, redirectionId);
        combo.redirection.Range.LowerBound.setVersionAsString(newVersion);
        combo.directive.State = "Changed";

        triggerEvent('lowerBoundUpdated', combo.directive, combo.redirection);
    }

    /// <summary>
    /// Actualiza el límite superior de la redirección.
    /// </summary>
    /// <param name="assemblyName">Nombre del ensamblado.</param>
    /// <param name="redirectionId">Id de la redirección.</param>
    /// <param name="newVersion">Nueva versión del límite superior.</param>
    function updateUpperBound(assemblyName, redirectionId, newVersion) {
        var combo = findRedirection(assemblyName, redirectionId);
        combo.redirection.Range.UpperBound.setVersionAsString(newVersion);
        combo.directive.State = "Changed";

        triggerEvent('upperBoundUpdated', combo.directive, combo.redirection);
    }

    /// <summary>
    /// Elimina la redirección.
    /// </summary>
    /// <param name="assemblyName">Nombre del ensamblado.</param>
    /// <param name="redirectionId">Id de la redirección.</param>
    function deleteRedirection(assemblyName, redirectionId) {
        var directive = list({ 'Name': assemblyName }).get()[0],
            redirectionToDelete,
            indexToDelete, i;

        for (i = 0; i < directive.Redirections.length; i++) {
            redirectionToDelete = directive.Redirections[i];
            if (redirectionToDelete.Id === redirectionId) {
                directive.Redirections.splice(i, 1);
            }
        }

        directive.State = "Changed";

        triggerEvent('redirectionDeleted', directive, redirectionToDelete);
    }

    /// <summary>
    /// Encuentra la redirección a editar, dado el nombre
    /// del ensamblado y el id de la redirección.
    /// </summary>
    /// <param name="assemblyName">Nombre del ensamblado.</param>
    /// <param name="redirectionId">Id de la redirección.</param>
    function findRedirection(assemblyName, redirectionId) {
        var directive = list({ 'Name': assemblyName }).get()[0],
            selectedRedirection;

        $.each(directive.Redirections, function (index, redirection) {
            if (redirection.Id === redirectionId) {
                selectedRedirection = redirection;
                return false;
            }
        });

        return {
            directive: directive,
            redirection: selectedRedirection
        };
    }

    /// <summary>
    /// Añade la propiedad de versión formateada
    /// a las directivas de enlace recuperadas desde
    /// el servidor, y la propiedad de estado.
    /// </summary>
    function adaptModel(data) {
        var directives = data.Data;

        $.each(directives, function (index, bindingDirective) {

            bindingDirective.State = "Unchanged";

            $.each(bindingDirective.InstalledVersions, function (index, version) {
                $.extend(version, versionMixing);
                version.Selected = false;
            });

            $.each(bindingDirective.Redirections, function (index, redirection) {
                extendRedirection(redirection);
            });

        });
    }

    /// <summary>
    /// Cambia el objeto de redirección para añadirle
    /// más capacidades.
    /// </summary>
    function extendRedirection(redirection) {
        redirectionId++;
        redirection.Id = redirectionId.toString();
        $.extend(redirection.TargetVersion, versionMixing);
        $.extend(redirection.Range.LowerBound, versionMixing);
        $.extend(redirection.Range.UpperBound, versionMixing);
    }

    /// <summary>
    /// Lanza el evento indicado. Los argumentos
    /// se pasan como parámetros al evento a lanzar.
    /// </summary>
    function triggerEvent() {
        var args = Array.prototype.slice.call(arguments, 0),
            doc = $(document);

        doc.trigger.apply(doc, args);
    }

    return {
        installAssemblies: installAssemblies,
        loadAll: loadAll,
        applyChanges: applyChanges,
        updateTargetVersion: updateTargetVersion,
        updateLowerBound: updateLowerBound,
        updateUpperBound: updateUpperBound,
        deleteRedirection: deleteRedirection,
        createRedirection: createRedirection,
        deleteAssembly: deleteAssembly,
        getList: getList
    };

})(jQuery);