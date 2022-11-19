using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAD_OSM_Parser
{

    public class OSMAddressGeoJson
    {
        public string type { get; set; }
        public string generator { get; set; }
        public Feature[] features { get; set; }
    }

    public class Feature
    {
        public Properties properties { get; set; }
    }

    public class Properties
    {
        [JsonProperty("addr:city")]
        public string addrcity { get; set; }
        [JsonProperty("addr:housenumber")]
        public string addrhousenumber { get; set; }
        [JsonProperty("addr:postcode")]
        public string addrpostcode { get; set; }
        [JsonProperty("addr:state")]
        public string addrstate { get; set; }
        [JsonProperty("addr:street")]
        public string addrstreet { get; set; }
    }

}
