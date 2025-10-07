using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : SteeringBehaviour
{
    /*
     * Para Seek, el m�todo GetSteering se comporta de la siguiente manera:
     * 
     *  (1) Se calcula el vector que une los Agentes agent y target (newDirection).
     *  
     *  (2) Construye un vector de aceleraci�n (aceleracion) con:
     *      - La misma direcci�n que el vector calculado en (1).
     *      - Magnitud = agent.AceleracionMaxima.
     *      
     *  (3) Construye el objeto Steering con:
     *      - lineal: la aceleraci�n calculada en (2).
     *      - angular = 0.
     */
    public override Steering GetSteering(AgentNPC agent)
    {
        // (1)
        Vector3 newDirection = target.Posicion - agent.Posicion;

        // (2)
        Vector3 aceleracion = newDirection.normalized * agent.AceleracionMaxima;

        // (3)
        Steering steer = new Steering();
        steer.Lineal = aceleracion;
        steer.Angular = 0;
        return steer;
    }
}
