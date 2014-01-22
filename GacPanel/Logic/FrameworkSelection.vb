Imports GacManipulation

''' <summary>
''' Representa la elección del framework a utilizar.
''' </summary>
Public Class FrameworkSelection

    ''' <summary>
    ''' Devuelve la factoría de rutas del framework elegido.
    ''' </summary>
    ''' <returns>Factoría de rutas del framework elegido.</returns>
    Public Shared Function GetSelection() As IFrameworkRoutesFactory
        Return New DefaultFramework2RoutesFactory()
    End Function

End Class
