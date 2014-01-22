Imports System.IO

''' <summary>
''' Factoría de rutas para el framework 1.1.
''' </summary>
Public Class DefaultFramework1_1RoutesFactory
    Implements IFrameworkRoutesFactory

    ''' <summary>
    ''' Obtiene las rutas por defecto para el framework 1.1.
    ''' </summary>
    ''' <returns>Objeto con las rutas configuradas por defecto
    ''' para el framework 1.1.</returns>
    Public Function GetRoutes() As FrameworkRoutes Implements IFrameworkRoutesFactory.GetRoutes
        Dim windowsDirectory As String = Environment.GetFolderPath(Environment.SpecialFolder.Windows)
        Dim frameworkPath As String
        Dim machineConfigPath As String
        Dim gacPath As String
        Dim gacUtilPath As String

        frameworkPath = Path.Combine(windowsDirectory, "Microsoft.NET\Framework\v1.1.4322")
        machineConfigPath = Path.Combine(windowsDirectory, "Microsoft.NET\Framework\v1.1.4322\CONFIG\machine.config")
        gacPath = Path.Combine(windowsDirectory, "assembly\GAC")
        gacUtilPath = Path.Combine(windowsDirectory, "Microsoft.NET\Framework\v1.1.4322\gacutil.exe")

        Return New FrameworkRoutes(frameworkPath, machineConfigPath, gacPath, gacUtilPath)
    End Function

End Class
