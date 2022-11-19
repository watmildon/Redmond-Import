using NAD_OSM_Parser;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

public static class AddressImport
{
    static string GEOJSON_FILE_PATH = @"..\..\..\testdata\OneHouse___address_point.geojson";
    static string CURRENT_OSM_DATA = null;

    /*
     * King County GeoJSON parser
     * Used to process address data files from KingCounty GIS 
     * (https://gis-kingcounty.opendata.arcgis.com/datasets/addresses-in-king-county-address-point/)
     * into appropriate files for OSM import.
     * 
     * This data is available to OSM as per email here:
     * https://groups.google.com/g/osm-seattle/c/XhQwyBlkqeI?hl=en
     * 
     * Contributor attribution here:
     * https://wiki.openstreetmap.org/wiki/Contributors#King_County.2C_Washington
     * 
     * To use:
     * download GIS data locally
     * modify the GEOJSON_FILE_PATH to point to the data file
     * run
     */
    public static void Main()
    {
        KingCountyGeoJSON kingCountyData = JsonConvert.DeserializeObject<KingCountyGeoJSON>(File.ReadAllText(GEOJSON_FILE_PATH));
        WriteGeoJSONForOSMImport(kingCountyData);

        //CountDupeGeoJSONForImport();

        return;
    }

    private static void CountDupeGeoJSONForImport()
    {
        if (CURRENT_OSM_DATA == null)
        {
            Console.WriteLine("No path supplied for current OSM data geojson");
        }
        HashSet<string> existingAddress = CreateCurrentAddressSet();
        KingCountyGeoJSON kingCountyData = JsonConvert.DeserializeObject<KingCountyGeoJSON>(File.ReadAllText(GEOJSON_FILE_PATH));
        int numDupes = 0;
        foreach (var element in kingCountyData.features)
        {
            string hnAndStreet = element.properties.ADDR_NUM + " " + OsmImportHelpers.ExpandStreetName(element.properties.FULLNAME);
            if (existingAddress.Contains(hnAndStreet.ToLowerInvariant()))
            {
                numDupes++;
            }
        }
        Console.WriteLine(numDupes);
    }

    private static HashSet<string> CreateCurrentAddressSet()
    {
        OSMAddressGeoJson currentOsmGeoJSON = JsonConvert.DeserializeObject<OSMAddressGeoJson>(File.ReadAllText(CURRENT_OSM_DATA));

        HashSet<string> existingAddress = new HashSet<string>(4000);


        foreach (var element in currentOsmGeoJSON.features)
        {
            string hnAndStreet = element.properties.addrhousenumber + " " + element.properties.addrstreet;
            hnAndStreet = hnAndStreet.ToLowerInvariant();
            if (!existingAddress.Contains(hnAndStreet))
            {
                existingAddress.Add(hnAndStreet);
            }
        }

        return existingAddress;
    }

    private static void WriteGeoJSONForOSMImport(KingCountyGeoJSON kingCountyData)
    {
        foreach (var entry in kingCountyData.features)
        {
            entry.properties.FULLNAME = OsmImportHelpers.ExpandStreetName(entry.properties.FULLNAME);
                        
            if (!String.IsNullOrEmpty(entry.properties.PLUS4))
            {
                entry.properties.ZIP5 = entry.properties.ZIP5 + "-" + entry.properties.PLUS4;
            }
            
            entry.properties.POSTALCTYNAME = OsmImportHelpers.Capitalize(entry.properties.POSTALCTYNAME);
            // null out all unnecessary fields to keep them from being written
            entry.properties.PLUS4 = null;
        }

        string outFilePath = Path.Combine(Path.GetDirectoryName(GEOJSON_FILE_PATH),
            Path.GetFileNameWithoutExtension(GEOJSON_FILE_PATH),
            "_OSM",
            Path.GetExtension(GEOJSON_FILE_PATH));

        JsonSerializer serializer = new JsonSerializer();
        serializer.NullValueHandling = NullValueHandling.Ignore;
        serializer.ContractResolver = new CustomDataContractResolver(); // converts to OSMisms

        using (StreamWriter sw = new StreamWriter(outFilePath))
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            serializer.Serialize(writer, kingCountyData);
        }
    }
}

