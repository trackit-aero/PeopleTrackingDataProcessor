Imports System.Collections.Concurrent
Imports Microsoft.VisualBasic.Logging
Imports System.Xml
Imports log4net
Imports System.Runtime.CompilerServices
Imports System.Data.SqlClient
Imports System.IO
Imports Google.Apis.Auth.OAuth2
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Public Module mdApp
    Public ReadOnly log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
    Public msgs As New BlockingCollection(Of PushMessage)
    Public dtPushNotify As DataTable = New DataTable()

    Public Class PushMessage
        Public Property username As String
        Public Property icon As String
        Public Property text As String
        Public Property dt As String
        Public Property tenantid As String
        Public Property Owner As String
        Public Property gseid As Integer
        Public Property userid As Integer
    End Class

    Public Enum TagType
        RFID
        BLE
    End Enum
    Public Enum DeviceType
        RFID
        BLE
    End Enum
    Public Class TagRead
        Public TagNumber As String
        Public TagID As Int32
        Public Battery As Int32?
        Public Distance As Int64?
        Public RSSI As Integer
        Public RawData As String
        Public ReadTime As DateTime
        Public GatewayKey As String
        Public GatewayType As String
        Public GatewayID As Int32
        Public LocationId As String
        Public LocationName As String
        Public Occupied As Boolean = False
        Public Exception As Boolean = False
        Public PersonID As String
        Public PersonName As String
        Public PersonType As String
    End Class
    Public Class TagList
        Public MacID As String
        Public ReadList As New List(Of TagRead)
    End Class
    Public Class LocList
        Public LocID As String
        Public ReadList As New List(Of TagRead)
    End Class
    Public Class TopicList
        Public TopicList As New List(Of EndPointTopic)
    End Class
    Public Class EndPointTopic
        Public Server As String
        Public Port As Integer
        Public Topic As String
    End Class
    Public Class TagData
        Public TagID As Integer
        Public TagNumber As String
        Public Occupied As Boolean = False
        Public PersonID As Integer
        Public PersonName As String
        Public PersonType As String
        Public GatewayID As Integer?
        Public LastLocationId As Integer?
        Public LastLocation As String
        Public LastSeenTime As DateTime?
        Public ValidLocId As New List(Of Int32)
    End Class
    Public Class Reader
        Public GatewayKey As String
        Public GatewayId As Integer
        Public GatewayType As String
        Public LocID As Integer
        Public LocName As String
    End Class
End Module
Public Class ListViewItemComparer
    Implements IComparer

    Private ColumnToSort As Integer
    Private OrderOfSort As SortOrder
    Private ObjectCompare As CaseInsensitiveComparer

    Public Sub New()
        ColumnToSort = 0
        OrderOfSort = SortOrder.Unspecified
        ObjectCompare = New CaseInsensitiveComparer()
    End Sub

    Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
        Dim compareResult As Integer
        Dim listviewX, listviewY As ListViewItem
        listviewX = CType(x, ListViewItem)
        listviewY = CType(y, ListViewItem)
        compareResult = ObjectCompare.Compare(listviewX.SubItems(ColumnToSort).Text, listviewY.SubItems(ColumnToSort).Text)

        If OrderOfSort = SortOrder.Ascending Then
            Return compareResult
        ElseIf OrderOfSort = SortOrder.Descending Then
            Return (-compareResult)
        Else
            Return 0
        End If
    End Function


    Public Property SortColumn As Integer
        Set(ByVal value As Integer)
            ColumnToSort = value
        End Set
        Get
            Return ColumnToSort
        End Get
    End Property

    Public Property Order As SortOrder
        Set(ByVal value As SortOrder)
            OrderOfSort = value
        End Set
        Get
            Return OrderOfSort
        End Get
    End Property
End Class

Public Module DocumentExtensions
    <Extension()>
    Function ToXmlDocument(ByVal xDocument As XDocument) As XmlDocument
        Dim xmlDocument = New XmlDocument()
        Using xmlReader = xDocument.CreateReader()
            xmlDocument.Load(xmlReader)
        End Using
        Return xmlDocument
    End Function
    <Extension()>
    Function ToXDocument(ByVal xmlDocument As XmlDocument) As XDocument
        Using nodeReader = New XmlNodeReader(xmlDocument)
            nodeReader.MoveToContent()
            Return XDocument.Load(nodeReader)
        End Using
    End Function
End Module
