Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.IO

''' <summary>
''' Representa las distintas versiones de
''' framework.
''' </summary>
Public Enum FrameworkVersion
    Version1_1
    Version2
    Version4
End Enum

''' <summary>
''' Representa el framework que usa el ejecutable actual.
''' </summary>
''' <remarks>Singleton.</remarks>
Public Class Framework

    Private Shared _frameworks As New Dictionary(Of FrameworkVersion, Framework)()
    Private Shared _syncObj As New Object()

    Private _frameworkPath As String
    Private _machineConfigPath As String
    Private _machineConfigFile As MachineConfigFile
    Private _gacPath As String
    Private _gac As Gac
    Private _gacUtilPath As String
    Private _gacutil As GacUtil

    ''' <summary>
    ''' Crea una nueva instancia de Framework a partir de las rutas relativas al mismo.
    ''' </summary>
    ''' <param name="routes">Objeto que contiene las rutas del framework
    ''' a utilizar.</param>
    Private Sub New(ByVal routes As FrameworkRoutes)
        Me._frameworkPath = routes.FrameworkPath
        Me._machineConfigPath = routes.MachineConfigPath
        Me._machineConfigFile = New MachineConfigFile(Me)
        Me._gacPath = routes.GacPath
        Me._gacUtilPath = routes.GacUtilPath
        Me._gacutil = New GacUtil(_gacUtilPath)
        Me._gac = New Gac(_gacutil)
    End Sub

#Region "Funciones importadas"

    ''' <summary>
    ''' Importación de la función que permite obtener la ruta donde está instalado
    ''' el framework que usa el ejecutable actual.
    ''' </summary>
    ''' <param name="pbuffer">Puntero al buffer de cadena donde se almacenará la ruta.</param>
    ''' <param name="cchBuffer">Tamaño del buffer en el que se almacenará la ruta.</param>
    ''' <param name="dwlength">Tamaño real de la cadena que representa la ruta en carácteres.</param>
    ''' <returns>Cero si ha tenido éxito. Otro valor en caso contrario.</returns>
    <DllImport("mscoree.dll")> _
    Private Shared Function GetCORSystemDirectory( _
        <MarshalAs(UnmanagedType.LPWStr)> ByVal pbuffer As System.Text.StringBuilder, _
        ByVal cchBuffer As Integer, _
        ByRef dwlength As Integer) As Integer
    End Function

#End Region

    ''' <summary>
    ''' Obtiene una instancia de Framework.
    ''' </summary>
    ''' <returns>Instancia de Framework.</returns>
    ''' <remarks>Singleton.</remarks>
    Public Shared Function Instance(ByVal version As FrameworkVersion) As Framework
        If Not _frameworks.ContainsKey(version) Then
            SyncLock _syncObj
                If Not _frameworks.ContainsKey(version) Then

                    Dim routesFactory As IFrameworkRoutesFactory

                    Select Case version

                        Case FrameworkVersion.Version1_1
                            routesFactory = New DefaultFramework1_1RoutesFactory()

                        Case FrameworkVersion.Version2
                            routesFactory = New DefaultFramework2RoutesFactory()

                        Case FrameworkVersion.Version4
                            routesFactory = New DefaultFramework4RoutesFactory()

                        Case Else
                            Throw New ArgumentException("La versión de Framework especificada no está reconocida", "version")

                    End Select

                    Dim framework As New Framework(routesFactory.GetRoutes())
                    _frameworks.Add(version, framework)

                End If
            End SyncLock
        End If

        Return _frameworks.Item(version)
    End Function

    ''' <summary>
    ''' Obtiene la ruta en la que está instalado el
    ''' framework.
    ''' </summary>
    Public ReadOnly Property FrameworkPath As String
        Get
            Return _frameworkPath
        End Get
    End Property

    ''' <summary>
    ''' Obtiene la ruta al archivo de configuración
    ''' del framework.
    ''' </summary>
    Public ReadOnly Property MachineConfigPath As String
        Get
            Return _machineConfigPath
        End Get
    End Property

    ''' <summary>
    ''' Obtiene un objeto que representa el archivo
    ''' de configuración del Framework.
    ''' </summary>
    Public ReadOnly Property MachineConfigFile As MachineConfigFile
        Get
            Return _machineConfigFile
        End Get
    End Property

    ''' <summary>
    ''' Obtiene la ruta a la GAC.
    ''' </summary>
    Public ReadOnly Property GacPath As String
        Get
            Return _gacPath
        End Get
    End Property

    ''' <summary>
    ''' Obtiene un objeto que representa la caché
    ''' de ensamblados global.
    ''' </summary>
    Public ReadOnly Property Gac As Gac
        Get
            Return _gac
        End Get
    End Property

    ''' <summary>
    ''' Obtiene la ruta a la herramienta gacutil.exe
    ''' del framework.
    ''' </summary>
    Public ReadOnly Property GacUtilPath As String
        Get
            Return _gacUtilPath
        End Get
    End Property

    ''' <summary>
    ''' Obtiene un objeto que representa la herramienta
    ''' de configuración de ensamblados.
    ''' </summary>
    Public ReadOnly Property GacUtil As GacUtil
        Get
            Return _gacutil
        End Get
    End Property

    ''' <summary>
    ''' Resuelve la ruta del framework en ejecución.
    ''' </summary>
    ''' <returns>Ruta del framework en ejecución.</returns>
    Public Function ResolveCurrentFrameworkPath() As String
        Dim sb As New System.Text.StringBuilder(255)
        Dim n As Integer

        'Obtenemos la ruta donde está instalado el framework.
        GetCORSystemDirectory(sb, sb.Capacity, n)
        Return sb.ToString().Substring(0, n - 1)
    End Function

    ''' <summary>
    ''' Obtiene las distintas rutas en la gac en la que se pueden encontrar
    ''' ensamblados instalados.
    ''' </summary>
    ''' <returns>Distintas rutas en la gac en la que se pueden encontrar
    ''' ensamblados instalados.</returns>
    ''' <remarks>Se excluye el directorio temporal usado por la GAC para instalar
    ''' ensamblados.</remarks>
    Public Function GacSearchPaths() As String()
        Dim strAssembliesDir As String = Nothing
        Dim lstDirectoriesToSearch As List(Of String) = New List(Of String)

        'Obtenemos el directorio de los ensamblados.
        strAssembliesDir = Path.GetDirectoryName(Me.GacPath().TrimEnd(Path.DirectorySeparatorChar))

        'Obtenemos los directorios que existen dentro del directorio de
        'los ensamblados.
        For Each strDirectory As String In Directory.GetDirectories(strAssembliesDir)
            'Tampoco debe empezar por Native.
            If Not Path.GetFileName(strDirectory).ToUpper().StartsWith("NATIVE") Then
                lstDirectoriesToSearch.Add(strDirectory)
            End If
        Next

        'Devolvemos la lista de directorios en los que buscar.
        Return lstDirectoriesToSearch.ToArray()
    End Function

End Class