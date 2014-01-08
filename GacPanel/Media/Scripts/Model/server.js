/// <summary>
/// Sirve de interfaz entre el servidor
/// y el cliente.
/// </summary>
var server = (function () {

    /// <summary>
    /// Recupera la lista de directivas de enlace del
    /// servidor.
    /// </summary>
    /// <param name="successCallback">Función a llamar
    /// cuando se recupera la lista con éxito.</param>
    /// <param name="failureCallback">Función a llamar
    /// cuando falla la conexión con el servidor.</param>
    function recoverDirectives(successCallback, failureCallback) {
        var recoverPromise = $.post('Handlers/DirectivesListHandler.ashx');
        recoverPromise.done(successCallback);
        recoverPromise.fail(failureCallback);
    }

    /// <summary>
    /// Aplica los cambios realizados en el servidor.
    /// </summary>
    /// <param name="data">Datos a enviar al servidor.</param>
    /// <param name="successCallback">Función a llamar
    /// cuando se aplican los cambios con éxito.</param>
    /// <param name="failureCallback">Función a llamar
    /// cuando falla la conexión con el servidor.</param>
    function applyChanges(data, successCallback, failureCallback) {
        var applyPromise = $.ajax({
            url: 'Handlers/ApplyChangesHandler.ashx',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            dataType: 'json'
        });

        applyPromise.done(successCallback);
        applyPromise.fail(failureCallback);
    }

    return {
        recoverDirectives: recoverDirectives,
        applyChanges: applyChanges
    };

})();