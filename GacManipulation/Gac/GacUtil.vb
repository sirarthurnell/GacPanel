Imports System.Diagnostics
Imports System.IO
Imports System.Text

''' <summary>
''' Representa el ejecutable usado para añadir o quitar ensamblados
''' de la caché de ensamblados.
''' </summary>
Public Class GacUtil

    Private _pathToGacUtil As String

    ''' <summary>
    ''' Crea una instancia de GacUtil.
    ''' </summary>
    ''' <param name="pathToGacUtil">Ruta al gacUtil.</param>
    Public Sub New(ByVal pathToGacUtil As String)
        If Not File.Exists(pathToGacUtil) Then
            Throw New ArgumentException("La herramienta gacutil no se encuentra en la ruta indicada", "pathToGacUtil")
        End If
        Me._pathToGacUtil = pathToGacUtil
    End Sub

#Region "Métodos públicos"

    ''' <summary>
    ''' Ejecuta la herramienta gacutil con los argumentos especificados.
    ''' </summary>
    ''' <param name="args">Argumentos con los que se ejecutará la herramienta gacutil.</param>
    ''' <returns>Salida de gacutil.</returns>
    ''' <exception cref="FailedGacUtilOperationException">Lanzada cuando se excede el tiempo
    ''' de espera para la operación que gacutil está realizado o bien cuando gacutil termina
    ''' de forma anormal.</exception>
    Public Function Execute(ByVal args As String) As String
        Dim procUtil As Process
        Dim procInfoUtil As ProcessStartInfo
        Dim datStartProcessDate As Date = Date.MinValue
        Dim output As String

        'Obtenemos un objeto de proceso que representará al gacUtil
        'con todos sus argumentos.
        procUtil = New Process()
        procInfoUtil = New ProcessStartInfo()
        procInfoUtil.FileName = _pathToGacUtil
        procInfoUtil.Arguments = args
        'procInfoUtil.WindowStyle = ProcessWindowStyle.Hidden
        procInfoUtil.UseShellExecute = False
        procInfoUtil.RedirectStandardOutput = True
        procInfoUtil.CreateNoWindow = True
        procUtil.StartInfo = procInfoUtil

        'Guardamos la fecha en la que se lanzó el proceso.
        datStartProcessDate = Date.Now
        procUtil.Start()

        'Mostramos la salida.
        output = procUtil.StandardOutput.ReadToEnd()

        If procUtil.ExitCode <> 0 Then
            'Si el ejecutable terminó de forma inesperada, lanzamos una excepción.
            Throw New FailedGacUtilOperationException("Gacutil falló de forma inesperada: " & output)
        Else
            Return output
        End If
    End Function

#End Region

End Class

#Region "Excepciones"

#Region "FailedGacUtilOperationException"

''' <summary>
''' Representa una excepción que se lanza cuando una operación que se esperaba
''' que realizase el gacutil ha fallado.
''' </summary>
Public Class FailedGacUtilOperationException
    Inherits Exception

    ''' <summary>
    ''' Crea una nueva instancia de FailedGacUtilOperationException.
    ''' </summary>
    Public Sub New()
        MyBase.New()
    End Sub

    ''' <summary>
    ''' Crea una nueva instancia de FailedGacUtilOperationException.
    ''' </summary>
    ''' <param name="strMessage">Mensaje que mostrará la excepción.</param>
    Public Sub New(ByVal strMessage As String)
        MyBase.New(strMessage)
    End Sub

    ''' <summary>
    ''' Crea una nueva instancia de FailedGacUtilOperationException.
    ''' </summary>
    ''' <param name="strMessage">Mensaje que mostrará la excepción.</param>
    ''' <param name="ex">Excepción anexada.</param>
    Public Sub New(ByVal strMessage As String, ByVal ex As Exception)
        MyBase.New(strMessage, ex)
    End Sub

End Class

#End Region

#End Region