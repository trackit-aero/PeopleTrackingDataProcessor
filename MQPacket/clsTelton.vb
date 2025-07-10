'Imports MongoDB.Bson
Imports System.Runtime.InteropServices
Imports System.Security.Cryptography
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.Tab

Public Class clsTeltonParser
    Public PackType As PacketType
    Public IMEI As String
    Public numRec As Integer = 0
    Public Records As New List(Of AVLRecord)
    Public codec As CodecType
    Public isValid As Boolean = True
    Public Sub New(data As Num)
        Try

            IMEI = data.Car
            Dim gpsTimeStamp As DateTime = epoch.AddSeconds(data.Udate).AddMinutes(180)
            Dim ignitionTimeStamp As DateTime = epoch.AddSeconds(data.IgnitionDate).AddMinutes(180)
            Dim lastSeenTimeStamp As DateTime = epoch.AddSeconds(data.LasttimeAnydata).AddMinutes(180)
            'Dim str() As String = data.Split(":")
            'Dim topic As String = str(0)
            ''Extract Unix Time First 4 bytes are unix time in reverse hex format
            'Dim UnixTime As String = str(1).Substring(0, 8)
            ''Reverse bytes
            'UnixTime = ReverseBytes(UnixTime)
            ''Convert to datetime
            'Dim timeStamp As DateTime = epoch.AddSeconds(Convert.ToInt32(UnixTime, 16)).AddMinutes(180)
            ''Extract IMEI - next 8 bytes after skipping 4 bytes for time are IMEI in reverse hex format
            'IMEI = str(1).Substring(8, 16)
            ''Reverse bytes
            'IMEI = ReverseBytes(IMEI)
            ''Convert to IMEI number from hex
            'IMEI = Convert.ToInt64(IMEI, 16).ToString 'Long.Parse(IMEI, Globalization.NumberStyles.HexNumber).ToString
            Dim v As New VehicleStatus
            Try
                If vlist.ContainsKey(IMEI) Then
                    v = vlist(IMEI)
                Else
                    log.Error("IMEI:" & IMEI & " not registered.")
                    Return
                End If
            Catch ex As Exception
                'log.Error("ER03:" & ex.Message, ex)
            End Try
            'Try
            '    If Not IO.Directory.Exists("C:\Log\" & IMEI) Then
            '        IO.Directory.CreateDirectory("C:\Log\" & IMEI)
            '    End If
            '    IO.File.AppendAllText("C:\Log\" & IMEI & "\" & IMEI & "-" & Today.ToString("yyyyMMdd") & ".txt", data & vbCrLf)
            'Catch ex As Exception
            'End Try
            Dim ds As DeviceStatus = mdGlobal.GetDeviceStatus(IMEI)
            If IsNothing(ds) Then
                ds = New DeviceStatus()
                ds.DeviceID = v.DeviceID
                ds.IMEI = IMEI
                ds.AssetID = v.AssetID
                ds.Latitude = 0
                ds.Longitude = 0
                ds.Speed = 0
                ds.Voltage = 0
                ds.DI1ChangeTime = ignitionTimeStamp
                mdGlobal.hashDevices.Add(IMEI, ds)
            End If

            If ds IsNot Nothing Then
                ds.LastSeen = lastSeenTimeStamp
                Dim rec As New AVLRecord
                With rec
                    .TStamp = ds.LastSeen
                    If .TStamp > Now Then
                        .TStamp = Now
                        log.Error(IMEI & ":Invalid time reported:" & .TStamp.ToString("dd MMM yyyy HH:mm:ss"))
                    End If

                    Dim Latitude = data.Lat
                    Dim Longitude = data.Lon
                    Dim Speed = data.Speed
                    Dim Satellites = 0 'Need to change this

                    ''Next 4 bytes are latitude in reverse hex format
                    'Dim Latitude = Convert.ToInt32(ReverseBytes(str(1).Substring(24, 8)), 16) / 1000000
                    ''Next 4 bytes are longitude in reverse hex format
                    'Dim Longitude = Convert.ToInt32(ReverseBytes(str(1).Substring(32, 8)), 16) / 1000000
                    ''ds.LastSeen = dt
                    ''Next 2 bytes are speed in reverse hex format
                    'Dim Speed = Convert.ToInt32(ReverseBytes(str(1).Substring(40, 4)), 16)
                    ''Next 1 byte is number of satellites
                    'Dim Satellites As Integer = Convert.ToInt32(str(1).Substring(44, 2), 16)
                    log.Debug(IMEI & ":GPS:" & Latitude & "/" & Longitude & "/SPD:" & Speed & ":SAT:" & Satellites)
                    ds.Satellite = Satellites
                    .Satellite = ds.Satellite
                    'Update Location entry and last seen time
                    If ds.Latitude <> Latitude Or ds.Longitude <> Longitude Or ds.Speed <> Speed Then
                        ds.Latitude = Latitude
                        ds.Longitude = Longitude
                        ds.Speed = Speed
                        'Record in database
                        dtDeviceEvents.Rows.Add(ds.DeviceID, EventType.Location, ds.LastSeen, ds.Latitude, ds.Longitude, ds.LocID, "", False, False, Nothing, Nothing, ds.AssetID, 0, ds.EnStat.ToString, ds.OStatus.ToString, "", 0, 0, ds.Voltage, ds.BV, ds.Speed, 0, 0, False, False, lastSeenTimeStamp.Subtract(epoch).TotalSeconds, ds.LastSeen.Subtract(epoch).TotalSeconds, 0)
                    Else
                        'Update last seen time
                        dtDeviceEvents.Rows.Add(ds.DeviceID, EventType.DeviceOnline, ds.LastSeen, ds.Latitude, ds.Longitude, ds.LocID, "", False, False, Nothing, Nothing, ds.AssetID, 0, ds.EnStat.ToString, ds.OStatus.ToString, "", 0, 0, ds.Voltage, ds.BV, ds.Speed, 0, 0, False, False, lastSeenTimeStamp.Subtract(epoch).TotalSeconds, ds.LastSeen.Subtract(epoch).TotalSeconds, 0)
                        'UpdateLiveLocation(ds.AssetID, ds.DeviceID, ds.VehicleID, ds.Latitude, ds.Longitude, ds.LastSeen, ds.LocName, ds.EnStat.ToString, ds.OStatus.ToString, ds.DriverID, ds.Voltage, ds.Speed, ds.Voltage, ds.BV)
                    End If
                    'Determine Geofence if any
                    Try
                        Dim sql As String = "select LocationID,LocationName from dbo.Location where geom.MakeValid().STContains(geometry::STGeomFromText('POINT(" & ds.Longitude & " " & ds.Latitude & ")', 4326))=1 order by LocationID desc"
                        Dim dt As DataTable = ExecSQL(sql)
                        If dt.Rows.Count > 0 Then
                            For Each rw As DataRow In dt.Rows
                                Dim el As New ELocation()
                                .Location.LocID = rw.Item(0)
                                .Location.LocName = rw.Item(1)
                                el.LocID = rw(0)
                                el.LocName = rw.Item(1)
                                'AllLocations.Add(el)
                            Next
                        Else
                            .Location.LocID = 0
                            .Location.LocName = ""
                        End If
                    Catch ex As Exception

                    End Try

                    If ds.Longitude = 0 And ds.Latitude = 0 Then
                        isValid = False
                        Return
                    End If
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Next 8 bytes as cardid
                    'Dim CardID As String = ReverseBytes(Str(1).Substring(24, 16))
                    'Convert last 4 bytes to integer to get printed card ID
                    'ds.CardID = Convert.ToInt32(CardID.Substring(8, 8), 16).ToString
                    'log.Debug(IMEI & ":CARD:" & ds.CardID)
                    '.iButton = ds.CardID
                    If data.Ibutton IsNot Nothing Then
                        Dim iButtonId As String = data.Ibutton.IbuttonId
                        ds.CardID = "01" & ReverseBytes(iButtonId.Substring(iButtonId.Length - 8))
                        log.Debug(IMEI & ":CARD:" & ds.CardID)
                        .iButton = ds.CardID
                        'Check if any driver is assigned
                        Dim dr As DB.DriverDetailsRow = Drivers.Where(Function(a) a.CardID Like ds.CardID.ToUpper & "*").FirstOrDefault
                        If dr IsNot Nothing Then
                            log.Debug(ds.IMEI & ":Driver found:" & ds.CardID & ":" & dr.DriverName)
                            ds.DriverID = dr.DriverID
                        Else
                            log.Debug(ds.IMEI & ":Driver not found for card:" & ds.CardID)
                        End If
                    End If

                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Next 2 bytes as DI convert hex to binary string with 0 and 1
                    'Dim DI As String = StrReverse(ConvertHex2Bin(Str(1).Substring(24, 4)))
                    '    Dim DI1 = IIf(DI.Substring(0, 1) = 1, True, False)
                    '    Dim DI2 = IIf(DI.Substring(1, 1) = 1, True, False)
                    '    Dim DI3 = IIf(DI.Substring(2, 1) = 1, True, False)
                    Dim DI1 = data.Ignition
                    Dim DI2 = 0
                    Dim DI3 = 0
                    log.Debug(IMEI & ":BIT:" & DI1 & ":" & DI2 & ":" & DI3)
                    'Process logic
                    If ds.DI1 = DI1 Then
                        'No change in DI1
                    Else
                        'Ignition change
                        If ds.DI1 Then
                            'Was On now Off
                            'Trip complete
                            dtAssetTrips.Rows.Add(ds.AssetID, ds.EngineStartTime, ds.DI1ChangeTime, 0, 0, 0, 0, 0, ds.DriverID, 0, 0, 0, 0, 0, 0, 0, 0, False, ds.tenantId)
                            .EngineOn = True
                        Else
                            'Was Off now On
                            ds.EngineStartTime = ignitionTimeStamp
                            .EngineOn = False
                        End If
                        ds.DI1ChangeTime = ignitionTimeStamp
                        ds.DI1 = DI1
                        v.isEngine = ds.DI1
                    End If
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Dim IntPercent As Integer = Convert.ToInt32(ReverseBytes(Str(1).Substring(24, 2)), 16)
                    'Internal battery %
                    'ds.BV = IntPercent
                    '.BV = ds.BV
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Next 4 bytes, reverse it and convert from hex to int and divide by 1000
                    'Dim voltage As Double = Convert.ToInt32(ReverseBytes(Str(1).Substring(24, 8)), 16) / 1000
                    If data.Ana0 IsNot Nothing Then
                        Dim voltage As Double = data.Ana0.Value * 3.13
                        ds.Voltage = voltage
                        log.Debug(IMEI & ":AIN1:" & voltage)
                        .MV = ds.Voltage
                        'v.LastFuelLevel = ds.Voltage
                        'Try
                        '    Dim FuelLevel As Integer = v.LastFuelLevel
                        '    If .EngineOn Then
                        '        .FVolt = ds.Voltage 'GetFuelPortState(.lstIO, v.FuelPort)
                        '        FuelLevel = Math.Round((.FVolt * v.FuelSlope + v.FuelOffset) / 10) * 10
                        '        If My.Settings.LogFuel Then
                        '            log.Debug(IMEI & ":FUELCalc:Volt>" & v.DeviceName & ">" & .FVolt & " > Level:" & FuelLevel & " > Slope:" & v.FuelSlope & " > Offset:" & v.FuelOffset)
                        '        End If
                        '        If FuelLevel > 100 Then
                        '            FuelLevel = 100
                        '        End If
                        '        If FuelLevel < 0 Then
                        '            FuelLevel = 0
                        '        End If
                        '        'Retain level in case current level just dropped to 0
                        '        If FuelLevel > 0 Then 'And v.LastFuelLevel > 0 Then
                        '            v.LastFuelLevel = FuelLevel
                        '        End If
                        '        'v.LastFuelLevel = FuelLevel
                        '    End If

                        'Catch ex As Exception
                        '    log.Error("ER06:" & ex.Message, ex)
                        'End Try
                    End If
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    If data.Ble IsNot Nothing Then
                        Dim totalTags As Integer = data.Ble.Event.Count
                        Dim maxPower As Integer = -1000
                        Dim TagID As String = ""
                        Dim TagSeen As DateTime
                        For i As Integer = 0 To totalTags - 1
                            'Next 4 bytes as Timestamp
                            'Dim TagTime As String = Str(1).Substring(24 + (i * 22), 8)
                            'TagTime = ReverseBytes(TagTime)
                            'Convert to datetime
                            Dim tdt As DateTime = epoch.AddSeconds(data.Ble.Event(i).Time).AddMinutes(180)
                            'Next 6 bytes as mac address
                            Dim macAdd As String = data.Ble.Event(i).BleAddress.Replace(":", "")
                            'Next 1 byte as RSSI
                            Dim RSSI As Integer = 256 + data.Ble.Event(i).Rssi
                            log.Debug(IMEI & ":BTAG:" & TagID & ":" & RSSI & ">" & tdt.ToString("dd MMM HH:mm:ss"))
                            If i = 0 Then
                                TagID = macAdd
                                maxPower = RSSI
                                TagSeen = tdt
                            Else
                                If RSSI > maxPower Then
                                    maxPower = RSSI
                                    TagID = macAdd
                                    TagSeen = tdt
                                End If
                            End If
                        Next

                        'Check if its valid location
                        Dim loc As DB.POIMasterRow = Locations.Where(Function(a) a.TagID = TagID).FirstOrDefault
                        If loc IsNot Nothing Then
                            Dim b As New BLETag
                            b.TagID = TagID
                            b.RSSI = maxPower
                            b.LocID = loc.POIId
                            b.SeenTime = TagSeen
                            ds.BLETags.Add(b)
                            'If ds.Satellite = 255 Then
                            ds.Latitude = loc.Latitude
                            ds.Longitude = loc.Longitude
                            ds.Speed = 0
                            ds.LocID = loc.POIId
                            'End If
                            If Not (ds.Latitude = loc.Latitude And ds.Longitude = loc.Longitude) Then
                                'Update Location
                                dtDeviceEvents.Rows.Add(ds.DeviceID, EventType.Location, ds.LastSeen, ds.Latitude, ds.Longitude, ds.LocID, "", False, False, Nothing, Nothing, ds.AssetID, 0, ds.EnStat.ToString, ds.OStatus.ToString, "", 0, 0, ds.Voltage, ds.BV, ds.Speed, 0, 0, False, False, lastSeenTimeStamp.Subtract(epoch).TotalSeconds, ds.LastSeen.Subtract(epoch).TotalSeconds, 0)
                                'UpdateLiveLocation(ds.AssetID, ds.DeviceID, ds.VehicleID, ds.Latitude, ds.Longitude, ds.LastSeen, ds.LocName, ds.EnStat.ToString, ds.OStatus.ToString, ds.DriverID, ds.Voltage, ds.Speed, ds.Voltage, ds.BV)
                            End If
                        End If
                    End If
                    .Longitude = ds.Longitude
                    .Latitude = ds.Latitude
                    .Speed = ds.Speed
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Next 2 bytes as SOC
                    'Dim SOC As Integer = Convert.ToInt32(ReverseBytes(str(1).Substring(24, 4)), 16)
                    'log.Debug(IMEI & ":SOC:" & SOC)
                    'ds.BV = SOC

                    If Not vlist(IMEI).isInit Then
                        vlist(IMEI).isInit = True
                        vlist(IMEI).LastSeen = .TStamp
                        vlist(IMEI).isInit = True
                        vlist(IMEI).Latitude = ds.Latitude
                        vlist(IMEI).Longitude = ds.Longitude
                        vlist(IMEI).EngineStartTime = ds.EngineStartTime
                        If vlist(IMEI).LastOp.StartTime = Nothing Then
                            vlist(IMEI).LastOp.StartTime = New Date(rec.TStamp.Year, rec.TStamp.Month, rec.TStamp.Day)
                        End If
                    End If

                    If vlist(IMEI).LastSeen > .TStamp Then
                        .TStamp = vlist(IMEI).LastSeen
                    End If

                    'log.Info(IMEI & "::TS:" & .TStamp.ToString("dd MMM HH:mm:ss") & "Lt:" & .Latitude & "/Lg:" & .Longitude & "/Sp:" & .Speed & "/An:" & .Heading & "/St:" & .Satellite)

                End With
                'Clear location if not seen in last x seconds
                If rec.TStamp.Subtract(v.LastSeen).TotalSeconds > My.Settings.MaxBLETimeOutInSecs Then
                    v.Location = ""
                End If
                'TODO: log vehicle data
                'vlta.Insert(vrw.VehicleID, rec.TStamp, rec.Latitude, rec.Longitude, v.Location, v.EngineStatus, rec.Speed, rec.Angle, v.MainBattery, v.BackBattery, 0)

                Records.Add(rec)
                'Verify if Data 2 is same as numRec
                'Verify CRC
                'Else
                '    PackType = PacketType.IMEPack
                '    'Dim IMEILen As Integer = ConvertHex2Int(data.Substring(0, 4))
                '    IMEI = System.Text.ASCIIEncoding.ASCII.GetString(ConvertHex2Byte(data.Substring(4)))
            Else
                log.Error("IMEI not found:" & IMEI)
            End If
        Catch ex As Exception
            log.Error(ex.Message, ex)
        End Try


    End Sub

    Public Sub New(data As String, _IMEI As String)
        If data.StartsWith("00000000") Then
            IMEI = _IMEI
            PackType = PacketType.AVLPack
            Dim v As New VehicleStatus
            Try
                If vlist.ContainsKey(IMEI) Then
                    v = vlist(IMEI)
                Else
                    log.Error("IMEI:" & IMEI & " not registered.")
                    Return
                End If
            Catch ex As Exception
                'log.Error("ER03:" & ex.Message, ex)
            End Try

            Try
                'Decode packet
                Dim iPtr As Integer = 4 * 2 'Leave for blank
                Dim dLen As Integer = ConvertHex2Int(data.Substring(iPtr, 8))
                iPtr += 4 * 2 'Len of data
                codec = ConvertHex2Int(data.Substring(iPtr, 2))
                iPtr += 1 * 2
                numRec = ConvertHex2Int(data.Substring(iPtr, 2))
                'log.Info("Records>>" & numRec)
                iPtr += 1 * 2
                For j As Integer = 0 To numRec - 1
                    'log.Info("Records::" & j + 1)
                    Dim rec As New AVLRecord
                    With rec
                        Dim jPtr As Integer = 0
                        Dim AVLData = data.Substring(iPtr)
                        Try
                            .TStamp = epoch.AddMilliseconds(ConvertHex2Int64(AVLData.Substring(0, 16)))
                        Catch ex As Exception
                            log.Error("ER47:" & ex.Message, ex)
                            log.Debug("DATA:" & data)
                        End Try
                        If .TStamp > Now Then
                            .TStamp = Now
                            log.Error(IMEI & ":Invalid time reported:" & .TStamp.ToString("dd MMM yyyy HH:mm:ss"))
                        End If
                        Select Case v.TimeZoneId
                            Case 45, 198, 451, 967, 1458, 1778, 2043, 2308, 2573, 2777, 3036
                                .TStamp = .TStamp.AddMinutes(180)
                            Case 50, 203, 456, 972, 1463, 1783, 2048, 2313, 2578, 2782, 3041
                                .TStamp = .TStamp.AddMinutes(240)
                            Case 56, 209, 462, 978, 1469, 1789, 2054, 2319, 2584, 2788, 3047
                                .TStamp = .TStamp.AddMinutes(330)
                            Case 30, 183
                                'Do nothing
                            Case Else
                                'Add more in future if any
                        End Select

                        If Not vlist(IMEI).isInit Then
                            vlist(IMEI).isInit = True
                            vlist(IMEI).LastSeen = .TStamp
                            vlist(IMEI).isInit = True
                            vlist(IMEI).Latitude = rec.Latitude
                            vlist(IMEI).Longitude = rec.Longitude
                            vlist(IMEI).EngineStartTime = rec.TStamp
                            If vlist(IMEI).LastOp.StartTime = Nothing Then
                                vlist(IMEI).LastOp.StartTime = New Date(rec.TStamp.Year, rec.TStamp.Month, rec.TStamp.Day)
                            End If
                        End If

                        If vlist(IMEI).LastSeen > .TStamp Then
                            .TStamp = vlist(IMEI).LastSeen
                        End If

                        jPtr = 8 * 2
                        .Prty = ConvertHex2Int(AVLData.Substring(jPtr, 2))
                        jPtr += 1 * 2
                        Dim GPSEl As String = AVLData.Substring(jPtr, 30)
                        .Longitude = ConvertHex2Int(GPSEl.Substring(0, 8)) / 10000000
                        .Latitude = ConvertHex2Int(GPSEl.Substring(8, 8)) / 10000000
                        .Altitude = ConvertHex2Int(GPSEl.Substring(16, 4))
                        .Heading = ConvertHex2Int(GPSEl.Substring(20, 4))
                        .Satellite = ConvertHex2Int(GPSEl.Substring(24, 2))
                        .Speed = ConvertHex2Int(GPSEl.Substring(26, 4))
                        jPtr += 30

                        'log.Info(IMEI & "::TS:" & .TStamp.ToString("dd MMM HH:mm:ss") & "Lt:" & .Latitude & "/Lg:" & .Longitude & "/Sp:" & .Speed & "/An:" & .Heading & "/St:" & .Satellite)

                        'Determine Geofence if any
                        Try
                            Dim sql As String = "select LocationID,LocationName from dbo.Location where geom.MakeValid().STContains(geometry::STGeomFromText('POINT(" & .Longitude & " " & .Latitude & ")', 4326))=1 order by LocationID desc"
                            Dim dt As DataTable = ExecSQL(sql)
                            If dt.Rows.Count > 0 Then
                                For Each rw As DataRow In dt.Rows
                                    Dim el As New ELocation()
                                    .Location.LocID = rw.Item(0)
                                    .Location.LocName = rw.Item(1)
                                    el.LocID = rw(0)
                                    el.LocName = rw.Item(1)
                                    'AllLocations.Add(el)
                                Next
                            Else
                                .Location.LocID = 0
                                .Location.LocName = ""
                            End If
                        Catch ex As Exception

                        End Try

                        If .Longitude = 0 And .Latitude = 0 Then
                            isValid = False
                            Return
                        End If

                        Dim IOEl As String = AVLData.Substring(jPtr)
                        iPtr += jPtr

                        Try

                            'Decode Elements
                            If codec = CodecType.Codec8 Then
                                .EventIO = ConvertHex2Int(IOEl.Substring(0, 2))
                                Dim TotalIOCnt As Integer = ConvertHex2Int(IOEl.Substring(2, 2))
                                Dim kPtr As Integer = 4
                                If TotalIOCnt > 0 Then
                                    For k As Integer = 0 To 3
                                        Dim b As Integer = Math.Pow(2, k)
                                        Dim N1 As Integer = ConvertHex2Int(IOEl.Substring(kPtr, 2))
                                        kPtr += 2
                                        If N1 > 0 Then
                                            FillIO(IOEl.Substring(kPtr, N1 * ((b * 2) + 2)), b * 2, rec, 2, v.DeviceType)
                                        End If
                                        kPtr += (((b * 2) + 2) * N1)
                                    Next
                                    iPtr += kPtr
                                End If
                            End If
                            If codec = CodecType.Codec8E Then
                                .EventIO = ConvertHex2Int(IOEl.Substring(0, 4))
                                Dim TotalIOCnt As Integer = ConvertHex2Int(IOEl.Substring(4, 4))
                                Dim kPtr As Integer = 8
                                If TotalIOCnt > 0 Then
                                    For k As Integer = 0 To 3
                                        Dim b As Integer = Math.Pow(2, k)
                                        Dim N1 As Integer = ConvertHex2Int(IOEl.Substring(kPtr, 4))
                                        kPtr += 4
                                        If N1 > 0 Then
                                            FillIO(IOEl.Substring(kPtr, N1 * ((b * 2) + 4)), b * 2, rec, 4, v.DeviceType)
                                        End If
                                        kPtr += (((b * 2) + 4) * N1)
                                    Next
                                    'Custom Attribs
                                    Dim NX As Integer = ConvertHex2Int(IOEl.Substring(kPtr, 4))
                                    kPtr += 4
                                    For l As Integer = 0 To NX - 1
                                        Dim cls As New clsIOValue
                                        cls.IOID = ConvertHex2Int(IOEl.Substring(kPtr, 4))

                                        'Dim avlt As AVL130 = cls.IOID

                                        kPtr += 4
                                        Dim nxl As Integer = ConvertHex2Int(IOEl.Substring(kPtr, 4))
                                        kPtr += 4
                                        cls.IOValue = IOEl.Substring(kPtr, 2 * nxl)
                                        IOAdd(v.DeviceType, rec, cls.IOID, cls)

                                        'log.Info("IO>>" & cls.IOID & ":" & cls.IOValue)
                                        If cls.IOID = 548 And v.DeviceType = DevType.FMC130 Then 'TFT100, FMC130
                                            'Process Advanced BLE
                                            Dim lptr As Integer = 2
                                            log.Debug(IMEI & ":AdvBLE:" & cls.IOValue)
                                        End If
                                        If cls.IOID = 548 And v.DeviceType = DevType.FMC640 Then 'TFT100, FMC130
                                            'log.Debug(IMEI & ":Beacon:" & cls.IOValue)
                                            Dim lptr As Integer = 0
                                            'Dim zdt As DB.vwZoneReadersDataTable = zta.GetData
                                            Dim bcnCount As Integer = cls.IOValue.Substring(lptr, 2)
                                            lptr += 2
                                            For i As Integer = 0 To bcnCount - 1
                                                Dim rssi = 255 - ConvertHex2Int(cls.IOValue.Substring(lptr, 2))
                                                lptr += 2
                                                Dim bcnLength As Integer = cls.IOValue.Substring(lptr, 2)
                                                lptr += 2
                                                Dim bcnID As String = cls.IOValue.Substring(lptr, bcnLength * 2)
                                                lptr += bcnLength * 2
                                                Dim distance As Integer = ConvertHex2Int(ReverseBytes(bcnID.Substring(0, 4)))
                                                Dim bcnMacEnd As String = ReverseBytes(bcnID.Substring(10, 4))
                                                log.Debug(IMEI & " :RSSI: " & rssi & " :bcnID: " & bcnID & " :mac ending: " & bcnMacEnd & " :distance: " & distance)
                                                'skip add id
                                                lptr += 18
                                                UpdateBLEData(False, v, "", bcnMacEnd, distance, rec.Latitude, rec.Longitude, rec.Location.LocID, rec.Location.LocName, rec.TStamp)
                                            Next
                                        End If
                                        If cls.IOID = 385 And v.DeviceType = DevType.FMC130 Then 'TFT100, FMC130
                                            Dim lptr As Integer = 2
                                            'Dim zdt As DB.vwZoneReadersDataTable = zta.GetData
                                            Dim zLoc As String = ""
                                            Dim zSSI As Integer = -200
                                            While lptr < cls.IOValue.Length - 1
                                                Dim bcn As New BLETags
                                                Dim typ As String = cls.IOValue.Substring(lptr, 2)
                                                lptr += 2
                                                If typ.StartsWith("2") Then
                                                    bcn.BType = BLEType.iBeacon
                                                    bcn.UUID = cls.IOValue.Substring(lptr, 32)
                                                    bcn.Major = cls.IOValue.Substring(lptr + 32, 4)
                                                    bcn.Minor = cls.IOValue.Substring(lptr + 36, 4)
                                                    lptr += 40
                                                ElseIf typ.StartsWith("0") Then
                                                    bcn.BType = BLEType.EddyStone
                                                    bcn.UUID = cls.IOValue.Substring(lptr, 32)
                                                    lptr += 32
                                                Else
                                                    bcn.BType = BLEType.Unknown
                                                    bcn.UUID = cls.IOValue.Substring(lptr, 32)
                                                    lptr += 32
                                                End If
                                                bcn.SSI = 255 - ConvertHex2Int(cls.IOValue.Substring(lptr, 2))
                                                lptr += 2
                                                Select Case typ
                                                    Case "21", "01"
                                                    Case "23", "03"
                                                        bcn.Battery = ConvertHex2Int(cls.IOValue.Substring(lptr, 4))
                                                        lptr += 4
                                                    Case "27", "07"
                                                        bcn.Battery = ConvertHex2Int(cls.IOValue.Substring(lptr, 4))
                                                        lptr += 4
                                                        bcn.Temperature = ConvertHex2Int(cls.IOValue.Substring(lptr, 4))
                                                        lptr += 4
                                                    Case Else
                                                        bcn.BType = BLEType.Unknown
                                                End Select
                                                'Check if iBeacon
                                                rec.BLEs.Add(bcn)

                                                If nmelist.ContainsKey(bcn.UUID) Then
                                                    'Update settings
                                                    nmelist(bcn.UUID) = bcn

                                                    Dim rnd As New Random(Now.Millisecond)
                                                    Dim rlat As Double = rec.Latitude + (rnd.Next(20, 60) * 0.000001)
                                                    Dim rlon As Double = rec.Longitude + (rnd.Next(20, 60) * 0.000001)
                                                    'Dim ret As Int32 = vta.UpdateQueryNME(rlat, rlon, rec.TStamp, bcn.UUID)
                                                    'ret = _vta.UpdateQueryBLE(rlat, rlon, rec.TStamp, rec.Location.LocName, bcn.UUID)

                                                    Dim DeviceId As Int32 = nmeIMEIlist(bcn.UUID)
                                                    Dim ret As Int32 = vta.UpdateQueryNME(rlat, rlon, rec.TStamp, DeviceId)
                                                    ret = _vta.UpdateQueryBLE(rlat, rlon, rec.TStamp, rec.Location.LocName, bcn.UUID)
                                                    log.Debug("NME Update >> GSE:" & v.DeviceName & "/UUID:" & bcn.UUID & "/Lat:" & rlat.ToString() & "/Long:" & rlon.ToString() & "/Time:" & rec.TStamp.ToString())

                                                End If

                                                'log.Info("BLE:" & bcn.BType.ToString & "/" & bcn.UUID & "/RSSI:" & bcn.SSI & "/Temp:" & bcn.Temperature & "/Bat:" & bcn.Battery & "/Maj:" & bcn.Major & "/Mnr:" & bcn.Minor)
                                                'Check if its a location
                                                'For Each zrw As DB.vwZoneReadersRow In zdt.Rows
                                                '    If zrw.BLEMac = bcn.UUID Then
                                                '        If bcn.SSI > zSSI Then
                                                '            zSSI = bcn.SSI
                                                '            zLoc = zrw.ZoneName
                                                '        End If
                                                '    End If
                                                'Next
                                            End While
                                            If zLoc <> "" Then
                                                vlist(IMEI).Location = zLoc
                                            End If
                                            If vlist(IMEI).LastSeen > rec.TStamp Then
                                                rec.TStamp = vlist(IMEI).LastSeen
                                            End If
                                            vlist(IMEI).LastSeen = rec.TStamp
                                        End If
                                        kPtr += 2 * nxl
                                    Next
                                    iPtr += kPtr
                                End If
                            End If
                            If codec = CodecType.Codec16 Then
                                .EventIO = ConvertHex2Int(IOEl.Substring(0, 4))
                                .GenerationType = ConvertHex2Int(IOEl.Substring(4, 2))
                                Dim TotalIOCnt As Integer = ConvertHex2Int(IOEl.Substring(6, 2))
                                Dim kPtr As Integer = 8
                                If TotalIOCnt > 0 Then
                                    For k As Integer = 0 To 3
                                        Dim b As Integer = Math.Pow(2, k)
                                        Dim N1 As Integer = ConvertHex2Int(IOEl.Substring(kPtr, 2))
                                        kPtr += 2
                                        If N1 > 0 Then
                                            FillIO(IOEl.Substring(kPtr, N1 * ((b * 2) + 4)), b * 2, rec, 4, v.DeviceType)
                                        End If
                                        kPtr += (((b * 2) + 4) * N1)
                                    Next
                                    iPtr += kPtr
                                End If
                            End If
                        Catch ex As Exception
                            log.Error("ER04:" & ex.Message, ex)
                        End Try

                        '.FuelType = v.FuelType

                        Try
                            'Adjust scales for the values
                            Dim rpm As Integer = 0
                            For Each y In rec.lstIO.Keys
                                Dim x As clsIOValue = rec.lstIO(y)
                                'If Not IOType2.Contains(y) Then
                                '    IOType2.Add(y)
                                '    isAdded = True
                                '    Continue For
                                'End If
                                Dim avlt As AVLGEN = y
                                Select Case avlt
                                    Case AVLGEN.ExternalVoltage
                                        x.IOValue /= 1000
                                        v.MainBattery = Double.Parse(x.IOValue).ToString("0.0")
                                        'If v.IMEI = "866728060513339" Or v.IMEI = "866728060445987" Then
                                        '    log.Debug(v.IMEI & ": MV - " & v.MainBattery)
                                        'End If
                                        'log.Debug("IMEI:" & v.IMEI & " > MV:" & v.MainBattery)
                                    Case AVLGEN.BatteryVoltage
                                        x.IOValue /= 1000
                                        v.BackBattery = Double.Parse(x.IOValue).ToString("0.0")
                                    Case AVLGEN.EngineTemp
                                        x.IOValue /= 10
                                    Case AVLGEN.BatteryCurrent
                                        x.IOValue /= 1000
                                    Case AVLGEN.AI1
                                        'x.IOValue /= 1000
                                    Case AVLGEN.FuelRateGPS
                                        x.IOValue /= 100
                                    Case AVLGEN.FuelLevel
                                        x.IOValue /= 10
                                    Case AVLGEN.TripOdometer
                                        x.IOValue /= 1000
                                    Case AVLGEN.TotalOdometer
                                        x.IOValue /= 1000
                                        rec.Odometer = x.IOValue
                                        'log.Debug(IMEI & ":Odometer:" & x.IOValue & ">VID:" & v.VehicleID)
                                        vta.UpdateOdo2(x.IOValue, v.VehicleID)
                                    Case AVLGEN.FuelUsed
                                        x.IOValue /= 1000
                                    Case AVLGEN.FuelConsumed
                                        x.IOValue /= 10
                                    Case AVLGEN.TotalMileage
                                        x.IOValue /= 1000
                                    Case AVLGEN.iButton
                                        'Check which driver
                                        If x.IOValue <> "0000000000000000" Then
                                            'Extract CardID
                                            Dim CardID As String = x.IOValue 'rec.iButton.Substring(2, 6)
                                            'CardID = CardID.Substring(8, 2) & CardID.Substring(6, 2) & CardID.Substring(4, 2) & CardID.Substring(2, 2)
                                            'Convert to integer
                                            'CardID = Convert.ToInt64(CardID, 16).ToString.PadLeft(10, "0")
                                            log.Debug(IMEI & ":iButton:" & x.IOValue & " > " & CardID)

                                            'Partial value compare
                                            rec.iButton = CardID
                                        End If
                                    Case AVLGEN.GreenDrivingType
                                        If x.IOValue = 1 Then
                                            x.IOValue = "HA"
                                        ElseIf x.IOValue = 2 Then
                                            x.IOValue = "HB"
                                        Else
                                            x.IOValue = "HC"
                                        End If
                                    Case AVLGEN.GreenDrivingValue
                                        x.IOValue /= 100
                                    Case AVLGEN.Beacon
                                        Dim bx As String = ""
                                        For Each b In rec.BLEs
                                            bx = bx & b.UUID & ","
                                        Next
                                        If bx.Length > 0 Then
                                            bx = bx.Substring(0, bx.Length - 1)
                                        End If
                                        x.IOValue = bx
                                End Select
                            Next
                        Catch ex As Exception
                            log.Error("ER05:" & ex.Message, ex)
                        End Try

                        Try
                            'Determine Engine on / off
                            If v.isEngine Then
                                'Dim y? As Boolean = GetPortState(rec.lstIO, 1)
                                'If y.HasValue Then
                                '    '.EngineOn = x.Value
                                '    log.Debug("DIN:" & y.Value)
                                '    If y.Value Then
                                '        MsgBox("Got it DIN")
                                '    End If
                                'End If

                                'Engine signal is either through IN1 or determined by device
                                If v.EnginePort = 0 Then
                                    'Determined by device
                                    'Check for Ignition
                                    If rec.lstIO.ContainsKey(AVLGEN.Ignition) Then
                                        If rec.lstIO(AVLGEN.Ignition).IOValue = 1 Then
                                            .EngineOn = True
                                        Else
                                            .EngineOn = False
                                        End If
                                    Else
                                        .EngineOn = v.EngineStatus
                                    End If
                                    v.EngineStatus = .EngineOn
                                Else
                                    'Determined by port v.EnginePort
                                    Dim x? As Boolean = GetPortState(rec.lstIO, v.EnginePort)
                                    If x.HasValue Then
                                        .EngineOn = x.Value
                                    Else
                                        .EngineOn = v.EngineStatus
                                    End If
                                    v.EngineStatus = .EngineOn
                                End If
                            Else
                                'Determine via RPM or voltage
                                If v.EnginePort = 99 Then
                                    'Determine via voltage

                                Else
                                    If rec.lstIO.ContainsKey(AVLGEN.EngineRPM) Then
                                        If CInt(rec.lstIO(AVLGEN.EngineRPM).IOValue) > 0 Then
                                            .EngineOn = True
                                        Else
                                            .EngineOn = False
                                        End If
                                    Else
                                        .EngineOn = v.EngineStatus
                                    End If
                                    v.EngineStatus = .EngineOn
                                End If
                            End If
                            Try
                                If v.isHCT Then
                                    Dim x? = GetPortState(rec.lstIO, v.HCTPort)
                                    If x.HasValue Then
                                        .HCTOn = x.Value
                                    End If
                                End If
                                If v.isLOP Then
                                    Dim x? = GetPortState(rec.lstIO, v.LOPPort)
                                    If x.HasValue Then
                                        .LOPOn = x.Value
                                    End If
                                End If
                            Catch ex As Exception
                                log.Error("ER12:" & ex.Message, ex)
                            End Try
                            Dim FuelLevel As Integer = v.LastFuelLevel
                            If .EngineOn Then
                                If v.FuelType = "SensorInput" Then
                                    .FVolt = GetFuelPortState(.lstIO, v.FuelPort)
                                    FuelLevel = Math.Round((.FVolt * v.FuelSlope + v.FuelOffset) / 10) * 10
                                    If My.Settings.LogFuel Then
                                        log.Debug(IMEI & ":FUELCalc:Volt>" & v.DeviceName & ">" & .FVolt & " > Level:" & FuelLevel & " > Slope:" & v.FuelSlope & " > Offset:" & v.FuelOffset)
                                    End If
                                    If FuelLevel > 100 Then
                                        FuelLevel = 100
                                    End If
                                    If FuelLevel < 0 Then
                                        FuelLevel = 0
                                    End If
                                    'Retain level in case current level just dropped to 0
                                    If FuelLevel > 0 Then 'And v.LastFuelLevel > 0 Then
                                        v.LastFuelLevel = FuelLevel
                                    End If
                                    'v.LastFuelLevel = FuelLevel
                                End If
                            End If

                            If v.isWork1 Then
                                Dim x? = GetPortState(rec.lstIO, v.Work1Port)
                                If x.HasValue Then
                                    .Work1On = IIf(x.Value, 1, 0)
                                End If
                            End If
                            If v.isWork2 Then
                                Dim x? = GetPortState(rec.lstIO, v.Work2Port)
                                If x.HasValue Then
                                    .Work2On = IIf(x.Value, 1, 0)
                                End If
                            End If
                            If v.isWork3 Then
                                Dim x? = GetPortState(rec.lstIO, v.Work3Port)
                                If x.HasValue Then
                                    .Work3On = IIf(x.Value, 1, 0)
                                End If
                            End If
                            If v.isWork4 Then
                                Dim x? = GetPortState(rec.lstIO, v.Work4Port)
                                If x.HasValue Then
                                    .Work4On = IIf(x.Value, 1, 0)
                                End If
                            End If

                            If .lstIO.ContainsKey(AVLGEN.GreenDrivingType) Then
                                If .lstIO(AVLGEN.GreenDrivingType).IOValue = "HA" Then
                                    .isHA = True
                                Else
                                    .isHA = False
                                End If
                                If .lstIO(AVLGEN.GreenDrivingType).IOValue = "HB" Then
                                    .isHB = True
                                Else
                                    .isHB = False
                                End If
                            End If

                        Catch ex As Exception
                            log.Error("ER06:" & ex.Message, ex)
                        End Try
                        '.MainVolt = v.MainBattery
                        .MV = v.MainBattery
                        .BV = v.BackBattery
                        'Dim cnts As String = ""
                        'cnts += IMEI + "," & .TStamp.ToString("dd MMM HH:mm:ss") & "," & .Latitude & "," & .Longitude & "," & .Speed & "," & .Heading & "," & .Satellite
                        'If rec.lstIO.Count > 0 Then
                        '    cnts = cnts
                        'End If
                        'For Each x In IOType
                        '    If .lstIO.ContainsKey(x) Then
                        '        cnts &= "," & .lstIO(x).IOValue
                        '    Else
                        '        cnts &= ","
                        '    End If
                        'Next
                        'If Not System.IO.File.Exists("C:\log\" & IMEI & ".txt") Then
                        '    isAdded = True
                        'End If
                        'System.IO.File.AppendAllText("C:\log\" & IMEI & ".txt", cnts & vbCrLf)

                        'If isAdded Then
                        '    Dim head As String = "IMEI,TStamp,Latitude,Longitude,Speed,Heading,Satellite"
                        '    For Each x In IOType
                        '        head &= "," & x.ToString
                        '    Next
                        '    System.IO.File.AppendAllText("C:\log\" & IMEI & ".txt", head & vbCrLf)
                        '    isAdded = False
                        'End If

                    End With
                    'Clear location if not seen in last x seconds
                    If rec.TStamp.Subtract(v.LastSeen).TotalSeconds > My.Settings.MaxBLETimeOutInSecs Then
                        v.Location = ""
                    End If
                    'TODO: log vehicle data
                    'vlta.Insert(vrw.VehicleID, rec.TStamp, rec.Latitude, rec.Longitude, v.Location, v.EngineStatus, rec.Speed, rec.Angle, v.MainBattery, v.BackBattery, 0)

                    Records.Add(rec)
                Next
                'Verify if Data 2 is same as numRec
                'Verify CRC
                'Else
                '    PackType = PacketType.IMEPack
                '    'Dim IMEILen As Integer = ConvertHex2Int(data.Substring(0, 4))
                '    IMEI = System.Text.ASCIIEncoding.ASCII.GetString(ConvertHex2Byte(data.Substring(4)))
            Catch ex As Exception
                log.Error("ER07:" & ex.Message, ex)
            End Try
        End If
    End Sub

    Private Sub IOAdd(deviceType As DevType, ByRef rec As AVLRecord, iOID As Integer, cls As clsIOValue)
        If deviceType = DevType.FMC130 Then
            Dim iox As AVL130 = iOID
            If iOID.ToString = iox.ToString Then
                log.Debug(IMEI & ":Missing AVL 130:" & iOID)
            End If
            'If rec.lstIO.ContainsKey(iox) Then
            '    'log.Warn("SAME IOID:" & iox.ToString)
            '    rec.lstIO(iox) = cls
            'Else
            '    rec.lstIO.Add(iox, cls)
            'End If
            Select Case iox
                Case AVL130.AccelaratorPedalPosition
                    recAdd(rec, AVLGEN.AccelaratorPedalPosition, cls)
                Case AVL130.AI1
                    recAdd(rec, AVLGEN.AI1, cls)
                Case AVL130.AI2
                    recAdd(rec, AVLGEN.AI2, cls)
                Case AVL130.AutoGeofence
                    recAdd(rec, AVLGEN.AutoGeofence, cls)
                Case AVL130.BatteryCurrent
                    recAdd(rec, AVLGEN.BatteryCurrent, cls)
                Case AVL130.BatteryVoltage
                    recAdd(rec, AVLGEN.BatteryVoltage, cls)
                Case AVL130.Beacon
                    recAdd(rec, AVLGEN.Beacon, cls)
                Case AVL130.CrashDetection
                    recAdd(rec, AVLGEN.CrashDetection, cls)
                Case AVL130.DI1
                    recAdd(rec, AVLGEN.DI1, cls)
                Case AVL130.DI2
                    recAdd(rec, AVLGEN.DI2, cls)
                Case AVL130.DI3
                    recAdd(rec, AVLGEN.DI3, cls)
                Case AVL130.DO1
                    recAdd(rec, AVLGEN.DO1, cls)
                Case AVL130.DO3
                    recAdd(rec, AVLGEN.DO3, cls)
                Case AVL130.DoorStatus
                    recAdd(rec, AVLGEN.DoorStatus, cls)
                Case AVL130.DTCFaultsCount
                    recAdd(rec, AVLGEN.DTCFaultsCount, cls)
                Case AVL130.EngineOil
                    recAdd(rec, AVLGEN.EngineOil, cls)
                Case AVL130.EngineRPM
                    recAdd(rec, AVLGEN.EngineRPM, cls)
                Case AVL130.EngineTemp
                    recAdd(rec, AVLGEN.EngineTemp, cls)
                Case AVL130.ExternalVoltage
                    recAdd(rec, AVLGEN.ExternalVoltage, cls)
                Case AVL130.FuelConsumed
                    recAdd(rec, AVLGEN.FuelConsumed, cls)
                Case AVL130.FuelConsumed2
                    recAdd(rec, AVLGEN.FuelConsumed, cls)
                Case AVL130.FuelLevel
                    recAdd(rec, AVLGEN.FuelLevel, cls)
                Case AVL130.FuelLevel2
                    recAdd(rec, AVLGEN.FuelLevel, cls)
                Case AVL130.FuelRateGPS
                    recAdd(rec, AVLGEN.FuelRateGPS, cls)
                Case AVL130.FuelUsed
                    recAdd(rec, AVLGEN.FuelUsed, cls)
                Case AVL130.GNSSStatus
                    recAdd(rec, AVLGEN.GNSSStatus, cls)
                Case AVL130.GPSSpeed
                    recAdd(rec, AVLGEN.Speed, cls)
                Case AVL130.GreenDrivingType
                    recAdd(rec, AVLGEN.GreenDrivingType, cls)
                Case AVL130.GreenDrivingValue
                    recAdd(rec, AVLGEN.GreenDrivingValue, cls)
                Case AVL130.GSMOperator
                    recAdd(rec, AVLGEN.GSMOperator, cls)
                Case AVL130.GSMSignal
                    recAdd(rec, AVLGEN.GSMSignal, cls)
                Case AVL130.HDOP
                    recAdd(rec, AVLGEN.HDOP, cls)
                Case AVL130.iButton
                    recAdd(rec, AVLGEN.iButton, cls)
                Case AVL130.Ignition
                    recAdd(rec, AVLGEN.Ignition, cls)
                Case AVL130.IgnitionOnCounter
                Case AVL130.Idling
                    recAdd(rec, AVLGEN.Idling, cls)
                Case AVL130.Moving
                    recAdd(rec, AVLGEN.Moving, cls)
                Case AVL130.OverSpeeding
                    recAdd(rec, AVLGEN.OverSpeeding, cls)
                Case AVL130.PDOP
                    recAdd(rec, AVLGEN.PDOP, cls)
                Case AVL130.ProgramNumber
                    recAdd(rec, AVLGEN.ProgramNumber, cls)
                Case AVL130.SleepMode
                    recAdd(rec, AVLGEN.SleepMode, cls)
                Case AVL130.Speed
                    recAdd(rec, AVLGEN.Speed, cls)
                Case AVL130.TotalMileage
                    recAdd(rec, AVLGEN.TotalMileage, cls)
                Case AVL130.TotalMileage2
                    recAdd(rec, AVLGEN.TotalMileage, cls)
                Case AVL130.Trip
                    recAdd(rec, AVLGEN.Trip, cls)
                Case AVL130.TripOdometer
                    recAdd(rec, AVLGEN.TripOdometer, cls)
                Case AVL130.TotalOdometer
                    recAdd(rec, AVLGEN.TotalOdometer, cls)
                Case AVL130.Unplug
                    'rec.lstIO.Add(AVLGEN.Unplug, cls)
                Case AVL130.VehicleSpeed
                    recAdd(rec, AVLGEN.Speed, cls)
                Case Else
                    rec.lstIO.Add(iox, cls)
            End Select
        End If
        If deviceType = DevType.FMC640 Then
            Dim iox As AVL640 = iOID
            If iOID.ToString = iox.ToString Then
                log.Debug(IMEI & ":Missing AVL 640:" & iOID)
            End If
            'If rec.lstIO.ContainsKey(iox) Then
            '    'log.Warn("SAME IOID:" & iox.ToString)
            '    rec.lstIO(iox) = cls
            'Else
            '    'rec.lstIO.Add(iox, cls)
            'End If
            Select Case iox
                Case AVL640.AccelaratorPedalPosition
                    recAdd(rec, AVLGEN.AccelaratorPedalPosition, cls)
                Case AVL640.AI1
                    recAdd(rec, AVLGEN.AI1, cls)
                    'log.Debug(IMEI & ":FUEL Voltage : " & cls.IOValue)
                Case AVL640.AI2
                    recAdd(rec, AVLGEN.AI2, cls)
                Case AVL640.AI3
                    recAdd(rec, AVLGEN.AI3, cls)
                Case AVL640.AI4
                    recAdd(rec, AVLGEN.AI4, cls)
                Case AVL640.AutoGeofence
                    recAdd(rec, AVLGEN.AutoGeofence, cls)
                Case AVL640.BatteryCurrent
                    recAdd(rec, AVLGEN.BatteryCurrent, cls)
                Case AVL640.BatteryVoltage
                    recAdd(rec, AVLGEN.BatteryVoltage, cls)
                Case AVL640.Beacon
                    recAdd(rec, AVLGEN.Beacon, cls)
                Case AVL640.CrashDetection
                    recAdd(rec, AVLGEN.CrashDetection, cls)
                Case AVL640.DI1
                    recAdd(rec, AVLGEN.DI1, cls)
                Case AVL640.DI2
                    recAdd(rec, AVLGEN.DI2, cls)
                Case AVL640.DI3
                    recAdd(rec, AVLGEN.DI3, cls)
                Case AVL640.DI4
                    recAdd(rec, AVLGEN.DI4, cls)
                Case AVL640.DO1
                    recAdd(rec, AVLGEN.DO1, cls)
                Case AVL640.DO3
                    recAdd(rec, AVLGEN.DO3, cls)
                Case AVL640.DoorStatus
                    recAdd(rec, AVLGEN.DoorStatus, cls)
                Case AVL640.DTCFaultsCount
                    recAdd(rec, AVLGEN.DTCFaultsCount, cls)
                Case AVL640.EngineOil
                    recAdd(rec, AVLGEN.EngineOil, cls)
                Case AVL640.EngineRPM
                    recAdd(rec, AVLGEN.EngineRPM, cls)
                Case AVL640.EngineTemp
                    recAdd(rec, AVLGEN.EngineTemp, cls)
                Case AVL640.ExternalVoltage
                    recAdd(rec, AVLGEN.ExternalVoltage, cls)
                Case AVL640.FuelConsumed
                    recAdd(rec, AVLGEN.FuelConsumed, cls)
                Case AVL640.FuelConsumed2
                    recAdd(rec, AVLGEN.FuelConsumed, cls)
                Case AVL640.FuelLevel
                    recAdd(rec, AVLGEN.FuelLevel, cls)
                Case AVL640.FuelLevel2
                    recAdd(rec, AVLGEN.FuelLevel, cls)
                Case AVL640.FuelRateGPS
                    recAdd(rec, AVLGEN.FuelRateGPS, cls)
                Case AVL640.FuelUsed
                    recAdd(rec, AVLGEN.FuelUsed, cls)
                Case AVL640.GNSSStatus
                    recAdd(rec, AVLGEN.GNSSStatus, cls)
                Case AVL640.GPSSpeed
                    recAdd(rec, AVLGEN.Speed, cls)
                Case AVL640.GreenDrivingType
                    recAdd(rec, AVLGEN.GreenDrivingType, cls)
                Case AVL640.GreenDrivingValue
                    recAdd(rec, AVLGEN.GreenDrivingValue, cls)
                Case AVL640.GSMOperator
                    recAdd(rec, AVLGEN.GSMOperator, cls)
                Case AVL640.GSMSignal
                    recAdd(rec, AVLGEN.GSMSignal, cls)
                Case AVL640.HDOP
                    recAdd(rec, AVLGEN.HDOP, cls)
                Case AVL640.iButton
                    recAdd(rec, AVLGEN.iButton, cls)
                Case AVL640.Ignition
                    recAdd(rec, AVLGEN.Ignition, cls)
                Case AVL640.Idling
                    recAdd(rec, AVLGEN.Idling, cls)
                Case AVL640.Moving
                    recAdd(rec, AVLGEN.Moving, cls)
                Case AVL640.OverSpeeding
                    recAdd(rec, AVLGEN.OverSpeeding, cls)
                Case AVL640.PDOP
                    recAdd(rec, AVLGEN.PDOP, cls)
                Case AVL640.ProgramNumber
                    recAdd(rec, AVLGEN.ProgramNumber, cls)
                Case AVL640.SleepMode
                    recAdd(rec, AVLGEN.SleepMode, cls)
                Case AVL640.Speed
                    recAdd(rec, AVLGEN.Speed, cls)
                Case AVL640.TotalMileage
                    recAdd(rec, AVLGEN.TotalOdometer, cls)
                Case AVL640.TotalMileage2
                    recAdd(rec, AVLGEN.TotalOdometer, cls)
                Case AVL640.Trip
                    recAdd(rec, AVLGEN.Trip, cls)
                Case AVL640.TripOdometer
                    recAdd(rec, AVLGEN.TripOdometer, cls)
                Case AVL640.TotalOdometer
                    recAdd(rec, AVLGEN.TotalOdometer, cls)
                Case AVL640.VehicleSpeed
                    recAdd(rec, AVLGEN.Speed, cls)
                Case AVL640.AxisX
                    recAdd(rec, AVLGEN.AxisX, cls)
                Case AVL640.AxisY
                    recAdd(rec, AVLGEN.AxisY, cls)
                Case AVL640.AxisZ
                    recAdd(rec, AVLGEN.AxisZ, cls)
                Case Else
                    If rec.lstIO.ContainsKey(iox) Then
                        rec.lstIO(iox) = cls
                    Else
                        rec.lstIO.Add(iox, cls)
                    End If
                    'rec.lstIO.Add(iox, cls)
            End Select
        End If
    End Sub

    Private Sub recAdd(rec As AVLRecord, ioid As AVLGEN, cls As clsIOValue)
        If rec.lstIO.ContainsKey(ioid) Then
            rec.lstIO(ioid) = cls
        Else
            rec.lstIO.Add(ioid, cls)
        End If
    End Sub

    Private Function GetPortState(lstIO As Dictionary(Of AVLGEN, clsIOValue), enginePort As Integer) As Boolean?
        Dim id As AVLGEN
        Select Case enginePort
            Case 1
                id = AVLGEN.DI1
            Case 2
                id = AVLGEN.DI2
            Case 3
                id = AVLGEN.DI3
            Case 4
                id = AVLGEN.DI4
            Case 5
                id = AVLGEN.DI5
            Case 6
                id = AVLGEN.DI6
            Case 91
                id = AVLGEN.AI1
            Case 92
                id = AVLGEN.AI2
            Case 93
                id = AVLGEN.AI3
            Case 94
                id = AVLGEN.AI4
            Case 95
                id = AVLGEN.AI5
            Case 96
                id = AVLGEN.AI6
        End Select
        If Not lstIO.ContainsKey(id) Then
            Return Nothing
        Else
            If id < 90 Then
                Return lstIO(id).IOValue > 0
            Else
                Return lstIO(id).IOValue > 5000
            End If
        End If
    End Function

    Private Function GetFuelPortState(lstIO As Dictionary(Of AVLGEN, clsIOValue), enginePort As Integer) As Integer
        Dim id As AVLGEN
        Select Case enginePort
            Case 1
                id = AVLGEN.DI1
            Case 2
                id = AVLGEN.DI2
            Case 3
                id = AVLGEN.DI3
            Case 4
                id = AVLGEN.DI4
            Case 5
                id = AVLGEN.DI5
            Case 6
                id = AVLGEN.DI6
            Case 91
                id = AVLGEN.AI1
            Case 92
                id = AVLGEN.AI2
            Case 93
                id = AVLGEN.AI3
            Case 94
                id = AVLGEN.AI4
            Case 95
                id = AVLGEN.AI5
            Case 96
                id = AVLGEN.AI6
        End Select
        If Not lstIO.ContainsKey(id) Then
            Return Nothing
        Else
            Return lstIO(id).IOValue
        End If
    End Function

    Private Sub FillIO(dt As String, byteSize As Integer, ByRef rec As AVLRecord, idSize As Integer, deviceType As DevType)
        For i As Integer = 0 To (dt.Length / (byteSize + idSize)) - 1
            Dim cls As New clsIOValue
            cls.IOID = ConvertHex2Int(dt.Substring(i * (byteSize + idSize), idSize))
            'cls.IOValue = dt.Substring((i * (byteSize + idSize)) + idSize, byteSize) 'ConvertHex2Int(dt.Substring((i * (byteSize + idSize)) + idSize, byteSize))
            If byteSize < 16 Then
                cls.IOValue = ConvertHex2Int(dt.Substring((i * (byteSize + idSize)) + idSize, byteSize))
            Else
                cls.IOValue = dt.Substring((i * (byteSize + idSize)) + idSize, byteSize) 'ConvertHex2Int(dt.Substring((i * (byteSize + idSize)) + idSize, byteSize))
            End If
            'rec.lstIO.Add(cls.IOID, cls)
            If cls.IOID = 67 Then
                cls.IOID = cls.IOID
            End If
            IOAdd(deviceType, rec, cls.IOID, cls)
        Next
    End Sub
