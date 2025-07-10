using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static partial class mdMoko
{
    // Public ReadOnly log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)

    public static string[] typeCodeArray = new[] { "ibeacon", "eddystone-uid", "eddystone-url", "eddystone-tlm", "bxp-devifo", "bxp-acc", "bxp-th", "bxp-button", "bxp-tag", "pir", "other", "tof" };
    public static string[] samplingRateArray = new[] { "1hz", "10hz", "25hz", "50hz", "100hz" };
    public static string[] fullScaleArray = new[] { "2g", "4g", "8g", "16g" };
    public static string[] frameTypeArray = new[] { "Single press mode", "Double press mode", "Long press mode", "Abnormal" };
    public static string[] pirDelayResponseStatusArray = new[] { "Low", "Medium", "High" };
    public static string[] fixModeNotifyArray = new[] { "Periodic", "Motion", "Downlink" };
    public static string[] fixResultArray = new[] { "GPS fix success", "LBS fix success", "Interrupted by Downlink", "GPS serial port is used", "GPS aiding timeout", "GPS timeout", "PDOP limit", "LBS failure" };
    public static string[] lowPowerArray = new[] { "10%", "20%", "30%", "40%", "50%" };
    public static string[] scannerReportArray = new[] { "Scanner off", "Always scan", "Always scan periodic report", "Periodic scan immediate report", "Periodic scan periodic report" };
    public static string[] pirDelayRespStatusArray = new[] { "Low delay", "Medium delay", "High delay", "All type" };
    public static string[] pirDoorStatusArray = new[] { "Close", "Open", "All type" };
    public static string[] pirSensorSensitivityArray = new[] { "Low sensitivity", "Medium sensitivity", "High sensitivity", "All type" };
    public static string[] pirSensorDetactionStatusArray = new[] { "No effective motion detected", "Effective motion detected", "All type" };
    public static string[] otherRelationArray = new[] { "A", "A&B", "A|B", "A&B&C", "(A&B)|C", "A|B|C" };
    public static string[] filterDuplicateDataRuleArray = new[] { "None", "MAC", "MAC+Data type", "MAC+RAW Data" };
    public static string[] fixModeArray = new[] { "OFF", "Periodic fix", "Motion fix" };

    // Converted version of HandlePayload from VB.NET to C#
    public static string HandlePayload(string value, string msgType, int index)
    {
        value = value.ToLower();
        List<string> hexStrArray = mdMoko.ToHexStrArray(value);
        int len = hexStrArray.Count;
        if (len >= 11)
        {
            var data = new Dictionary<string, object>();
            data["flag"] = string.Join("", hexStrArray.GetRange(1, 2));
            data["gatewayMac"] = string.Join("", hexStrArray.GetRange(3, 6));
            data["length"] = mdMoko.ParseHexStrArrayToInt(hexStrArray.GetRange(9, 2).ToArray());

            List<string> deviceDataArray = hexStrArray.GetRange(11, len - 11);
            int deviceDataIndex = 0;
            int deviceDataLength = deviceDataArray.Count;

            switch (data["flag"]?.ToString())
            {
                case "2003": return ParseDevInfo(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "3004": return ParseDevStatus(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "3006": return ParseOTAResult(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2007": return ParseNTPSettings(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2008": return ParseTime(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2009": return ParseCommureTimeout(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "200a": return ParseIndicator(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "200b": return ParseUpdateFileStatus(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "200c": return ParseReportSettings(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "200d": return ParsePowerOffNotify(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "200e": return ParseBleConnectPassword(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "200f": return ParsePasswordVerify(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "3010": return ParsePowerOffAlarm(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "3011": return ParseLowPowerAlarm(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2012": return ParseLowPower(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2013": return ParseBattery(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2015": return ParsePowerOn(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2020": return ParseNetworkSettings(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2021": return ParseConnTimeout(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2030": return ParseMqttSettings(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "3032": return ParseMqttCertResult(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2040": return ParseScannerReportMode(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2041": return ParseAlwaysScan(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2042": return ParsePeriodicScanImmediateReport(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2043": return ParsePeriodicScanPeriodicReport(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2050": return ParseFilterRelationship(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2051": return ParseFilterRssi(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2052": return ParseFilterPhy(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2053": return ParseFilterMac(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2054": return ParseFilterName(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2055": return ParseFilterRawdata(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2056": return ParseFilterIbeacon(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2057": return ParseFilterEddystoneUID(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2058": return ParseFilterEddystoneURL(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2059": return ParseFilterEddystoneTLM(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "205a": return ParseFilterBXPDeviceInfo(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "205b": return ParseFilterBXPACC(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "205c": return ParseFilterBXPTH(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "205d": return ParseFilterBXPButton(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "205e": return ParseFilterBXPTag(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "205f": return ParseFilterPIR(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2060": return ParseFilterTOF(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2061": return ParseFilterOther(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2062": return ParseFilterDuplicateData(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2070": return ParseAdvSettings(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2071": return ParseIbeaconSettings(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2080": return ParseFixMode(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2081": return ParsePeriodicFix(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2082": return ParseAxisParams(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2083": return ParseMotionStartParams(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2084": return ParseMotionInTripParams(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2085": return ParseMotionStopParams(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2086": return ParseStationaryParams(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2087": return ParseGPSParams(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2090": return ParseIbeaconPayload(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2091": return ParseEddystoneUIDPayload(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2092": return ParseEddystoneURLPayload(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2093": return ParseEddystoneTLMPayload(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2094": return ParseBXPDevInfoPayload(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2095": return ParseBXPAccPayload(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2096": return ParseBXPTHPayload(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2097": return ParseBXPButtonPayload(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2098": return ParseBXPTagPayload(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "2099": return ParsePIRPayload(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "209a": return ParseTOFPayload(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "209b": return ParseOtherPayload(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "3089":
                case "30b1": return ParseFixData(deviceDataIndex, deviceDataLength, deviceDataArray, data);
                case "30a0":
                case "30b2": return ParseScanDevices(deviceDataIndex, deviceDataLength, deviceDataArray, data);
            }
        }
        return value;
    }


    public static void ParseIbeacon(ref Dictionary<string, object> deviceItem, int paramTag, List<string> deviceDataArray, int deviceDataIndex, int paramLength)
    {
        if (paramTag == 0xA)
        {
            deviceItem["uuid"] = string.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength));
        }
        else if (paramTag == 0xB)
        {
            deviceItem["major"] = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
        }
        else if (paramTag == 0xC)
        {
            deviceItem["minor"] = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
        }
        else if (paramTag == 0xD)
        {
            deviceItem["rssi_1m"] = SignedHexToInt(deviceDataArray[deviceDataIndex]);
        }
    }

    // Function to parse Eddystone UID data
    public static void ParseEddystoneUID(ref Dictionary<string, object> deviceItem, int paramTag, List<string> deviceDataArray, int deviceDataIndex, int paramLength)
    {
        if (paramTag == 0xA)
        {
            deviceItem["namespace"] = string.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength));
        }
        else if (paramTag == 0xB)
        {
            deviceItem["rssi_0m"] = SignedHexToInt(deviceDataArray[deviceDataIndex]);
        }
        else if (paramTag == 0xC)
        {
            deviceItem["instance"] = string.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength));
        }
    }

    // Function to parse Eddystone URL data
    public static void ParseEddystoneURL(ref Dictionary<string, object> deviceItem, int paramTag, List<string> deviceDataArray, int deviceDataIndex, int paramLength)
    {
        if (paramTag == 0xA)
        {
            deviceItem["url"] = HexStrToString(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
        }
        else if (paramTag == 0xB)
        {
            deviceItem["rssi_0m"] = SignedHexToInt(deviceDataArray[deviceDataIndex]);
        }
    }

    // Function to parse Eddystone TLM data
    public static void ParseEddystoneTLM(ref Dictionary<string, object> deviceItem, int paramTag, List<string> deviceDataArray, int deviceDataIndex, int paramLength)
    {
        if (paramTag == 0xA)
        {
            deviceItem["tlmVersion"] = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
        }
        else if (paramTag == 0xB)
        {
            deviceItem["battVoltage"] = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray()) + "mV";
        }
        else if (paramTag == 0xC)
        {
            deviceItem["temperature"] = (SignedHexToInt(string.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength))) / 256d).ToString("F1") + "℃";
        }
        else if (paramTag == 0xD)
        {
            deviceItem["advCnt"] = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
        }
        else if (paramTag == 0xE)
        {
            deviceItem["escCnt"] = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
        }
    }

    // Function to parse device info data
    public static void ParseDeviceInfo(ref Dictionary<string, object> deviceItem, int paramTag, List<string> deviceDataArray, int deviceDataIndex, int paramLength)
    {
        if (paramTag == 0xA)
        {
            deviceItem["txPower"] = SignedHexToInt(deviceDataArray[deviceDataIndex]);
        }
        else if (paramTag == 0xB)
        {
            deviceItem["rangingData"] = SignedHexToInt(deviceDataArray[deviceDataIndex]);
        }
        else if (paramTag == 0xC)
        {
            deviceItem["advInterval"] = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16) * 100 + "ms";
        }
        else if (paramTag == 0xD)
        {
            deviceItem["battVoltage"] = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray()) + "mV";
        }
        else if (paramTag == 0xE)
        {
            int status = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
            deviceItem["passwordVerificateStatus"] = (status & 0x3) == 0 ? "Enable" : "Disable";
            deviceItem["ambientLightSensorStatus"] = (status >> 2 & 1) == 0 ? "Enable" : "Disable";
            deviceItem["hallDoorSensorStatus"] = (status >> 3 & 1) == 0 ? "Enable" : "Disable";
        }
        else if (paramTag == 0xF)
        {
            int status = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
            deviceItem["connectable"] = (status & 1) == 0 ? "Enable" : "Disable";
            deviceItem["ambientLightStatus"] = (status >> 1 & 1) == 0 ? "Enable" : "Disable";
            deviceItem["doorStatus"] = (status >> 2 & 1) == 0 ? "Enable" : "Disable";
        }
        else if (paramTag == 0x10)
        {
            deviceItem["firmwareVersion"] = HexStrToString(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
        }
        else if (paramTag == 0x11)
        {
            deviceItem["deviceName"] = HexStrToString(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
        }
    }

    // Function to parse BXP ACC data
    public static void ParseBXPACC(ref Dictionary<string, object> deviceItem, int paramTag, List<string> deviceDataArray, int deviceDataIndex, int paramLength)
    {
        if (paramTag == 0xA)
        {
            deviceItem["txPower"] = SignedHexToInt(deviceDataArray[deviceDataIndex]);
        }
        else if (paramTag == 0xB)
        {
            deviceItem["rangingData"] = SignedHexToInt(deviceDataArray[deviceDataIndex]);
        }
        else if (paramTag == 0xC)
        {
            deviceItem["advInterval"] = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16) * 100 + "ms";
        }
        else if (paramTag == 0xD)
        {
            deviceItem["samplingRate"] = samplingRateArray[Convert.ToInt32(deviceDataArray[deviceDataIndex], 16)];
        }
        else if (paramTag == 0xE)
        {
            deviceItem["fullScale"] = fullScaleArray[Convert.ToInt32(deviceDataArray[deviceDataIndex], 16)];
        }
        else if (paramTag == 0xF)
        {
            deviceItem["motionThreshold"] = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16) * 0.1d + "g";
        }
        else if (paramTag == 0x10)
        {
            string[] axisDataArray = deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray();
            deviceItem["axisDataX"] = axisDataArray[0] + "mg";
            deviceItem["axisDataY"] = axisDataArray[1] + "mg";
            deviceItem["axisDataZ"] = axisDataArray[2] + "mg";
        }
        else if (paramTag == 0x11)
        {
            deviceItem["battVoltage"] = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray()) + "mV";
        }
    }

    // Function to parse BXP TH data
    public static void ParseBXPTH(ref Dictionary<string, object> deviceItem, int paramTag, List<string> deviceDataArray, int deviceDataIndex, int paramLength)
    {
        if (paramTag == 0xA)
        {
            deviceItem["txPower"] = SignedHexToInt(deviceDataArray[deviceDataIndex]);
        }
        else if (paramTag == 0xB)
        {
            deviceItem["rangingData"] = SignedHexToInt(deviceDataArray[deviceDataIndex]);
        }
        else if (paramTag == 0xC)
        {
            deviceItem["advInterval"] = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16) * 100 + "ms";
        }
        else if (paramTag == 0xD)
        {
            deviceItem["temperature"] = (SignedHexToInt(string.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength))) * 0.1d).ToString("F1") + "℃";
        }
        else if (paramTag == 0xE)
        {
            deviceItem["humidity"] = (SignedHexToInt(string.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength))) * 0.1d).ToString("F1") + "%";
        }
        else if (paramTag == 0xF)
        {
            deviceItem["battVoltage"] = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray()) + "mV";
        }
    }

    // Function to parse BXP Button data
    public static void ParseBXPButton(ref Dictionary<string, object> deviceItem, int paramTag, List<string> deviceDataArray, int deviceDataIndex, int paramLength)
    {
        if (paramTag == 0xA)
        {
            deviceItem["frameType"] = frameTypeArray[Convert.ToInt32(deviceDataArray[deviceDataIndex], 16) & 0xF];
        }
        else if (paramTag == 0xB)
        {
            int status = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
            deviceItem["passwordVerifyFlag"] = status & 1;
            deviceItem["triggerStatus"] = status >> 1 & 1;
        }
        else if (paramTag == 0xC)
        {
            deviceItem["triggerCount"] = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
        }
        else if (paramTag == 0xD)
        {
            deviceItem["devId"] = string.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength));
        }
        else if (paramTag == 0xE)
        {
            deviceItem["firmwareType"] = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
        }
        else if (paramTag == 0xF)
        {
            deviceItem["devName"] = HexStrToString(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
        }
        else if (paramTag == 0x10)
        {
            deviceItem["fullScale"] = fullScaleArray[Convert.ToInt32(deviceDataArray[deviceDataIndex], 16)];
        }
        else if (paramTag == 0x11)
        {
            deviceItem["motionThreshold"] = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16) * 0.1d + "g";
        }
        else if (paramTag == 0x12)
        {
            string[] axisDataArray = deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray();
            deviceItem["axisDataX"] = axisDataArray[0] + "mg";
            deviceItem["axisDataY"] = axisDataArray[1] + "mg";
            deviceItem["axisDataZ"] = axisDataArray[2] + "mg";
        }
        else if (paramTag == 0x13)
        {
            deviceItem["temperature"] = (SignedHexToInt(string.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength))) * 0.25d).ToString("F1") + "℃";
        }
        else if (paramTag == 0x14)
        {
            deviceItem["rangingData"] = SignedHexToInt(deviceDataArray[deviceDataIndex]);
        }
        else if (paramTag == 0x15)
        {
            deviceItem["battVoltage"] = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray()) + "mV";
        }
        else if (paramTag == 0x16)
        {
            deviceItem["txPower"] = SignedHexToInt(deviceDataArray[deviceDataIndex]);
        }
    }

    // Function to parse BXP Tag data
    public static void ParseBXPTag(ref Dictionary<string, object> deviceItem, int paramTag, List<string> deviceDataArray, int deviceDataIndex, int paramLength)
    {
        if (paramTag == 0xA)
        {
            int status = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
            deviceItem["hallSensorStatus"] = (status & 1) == 1 ? "Enable" : "Disable";
            deviceItem["axisStatus"] = (status >> 1 & 1) == 1 ? "Enable" : "Disable";
            deviceItem["axisEquippedStatus"] = (status >> 2 & 1) == 1 ? "Enable" : "Disable";
        }
        else if (paramTag == 0xB)
        {
            deviceItem["hallTriggerEventCount"] = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
        }
        else if (paramTag == 0xC)
        {
            deviceItem["motionTriggerEventCount"] = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
        }
        else if (paramTag == 0xD)
        {
            string[] axisDataArray = deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray();
            deviceItem["axisDataX"] = axisDataArray[0] + "mg";
            deviceItem["axisDataY"] = axisDataArray[1] + "mg";
            deviceItem["axisDataZ"] = axisDataArray[2] + "mg";
        }
        else if (paramTag == 0xE)
        {
            deviceItem["battVoltage"] = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray()) + "mV";
        }
        else if (paramTag == 0xF)
        {
            deviceItem["tagId"] = string.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength));
        }
        else if (paramTag == 0x10)
        {
            deviceItem["devName"] = HexStrToString(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
        }
    }

    // Function to parse PIR data
    public static void ParsePIR(ref Dictionary<string, object> deviceItem, int paramTag, List<string> deviceDataArray, int deviceDataIndex, int paramLength)
    {
        if (paramTag == 0xA)
        {
            deviceItem["pirDelayResponseStatus"] = pirDelayResponseStatusArray[Convert.ToInt32(deviceDataArray[deviceDataIndex], 16)];
        }
        else if (paramTag == 0xB)
        {
            deviceItem["doorStatus"] = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16) == 0 ? "open" : "close";
        }
        else if (paramTag == 0xC)
        {
            deviceItem["sensorSensitivity"] = pirDelayResponseStatusArray[Convert.ToInt32(deviceDataArray[deviceDataIndex], 16)];
        }
        else if (paramTag == 0xD)
        {
            deviceItem["sensorDetectionStatus"] = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16) == 0 ? "no effective motion" : "effective motion";
        }
        else if (paramTag == 0xE)
        {
            deviceItem["battVoltage"] = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray()) + "mV";
        }
        else if (paramTag == 0xF)
        {
            deviceItem["major"] = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
        }
        else if (paramTag == 0x10)
        {
            deviceItem["minor"] = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
        }
        else if (paramTag == 0x11)
        {
            deviceItem["rssi_1m"] = SignedHexToInt(deviceDataArray[deviceDataIndex]);
        }
        else if (paramTag == 0x12)
        {
            deviceItem["txPower"] = SignedHexToInt(deviceDataArray[deviceDataIndex]);
        }
        else if (paramTag == 0x13)
        {
            deviceItem["devName"] = HexStrToString(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
        }
    }

    // Function to parse TOF data
    public static void ParseTOF(ref Dictionary<string, object> deviceItem, int paramTag, List<string> deviceDataArray, int deviceDataIndex, int paramLength)
    {
        // log.Debug("ParseTOF")
        if (paramTag == 0xA)
        {
            deviceItem["manufacturerVendorCode"] = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
        }
        else if (paramTag == 0xB)
        {
            deviceItem["battVoltage"] = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray()) + "mV";
        }
        else if (paramTag == 0xC)
        {
            deviceItem["userData"] = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
        }
        else if (paramTag == 0xD)
        {
            deviceItem["randingDistance"] = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
        }
    }

    // Function to parse Other data
    public static void ParseOther(ref Dictionary<string, object> deviceItem, int paramTag, List<string> deviceDataArray, int deviceDataIndex, int paramLength)
    {
        if (paramTag == 0xA)
        {
            deviceItem["dataBlock1"] = string.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength));
        }
        else if (paramTag == 0xB)
        {
            deviceItem["dataBlock2"] = string.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength));
        }
        else if (paramTag == 0xC)
        {
            deviceItem["dataBlock3"] = string.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength));
        }
        else if (paramTag == 0xD)
        {
            deviceItem["dataBlock4"] = string.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength));
        }
        else if (paramTag == 0xE)
        {
            deviceItem["dataBlock5"] = string.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength));
        }
        else if (paramTag == 0xF)
        {
            deviceItem["dataBlock6"] = string.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength));
        }
        else if (paramTag == 0x10)
        {
            deviceItem["dataBlock7"] = string.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength));
        }
        else if (paramTag == 0x11)
        {
            deviceItem["dataBlock8"] = string.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength));
        }
        else if (paramTag == 0x12)
        {
            deviceItem["dataBlock9"] = string.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength));
        }
        else if (paramTag == 0x13)
        {
            deviceItem["dataBlock10"] = string.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength));
        }
    }
    public static int SignedHexToInt(string hexStr)
    {
        int value = Convert.ToInt32(hexStr, 16);
        if ((value & 0x80) != 0)
        {
            value -= 0x100;
        }
        return value;
    }


    public static string HexStrToString(string[] hexArray)
    {
        var sb = new StringBuilder();
        foreach (string hex in hexArray)
            sb.Append(Convert.ToChar(Convert.ToUInt32(hex, 16)));
        return sb.ToString();
    }

    public static int ParseHexStrArrayToInt(string[] hexArray)
    {
        return Convert.ToInt32(string.Join("", hexArray), 16);
    }

    public static DateTime ParseTime(int timestamp, int timezone)
    {
        // Unix epoch start time
        DateTimeOffset epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        // Add seconds to epoch
        DateTimeOffset utcTime = epoch.AddSeconds(timestamp);

        // Apply the timezone offset
        DateTimeOffset targetTime = utcTime.ToOffset(TimeSpan.FromHours(timezone));

        return targetTime.DateTime;
    }
    public static List<string> ToHexStrArray(string value)
    {
        string rep = value.Replace(" ", "");
        var array = new List<string>();
        int arrLen = rep.Length / 2;
        for (int i = 0, loopTo = arrLen - 1; i <= loopTo; i++)
            array.Add(rep.Substring(i * 2, 2));
        return array;
    }

    public static int ParseHexStrArraytoInt(List<string> hexStrArray)
    {
        return Convert.ToInt32(string.Join("", hexStrArray), 16);
    }

    public static string HexStrToString(List<string> value)
    {
        var chars = new List<char>();
        int arrLen = value.Count;
        for (int i = 0; i < arrLen; i++)
        {
            chars.Add((char)Convert.ToInt32(value[i], 16));
        }
        return new string(chars.ToArray());
    }

    public static string ParseDevInfo(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        var deviceInfo = new Dictionary<string, string>();

        while (deviceDataIndex < deviceDataLength)
        {
            int paramTag = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
            deviceDataIndex += 1;

            int paramLength = ParseHexStrArraytoInt(deviceDataArray.GetRange(deviceDataIndex, 2));
            deviceDataIndex += 2;

            switch (paramTag)
            {
                case 0:
                    {
                        deviceInfo["deviceName"] = HexStrToString(deviceDataArray.GetRange(deviceDataIndex, paramLength));
                        break;
                    }
                case 1:
                    {
                        deviceInfo["productModel"] = HexStrToString(deviceDataArray.GetRange(deviceDataIndex, paramLength));
                        break;
                    }
                case 2:
                    {
                        deviceInfo["companyName"] = HexStrToString(deviceDataArray.GetRange(deviceDataIndex, paramLength));
                        break;
                    }
                case 3:
                    {
                        deviceInfo["hardwareVersion"] = HexStrToString(deviceDataArray.GetRange(deviceDataIndex, paramLength));
                        break;
                    }
                case 4:
                    {
                        deviceInfo["softwareVersion"] = HexStrToString(deviceDataArray.GetRange(deviceDataIndex, paramLength));
                        break;
                    }
                case 5:
                    {
                        deviceInfo["firmwareVersion"] = HexStrToString(deviceDataArray.GetRange(deviceDataIndex, paramLength));
                        break;
                    }
                case 6:
                    {
                        deviceInfo["imei"] = HexStrToString(deviceDataArray.GetRange(deviceDataIndex, paramLength));
                        break;
                    }
                case 7:
                    {
                        deviceInfo["iccid"] = HexStrToString(deviceDataArray.GetRange(deviceDataIndex, paramLength));
                        break;
                    }
            }

            deviceDataIndex += paramLength;
        }

        data["deviceInfo"] = deviceInfo;
        return Newtonsoft.Json.JsonConvert.SerializeObject(data);
    }

    public static string ParseDevStatus(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        var deviceStatus = new Dictionary<string, object>();

        while (deviceDataIndex < deviceDataLength)
        {
            int paramTag = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
            deviceDataIndex += 1;
            int paramLength = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray());
            deviceDataIndex += 2;

            switch (paramTag)
            {
                case 0:
                    {
                        deviceStatus["timestamp"] = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
                        break;
                    }
                case 1:
                    {
                        deviceStatus["netwrokType"] = HexStrToString(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
                        break;
                    }
                case 2:
                    {
                        deviceStatus["csq"] = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
                        break;
                    }
                case 3:
                    {
                        deviceStatus["battVoltage"] = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray()) + "mV";
                        break;
                    }
                case 4:
                    {
                        string[] axisDataArray = deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray();
                        deviceStatus["axisDataX"] = axisDataArray[0] + "mg";
                        deviceStatus["axisDataY"] = axisDataArray[1] + "mg";
                        deviceStatus["axisDataZ"] = axisDataArray[2] + "mg";
                        break;
                    }
                case 5:
                    {
                        deviceStatus["accStatus"] = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
                        break;
                    }
                case 6:
                    {
                        deviceStatus["imei"] = HexStrToString(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
                        break;
                    }
            }

            deviceDataIndex += paramLength;
        }

        data["deviceStatus"] = deviceStatus;
        return Newtonsoft.Json.JsonConvert.SerializeObject(data);
    }
    public static string ParseTOFPayload(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        var tofPayload = new Dictionary<string, object>();

        while (deviceDataIndex < deviceDataLength)
        {
            int paramTag = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
            deviceDataIndex += 1;
            int paramLength = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray());
            deviceDataIndex += 2;

            if (paramTag == 0)
            {
                int value = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
                tofPayload["rssi"] = value & 1;
                tofPayload["timestamp"] = value >> 1 & 1;
                tofPayload["manufacturerVendorCode"] = value >> 2 & 1;
                tofPayload["battVoltage"] = value >> 3 & 1;
                tofPayload["userData"] = value >> 4 & 1;
                tofPayload["randingDistance"] = value >> 5 & 1;
                tofPayload["rawAdv"] = value >> 6 & 1;
                tofPayload["rawResp"] = value >> 7 & 1;
            }

            deviceDataIndex += paramLength;
        }

        data["tofPayload"] = tofPayload;
        return Newtonsoft.Json.JsonConvert.SerializeObject(data);
    }
    public static string ParseOTAResult(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseNTPSettings(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        return default;
        // Your implementation here
    }

    public static string ParseTime(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;

    }

    public static string ParseCommureTimeout(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseIndicator(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseUpdateFileStatus(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseReportSettings(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParsePowerOffNotify(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseBleConnectPassword(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParsePasswordVerify(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParsePowerOffAlarm(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        var powerOffAlarm = new Dictionary<string, object>();

        while (deviceDataIndex < deviceDataLength)
        {
            int paramTag = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
            deviceDataIndex += 1;

            int paramLength = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray());
            deviceDataIndex += 2;

            if (paramTag == 0)
            {
                int timestamp = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
                powerOffAlarm["timestamp"] = timestamp;
                powerOffAlarm["timeStr"] = ParseTime(timestamp, 0);
            }
            else if (paramTag == 1)
            {
                int battVoltage = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
                powerOffAlarm["battVoltage"] = battVoltage + "mV";
            }

            deviceDataIndex += paramLength;
        }

        data["powerOffAlarm"] = powerOffAlarm;
        return Newtonsoft.Json.JsonConvert.SerializeObject(data);
    }

    public static string ParseLowPowerAlarm(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        var lowPowerAlarm = new Dictionary<string, object>();

        while (deviceDataIndex < deviceDataLength)
        {
            int paramTag = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
            deviceDataIndex += 1;

            int paramLength = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray());
            deviceDataIndex += 2;

            if (paramTag == 0)
            {
                int timestamp = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
                lowPowerAlarm["timestamp"] = timestamp;
                lowPowerAlarm["timeStr"] = ParseTime(timestamp, 0);
            }
            else if (paramTag == 1)
            {
                int battVoltage = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
                lowPowerAlarm["battVoltage"] = battVoltage + "mV";
            }

            deviceDataIndex += paramLength;
        }

        data["lowPowerAlarm"] = lowPowerAlarm;
        return Newtonsoft.Json.JsonConvert.SerializeObject(data);
    }



    public static string ParseLowPower(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        var lowPower = new Dictionary<string, object>();

        while (deviceDataIndex < deviceDataLength)
        {
            int paramTag = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
            deviceDataIndex += 1;
            int paramLength = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray());
            deviceDataIndex += 2;

            if (paramTag == 0)
            {
                lowPower["enable"] = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
            }
            else if (paramTag == 1)
            {
                lowPower["percentage"] = lowPowerArray[Convert.ToInt32(deviceDataArray[deviceDataIndex], 16)];
            }

            deviceDataIndex += paramLength;
        }

        data["lowPower"] = lowPower;
        return Newtonsoft.Json.JsonConvert.SerializeObject(data);
    }

    public static string ParseBattery(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        var battery = new Dictionary<string, object>();

        while (deviceDataIndex < deviceDataLength)
        {
            int paramTag = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
            deviceDataIndex += 1;
            int paramLength = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray());
            deviceDataIndex += 2;

            if (paramTag == 0)
            {
                battery["voltage"] = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray()) + "mV";
            }

            deviceDataIndex += paramLength;
        }

        data["battery"] = battery;
        return Newtonsoft.Json.JsonConvert.SerializeObject(data);
    }

    public static string ParsePowerOn(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        var powerOn = new Dictionary<string, object>();

        while (deviceDataIndex < deviceDataLength)
        {
            int paramTag = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
            deviceDataIndex += 1;
            int paramLength = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray());
            deviceDataIndex += 2;

            if (paramTag == 0)
            {
                powerOn["enable"] = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
            }

            deviceDataIndex += paramLength;
        }

        data["powerOn"] = powerOn;
        return Newtonsoft.Json.JsonConvert.SerializeObject(data);
    }

    public static string ParseNetworkSettings(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseConnTimeout(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseMqttSettings(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseMqttCertResult(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseScannerReportMode(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseAlwaysScan(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParsePeriodicScanImmediateReport(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParsePeriodicScanPeriodicReport(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseFilterRelationship(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseFilterRssi(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseFilterPhy(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseFilterMac(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseFilterName(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseFilterRawdata(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseFilterIbeacon(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseFilterEddystoneUID(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseFilterEddystoneURL(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseFilterEddystoneTLM(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseFilterBXPDeviceInfo(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        var filterBXPDeviceInfo = new Dictionary<string, int>();

        while (deviceDataIndex < deviceDataLength)
        {
            int paramTag = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
            deviceDataIndex += 1;

            int paramLength = ParseHexStrArraytoInt(deviceDataArray.GetRange(deviceDataIndex, 2));
            deviceDataIndex += 2;

            if (paramTag == 0)
            {
                filterBXPDeviceInfo["enable"] = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
            }

            deviceDataIndex += paramLength;
        }

        data["filterBXPDeviceInfo"] = filterBXPDeviceInfo;
        return Newtonsoft.Json.JsonConvert.SerializeObject(data);
    }


    public static string ParseFilterBXPACC(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseFilterBXPTH(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseFilterBXPButton(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseFilterBXPTag(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseFilterPIR(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseFilterTOF(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseFilterOther(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseFilterDuplicateData(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseAdvSettings(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseIbeaconSettings(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseFixMode(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParsePeriodicFix(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseAxisParams(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseMotionStartParams(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseMotionInTripParams(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseMotionStopParams(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }

    public static string ParseStationaryParams(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        // Your implementation here
        return default;
    }
    public static string ParseGPSParams(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        var gpsParams = new Dictionary<string, object>();

        while (deviceDataIndex < deviceDataLength)
        {
            int paramTag = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
            deviceDataIndex += 1;
            int paramLength = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray());
            deviceDataIndex += 2;

            if (paramTag == 0)
            {
                gpsParams["timeout"] = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
            }
            else if (paramTag == 1)
            {
                gpsParams["pdop"] = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
            }

            deviceDataIndex += paramLength;
        }

        data["gpsParams"] = gpsParams;
        return Newtonsoft.Json.JsonConvert.SerializeObject(data);
    }

    public static string ParseIbeaconPayload(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        var ibeaconPayload = new Dictionary<string, object>();

        while (deviceDataIndex < deviceDataLength)
        {
            int paramTag = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
            deviceDataIndex += 1;
            int paramLength = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray());
            deviceDataIndex += 2;

            if (paramTag == 0)
            {
                int value = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
                ibeaconPayload["rssi"] = value & 1;
                ibeaconPayload["timestamp"] = value >> 1 & 1;
                ibeaconPayload["uuid"] = value >> 2 & 1;
                ibeaconPayload["major"] = value >> 3 & 1;
                ibeaconPayload["minor"] = value >> 4 & 1;
                ibeaconPayload["rssi_1m"] = value >> 5 & 1;
                ibeaconPayload["rawAdv"] = value >> 6 & 1;
                ibeaconPayload["rawResp"] = value >> 7 & 1;
            }

            deviceDataIndex += paramLength;
        }

        data["ibeaconPayload"] = ibeaconPayload;
        return Newtonsoft.Json.JsonConvert.SerializeObject(data);
    }

    public static string ParseEddystoneUIDPayload(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        var eddystoneUIDPayload = new Dictionary<string, object>();

        while (deviceDataIndex < deviceDataLength)
        {
            int paramTag = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
            deviceDataIndex += 1;
            int paramLength = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray());
            deviceDataIndex += 2;

            if (paramTag == 0)
            {
                int value = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
                eddystoneUIDPayload["rssi"] = value & 1;
                eddystoneUIDPayload["timestamp"] = value >> 1 & 1;
                eddystoneUIDPayload["rssi_0m"] = value >> 2 & 1;
                eddystoneUIDPayload["namespace"] = value >> 3 & 1;
                eddystoneUIDPayload["instance"] = value >> 4 & 1;
                eddystoneUIDPayload["rawAdv"] = value >> 5 & 1;
                eddystoneUIDPayload["rawResp"] = value >> 6 & 1;
            }

            deviceDataIndex += paramLength;
        }

        data["eddystoneUIDPayload"] = eddystoneUIDPayload;
        return Newtonsoft.Json.JsonConvert.SerializeObject(data);
    }

    public static string ParseEddystoneURLPayload(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        var eddystoneURLPayload = new Dictionary<string, object>();

        while (deviceDataIndex < deviceDataLength)
        {
            int paramTag = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
            deviceDataIndex += 1;
            int paramLength = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray());
            deviceDataIndex += 2;

            if (paramTag == 0)
            {
                int value = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
                eddystoneURLPayload["rssi"] = value & 1;
                eddystoneURLPayload["timestamp"] = value >> 1 & 1;
                eddystoneURLPayload["rssi_0m"] = value >> 2 & 1;
                eddystoneURLPayload["url"] = value >> 3 & 1;
                eddystoneURLPayload["rawAdv"] = value >> 4 & 1;
                eddystoneURLPayload["rawResp"] = value >> 5 & 1;
            }

            deviceDataIndex += paramLength;
        }

        data["eddystoneURLPayload"] = eddystoneURLPayload;
        return Newtonsoft.Json.JsonConvert.SerializeObject(data);
    }

    public static string ParseEddystoneTLMPayload(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        var eddystoneTLMPayload = new Dictionary<string, object>();

        while (deviceDataIndex < deviceDataLength)
        {
            int paramTag = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
            deviceDataIndex += 1;
            int paramLength = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray());
            deviceDataIndex += 2;

            if (paramTag == 0)
            {
                int value = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
                eddystoneTLMPayload["rssi"] = value & 1;
                eddystoneTLMPayload["timestamp"] = value >> 1 & 1;
                eddystoneTLMPayload["tlmVersion"] = value >> 2 & 1;
                eddystoneTLMPayload["battVoltage"] = value >> 3 & 1;
                eddystoneTLMPayload["temperature"] = value >> 4 & 1;
                eddystoneTLMPayload["advCnt"] = value >> 5 & 1;
                eddystoneTLMPayload["secCnt"] = value >> 6 & 1;
                eddystoneTLMPayload["rawAdv"] = value >> 7 & 1;
                eddystoneTLMPayload["rawResp"] = value >> 8 & 1;
            }

            deviceDataIndex += paramLength;
        }

        data["eddystoneTLMPayload"] = eddystoneTLMPayload;
        return Newtonsoft.Json.JsonConvert.SerializeObject(data);
    }

    public static string ParseBXPDevInfoPayload(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        var bxpDevInfoPayload = new Dictionary<string, object>();

        while (deviceDataIndex < deviceDataLength)
        {
            int paramTag = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
            deviceDataIndex += 1;
            int paramLength = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray());
            deviceDataIndex += 2;

            if (paramTag == 0)
            {
                int value = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
                bxpDevInfoPayload["rssi"] = value & 1;
                bxpDevInfoPayload["timestamp"] = value >> 1 & 1;
                bxpDevInfoPayload["txPower"] = value >> 2 & 1;
                bxpDevInfoPayload["rangingData"] = value >> 3 & 1;
                bxpDevInfoPayload["advInterval"] = value >> 4 & 1;
                bxpDevInfoPayload["battVoltage"] = value >> 5 & 1;
                bxpDevInfoPayload["devicePropertyIndicator"] = value >> 6 & 1;
                bxpDevInfoPayload["switchStatusIndicator"] = value >> 7 & 1;
                bxpDevInfoPayload["firmwareVersion"] = value >> 8 & 1;
                bxpDevInfoPayload["deviceName"] = value >> 9 & 1;
                bxpDevInfoPayload["rawAdv"] = value >> 10 & 1;
                bxpDevInfoPayload["rawResp"] = value >> 11 & 1;
            }

            deviceDataIndex += paramLength;
        }

        data["bxpDevInfoPayload"] = bxpDevInfoPayload;
        return Newtonsoft.Json.JsonConvert.SerializeObject(data);
    }

    public static string ParseBXPAccPayload(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        var bxpAccPayload = new Dictionary<string, object>();

        while (deviceDataIndex < deviceDataLength)
        {
            int paramTag = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
            deviceDataIndex += 1;
            int paramLength = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray());
            deviceDataIndex += 2;

            if (paramTag == 0)
            {
                int value = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
                bxpAccPayload["rssi"] = value & 1;
                bxpAccPayload["timestamp"] = value >> 1 & 1;
                bxpAccPayload["txPower"] = value >> 2 & 1;
                bxpAccPayload["rangingData"] = value >> 3 & 1;
                bxpAccPayload["advInterval"] = value >> 4 & 1;
                bxpAccPayload["samplingRate"] = value >> 5 & 1;
                bxpAccPayload["fullScale"] = value >> 6 & 1;
                bxpAccPayload["motionThreshold"] = value >> 7 & 1;
                bxpAccPayload["axisData"] = value >> 8 & 1;
                bxpAccPayload["battVoltage"] = value >> 9 & 1;
                bxpAccPayload["rawAdv"] = value >> 10 & 1;
            }

            deviceDataIndex += paramLength;
        }

        data["bxpAccPayload"] = bxpAccPayload;
        return Newtonsoft.Json.JsonConvert.SerializeObject(data);
    }

    public static string ParseBXPTHPayload(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        var bxpTHPayload = new Dictionary<string, object>();

        while (deviceDataIndex < deviceDataLength)
        {
            int paramTag = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
            deviceDataIndex += 1;
            int paramLength = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray());
            deviceDataIndex += 2;

            if (paramTag == 0)
            {
                int value = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
                bxpTHPayload["rssi"] = value & 1;
                bxpTHPayload["timestamp"] = value >> 1 & 1;
                bxpTHPayload["txPower"] = value >> 2 & 1;
                bxpTHPayload["rangingData"] = value >> 3 & 1;
                bxpTHPayload["advInterval"] = value >> 4 & 1;
                bxpTHPayload["temperature"] = value >> 5 & 1;
                bxpTHPayload["humidity"] = value >> 6 & 1;
                bxpTHPayload["battVoltage"] = value >> 7 & 1;
                bxpTHPayload["rawAdv"] = value >> 8 & 1;
            }

            deviceDataIndex += paramLength;
        }

        data["bxpTHPayload"] = bxpTHPayload;
        return Newtonsoft.Json.JsonConvert.SerializeObject(data);
    }

    public static string ParseBXPButtonPayload(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        var bxpButtonPayload = new Dictionary<string, object>();

        while (deviceDataIndex < deviceDataLength)
        {
            int paramTag = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
            deviceDataIndex += 1;
            int paramLength = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray());
            deviceDataIndex += 2;

            if (paramTag == 0)
            {
                int value = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
                bxpButtonPayload["rssi"] = value & 1;
                bxpButtonPayload["timestamp"] = value >> 1 & 1;
                bxpButtonPayload["frameType"] = value >> 2 & 1;
                bxpButtonPayload["statusFlag"] = value >> 3 & 1;
                bxpButtonPayload["triggerCount"] = value >> 4 & 1;
                bxpButtonPayload["deviceId"] = value >> 5 & 1;
                bxpButtonPayload["firmwareType"] = value >> 6 & 1;
                bxpButtonPayload["deviceName"] = value >> 7 & 1;
                bxpButtonPayload["fullScale"] = value >> 8 & 1;
                bxpButtonPayload["motionThreshold"] = value >> 9 & 1;
                bxpButtonPayload["axisData"] = value >> 10 & 1;
                bxpButtonPayload["temperature"] = value >> 11 & 1;
                bxpButtonPayload["rangingData"] = value >> 12 & 1;
                bxpButtonPayload["battVoltage"] = value >> 13 & 1;
                bxpButtonPayload["txPower"] = value >> 14 & 1;
                bxpButtonPayload["rawAdv"] = value >> 15 & 1;
                bxpButtonPayload["rawResp"] = value >> 16 & 1;
            }

            deviceDataIndex += paramLength;
        }

        data["bxpButtonPayload"] = bxpButtonPayload;
        return Newtonsoft.Json.JsonConvert.SerializeObject(data);
    }

    public static string ParseBXPTagPayload(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        var bxpTagPayload = new Dictionary<string, object>();

        while (deviceDataIndex < deviceDataLength)
        {
            int paramTag = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
            deviceDataIndex += 1;
            int paramLength = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray());
            deviceDataIndex += 2;

            if (paramTag == 0)
            {
                int value = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
                bxpTagPayload["rssi"] = value & 1;
                bxpTagPayload["timestamp"] = value >> 1 & 1;
                bxpTagPayload["sensorData"] = value >> 2 & 1;
                bxpTagPayload["hallTriggerEventCount"] = value >> 3 & 1;
                bxpTagPayload["motionTriggerEventCount"] = value >> 4 & 1;
                bxpTagPayload["axisData"] = value >> 5 & 1;
                bxpTagPayload["battVoltage"] = value >> 6 & 1;
                bxpTagPayload["tagId"] = value >> 7 & 1;
                bxpTagPayload["deviceName"] = value >> 8 & 1;
                bxpTagPayload["rawAdv"] = value >> 9 & 1;
                bxpTagPayload["rawResp"] = value >> 10 & 1;
            }

            deviceDataIndex += paramLength;
        }

        data["bxpTagPayload"] = bxpTagPayload;
        return Newtonsoft.Json.JsonConvert.SerializeObject(data);
    }

    public static string ParsePIRPayload(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        var pirPayload = new Dictionary<string, object>();

        while (deviceDataIndex < deviceDataLength)
        {
            int paramTag = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
            deviceDataIndex += 1;
            int paramLength = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray());
            deviceDataIndex += 2;

            if (paramTag == 0)
            {
                int value = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
                pirPayload["rssi"] = value & 1;
                pirPayload["timestamp"] = value >> 1 & 1;
                pirPayload["pirDelayResponseStatus"] = value >> 2 & 1;
                pirPayload["doorStatus"] = value >> 3 & 1;
                pirPayload["sensorSensitivity"] = value >> 4 & 1;
                pirPayload["sensorDetectionStatus"] = value >> 5 & 1;
                pirPayload["battVoltage"] = value >> 6 & 1;
                pirPayload["major"] = value >> 7 & 1;
                pirPayload["minor"] = value >> 8 & 1;
                pirPayload["rssi_1m"] = value >> 9 & 1;
                pirPayload["txPower"] = value >> 10 & 1;
                pirPayload["advName"] = value >> 11 & 1;
                pirPayload["rawAdv"] = value >> 12 & 1;
                pirPayload["rawResp"] = value >> 13 & 1;
            }

            deviceDataIndex += paramLength;
        }

        data["pirPayload"] = pirPayload;
        return Newtonsoft.Json.JsonConvert.SerializeObject(data);
    }
    public static string ParseOtherPayload(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        var otherPayload = new Dictionary<string, object>();
        var array = new List<Dictionary<string, object>>();

        while (deviceDataIndex < deviceDataLength)
        {
            int paramTag = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
            deviceDataIndex += 1;
            int paramLength = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray());
            deviceDataIndex += 2;

            if (paramTag == 0)
            {
                int value = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
                otherPayload["rssi"] = value & 1;
                otherPayload["timestamp"] = value >> 1 & 1;
                otherPayload["rawAdv"] = value >> 2 & 1;
                otherPayload["rawResp"] = value >> 3 & 1;
            }
            else if (paramTag == 1)
            {
                var item = new Dictionary<string, object>();
                item["type"] = deviceDataArray[deviceDataIndex];
                item["start"] = Convert.ToInt32(deviceDataArray[deviceDataIndex + 1], 16);
                item["end"] = Convert.ToInt32(deviceDataArray[deviceDataIndex + 2], 16);
                item["data"] = string.Join("", deviceDataArray.Skip(deviceDataIndex + 3).Take(paramLength - 3));
                array.Add(item);
            }

            deviceDataIndex += paramLength;
        }

        otherPayload["array"] = array;
        data["otherPayload"] = otherPayload;
        return Newtonsoft.Json.JsonConvert.SerializeObject(data);
    }

    public static string ParseFixData(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        var fixData = new Dictionary<string, object>();

        while (deviceDataIndex < deviceDataLength)
        {
            int paramTag = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
            deviceDataIndex += 1;

            int paramLength = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray());
            deviceDataIndex += 2;

            if (paramTag == 0)
            {
                int timestamp = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
                fixData["timestamp"] = timestamp;
                fixData["current_time"] = ParseTime(timestamp, 0);
            }
            else if (paramTag == 1)
            {
                int modeIndex = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
                fixData["fixMode"] = fixModeNotifyArray[modeIndex];
            }
            else if (paramTag == 2)
            {
                int resultIndex = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
                fixData["fixResult"] = fixResultArray[resultIndex];
            }
            else if (paramTag == 3)
            {
                string joinedHex = string.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength));
                string lngHex = joinedHex.Substring(0, (paramLength - 4) * 2);
                string latHex = joinedHex.Substring((paramLength - 4) * 2);

                double longitude = SignedHexToInt(lngHex) * 0.0000001;
                double latitude = SignedHexToInt(latHex) * 0.0000001;

                fixData["longitude"] = longitude.ToString("F7");
                fixData["latitude"] = latitude.ToString("F7");
            }
            else if (paramTag == 4)
            {
                int value = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(paramLength).ToArray());
                fixData["tac_lac"] = value & 0xFFFF;
                fixData["ci"] = (value >> 4) & 0xFFFFFFFF;
            }

            deviceDataIndex += paramLength;
        }

        data["fixData"] = fixData;
        return Newtonsoft.Json.JsonConvert.SerializeObject(data);
    }
    public static string ParseScanDevices(int deviceDataIndex, int deviceDataLength, List<string> deviceDataArray, Dictionary<string, object> data)
    {
        var deviceArray = new List<Dictionary<string, object>>();
        var deviceItem = new Dictionary<string, object>();
        bool doPost = false;

        while (deviceDataIndex < deviceDataLength)
        {
            int paramTag = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
            deviceDataIndex += 1;

            switch (paramTag)
            {
                case 0:
                    if (deviceItem.Count != 0)
                    {
                        deviceArray.Add(deviceItem);
                    }
                    deviceItem = new Dictionary<string, object>();
                    deviceDataIndex += 2;
                    int typeCode = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
                    deviceItem["typeCode"] = typeCode;
                    deviceItem["type"] = typeCodeArray[typeCode];
                    deviceDataIndex += 1;
                    break;

                case 0x1:
                    deviceDataIndex += 2;
                    deviceItem["mac"] = string.Join("", deviceDataArray.Skip(deviceDataIndex).Take(6));
                    deviceDataIndex += 6;
                    break;

                case 0x2:
                    deviceDataIndex += 2;
                    int connectableVal = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
                    deviceItem["connectable"] = connectableVal == 0 ? "Unconnectable" : "Connectable";
                    deviceDataIndex += 1;
                    break;

                case 0x3:
                    deviceDataIndex += 2;
                    int timestamp = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(4).ToArray());
                    deviceDataIndex += 4;
                    int timezone = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16);
                    deviceDataIndex += 1;
                    deviceItem["timestamp"] = timestamp;
                    deviceItem["timezone"] = timezone;
                    deviceItem["current_time"] = ParseTime(timestamp, timezone);
                    break;

                case 0x4:
                    deviceDataIndex += 2;
                    int rssi = Convert.ToInt32(deviceDataArray[deviceDataIndex], 16) - 256;
                    deviceItem["rssi"] = rssi;
                    deviceDataIndex += 1;
                    break;

                case 0x5:
                    {
                        int paramLength = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray());
                        deviceDataIndex += 2;
                        deviceItem["advPacket"] = string.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength));
                        deviceDataIndex += paramLength;
                        break;
                    }

                case 0x6:
                    {
                        int paramLength = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray());
                        deviceDataIndex += 2;
                        deviceItem["responsePacket"] = string.Join("", deviceDataArray.Skip(deviceDataIndex).Take(paramLength));
                        deviceDataIndex += paramLength;
                        break;
                    }

                default:
                    if (paramTag >= 0xA)
                    {
                        int paramLength = ParseHexStrArrayToInt(deviceDataArray.Skip(deviceDataIndex).Take(2).ToArray());
                        deviceDataIndex += 2;

                        int typeCodeVal = Convert.ToInt32(deviceItem["typeCode"]);
                        switch (typeCodeVal)
                        {
                            case 0:
                                ParseIbeacon(ref deviceItem, paramTag, deviceDataArray, deviceDataIndex, paramLength);
                                break;
                            case 1:
                                ParseEddystoneUID(ref deviceItem, paramTag, deviceDataArray, deviceDataIndex, paramLength);
                                break;
                            case 2:
                                ParseEddystoneURL(ref deviceItem, paramTag, deviceDataArray, deviceDataIndex, paramLength);
                                break;
                            case 3:
                                ParseEddystoneTLM(ref deviceItem, paramTag, deviceDataArray, deviceDataIndex, paramLength);
                                break;
                            case 4:
                                ParseDeviceInfo(ref deviceItem, paramTag, deviceDataArray, deviceDataIndex, paramLength);
                                break;
                            case 5:
                                ParseBXPACC(ref deviceItem, paramTag, deviceDataArray, deviceDataIndex, paramLength);
                                break;
                            case 6:
                                ParseBXPTH(ref deviceItem, paramTag, deviceDataArray, deviceDataIndex, paramLength);
                                break;
                            case 7:
                                ParseBXPButton(ref deviceItem, paramTag, deviceDataArray, deviceDataIndex, paramLength);
                                break;
                            case 8:
                                ParseBXPTag(ref deviceItem, paramTag, deviceDataArray, deviceDataIndex, paramLength);
                                break;
                            case 9:
                                ParsePIR(ref deviceItem, paramTag, deviceDataArray, deviceDataIndex, paramLength);
                                break;
                            case 10:
                                ParseOther(ref deviceItem, paramTag, deviceDataArray, deviceDataIndex, paramLength);
                                break;
                            case 11:
                                ParseTOF(ref deviceItem, paramTag, deviceDataArray, deviceDataIndex, paramLength);
                                doPost = true;
                                break;
                        }

                        deviceDataIndex += paramLength;
                    }
                    break;
            }
        }

        if (deviceItem.Count != 0)
        {
            deviceArray.Add(deviceItem);
        }

        data["deviceArray"] = deviceArray;

        //if (doPost)
        //{
        //    log.Debug(JsonConvert.SerializeObject(data));
        //}

        return JsonConvert.SerializeObject(data);
    }

}