Imports System.Web
Imports System.Web.Services
Imports System.IO
Imports GacManipulation

Public Class DirectivesListHandler
    Implements System.Web.IHttpHandler

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        Dim framework As Framework = framework.Instance(FrameworkVersion.Version2)
        Dim machineConfig As New MachineConfigFile(framework.MachineConfigPath)
        machineConfig.Load()

        Dim directives = machineConfig.Directives

        Dim result As New OperationResult(Of List(Of BindingDirective))(True, directives)
        JsonResponse.TransmitOject(context.Response, result)

    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class