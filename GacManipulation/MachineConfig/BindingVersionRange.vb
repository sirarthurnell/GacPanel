''' <summary>
''' Representa un intervalo de versiones para operar
''' con directivas de enlace.
''' </summary>
Public Structure BindingVersionRange

    Private _lowerBound As BindingVersion
    Private _upperBound As BindingVersion

    ''' <summary>
    ''' Crea una nueva instancia de BindingVersionRange.
    ''' </summary>
    ''' <param name="lowerBound">Límite inferior.</param>
    ''' <param name="upperBound">Límite superior.</param>
    Public Sub New(ByVal lowerBound As BindingVersion, ByVal upperBound As BindingVersion)
        _lowerBound = lowerBound
        _upperBound = upperBound
    End Sub

    ''' <summary>
    ''' Crea una nueva instancia de BindingVersionRange.
    ''' </summary>
    ''' <param name="lowerBound">Límite inferior.</param>
    ''' <param name="upperBound">Límite superior.</param>
    Public Sub New(ByVal lowerBound As String, ByVal upperBound As String)
        _lowerBound = New BindingVersion(lowerBound)
        _upperBound = New BindingVersion(upperBound)
    End Sub

    ''' <summary>
    ''' Crea una nueva instancia de BindingVersionRange.
    ''' </summary>
    ''' <param name="range">Intervalo de versiones.</param>
    Public Sub New(ByVal range As String)
        Dim rangeParts() As String = range.Split("-")
        _lowerBound = New BindingVersion(rangeParts(0))
        _upperBound = New BindingVersion(rangeParts(1))
    End Sub

    ''' <summary>
    ''' Obtiene el límite inferior del rango.
    ''' </summary>
    Public ReadOnly Property LowerBound() As BindingVersion
        Get
            Return _lowerBound
        End Get
    End Property

    ''' <summary>
    ''' Obtiene el límite superior del rango.
    ''' </summary>
    Public ReadOnly Property UpperBound() As BindingVersion
        Get
            Return _upperBound
        End Get
    End Property

    ''' <summary>
    ''' Comprueba si dos rangos son iguales.
    ''' </summary>
    ''' <param name="obj">Otro rango con el
    ''' que comparar.</param>
    ''' <returns>True si son iguales. False en caso
    ''' contrario.</returns>
    Public Overrides Function Equals(obj As Object) As Boolean
        If obj Is Nothing Then
            Return False
        End If

        If Me.GetHashCode() = obj.GetHashCode() Then
            Return True
        End If

        If Not TypeOf (obj) Is BindingRedirect Then
            Return False
        End If

        Dim castObj As BindingVersionRange = CType(obj, BindingVersionRange)
        If Not Me.LowerBound.Equals(castObj.LowerBound) _
        OrElse Not Me.UpperBound.Equals(castObj.UpperBound) Then
            Return False
        End If

        Return True
    End Function

    ''' <summary>
    ''' Obtiene una representación en cadena del
    ''' rango de versiones.
    ''' </summary>
    ''' <returns>Representación en cadena del
    ''' rango de versiones.</returns>
    Public Overrides Function ToString() As String
        Return LowerBound.ToString() & "-" & UpperBound.ToString()
    End Function

End Structure
