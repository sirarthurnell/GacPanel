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
        var recoverPromise = $.post('DirectivesListHandler.ashx');
        recoverPromise.done(successCallback);
        recoverPromise.fail(failureCallback);
    }

    return {
        recoverDirectives: recoverDirectives
    };

})();