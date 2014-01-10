Imports System.Web
Imports System.Web.Services
Imports System.IO
Imports Newtonsoft.Json
Imports GacManipulation

Public Class ApplyChangesHandler
    Implements System.Web.IHttpHandler, System.Web.SessionState.IRequiresSessionState

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Try

            Dim directivesAsJson As String = New StreamReader(context.Request.InputStream).ReadToEnd()
            Dim changes = JsonConvert.DeserializeObject(Of List(Of RootObject))(directivesAsJson)

            Dim applycator As New ConfigurationApplycator(Framework.Instance(FrameworkVersion.Version2))

            Dim assembliesToInstall As List(Of AssemblyToInstall)
            assembliesToInstall = context.Session(Keys.AssembliesToInstall)
            If Not assembliesToInstall Is Nothing Then
                applycator.InstallAssemblies(assembliesToInstall)
                context.Session(Keys.AssembliesToInstall) = Nothing
            End If

            applycator.ApplyChanges(changes)

            JsonResponse.TransmitOk(context.Response)

        Catch ex As Exception
            JsonResponse.TransmitError(context.Response, ex)
        End Try
    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class