End Class
Public Class AVLRecord
    Public TStamp As DateTime
    Public Prty As Priority
    Public Latitude As Double = 0
    Public Longitude As Double = 0
    Public Altitude As Double
    Public Heading As Integer = 0
    Public Satellite As Integer
    Public Speed As Double = 0
    Public EventIO As Integer
    Public lstIO As New Dictionary(Of AVLGEN, clsIOValue)
    Public GenerationType As GenType
    Public BLEs As New List(Of BLETags)
    Public iButton As String = ""

    Public isCorrected As Boolean = False

    Public Odometer As Double
    Public MaxSpeed
    Public DriverID As String
    Public DeviceID As Integer?
    Public AssetID As Integer?
    Public VehicleID As Integer?
    Public DeviceType As DeviceType
    Public isMaintain As Boolean
    Public isAllocated As Boolean = True
    Public Location As New ELocation
    'Public FuelLevel As Integer
    Public MV As Double = 0
    Public BV As Double = 0
    Public FVolt As Double = 0
    Public MainVolt As Double = 0

    Public EngineOn As Boolean
    Public Work1On As Integer
    Public Work2On As Integer
    Public Work3On As Integer
    Public Work4On As Integer
    Public HCTOn As Integer
    Public LOPOn As Integer
    Public isHA As Boolean
    Public isHB As Boolean
    Public FuelType As String
    'Public Inputs(7) As Integer
