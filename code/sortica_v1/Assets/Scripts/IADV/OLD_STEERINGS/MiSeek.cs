using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiSeek : SteeringBehaviour
{
    
    /*
     * Este SteeringBehaviour es, en esencia, el Seek de Craig Reynolds (diapositiva 13, tema 5).
     * Los pasos para calcular el Steering son:
     * 
     *      (1) Se halla el vector que une los Agentes agent y target (newDirection).
     *      
     *      (2) Se define la velocidad deseada (velocidadDeseada) como un vector con:
     *          - La direcci�n igual a newDirection.
     *          - La magnitud igual a la velocidad m�xima del Agente agent.
     *          
     *      (3) Aplicando las f�rmulas de Newton, se halla la aceleraci�n:
     *          v = v_0 + a * t --> velocidadDeseada = v_0 + a * t -->
     *          --> a * t = velocidadDeseada - v_0 --> a = (velocidadDeseada - v_0) / t
     *          Esta aceleración se ajusta a la aceleración máxima del agente.
     *          
     *      (4) Se construye un Steering (steer) con:
     *          - lineal igual a la aceleraci�n calculada en (3).
     *          - angular = 0.
     */
    public override Steering GetSteering(AgentNPC agent)
    {
        // (1)
        Vector3 newDirection = target.Posicion - agent.Posicion;

        // (2)
        Vector3 velocidadDeseada = newDirection.normalized * agent.VelocidadMaxima;

        // (3)
        Vector3 nuevaAceleracion = (velocidadDeseada - agent.Velocidad) / Time.deltaTime;

        if(nuevaAceleracion.magnitude > agent.AceleracionMaxima)
        {
            nuevaAceleracion = nuevaAceleracion.normalized * agent.AceleracionMaxima;
        }

        // (4)
        Steering steer = new Steering();
        steer.Lineal = nuevaAceleracion;
        steer.Angular = 0;
        return steer;
    }
}
