Imports Newtonsoft.Json

''' <summary>
''' Contiene métodos para el envío de mensajes
''' en formato JSON al cliente.
''' </summary>
Public NotInheritable Class JsonResponse

    Private Sub New()
    End Sub

    ''' <summary>
    ''' Transmite los resultados de la operación al cliente.
    ''' </summary>
    ''' <param name="response">Objeto de respuesta.</param>
    ''' <param name="obj">Objeto a transmitir.</param>
    Public Shared Sub TransmitOject(ByVal response As HttpResponse, ByVal obj As Object)
        TransmitJson(response, JsonConvert.SerializeObject(obj))
    End Sub

    ''' <summary>
    ''' Transmite el objeto json indicado.
    ''' </summary>
    ''' <param name="response">Objeto de respuesta.</param>
    ''' <param name="json">Texto en formato JSON.</param>
    Public Shared Sub TransmitJson(ByVal response As HttpResponse, ByVal json As String)
        response.ContentType = "application/json; charset=utf-8"
        response.Write(json)
    End Sub

    ''' <summary>
    ''' Transmite el error indicado por la excepción.
    ''' </summary>
    ''' <param name="response">Objeto de respuesta.</param>
    ''' <param name="ex">Excepción cuyo mensaje será transmitido.</param>
    Public Shared Sub TransmitError(ByVal response As HttpResponse, ByVal ex As Exception)
        Dim result As New OperationResult(Of String)(False, ex.ToString())
        JsonResponse.TransmitOject(response, result)
    End Sub

    ''' <summary>
    ''' Transmite un mensaje de OK al cliente.
    ''' </summary>
    ''' <param name="response">Objeto de respuesta.</param>
    Public Shared Sub TransmitOk(ByVal response As HttpResponse)
        Dim result As New OperationResult(Of String)(True, "OK")
        JsonResponse.TransmitOject(response, result)
    End Sub

End Class
