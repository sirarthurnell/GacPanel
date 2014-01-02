''' <summary>
''' Traduce objetos de directiva de enlace a su
''' representación en XML.
''' </summary>
Public Class BindingDirectiveTranslator

    ''' <summary>
    ''' Traduce una colección de directivas de enlace
    ''' a su representación en XML.
    ''' </summary>
    ''' <param name="directives">Directivas de enlace
    ''' a traducir.</param>
    ''' <param name="ns">Espacio de nombres a usar.</param>
    ''' <returns>Directivas de enlace en XML.</returns>
    Public Function ToXml(ByVal directives As IEnumerable(Of BindingDirective), ByVal ns As XNamespace) As IEnumerable(Of XElement)
        Dim translatedDirectives = From directive In directives
                                   Select ToXml(directive, ns)

        Return translatedDirectives
    End Function

    ''' <summary>
    ''' Traduce un objeto de directiva de enlace en un
    ''' nodo XML.
    ''' </summary>
    ''' <param name="directive">Directiva de enlace.</param>
    ''' <param name="ns">Espacio de nombres a usar.</param>
    ''' <returns>Directiva de enlace expresada como XML.</returns>
    Public Function ToXml(ByVal directive As BindingDirective, ByVal ns As XNamespace) As XElement
        Dim dependentAssembly As XElement =
            <dependentAssembly>
                <assemblyIdentity name=<%= directive.Name %> publicKeyToken=<%= directive.Token %>/>
                <%= From redirect In directive.Redirections Select <bindingRedirect oldVersion=<%= redirect.Range %> newVersion=<%= redirect.TargetVersion %>/> %>
            </dependentAssembly>

        'Aplicamos el espacio de nombres si lo hay.
        If Not ns Is Nothing Then
            dependentAssembly.Name = ns + dependentAssembly.Name.LocalName
            For Each element In dependentAssembly.Elements()
                element.Name = ns + element.Name.LocalName
            Next
        End If

        Return dependentAssembly
    End Function

End Class
