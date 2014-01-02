Imports System.IO
Imports Mono.Cecil

''' <summary>
''' Junta información de un ensamblado.
''' </summary>
Public Class AssemblyInfo

    Private _path As String
    Private _name As String
    Private _fullName As String
    Private _version As String
    Private _publicKeyToken As String
    Private _hasPublicKey As Boolean

    ''' <summary>
    ''' Crea una nueva instancia de AssemblyInfo.
    ''' </summary>
    ''' <param name="path">Ruta al ensamblado.</param>
    Public Sub New(ByVal path As String)
        If Not File.Exists(path) Then
            Throw New ArgumentException("El ensamblado indicado no existe", "assemblyPath")
        End If

        _path = path
        Load()
    End Sub

    ''' <summary>
    ''' Carga los datos del ensamblado.
    ''' </summary>
    Private Sub Load()
        Dim definition As AssemblyDefinition = AssemblyDefinition.ReadAssembly(_path)
        Dim name As AssemblyNameDefinition = definition.Name

        _hasPublicKey = name.HasPublicKey
        _name = name.Name
        _fullName = name.FullName
        _version = name.Version.ToString()
        _publicKeyToken = TranslateToken(name.PublicKeyToken)
    End Sub

    ''' <summary>
    ''' Obtiene la ruta en disco al ensamblado.
    ''' </summary>
    Public ReadOnly Property Path As String
        Get
            Return _path
        End Get
    End Property

    ''' <summary>
    ''' Obtiene si el ensamblado tiene clave pública.
    ''' </summary>
    Public ReadOnly Property HasPublicKey As Boolean
        Get
            Return _hasPublicKey
        End Get
    End Property

    ''' <summary>
    ''' Obtiene el nombre corto del ensamblado.
    ''' </summary>
    Public ReadOnly Property Name As String
        Get
            Return _name
        End Get
    End Property

    ''' <summary>
    ''' Obtiene el nombre largo del ensamblado.
    ''' </summary>
    Public ReadOnly Property FullName As String
        Get
            Return _fullName
        End Get
    End Property

    ''' <summary>
    ''' Obtiene el token público del ensamblado.
    ''' </summary>
    Public ReadOnly Property PublicKeyToken As String
        Get
            Return _publicKeyToken
        End Get
    End Property

    ''' <summary>
    ''' Obtiene la versión del ensamblado.
    ''' </summary>
    Public ReadOnly Property Version As String
        Get
            Return _version
        End Get
    End Property

    ''' <summary>
    ''' Traduce el token especificado a cadena.
    ''' </summary>
    ''' <param name="token">Token a traducir.</param>
    ''' <returns>Token como cadena.</returns>
    Private Function TranslateToken(ByVal token() As Byte) As String
        Dim tokenAsString As String = String.Empty

        For i As Integer = 0 To token.GetUpperBound(0)
            tokenAsString &= String.Format("{0:x2}", token(i))
        Next

        Return tokenAsString
    End Function

End Class
