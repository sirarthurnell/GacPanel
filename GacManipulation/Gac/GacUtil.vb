Imports System.Diagnostics
Imports System.IO

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
        If Not File.Exists(_pathToGacUtil) Then
            Throw New ArgumentException("La herramienta gacutil no se encuentra en la ruta indicada", "pathToGacUtil")
        End If
        Me._pathToGacUtil = pathToGacUtil
    End Sub

#Region "Métodos públicos"

    ''' <summary>
    ''' Ejecuta la herramienta gacutil con los argumentos especificados.
    ''' </summary>
    ''' <param name="args">Argumentos con los que se ejecutará la herramienta gacutil.</param>
    ''' <param name="assemblyNamePath">Nombre o path del ensamblado a operar.</param>
    ''' <exception cref="FailedGacUtilOperationException">Lanzada cuando se excede el tiempo
    ''' de espera para la operación que gacutil está realizado o bien cuando gacutil termina
    ''' de forma anormal.</exception>
    Public Sub Execute(ByVal args As String, ByVal assemblyNamePath As String)
        Dim procUtil As Process
        Dim procInfoUtil As ProcessStartInfo
        Dim datStartProcessDate As Date = Date.MinValue

        If Not File.Exists(assemblyNamePath) Then
            Throw New ArgumentException("El ensamblado especificado no se encuentra en la ruta indicada", "assemblyNamePath")
        End If

        'Obtenemos un objeto de proceso que representará al gacUtil
        'con todos sus argumentos.
        procUtil = New Process()
        procInfoUtil = New ProcessStartInfo()
        procInfoUtil.FileName = _pathToGacUtil

        'Añadimos el nombre del ensamblado a los argumentos.
        args = " /" & args & " " & """" & assemblyNamePath & """"

        'Añadimos a los argumentos que se ejecute gacutil en modo silencioso.
        args &= " /silent"

        procInfoUtil.Arguments = args
        procInfoUtil.WindowStyle = ProcessWindowStyle.Hidden
        procUtil.StartInfo = procInfoUtil

        'Guardamos la fecha en la que se lanzó el proceso.
        datStartProcessDate = Date.Now
        procUtil.Start()

        'Esperamos hasta que el proceso haya acabado o hayan pasado 10 segundos.
        Do
            System.Threading.Thread.Sleep(50)
        Loop While procUtil.HasExited = False Or Date.Now.Subtract(datStartProcessDate).Seconds > 10

        'Si han pasado 10 segundos, lanzamos una excepción por no haber podido
        'ejecutar el gacutil correctamente.
        If Date.Now.Subtract(datStartProcessDate).Seconds > 10 Then
            'Cerramos el gacutil.
            procUtil.Kill()

            'Lanzamos la excepción para notificar que ha sido imposible completar
            'la operación.
            Throw New FailedGacUtilOperationException("Se ha excedido el tiempo de espera para la operación a realizar. Argumentos: " & args)
        Else
            'Si el ejecutable terminó de forma inesperada, lanzamos una excepción.
            If procUtil.ExitCode <> 0 Then
                Throw New FailedGacUtilOperationException("Gacutil falló de forma inesperada.")
            End If
        End If
    End Sub

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