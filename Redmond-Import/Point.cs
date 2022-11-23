public class Point
{
    public Point(double lon, double lat)
    {
        LAT=lat;
        LON=lon;
    }

    public double LAT { get; internal set; }
    public double LON { get; internal set; }
}