Imports System.Web
Imports System.Web.Services
Imports System.IO

Public Class UploadHandler
    Implements System.Web.IHttpHandler

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Try

            Dim uploadedFiles As HttpFileCollection = context.Request.Files
            Dim filesCount As Integer = context.Request.Files.Count

            If filesCount > 0 Then

                Dim tempPath As String = context.Server.MapPath("~/Temp")

                For i As Integer = 0 To uploadedFiles.Count - 1
                    Dim uploadedFile = uploadedFiles(i)
                    Dim fileName As String = Path.GetFileName(uploadedFile.FileName)
                    Dim savePath As String = Path.Combine(tempPath, fileName)
                    If File.Exists(savePath) Then
                        File.Delete(savePath)
                    End If
                    uploadedFile.SaveAs(savePath)
                Next

                Dim result As New OperationResult(Of String)(True, filesCount & " archivos recibidos")
                JsonResponse.TransmitOject(context.Response, result)

            End If

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