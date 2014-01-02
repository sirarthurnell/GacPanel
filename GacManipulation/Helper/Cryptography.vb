Imports System.Security.Cryptography
Imports System.Text

''' <summary>
''' Contiene métodos para trabajo con criptografía.
''' </summary>
Public NotInheritable Class Cryptography

    Private Sub New()
    End Sub

    ''' <summary>
    ''' Computa la firma MD5 para un texto dado.
    ''' </summary>
    ''' <param name="text">Texto al que computar
    ''' la firma MD5.</param>
    ''' <returns>Firma computada.</returns>
    Public Shared Function ComputeMd5(ByVal text As String) As String
        Dim md5 = MD5CryptoServiceProvider.Create()
        Dim utf8 As Encoding = Encoding.UTF8
        Dim md5Bytes() As Byte = md5.ComputeHash(utf8.GetBytes(text))
        Dim sb As New StringBuilder()

        For Each md5Byte In md5Bytes
            sb.AppendFormat("{0:x2}", md5Byte)
        Next

        Return sb.ToString()
    End Function

End Class
