''' <summary>
''' Interfaz a implementar por aquellas clases
''' de las que se pueden obtener directivas de
''' enlace.
''' </summary>
Public Interface IBindingDirectiveSource

    ''' <summary>
    ''' Cuando se implementa, obtiene una lista de
    ''' directivas de enlace.
    ''' </summary>
    ReadOnly Property Directives As List(Of BindingDirective)

End Interface
