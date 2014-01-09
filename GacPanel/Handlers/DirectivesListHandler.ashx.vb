Imports System.Web
Imports System.Web.Services
Imports System.IO
Imports GacManipulation

Public Class DirectivesListHandler
    Implements System.Web.IHttpHandler, System.Web.SessionState.IRequiresSessionState

    Public ReadOnly FakeJson As Boolean = False

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        If FakeJson Then
            SendFakeJson(context)
        Else
            Try

                context.Session(Keys.AssembliesToInstall) = Nothing

                Dim framework As Framework = framework.Instance(FrameworkVersion.Version2)
                Dim machineConfig As MachineConfigFile = framework.MachineConfigFile
                Dim directives = machineConfig.Directives

                Dim result As New OperationResult(Of List(Of BindingDirective))(True, directives)
                JsonResponse.TransmitOject(context.Response, result)

            Catch ex As Exception
                JsonResponse.TransmitError(context.Response, ex)
            End Try
        End If
    End Sub

    ''' <summary>
    ''' Envía un archivo JSON para hacer pruebas.
    ''' </summary>
    ''' <param name="context">Contexto ASP.NET.</param>
    Sub SendFakeJson(ByVal context As HttpContext)
        Dim json As String

        Using sr As New StreamReader(context.Server.MapPath("Fake\assembliesListFake.json"))
            json = sr.ReadToEnd()
        End Using

        JsonResponse.TransmitJson(context.Response, json)
    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class