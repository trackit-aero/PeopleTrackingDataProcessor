
Public Class DevScanResult
    Public Property flag As String
    Public Property gatewayMac As String
    Public Property length As Integer
    Public Property deviceArray As List(Of Devicearray)
End Class

Public Class Rootobject
    Public Property typeCode As Integer
    Public Property type As String
    Public Property mac As String
    Public Property connectable As String
    Public Property timestamp As Integer
    Public Property timezone As Integer
    Public Property current_time As Date
    Public Property rssi As Integer
    Public Property advPacket As String
    Public Property responsePacket As String
    Public Property manufacturerVendorCode As Integer
    Public Property battVoltage As String
    Public Property userData As Integer
    Public Property randingDistance As Integer
End Class

Public Class Devicearray
    Public Property typeCode As Integer
    Public Property type As String
    Public Property mac As String
    Public Property connectable As String
    Public Property timestamp As Integer
    Public Property timezone As Integer
    Public Property current_time As Date
    Public Property rssi As Integer
    Public Property txPower As Integer
    Public Property rangingData As Integer
    Public Property advInterval As String
    Public Property battVoltage As String
    Public Property passwordVerificateStatus As String
    Public Property ambientLightSensorStatus As String
    Public Property hallDoorSensorStatus As String
    Public Property ambientLightStatus As String
    Public Property doorStatus As String
    Public Property firmwareVersion As String
    Public Property deviceName As String
    Public Property advPacket As String
    Public Property responsePacket As String
    Public Property manufacturerVendorCode As Integer
    Public Property userData As Integer
    Public Property randingDistance As Integer
End Class

Public Class DevStatusResult
    Public Property flag As String
    Public Property gatewayMac As String
    Public Property length As Integer
    Public Property deviceStatus As Devicestatus
End Class

'Public Class Devicestatus
'    Public Property timestamp As Integer
'    Public Property netwrokType As String
'    Public Property csq As Integer
'    Public Property battVoltage As String
'    Public Property axisDataX As String
'    Public Property axisDataY As String
'    Public Property axisDataZ As String
'    Public Property accStatus As Integer
'    Public Property imei As String
'End Class
