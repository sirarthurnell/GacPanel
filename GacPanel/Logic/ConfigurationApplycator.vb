Imports GacManipulation

''' <summary>
''' Filtra y aplica la configuración especificada
''' por el cliente.
''' </summary>
Public Class ConfigurationApplycator

    Private _framework As Framework
    Private _gac As Gac
    Private _machineConfig As MachineConfigFile

    ''' <summary>
    ''' Crea una nueva instancia de ConfigurationApplycator.
    ''' </summary>
    ''' <param name="framework">Framework en el que
    ''' se aplicarán los cambios.</param>
    Public Sub New(ByVal framework As Framework)
        _framework = framework
        _gac = _framework.Gac
        _machineConfig = _framework.MachineConfigFile
    End Sub

    ''' <summary>
    ''' Aplica los cambios especificados por el cliente.
    ''' </summary>
    ''' <param name="changes">Cambios especificados.</param>
    Public Sub ApplyChanges(ByVal changes As IEnumerable(Of RootObject))
        Dim changesCopy As New List(Of RootObject)(changes)
        RemoveRemovedAssemblies(changesCopy)
        ApplyDirectivesChangesFromClient(changesCopy)
        _machineConfig.SaveDirectives()
    End Sub

    ''' <summary>
    ''' Instala los ensamblados especificados.
    ''' </summary>
    ''' <param name="assemblies">Lista de ensamblados
    ''' a instalar.</param>
    Public Sub InstallAssemblies(ByVal assemblies As IEnumerable(Of AssemblyToInstall))
        Dim assembliesPaths As New List(Of String)()

        For Each currentAssembly In assemblies
            assembliesPaths.Add(currentAssembly.Path)
        Next

        _gac.InstallAssemblies(assembliesPaths)
    End Sub

    ''' <summary>
    ''' Elimina el ensamblado marcado como eliminado.
    ''' </summary>
    ''' <param name="changes">Lista de cambios.</param>
    Private Sub RemoveRemovedAssemblies(ByVal changes As List(Of RootObject))
        Dim changesCopy As New List(Of RootObject)(changes)
        Dim currentDirectives = _machineConfig.Directives
        Dim possiblyEmptyDirectives As New List(Of BindingDirective)()

        For Each currentChange In changesCopy

            Dim currentChangeName As String = currentChange.Name
            If currentChange.State = "Removed" Then

                Dim versionToRemove = (From version In currentChange.InstalledVersions
                                      Where version.Selected).FirstOrDefault()
                Dim versionToRemoveAsString = String.Join(".", versionToRemove.Parts)
                _gac.UnistallAssembly(currentChange.Name, currentChange.Token, versionToRemoveAsString)

                Dim currentDirective = (From directive In currentDirectives
                                       Where String.Compare(directive.Name, currentChangeName, StringComparison.InvariantCultureIgnoreCase) = 0).FirstOrDefault()
                Dim redirectionToRemove = (From redirection In currentDirective.Redirections
                                          Where redirection.TargetVersion.ToString() = versionToRemoveAsString).FirstOrDefault()
                currentDirective.Redirections.Remove(redirectionToRemove)

                possiblyEmptyDirectives.Add(currentDirective)
            End If

        Next

        RemoveEmptyDirectives(possiblyEmptyDirectives)
    End Sub

    ''' <summary>
    ''' Elimina las directivas indicadas.
    ''' </summary>
    ''' <param name="directives">Directivas a eliminar.</param>
    Private Sub RemoveEmptyDirectives(ByVal directives As IEnumerable(Of BindingDirective))
        Dim currentDirectives = _machineConfig.Directives
        For Each changedDirective In directives
            Dim currentChanged As BindingDirective = changedDirective
            Dim index = currentDirectives.FindIndex(Function(currentDirective)
                                                        Return String.Compare(currentDirective.Name, currentChanged.Name, StringComparison.InvariantCultureIgnoreCase) = 0
                                                    End Function)

            If index >= 0 AndAlso currentChanged.Redirections.Count = 0 Then
                currentDirectives.RemoveAt(index)
            End If
        Next
    End Sub

    ''' <summary>
    ''' Aplica los cambios relativos a las directivas de enlace.
    ''' </summary>
    ''' <param name="changes">Cambios relativos a las directivas
    ''' de enlace.</param>
    Private Sub ApplyDirectivesChangesFromClient(ByVal changes As IEnumerable(Of RootObject))
        Dim changedDirectives = From clientDirective In changes
                                Where clientDirective.State = "Changed" OrElse clientDirective.State = "NewInstall"
                                Select TranslateToBindingDirective(clientDirective)
        AddReplaceDirectives(changedDirectives)
    End Sub

    ''' <summary>
    ''' Añade/reemplaza las directivas de enlace indicadas.
    ''' </summary>
    ''' <param name="directives">Directivas de enlace a aplicar.</param>
    Private Sub AddReplaceDirectives(ByVal directives As IEnumerable(Of BindingDirective))
        Dim currentDirectives = _machineConfig.Directives
        For Each changedDirective In directives
            Dim currentChanged As BindingDirective = changedDirective
            Dim index = currentDirectives.FindIndex(Function(currentDirective)
                                                        Return String.Compare(currentDirective.Name, currentChanged.Name, StringComparison.InvariantCultureIgnoreCase) = 0
                                                    End Function)

            If index < 0 Then
                currentDirectives.Add(currentChanged)
            Else
                currentDirectives(index) = currentChanged
            End If
        Next

        RemoveEmptyDirectives(directives)
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
