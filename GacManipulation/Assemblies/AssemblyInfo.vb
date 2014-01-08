Imports System.IO
Imports Mono.Cecil

''' <summary>
''' Junta información de un ensamblado.
''' </summary>
Public Class AssemblyInfo

    Private _name As String
    Private _fullName As String
    Private _version As String
    Private _publicKeyToken As String
    Private _hasPublicKey As Boolean

    ''' <summary>
    ''' Crea una nueva instancia de AssemblyInfo.
    ''' </summary>
    Protected Sub New()
    End Sub

    ''' <summary>
    ''' Crea una nueva instancia de AssemblyInfo a partir
    ''' de un nombre completo de ensamblado.
    ''' </summary>
    ''' <param name="fullName">Nombre completo del ensamblado.</param>
    ''' <returns>Información del ensamblado.</returns>
    Public Shared Function CreateFromFullName(ByVal fullName As String) As AssemblyInfo
        Dim newAssemblyInfo As New AssemblyInfo()
        newAssemblyInfo.FullName = fullName.Trim()

        Dim parts() As String = newAssemblyInfo.FullName.Split(",")

        newAssemblyInfo.Name = parts(0).Trim()
        For i As Integer = 0 To parts.GetUpperBound(0)
            Dim current As String = parts(i).Trim()

            If current.StartsWith("Version=") Then
                newAssemblyInfo.Version = current.Split("=")(1).Trim()
            End If

            If current.StartsWith("PublicKeyToken=") Then
                newAssemblyInfo.PublicKeyToken = current.Split("=")(1).Trim()
                newAssemblyInfo.HasPublicKey = True
            End If
        Next

        Return newAssemblyInfo
    End Function

    ''' <summary>
    ''' Carga los datos del ensamblado.
    ''' </summary>
    ''' <param name="path">Ruta en disco del ensamblado.</param>
    Public Shared Function Load(ByVal path As String) As AssemblyInfo
        If Not File.Exists(path) Then
            Throw New ArgumentException("El ensamblado indicado no existe", "assemblyPath")
        End If

        Dim definition As AssemblyDefinition = AssemblyDefinition.ReadAssembly(path)
        Dim name As AssemblyNameDefinition = definition.Name
        Dim newAssemblyInfo As New AssemblyInfo()

        newAssemblyInfo.HasPublicKey = name.HasPublicKey
        newAssemblyInfo.Name = name.Name
        newAssemblyInfo.FullName = name.FullName
        newAssemblyInfo.Version = name.Version.ToString()
        newAssemblyInfo.PublicKeyToken = TranslateToken(name.PublicKeyToken)
        Return newAssemblyInfo
    End Function

    ''' <summary>
    ''' Obtiene si el ensamblado tiene clave pública.
    ''' </summary>
    Public Property HasPublicKey As Boolean
        Get
            Return _hasPublicKey
        End Get
        Private Set(value As Boolean)
            _hasPublicKey = value
        End Set
    End Property

    ''' <summary>
    ''' Obtiene el nombre corto del ensamblado.
    ''' </summary>
    Public Property Name As String
        Get
            Return _name
        End Get
        Private Set(value As String)
            _name = value
        End Set
    End Property

    ''' <summary>
    ''' Obtiene el nombre largo del ensamblado.
    ''' </summary>
    Public Property FullName As String
        Get
            Return _fullName
        End Get
        Private Set(value As String)
            _fullName = value
        End Set
    End Property

    ''' <summary>
    ''' Obtiene el token público del ensamblado.
    ''' </summary>
    Public Property PublicKeyToken As String
        Get
            Return _publicKeyToken
        End Get
        Private Set(value As String)
            _publicKeyToken = value
        End Set
    End Property

    ''' <summary>
    ''' Obtiene la versión del ensamblado.
    ''' </summary>
    Public Property Version As String
        Get
            Return _version
        End Get
        Private Set(value As String)
            _version = value
        End Set
    End Property

    ''' <summary>
    ''' Traduce el token especificado a cadena.
    ''' </summary>
    ''' <param name="token">Token a traducir.</param>
    ''' <returns>Token como cadena.</returns>
    Private Shared Function TranslateToken(ByVal token() As Byte) As String
        Dim tokenAsString As String = String.Empty

        For i As Integer = 0 To token.GetUpperBound(0)
            tokenAsString &= String.Format("{0:x2}", token(i))
        Next

        Return tokenAsString
    End Function

End Class
