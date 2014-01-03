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

    var list = null;

    /// <summary>
    /// Obtiene la lista de directivas de enlace.
    /// </summary>
    function getList() {
        return list;
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

                addFormattedVersion(data);
                list = TAFFY(data.Data);
                shortedList = list().order('Name').get();
                successCallback({ Data: shortedList });
            },

            failureCallback

        );

    }

    /// <summary>
    /// Actualiza el límite inferior de la redirección.
    /// </summary>
    /// <param name="assemblyName">Nombre del ensamblado.</param>
    /// <param name="redirectionId">Id de la redirección.</param>
    /// <param name="newVersion">Nueva versión del límite superior.</param>
    function updateLowerBound(assemblyName, redirectionId, newVersion) {
        var redirection = findRedirection(assemblyName, redirectionId);
        redirection.Range.LowerBound.setVersionAsString(newVersion);
    }

    /// <summary>
    /// Actualiza el límite superior de la redirección.
    /// </summary>
    /// <param name="assemblyName">Nombre del ensamblado.</param>
    /// <param name="redirectionId">Id de la redirección.</param>
    /// <param name="newVersion">Nueva versión del límite superior.</param>
    function updateUpperBound(assemblyName, redirectionId, newVersion) {
        var redirection = findRedirection(assemblyName, redirectionId);
        redirection.Range.UpperBound.setVersionAsString(newVersion);
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

        return selectedRedirection;
    }

    /// <summary>
    /// Añade la propiedad de versión formateada
    /// a las directivas de enlace recuperadas desde
    /// el servidor.
    /// </summary>
    function addFormattedVersion(data) {
        var directives = data.Data,
            redirectionId = 0;

        $.each(directives, function (index, bindingDirective) {

            $.each(bindingDirective.Redirections, function (index, redirection) {
                redirectionId++;
                redirection.Id = redirectionId.toString();
                $.extend(redirection.TargetVersion, versionMixing);
                $.extend(redirection.Range.LowerBound, versionMixing);
                $.extend(redirection.Range.UpperBound, versionMixing);
            });

        });
    }

    return {
        loadAll: loadAll,
        updateLowerBound: updateLowerBound,
        updateUpperBound: updateUpperBound,
        deleteRedirection: deleteRedirection,
        getList: getList
    };

})();