End Class

Public Class BLETags
    Public BType As BLEType
    Public UUID As String
    Public Major As String
    Public Minor As String
    Public Battery As Integer
    Public SSI As Integer
    Public Temperature As Integer
End Class
Public Enum BLEType
    EddyStone = 0
    iBeacon = 1
    Unknown = 9
End Enum
Public Enum PacketType
    IMEPack
    AVLPack
End Enum
Public Enum CodecType
    Codec8 = 8
    Codec8E = 142
    Codec16 = 16
    Codec12 = 12
    Codec13 = 13
    Codec14 = 14
End Enum
Public Enum Priority
    Low = 0
    High = 1
    Panic = 2
End Enum
Public Enum GenType
    OnExit = 0
    OnEntrance = 1
    OnBoth = 2
    Reserved = 3
    Hysteresis = 4
    OnChange = 5
    Eventual = 6
    Periodical = 7
End Enum
Public Class clsIOValue
    Public IOID As Integer
    Public IOValue As String
End Class
Public Class VehicleStatus

    Public DeviceID As Integer
    Public AssetID As Integer = 0
    Public VehicleID As Integer
    Public DType As DeviceType = TeltonProcX1.DeviceType.UNKNOWN
    Public IMEI As String
    Public DeviceName As String = ""
    Public WMode As WorkMode = WorkMode.UNKNOWN
    Public EngineStartTime? As Date
    Public EngineStartLoc As New EventTiming
    Public LastRec? As Date
    Public Latitude As Double
    Public Longitude As Double
    Public LastLatchInput As Integer = 0
    Public EnStat As EngineStatus = TeltonProcX1.EngineStatus.OFF
    Public OStatus As OpStatus = TeltonProcX1.OpStatus.AVAILABLE
    Public HCT? As Boolean = Nothing
    Public LOP? As Boolean = Nothing
    Public LowVoltage? As Boolean = Nothing
    Public UnderVoltage? As Boolean = Nothing
    Public OverVoltage? As Boolean = Nothing
    Public isSpeeding As Boolean = False
    Public SpeedStart As Date
    Public isSpeedReported As Boolean = False
    Public VisitStart As Date
    Public isVisitRecorded As Boolean = False
    Public isSpeedNotified As Boolean = False
    Public LastSpeedReported As DateTime
    Public isHA As Integer = 0
    Public HATime As New Date(1970, 1, 1)
    Public isHB As Integer = 0
    Public HBTime As New Date(1970, 1, 1)
    Public MaxSpeed As Double
    Public SpeedLimit As Double
    Public LogID As Integer
    Public LastOp As EventTiming
    'Public LastOp(3) As EventTiming
    Public battery As Boolean = False
    Public LastOdom As Double
    Public GeoID As Integer = 0
    Public GeoName As String
    Public LastGeoTime As Date
    Public MinStopSpeed As Integer
    Public Owner As String = ""
    Public EType As Integer
    Public FLow As Double
    Public FHigh As Double
    Public isFuelEnabled As Boolean = False
    Public isEngineSignal As Boolean = True
    Public EngineCutOffLevel As Double
    Public tenantId As Integer
    Friend LastReport As Date
    Public isHCTSupported As Boolean = False
    Public isLOPSupported As Boolean = False
    Public isUnderVoltageSupported As Boolean = False
    Public isOverVoltageSupported As Boolean = False
    Public isLowVoltageSupported As Boolean = False
    Public LastRecordTime As DateTime
    Public RecordCount As Integer = 0
    Public DriverID As Integer
    Public GSEType As String = ""
    Public GSETypeDesc As String = ""
    Public Category As String = ""
    Public Make As String = ""
    Public Model As String = ""
    Public DriverName As String = ""
    Public iButton As String = ""
    Public Occupancy As String = "X"
    Public LastOccupy As DateTime = Now
    Public LastLatitude As Double
    Public LastLongitude As Double
    Public LastLocId As Double
    Public Sub AddToQ(e As EventTiming)
        'log.Info("AddToQ : " & e.OpStatus.ToString)
        If LastOp.OpStatus = e.OpStatus Then
            'No change
        Else
            'Dim x As EventTiming = LastOp(0)
            'Dim y As EventTiming = LastOp(1)
            'LastOp(2) = y
            'LastOp(1) = x
            LastOp = e
        End If
    End Sub
    Public Sub New()
    End Sub


    'Public VehicleID As Integer = 0
    'Public IMEI As String
    Public Location As String
    Public LastSeen As DateTime
    Public EngineStatus As Boolean = False
    Public OpStatus As String
    Public MainBattery As Double = 0
    Public BackBattery As Double = 0
    Public HDOP As Integer = 99
    Public PDOP As Integer = 99
    Public isEngine As Boolean
    Public EnginePort As Integer = 0
    Public isHCT As Boolean = False
    Public HCTPort As Integer
    Public isLOP As Boolean = False
    Public LOPPort As Integer
    Public isWork1 As Boolean = False
    Public isWork2 As Boolean = False
    Public isWork3 As Boolean = False
    Public isWork4 As Boolean = False
    Public Work1Port As Integer
    Public Work2Port As Integer
    Public Work3Port As Integer
    Public Work4Port As Integer
    Public Work1Name As String
    Public Work2Name As String
    Public Work3Name As String
    Public Work4Name As String
    Public EngineRPM As Integer
    Public isInit As Boolean = False
    Public TimeZoneId As Integer
    Public FuelType As String
    Public FuelPort As Integer
    Public FuelOffset As Double
    Public FuelSlope As Double
    Public DeviceType As DevType
    'Public LowVoltage As Double
    'Public UnderVoltage As Double
    'Public OverVoltage As Double
    Public LastFuelLevel As Double = 0
    'Public TenantID As Integer
    Public Odometer As Integer = 0
End Class
Public Enum DevType
    PTG52MCB = 1
    PTG52PBT = 2
    PTG52GPU = 3
    PTG52TTR = 4
    PTG52FMV = 5
    PTG52ACU = 6
    PTG52LDL = 7
    P52 = 8
    FMC130 = 9
    FMC640 = 10
    ISD9 = 11
End Enum