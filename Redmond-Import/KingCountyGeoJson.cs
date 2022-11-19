public class Geometry
{
    public string type { get; set; }
    public List<double> coordinates { get; set; }
}

public class Properties
{
    public string ADDR_NUM { get; set; } // ex: 12345
    public string FULLNAME { get; set; } // ex NE 90th ST
    public string ZIP5 { get; set; }
    public string PLUS4 { get; set; }
    public string POSTALCTYNAME { get; set; }
    public string Unit { get; set; }

    public string name { get; set; } 
}

public class KingCountyGeoJSON
{
    public string type { get; set; }
    public string name { get; set; }
    public Crs crs { get; set; }
    public List<Feature> features { get; set; }
}
public class Crs
{
    public string type { get; set; }
    public Properties properties { get; set; }
}

public class Feature
{
    public string type { get; set; }
    public Properties properties { get; set; }
    public Geometry geometry { get; set; }
}