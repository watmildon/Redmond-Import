using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// from: https://stackoverflow.com/questions/14671206/how-to-compute-convex-hull-in-c-sharp
// lisence: https://creativecommons.org/licenses/by-sa/4.0/
class ConvexHull
{
    public static double cross(Point O, Point A, Point B)
    {
        return (A.LON - O.LON) * (B.LAT - O.LAT) - (A.LAT - O.LAT) * (B.LON - O.LON);
    }

    public static List<Point> GetConvexHull(List<Point> points)
    {
        if (points == null)
            return null;

        if (points.Count() <= 1)
            return points;

        int n = points.Count(), k = 0;
        List<Point> H = new List<Point>(new Point[2 * n]);

        points.Sort((a, b) =>
             a.LON == b.LON ? a.LAT.CompareTo(b.LAT) : a.LON.CompareTo(b.LON));

        // Build lower hull
        for (int i = 0; i < n; ++i)
        {
            while (k >= 2 && cross(H[k - 2], H[k - 1], points[i]) <= 0)
                k--;
            H[k++] = points[i];
        }

        // Build upper hull
        for (int i = n - 2, t = k + 1; i >= 0; i--)
        {
            while (k >= t && cross(H[k - 2], H[k - 1], points[i]) <= 0)
                k--;
            H[k++] = points[i];
        }

        return H.Take(k - 1).ToList();
    }
}

