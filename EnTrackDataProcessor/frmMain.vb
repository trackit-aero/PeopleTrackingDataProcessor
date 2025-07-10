Imports uPLibrary.Networking.M2Mqtt.Messages
Imports System.Text
Imports QuickType
Imports log4net
Imports System.Collections.Concurrent
Imports System.ComponentModel
Imports Newtonsoft.Json.Linq
Imports System.Net
Imports Microsoft.SqlServer
Imports System.Net.Mail
Imports System.IO

Public Class frmMain
    Public ReadOnly log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
    'Dim WithEvents client As uPLibrary.Networking.M2Mqtt.MqttClient
    Dim clients As New Dictionary(Of String, uPLibrary.Networking.M2Mqtt.MqttClient)
    Private WithEvents connectionCheckTimer As Timer

    Dim clientID As String
    Dim TrackBar2Value As Integer = 0
    Public tagsSeen As New BlockingCollection(Of TagRead)
    'Dim hshEndPoint As New Hashtable
    Dim hshEndPoint As New Dictionary(Of String, EndPointTopic)
    Dim hshLoc As New Hashtable
    Dim hshRead As New Hashtable
    Public hshLastLoc As New Hashtable
    Public hshLastLocTime As New Hashtable
    Dim hshTags As New Dictionary(Of String, TagData)
    Dim qta As New DBTableAdapters.QueriesTableAdapter
    Dim LiveMinuteBuffer As Int16 = 0
    Dim WithEvents msmq As Messaging.MessageQueue
    Dim lastRefresh As DateTime = DateTime.Today.AddDays(-2)
    Dim lastEmail As DateTime = DateTime.Today.AddDays(-2)
    Dim MQTTServer As String = My.Settings.MQTTServer
    Dim MQTTPort As Int16 = My.Settings.MQTTPort
    Dim MQTTTopic As String = My.Settings.MQTTTopic
    Dim rta As New DBTableAdapters.ReaderTableAdapter
    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        tmrScan.Enabled = False
        connectionCheckTimer.Stop() ' Stop the timer when closing

        If Not IsNothing(clients) Then
            For Each k As String In clients.Keys
                Dim client As uPLibrary.Networking.M2Mqtt.MqttClient = clients(k)
                Try
                    If Not IsNothing(client) Then
                        For Each p As String In hshEndPoint.Keys
                            Dim topic As EndPointTopic = hshEndPoint(p)
                            If topic.Server = k Then
                                Try
                                    client.Unsubscribe(New String() {topic.Topic})
                                Catch ex As Exception
                                    MessageBox.Show($"Error Unsubscribing to topic {topic.Topic} on broker {k}: {ex.Message}")
                                End Try
                            End If

                        Next
                    End If

                Catch ex As Exception
                    MessageBox.Show($"Error Unsubscribing topic on broker {k}: {ex.Message}")
                End Try
            Next
        End If


        If btnStart.Text = "Stop" Then
            btnStart_Click(Nothing, Nothing)
        End If
    End Sub
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Try
            InitializeDevices()
            InitializeTags()
            tmrScan.Interval = My.Settings.BucketPeriodInSec * 1000
            tmrScan.Enabled = True
        Catch ex As Exception

        End Try


        Try
            Dim client As New uPLibrary.Networking.M2Mqtt.MqttClient(MQTTServer)
            AddHandler client.MqttMsgPublishReceived, AddressOf client_MqttMsgPublishReceived
            Try
                client.Connect(Guid.NewGuid.ToString())
                clients.Add(MQTTServer, client)
            Catch ex As Exception
                MessageBox.Show($"Error connecting to broker {MQTTServer}: {ex.Message}")
            End Try
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

        ' Initialize the timer
        connectionCheckTimer = New Timer()
        connectionCheckTimer.Interval = 10000 ' Set interval to 10 seconds
        connectionCheckTimer.Start() ' Start the timer
    End Sub
    Private Sub connectionCheckTimer_Tick(sender As Object, e As EventArgs) Handles connectionCheckTimer.Tick
        CheckConnection() ' Call the CheckConnection method periodically
    End Sub

    Private Sub CheckConnection()
        If btnStart.Text = "Stop" Then
            For Each kvp In hshEndPoint
                Dim server = kvp.Value.Server

                If clients.ContainsKey(server) Then
                    Dim client As uPLibrary.Networking.M2Mqtt.MqttClient = clients(server)
                    If Not (client.IsConnected) Then
                        log.Error("Client is not connected to the MQTT broker.")
                        Try
                            client.Connect(client.ClientId)
                            log.Error("Reconnected to the MQTT broker.")

                        Catch ex As Exception
                            log.Error("Reconnect failed: " & ex.Message)

                        End Try
                    End If

                    If (client.IsConnected) Then
                        Dim topic As EndPointTopic = kvp.Value
                        Try
                            client.Subscribe(New String() {topic.Topic}, New Byte() {2})
                        Catch ex As Exception
                            MessageBox.Show($"Error subscribing to topic {topic.Topic} on broker {server}: {ex.Message}")
                        End Try
                    End If
                End If
            Next
        End If
    End Sub
    Private Sub InitializeDevices()
        Try
            Dim ta As New DBTableAdapters.vwActiveReaderTableAdapter
            Dim dt As DB.vwActiveReaderDataTable = ta.GetData()
            Dim arr As New ArrayList
            ComboBox1.Items.Clear()

            'hshEndPoint.Clear()
            hshEndPoint.Clear()
            For Each rw As DB.vwActiveReaderRow In dt.Rows

                Dim readerType As String = If(rw.IsReaderTypeNull(), "", rw.ReaderType)
                Dim macAddress As String = If(rw.IsReaderCodeNull(), "", rw.ReaderCode.ToLower())
                Dim locationId As String = If(rw.IsLocationIdNull(), 0, rw.LocationId)

                arr.Add(rw.ReaderId.ToString() & "|" & readerType & "|" & macAddress & "|" & locationId.ToString() & "|" & rw.LocationName)
                ComboBox1.Items.Add(readerType & " > " & macAddress & " > " & rw.LocationName)

                If Not hshEndPoint.ContainsKey(macAddress) Then
                    Dim topic As New EndPointTopic With {
                    .Server = MQTTServer,
                        .Port = MQTTPort,
                        .Topic = "/" + readerType + "/" + macAddress + "/" + MQTTTopic
                    }
                    hshEndPoint.Add(macAddress, topic)
                End If


                If Not hshLoc.Contains(macAddress) Then
                    Dim l As New Reader With {
                            .GatewayKey = macAddress,
                            .GatewayType = readerType,
                            .GatewayId = rw.ReaderId,
                            .LocID = locationId,
                            .LocName = rw.LocationName ',.MinRSSI = rw.RSSISensitivity
                        }
                    hshLoc.Add(macAddress, l)
                End If
            Next

            ComboBox1.Tag = arr
            ComboBox1.SelectedIndex = 0
        Catch ex As Exception
            log.Debug(ex.Message)
        End Try
    End Sub

    Private Sub InitializeTags()
        Try
            Dim ta As New DBTableAdapters.vwActiveTagTableAdapter
            Dim plta As New DBTableAdapters.vwPersonLocationTableAdapter
            Dim dt As DB.vwActiveTagDataTable = ta.GetData
            Dim arr As New ArrayList
            ComboBox2.Items.Clear()
            For Each rw As DB.vwActiveTagRow In dt.Rows
                If rw.IsTagNumberNull Then
                    Continue For
                End If

                arr.Add(rw.TagNumber.ToLower())
                ComboBox2.Items.Add(If(rw.IsPersonIdNull(), " ", rw.PersonName) & " > " & rw.TagNumber.ToLower())

                If Not hshTags.ContainsKey(rw.TagNumber) Then
                    Dim b As New TagData With {
                        .TagID = rw.TagId,
                        .TagNumber = rw.TagNumber.ToLower(),
                        .Occupied = If(rw.IsPersonIdNull(), False, True),
                        .PersonID = If(rw.IsPersonIdNull(), Nothing, rw.PersonId),
                        .PersonName = If(rw.IsPersonIdNull(), Nothing, rw.PersonName),
                        .PersonType = If(rw.IsPersonIdNull(), Nothing, rw.PersonType),
                        .GatewayID = Nothing,
                        .LastLocationId = If(rw.IsLastSeenAtNull(), Nothing, rw.LastSeenAt),
                        .LastSeenTime = If(rw.IsLastSeenTimeNull(), Nothing, rw.LastSeenTime)
                    }
                    If (b.Occupied) Then
                        Dim pldt As DataTable = plta.GetData(b.PersonID)
                        b.ValidLocId = pldt.AsEnumerable() _
                               .Select(Function(row) Convert.ToInt32(row("LocationId"))) _
                               .ToList()
                    End If
                    hshTags.Add(rw.TagNumber.ToLower(), b)
                End If
            Next
            ComboBox2.Tag = arr
            ComboBox2.SelectedIndex = 0
        Catch ex As Exception
        End Try

    End Sub
    Private Sub client_MqttMsgPublishReceived(sender As Object, e As MqttMsgPublishEventArgs)
        Try
            Dim validMokoPacket As Boolean = False
            Dim validMqPacket As Boolean = False

            For Each p As String In hshEndPoint.Keys
                Dim topic As EndPointTopic = hshEndPoint(p)
                If topic.Topic = e.Topic Then
                    If (e.Topic.Contains("MKGW4")) Then
                        validMokoPacket = True
                    Else
                        validMqPacket = True
                    End If
                    Exit For
                End If
            Next

            If Not (validMokoPacket Or validMqPacket) Then
                Exit Sub
            End If

            If validMokoPacket Then
                Dim ReceivedMessage As String = BitConverter.ToString(e.Message).Replace("-", "")

                If (Convert.ToBoolean(My.Settings.LogData)) Then
                    log.Debug("Received Message:" & ReceivedMessage)
                End If

                Dim str = mdMoko.HandlePayload(ReceivedMessage, "", 0)
                'convert json to object
                If ReceivedMessage.Substring(2, 4) = "30A0" Then
                    Dim q As MokoPacket = MokoPacket.FromJson(str)

                    Dim gatewayKey As String = q.gatewayMac
                    Dim g As Reader = hshLoc(gatewayKey)

                    If IsNothing(g) Then
                        log.Error("No gateway recieved:" & ReceivedMessage)
                    Else
                        rta.UpdateStatus(DateTime.Now, "Connected", g.GatewayId)
                        For Each x In q.deviceArray
                            AddTag(x, g)
                        Next
                    End If
                End If
            Else

                Dim ReceivedMessage As String = Encoding.UTF8.GetString(e.Message)

                If (Convert.ToBoolean(My.Settings.LogData)) Then
                    log.Debug("Received Message:" & ReceivedMessage)
                End If

                Dim q() As MqPacket = MqPacket.FromJson(ReceivedMessage)
                Dim g As MqPacket
                For Each x In q
                    If x.Type = "Gateway" Then
                        g = x
                        Exit For
                    End If
                Next

                If IsNothing(g) Then
                    log.Error("No gateway recieved:" & ReceivedMessage)
                Else
                    Dim macAddress As String = If(IsDBNull(g.Mac), "", g.Mac)
                    Dim gw As Reader = hshLoc(macAddress)

                    If IsNothing(gw) Then
                        'log.Debug("Unknown Gateway:" & t.MacID & ">" & t.Gateway)
                        Exit Sub
                    End If

                    rta.UpdateStatus(DateTime.Now, "Connected", gw.GatewayId)
                    For Each x In q
                        'If x.Rssi < gw.MinRSSI Then
                        '    Continue For
                        'End If
                        Select Case x.Type
                            Case "BXP-iBeacon"
                                AddTag(x, gw)
                            Case "BXP-DeviceInfo"
                                AddTag(x, gw)
                            Case "iBeacon"
                                AddTag(x, gw)
                            Case "Gateway"
                            Case Else
                                AddTag(x, gw)
                        End Select
                    Next
                End If
            End If

        Catch ex As Exception
            log.Error("Error in client_MqttMsgPublishReceived: " & ex.Message)
        End Try
    End Sub

    Private Sub AddTag(x As Devicearray, g As Reader)


        Dim batteryString As String = x.battVoltage
        Dim batteryNumeric As Integer? = Nothing


        If x.mac.ToUpper() = "C300004FB5E7" Then
            batteryString = x.battVoltage
        End If

        If Not IsNothing(batteryString) Then
            batteryNumeric = 0
            ' Use regex to remove non-digit characters
            Dim digitsOnly As String = System.Text.RegularExpressions.Regex.Replace(batteryString, "[^\d]", "")

            ' Try to parse it into an integer
            If Integer.TryParse(digitsOnly, batteryNumeric) = False Then
                batteryNumeric = Nothing ' fallback if parsing fails
            End If
        End If

        Dim t As New TagRead With {
            .TagNumber = x.mac,
            .RSSI = x.rssi,
            .ReadTime = Now,
            .RawData = x.advPacket,
            .GatewayKey = g.GatewayKey,
            .GatewayType = g.GatewayType,
            .GatewayID = g.GatewayId,
            .LocationId = g.LocID,
            .LocationName = g.LocName,
            .Battery = batteryNumeric,
            .Distance = x.rangingData
        }

        Dim oth = HandleRawData(x.advPacket)

        If Not IsNothing(oth) Then
            If Not IsNothing(oth.Battery) And IsNothing(t.Battery) Then
                t.Battery = oth.Battery.ToString
            End If
            If Not IsNothing(oth.RangingDistance) And IsNothing(t.Distance) Then
                t.Distance = oth.RangingDistance.ToString
            End If
        End If

        tagsSeen.Add(t)
    End Sub

    Private Sub AddTag(x As MqPacket, g As Reader)

        Dim batteryString As String = x.Battery
        Dim batteryNumeric As Integer? = Nothing

        If x.Mac.ToUpper() = "C300004FB5E7" Then
            batteryString = x.Battery
        End If

        If Not IsNothing(batteryString) Then
            batteryNumeric = 0
            ' Use regex to remove non-digit characters
            Dim digitsOnly As String = System.Text.RegularExpressions.Regex.Replace(batteryString, "[^\d]", "")

            ' Try to parse it into an integer
            If Integer.TryParse(digitsOnly, batteryNumeric) = False Then
                batteryNumeric = Nothing ' fallback if parsing fails
            End If
        End If

        Dim macAddress As String = If(IsDBNull(g.GatewayKey), "", g.GatewayKey)
        Dim t As New TagRead With {
            .TagNumber = x.Mac,
            .RSSI = x.Rssi,
            .ReadTime = Now,
            .RawData = x.RawData,
            .GatewayKey = g.GatewayKey,
            .GatewayType = g.GatewayType,
            .GatewayID = g.GatewayId,
            .LocationId = g.LocID,
            .LocationName = g.LocName,
            .Battery = batteryNumeric,
            .Distance = x.RangingDistance
        }

        Dim oth = HandleRawData(x.RawData)

        If Not IsNothing(oth) Then
            If Not IsNothing(oth.Battery) And IsNothing(t.Battery) Then
                t.Battery = oth.Battery.ToString
            End If
            If Not IsNothing(oth.RangingDistance) And IsNothing(t.Distance) Then
                t.Distance = oth.RangingDistance.ToString
            End If
        End If

        tagsSeen.Add(t)
    End Sub
    Private Function HandleRawData(rawData As String) As OtherInfo

        If IsNothing(rawData) Then
            Return Nothing
        End If
        Dim othDetails As OtherInfo = New OtherInfo

        Dim hexStrArray As List(Of String) = HexConverter.ToHexStrArray(rawData.ToUpper())
        Dim len As Integer = hexStrArray.Count
        If len >= 11 Then
            Dim data As New Dictionary(Of String, Object)
            data("TOFCode") = String.Join("", hexStrArray.GetRange(5, 4)) 'mfgCode 2 bytes + beaconCode 2 bytes
            data("M2Code") = String.Join("", hexStrArray.GetRange(8, 3))  'UUID 2 bytes + frameType 1 byte

            Select Case data("TOFCode")
                Case "5900076C"
                    ' Device info
                    othDetails.Battery = ParseHexArrayLE(9, 2, hexStrArray) ' & "mV"
                    othDetails.RangingDistance = ParseHexArrayLE(14, 2, hexStrArray) ' & "mm"
            End Select
            Select Case data("M2Code")
                Case "ABFE40"
                    othDetails.Battery = HexConverter.ParseHexStrArrayToInt(hexStrArray.Skip(13).Take(2).ToArray()) ' & "mV"
            End Select
        End If
        Return othDetails
    End Function
    Function ParseHexArrayLE(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String)) As Integer

        Dim hexStrArray As String() = deviceDataArray.Skip(deviceDataIndex).Take(deviceDataLength).ToArray()
        ' Create a byte array from the hex strings in the list, reversing for little-endian
        Dim reverseHexStrArray As List(Of String) = hexStrArray.AsEnumerable().Reverse().ToList()

        ' Convert the byte array to an integer
        Return HexConverter.ParseHexStrArrayToInt(reverseHexStrArray)
    End Function

    Private Sub SubscribeAll()
        If Not IsNothing(clients) Then

            For Each k As String In clients.Keys
                Dim client As uPLibrary.Networking.M2Mqtt.MqttClient = clients(k)
                Try
                    If Not IsNothing(client) Then

                        For Each kvp In hshEndPoint
                            Dim server = kvp.Value.Server

                            If k = server Then
                                If Not (client.IsConnected) Then
                                    log.Error("Client is not connected to the MQTT broker.")
                                    Try
                                        client.Connect(client.ClientId)
                                        log.Error("Reconnected to the MQTT broker.")

                                    Catch ex As Exception
                                        log.Error("Reconnect failed: " & ex.Message)

                                    End Try
                                End If

                                If (client.IsConnected) Then
                                    Dim topic As EndPointTopic = kvp.Value
                                    Try
                                        client.Subscribe(New String() {topic.Topic}, New Byte() {2})
                                    Catch ex As Exception
                                        MessageBox.Show($"Error subscribing to topic {topic.Topic} on broker {server}: {ex.Message}")
                                    End Try
                                End If
                            End If
                        Next

                    End If
                Catch ex As Exception
                    MessageBox.Show($"Error Unsubscribing topic on broker {k}: {ex.Message}")
                End Try
            Next
        End If
    End Sub
    Private Sub btnStart_Click(sender As Object, e As EventArgs) Handles btnStart.Click
        If btnStart.Text = "Start" Then
            btnStart.Text = "Stop"

            SubscribeAll()

            tmrScan.Enabled = True
        Else
            tmrScan.Enabled = False
            btnStart.Text = "Start"

            For Each k As String In clients.Keys
                Dim client As uPLibrary.Networking.M2Mqtt.MqttClient = clients(k)
                Try
                    If Not IsNothing(client) Then
                        If client.IsConnected Then
                            client.Disconnect()
                        End If
                    End If
                Catch ex As Exception
                    MessageBox.Show($"Error while disconencting client {k}: {ex.Message}")
                End Try
            Next
        End If
    End Sub
    Private Sub GetReaders()
        Try
            hshLoc.Clear()
            Dim ta As New DBTableAdapters.vwActiveReaderTableAdapter
            Dim dt As DB.vwActiveReaderDataTable = ta.GetData()
            For Each rw As DB.vwActiveReaderRow In dt.Rows

                Dim macAddress As String = If(rw.IsReaderCodeNull(), "", rw.ReaderCode.ToLower())

                If Not hshLoc.Contains(macAddress) Then
                    Dim l As New Reader With {
                            .GatewayKey = macAddress,
                            .GatewayType = rw.ReaderType,
                            .GatewayId = rw.ReaderId,
                            .LocID = rw.LocationId,
                            .LocName = rw.LocationName ', .MinRSSI = rw.RSSISensitivity
                        }
                    hshLoc.Add(macAddress, l)
                End If
            Next
            'log.Debug("Readers:" & hshLoc.Count)
        Catch ex As Exception
            log.Debug(ex.Message, ex)
        End Try
    End Sub
    Private Sub GetTags()
        Try
            hshTags.Clear()
            Dim ta As New DBTableAdapters.vwActiveTagTableAdapter
            Dim plta As New DBTableAdapters.vwPersonLocationTableAdapter
            Dim dt As DB.vwActiveTagDataTable = ta.GetData
            'log.Debug("Allocations:" & dt.Rows.Count)
            For Each rw As DB.vwActiveTagRow In dt.Rows
                If rw.IsTagNumberNull Then
                    Continue For
                End If
                If Not hshTags.ContainsKey(rw.TagNumber.ToLower()) Then
                    Dim b As New TagData With {
                        .TagID = rw.TagId,
                        .TagNumber = rw.TagNumber.ToLower(),
                        .Occupied = If(rw.IsPersonIdNull(), False, True),
                        .PersonID = If(rw.IsPersonIdNull(), Nothing, rw.PersonId),
                        .PersonName = If(rw.IsPersonIdNull(), Nothing, rw.PersonName),
                        .PersonType = If(rw.IsPersonIdNull(), Nothing, rw.PersonType),
                        .GatewayID = Nothing,
                        .LastLocationId = If(rw.IsLastSeenAtNull(), Nothing, rw.LastSeenAt),
                        .LastSeenTime = If(rw.IsLastSeenTimeNull(), Nothing, rw.LastSeenTime)
                    }

                    If (b.Occupied) Then
                        Dim pldt As DataTable = plta.GetData(b.PersonID)
                        b.ValidLocId = pldt.AsEnumerable() _
                               .Select(Function(row) Convert.ToInt32(row("LocationId"))) _
                               .ToList()
                    End If
                    hshTags.Add(rw.TagNumber.ToLower(), b)
                End If
            Next
        Catch ex As Exception
            log.Error(ex.Message, ex)
        End Try
    End Sub
    Private Sub tmrScan_Tick(sender As Object, e As EventArgs) Handles tmrScan.Tick
        If Now.Subtract(lastRefresh).TotalSeconds > 120 Then
            GetReaders()
            GetTags()
            lastRefresh = Now
        End If

        Dim ReaderNotificationMinute As Int16 = My.Settings.ReaderNotificationMinute
        If Now.Subtract(lastEmail).TotalMinutes > ReaderNotificationMinute Then
            Dim ta As New DBTableAdapters.vwActiveReaderTableAdapter
            Dim dt As DB.vwActiveReaderDataTable = ta.GetDataByOffline(DateTime.Now)
            If dt.Rows.Count > 0 Then
                If Not bgEmail.IsBusy Then
                    lastEmail = Now
                    bgEmail.RunWorkerAsync(dt)
                End If
            End If
        End If

        'End If
        'End If
        If Not bgProcess.IsBusy Then
            bgProcess.RunWorkerAsync()
        End If
    End Sub
    Dim lst As New List(Of TagRead)

    Private Sub bgEmail_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles bgEmail.DoWork
        Dim dt As DB.vwActiveReaderDataTable = e.Argument
        Dim str As String = "The following readers are offline:<br><br>"
        For Each rw As DB.vwActiveReaderRow In dt.Rows
            str &= "<b>" & rw.ReaderCode & ", located at " & rw.LocationName & "</b><br>"
        Next
        str &= "<br>Please check the offline devices configuration and power/connectivity<br><br>-----EnTrackPeople-----"
        MailUtils.SendEmail(My.Settings.RouteEmailTo.Split(","), "Reader Offline - EnTrackPersonSuite", str)
    End Sub

    Private Sub AutoSizeList(ByRef lst As ListView)
        For i As Integer = 0 To lst.Columns.Count - 1
            lst.Columns(i).Width = -2
        Next
    End Sub
    Private Sub bgProcess_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles bgProcess.DoWork
        'ProcessTags()

        ''hshLoc has device details against MACAddress key
        ''hshTags has Person details against TagNo key

        hshRead.Clear()
        ''tagsSeen  has tag(TagRead) details from device reading
        Dim x As Integer = tagsSeen.Count
        'log.Debug("Begin Calculations." & x & " reads...")
        For i As Integer = 1 To x
            Dim t As TagRead = tagsSeen.Take
            If hshTags.ContainsKey(t.TagNumber) Then 'Validate the read tag  
                'Check GW
                If hshRead.Contains(t.TagNumber) Then
                    Dim z As TagRead = hshRead(t.TagNumber)
                    If t.RSSI > z.RSSI Then
                        t.TagID = z.TagID
                        t.PersonID = z.PersonID
                        t.PersonName = z.PersonName
                        t.PersonType = z.PersonType
                        t.Occupied = z.Occupied
                        'Get Last Seen details from TagRead hshRead
                        hshRead(t.TagNumber) = t
                    End If
                Else
                    Dim bt As TagData = hshTags(t.TagNumber)
                    t.TagID = bt.TagID
                    If (bt.Occupied) Then
                        t.Occupied = bt.Occupied
                        t.PersonID = bt.PersonID
                        t.PersonName = bt.PersonName
                        t.PersonType = bt.PersonType
                        If (bt.ValidLocId.Contains(t.LocationId)) Then
                            t.Exception = False
                        Else
                            t.Exception = True
                        End If
                    End If
                    'Get Last Seen details from TagRead hshTags
                    hshRead.Add(t.TagNumber, t)
                End If
            Else
                'log.Debug("Unknown Tag:" & t.MacID & ">" & t.Gateway)
            End If

        Next
        'log.Debug("Tags Found : " & hTagList.Count)

        'Process Person Tag Reads
        'Check if LastLoc was same
        Dim mta As New DBTableAdapters.MovementHistoryTableAdapter
        Dim pta As New DBTableAdapters.PersonTableAdapter
        Dim tta As New DBTableAdapters.TagTableAdapter
        'Post to DB if different
        For Each s As String In hshRead.Keys
            Dim r As TagRead = hshRead(s)

            If (r.Occupied) Then
                Dim ActiveMinutesBuffer As Int16 = My.Settings.ActiveMinutesBuffer
                Dim mdt As DB.MovementHistoryDataTable = mta.GetLast2ReadData(r.PersonID)

                If mdt.Rows.Count > 0 Then
                    Dim mprw As DB.MovementHistoryRow = mdt.Rows(0)
                    If (DateDiff(DateInterval.Minute, mprw.EndTime, r.ReadTime) > ActiveMinutesBuffer) Then
                        mta.Insert(r.PersonID, r.TagID, r.ReadTime, r.ReadTime.AddSeconds(1), r.LocationId, r.GatewayID, r.Exception, r.RSSI, r.Battery, r.RawData, "")
                    Else
                        If (mprw.LocationId = r.LocationId) Then
                            If (mprw.EndTime.Day <> r.ReadTime.Day) Then
                                Dim lastSecondOfDay As DateTime = mprw.EndTime.Date.AddDays(1).AddSeconds(-1)
                                mta.UpdateLastReadTime(lastSecondOfDay, r.RSSI, "RD :" + r.RawData + ", Battery :" + r.Battery.ToString() + ", Distance :" + r.Distance.ToString(), mprw.TID)
                                Dim firstSecondOfDay As DateTime = r.ReadTime.Date
                                mta.Insert(r.PersonID, r.TagID, firstSecondOfDay, r.ReadTime, r.LocationId, r.GatewayID, r.Exception, r.RSSI, r.Battery, r.RawData, "")
                            Else
                                mta.UpdateLastReadTime(r.ReadTime, r.RSSI, "RD :" + r.RawData + ", Battery :" + r.Battery.ToString() + ", Distance :" + r.Distance.ToString(), mprw.TID)
                            End If
                        Else
                            mta.Insert(r.PersonID, r.TagID, r.ReadTime, r.ReadTime.AddSeconds(1), r.LocationId, r.GatewayID, r.Exception, r.RSSI, r.Battery, r.RawData, "")
                        End If
                    End If
                Else
                    mta.Insert(r.PersonID, r.TagID, r.ReadTime, r.ReadTime.AddSeconds(1), r.LocationId, r.GatewayID, r.Exception, r.RSSI, r.Battery, r.RawData, "")
                End If

                Try
                    pta.UpdateTagReadInfo(r.ReadTime, r.LocationId, r.PersonID)

                    If Not (r.Battery Is Nothing) Then
                        'Update current location of Person and refresh hshTag
                        tta.UpdateTagReadInfoWB(r.ReadTime, r.LocationId, r.Battery, r.TagID)
                    Else
                        'Update current location of Person and refresh hshTag
                        tta.UpdateTagReadInfoNB(r.ReadTime, r.LocationId, r.TagID)
                    End If

                Catch ex As Exception
                    log.Error(ex.Message, ex)
                End Try
            Else
                Try
                    If Not (r.Battery Is Nothing) Then
                        'Update current location of Person and refresh hshTag
                        tta.UpdateTagReadInfoWB(r.ReadTime, r.LocationId, r.Battery, r.TagID)
                    Else
                        'Update current location of Person and refresh hshTag
                        tta.UpdateTagReadInfoNB(r.ReadTime, r.LocationId, r.TagID)
                    End If

                Catch ex As Exception
                    log.Error(ex.Message, ex)
                End Try
            End If

            If hshLastLoc.Contains(r.TagNumber) Then
                If hshLastLoc(r.TagNumber) <> r.LocationId Then
                    hshLastLoc(r.TagNumber) = r.LocationId
                    hshLastLocTime(r.TagNumber) = r.ReadTime
                End If
            Else
                hshLastLoc.Add(r.TagNumber, r.LocationId)
                hshLastLocTime.Add(r.TagNumber, r.ReadTime)
            End If
        Next

    End Sub
    Private Sub bgProcess_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles bgProcess.RunWorkerCompleted
        'Update List
        For Each k As String In hshRead.Keys
            Dim t As TagRead = hshRead(k)
            Dim flg As Boolean = False

            Dim deviceType As String = t.GatewayType
            Dim DeviceKey As String = t.GatewayKey
            Dim LocationName As String = If(IsDBNull(t.LocationId), "N/A", t.LocationName)
            Dim Battery As String = If(IsDBNull(t.Battery), "", t.Battery)
            Dim DeviceDetails As String = deviceType & " > " & DeviceKey & " > " & LocationName

            For i As Integer = 0 To ListView1.Items.Count - 1
                If t.TagNumber = ListView1.Items(i).Tag Then
                    'Update
                    flg = True
                    ListView1.Items(i).SubItems(1).Text = t.PersonName
                    ListView1.Items(i).SubItems(2).Text = DeviceDetails
                    ListView1.Items(i).SubItems(3).Text = Date.Parse(hshLastLocTime(t.TagNumber)).ToString("dd MMM HH:mm:ss")
                    ListView1.Items(i).SubItems(4).Text = t.ReadTime.ToString(("dd MMM HH:mm:ss"))
                    ListView1.Items(i).SubItems(5).Text = t.PersonType
                    ListView1.Items(i).SubItems(6).Text = Battery
                    Exit For
                End If
            Next
            If Not flg Then
                Dim li As New ListViewItem(t.TagNumber)
                li.SubItems.Add(t.PersonName)
                li.SubItems.Add(DeviceDetails)
                li.SubItems.Add(t.ReadTime.ToString("dd MMM HH:mm:ss"))
                li.SubItems.Add(t.ReadTime.ToString("dd MMM HH:mm:ss"))
                li.SubItems.Add(t.PersonType)
                li.SubItems.Add(Battery)
                li.Tag = t.TagNumber
                ListView1.Items.Add(li)
            End If
        Next
    End Sub
    Private Sub tmrPost_Tick(sender As Object, e As EventArgs) Handles tmrPost.Tick
        For i As Integer = 0 To ListView4.Items.Count - 1
            Dim s() As String = ListView4.Items(i).Tag.ToString.Split(",")
            Dim d As New TagRead
            d.TagNumber = s(0)
            d.RSSI = ListView4.Items(i).SubItems(2).Text
            d.ReadTime = Now
            Dim str() As String = s(1).Split("|")
            d.GatewayID = str(0)
            d.GatewayType = str(1)
            d.GatewayKey = str(2)
            d.LocationId = str(3)
            d.LocationName = str(4)
            tagsSeen.Add(d)
        Next
    End Sub

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        ListView1.Items.Clear()
    End Sub
    Private Sub btnSend_Click(sender As Object, e As EventArgs) Handles btnSend.Click
        Dim ta As New DBTableAdapters.vwActiveReaderTableAdapter
        Dim dt As DB.vwActiveReaderDataTable = ta.GetDataByOffline(DateTime.Now)
        bgEmail.RunWorkerAsync(dt)
    End Sub

    Private Sub btnPost_Click(sender As Object, e As EventArgs) Handles btnPost.Click
        Dim d As New TagRead
        Dim arr As ArrayList = ComboBox2.Tag
        d.TagNumber = arr(ComboBox2.SelectedIndex)
        d.RSSI = NumericUpDown1.Value
        d.ReadTime = Now

        arr = ComboBox1.Tag
        Dim str() As String = arr(ComboBox1.SelectedIndex).ToString.Split("|")
        d.GatewayID = str(0)
        d.GatewayType = str(1)
        d.GatewayKey = str(2)
        d.LocationId = str(3)
        d.LocationName = str(4)
        tagsSeen.Add(d)
    End Sub

    Private Sub btnAddPostList_Click(sender As Object, e As EventArgs) Handles btnAddPostList.Click
        If Not tmrPost.Enabled Then
            tmrPost.Enabled = True
        End If
        Dim arrTag As ArrayList = ComboBox2.Tag
        Dim arrRdr As ArrayList = ComboBox1.Tag
        Dim flg As Boolean = False
        For i As Integer = 0 To ListView4.Items.Count - 1
            Dim x() As String = ListView4.Items(i).Tag.ToString.Split(",")
            If ListView4.Items(i).SubItems(1).Text = ComboBox2.Text Then
                flg = True
                ListView4.Items(i).SubItems(0).Text = ComboBox1.Text
                ListView4.Items(i).SubItems(2).Text = NumericUpDown1.Value
                ListView4.Items(i).Tag = arrTag(ComboBox2.SelectedIndex) & "," & arrRdr(ComboBox1.SelectedIndex)
                Exit For
            End If
        Next
        If Not flg Then
            Dim li As New ListViewItem(ComboBox1.Text)
            li.SubItems.Add(ComboBox2.Text)
            li.SubItems.Add(NumericUpDown1.Value)
            li.Tag = arrTag(ComboBox2.SelectedIndex) & "," & arrRdr(ComboBox1.SelectedIndex)
            ListView4.Items.Add(li)
        End If

        AutoSizeList(ListView4)
    End Sub

    Private Sub btnRemovePostList_Click(sender As Object, e As EventArgs) Handles btnRemovePostList.Click
        If ListView4.SelectedItems.Count = 0 Then
            Exit Sub
        End If
        For i As Integer = ListView4.Items.Count - 1 To 0 Step -1
            If ListView4.Items(i).Selected Then
                ListView4.Items.RemoveAt(i)
            End If
        Next
    End Sub

    Private Sub bgProcess_Disposed(sender As Object, e As EventArgs) Handles bgProcess.Disposed

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' Retrieve the full connection strings from the settings
        Dim connectionStringProcessor As String = Global.EnTrackxDataProcessor.My.MySettings.Default.ConnectionString

        ' Extract Data Source and Initial Catalog for Processor Config
        Dim dataSourceProcessor As String = GetConnectionStringValue(connectionStringProcessor, "Data Source")
        Dim initialCatalogProcessor As String = GetConnectionStringValue(connectionStringProcessor, "Initial Catalog")

        ' Construct the message
        Dim message As String = $"Processor Config: Data Source = {dataSourceProcessor}, Database = {initialCatalogProcessor}" & vbCrLf & vbCrLf &
                                $"MQTT Details: Server  = {MQTTServer}, Port = {MQTTPort}, Topic = {MQTTTopic}"

        ' Display the message in a message box
        MsgBox(message)
    End Sub

    ' Helper function to extract the value from the connection string
    Private Function GetConnectionStringValue(connectionString As String, key As String) As String
        ' Split the connection string into parts
        Dim parts As String() = connectionString.Split(";"c)
        For Each part As String In parts
            If part.Trim().StartsWith(key & "=", StringComparison.OrdinalIgnoreCase) Then
                Return part.Substring(key.Length + 1).Trim()
            End If
        Next
        Return String.Empty
    End Function

    Private Sub btnRefreshLoc_Click(sender As Object, e As EventArgs) Handles btnRefreshLoc.Click
        InitializeDevices()
    End Sub

    Private Sub btnRefreshTag_Click(sender As Object, e As EventArgs) Handles btnRefreshTag.Click
        InitializeTags()
    End Sub

    Private Sub btnClearPostList_Click(sender As Object, e As EventArgs) Handles btnClearPostList.Click
        ListView4.Items.Clear()
    End Sub

End Class
