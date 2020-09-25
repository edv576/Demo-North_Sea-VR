using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Values : MonoBehaviour
{
    internal static readonly double D2R = Math.PI / 180.0;
    internal static readonly double TwoPi = Math.PI + Math.PI;
    internal static readonly double HalfPi = Math.PI / 2;
    internal static readonly double Radius = 6378137;

    /* Coordinates of Amersfoort in degrees: 5 23' 15.5'', 52 9' 22.178'' */
    internal static readonly double AmersfoortXll = 5.0 + (23.0 / 60.0) + (15.500 / 3600.0);
    internal static readonly double AmersfoortYll = 52.0 + (9.0 / 60.0) + (22.178 / 3600.0);

    /* Coordinates of Amersfoort in Rijksdriehoek meting */
    internal static readonly double AmersfoortXrd = 155000.0;
    internal static readonly double AmersfoortYrd = 463000.0;
}
