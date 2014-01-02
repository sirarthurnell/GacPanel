''' <summary>
''' Representa la redirección de una directiva
''' de enlace.
''' </summary>
Public Structure BindingRedirect
    Implements IComparable(Of BindingRedirect)

    Private _range As BindingVersionRange
    Private _targetVersion As BindingVersion

    ''' <summary>
    ''' Crea una nueva instancia de BindingRedirect.
    ''' </summary>
    ''' <param name="range">Rango de versiones a redireccionar.</param>
    ''' <param name="targetVersion">Versión de destino.</param>
    Public Sub New(ByVal range As BindingVersionRange, ByVal targetVersion As BindingVersion)
        _range = range
        _targetVersion = targetVersion
    End Sub

    ''' <summary>
    ''' Obtiene el rango de versiones a redireccionar.
    ''' </summary>
    Public ReadOnly Property Range As BindingVersionRange
        Get
            Return _range
        End Get
    End Property

    ''' <summary>
    ''' Obtiene la versión de destino.
    ''' </summary>
    Public ReadOnly Property TargetVersion As BindingVersion
        Get
            Return _targetVersion
        End Get
    End Property

    ''' <summary>
    ''' Obtiene una representación en cadena
    ''' de la redirección.
    ''' </summary>
    ''' <returns>Representación en cadena de
    ''' la redirección.</returns>
    Public Overrides Function ToString() As String
        Return String.Format("oldVersion=""{0}"" newVersion=""{1}""", Range, TargetVersion)
    End Function

    ''' <summary>
    ''' Compara esta instancia con otra redirección.
    ''' </summary>
    ''' <param name="other">Otra redirección con la
    ''' que se quiere comparar esta instancia.</param>
    ''' <returns>Menor que cero si esta instancia es de menor número
    ''' de versión. 0 si son iguales. Mayor que cero si es mayor.</returns>
    Public Function CompareTo(other As BindingRedirect) As Integer Implements System.IComparable(Of BindingRedirect).CompareTo
        Return Me.TargetVersion.CompareTo(other.TargetVersion)
    End Function

    ''' <summary>
    ''' Comprueba si dos redirecciones son iguales.
    ''' </summary>
    ''' <param name="obj">Otra redirección con la
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

        Dim castObj As BindingRedirect = CType(obj, BindingRedirect)
        If Not Me.TargetVersion.Equals(castObj.TargetVersion) Then
            Return False
        End If

        If Not Me.Range.Equals(castObj.Range) Then
            Return False
        End If

        Return True
    End Function

End Structure
