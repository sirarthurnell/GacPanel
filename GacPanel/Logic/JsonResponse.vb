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

End Class
