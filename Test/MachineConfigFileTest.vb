Imports System.Text
Imports GacManipulation

<TestClass()>
Public Class MachineConfigFileTest

    Private testContextInstance As TestContext
    Private _machineFile As MachineConfigFile

    '''<summary>
    '''Obtiene o establece el contexto de las pruebas que proporciona
    '''información y funcionalidad para la ejecución de pruebas actual.
    '''</summary>
    Public Property TestContext() As TestContext
        Get
            Return testContextInstance
        End Get
        Set(ByVal value As TestContext)
            testContextInstance = value
        End Set
    End Property

#Region "Atributos de prueba adicionales"
    '
    ' Puede usar los siguientes atributos adicionales conforme escribe las pruebas:
    '
    ' Use ClassInitialize para ejecutar el código antes de ejecutar la primera prueba en la clase
    '<ClassInitialize()> Public Shared Sub MyClassInitialize(ByVal testContext As TestContext)
    'End Sub
    '
    ' Use ClassCleanup para ejecutar el código después de haberse ejecutado todas las pruebas en una clase
    ' <ClassCleanup()> Public Shared Sub MyClassCleanup()
    ' End Sub
    '
    ' Usar TestInitialize para ejecutar el código antes de ejecutar cada prueba
    <TestInitialize()> Public Sub MyTestInitialize()
        _machineFile = New MachineConfigFile("C:\Documents and Settings\amedinaj\Mis documentos\Visual Studio 2010\Projects\GacPanel\Test\Files\machine.config.test.xml")
        _machineFile.Load()
    End Sub
    '
    ' Use TestCleanup para ejecutar el código una vez ejecutadas todas las pruebas
    ' <TestCleanup()> Public Sub MyTestCleanup()
    ' End Sub
    '
#End Region

    <TestMethod()>
    Public Sub TestLoad()
        Assert.IsTrue(_machineFile.Directives.Count > 1)
    End Sub

    <TestMethod()>
    Public Sub TestThereAreRedirections()
        Assert.IsTrue(_machineFile.Directives(0).Redirections.Count > 0)
    End Sub

    <TestMethod()>
    Public Sub TestChangeDirective()
        Dim firstRedirection As BindingRedirect = _machineFile.Directives(0).Redirections(0)
        firstRedirection = New BindingRedirect(New BindingVersionRange("1.0.0.0-5.5.5.5"), firstRedirection.TargetVersion)
        _machineFile.Directives(0).Redirections(0) = firstRedirection

        _machineFile.SaveDirectives()

        Dim changedRedirection As BindingRedirect = _machineFile.Directives(0).Redirections(0)
        Assert.IsTrue(firstRedirection.CompareTo(changedRedirection) = 0)
    End Sub

    <TestMethod()>
    Public Sub TestGetMd5()
        Dim md5 As String = _machineFile.Md5
        Assert.IsNotNull(md5)
        Assert.AreNotEqual(md5, String.Empty)
    End Sub
End Class
