using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Clase que tiene las características físicas del agente (masa, transform, aceleración y velocidad máxima, etc).
public class Bodi : MonoBehaviour
{
    public float alturaPersonaje = 1f;

    // Valor de velocidad (escalar) máximo que puede alcanzar el agente.
    [SerializeField] private float _velocidadMaxima;
    public float VelocidadMaxima
    {
        get { return _velocidadMaxima; }
        set
        {
            // No tiene sentido que el valor sea negativo.
            if (value < 0)
            {
                _velocidadMaxima = 0;
                return;
            }

            _velocidadMaxima = value;
        }
    }

    // Vector que representa la velocidad del agente.
    [SerializeField] private Vector3 _velocidad;
    public Vector3 Velocidad
    {
        get { return _velocidad; }
        set
        {
            // Si se supera la velocidad máxima, se ajusta la velocidad a dicha velocidad máxima.
            if (value.magnitude > VelocidadMaxima)
            {
                _velocidad = value.normalized * VelocidadMaxima;
                return;
            }

            _velocidad = value;
        }
    }

    // Valor que representa la magnitud del vector velocidad (escalar).
    public float Speed
    {
        get
        {
            return Velocidad.magnitude;
        }
    }

    // Valor de acelaración (escalar) máximo que puede alcanzar el agente.
    [SerializeField] private float _aceleracionMaxima;
    public float AceleracionMaxima
    {
        get { return _aceleracionMaxima; }
        set
        {
            // No tiene sentido que el valor sea negativo.
            if (value < 0)
            {
                _aceleracionMaxima = 0;
                return;
            }

            _aceleracionMaxima = value;
        }
    }

    // Vector que representa la aceleración del agente.
    [SerializeField] private Vector3 _aceleracion;
    public Vector3 Aceleracion
    {
        get { return _aceleracion; }
        set
        {
            // Si se supera la aceleración máxima, se ajusta la aceleración a dicha aceleración máxima.
            if (value.magnitude > AceleracionMaxima)
            {
                _aceleracion = value.normalized * AceleracionMaxima;
                return;
            }

            _aceleracion = value;
        }
    }

    // Valor (escalar) que representa la masa del agente.
    [SerializeField] private float _masa;
    public float Masa
    {
        get { return _masa; }
        set
        {
            // No tiene sentido que el valor sea negativo.
            if (value < 0)
            {
                _masa = 0;
                return;
            }

            _masa = value;
        }
    }

    // Límites superior e inferior para el ángulo de orientación del agente.
    private static float LIM_INFERIOR_ANGULO = 0;
    private static float LIM_SUPERIOR_ANGULO = 360;

    // Valor (escalar) de la orientación ACTUAL (sobre el eje Y).
    [SerializeField] private float _orientacion;
    public float Orientacion
    {
        get { return _orientacion; }
        set
        {
            _orientacion = MapToRange(value, LIM_INFERIOR_ANGULO, LIM_SUPERIOR_ANGULO);
        }
    }

    // Valor de velocidad angular (escalar) máximo que puede alcanzar el agente.
    [SerializeField] private float _rotacionMaxima;
    public float RotacionMaxima
    {
        get { return _rotacionMaxima; }
        set
        {
            // No tiene sentido que el valor sea negativo.
            if (value < 0)
            {
                _rotacionMaxima = 0;
                return;
            }

            _rotacionMaxima = value;
        }
    }

    // Valor (escalar) que representa la velocidad angular del agente
    [SerializeField] private float _rotacion;
    public float Rotacion
    {
        get { return _rotacion; }
        set
        {
            float magnitudRotacion = Mathf.Abs(value);
            float sentidoRotacion = value / magnitudRotacion;

            // Si se supera la velocidad angular máxima, se ajusta la velocidad angular a dicha velocidad angular máxima.
            if (magnitudRotacion > RotacionMaxima)
            {
                _rotacion = RotacionMaxima * sentidoRotacion;
                return;
            }

            _rotacion = value;
        }
    }

    // Valor de aceleración angular (escalar) máximo que puede alcanzar el agente.
    [SerializeField] private float _angularMaxima;
    public float AngularMaxima
    {
        get { return _angularMaxima; }
        set
        {
            // No tiene sentido que el valor sea negativo.
            if (value < 0)
            {
                _angularMaxima = 0;
                return;
            }

            _angularMaxima = value;
        }
    }

    // Valor (escalar) que representa la aceleración angular del agente.
    [SerializeField] private float _angular;
    public float Angular
    {
        get { return _angular; }
        set
        {
            // Si se supera la aceleración angular máxima, se ajusta la aceleración angular a dicha aceleración angular máxima.
            if (Mathf.Abs(value) > AngularMaxima)
            {
                // Si el valor es negativo, se ajusta a la aceleración angular máxima negativa, si no, se le asigna la positiva.
                if (value < 0)
                {
                    _angular = -AngularMaxima;
                    return;
                }
                else
                {
                    _angular = AngularMaxima;
                    return;
                }
            }

            _angular = value;
        }
    }

    // Valor que representa la posición del agente.
    public Vector3 Posicion
    {
        get { return transform.position; }
        set { transform.position = value; }
    }

    /*
     * Función auxiliar.
     * Dado un número real x, calcula dónde estaría entre un límite inferior y un
     * límite superior y lo devuelve.
     */
    public static float MapToRange(float x, float limiteInferior, float limiteSuperior)
    {
        if (limiteInferior > limiteSuperior)
        {
            throw new System.Exception("El límite inferior del intervalo no puede ser mayor" +
                " que el límite superior.");
        }

        float tamIntervalo = limiteSuperior - limiteInferior;
        float xEnIntervalo = x % tamIntervalo;

        if (xEnIntervalo < 0)
        {
            xEnIntervalo += tamIntervalo;
        }

        return xEnIntervalo + limiteInferior;
    }

    /*
     * Retorna el ángulo formado dada una posición.
     */
    public static float PositionToAngle(Vector3 posicion)
    {
        float longCatetoOpuesto = posicion.x;
        float longCatetoContiguo = posicion.z;

        return Mathf.Atan2(longCatetoOpuesto, longCatetoContiguo);
    }

    /*
     * Método de clase; equivalente a PositionToAngle(this.Posicion).
     */
    public float PositionToAngle()
    {
        return PositionToAngle(Posicion);
    }

    /// <summary>
    /// Dada una orientación, devuelve el vector que forma un ángulo de dicha orientación con el eje de coordenadas(z, x).
    /// </summary>
    /// <param name="orientacion"></param>
    /// <returns></returns>
    public static Vector3 OrientationToVector(float orientacion)
    {
        float posX = Mathf.Sin(orientacion * Mathf.Deg2Rad);
        float posZ = Mathf.Cos(orientacion * Mathf.Deg2Rad);
        return new Vector3(posX, 0, posZ);
    }

    /// <summary>
    /// Devuelve el vector que representa la orientación del Agente. Está formado por (seno(Orientacion), 0, coseno(Orientacion)).
    /// </summary>
    /// <returns></returns>
    public Vector3 OrientationToVector()
    {
        return OrientationToVector(Orientacion);
    }

    /*
     * Devuelve la orientación en el rango [-180, 180] del personaje.
     */
    public float Heading180()
    {
        return MapToRange(Orientacion, -180, 180);
    }

    /*
     * Devuelve la orientación en radianes.
     */
    public float HeadingRadianes()
    {
        return Orientacion * Mathf.Deg2Rad;
    }
}