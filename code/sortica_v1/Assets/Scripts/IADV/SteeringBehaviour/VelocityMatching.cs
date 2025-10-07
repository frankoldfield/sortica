using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityMatching : SteeringBehaviour
{

    /*
     * El objetivo de VelocityMatching es alcanzar la velocidad del target. Para ello, se siguen los siguientes
     * pasos:
     * 
     *      (1) Se halla la diferencia de velocidad entre target y agente (diferenciaVelocidad).
     *      
     *      (2) Se calcula la aceleración que habría que aplicarle a agent para que alcanzase la velocidad del
     *      target (deltaVelocidad).
     *      
     *      (3) Se construye y devuelve un objeto Steering con:
     *          - lineal igual a la aceleración calculada en (2).
     *          - angular = 0.
     */
    public override Steering GetSteering(AgentNPC agent)
    {
        // (1)
        Vector3 diferenciaVelocidad = target.Velocidad - agent.Velocidad;

        // (2)
        Vector3 deltaVelocidad = diferenciaVelocidad/Time.deltaTime;

        // (3)
        Steering steer = new Steering();

        if (deltaVelocidad.magnitude > agent.AceleracionMaxima)
        {
            steer.Lineal = deltaVelocidad.normalized * agent.AceleracionMaxima;
        }
        else {
            steer.Lineal = deltaVelocidad;
        }
        steer.Angular = 0;

        return steer;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public Steering GetSteering(AgentNPC agent)
    //{

    //}
}
