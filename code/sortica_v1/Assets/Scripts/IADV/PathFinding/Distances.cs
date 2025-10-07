using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Distances
{
    public enum Distancia
    {
        Manhattan,
        Chebyshev,
        Euclidea
    }

    public static float ChebyshevDistance(Tile a, Tile b)
    {
        float distance = Mathf.Max(Mathf.Abs(a.Position.x - b.Position.x), Mathf.Abs(a.Position.y - b.Position.y));
        return distance;
    }

    public static float ManhattanDistance(Tile a, Tile b)
    {
        float distance = Mathf.Abs(a.Position.x - b.Position.x) + Mathf.Abs(a.Position.y - b.Position.y);
        return distance;
    }

    public static float EuclideanDistance(Tile a, Tile b)
    {
        float distance = Mathf.Sqrt(Mathf.Pow(Mathf.Abs(a.Position.x - b.Position.x), 2) + Mathf.Pow(Mathf.Abs(a.Position.y - b.Position.y), 2));
        return distance;
    }

    public static float GetDistance(Tile a, Tile b, Distancia distanciaSeleccionada)
    {
        float distance;

        switch (distanciaSeleccionada)
        {
            case Distancia.Chebyshev:
                distance = ChebyshevDistance(a, b);
                break;
            case Distancia.Manhattan:
                distance = ManhattanDistance(a, b);
                break;
            case Distancia.Euclidea:
                distance = EuclideanDistance(a, b);
                break;
            default:
                distance = ChebyshevDistance(a, b);
                break;
        }

        return distance;
    }
}
