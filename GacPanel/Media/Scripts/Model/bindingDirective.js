/// <summary>
/// Modelo que representa las directivas de enlace
/// en el cliente.
/// </summary>
bindingDirective = (function () {

    var versionMixing = {
        VersionAsString: function () {
            return this.Parts.join('.');
        },
        setVersionAsString: function (newVersion) {
            var newPartsAsString = newVersion.split('.'),
                newParts = [];

            //TODO validar la entrada del usuario.

            $.each(newPartsAsString, function (index, part) {
                newParts.push(parseInt(part));
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
        console.log(data);
        $.each(data.Data, function (index, directive) {
            list.insert(directive);
            directive.State = 'NewInstall';
        });
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
    function deleteAssembly(assemblyName) {
        var directive = list({ 'Name': assemblyName }).get()[0];
        directive.State = "Removed";
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

})();