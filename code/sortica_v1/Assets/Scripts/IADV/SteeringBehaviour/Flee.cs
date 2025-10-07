using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : SteeringBehaviour
{
    /*
     * Para Flee, el GetSteering tiene como objetivo realizar un Seek en la dirección contraria. Por tanto:
     * 
     *      (1) Se calcula el vector que une los Agentes target y agent (EN ESE ORDEN, ES DECIR, EL VECTOR QUE UNE
     *      TARGET CON AGENT).
     *      
     *      (2) Se construye un vector "lineal" con:
     *          - La misma dirección que el vector calculado en (1).
     *          - Magnitud igual a la aceleración máxima de agent.
     *          
     *      (3) Se construye y devuelve un objeto Steering con:
     *          - lineal igual al vector calculado en (2).
     *          - angular = 0.
     */
    public override Steering GetSteering(AgentNPC agent)
    {
        // (1)
        Vector3 newDirection = agent.Posicion - target.Posicion;

        // (2)
        Vector3 lineal = newDirection.normalized * agent.AceleracionMaxima;

        // (3)
        Steering steer = new Steering();
        steer.Lineal = lineal;
        steer.Angular = 0;
        return steer;
    }
}
