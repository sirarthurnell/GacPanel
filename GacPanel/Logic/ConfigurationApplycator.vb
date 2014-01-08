﻿Imports GacManipulation

''' <summary>
''' Filtra y aplica la configuración especificada
''' por el cliente.
''' </summary>
Public Class ConfigurationApplycator

    Private _framework As Framework
    Private _gac As Gac

    ''' <summary>
    ''' Crea una nueva instancia de ConfigurationApplycator.
    ''' </summary>
    ''' <param name="framework">Framework en el que
    ''' se aplicarán los cambios.</param>
    Public Sub New(ByVal framework As Framework)
        _framework = framework
        _gac = _framework.Gac
    End Sub

    ''' <summary>
    ''' Aplica los cambios especificados por el cliente.
    ''' </summary>
    ''' <param name="changes">Cambios especificados.</param>
    Public Sub ApplyChanges(ByVal changes As IEnumerable(Of RootObject))
        Dim changesCopy As New List(Of RootObject)(changes)
        RemoveRemovedAssemblies(changesCopy)
        ApplyDirectivesChangesFromClient(changesCopy)
    End Sub

    ''' <summary>
    ''' Instala los ensamblados especificados.
    ''' </summary>
    ''' <param name="assemblies">Lista de ensamblados
    ''' a instalar.</param>
    Public Sub InstallAssemblies(ByVal assemblies As IEnumerable(Of AssemblyToInstall))
        Dim newDirectives As New List(Of BindingDirective)()

        For Each currentAssembly In assemblies
            _gac.InstallAssembly(currentAssembly.Path)
            newDirectives.Add(currentAssembly.SuggestedDirective)
        Next

        ApplyDirectives(newDirectives)
    End Sub

    ''' <summary>
    ''' Elimina el ensamblado marcado como eliminado.
    ''' </summary>
    ''' <param name="changes">Lista de cambios.</param>
    Private Sub RemoveRemovedAssemblies(ByVal changes As List(Of RootObject))
        Dim changesCopy As New List(Of RootObject)(changes)

        For Each currentChange In changesCopy
            If currentChange.State = "Removed" Then
                Dim versionToRemove = String.Join(".", currentChange.InstalledVersions(0).Parts)
                _gac.UnistallAssembly(currentChange.Name, currentChange.Token, versionToRemove)
                changes.Remove(currentChange)
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
                                        Where clientDirective.State = "Changed"
                                        Select TranslateToBindingDirective(clientDirective)

        ApplyDirectives(changedDirectives)
    End Sub

    ''' <summary>
    ''' Aplica las directivas de enlace indicadas.
    ''' </summary>
    ''' <param name="directives">Directivas de enlace a aplicar.</param>
    Private Sub ApplyDirectives(ByVal directives As IEnumerable(Of BindingDirective))
        Dim machineConfig = _framework.MachineConfigFile
        machineConfig.Load()

        Dim currentDirectives = machineConfig.Directives
        For Each changedDirective In directives
            Dim currentChanged As BindingDirective = changedDirective
            Dim index = currentDirectives.FindIndex(Function(currentDirective)
                                                        Return String.Compare(currentDirective.Name, currentChanged.Name, StringComparison.InvariantCultureIgnoreCase) = 0
                                                    End Function)

            currentDirectives(index) = currentChanged
        Next

        machineConfig.SaveDirectives()
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