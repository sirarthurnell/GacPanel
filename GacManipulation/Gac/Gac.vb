''' <summary>
''' Representa la caché de ensamblados global.
''' </summary>
Public Class Gac

    Private _gacutil As GacUtil
    Private _assemblies As List(Of AssemblyInfo)

    ''' <summary>
    ''' Crea una nueva instancia de Gac.
    ''' </summary>
    ''' <param name="gacutil">Herramienta
    ''' GacUtil a utilizar.</param>
    Public Sub New(ByVal gacutil As GacUtil)
        _gacutil = gacutil
    End Sub

    ''' <summary>
    ''' Obtiene la lista de versiones instaladas
    ''' en la GAC para el ensamblado indicado.
    ''' </summary>
    ''' <param name="name">Nombre corto del ensamblado.</param>
    ''' <returns>Lista de versiones instaladas en la GAC.</returns>
    Public Function GetVersionsForAssembly(ByVal name As String) As List(Of String)
        Dim list = GetAssembliesList()
        Dim versions = From assembly In list
                       Where String.Compare(assembly.Name, name, StringComparison.InvariantCultureIgnoreCase) = 0
                       Select assembly.Version

        Return versions.ToList()
    End Function

    ''' <summary>
    ''' Obtiene la lista de ensamblados de la GAC.
    ''' </summary>
    ''' <returns>Lista de ensamblados de la GAC.</returns>
    Public Function GetAssembliesList() As List(Of AssemblyInfo)
        If _assemblies Is Nothing Then

            _assemblies = New List(Of AssemblyInfo)()
            Dim text As String = _gacutil.Execute("/l")
            Dim fullNamesList() As String = text.Split(vbCrLf)

            For i As Integer = 4 To fullNamesList.GetUpperBound(0) - 3
                Dim fullName As String = fullNamesList(i)
                Dim currentInfo As AssemblyInfo = AssemblyInfo.CreateFromFullName(fullName)
                _assemblies.Add(currentInfo)
            Next

        End If

        Return _assemblies
    End Function

    ''' <summary>
    ''' Solicita que se vuelvan a cargar los
    ''' datos de la GAC.
    ''' </summary>
    Public Sub Reset()
        _assemblies = Nothing
    End Sub

End Class
