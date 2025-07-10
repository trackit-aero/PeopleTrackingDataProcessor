// Converted from clsMoko.vb
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using QuickType;

public partial class MokoPacket
{
    public string flag { get; set; }
    public string gatewayMac { get; set; }
    public int length { get; set; }
    public List<Devicearray> deviceArray { get; set; }
}
public class Devicearray
{
    public int typeCode { get; set; }
    public string type { get; set; }
    public string mac { get; set; }
    public string connectable { get; set; }
    public int timestamp { get; set; }
    public int timezone { get; set; }
    public DateTime current_time { get; set; }
    public int rssi { get; set; }
    public int txPower { get; set; }
    public int rangingData { get; set; }
    public string advInterval { get; set; }
    public string battVoltage { get; set; }
    public string passwordVerificateStatus { get; set; }
    public string ambientLightSensorStatus { get; set; }
    public string hallDoorSensorStatus { get; set; }
    public string ambientLightStatus { get; set; }
    public string doorStatus { get; set; }
    public string firmwareVersion { get; set; }
    public string deviceName { get; set; }
    public string advPacket { get; set; }
    public string responsePacket { get; set; }
    public int manufacturerVendorCode { get; set; }
    public int userData { get; set; }
    public int randingDistance { get; set; }
}

public partial class MokoPacket
{
    public static MokoPacket FromJson(string json)
    {
        return JsonConvert.DeserializeObject<MokoPacket>(json, Converter.Settings);

    }
}


