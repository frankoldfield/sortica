using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AgentNPC : Agente
{
    // Steering que se aplicará para el movimiento final. Sus atributos se calcularán mediante el árbitro.
    Steering steer = null;

    protected new void Awake()
    {
        base.Awake();
    }

    /*
     * En el método Update se aplica el Steering calculado al final del frame anterior mediante el m�todo
     * ApplySteering().
     */
    protected void Update()
    {
        ApplySteering();
    }

    /*
     * En LateUpdate se calcula el steering que, en el siguiente frame, se aplicará. Aquí es cuando el árbitro
     * entra en juego. Se tiene que buscar el árbitro todos los frames porque puede cambiar en tiempo de ejecución.
     */
    private void LateUpdate()
    {
        SteeringBehaviour steeringBehaviour = GetComponent<SteeringBehaviour>();
        if(steeringBehaviour != null)
        {
            steer = steeringBehaviour.GetSteering(this);
        }
    }

    /*
     * ApplySteering usa el atributo "steer" para realizar el movimiento. Este atributo, instancia de Steering,
     * tiene las acelerac�ones angular (angular) y lineal (lineal) que se aplican siguiendo las ecuaciones de
     * Newton (y teniendo en cuenta una aceleraci�n uniforme):
     *  
     *  Para el movimiento
     *  - v = v_0 + a * t
     *  - x = x_0 + v * t
     *  
     *  Para la rotaci�n
     *  - w = w_0 + alpha * t
     *  - theta = theta_0 + w * t
     */
    void ApplySteering()
    {
        if(steer is null)
        {
            return;
        }

        float time = Time.deltaTime;

        // La aceleraci�n lineal ser� la contenida en el atributo steer.
        Aceleracion = steer.Lineal;
        // v = v0 + at
        Velocidad = Velocidad + Aceleracion * time;
        // x = x0 + vt
        Posicion = Posicion + Velocidad * time;

        // La aceleraci�n angular ser�, de igual forma, la contenida en el atributo steer.
        Angular = steer.Angular;
        // w = w0 + alpha * t
        Rotacion = Rotacion + Angular * time;
        // orientacion = orientacion0 + w * t
        Orientacion = Orientacion + Rotacion * time;

        transform.rotation = new Quaternion();
        transform.Rotate(Vector3.up, Orientacion);
    }
}
