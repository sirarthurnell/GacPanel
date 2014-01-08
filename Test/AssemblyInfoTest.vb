Imports Microsoft.VisualStudio.TestTools.UnitTesting

Imports GacManipulation



'''<summary>
'''Se trata de una clase de prueba para AssemblyInfoTest y se pretende que
'''contenga todas las pruebas unitarias AssemblyInfoTest.
'''</summary>
<TestClass()> _
Public Class AssemblyInfoTest


    Private testContextInstance As TestContext

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
    'Use TestInitialize para ejecutar código antes de ejecutar cada prueba
    '<TestInitialize()>  _
    'Public Sub MyTestInitialize()
    'End Sub
    '
    'Use TestCleanup para ejecutar código después de que se hayan ejecutado todas las pruebas
    '<TestCleanup()>  _
    'Public Sub MyTestCleanup()
    'End Sub
    '
#End Region


    '''<summary>
    '''Una prueba de CreateFromFullName
    '''</summary>
    <TestMethod()> _
    Public Sub CreateFromFullNameTest()
        Dim fullName As String = "WindowsFormsIntegration.Package.resources, Version=10.0.0.0, Culture=es, PublicKeyToken=b03f5f7f11d50a3a, processorArchitecture=MSIL"
        Dim actual As AssemblyInfo = AssemblyInfo.CreateFromFullName(fullName)

        Assert.AreEqual(fullName, actual.FullName)
        Assert.AreEqual("WindowsFormsIntegration.Package.resources", actual.Name)
        Assert.AreEqual("10.0.0.0", actual.Version)
        Assert.AreEqual("b03f5f7f11d50a3a", actual.PublicKeyToken)
        Assert.IsTrue(actual.HasPublicKey)
    End Sub
End Class
