Imports System.IO

''' <summary>
''' Factoría de rutas para el framework 4.
''' </summary>
Public Class DefaultFramework4RoutesFactory
    Implements IFrameworkRoutesFactory

    ''' <summary>
    ''' Obtiene las rutas por defecto para el framework 4.
    ''' </summary>
    ''' <returns>Objeto con las rutas configuradas por defecto
    ''' para el framework 4.</returns>
    Public Function GetRoutes() As FrameworkRoutes Implements IFrameworkRoutesFactory.GetRoutes
        Dim windowsDirectory As String = Environment.GetFolderPath(Environment.SpecialFolder.Windows)
        Dim programFilesDirectory As String = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
        Dim frameworkPath As String
        Dim machineConfigPath As String
        Dim gacPath As String
        Dim gacUtilPath As String

        frameworkPath = Path.Combine(windowsDirectory, "Microsoft.NET\Framework\v4.0.30319")
        machineConfigPath = Path.Combine(windowsDirectory, "Microsoft.NET\Framework\v4.0.30319\CONFIG\machine.config")
        gacPath = Path.Combine(windowsDirectory, "Microsoft.NET\Assembly")
        gacUtilPath = Path.Combine(programFilesDirectory, "Microsoft SDKs\Windows\v7.0A\bin\NETFX 4.0 Tools\gacutil.exe")

        Return New FrameworkRoutes(frameworkPath, machineConfigPath, gacPath, gacUtilPath)
    End Function

    ''' <summary>
    ''' Obtiene el número de versión 4.
    ''' </summary>
    Public ReadOnly Property Version As String Implements IFrameworkRoutesFactory.Version
        Get
            Return "4"
        End Get
    End Property

End Class
