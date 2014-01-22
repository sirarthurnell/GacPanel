''' <summary>
''' Interfaz a implementar por aquellos objetos
''' factoría encargados de proporcionar un objeto
''' de rutas de framework.
''' </summary>
Public Interface IFrameworkRoutesFactory

    ''' <summary>
    ''' Obtiene el objeto que contiene información
    ''' sobre las rutas del framework a utilizar.
    ''' </summary>
    ''' <returns>Objeto con las rutas del framework.</returns>
    Function GetRoutes() As FrameworkRoutes

End Interface
