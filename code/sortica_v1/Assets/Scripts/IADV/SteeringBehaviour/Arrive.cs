using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrive : SteeringBehaviour
{

    // Atributo que establece el tiempo, en segundos, en el que se debería completar el movimiento.
    [SerializeField] private float timeToTarget;

    /*
     * El método GetSteering para Arrive tiene el siguiente comportamiento:
     *      
     *      (1) Calcula el vector que une los Agentes agent y target (newDirection), así como la distancia que
     *      los separa (distance).
     *      
     *      (2) Calcula la velocidad (tomada como magnitud) deseada (targetSpeed) con el siguiente criterio:
     *          
     *          (2.1) Si agent se encuentra dentro del radio interior de target (distance < target.radioInterior),
     *          la velocidad objetivo es 0.
     *          
     *          (2.2) Si no, si agent se encuentra dentro del radio exterior de target 
     *          (target.RadioInterior < distance < target.RadioExterior), entonces la velocidad objetivo es una
     *          proporción de la velocidad máxima del agente:
     *              targetSped = agent.VelocidadMaxima * distance / target.RadioExterior.
     *              
     *          (2.3) Si no se encuentra dentro de ninguno de los radios, la velocidad objetivo es la velocidad máxima
     *          del agente.
     *          
     *      (3) Construye un vector targetVelocity con:
     *          - La misma dirección que newDirection,
     *          - Magnitud igual a targetSpeed.
     *          
     *      (4) ** Se calcula la aceleración como:
     *          targetVelocity = v_0 + a * timeToTarget --> ... -> a = (targetVelocity - v_0) / timeToTarget
     *      
     *      (5) Se construye un objeto Steering con:
     *          - lineal igual a la aceleración calculada en (4).
     *          - angular = 0.
     *      
     *      ** Véase que se comprueba si timeToTarget es <= 0 para, en ese caso, que se use Time.deltaTime.
     *      Esto dota de coherencia, pues no tiene sentido establecernos llegar a un objetivo en tiempo menor
     *      o igual a 0, así que, como mínimo, tendría que ser en un frame (Time.deltaTime).
     */
    public override Steering GetSteering(AgentNPC agent)
    {
        // (1)
        Vector3 newDirection = target.Posicion - agent.Posicion;
        float distance = newDirection.magnitude;

        // (2)
        float targetSpeed;

        // (2.1)
        if(distance < this.target.RadioInterior)
        {
            targetSpeed = 0;
        }
        
        // (2.2)
        else if(distance < this.target.RadioExterior)
        {
            targetSpeed = agent.VelocidadMaxima * distance / this.target.RadioExterior;
        }
        
        // (2.3)
        else
        {
            targetSpeed = agent.VelocidadMaxima;
        }

        // (3)
        Vector3 targetVelocity = (newDirection.normalized) * targetSpeed;

        // (4)
        float timeToTarget = this.timeToTarget;
        if (this.timeToTarget == 0)
        {
            timeToTarget = Time.deltaTime;
        }
        Vector3 aceleracionLineal = (targetVelocity - agent.Velocidad) / timeToTarget;

        if(aceleracionLineal.magnitude > agent.AceleracionMaxima)
        {
            aceleracionLineal = aceleracionLineal.normalized * agent.AceleracionMaxima;
        }

        // (5)
        Steering steer = new Steering();
        steer.Lineal = aceleracionLineal;
        steer.Angular = 0;
        return steer;

    }
}
