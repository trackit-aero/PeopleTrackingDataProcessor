// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using QuickType;
//
//    var data = MqPacket.FromJson(jsonString);

namespace QuickType
{
    using System;
    using System.Net;
    using System.Collections.Generic;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System.Text;

    public static class Serialize
    {
        public static string ToJson(this MqPacket[] self)
        {
            return JsonConvert.SerializeObject(self, Converter.Settings);
        }

        public static JTokenType CheckJsonType(string json)
        {
            JToken parsedJson = JToken.Parse(json);

            return parsedJson.Type;
        }
    }

    public class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }


    public class OtherInfo
    {
        public int? Battery { get; set; }
        public long? RangingDistance { get; set; }
    }

    public static class HexConverter
    {
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
            StringBuilder sb = new StringBuilder();
            foreach (string hex in hexArray)
            {
                sb.Append(Convert.ToChar(Convert.ToUInt32(hex, 16)));
            }
            return sb.ToString();
        }

        public static long ParseHexStrArrayToInt(string[] hexArray)
        {
            string temp = string.Join("", hexArray);
            return Convert.ToInt64(temp, 16);
        }

        public static DateTime ParseTime(int timestamp, int timezone)
        {
            //// Implement the parsing logic for time based on timestamp and timezone
            //return DateTimeOffset.FromUnixTimeSeconds(timestamp).ToLocalTime().DateTime.AddHours(timezone);

            // Create a DateTime representing the Unix epoch (January 1, 1970)
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Add the number of seconds from the Unix timestamp
            DateTime dateTime = epoch.AddSeconds(timestamp);

            // Adjust for the specified timezone offset
            return dateTime.ToLocalTime().AddHours(timezone);
        }

        public static List<string> ToHexStrArray(string value)
        {
            string rep = value.Replace(" ", "");
            List<string> array = new List<string>();
            int arrLen = rep.Length / 2;
            for (int i = 0; i < arrLen; i++)
            {
                array.Add(rep.Substring(i * 2, 2));
            }
            return array;
        }

        public static int ParseHexStrArrayToInt(List<string> hexStrArray)
        {
            return Convert.ToInt32(string.Join("", hexStrArray), 16);
        }

        public static string HexStrToString(List<string> value)
        {
            List<char> array = new List<char>();
            int arrLen = value.Count;
            for (int i = 0; i < arrLen; i++)
            {
                array.Add((char)Convert.ToInt32(value[i], 16));
            }
            return new string(array.ToArray());
        }
    }
}
