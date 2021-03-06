﻿Imports System.IO

''' <summary>
''' Representa la caché de ensamblados global.
''' </summary>
Public Class Gac

    Private _gacutil As GacUtil
    Private _assemblies As List(Of AssemblyInfo)

    ''' <summary>
    ''' Crea una nueva instancia de Gac.
    ''' </summary>
    ''' <param name="gacutil">Herramienta
    ''' GacUtil a utilizar.</param>
    Public Sub New(ByVal gacutil As GacUtil)
        _gacutil = gacutil
    End Sub

    ''' <summary>
    ''' Instala el ensamblado especificado.
    ''' </summary>
    ''' <param name="path">Ruta al ensamblado a instalar.</param>
    Public Sub InstallAssembly(ByVal path As String)
        Dim args As String = String.Format("/i ""{0}""", path)
        _gacutil.Execute(args)
    End Sub

    ''' <summary>
    ''' Instala los ensamblados especificados.
    ''' </summary>
    ''' <param name="paths">Rutas a los ensamblados
    ''' a instalar.</param>
    Public Sub InstallAssemblies(ByVal paths As IEnumerable(Of String))
        Dim tempFile As String = Path.GetTempFileName()

        Using sw As New StreamWriter(tempFile)
            For Each currentPath In paths
                sw.WriteLine(currentPath)
            Next
        End Using

        Dim args As String = String.Format("/il ""{0}""", tempFile)
        _gacutil.Execute(args)

        If File.Exists(tempFile) Then
            File.Delete(tempFile)
        End If
    End Sub

    ''' <summary>
    ''' Quita el ensamblado especificado.
    ''' </summary>
    ''' <param name="name">Nombre del ensamblado.</param>
    ''' <param name="publicKeyToken">Token público.</param>
    ''' <param name="version">Versión.</param>
    Public Sub UnistallAssembly(ByVal name As String, ByVal publicKeyToken As String, ByVal version As String)
        Dim args As String = String.Format("/u {0},Version={1},Culture=neutral,PublicKeyToken={2}", name, version, publicKeyToken)
        _gacutil.Execute(args)
    End Sub

    ''' <summary>
    ''' Obtiene la lista de versiones instaladas
    ''' en la GAC para el ensamblado indicado.
    ''' </summary>
    ''' <param name="name">Nombre corto del ensamblado.</param>
    ''' <returns>Lista de versiones instaladas en la GAC.</returns>
    Public Function GetVersionsForAssembly(ByVal name As String) As List(Of String)
        Dim list = GetAssembliesList()
        Dim versions = From assembly In list
                       Where String.Compare(assembly.Name, name, StringComparison.InvariantCultureIgnoreCase) = 0
                       Select assembly.Version

        Return versions.ToList()
    End Function

    ''' <summary>
    ''' Obtiene la lista de ensamblados de la GAC.
    ''' </summary>
    ''' <returns>Lista de ensamblados de la GAC.</returns>
    Public Function GetAssembliesList() As List(Of AssemblyInfo)
        If _assemblies Is Nothing Then

            _assemblies = New List(Of AssemblyInfo)()
            Dim text As String = _gacutil.Execute("/l")
            Dim fullNamesList() As String = text.Split(vbCrLf)

            For i As Integer = 4 To fullNamesList.GetUpperBound(0) - 3
                Dim fullName As String = fullNamesList(i)
                Dim currentInfo As AssemblyInfo = AssemblyInfo.CreateFromFullName(fullName)
                _assemblies.Add(currentInfo)
            Next

        End If

        Return _assemblies
    End Function

    ''' <summary>
    ''' Solicita que se vuelvan a cargar los
    ''' datos de la GAC.
    ''' </summary>
    Public Sub Reset()
        _assemblies = Nothing
    End Sub

End Class
