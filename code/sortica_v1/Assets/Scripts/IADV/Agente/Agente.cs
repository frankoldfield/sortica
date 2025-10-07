using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Agente : Bodi
{

    protected void Awake()
    {
        Orientacion = MapToRange(transform.eulerAngles.y, 0, 360);
    }

    // Distancia mínima que tiene que exitir entre diferentes propiedades del agente (ángulo o radios, por ejemplo).
    [SerializeField] private static float DISTANCIA_MINIMA = 2f;

    // Valor mínimo para el radio interior.
    private static float _radioInteriorMinimo = 0.01f;
    public float RadioInteriorMinimo
    {
        get { return _radioInteriorMinimo; }
        set
        {
            // No nos interesa que el valor sea más pequeño que 0.01.
            if (value <= 0.01)
            {
                _radioInteriorMinimo = 0.01f;
                return;
            }

            _radioInteriorMinimo = value;
        }
    }

    // Valor del radio interior o de colisión del agente.
    [SerializeField] private float _radioInterior;
    public float RadioInterior
    {
        get { return _radioInterior; }
        set
        {
            // Si el nuevo radio interior es menor que el mínimo, no se actualiza.
            if (value <= RadioInteriorMinimo)
            {
                _radioInterior = RadioInteriorMinimo;
            }
            else
            {
                _radioInterior = value;
            }

            // Si fuese necesario, ajustar el radio exterior (tiene que cumplir con la distancia mínima y con ser el exterior, claro está).
            if (RadioExterior < _radioInterior + DISTANCIA_MINIMA)
            {
                RadioExterior = _radioInterior + DISTANCIA_MINIMA;
            }
        }
    }

    // Valor del radio exterior o de llegada del agente.
    [SerializeField] private float _radioExterior;
    public float RadioExterior
    {
        get { return _radioExterior; }
        set
        {
            // Si el nuevo radio exterior incumple la distancia mínimo entre radios, se ajusta.
            if (value < RadioInterior + DISTANCIA_MINIMA)
            {
                _radioExterior = RadioInterior + DISTANCIA_MINIMA;
            }
            else
            {
                _radioExterior = value;
            }
        }
    }

    // Valor que representa el ángulo interior o de colisión.
    [SerializeField] private float _anguloInterior = 30f;
    public float AnguloInterior
    {
        get { return _anguloInterior; }
        set
        {
            _anguloInterior = value;

            // Si fuese necesario, ajustar el ángulo exterior.
            if (AnguloExterior < _anguloInterior + DISTANCIA_MINIMA)
            {
                AnguloExterior = _anguloInterior + DISTANCIA_MINIMA;
            }
        }
    }

    // Valor que representa el ángulo exterior o de llegada.
    [SerializeField] private float _anguloExterior = 45f;
    public float AnguloExterior
    {
        get { return _anguloExterior; }
        set
        {
            // Si el nuevo ángulo exterior incumple la distancia mínima entre ángulos, se ajusta.
            if (value < AnguloInterior + DISTANCIA_MINIMA)
            {
                _anguloExterior = AnguloInterior + DISTANCIA_MINIMA;
            }
            else
            {
                _anguloExterior = value;
            }
        }
    }

    // Valores booleanos para debug, indica si deben dibujarse los radios y los ángulos, respectivamente.
    public bool DibujarRadios = false;
    public bool DibujarAngulos = false;
    public bool DibujarVelocidad = false;
    public bool DibujarAceleracion = false;
    // Valor que representa cómo de larga será la línea que dibuja los ángulos.
    [SerializeField] private static float LONGITUD_GIZMO_ANGULO = 10f;

    /*
    * Método que se encarga de dibujar los gizmos.
    */
    private void OnDrawGizmos()
    {

        if (DibujarRadios)
        {
            // Radio exterior.
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, RadioExterior);

            // Radio interior.
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, RadioInterior);
        }

        if (DibujarAngulos)
        {
            // �ngulo de visi�n exterior.

            // Hacia la derecha.
            float angulo = Orientacion + AnguloExterior / 2;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position + new Vector3(0, alturaPersonaje, 0), hallarPosicionFinalRecta(angulo) + new Vector3(0, alturaPersonaje, 0));

            // Hacia la izquierda.
            angulo = Orientacion - AnguloExterior / 2;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position + new Vector3(0, alturaPersonaje, 0), hallarPosicionFinalRecta(angulo) + new Vector3(0, alturaPersonaje, 0));
            // �ngulo de visi�n interior.

            // Hacia la derecha.
            angulo = Orientacion + AnguloInterior / 2;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position + new Vector3(0, alturaPersonaje, 0), hallarPosicionFinalRecta(angulo) + new Vector3(0, alturaPersonaje, 0));

            // Hacia la izquierda.
            angulo = Orientacion - AnguloInterior / 2;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position + new Vector3(0, alturaPersonaje, 0), hallarPosicionFinalRecta(angulo) + new Vector3(0, alturaPersonaje, 0));
        }

        if(DibujarVelocidad)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(Posicion + new Vector3(0, alturaPersonaje, 0), Posicion + Velocidad + new Vector3(0, alturaPersonaje, 0));
        }

        if (DibujarAceleracion)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(Posicion + new Vector3(0, alturaPersonaje, 0), Posicion + Aceleracion + new Vector3(0, alturaPersonaje, 0));
        }

    }

    /*
    * Función auxiliar.
    * Se encarga de hallar la posición final de una recta.
    */
    private Vector3 hallarPosicionFinalRecta(float angulo)
    {
        float posX = Mathf.Sin(angulo * Mathf.Deg2Rad);
        float posZ = Mathf.Cos(angulo * Mathf.Deg2Rad);
        return transform.position + LONGITUD_GIZMO_ANGULO * new Vector3(posX, transform.position.y, posZ);
    }

}