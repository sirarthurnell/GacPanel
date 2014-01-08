Imports System.Collections.Generic

Imports Microsoft.VisualStudio.TestTools.UnitTesting

Imports GacManipulation



'''<summary>
'''Se trata de una clase de prueba para GacUtilTest y se pretende que
'''contenga todas las pruebas unitarias GacUtilTest.
'''</summary>
<TestClass()> _
Public Class GacUtilTest

    Private _framework As Framework
    Private _gacutil As GacUtil
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
    <TestInitialize()> _
    Public Sub MyTestInitialize()
        _framework = Framework.Instance(FrameworkVersion.Version4)
        _gacutil = _framework.GacUtil
    End Sub
    '
    'Use TestCleanup para ejecutar código después de que se hayan ejecutado todas las pruebas
    '<TestCleanup()>  _
    'Public Sub MyTestCleanup()
    'End Sub
    '
#End Region


    '''<summary>
    '''Una prueba de Execute
    '''</summary>
    <TestMethod()> _
    Public Sub ExecuteTest()
        Dim output As String
        output = _gacutil.Execute("/l")
        Assert.AreNotEqual(Nothing, output)
    End Sub

End Class
