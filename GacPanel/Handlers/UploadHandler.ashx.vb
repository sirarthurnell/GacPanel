Imports System.Web
Imports System.Web.Services
Imports System.IO
Imports GacPanel
Imports GacManipulation

Public Class UploadHandler
    Implements System.Web.IHttpHandler, System.Web.SessionState.IRequiresSessionState

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Try

            Dim uploadedFilesPaths = GetUploadedFiles(context)
            Dim proposedAssemblies = ConstructAssembliesToInstallList(uploadedFilesPaths)
            Dim directivesList = From proposedAssembly In proposedAssemblies
                                 Select proposedAssembly.SuggestedDirective

            If context.Session(Keys.AssembliesToInstall) Is Nothing Then
                context.Session(Keys.AssembliesToInstall) = proposedAssemblies
            Else
                Dim sessionList As List(Of AssemblyToInstall) = context.Session(Keys.AssembliesToInstall)
                sessionList.AddRange(proposedAssemblies)
            End If

            Dim result As New OperationResult(Of List(Of BindingDirective))(True, directivesList.ToList())
            JsonResponse.TransmitOject(context.Response, result)

        Catch ex As Exception
            JsonResponse.TransmitError(context.Response, ex)
        End Try
    End Sub

    ''' <summary>
    ''' Obtiene la lista de ensamblados que se pretenden
    ''' instalar en el servidor.
    ''' </summary>
    ''' <param name="filesPaths">Lista de rutas a los
    ''' archivos de los ensamblados.</param>
    ''' <returns>Lista de ensamblados propuestos para su
    ''' instalación.</returns>
    Private Function ConstructAssembliesToInstallList(ByVal filesPaths As List(Of String)) As List(Of AssemblyToInstall)
        Dim proposedAssemblies As New List(Of AssemblyToInstall)()
        Dim frameworkToUse = Framework.Instance(FrameworkSelection.GetSelection())
        Dim machineConfig As MachineConfigFile = frameworkToUse.MachineConfigFile
        Dim resolver As New DirectiveResolver(machineConfig)

        For Each assemblyPath In filesPaths
            Try
                Dim newAssemblyToInstall As New AssemblyToInstall(resolver, assemblyPath)
                proposedAssemblies.Add(newAssemblyToInstall)
            Catch ex As Exception
                Throw ex
            End Try
        Next

        Return proposedAssemblies
    End Function

    ''' <summary>
    ''' Obtiene las rutas a los archivos subidos.
    ''' </summary>
    ''' <returns>Rutas a los archivos subidos.</returns>
    Private Function GetUploadedFiles(ByVal context As HttpContext) As List(Of String)
        Dim uploadedFiles As HttpFileCollection = context.Request.Files
        Dim filesCount As Integer = context.Request.Files.Count
        Dim filesPaths As New List(Of String)()

        If filesCount > 0 Then

            Dim rootPath As String = context.Server.MapPath("~")
            Dim tempPath As String = Path.Combine(rootPath, "Temp")
            If Not Directory.Exists(tempPath) Then
                Directory.CreateDirectory(tempPath)
            End If

            For i As Integer = 0 To uploadedFiles.Count - 1
                Dim uploadedFile = uploadedFiles(i)
                Dim fileName As String = Path.GetFileName(uploadedFile.FileName)
                Dim savePath As String = Path.Combine(tempPath, fileName)
                If File.Exists(savePath) Then
                    File.Delete(savePath)
                End If
                uploadedFile.SaveAs(savePath)
                filesPaths.Add(savePath)
            Next

        End If

        Return filesPaths
    End Function

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class