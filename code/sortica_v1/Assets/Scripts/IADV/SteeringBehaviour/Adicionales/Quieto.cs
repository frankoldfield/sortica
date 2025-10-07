using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quieto : SteeringBehaviour
{
    /*
     * La idea es decelerar al personaje en caso de que esté moviéndose (tanto angular como linealmente).
     */
    public override Steering GetSteering(AgentNPC agent)
    {
        Steering steer = new Steering();

        // Lineal
        if(agent.Speed > 0)
        { 
            Vector3 velocidadObjetivo = new Vector3(0, 0, 0);

            Vector3 aceleracion = (velocidadObjetivo - agent.Velocidad) / Time.deltaTime;

            if(aceleracion.magnitude > agent.AceleracionMaxima)
            {
                aceleracion = aceleracion.normalized * agent.AceleracionMaxima;
            }

            steer.Lineal = aceleracion;
        }
        else
        {
            steer.Lineal = new Vector3(0, 0, 0);
        }

        // Angular
        if(Mathf.Abs(agent.Rotacion) > 0)
        {
            float rotacionObjetivo = 0;

            float angular = (rotacionObjetivo - agent.Rotacion) / Time.deltaTime;

            if(angular > agent.AngularMaxima)
            {
                angular = (angular / Mathf.Abs(angular)) * agent.AngularMaxima;
            }

            steer.Angular = angular;
        }
        else
        {
            steer.Angular = 0;
        }

        return steer;
    }
}
