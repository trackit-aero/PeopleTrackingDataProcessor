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

        public partial class MqPacket
    {
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("payloadReportedAt")]
        public DateTime PayloadReportedAt { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("purpletype")]
        public string PurpleType { get; set; }

        [JsonProperty("mac")]
        public string Mac { get; set; }

        [JsonProperty("readerip")]
        public string ReaderIP { get; set; }

        [JsonProperty("antenna")]
        public string Antenna { get; set; }

        [JsonProperty("bleName")]
        public string BleName { get; set; }

        [JsonProperty("rssi")]
        public long? Rssi { get; set; }

        [JsonProperty("battery")]
        public long? Battery { get; set; }

        [JsonProperty("rangingDistance")]
        public long? RangingDistance { get; set; }

        [JsonProperty("rawData")]
        public string RawData { get; set; }
    }


    public partial class MqPacket
    {
        public static MqPacket[] FromJson(string json)
        {
            return JsonConvert.DeserializeObject<MqPacket[]>(json, Converter.Settings);

        }
    }

}
