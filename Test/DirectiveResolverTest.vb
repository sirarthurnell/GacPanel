Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports GacManipulation
Imports System.IO

'''<summary>
'''Se trata de una clase de prueba para DirectiveResolverTest y se pretende que
'''contenga todas las pruebas unitarias DirectiveResolverTest.
'''</summary>
<TestClass()> _
Public Class DirectiveResolverTest


    Private testContextInstance As TestContext
    Private _machineFile As MachineConfigFile

    '''<summary>
    '''Obtiene o establece el contexto de la prueba que proporciona
    '''la información y funcionalidad para la ejecución de pruebas actual.
    '''</summary>
    Public Property TestContext() As TestContext
        Get
            Return testContextInstance
        End Get
        Set(value As TestContext)
            testContextInstance = Value
        End Set
    End Property

#Region "Atributos de prueba adicionales"
    '
    'Puede utilizar los siguientes atributos adicionales mientras escribe sus pruebas:
    '
    'Use ClassInitialize para ejecutar código antes de ejecutar la primera prueba en la clase 
    '<ClassInitialize()>  _
    'Public Shared Sub MyClassInitialize(ByVal testContext As TestContext)
    'End Sub
    '
    'Use ClassCleanup para ejecutar código después de haber ejecutado todas las pruebas en una clase
    '<ClassCleanup()>  _
    'Public Shared Sub MyClassCleanup()
    'End Sub
    '
    ' Usar TestInitialize para ejecutar el código antes de ejecutar cada prueba
    <TestInitialize()> Public Sub MyTestInitialize()
        _machineFile = New MachineConfigFile(Framework.Instance(New DefaultFramework2RoutesFactory()))
    End Sub
    '
    'Use TestCleanup para ejecutar código después de que se hayan ejecutado todas las pruebas
    '<TestCleanup()>  _
    'Public Sub MyTestCleanup()
    'End Sub
    '
#End Region

    '''<summary>
    '''Una prueba de Suggest
    '''</summary>
    <TestMethod()> _
    Public Sub SuggestTest()
        Dim assemblyPath As String = "C:\Documents and Settings\amedinaj\Mis documentos\Visual Studio 2010\Projects\GacPanel\Test\Files\Mityc.Sigetel.Base.dll"
        Dim resolver As DirectiveResolver = New DirectiveResolver(_machineFile)
        Dim directive As BindingDirective = resolver.Suggest(assemblyPath)

        Assert.IsNotNull(directive)
    End Sub

    '''<summary>
    '''Una prueba de SuggestAndApply
    '''</summary>
    <TestMethod()> _
    Public Sub SuggestAndApplyTest()
        Dim assemblyPath As String = "C:\Documents and Settings\amedinaj\Mis documentos\Visual Studio 2010\Projects\GacPanel\Test\Files\Mityc.Sigetel.Base.dll"
        Dim assemblyName As String = Path.GetFileNameWithoutExtension(assemblyPath)
        Dim resolver As New DirectiveResolver(_machineFile)

        Dim currentDirective = (From directive In _machineFile.Directives
                               Where String.Compare(directive.Name, assemblyName, True) = 0).Single()
        Dim currentRedirection = currentDirective.Redirections(0)

        resolver.SuggestAndApply(assemblyPath)

        Dim suggestedApplied = (From directive In _machineFile.Directives
                               Where String.Compare(directive.Name, assemblyName, True) = 0).Single()
        Dim suggestedAppliedRedirection = suggestedApplied.Redirections(0)

        Assert.IsNotNull(suggestedApplied)
        Assert.AreEqual(currentDirective.Name, suggestedApplied.Name)
        Assert.AreNotEqual(currentRedirection, suggestedAppliedRedirection)
    End Sub
End Class
