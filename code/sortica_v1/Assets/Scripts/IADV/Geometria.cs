using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Clase, únicamente, con métodos de geometría o matemáticos.
 */
public class Geometria
{
    /*
     * Dado un vector con una cierta dirección, halla un nuevo vector dirección teniendo en cuenta una rotación.
     * Para ello, se usa la matriz de rotación teniendo en cuenta que:
     *      
     *      - (x0, y0) = (cos(a), sen(a))
     *      
     *      - (x, y) = (cos(a + b), sen(a + b))
     *      
     *      - x = cos(a + b) = cos(a) * cos(b) - sen(a) * sen(b) = x0 * cos(b) - y0 * sen(b)
     *      
     *      - y = sen(a + b) = sen(a) * cos(b) + cos(a) * sen(b) = y0 * cos(b) + x0 * sen(b) =
     *          x0 * sen(b) + y0 * cos(b)
     *          
     *      Por tanto:
     *      
     *      [   x   ]   [   cos(b), -sen(b) ]   [   x0  ]                    [  x0  ]
     *      [       ] = [                   ] * [       ] = matrizRotacion * [      ]
     *      [   y   ]   [   sen(b), cos(b)  ]   [   y0  ]                    [  y0  ]
     *      
     */
    public static Vector3 hallarNuevaDireccion(float rotacion, Vector3 vector)
    {
        rotacion *= Mathf.Deg2Rad;

        float[,] matrizRotacion =
        {
            {   Mathf.Cos(rotacion),    -Mathf.Sin(rotacion)    },
            {   Mathf.Sin(rotacion),    Mathf.Cos(rotacion)     }
        };

        float x0 = vector.z;
        float y0 = vector.x;

        float xRotado = matrizRotacion[0, 0] * x0 + matrizRotacion[0, 1] * y0;
        float yRotado = matrizRotacion[1, 0] * x0 + matrizRotacion[1, 1] * y0;

        return new Vector3(
            yRotado,
            0,
            xRotado
            );
    }

    public static Vector3 hallarPosicionRelativa(float orientacionRelativa, Vector3 posicionPivote)
    {
        return hallarNuevaDireccion(orientacionRelativa, posicionPivote);
    }

    ///*
    // * Dado un pivote o referencia y una posición relativa a este, se busca calcular la
    // * posición real en el mundo. Para ello, se utiliza la matriz de rotación.
    // */
    //public static Vector3 hallarPosicionDesdeRelativa(Vector3 posicionRelativa, Vector3 posicionPivote)
    //{
    //    float orientacionRelativa = Bodi.PositionToAngle(posicionRelativa);
    //    float[,] matrizRotacion =
    //    {
    //        { Mathf.Cos(orientacionRelativa), -Mathf.Sin(orientacionRelativa)},
    //        { Mathf.Sin(orientacionRelativa), Mathf.Cos(orientacionRelativa)}
    //    };

    //    float zPivote = posicionPivote.z;
    //    float xPivote = posicionPivote.x;

    //    float zRelativo = posicionRelativa.z;
    //    float xRelativo = posicionRelativa.x;

    //    float zAbsoluto = zPivote + (matrizRotacion[0, 0] * zPivote + matrizRotacion[0, 1] * xPivote);
    //    float xAbsoluto = xPivote + (matrizRotacion[1, 0] * zPivote + matrizRotacion[1, 1] * xPivote);

    //    return new Vector3(xAbsoluto, 0, zAbsoluto);
    //}

    /*
     * Dado un pivote o referencia y una posición relativa a este, se busca calcular la
     * posición real en el mundo. Para ello, se utiliza la matriz de rotación.
     */
    public static Vector3 hallarPosicionDesdeRelativa(Vector3 posicionRelativa, Vector3 posicionPivote, float orientacionPivote)
    {
        orientacionPivote *= Mathf.Deg2Rad;

        float[,] matrizRotacion =
        {
            { Mathf.Cos(orientacionPivote), -Mathf.Sin(orientacionPivote)},
            { Mathf.Sin(orientacionPivote), Mathf.Cos(orientacionPivote)}
        };

        Vector3 relativaRotada = new Vector3(
            matrizRotacion[1, 0] * posicionRelativa.z + matrizRotacion[1, 1] * posicionRelativa.x,
            0,
            matrizRotacion[0, 0] * posicionRelativa.z + matrizRotacion[0, 1] * posicionRelativa.x
            );

        return posicionPivote + relativaRotada;
    }

    /*
     * Calcula la posición final de una recta dado el origen y la dirección.
     */
    public static Vector3 posicionFinalRecta(Vector3 origen, Vector3 direccion, float distanciaRecta)
    {
        return origen + direccion.normalized * distanciaRecta;
    }
}
