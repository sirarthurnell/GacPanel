''' <summary>
''' Contiene la respuesta resultado de una
''' operación realizada.
''' </summary>
Public Class OperationResult(Of T)

    Private _success As Boolean
    Private _data As T

    ''' <summary>
    ''' Crea una nueva instancia de OperationResult.
    ''' </summary>
    ''' <param name="success">Indica si ha tenido éxito
    ''' la operación.</param>
    ''' <param name="data">Datos asociados a la operación.</param>
    Public Sub New(ByVal success As Boolean, ByVal data As T)
        _success = success
        _data = data
    End Sub

    ''' <summary>
    ''' Devuelve si la operación tuvo éxito.
    ''' </summary>
    Public ReadOnly Property Success As Boolean
        Get
            Return _success
        End Get
    End Property

    ''' <summary>
    ''' Devuelve los datos asociados a la operación.
    ''' </summary>
    Public ReadOnly Property Data As T
        Get
            Return _data
        End Get
    End Property

End Class
