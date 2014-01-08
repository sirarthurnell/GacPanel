Imports GacManipulation

''' <summary>
''' Representa un ensamblado que se desea instalar
''' en la GAC.
''' </summary>
Public Class AssemblyToInstall

    Private _path As String
    Private _suggestedDirective As BindingDirective

    ''' <summary>
    ''' Crea una nueva instancia de AssemblyToInstall.
    ''' </summary>
    ''' <param name="path">Ruta al ensamblado.</param>
    Public Sub New(ByVal directiveResolver As DirectiveResolver, ByVal path As String)
        _path = path
        _suggestedDirective = directiveResolver.Suggest(_path)
    End Sub

    ''' <summary>
    ''' Obtiene la ruta al ensamblado que se quiere instalar.
    ''' </summary>
    Public ReadOnly Property Path As String
        Get
            Return _path
        End Get
    End Property

    ''' <summary>
    ''' Obtiene la directiva de enlace sugerida.
    ''' </summary>
    Public ReadOnly Property SuggestedDirective As BindingDirective
        Get
            Return _suggestedDirective
        End Get
    End Property

End Class
