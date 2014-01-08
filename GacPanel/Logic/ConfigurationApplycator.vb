Imports GacManipulation

''' <summary>
''' Filtra y aplica la configuración especificada
''' por el cliente.
''' </summary>
Public Class ConfigurationApplycator

    ''' <summary>
    ''' Aplica los cambios especificados por el cliente.
    ''' </summary>
    ''' <param name="changes">Cambios especificados.</param>
    Public Sub ApplyChanges(ByVal changes As List(Of RootObject))
        Dim framework As Framework = framework.Instance(FrameworkVersion.Version2)
    End Sub

    ''' <summary>
    ''' Traduce la directiva de enlace procedente del cliente
    ''' en directiva de enlace para aplicar en el servidor.
    ''' </summary>
    ''' <param name="fromClient">Directiva de enlace
    ''' procedente del cliente.</param>
    ''' <returns>Directiva de enlace de servidor.</returns>
    Private Function TranslateToBindingDirective(ByVal fromClient As RootObject) As BindingDirective
        Dim serverDirective As New BindingDirective(fromClient.Name, fromClient.Token)

        For Each version In fromClient.InstalledVersions
            serverDirective.InstalledVersions.Add(New BindingVersion(version.Parts))
        Next

        For Each redirection In fromClient.Redirections

            Dim target As New BindingVersion(redirection.TargetVersion.Parts)
            Dim lowerBound As New BindingVersion(redirection.Range.LowerBound.Parts)
            Dim upperBound As New BindingVersion(redirection.Range.UpperBound.Parts)
            Dim range As New BindingVersionRange(lowerBound, upperBound)
            Dim serverRedirection As New BindingRedirect(range, target)

            serverDirective.Redirections.Add(serverRedirection)

        Next

        Return serverDirective
    End Function

End Class
