using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RD : MonoBehaviour
{
    /// <summary>
    /// Project WGS84 lat/long to RD coordinates
    /// </summary>
    /// <param name="lon">longitude (datum Amersfoort)</param>
    /// <param name="lat">latitude (datum Amersfoort)</param>
    /// <param name="x_out">RD x coordinate out</param>
    /// <param name="y_out">RD y coordinate out</param>
    public static void AmersfoortLonLatToRd(double lon, double lat, out double x_out, out double y_out)
    {
        var rd = AmersfoortLonLatToRdCalculation(lon, lat);
        x_out = rd.x;
        y_out = rd.y;
    }

    /// <summary>
    /// Project WGS84 lat/long to RD coordinates
    /// Returns a Point
    /// </summary>
    /// <param name="lon">longitude (datum Amersfoort)</param>
    /// <param name="lat">latitude (datum Amersfoort)</param>
    public static Vector2 AmersfoortLonLatToRd(double lon, double lat)
    {
        return AmersfoortLonLatToRdCalculation(lon, lat);
    }

    private static Vector2 AmersfoortLonLatToRdCalculation(double lon, double lat)
    {
        var lonlatX = lon;
        var lonlatY = lat;

        // Calculate x and y in units of 10000 seconds
        // (Note: lat, lon are converted to degrees (0-360))
        var lambda1 = ((lonlatX / Values.D2R) - Values.AmersfoortXll) * 3600.0 / 10000.0;
        var phi1 = ((lonlatY / Values.D2R) - Values.AmersfoortYll) * 3600.0 / 10000.0;

        /* Precompute powers */
        var phi2 = phi1 * phi1;
        var phi3 = phi2 * phi1;
        var phi4 = phi3 * phi1;
        var lambda2 = lambda1 * lambda1;
        var lambda3 = lambda2 * lambda1;
        var lambda4 = lambda3 * lambda1;
        var lambda5 = lambda4 * lambda1;

        /* Calculate coordinates in meters
        * "DG Rijkswaterstaat. Coordinaattransformaties en kaartprojecties. 1993, p. 19"
        */
        var xOut = Values.AmersfoortXrd
                      + 190066.98903 * lambda1
                      - 11830.85831 * phi1 * lambda1
                      - 144.19754 * phi2 * lambda1
                      - 32.38360 * lambda3
                      - 2.34078 * phi3 * lambda1
                      - 0.60639 * phi1 * lambda3
                      + 0.15774 * phi2 * lambda3
                      - 0.04158 * phi4 * lambda1
                      - 0.00661 * lambda5;

        var yOut = Values.AmersfoortYrd
                      + 309020.31810 * phi1
                      + 3638.36193 * lambda2
                      - 157.95222 * phi1 * lambda2
                      + 72.97141 * phi2
                      + 59.79734 * phi3
                      - 6.43481 * phi2 * lambda2
                      + 0.09351 * lambda4
                      - 0.07379 * phi3 * lambda2
                      - 0.05419 * phi1 * lambda4
                      - 0.03444 * phi4;

        return new Vector2((float)xOut, (float)yOut);
    }

    /// <summary>
    /// Transforms RD coordinates to Amersfoort lat/long
    /// </summary>
    /// <param name="xIn">x coordinate</param>
    /// <param name="yIn">y coordinate</param>
    /// <param name="lonOut">longitude (datum Amersfoort)</param>
    /// <param name="latOut">latitude (datum Amersfoort)</param>
    public static void RdToAmersfoortLonLat(double xIn, double yIn, out double lonOut, out double latOut)
    {
        var lonLat = RdToAmersfoortLonLatCalculation(xIn, yIn);
        lonOut = lonLat.x;
        latOut = lonLat.y;
    }

    /// <summary>
    /// Transforms RD coordinates to Amersfoort lat/long
    /// Returns Point
    /// </summary>
    /// <param name="xIn">x coordinate</param>
    /// <param name="yIn">y coordinate</param>
    public static Vector2 RdToAmersfoortLonLat(double xIn, double yIn)
    {
        return RdToAmersfoortLonLatCalculation(xIn, yIn);
    }

    static Vector2 RdToAmersfoortLonLatCalculation(double xIn, double yIn)
    {
        /* Calculate coordinates in 100 kilometers units
        * with Amersfoort(155,463) as origin
        */
        var x = xIn;
        var y = yIn;

        x = (x - Values.AmersfoortXrd) / 100000.0;
        y = (y - Values.AmersfoortYrd) / 100000.0;

        /* Precompute second, third and fourth power */
        var x2 = x * x;
        var x3 = x2 * x;
        var x4 = x3 * x;
        var x5 = x4 * x;

        var y2 = y * y;
        var y3 = y2 * y;
        var y4 = y3 * y;
        var y5 = y4 * y;

        /*
        * Calculate coordinates in lat-long seconds
        * "DG Rijkswaterstaat. Coordinaattransformaties en kaartprojecties. 1993, p. 20"
        * (Lambda = X direction, phi is Y direction)
        */
        var lambda = +5261.3028966 * x
                        + 105.9780241 * x * y
                        + 2.4576469 * x * y2
                        - 0.8192156 * x3
                        - 0.0560092 * x3 * y
                        + 0.0560089 * x * y3
                        - 0.0025614 * x3 * y2
                        + 0.0012770 * x * y4
                        + 0.0002574 * x5
                        - 0.0000973 * x3 * y3
                        + 0.0000293 * x5 * y
                        + 0.0000291 * x * y5;

        var phi = +3236.0331637 * y
                     - 32.5915821 * x2
                     - 0.2472814 * y2
                     - 0.8501341 * x2 * y
                     - 0.0655238 * y3
                     - 0.0171137 * x2 * y2
                     + 0.0052771 * x4
                     - 0.0003859 * x2 * y3
                     + 0.0003314 * x4 * y
                     + 0.0000371 * y4
                     + 0.0000143 * x4 * y2
                     - 0.0000090 * x2 * y4;

        /* x and y are in seconds from Amersfoort. Recompute degrees
        * and add Amersfoort in degrees
        */
        var lonOut = Values.D2R * (Values.AmersfoortXll + (lambda / 3600.0));
        var latOut = Values.D2R * (Values.AmersfoortYll + (phi / 3600.0));

        return new Vector2((float)lonOut, (float)latOut);
    }

    /// <summary>
    /// Transforms RD coordinates to WGS84 lat/long
    /// </summary>
    /// <param name="xIn">RD x coordinate</param>
    /// <param name="yIn">RD y coordinate</param>
    /// <param name="lonOut">WGS84 longitude out</param>
    /// <param name="latOut">WGS84 latitude out</param>
    public static void RdToLonLat(double xIn, double yIn, out double lonOut, out double latOut)
    {
        var aLonLat = RdToAmersfoortLonLatCalculation(xIn, yIn);
        var lonLat = AmersfoortLonLatToLonLatCalculation(aLonLat.x, aLonLat.y);
        lonOut = lonLat.x;
        latOut = lonLat.y;
    }

    /// <summary>
    /// Transforms RD coordinates to WGS84 lat/long\n
    /// Returns a Point
    /// </summary>
    /// <param name="xIn">RD x coordinate</param>
    /// <param name="yIn">RD y coordinate</param>
    public static Vector2 RdToLonLat(double xIn, double yIn)
    {
        var aLonLat = RdToAmersfoortLonLatCalculation(xIn, yIn);
        var lonLat = AmersfoortLonLatToLonLatCalculation(aLonLat.x, aLonLat.y);

        return new Vector2((float)lonLat.x * 57.2957795f, (float)lonLat.y * 57.2957795f);
    }

    /// <summary>
    /// Transforms WGS84 Lon Lat coordinates to RD
    /// </summary>
    /// <param name="lon">WGS84 lon coordinate</param>
    /// <param name="lat">WGS84 lat coordinate</param>
    /// <param name="xOut">RD x coordinate out</param>
    /// <param name="yOut">RD y coordinate out</param>
    public static void LonLatToRd(double lon, double lat, out double xOut, out double yOut)
    {
        var aLonLat = LonLatToAmersfoortLonLatCalculation(lon, lat);
        var rd = AmersfoortLonLatToRdCalculation(aLonLat.x, aLonLat.y);
        xOut = rd.x;
        yOut = rd.y;
    }

    /// <summary>
    /// Transforms WGS84 Lon Lat coordinates to RD
    /// </summary>
    /// <param name="lon">WGS84 lon coordinate</param>
    /// <param name="lat">WGS84 lat coordinate</param>
    public static Vector2 LonLatToRd(double lon, double lat)
    {
        var aLonLat = LonLatToAmersfoortLonLatCalculation(lon * 0.0174532925, lat * 0.0174532925);
        var rd = AmersfoortLonLatToRdCalculation(aLonLat.x, aLonLat.y);

        return rd;
    }

    /// <summary>
    /// Transform Amersfoort lon/lat to WGS84 lon/lat
    /// </summary>
    /// <param name="lonIn">longitude (datum Amersfoort)</param>
    /// <param name="latIn">latitude (datum Amersfoort)</param>
    /// <param name="lonOut">longitude (datum WGS84) out</param>
    /// <param name="latOut">latitude (datum WGS84) out</param>
    public static void AmersfoortLonLatToLonLat(double lonIn, double latIn, out double lonOut, out double latOut)
    {
        var lonLat = AmersfoortLonLatToLonLatCalculation(lonIn, latIn);
        lonOut = lonLat.x;
        latOut = lonLat.y;
    }

    /// <summary>
    /// Transform Amersfoort lon/lat to WGS84 lon/lat
    /// </summary>
    /// <param name="lonIn">longitude (datum Amersfoort)</param>
    /// <param name="latIn">latitude (datum Amersfoort)</param>
    public static Vector2 AmersfoortLonLatToLonLat(double lonIn, double latIn)
    {
        return AmersfoortLonLatToLonLatCalculation(lonIn, latIn);
    }

    static Vector2 AmersfoortLonLatToLonLatCalculation(double lonIn, double latIn)
    {
        // delta(WGSlat, WGSlong) = (c1, c2) + A * delta(Blat, Blong)
        // in which A is the matrix:
        //     a11 a12
        //     a21 a22
        // and the deltas are offsets from (52, 5)

        // formula from an article by ir.T.G.Schut
        // published in NGT Geodesia, june 1992.

        //input is in rad, these are converted to degrees first
        lonIn = lonIn / Values.D2R;
        latIn = latIn / Values.D2R;

        const double c1 = -96.862 / 100000.0;
        const double c2 = -37.902 / 100000.0;
        const double a11 = -11.714 / 100000.0 + 1.0;
        const double a12 = -0.125 / 100000.0;
        const double a21 = 0.329 / 100000.0;
        const double a22 = -14.667 / 100000.0 + 1.0;

        const double lat0 = 52.0; // North
        const double long0 = 5.0; // East

        var deltaLat = latIn - lat0;
        var deltaLong = lonIn - long0;

        var latOut = c1 + lat0 + a11 * deltaLat + a12 * deltaLong;
        var lonOut = c2 + long0 + a21 * deltaLat + a22 * deltaLong;

        //and back from degrees to rad
        lonOut = lonOut * Values.D2R;
        latOut = latOut * Values.D2R;

        return new Vector2((float)lonOut, (float)latOut);
    }

    /// <summary>
    /// Transform WGS84 lat/long to Amersfoort lat/long
    /// </summary>
    /// <param name="lonIn">longitude (datum WGS84)</param>
    /// <param name="latIn">latitude (datum WGS84)</param>
    /// <param name="lonOut">longitude (datum Amersfoort)</param>
    /// <param name="latOut">latitude (datum Amersfoort)</param>
    public static void LonLatToAmersfoortLonLat(double lonIn, double latIn, out double lonOut, out double latOut)
    {
        var lonLat = LonLatToAmersfoortLonLatCalculation(lonIn, latIn);
        lonOut = lonLat.x;
        latOut = lonLat.y;
    }

    /// <summary>
    /// Transform WGS84 lat/long to Amersfoort lat/long
    /// </summary>
    /// <param name="lonIn">longitude (datum WGS84)</param>
    /// <param name="latIn">latitude (datum WGS84)</param>
    public static Vector2 LonLatToAmersfoortLonLat(double lonIn, double latIn)
    {
        return LonLatToAmersfoortLonLatCalculation(lonIn, latIn);
    }

    /// <summary>
    /// Transform Lon Lat to Amersfoort Lon Lat
    /// </summary>
    /// <param name="lonIn"></param>
    /// <param name="latIn"></param>
    /// <returns></returns>
    public static Vector2 LonLatToAmersfoortLonLatCalculation(double lonIn, double latIn)
    {
        // delta(Blat, Blong) = D * { delta(WGSlat, WGSlong) - (c1, c2) }
        // in which D is the inverse van matrix A
        // and the deltas are offsets from (52, 5)

        //input is in rad, these are converted to degrees first
        lonIn = lonIn / Values.D2R;
        latIn = latIn / Values.D2R;

        const double c1 = -96.862 / 100000.0;
        const double c2 = -37.902 / 100000.0;
        const double a11 = -11.714 / 100000.0 + 1.0;
        const double a12 = -0.125 / 100000.0;
        const double a21 = 0.329 / 100000.0;
        const double a22 = -14.667 / 100000.0 + 1.0;

        const double lat0 = 52.0; // North
        const double long0 = 5.0; // East

        const double normA = 1.0 / (a11 * a22 - a12 * a21);
        const double d11 = +a22 * normA;
        const double d12 = -a12 * normA;
        const double d21 = -a21 * normA;
        const double d22 = +a11 * normA;

        var deltaLat = latIn - lat0 - c1;
        var deltaLong = lonIn - long0 - c2;

        var latOut = lat0 + d11 * deltaLat + d12 * deltaLong;
        var lonOut = long0 + d21 * deltaLat + d22 * deltaLong;

        //and back from degrees to rad
        lonOut = lonOut * Values.D2R;
        latOut = latOut * Values.D2R;

        return new Vector2((float)lonOut, (float)latOut);
    }
}
