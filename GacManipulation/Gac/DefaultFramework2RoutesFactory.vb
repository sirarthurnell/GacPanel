Imports System.IO

''' <summary>
''' Factoría de rutas para el framework 2.
''' </summary>
Public Class DefaultFramework2RoutesFactory
    Implements IFrameworkRoutesFactory

    ''' <summary>
    ''' Obtiene las rutas por defecto para el framework 2.
    ''' </summary>
    ''' <returns>Objeto con las rutas configuradas por defecto
    ''' para el framework 2.</returns>
    Public Function GetRoutes() As FrameworkRoutes Implements IFrameworkRoutesFactory.GetRoutes
        Dim windowsDirectory As String = Environment.GetFolderPath(Environment.SpecialFolder.Windows)
        Dim frameworkPath As String
        Dim machineConfigPath As String
        Dim gacPath As String
        Dim gacUtilPath As String

        frameworkPath = Path.Combine(windowsDirectory, "Microsoft.NET\Framework\v2.0.50727")
        machineConfigPath = Path.Combine(windowsDirectory, "Microsoft.NET\Framework\v2.0.50727\CONFIG\machine.config")
        gacPath = Path.Combine(windowsDirectory, "assembly\GAC")
        gacUtilPath = Path.Combine(windowsDirectory, "Microsoft.NET\Framework\v2.0.50727\gacutil.exe")

        Return New FrameworkRoutes(frameworkPath, machineConfigPath, gacPath, gacUtilPath)
    End Function

    ''' <summary>
    ''' Obtiene el número de versión 2.
    ''' </summary>
    Public ReadOnly Property Version As String Implements IFrameworkRoutesFactory.Version
        Get
            Return "2"
        End Get
    End Property

End Class
