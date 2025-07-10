Imports System.Text
Imports log4net

Public Module mdMoko
    'Public ReadOnly log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)

    Public typeCodeArray As String() = {"ibeacon", "eddystone-uid", "eddystone-url", "eddystone-tlm", "bxp-devifo", "bxp-acc", "bxp-th", "bxp-button", "bxp-tag", "pir", "other", "tof"}
    Public samplingRateArray As String() = {"1hz", "10hz", "25hz", "50hz", "100hz"}
    Public fullScaleArray As String() = {"2g", "4g", "8g", "16g"}
    Public frameTypeArray As String() = {"Single press mode", "Double press mode", "Long press mode", "Abnormal"}
    Public pirDelayResponseStatusArray As String() = {"Low", "Medium", "High"}
    Public fixModeNotifyArray As String() = {"Periodic", "Motion", "Downlink"}
    Public fixResultArray As String() = {"GPS fix success", "LBS fix success", "Interrupted by Downlink", "GPS serial port is used", "GPS aiding timeout", "GPS timeout", "PDOP limit", "LBS failure"}
    Public lowPowerArray As String() = {"10%", "20%", "30%", "40%", "50%"}
    Public scannerReportArray As String() = {"Scanner off", "Always scan", "Always scan periodic report", "Periodic scan immediate report", "Periodic scan periodic report"}
    Public pirDelayRespStatusArray As String() = {"Low delay", "Medium delay", "High delay", "All type"}
    Public pirDoorStatusArray As String() = {"Close", "Open", "All type"}
    Public pirSensorSensitivityArray As String() = {"Low sensitivity", "Medium sensitivity", "High sensitivity", "All type"}
    Public pirSensorDetactionStatusArray As String() = {"No effective motion detected", "Effective motion detected", "All type"}
    Public otherRelationArray As String() = {"A", "A&B", "A|B", "A&B&C", "(A&B)|C", "A|B|C"}
    Public filterDuplicateDataRuleArray As String() = {"None", "MAC", "MAC+Data type", "MAC+RAW Data"}
    Public fixModeArray As String() = {"OFF", "Periodic fix", "Motion fix"}

    Public Function HandlePayload(value As String, msgType As String, index As Integer) As String
        value = value.ToLower()
        Dim hexStrArray As List(Of String) = ToHexStrArray(value)
        Dim len As Integer = hexStrArray.Count
        If len >= 11 Then
            Dim data As New Dictionary(Of String, Object)
            data("flag") = String.Join("", hexStrArray.GetRange(1, 2))
            data("gatewayMac") = String.Join("", hexStrArray.GetRange(3, 6))
            data("length") = ParseHexStrArraytoInt(hexStrArray.GetRange(9, 2))
            Dim deviceDataArray As List(Of String) = hexStrArray.GetRange(11, len - 11)
            Dim deviceDataIndex As Integer = 0
            Dim deviceDataLength As Integer = deviceDataArray.Count

            Select Case data("flag")
                Case "2003"
                    ' Device info
                    Return ParseDevInfo(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "3004"
                    ' Device status
                    Return ParseDevStatus(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "3006"
                    ' OTA result
                    Return ParseOTAResult(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2007"
                    ' NTP Settings
                    Return ParseNTPSettings(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2008"
                    ' Time
                    Return ParseTime(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2009"
                    ' Commure timeout
                    Return ParseCommureTimeout(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "200a"
                    ' Indicator
                    Return ParseIndicator(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "200b"
                    ' Cert or OTA status
                    Return ParseUpdateFileStatus(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "200c"
                    ' Report settings
                    Return ParseReportSettings(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "200d"
                    ' Power off notification
                    Return ParsePowerOffNotify(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "200e"
                    ' Ble connect password
                    Return ParseBleConnectPassword(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "200f"
                    ' Password verify
                    Return ParsePasswordVerify(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "3010"
                    ' Power off alarm
                    Return ParsePowerOffAlarm(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "3011"
                    ' Low power alarm
                    Return ParseLowPowerAlarm(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2012"
                    ' Low power settings
                    Return ParseLowPower(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2013"
                    ' Battery voltage
                    Return ParseBattery(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2015"
                    ' Power on enable when charging
                    Return ParsePowerOn(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2020"
                    ' Network settings
                    Return ParseNetworkSettings(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2021"
                    ' Connect timeout
                    Return ParseConnTimeout(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2030"
                    ' MQTT settings
                    Return ParseMqttSettings(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "3032"
                    ' MQTT cert result
                    Return ParseMqttCertResult(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2040"
                    ' Scanner report mode
                    Return ParseScannerReportMode(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2041"
                    ' Always scan
                    Return ParseAlwaysScan(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2042"
                    ' Periodic Scan Immediate Report
                    Return ParsePeriodicScanImmediateReport(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2043"
                    ' Periodic Scan Periodic Report
                    Return ParsePeriodicScanPeriodicReport(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2050"
                    ' Filter relationship
                    Return ParseFilterRelationship(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2051"
                    ' Filter rssi
                    Return ParseFilterRssi(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2052"
                    ' Filter phy
                    Return ParseFilterPhy(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2053"
                    ' Filter mac
                    Return ParseFilterMac(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2054"
                    ' Filter name
                    Return ParseFilterName(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2055"
                    ' Filter rawdata
                    Return ParseFilterRawdata(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2056"
                    ' Filter ibeacon
                    Return ParseFilterIbeacon(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2057"
                    ' Filter eddystone_uid
                    Return ParseFilterEddystoneUID(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2058"
                    ' Filter eddystone_url
                    Return ParseFilterEddystoneURL(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2059"
                    ' Filter eddystone_tlm
                    Return ParseFilterEddystoneTLM(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "205a"
                    ' Filter bxp-devinfo
                    Return ParseFilterBXPDeviceInfo(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "205b"
                    ' Filter bxp-acc
                    Return ParseFilterBXPACC(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "205c"
                    ' Filter bxp-th
                    Return ParseFilterBXPTH(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "205d"
                    ' Filter bxp-button
                    Return ParseFilterBXPButton(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "205e"
                    ' Filter bxp-tag
                    Return ParseFilterBXPTag(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "205f"
                    ' Filter pir
                    Return ParseFilterPIR(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2060"
                    ' Filter tof
                    Return ParseFilterTOF(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2061"
                    ' Filter other
                    Return ParseFilterOther(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2062"
                    ' Filter Duplicate data
                    Return ParseFilterDuplicateData(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2070"
                    ' Adv settings
                    Return ParseAdvSettings(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2071"
                    ' Ibeacon settings
                    Return ParseIbeaconSettings(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2080"
                    ' Fix mode
                    Return ParseFixMode(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2081"
                    ' Fix interval
                    Return ParsePeriodicFix(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2082"
                    ' 3-Axis params
                    Return ParseAxisParams(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2083"
                    ' Motion start params
                    Return ParseMotionStartParams(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2084"
                    ' Motion in trip params
                    Return ParseMotionInTripParams(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2085"
                    ' Motion stop params
                    Return ParseMotionStopParams(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2086"
                    ' Stationary params
                    Return ParseStationaryParams(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2087"
                    ' GPS params
                    Return ParseGPSParams(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2090"
                    ' Ibeacon payload
                    Return ParseIbeaconPayload(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2091"
                    ' EddystoneUID payload
                    Return ParseEddystoneUIDPayload(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2092"
                    ' EddystoneURL payload
                    Return ParseEddystoneURLPayload(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2093"
                    ' EddystoneTLM payload
                    Return ParseEddystoneTLMPayload(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2094"
                    ' bxp-devinfo payload
                    Return ParseBXPDevInfoPayload(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2095"
                    ' bxp-acc payload
                    Return ParseBXPAccPayload(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2096"
                    ' bxp-th payload
                    Return ParseBXPTHPayload(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2097"
                    ' bxp-button payload
                    Return ParseBXPButtonPayload(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2098"
                    ' bxp-tag payload
                    Return ParseBXPTagPayload(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "2099"
                    ' pir payload
                    Return ParsePIRPayload(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "209a"
                    ' tof payload
                    Return ParseTOFPayload(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "209b"
                    ' other payload
                    Return ParseOtherPayload(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "3089", "30b1"
                    ' Fix data
                    Return ParseFixData(deviceDataIndex, deviceDataLength, deviceDataArray, data)
                Case "30a0", "30b2"
                    ' Scan devices
                    'log.Debug(value)
                    Return ParseScanDevices(deviceDataIndex, deviceDataLength, deviceDataArray, data)
            End Select
        End If
        Return value
    End Function

    'Public Function ParseBXPDevInfoPayload(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
    '    Dim bxpDevInfoPayload As New Dictionary(Of String, Integer)

    '    While deviceDataIndex < deviceDataLength
    '        Dim paramTag As Integer = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
    '        deviceDataIndex += 1

    '        Dim paramLength As Integer = ParseHexStrArraytoInt(deviceDataArray.GetRange(deviceDataIndex, 2))
    '        deviceDataIndex += 2

    '        If paramTag = 0 Then
    '            Dim value As Integer = ParseHexStrArraytoInt(deviceDataArray.GetRange(deviceDataIndex, paramLength))
    '            bxpDevInfoPayload("rssi") = value And &H1
    '            bxpDevInfoPayload("timestamp") = (value >> 1) And &H1
    '            bxpDevInfoPayload("txPower") = (value >> 2) And &H1
    '            bxpDevInfoPayload("rangingData") = (value >> 3) And &H1
    '            bxpDevInfoPayload("advInterval") = (value >> 4) And &H1
    '            bxpDevInfoPayload("battVoltage") = (value >> 5) And &H1
    '            bxpDevInfoPayload("devicePropertyIndicator") = (value >> 6) And &H1
    '            bxpDevInfoPayload("switchStatusIndicator") = (value >> 7) And &H1
    '            bxpDevInfoPayload("firmwareVersion") = (value >> 8) And &H1
    '            bxpDevInfoPayload("deviceName") = (value >> 9) And &H1
    '            bxpDevInfoPayload("rawAdv") = (value >> 10) And &H1
    '            bxpDevInfoPayload("rawResp") = (value >> 11) And &H1
    '        End If

    '        deviceDataIndex += paramLength
    '    End While

    '    data("bxpDevInfoPayload") = bxpDevInfoPayload
    '    Return Newtonsoft.Json.JsonConvert.SerializeObject(data)
    'End Function
    Sub ParseIbeacon(ByRef deviceItem As Dictionary(Of String, Object), paramTag As Integer, deviceDataArray As List(Of String), deviceDataIndex As Integer, paramLength As Integer)
        If paramTag = &HA Then
            deviceItem("uuid") = String.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength))
        ElseIf paramTag = &HB Then
            deviceItem("major") = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
        ElseIf paramTag = &HC Then
            deviceItem("minor") = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
        ElseIf paramTag = &HD Then
            deviceItem("rssi_1m") = SignedHexToInt(deviceDataArray(deviceDataIndex))
        End If
    End Sub

    ' Function to parse Eddystone UID data
    Sub ParseEddystoneUID(ByRef deviceItem As Dictionary(Of String, Object), paramTag As Integer, deviceDataArray As List(Of String), deviceDataIndex As Integer, paramLength As Integer)
        If paramTag = &HA Then
            deviceItem("namespace") = String.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength))
        ElseIf paramTag = &HB Then
            deviceItem("rssi_0m") = SignedHexToInt(deviceDataArray(deviceDataIndex))
        ElseIf paramTag = &HC Then
            deviceItem("instance") = String.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength))
        End If
    End Sub

    ' Function to parse Eddystone URL data
    Sub ParseEddystoneURL(ByRef deviceItem As Dictionary(Of String, Object), paramTag As Integer, deviceDataArray As List(Of String), deviceDataIndex As Integer, paramLength As Integer)
        If paramTag = &HA Then
            deviceItem("url") = HexStrToString(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
        ElseIf paramTag = &HB Then
            deviceItem("rssi_0m") = SignedHexToInt(deviceDataArray(deviceDataIndex))
        End If
    End Sub

    ' Function to parse Eddystone TLM data
    Sub ParseEddystoneTLM(ByRef deviceItem As Dictionary(Of String, Object), paramTag As Integer, deviceDataArray As List(Of String), deviceDataIndex As Integer, paramLength As Integer)
        If paramTag = &HA Then
            deviceItem("tlmVersion") = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
        ElseIf paramTag = &HB Then
            deviceItem("battVoltage") = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray()) & "mV"
        ElseIf paramTag = &HC Then
            deviceItem("temperature") = (SignedHexToInt(String.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength))) / 256).ToString("F1") & "℃"
        ElseIf paramTag = &HD Then
            deviceItem("advCnt") = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
        ElseIf paramTag = &HE Then
            deviceItem("escCnt") = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
        End If
    End Sub

    ' Function to parse device info data
    Sub ParseDeviceInfo(ByRef deviceItem As Dictionary(Of String, Object), paramTag As Integer, deviceDataArray As List(Of String), deviceDataIndex As Integer, paramLength As Integer)
        If paramTag = &HA Then
            deviceItem("txPower") = SignedHexToInt(deviceDataArray(deviceDataIndex))
        ElseIf paramTag = &HB Then
            deviceItem("rangingData") = SignedHexToInt(deviceDataArray(deviceDataIndex))
        ElseIf paramTag = &HC Then
            deviceItem("advInterval") = (Convert.ToInt32(deviceDataArray(deviceDataIndex), 16) * 100) & "ms"
        ElseIf paramTag = &HD Then
            deviceItem("battVoltage") = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray()) & "mV"
        ElseIf paramTag = &HE Then
            Dim status = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
            deviceItem("passwordVerificateStatus") = If((status And &H3) = 0, "Enable", "Disable")
            deviceItem("ambientLightSensorStatus") = If((status >> 2 And 1) = 0, "Enable", "Disable")
            deviceItem("hallDoorSensorStatus") = If((status >> 3 And 1) = 0, "Enable", "Disable")
        ElseIf paramTag = &HF Then
            Dim status = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
            deviceItem("connectable") = If((status And 1) = 0, "Enable", "Disable")
            deviceItem("ambientLightStatus") = If((status >> 1 And 1) = 0, "Enable", "Disable")
            deviceItem("doorStatus") = If((status >> 2 And 1) = 0, "Enable", "Disable")
        ElseIf paramTag = &H10 Then
            deviceItem("firmwareVersion") = HexStrToString(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
        ElseIf paramTag = &H11 Then
            deviceItem("deviceName") = HexStrToString(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
        End If
    End Sub

    ' Function to parse BXP ACC data
    Sub ParseBXPACC(ByRef deviceItem As Dictionary(Of String, Object), paramTag As Integer, deviceDataArray As List(Of String), deviceDataIndex As Integer, paramLength As Integer)
        If paramTag = &HA Then
            deviceItem("txPower") = SignedHexToInt(deviceDataArray(deviceDataIndex))
        ElseIf paramTag = &HB Then
            deviceItem("rangingData") = SignedHexToInt(deviceDataArray(deviceDataIndex))
        ElseIf paramTag = &HC Then
            deviceItem("advInterval") = (Convert.ToInt32(deviceDataArray(deviceDataIndex), 16) * 100) & "ms"
        ElseIf paramTag = &HD Then
            deviceItem("samplingRate") = samplingRateArray(Convert.ToInt32(deviceDataArray(deviceDataIndex), 16))
        ElseIf paramTag = &HE Then
            deviceItem("fullScale") = fullScaleArray(Convert.ToInt32(deviceDataArray(deviceDataIndex), 16))
        ElseIf paramTag = &HF Then
            deviceItem("motionThreshold") = (Convert.ToInt32(deviceDataArray(deviceDataIndex), 16) * 0.1) & "g"
        ElseIf paramTag = &H10 Then
            Dim axisDataArray = deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray()
            deviceItem("axisDataX") = axisDataArray(0) & "mg"
            deviceItem("axisDataY") = axisDataArray(1) & "mg"
            deviceItem("axisDataZ") = axisDataArray(2) & "mg"
        ElseIf paramTag = &H11 Then
            deviceItem("battVoltage") = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray()) & "mV"
        End If
    End Sub

    ' Function to parse BXP TH data
    Sub ParseBXPTH(ByRef deviceItem As Dictionary(Of String, Object), paramTag As Integer, deviceDataArray As List(Of String), deviceDataIndex As Integer, paramLength As Integer)
        If paramTag = &HA Then
            deviceItem("txPower") = SignedHexToInt(deviceDataArray(deviceDataIndex))
        ElseIf paramTag = &HB Then
            deviceItem("rangingData") = SignedHexToInt(deviceDataArray(deviceDataIndex))
        ElseIf paramTag = &HC Then
            deviceItem("advInterval") = (Convert.ToInt32(deviceDataArray(deviceDataIndex), 16) * 100) & "ms"
        ElseIf paramTag = &HD Then
            deviceItem("temperature") = (SignedHexToInt(String.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength))) * 0.1).ToString("F1") & "℃"
        ElseIf paramTag = &HE Then
            deviceItem("humidity") = (SignedHexToInt(String.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength))) * 0.1).ToString("F1") & "%"
        ElseIf paramTag = &HF Then
            deviceItem("battVoltage") = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray()) & "mV"
        End If
    End Sub

    ' Function to parse BXP Button data
    Sub ParseBXPButton(ByRef deviceItem As Dictionary(Of String, Object), paramTag As Integer, deviceDataArray As List(Of String), deviceDataIndex As Integer, paramLength As Integer)
        If paramTag = &HA Then
            deviceItem("frameType") = frameTypeArray(Convert.ToInt32(deviceDataArray(deviceDataIndex), 16) And &HF)
        ElseIf paramTag = &HB Then
            Dim status = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
            deviceItem("passwordVerifyFlag") = status And 1
            deviceItem("triggerStatus") = status >> 1 And 1
        ElseIf paramTag = &HC Then
            deviceItem("triggerCount") = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
        ElseIf paramTag = &HD Then
            deviceItem("devId") = String.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength))
        ElseIf paramTag = &HE Then
            deviceItem("firmwareType") = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
        ElseIf paramTag = &HF Then
            deviceItem("devName") = HexStrToString(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
        ElseIf paramTag = &H10 Then
            deviceItem("fullScale") = fullScaleArray(Convert.ToInt32(deviceDataArray(deviceDataIndex), 16))
        ElseIf paramTag = &H11 Then
            deviceItem("motionThreshold") = (Convert.ToInt32(deviceDataArray(deviceDataIndex), 16) * 0.1) & "g"
        ElseIf paramTag = &H12 Then
            Dim axisDataArray = deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray()
            deviceItem("axisDataX") = axisDataArray(0) & "mg"
            deviceItem("axisDataY") = axisDataArray(1) & "mg"
            deviceItem("axisDataZ") = axisDataArray(2) & "mg"
        ElseIf paramTag = &H13 Then
            deviceItem("temperature") = (SignedHexToInt(String.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength))) * 0.25).ToString("F1") & "℃"
        ElseIf paramTag = &H14 Then
            deviceItem("rangingData") = SignedHexToInt(deviceDataArray(deviceDataIndex))
        ElseIf paramTag = &H15 Then
            deviceItem("battVoltage") = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray()) & "mV"
        ElseIf paramTag = &H16 Then
            deviceItem("txPower") = SignedHexToInt(deviceDataArray(deviceDataIndex))
        End If
    End Sub

    ' Function to parse BXP Tag data
    Sub ParseBXPTag(ByRef deviceItem As Dictionary(Of String, Object), paramTag As Integer, deviceDataArray As List(Of String), deviceDataIndex As Integer, paramLength As Integer)
        If paramTag = &HA Then
            Dim status = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
            deviceItem("hallSensorStatus") = If((status And 1) = 1, "Enable", "Disable")
            deviceItem("axisStatus") = If((status >> 1 And 1) = 1, "Enable", "Disable")
            deviceItem("axisEquippedStatus") = If((status >> 2 And 1) = 1, "Enable", "Disable")
        ElseIf paramTag = &HB Then
            deviceItem("hallTriggerEventCount") = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
        ElseIf paramTag = &HC Then
            deviceItem("motionTriggerEventCount") = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
        ElseIf paramTag = &HD Then
            Dim axisDataArray = deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray()
            deviceItem("axisDataX") = axisDataArray(0) & "mg"
            deviceItem("axisDataY") = axisDataArray(1) & "mg"
            deviceItem("axisDataZ") = axisDataArray(2) & "mg"
        ElseIf paramTag = &HE Then
            deviceItem("battVoltage") = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray()) & "mV"
        ElseIf paramTag = &HF Then
            deviceItem("tagId") = String.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength))
        ElseIf paramTag = &H10 Then
            deviceItem("devName") = HexStrToString(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
        End If
    End Sub

    ' Function to parse PIR data
    Sub ParsePIR(ByRef deviceItem As Dictionary(Of String, Object), paramTag As Integer, deviceDataArray As List(Of String), deviceDataIndex As Integer, paramLength As Integer)
        If paramTag = &HA Then
            deviceItem("pirDelayResponseStatus") = pirDelayResponseStatusArray(Convert.ToInt32(deviceDataArray(deviceDataIndex), 16))
        ElseIf paramTag = &HB Then
            deviceItem("doorStatus") = If(Convert.ToInt32(deviceDataArray(deviceDataIndex), 16) = 0, "open", "close")
        ElseIf paramTag = &HC Then
            deviceItem("sensorSensitivity") = pirDelayResponseStatusArray(Convert.ToInt32(deviceDataArray(deviceDataIndex), 16))
        ElseIf paramTag = &HD Then
            deviceItem("sensorDetectionStatus") = If(Convert.ToInt32(deviceDataArray(deviceDataIndex), 16) = 0, "no effective motion", "effective motion")
        ElseIf paramTag = &HE Then
            deviceItem("battVoltage") = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray()) & "mV"
        ElseIf paramTag = &HF Then
            deviceItem("major") = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
        ElseIf paramTag = &H10 Then
            deviceItem("minor") = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
        ElseIf paramTag = &H11 Then
            deviceItem("rssi_1m") = SignedHexToInt(deviceDataArray(deviceDataIndex))
        ElseIf paramTag = &H12 Then
            deviceItem("txPower") = SignedHexToInt(deviceDataArray(deviceDataIndex))
        ElseIf paramTag = &H13 Then
            deviceItem("devName") = HexStrToString(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
        End If
    End Sub

    ' Function to parse TOF data
    Sub ParseTOF(ByRef deviceItem As Dictionary(Of String, Object), paramTag As Integer, deviceDataArray As List(Of String), deviceDataIndex As Integer, paramLength As Integer)
        'log.Debug("ParseTOF")
        If paramTag = &HA Then
            deviceItem("manufacturerVendorCode") = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
        ElseIf paramTag = &HB Then
            deviceItem("battVoltage") = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray()) & "mV"
        ElseIf paramTag = &HC Then
            deviceItem("userData") = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
        ElseIf paramTag = &HD Then
            deviceItem("randingDistance") = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
        End If
    End Sub

    ' Function to parse Other data
    Sub ParseOther(ByRef deviceItem As Dictionary(Of String, Object), paramTag As Integer, deviceDataArray As List(Of String), deviceDataIndex As Integer, paramLength As Integer)
        If paramTag = &HA Then
            deviceItem("dataBlock1") = String.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength))
        ElseIf paramTag = &HB Then
            deviceItem("dataBlock2") = String.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength))
        ElseIf paramTag = &HC Then
            deviceItem("dataBlock3") = String.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength))
        ElseIf paramTag = &HD Then
            deviceItem("dataBlock4") = String.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength))
        ElseIf paramTag = &HE Then
            deviceItem("dataBlock5") = String.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength))
        ElseIf paramTag = &HF Then
            deviceItem("dataBlock6") = String.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength))
        ElseIf paramTag = &H10 Then
            deviceItem("dataBlock7") = String.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength))
        ElseIf paramTag = &H11 Then
            deviceItem("dataBlock8") = String.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength))
        ElseIf paramTag = &H12 Then
            deviceItem("dataBlock9") = String.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength))
        ElseIf paramTag = &H13 Then
            deviceItem("dataBlock10") = String.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength))
        End If
    End Sub

    Function SignedHexToInt(hexStr As String) As Integer
        Dim value As Integer = Convert.ToInt32(hexStr, 16)
        If value And &H80 Then
            value = value - &H100
        End If
        Return value
    End Function

    Function HexStrToString(hexArray As String()) As String
        Dim sb As New StringBuilder()
        For Each hex As String In hexArray
            sb.Append(Convert.ToChar(Convert.ToUInt32(hex, 16)))
        Next
        Return sb.ToString()
    End Function

    Function ParseHexStrArrayToInt(hexArray As String()) As Integer
        Return Convert.ToInt32(String.Join("", hexArray), 16)
    End Function

    Function ParseTime(timestamp As Integer, timezone As Integer) As DateTime
        ' Implement the parsing logic for time based on timestamp and timezone
        Return DateTimeOffset.FromUnixTimeSeconds(timestamp).ToLocalTime().DateTime.AddHours(timezone)
    End Function
    Public Function ToHexStrArray(value As String) As List(Of String)
        Dim rep As String = value.Replace(" ", "")
        Dim array As New List(Of String)()
        Dim arrLen As Integer = rep.Length \ 2
        For i As Integer = 0 To arrLen - 1
            array.Add(rep.Substring(i * 2, 2))
        Next
        Return array
    End Function

    Function ParseHexStrArraytoInt(hexStrArray As List(Of String)) As Integer
        Return Convert.ToInt32(String.Join("", hexStrArray), 16)
    End Function

    Function HexStrToString(value As List(Of String)) As String
        Dim array As New List(Of Char)()
        Dim arrLen As Integer = value.Count
        For i As Integer = 0 To arrLen - 1
            array.Add(ChrW(Convert.ToInt32(value(i), 16)))
        Next
        Return New String(array.ToArray())
    End Function


    Public Function ParseDevInfo(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        Dim deviceInfo As New Dictionary(Of String, String)

        While deviceDataIndex < deviceDataLength
            Dim paramTag As Integer = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
            deviceDataIndex += 1

            Dim paramLength As Integer = ParseHexStrArraytoInt(deviceDataArray.GetRange(deviceDataIndex, 2))
            deviceDataIndex += 2

            Select Case paramTag
                Case 0
                    deviceInfo("deviceName") = HexStrToString(deviceDataArray.GetRange(deviceDataIndex, paramLength))
                Case 1
                    deviceInfo("productModel") = HexStrToString(deviceDataArray.GetRange(deviceDataIndex, paramLength))
                Case 2
                    deviceInfo("companyName") = HexStrToString(deviceDataArray.GetRange(deviceDataIndex, paramLength))
                Case 3
                    deviceInfo("hardwareVersion") = HexStrToString(deviceDataArray.GetRange(deviceDataIndex, paramLength))
                Case 4
                    deviceInfo("softwareVersion") = HexStrToString(deviceDataArray.GetRange(deviceDataIndex, paramLength))
                Case 5
                    deviceInfo("firmwareVersion") = HexStrToString(deviceDataArray.GetRange(deviceDataIndex, paramLength))
                Case 6
                    deviceInfo("imei") = HexStrToString(deviceDataArray.GetRange(deviceDataIndex, paramLength))
                Case 7
                    deviceInfo("iccid") = HexStrToString(deviceDataArray.GetRange(deviceDataIndex, paramLength))
            End Select

            deviceDataIndex += paramLength
        End While

        data("deviceInfo") = deviceInfo
        Return Newtonsoft.Json.JsonConvert.SerializeObject(data)
    End Function

    Function ParseDevStatus(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        Dim deviceStatus As New Dictionary(Of String, Object)()

        While deviceDataIndex < deviceDataLength
            Dim paramTag As Integer = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
            deviceDataIndex += 1
            Dim paramLength As Integer = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray())
            deviceDataIndex += 2

            Select Case paramTag
                Case 0
                    deviceStatus("timestamp") = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
                Case 1
                    deviceStatus("netwrokType") = HexStrToString(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
                Case 2
                    deviceStatus("csq") = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
                Case 3
                    deviceStatus("battVoltage") = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray()) & "mV"
                Case 4
                    Dim axisDataArray = deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray()
                    deviceStatus("axisDataX") = axisDataArray(0) & "mg"
                    deviceStatus("axisDataY") = axisDataArray(1) & "mg"
                    deviceStatus("axisDataZ") = axisDataArray(2) & "mg"
                Case 5
                    deviceStatus("accStatus") = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
                Case 6
                    deviceStatus("imei") = HexStrToString(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
            End Select

            deviceDataIndex += paramLength
        End While

        data("deviceStatus") = deviceStatus
        Return Newtonsoft.Json.JsonConvert.SerializeObject(data)
    End Function
    Public Function ParseTOFPayload(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        Dim tofPayload As New Dictionary(Of String, Object)()

        While deviceDataIndex < deviceDataLength
            Dim paramTag As Integer = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
            deviceDataIndex += 1
            Dim paramLength As Integer = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray())
            deviceDataIndex += 2

            If paramTag = 0 Then
                Dim value As Integer = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
                tofPayload("rssi") = value And 1
                tofPayload("timestamp") = (value >> 1) And 1
                tofPayload("manufacturerVendorCode") = (value >> 2) And 1
                tofPayload("battVoltage") = (value >> 3) And 1
                tofPayload("userData") = (value >> 4) And 1
                tofPayload("randingDistance") = (value >> 5) And 1
                tofPayload("rawAdv") = (value >> 6) And 1
                tofPayload("rawResp") = (value >> 7) And 1
            End If

            deviceDataIndex += paramLength
        End While

        data("tofPayload") = tofPayload
        Return Newtonsoft.Json.JsonConvert.SerializeObject(data)
    End Function
    Public Function ParseOTAResult(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseOTAResult")
    End Function

    Public Function ParseNTPSettings(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        log.Debug("ParseNTPSettings")
        ' Your implementation here
    End Function

    Public Function ParseTime(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseTime")

    End Function

    Public Function ParseCommureTimeout(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseCommureTimeout")
    End Function

    Public Function ParseIndicator(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseIndicator")
    End Function

    Public Function ParseUpdateFileStatus(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseUpdateFileStatus")
    End Function

    Public Function ParseReportSettings(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseReportSettings")
    End Function

    Public Function ParsePowerOffNotify(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParsePowerOffNotify")
    End Function

    Public Function ParseBleConnectPassword(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseBleConnectPassword")
    End Function

    Public Function ParsePasswordVerify(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParsePasswordVerify")
    End Function

    Function ParsePowerOffAlarm(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        Dim powerOffAlarm As New Dictionary(Of String, Object)()

        While deviceDataIndex < deviceDataLength
            Dim paramTag As Integer = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
            deviceDataIndex += 1
            Dim paramLength As Integer = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray())
            deviceDataIndex += 2

            If paramTag = 0 Then
                powerOffAlarm("timestamp") = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
                powerOffAlarm("timeStr") = ParseTime(powerOffAlarm("timestamp"), 0)
            ElseIf paramTag = 1 Then
                powerOffAlarm("battVoltage") = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray()) & "mV"
            End If

            deviceDataIndex += paramLength
        End While

        data("powerOffAlarm") = powerOffAlarm
        Return Newtonsoft.Json.JsonConvert.SerializeObject(data)
    End Function

    Function ParseLowPowerAlarm(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        Dim lowPowerAlarm As New Dictionary(Of String, Object)()

        While deviceDataIndex < deviceDataLength
            Dim paramTag As Integer = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
            deviceDataIndex += 1
            Dim paramLength As Integer = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray())
            deviceDataIndex += 2

            If paramTag = 0 Then
                lowPowerAlarm("timestamp") = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
                lowPowerAlarm("timeStr") = ParseTime(lowPowerAlarm("timestamp"), 0)
            ElseIf paramTag = 1 Then
                lowPowerAlarm("battVoltage") = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray()) & "mV"
            End If

            deviceDataIndex += paramLength
        End While

        data("lowPowerAlarm") = lowPowerAlarm
        Return Newtonsoft.Json.JsonConvert.SerializeObject(data)
    End Function


    Function ParseLowPower(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        Dim lowPower As New Dictionary(Of String, Object)()

        While deviceDataIndex < deviceDataLength
            Dim paramTag As Integer = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
            deviceDataIndex += 1
            Dim paramLength As Integer = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray())
            deviceDataIndex += 2

            If paramTag = 0 Then
                lowPower("enable") = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
            ElseIf paramTag = 1 Then
                lowPower("percentage") = lowPowerArray(Convert.ToInt32(deviceDataArray(deviceDataIndex), 16))
            End If

            deviceDataIndex += paramLength
        End While

        data("lowPower") = lowPower
        Return Newtonsoft.Json.JsonConvert.SerializeObject(data)
    End Function

    Function ParseBattery(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        Dim battery As New Dictionary(Of String, Object)()

        While deviceDataIndex < deviceDataLength
            Dim paramTag As Integer = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
            deviceDataIndex += 1
            Dim paramLength As Integer = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray())
            deviceDataIndex += 2

            If paramTag = 0 Then
                battery("voltage") = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray()) & "mV"
            End If

            deviceDataIndex += paramLength
        End While

        data("battery") = battery
        Return Newtonsoft.Json.JsonConvert.SerializeObject(data)
    End Function

    Function ParsePowerOn(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        Dim powerOn As New Dictionary(Of String, Object)()

        While deviceDataIndex < deviceDataLength
            Dim paramTag As Integer = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
            deviceDataIndex += 1
            Dim paramLength As Integer = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray())
            deviceDataIndex += 2

            If paramTag = 0 Then
                powerOn("enable") = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
            End If

            deviceDataIndex += paramLength
        End While

        data("powerOn") = powerOn
        Return Newtonsoft.Json.JsonConvert.SerializeObject(data)
    End Function

    Public Function ParseNetworkSettings(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseNetworkSettings")
    End Function

    Public Function ParseConnTimeout(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseConnTimeout")
    End Function

    Public Function ParseMqttSettings(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseMqttSettings")
    End Function

    Public Function ParseMqttCertResult(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseMqttCertResult")
    End Function

    Public Function ParseScannerReportMode(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseScannerReportMode")
    End Function

    Public Function ParseAlwaysScan(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseAlwaysScan")
    End Function

    Public Function ParsePeriodicScanImmediateReport(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParsePeriodicScanImmediateReport")
    End Function

    Public Function ParsePeriodicScanPeriodicReport(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParsePeriodicScanPeriodicReport")
    End Function

    Public Function ParseFilterRelationship(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseFilterRelationship")
    End Function

    Public Function ParseFilterRssi(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseFilterRssi")
    End Function

    Public Function ParseFilterPhy(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseFilterPhy")
    End Function

    Public Function ParseFilterMac(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseFilterMac")
    End Function

    Public Function ParseFilterName(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseFilterName")
    End Function

    Public Function ParseFilterRawdata(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseFilterRawdata")
    End Function

    Public Function ParseFilterIbeacon(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseFilterIbeacon")
    End Function

    Public Function ParseFilterEddystoneUID(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseFilterEddystoneUID")
    End Function

    Public Function ParseFilterEddystoneURL(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseFilterEddystoneURL")
    End Function

    Public Function ParseFilterEddystoneTLM(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseFilterEddystoneTLM")
    End Function

    Function ParseFilterBXPDeviceInfo(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        Dim filterBXPDeviceInfo As New Dictionary(Of String, Integer)

        While deviceDataIndex < deviceDataLength
            Dim paramTag As Integer = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
            deviceDataIndex += 1

            Dim paramLength As Integer = ParseHexStrArraytoInt(deviceDataArray.GetRange(deviceDataIndex, 2))
            deviceDataIndex += 2

            If paramTag = 0 Then
                filterBXPDeviceInfo("enable") = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
            End If

            deviceDataIndex += paramLength
        End While

        data("filterBXPDeviceInfo") = filterBXPDeviceInfo
        Return Newtonsoft.Json.JsonConvert.SerializeObject(data)
    End Function


    Public Function ParseFilterBXPACC(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseFilterBXPACC")
    End Function

    Public Function ParseFilterBXPTH(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseFilterBXPTH")
    End Function

    Public Function ParseFilterBXPButton(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseFilterBXPButton")
    End Function

    Public Function ParseFilterBXPTag(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseFilterBXPTag")
    End Function

    Public Function ParseFilterPIR(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseFilterPIR")
    End Function

    Public Function ParseFilterTOF(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseFilterTOF")
    End Function

    Public Function ParseFilterOther(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseFilterOther")
    End Function

    Public Function ParseFilterDuplicateData(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseFilterDuplicateData")
    End Function

    Public Function ParseAdvSettings(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseAdvSettings")
    End Function

    Public Function ParseIbeaconSettings(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseIbeaconSettings")
    End Function

    Public Function ParseFixMode(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseFixMode")
    End Function

    Public Function ParsePeriodicFix(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParsePeriodicFix")
    End Function

    Public Function ParseAxisParams(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseAxisParams")
    End Function

    Public Function ParseMotionStartParams(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseMotionStartParams")
    End Function

    Public Function ParseMotionInTripParams(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseMotionInTripParams")
    End Function

    Public Function ParseMotionStopParams(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseMotionStopParams")
    End Function

    Public Function ParseStationaryParams(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        ' Your implementation here
        log.Debug("ParseStationaryParams")
    End Function
    Function ParseGPSParams(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        Dim gpsParams As New Dictionary(Of String, Object)()

        While deviceDataIndex < deviceDataLength
            Dim paramTag As Integer = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
            deviceDataIndex += 1
            Dim paramLength As Integer = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray())
            deviceDataIndex += 2

            If paramTag = 0 Then
                gpsParams("timeout") = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
            ElseIf paramTag = 1 Then
                gpsParams("pdop") = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
            End If

            deviceDataIndex += paramLength
        End While

        data("gpsParams") = gpsParams
        Return Newtonsoft.Json.JsonConvert.SerializeObject(data)
    End Function

    Function ParseIbeaconPayload(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        Dim ibeaconPayload As New Dictionary(Of String, Object)()

        While deviceDataIndex < deviceDataLength
            Dim paramTag As Integer = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
            deviceDataIndex += 1
            Dim paramLength As Integer = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray())
            deviceDataIndex += 2

            If paramTag = 0 Then
                Dim value As Integer = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
                ibeaconPayload("rssi") = value And 1
                ibeaconPayload("timestamp") = (value >> 1) And 1
                ibeaconPayload("uuid") = (value >> 2) And 1
                ibeaconPayload("major") = (value >> 3) And 1
                ibeaconPayload("minor") = (value >> 4) And 1
                ibeaconPayload("rssi_1m") = (value >> 5) And 1
                ibeaconPayload("rawAdv") = (value >> 6) And 1
                ibeaconPayload("rawResp") = (value >> 7) And 1
            End If

            deviceDataIndex += paramLength
        End While

        data("ibeaconPayload") = ibeaconPayload
        Return Newtonsoft.Json.JsonConvert.SerializeObject(data)
    End Function

    Function ParseEddystoneUIDPayload(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        Dim eddystoneUIDPayload As New Dictionary(Of String, Object)()

        While deviceDataIndex < deviceDataLength
            Dim paramTag As Integer = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
            deviceDataIndex += 1
            Dim paramLength As Integer = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray())
            deviceDataIndex += 2

            If paramTag = 0 Then
                Dim value As Integer = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
                eddystoneUIDPayload("rssi") = value And 1
                eddystoneUIDPayload("timestamp") = (value >> 1) And 1
                eddystoneUIDPayload("rssi_0m") = (value >> 2) And 1
                eddystoneUIDPayload("namespace") = (value >> 3) And 1
                eddystoneUIDPayload("instance") = (value >> 4) And 1
                eddystoneUIDPayload("rawAdv") = (value >> 5) And 1
                eddystoneUIDPayload("rawResp") = (value >> 6) And 1
            End If

            deviceDataIndex += paramLength
        End While

        data("eddystoneUIDPayload") = eddystoneUIDPayload
        Return Newtonsoft.Json.JsonConvert.SerializeObject(data)
    End Function

    Function ParseEddystoneURLPayload(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        Dim eddystoneURLPayload As New Dictionary(Of String, Object)()

        While deviceDataIndex < deviceDataLength
            Dim paramTag As Integer = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
            deviceDataIndex += 1
            Dim paramLength As Integer = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray())
            deviceDataIndex += 2

            If paramTag = 0 Then
                Dim value As Integer = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
                eddystoneURLPayload("rssi") = value And 1
                eddystoneURLPayload("timestamp") = (value >> 1) And 1
                eddystoneURLPayload("rssi_0m") = (value >> 2) And 1
                eddystoneURLPayload("url") = (value >> 3) And 1
                eddystoneURLPayload("rawAdv") = (value >> 4) And 1
                eddystoneURLPayload("rawResp") = (value >> 5) And 1
            End If

            deviceDataIndex += paramLength
        End While

        data("eddystoneURLPayload") = eddystoneURLPayload
        Return Newtonsoft.Json.JsonConvert.SerializeObject(data)
    End Function

    Function ParseEddystoneTLMPayload(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        Dim eddystoneTLMPayload As New Dictionary(Of String, Object)()

        While deviceDataIndex < deviceDataLength
            Dim paramTag As Integer = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
            deviceDataIndex += 1
            Dim paramLength As Integer = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray())
            deviceDataIndex += 2

            If paramTag = 0 Then
                Dim value As Integer = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
                eddystoneTLMPayload("rssi") = value And 1
                eddystoneTLMPayload("timestamp") = (value >> 1) And 1
                eddystoneTLMPayload("tlmVersion") = (value >> 2) And 1
                eddystoneTLMPayload("battVoltage") = (value >> 3) And 1
                eddystoneTLMPayload("temperature") = (value >> 4) And 1
                eddystoneTLMPayload("advCnt") = (value >> 5) And 1
                eddystoneTLMPayload("secCnt") = (value >> 6) And 1
                eddystoneTLMPayload("rawAdv") = (value >> 7) And 1
                eddystoneTLMPayload("rawResp") = (value >> 8) And 1
            End If

            deviceDataIndex += paramLength
        End While

        data("eddystoneTLMPayload") = eddystoneTLMPayload
        Return Newtonsoft.Json.JsonConvert.SerializeObject(data)
    End Function

    Function ParseBXPDevInfoPayload(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        Dim bxpDevInfoPayload As New Dictionary(Of String, Object)()

        While deviceDataIndex < deviceDataLength
            Dim paramTag As Integer = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
            deviceDataIndex += 1
            Dim paramLength As Integer = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray())
            deviceDataIndex += 2

            If paramTag = 0 Then
                Dim value As Integer = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
                bxpDevInfoPayload("rssi") = value And 1
                bxpDevInfoPayload("timestamp") = (value >> 1) And 1
                bxpDevInfoPayload("txPower") = (value >> 2) And 1
                bxpDevInfoPayload("rangingData") = (value >> 3) And 1
                bxpDevInfoPayload("advInterval") = (value >> 4) And 1
                bxpDevInfoPayload("battVoltage") = (value >> 5) And 1
                bxpDevInfoPayload("devicePropertyIndicator") = (value >> 6) And 1
                bxpDevInfoPayload("switchStatusIndicator") = (value >> 7) And 1
                bxpDevInfoPayload("firmwareVersion") = (value >> 8) And 1
                bxpDevInfoPayload("deviceName") = (value >> 9) And 1
                bxpDevInfoPayload("rawAdv") = (value >> 10) And 1
                bxpDevInfoPayload("rawResp") = (value >> 11) And 1
            End If

            deviceDataIndex += paramLength
        End While

        data("bxpDevInfoPayload") = bxpDevInfoPayload
        Return Newtonsoft.Json.JsonConvert.SerializeObject(data)
    End Function

    Function ParseBXPAccPayload(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        Dim bxpAccPayload As New Dictionary(Of String, Object)()

        While deviceDataIndex < deviceDataLength
            Dim paramTag As Integer = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
            deviceDataIndex += 1
            Dim paramLength As Integer = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray())
            deviceDataIndex += 2

            If paramTag = 0 Then
                Dim value As Integer = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
                bxpAccPayload("rssi") = value And 1
                bxpAccPayload("timestamp") = (value >> 1) And 1
                bxpAccPayload("txPower") = (value >> 2) And 1
                bxpAccPayload("rangingData") = (value >> 3) And 1
                bxpAccPayload("advInterval") = (value >> 4) And 1
                bxpAccPayload("samplingRate") = (value >> 5) And 1
                bxpAccPayload("fullScale") = (value >> 6) And 1
                bxpAccPayload("motionThreshold") = (value >> 7) And 1
                bxpAccPayload("axisData") = (value >> 8) And 1
                bxpAccPayload("battVoltage") = (value >> 9) And 1
                bxpAccPayload("rawAdv") = (value >> 10) And 1
            End If

            deviceDataIndex += paramLength
        End While

        data("bxpAccPayload") = bxpAccPayload
        Return Newtonsoft.Json.JsonConvert.SerializeObject(data)
    End Function

    Function ParseBXPTHPayload(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        Dim bxpTHPayload As New Dictionary(Of String, Object)()

        While deviceDataIndex < deviceDataLength
            Dim paramTag As Integer = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
            deviceDataIndex += 1
            Dim paramLength As Integer = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray())
            deviceDataIndex += 2

            If paramTag = 0 Then
                Dim value As Integer = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
                bxpTHPayload("rssi") = value And 1
                bxpTHPayload("timestamp") = (value >> 1) And 1
                bxpTHPayload("txPower") = (value >> 2) And 1
                bxpTHPayload("rangingData") = (value >> 3) And 1
                bxpTHPayload("advInterval") = (value >> 4) And 1
                bxpTHPayload("temperature") = (value >> 5) And 1
                bxpTHPayload("humidity") = (value >> 6) And 1
                bxpTHPayload("battVoltage") = (value >> 7) And 1
                bxpTHPayload("rawAdv") = (value >> 8) And 1
            End If

            deviceDataIndex += paramLength
        End While

        data("bxpTHPayload") = bxpTHPayload
        Return Newtonsoft.Json.JsonConvert.SerializeObject(data)
    End Function

    Function ParseBXPButtonPayload(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        Dim bxpButtonPayload As New Dictionary(Of String, Object)()

        While deviceDataIndex < deviceDataLength
            Dim paramTag As Integer = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
            deviceDataIndex += 1
            Dim paramLength As Integer = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray())
            deviceDataIndex += 2

            If paramTag = 0 Then
                Dim value As Integer = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
                bxpButtonPayload("rssi") = value And 1
                bxpButtonPayload("timestamp") = (value >> 1) And 1
                bxpButtonPayload("frameType") = (value >> 2) And 1
                bxpButtonPayload("statusFlag") = (value >> 3) And 1
                bxpButtonPayload("triggerCount") = (value >> 4) And 1
                bxpButtonPayload("deviceId") = (value >> 5) And 1
                bxpButtonPayload("firmwareType") = (value >> 6) And 1
                bxpButtonPayload("deviceName") = (value >> 7) And 1
                bxpButtonPayload("fullScale") = (value >> 8) And 1
                bxpButtonPayload("motionThreshold") = (value >> 9) And 1
                bxpButtonPayload("axisData") = (value >> 10) And 1
                bxpButtonPayload("temperature") = (value >> 11) And 1
                bxpButtonPayload("rangingData") = (value >> 12) And 1
                bxpButtonPayload("battVoltage") = (value >> 13) And 1
                bxpButtonPayload("txPower") = (value >> 14) And 1
                bxpButtonPayload("rawAdv") = (value >> 15) And 1
                bxpButtonPayload("rawResp") = (value >> 16) And 1
            End If

            deviceDataIndex += paramLength
        End While

        data("bxpButtonPayload") = bxpButtonPayload
        Return Newtonsoft.Json.JsonConvert.SerializeObject(data)
    End Function

    Function ParseBXPTagPayload(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        Dim bxpTagPayload As New Dictionary(Of String, Object)()

        While deviceDataIndex < deviceDataLength
            Dim paramTag As Integer = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
            deviceDataIndex += 1
            Dim paramLength As Integer = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray())
            deviceDataIndex += 2

            If paramTag = 0 Then
                Dim value As Integer = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
                bxpTagPayload("rssi") = value And 1
                bxpTagPayload("timestamp") = (value >> 1) And 1
                bxpTagPayload("sensorData") = (value >> 2) And 1
                bxpTagPayload("hallTriggerEventCount") = (value >> 3) And 1
                bxpTagPayload("motionTriggerEventCount") = (value >> 4) And 1
                bxpTagPayload("axisData") = (value >> 5) And 1
                bxpTagPayload("battVoltage") = (value >> 6) And 1
                bxpTagPayload("tagId") = (value >> 7) And 1
                bxpTagPayload("deviceName") = (value >> 8) And 1
                bxpTagPayload("rawAdv") = (value >> 9) And 1
                bxpTagPayload("rawResp") = (value >> 10) And 1
            End If

            deviceDataIndex += paramLength
        End While

        data("bxpTagPayload") = bxpTagPayload
        Return Newtonsoft.Json.JsonConvert.SerializeObject(data)
    End Function

    Function ParsePIRPayload(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        Dim pirPayload As New Dictionary(Of String, Object)()

        While deviceDataIndex < deviceDataLength
            Dim paramTag As Integer = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
            deviceDataIndex += 1
            Dim paramLength As Integer = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray())
            deviceDataIndex += 2

            If paramTag = 0 Then
                Dim value As Integer = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
                pirPayload("rssi") = value And 1
                pirPayload("timestamp") = (value >> 1) And 1
                pirPayload("pirDelayResponseStatus") = (value >> 2) And 1
                pirPayload("doorStatus") = (value >> 3) And 1
                pirPayload("sensorSensitivity") = (value >> 4) And 1
                pirPayload("sensorDetectionStatus") = (value >> 5) And 1
                pirPayload("battVoltage") = (value >> 6) And 1
                pirPayload("major") = (value >> 7) And 1
                pirPayload("minor") = (value >> 8) And 1
                pirPayload("rssi_1m") = (value >> 9) And 1
                pirPayload("txPower") = (value >> 10) And 1
                pirPayload("advName") = (value >> 11) And 1
                pirPayload("rawAdv") = (value >> 12) And 1
                pirPayload("rawResp") = (value >> 13) And 1
            End If

            deviceDataIndex += paramLength
        End While

        data("pirPayload") = pirPayload
        Return Newtonsoft.Json.JsonConvert.SerializeObject(data)
    End Function
    Function ParseOtherPayload(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        Dim otherPayload As New Dictionary(Of String, Object)()
        Dim array As New List(Of Dictionary(Of String, Object))()

        While deviceDataIndex < deviceDataLength
            Dim paramTag As Integer = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
            deviceDataIndex += 1
            Dim paramLength As Integer = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray())
            deviceDataIndex += 2

            If paramTag = 0 Then
                Dim value As Integer = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
                otherPayload("rssi") = value And 1
                otherPayload("timestamp") = (value >> 1) And 1
                otherPayload("rawAdv") = (value >> 2) And 1
                otherPayload("rawResp") = (value >> 3) And 1
            ElseIf paramTag = 1 Then
                Dim item As New Dictionary(Of String, Object)()
                item("type") = deviceDataArray(deviceDataIndex)
                item("start") = Convert.ToInt32(deviceDataArray(deviceDataIndex + 1), 16)
                item("end") = Convert.ToInt32(deviceDataArray(deviceDataIndex + 2), 16)
                item("data") = String.Join("", deviceDataArray.Skip(deviceDataIndex + 3).Take(paramLength - 3))
                array.Add(item)
            End If

            deviceDataIndex += paramLength
        End While

        otherPayload("array") = array
        data("otherPayload") = otherPayload
        Return Newtonsoft.Json.JsonConvert.SerializeObject(data)
    End Function

    Function ParseFixData(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        Dim fixData As New Dictionary(Of String, Object)()

        While deviceDataIndex < deviceDataLength
            Dim paramTag As Integer = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
            deviceDataIndex += 1
            Dim paramLength As Integer = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray())
            deviceDataIndex += 2

            If paramTag = 0 Then
                fixData("timestamp") = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
                fixData("current_time") = ParseTime(fixData("timestamp"), 0)
            ElseIf paramTag = 1 Then
                fixData("fixMode") = fixModeNotifyArray(Convert.ToInt32(deviceDataArray(deviceDataIndex), 16))
            ElseIf paramTag = 2 Then
                fixData("fixResult") = fixResultArray(Convert.ToInt32(deviceDataArray(deviceDataIndex), 16))
            ElseIf paramTag = 3 Then
                fixData("longitude") = (SignedHexToInt(String.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength - 4))) * 0.0000001).ToString("F7")
                fixData("latitude") = (SignedHexToInt(String.Join("", deviceDataArray.Skip(deviceDataIndex + 4).Take(paramLength - 4))) * 0.0000001).ToString("F7")
            ElseIf paramTag = 4 Then
                Dim value As Integer = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray())
                fixData("tac_lac") = value And &HFFFF
                fixData("ci") = (value >> 4) And &HFFFFFFFF
            End If

            deviceDataIndex += paramLength
        End While

        data("fixData") = fixData
        Return Newtonsoft.Json.JsonConvert.SerializeObject(data)
    End Function

    Function ParseScanDevices(deviceDataIndex As Integer, deviceDataLength As Integer, deviceDataArray As List(Of String), data As Dictionary(Of String, Object)) As String
        Dim deviceArray As New List(Of Dictionary(Of String, Object))()
        Dim deviceItem As New Dictionary(Of String, Object)()

        Dim doPost As Boolean = False

        While deviceDataIndex < deviceDataLength
            Dim paramTag As Integer = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
            deviceDataIndex += 1
            'log.Debug("paramTag:" & paramTag)

            Select Case paramTag
                Case 0
                    If deviceItem.Count <> 0 Then
                        deviceArray.Add(deviceItem)
                    End If
                    deviceItem = New Dictionary(Of String, Object)()
                    deviceDataIndex += 2
                    deviceItem("typeCode") = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
                    deviceItem("type") = typeCodeArray(deviceItem("typeCode"))
                    deviceDataIndex += 1

                Case &H1
                    deviceDataIndex += 2
                    deviceItem("mac") = String.Join("", deviceDataArray.Skip(deviceDataIndex).Take(6))
                    deviceDataIndex += 6

                Case &H2
                    deviceDataIndex += 2
                    deviceItem("connectable") = If(Convert.ToInt32(deviceDataArray(deviceDataIndex), 16) = 0, "Unconnectable", "Connectable")
                    deviceDataIndex += 1

                Case &H3
                    deviceDataIndex += 2
                    deviceItem("timestamp") = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(4).ToArray())
                    deviceDataIndex += 4
                    deviceItem("timezone") = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16)
                    deviceDataIndex += 1
                    deviceItem("current_time") = ParseTime(deviceItem("timestamp"), deviceItem("timezone"))

                Case &H4
                    deviceDataIndex += 2
                    deviceItem("rssi") = Convert.ToInt32(deviceDataArray(deviceDataIndex), 16) - 256
                    deviceDataIndex += 1

                Case &H5
                    Dim paramLength As Integer = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray())
                    deviceDataIndex += 2
                    deviceItem("advPacket") = String.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength))
                    deviceDataIndex += paramLength

                Case &H6
                    Dim paramLength = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray())
                    deviceDataIndex += 2
                    deviceItem("responsePacket") = String.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength))
                    deviceDataIndex += paramLength

                Case Else
                    If paramTag >= &HA Then
                        Dim paramLength = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray())
                        deviceDataIndex += 2
                        'log.Debug("paramLength:" & paramLength & "/deviceItem:" & deviceItem("typeCode"))
                        Select Case deviceItem("typeCode")
                            Case 0
                                ParseIbeacon(deviceItem, paramTag, deviceDataArray, deviceDataIndex, paramLength)
                            Case 1
                                ParseEddystoneUID(deviceItem, paramTag, deviceDataArray, deviceDataIndex, paramLength)
                            Case 2
                                ParseEddystoneURL(deviceItem, paramTag, deviceDataArray, deviceDataIndex, paramLength)
                            Case 3
                                ParseEddystoneTLM(deviceItem, paramTag, deviceDataArray, deviceDataIndex, paramLength)
                            Case 4
                                ParseDeviceInfo(deviceItem, paramTag, deviceDataArray, deviceDataIndex, paramLength)
                            Case 5
                                ParseBXPACC(deviceItem, paramTag, deviceDataArray, deviceDataIndex, paramLength)
                            Case 6
                                ParseBXPTH(deviceItem, paramTag, deviceDataArray, deviceDataIndex, paramLength)
                            Case 7
                                ParseBXPButton(deviceItem, paramTag, deviceDataArray, deviceDataIndex, paramLength)
                            Case 8
                                ParseBXPTag(deviceItem, paramTag, deviceDataArray, deviceDataIndex, paramLength)
                            Case 9
                                ParsePIR(deviceItem, paramTag, deviceDataArray, deviceDataIndex, paramLength)
                            Case 11
                                ParseTOF(deviceItem, paramTag, deviceDataArray, deviceDataIndex, paramLength)
                                doPost = True
                            Case 10
                                ParseOther(deviceItem, paramTag, deviceDataArray, deviceDataIndex, paramLength)
                        End Select

                        deviceDataIndex += paramLength
                    End If
            End Select
        End While

        If deviceItem.Count <> 0 Then
            deviceArray.Add(deviceItem)
        End If

        data("deviceArray") = deviceArray
        If doPost Then
            log.Debug(Newtonsoft.Json.JsonConvert.SerializeObject(data))
        End If
        Return Newtonsoft.Json.JsonConvert.SerializeObject(data)
    End Function

End Module
