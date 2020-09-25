using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Windows;


public class Spherical : MonoBehaviour
{

    /// <summary>
    /// Transforms WGS84 Lat Lon to Spherical Mercator
    /// </summary>
    /// <param name="lon">Longitude</param>
    /// <param name="lat">Latitude</param>
    /// <param name="xOut"></param>
    /// <param name="yOut"></param>
    public static void LonLatToSpherical(double lon, double lat, out double xOut, out double yOut)
    {
        var spherical = LonLatToSphericalCalculation(lon, lat);
        xOut = spherical.x;
        yOut = spherical.y;
    }

    /// <summary>
    /// Transforms WGS84 Lat Lon to Spherical Mercator
    /// Returns Point
    /// </summary>
    /// <param name="lon">Longitude</param>
    /// <param name="lat">Latitude</param>
    public static Vector2 LonLatToSpherical(double lon, double lat)
    {
        return LonLatToSphericalCalculation(lon, lat);
    }

    static Vector2 LonLatToSphericalCalculation(double lon, double lat)
    {
        var lonRadians = (Values.D2R * lon);
        var latRadians = (Values.D2R * lat);

        var x = Values.Radius * lonRadians;
        var y = Values.Radius * Math.Log(Math.Tan(Math.PI * 0.25 + latRadians * 0.5));

        return new Vector2((float)x, (float)y);
    }

    /// <summary>
    /// Transforms Spherical Mercator to Lon Lat WGS84
    /// </summary>
    /// <param name="x">Spherical x</param>
    /// <param name="y">Spherical y</param>
    /// <param name="xOut">WGS84 Longitude output value</param>
    /// <param name="yOut">WGS84 Latitude output value</param>
    public static void SphericalToLonLat(double x, double y, out double xOut, out double yOut)
    {
        var lonLat = SphericalToLonLatCalculation(x, y);
        xOut = lonLat.x;
        yOut = lonLat.y;
    }

    /// <summary>
    /// Transforms Spherical Mercator to Lon Lat WGS84
    /// Returns a System.Windows.Point
    /// </summary>
    /// <param name="x">Spherical x</param>
    /// <param name="y">Spherical y</param>
    public static Vector2 SphericalToLonLat(double x, double y)
    {
        return SphericalToLonLatCalculation(x, y);
    }

    private static Vector2 SphericalToLonLatCalculation(double x, double y)
    {
        var ts = Math.Exp(-y / (Values.Radius));
        var latRadians = Values.HalfPi - 2 * Math.Atan(ts);
        var lonRadians = x / (Values.Radius);
        var lon = (lonRadians / Values.D2R);
        var lat = (latRadians / Values.D2R);

        return new Vector2((float)lon, (float)lat);
    }
}
