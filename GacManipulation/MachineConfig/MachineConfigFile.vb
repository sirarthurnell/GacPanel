Imports System.Xml.XPath
Imports System.IO

''' <summary>
''' Representa el archivo de configuración
''' del framework.
''' </summary>
Public Class MachineConfigFile
    Implements IBindingDirectiveSource

    Private _framework As Framework
    Private _pathToFile As String
    Private _directives As List(Of BindingDirective)
    Private _md5 As String

    ''' <summary>
    ''' Crea una nueva instancia de MachineConfig.
    ''' </summary>
    ''' <param name="framework">Framework al que
    ''' pertenece el archivo.</param>
    Public Sub New(ByVal framework As Framework)
        _framework = framework
        _pathToFile = _framework.MachineConfigPath
    End Sub

    ''' <summary>
    ''' Carga el archivo machine.config.
    ''' </summary>
    Public Sub Load()
        Dim text As String = ReadText()
        _md5 = Cryptography.ComputeMd5(text)

        Dim machineFile As XDocument = XDocument.Parse(text)
        Dim ns As XNamespace = "urn:schemas-microsoft-com:asm.v1"
        Dim allDirectives = machineFile.Descendants(ns + "dependentAssembly") _
                            .Select(Function(dependentAssembly)
                                        Dim name As String = dependentAssembly.Element(ns + "assemblyIdentity").Attribute("name").Value
                                        Dim token As String = dependentAssembly.Element(ns + "assemblyIdentity").Attribute("publicKeyToken").Value
                                        Dim newDirective As New BindingDirective(name, token)

                                        Dim redirections = (From redirect In dependentAssembly.Elements(ns + "bindingRedirect")) _
                                                           .Select(Function(y)
                                                                       Dim range As New BindingVersionRange(y.Attribute("oldVersion"))
                                                                       Dim target As New BindingVersion(y.Attribute("newVersion"))
                                                                       Dim redirection As New BindingRedirect(range, target)
                                                                       Return redirection
                                                                   End Function)

                                        newDirective.Redirections.AddRange(redirections)

                                        Dim installedVersions = _framework.Gac.GetVersionsForAssembly(newDirective.Name)
                                        For Each installedVersion In installedVersions
                                            newDirective.InstalledVersions.Add(New BindingVersion(installedVersion))
                                        Next

                                        Return newDirective
                                    End Function)

        _directives = allDirectives.ToList()
    End Sub

    ''' <summary>
    ''' Obtiene la firma Md5 del archivo cargado.
    ''' </summary>
    Public ReadOnly Property Md5 As String
        Get
            Return _md5
        End Get
    End Property

    ''' <summary>
    ''' Lee el texto del archivo machine.config.
    ''' </summary>
    ''' <returns>Texto del archivo machine.config.</returns>
    Private Function ReadText() As String
        Using sr As New StreamReader(_pathToFile)
            Return sr.ReadToEnd()
        End Using
    End Function

    ''' <summary>
    ''' Directivas de enlace declaradas en el archivo
    ''' machine.config.
    ''' </summary>
    Public ReadOnly Property Directives As List(Of BindingDirective) Implements IBindingDirectiveSource.Directives
        Get
            Return _directives
        End Get
    End Property

    ''' <summary>
    ''' Guarda las directivas de enlace en el archivo
    ''' machine.config.
    ''' </summary>
    Public Sub SaveDirectives()
        Dim machineFile As XDocument = XDocument.Load(_pathToFile)
        Dim ns As XNamespace = "urn:schemas-microsoft-com:asm.v1"
        Dim oldAssemblyBinding As XElement = machineFile.Descendants(ns + "assemblyBinding").FirstOrDefault()

        Dim translator As New BindingDirectiveTranslator()
        oldAssemblyBinding.ReplaceNodes(translator.ToXml(Directives, ns))
        machineFile.Save(_pathToFile, SaveOptions.OmitDuplicateNamespaces)

        Load()
    End Sub

End Class
