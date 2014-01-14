Imports Mono.Cecil
Imports System.IO

''' <summary>
''' Se encarga de comparar y sugerir directivas de
''' enlace a aplicar a un ensamblado.
''' </summary>
Public Class DirectiveResolver

    Private _directiveSource As IBindingDirectiveSource

    ''' <summary>
    ''' Crea una nueva instancia de DirectiveResolver.
    ''' </summary>
    ''' <param name="directiveSource">Fuente de directivas
    ''' de enlace.</param>
    Public Sub New(ByVal directiveSource As IBindingDirectiveSource)
        _directiveSource = directiveSource
    End Sub

    ''' <summary>
    ''' Recupera y aplica la directiva de enlace automáticamente
    ''' sugerida para el ensamblado especificado.
    ''' </summary>
    ''' <param name="assemblyPath">Ruta al ensamblado.</param>
    ''' <returns>Directiva de enlace sugerida y aplicada.</returns>
    Public Function SuggestAndApply(ByVal assemblyPath As String) As BindingDirective
        Dim directiveToApply As BindingDirective = Suggest(assemblyPath)
        Dim directives = _directiveSource.Directives
        Dim lastIndex As Integer = directives.Count - 1

        For i As Integer = 0 To lastIndex
            If String.Compare(directives(i).Name, directiveToApply.Name, StringComparison.InvariantCultureIgnoreCase) = 0 Then
                directives(i) = directiveToApply
            End If
        Next

        Return directiveToApply
    End Function

    ''' <summary>
    ''' Sugiere una directiva de enlace a aplicar para el
    ''' ensamblado indicado.
    ''' </summary>
    ''' <param name="assemblyPath">Ruta al ensamblado.</param>
    ''' <returns>Directiva de enlace sugerida.</returns>
    Public Function Suggest(ByVal assemblyPath As String) As BindingDirective
        If Not File.Exists(assemblyPath) Then
            Throw New ArgumentException("El ensamblado indicado no existe", "assemblyPath")
        End If

        Dim info As AssemblyInfo = AssemblyInfo.Load(assemblyPath)
        Dim currentDirective = (From directive In _directiveSource.Directives
                               Where String.Compare(directive.Name, info.Name, StringComparison.InvariantCultureIgnoreCase) = 0).FirstOrDefault()

        Dim directiveToUse As BindingDirective
        If currentDirective Is Nothing Then
            directiveToUse = New BindingDirective(info.Name, info.PublicKeyToken)
            directiveToUse.InstalledVersions.Add(New BindingVersion(info.Version))
        Else
            directiveToUse = currentDirective
        End If

        Dim newRedirection As BindingRedirect = CreateRedirect(info.Version)
        Dim redirectionsCount As Integer = directiveToUse.Redirections.Count
        Dim lastIndex As Integer = redirectionsCount - 1
        If redirectionsCount > 0 Then
            directiveToUse.Redirections(lastIndex) = newRedirection
        Else
            directiveToUse.Redirections.Add(newRedirection)
        End If

        Return directiveToUse
    End Function

    ''' <summary>
    ''' Crea una redirección nueva basada en el número
    ''' de versión especificado.
    ''' </summary>
    ''' <param name="version">Versión a partir de la cuál
    ''' crear la redirección.</param>
    ''' <returns>Redirección creada.</returns>
    Private Function CreateRedirect(ByVal version As String) As BindingRedirect
        Dim range As BindingVersionRange = CreateRange(version)
        Dim redirect As New BindingRedirect(range, New BindingVersion(version))
        Return redirect
    End Function

    ''' <summary>
    ''' Crea un rango de versiones para abarcar la
    ''' versión indicada.
    ''' </summary>
    ''' <param name="version">Versión que se pretende
    ''' abarcar por el rango de versiones.</param>
    ''' <returns>Rango de versiones que abarca la versión
    ''' especificada.</returns>
    Private Function CreateRange(ByVal version As String) As BindingVersionRange
        Dim currentVersion As New BindingVersion(version)
        Dim lowerBound As String = String.Empty
        Dim upperBound As String = String.Empty

        For i As Integer = 0 To currentVersion.Parts.Count - 1
            If i < 1 Then
                lowerBound = "1"
            Else
                lowerBound &= ".0"
            End If

            If i < currentVersion.Parts.Count - 1 Then
                upperBound &= currentVersion.Parts(i) & "."
            End If
        Next

        upperBound &= "65535"

        Dim newRange As New BindingVersionRange(lowerBound, upperBound)
        Return newRange
    End Function

End Class
