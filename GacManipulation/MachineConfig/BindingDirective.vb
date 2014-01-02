''' <summary>
''' Representa una directiva de enlace.
''' </summary>
Public Class BindingDirective

    Private _name As String
    Private _token As String
    Private _redirections As New List(Of BindingRedirect)()

    ''' <summary>
    ''' Crea una nueva instancia de BindingDirective.
    ''' </summary>
    ''' <param name="name">Nombre del ensamblado.</param>
    ''' <param name="token">Token del ensamblado.</param>
    Public Sub New(ByVal name As String, ByVal token As String)
        _name = name
        _token = token
    End Sub

    ''' <summary>
    ''' Obtiene el nombre del ensamblado.
    ''' </summary>
    Public ReadOnly Property Name As String
        Get
            Return _name
        End Get
    End Property

    ''' <summary>
    ''' Obtiene el token público del ensamblado.
    ''' </summary>
    Public ReadOnly Property Token As String
        Get
            Return _token
        End Get
    End Property

    ''' <summary>
    ''' Obtiene la lista de redirecciones de esta
    ''' directiva de enlace.
    ''' </summary>
    Public ReadOnly Property Redirections As List(Of BindingRedirect)
        Get
            Return _redirections
        End Get
    End Property

    ''' <summary>
    ''' Obtiene una representación en cadena de
    ''' la directiva de enlace actual.
    ''' </summary>
    ''' <returns>Representación en cadena de
    ''' la directiva de enlace actual.</returns>
    Public Overrides Function ToString() As String
        Return String.Format("Name={0}, Token={1}", Name, Token)
    End Function

End Class
