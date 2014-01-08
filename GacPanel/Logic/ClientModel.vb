
Public Class InstalledVersion
    Public Property Parts() As List(Of Integer)
        Get
            Return m_Parts
        End Get
        Set(value As List(Of Integer))
            m_Parts = Value
        End Set
    End Property
    Private m_Parts As List(Of Integer)
End Class

Public Class LowerBound
    Public Property Parts() As List(Of Integer)
        Get
            Return m_Parts
        End Get
        Set(value As List(Of Integer))
            m_Parts = Value
        End Set
    End Property
    Private m_Parts As List(Of Integer)
End Class

Public Class UpperBound
    Public Property Parts() As List(Of Integer)
        Get
            Return m_Parts
        End Get
        Set(value As List(Of Integer))
            m_Parts = Value
        End Set
    End Property
    Private m_Parts As List(Of Integer)
End Class

Public Class Range
    Public Property LowerBound() As LowerBound
        Get
            Return m_LowerBound
        End Get
        Set(value As LowerBound)
            m_LowerBound = Value
        End Set
    End Property
    Private m_LowerBound As LowerBound
    Public Property UpperBound() As UpperBound
        Get
            Return m_UpperBound
        End Get
        Set(value As UpperBound)
            m_UpperBound = Value
        End Set
    End Property
    Private m_UpperBound As UpperBound
End Class

Public Class TargetVersion
    Public Property Parts() As List(Of Integer)
        Get
            Return m_Parts
        End Get
        Set(value As List(Of Integer))
            m_Parts = Value
        End Set
    End Property
    Private m_Parts As List(Of Integer)
End Class

Public Class Redirection
    Public Property Range() As Range
        Get
            Return m_Range
        End Get
        Set(value As Range)
            m_Range = Value
        End Set
    End Property
    Private m_Range As Range
    Public Property TargetVersion() As TargetVersion
        Get
            Return m_TargetVersion
        End Get
        Set(value As TargetVersion)
            m_TargetVersion = Value
        End Set
    End Property
    Private m_TargetVersion As TargetVersion
    Public Property Id() As String
        Get
            Return m_Id
        End Get
        Set(value As String)
            m_Id = Value
        End Set
    End Property
    Private m_Id As String
End Class

Public Class RootObject
    Public Property Name() As String
        Get
            Return m_Name
        End Get
        Set(value As String)
            m_Name = Value
        End Set
    End Property
    Private m_Name As String
    Public Property Token() As String
        Get
            Return m_Token
        End Get
        Set(value As String)
            m_Token = Value
        End Set
    End Property
    Private m_Token As String
    Public Property InstalledVersions() As List(Of InstalledVersion)
        Get
            Return m_InstalledVersions
        End Get
        Set(value As List(Of InstalledVersion))
            m_InstalledVersions = Value
        End Set
    End Property
    Private m_InstalledVersions As List(Of InstalledVersion)
    Public Property Redirections() As List(Of Redirection)
        Get
            Return m_Redirections
        End Get
        Set(value As List(Of Redirection))
            m_Redirections = Value
        End Set
    End Property
    Private m_Redirections As List(Of Redirection)
    Public Property State() As String
        Get
            Return m_State
        End Get
        Set(value As String)
            m_State = Value
        End Set
    End Property
    Private m_State As String
    Public Property ___id() As String
        Get
            Return m____id
        End Get
        Set(value As String)
            m____id = Value
        End Set
    End Property
    Private m____id As String
    Public Property ___s() As Boolean
        Get
            Return m____s
        End Get
        Set(value As Boolean)
            m____s = Value
        End Set
    End Property
    Private m____s As Boolean
End Class

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================
