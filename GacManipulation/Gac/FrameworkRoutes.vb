''' <summary>
''' Contiene información sobre las rutas del framework
''' a utilizar.
''' </summary>
Public Class FrameworkRoutes

    Private _frameworkPath As String
    Private _machineConfigPath As String
    Private _gacPath As String
    Private _gacUtilPath As String

    ''' <summary>
    ''' Crea una nueva instancia de FrameworkRoutes.
    ''' </summary>
    ''' <param name="frameworkPath">Ruta al directorio en
    ''' el que se encuentra instalado el framework a usar.</param>
    ''' <param name="machineConfigPath">Ruta donde se encuentra
    ''' el archivo machine.config del framework a utilizar.</param>
    ''' <param name="gacPath">Ruta a la GAC a utilizar.</param>
    ''' <param name="gacUtilPath">Ruta a la herramienta gacutil.exe
    ''' a utilizar.</param>
    Public Sub New(ByVal frameworkPath As String, ByVal machineConfigPath As String, ByVal gacPath As String, ByVal gacUtilPath As String)
        _frameworkPath = frameworkPath
        _machineConfigPath = machineConfigPath
        _gacPath = gacPath
        _gacUtilPath = gacUtilPath
    End Sub

    ''' <summary>
    ''' Ruta al directorio donde se encuentra el framework
    ''' instalado.
    ''' </summary>
    Public ReadOnly Property FrameworkPath As String
        Get
            Return _frameworkPath
        End Get
    End Property

    ''' <summary>
    ''' Ruta al archivo machine.config.
    ''' </summary>
    Public ReadOnly Property MachineConfigPath As String
        Get
            Return _machineConfigPath
        End Get
    End Property

    ''' <summary>
    ''' Ruta dónde se encuentra la GAC a utilizar.
    ''' </summary>
    Public ReadOnly Property GacPath As String
        Get
            Return _gacPath
        End Get
    End Property

    ''' <summary>
    ''' Ruta dónde se encuentra la herramienta
    ''' gacutil a utilizar.
    ''' </summary>
    Public ReadOnly Property GacUtilPath As String
        Get
            Return _gacUtilPath
        End Get
    End Property

End Class
