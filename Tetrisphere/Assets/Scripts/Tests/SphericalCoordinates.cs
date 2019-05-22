using System;
using UnityEngine;

[Serializable]
public class SphericalCoordinates
{
    public float radius;
    public float polar;
    public float elevation;

    public Vector3 ToCartesian()
    {
        Vector3 outCart = new Vector3();
        SphericalCoordinates.SphericalToCartesian(this.radius, this.polar, this.elevation, out outCart);
        return outCart;
    }

    public static SphericalCoordinates CartesianToSpherical(Vector3 cartCoords)
    {
        SphericalCoordinates sphericalCoordinates = new SphericalCoordinates();
        SphericalCoordinates.CartesianToSpherical(cartCoords, out sphericalCoordinates.radius, out sphericalCoordinates.polar, out sphericalCoordinates.elevation);
        return sphericalCoordinates;
    }

    public static void SphericalToCartesian(float radius, float polar, float elevation, out Vector3 outCart)
    {
        float num = radius * Mathf.Cos(elevation);
        outCart.x = num * Mathf.Cos(polar);
        outCart.y = radius * Mathf.Sin(elevation);
        outCart.z = num * Mathf.Sin(polar);
    }

    public static void CartesianToSpherical(Vector3 cartCoords, out float outRadius, out float outPolar, out float outElevation)
    {
        if ((double)cartCoords.x == 0.0)
            cartCoords.x = 1.401298E-45f;
        outRadius = Mathf.Sqrt((float)((double)cartCoords.x * (double)cartCoords.x + (double)cartCoords.y * (double)cartCoords.y + (double)cartCoords.z * (double)cartCoords.z));
        outPolar = Mathf.Atan(cartCoords.z / cartCoords.x);
        if ((double)cartCoords.x < 0.0)
            outPolar = outPolar + 3.141593f;
        outElevation = Mathf.Asin(cartCoords.y / outRadius);
    }
}