// Used when writing geoJSON to disk to get the proper OSM tags for property names
public class CustomDataContractResolver : DefaultContractResolver
{
    public static readonly CustomDataContractResolver Instance = new CustomDataContractResolver();

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);
        if (property.DeclaringType == typeof(Properties))
        {
            if (property.PropertyName.Equals("ADDR_NUM"))
            {
                property.PropertyName = "addr:housenumber";
            }
            if (property.PropertyName.Equals("ZIP5"))
            {
                property.PropertyName = "addr:postcode";
            }
            if (property.PropertyName.Equals("FULLNAME"))
            {
                property.PropertyName = "addr:street";
            }
            if (property.PropertyName.Equals("POSTALCTYNAME"))
            {
                property.PropertyName = "addr:city";
            }
            if (property.PropertyName.Equals("Unit"))
            {
                property.PropertyName = "addr:unit";
            }
        }
        return property;
    }
}

public static class OsmImportHelpers
{
    public static string ExpandStreetName(string fullName)
    {
        string result = "";
        string[] addParts = fullName.Split(' ');
        for(int i = 0; i < addParts.Length; i++)
        {
            result += (ExpandPart(addParts[i]));
            if (i <addParts.Length - 1)
            {
                result += ' ';
            }
        }

        return result;
    }

    private static string ExpandPart(string name)
    {
        //Ported from the script at https://gitlab.com/zyphlar/salem-import

        // Specific suffixes like "123th" we have lower
        if (Regex.IsMatch(name,"[0-9]+TH"))
            return name.ToLowerInvariant();
        if (Regex.IsMatch(name, "[0-9]+ND"))
            return name.ToLowerInvariant();
        if (Regex.IsMatch(name, "[0-9]+ST"))
            return name.ToLowerInvariant();
        if (Regex.IsMatch(name, "[0-9]+RD"))
            return name.ToLowerInvariant();

        // Weird names like 123D we keep upper
        if (Regex.IsMatch(name, "[0-9]+[A-Z]+"))
            return name;

        // Prefixes we want to keep uppercase
        if (name == "US")
            return "US";
        if (name == "SR")
            return "SR";
        if (name == "CR")
            return "CR";
        if (name == "C")
            return "C";

        // Directions
        if (name == "N")
            return "North";
        if (name == "NE")
            return "Northeast";
        if (name == "E")
            return "East";
        if (name == "SE")
            return "Southeast";
        if (name == "S")
            return "South";
        if (name == "SW")
            return "Southwest";
        if (name == "W")
            return "West";
        if (name == "NW")
            return "Northwest";
        // Suffixes
        if (name == "AV")
            return "Avenue";
        if (name == "AVE")
            return "Avenue";
        if (name == "BLVD")
            return "Boulevard";
        if (name == "BND")
            return "Bend";
        if (name == "CIR")
            return "Circle";
        if (name == "CT")
            return "Court";
        if (name == "DR")
            return "Drive";
        if (name == "FLDS")
            return "Fields";
        if (name == "GRV")
            return "Grove";
        if (name == "HOLW")
            return "Hollow";
        if (name == "HW")
            return "Highway";
        if (name == "HWY")
            return "Highway";
        if (name == "LN")
            return "Lane";
        if (name == "LP")
            return "Loop";
        if (name == "LOOP")
            return "Loop";
        if (name == "PATH")
            return "Path";
        if (name == "PL")
            return "Place";
        if (name == "RD")
            return "Road";
        if (name == "RUN")
            return "Run";
        if (name == "ST")
            return "Street";
        if (name == "TER")
            return "Terrace";
        if (name == "TL")
            return "Trail";
        if (name == "TRL")
            return "Trail";
        if (name == "VW")
            return "View";
        if (name == "WAY")
            return "Way";
        if (name == "XING")
            return "Crossing";
        // Irish names
        if (name == "MCCRAY")
            return "McCray";
        if (name == "MCKOWN")
            return "McKown";

        return name = Capitalize(name);
    }

    public static string Capitalize(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }
        char[] a = s.ToLowerInvariant().ToCharArray();
        a[0] = char.ToUpper(a[0]);
        return new string(a);
    }
}