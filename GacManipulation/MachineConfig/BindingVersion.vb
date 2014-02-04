Imports System.Collections.ObjectModel

''' <summary>
''' Representa un número de versión de un ensamblado
''' a usar en una operación de directivas de enlace.
''' </summary>
Public Structure BindingVersion
    Implements IComparable(Of BindingVersion)

    Private _version As List(Of Integer)

    ''' <summary>
    ''' Crea una nueva instancia de BindingVersion.
    ''' </summary>
    ''' <param name="numbers">Enumerable con los
    ''' números de la versión.</param>
    Public Sub New(ByVal numbers As IEnumerable(Of Integer))
        _version = New List(Of Integer)(numbers)
    End Sub

    ''' <summary>
    ''' Crea una nueva instancia de BindingVersion.
    ''' </summary>
    ''' <param name="version">Número de versión.</param>
    Public Sub New(ByVal version As String)
        _version = New List(Of Integer)()
        ParseAndFillNumbers(version)
    End Sub

    ''' <summary>
    ''' Parsea el número de versión expresado como cadena.
    ''' </summary>
    ''' <param name="version">Número de versión a parsear.</param>
    Private Sub ParseAndFillNumbers(ByVal version As String)
        Dim numbers() As String

        If String.IsNullOrEmpty(version) Then
            Throw New ArgumentException("La versión no puede ser una cadena vacía o nula.", "version")
        End If

        Try
            numbers = version.Split(".")
            For Each current In numbers
                _version.Add(CInt(current))
            Next
        Catch ex As Exception
            Throw New ArgumentException("Formato de número de versión incorrecto.", "version", ex)
        End Try
    End Sub

    ''' <summary>
    ''' Obtiene las partes que componen este
    ''' número de versión.
    ''' </summary>
    Public ReadOnly Property Parts() As ReadOnlyCollection(Of Integer)
        Get
            Return _version.AsReadOnly()
        End Get
    End Property

    ''' <summary>
    ''' Obtiene una representación en cadena del
    ''' número de versión.
    ''' </summary>
    ''' <returns>Representación en cadena del
    ''' número de versión.</returns>
    Public Overrides Function ToString() As String
        Return String.Join(".", _version)
    End Function

    ''' <summary>
    ''' Compara esta instancia con otra instancia
    ''' de BindingVersion.
    ''' </summary>
    ''' <param name="other">Otra instancia que
    ''' comparar.</param>
    ''' <returns>Un número igual que 0 si son iguales.
    ''' Un número mayor que cero si esta instancia es mayor
    ''' que con la que se compara.
    ''' Un número menor que cero si esta instancia es menor.</returns>
    Public Function CompareTo(other As BindingVersion) As Integer Implements System.IComparable(Of BindingVersion).CompareTo
        Dim totalChunks As Integer = Math.Min(_version.Count, other._version.Count)

        For i As Integer = 0 To totalChunks - 1
            If _version(i) = other._version(i) Then
                Continue For
            Else
                Return _version(i) - other._version(i)
            End If
        Next

        If _version.Count > other._version.Count Then
            Return 1
        ElseIf _version.Count < other._version.Count Then
            Return -1
        Else
            Return 0
        End If
    End Function

    ''' <summary>
    ''' Comprueba si dos números de versión son iguales.
    ''' </summary>
    ''' <param name="obj">Otro número de versión con el
    ''' que comparar.</param>
    ''' <returns>True si son iguales. False en caso
    ''' contrario.</returns>
    Public Overrides Function Equals(obj As Object) As Boolean
        If obj Is Nothing Then
            Return False
        End If

        If Me.ToString().GetHashCode() = obj.ToString().GetHashCode() Then
            Return True
        End If

        If Not TypeOf (obj) Is BindingVersion Then
            Return False
        End If

        Dim castObj As BindingVersion = CType(obj, BindingVersion)
        If Me.Parts.Count <> castObj.Parts.Count Then
            Return False
        End If

        For i As Integer = 0 To Me.Parts.Count - 1
            If Me.Parts(i) <> castObj.Parts(i) Then
                Return False
            End If
        Next

        Return True
    End Function

End Structure
