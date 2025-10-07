using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SteeringBehaviour : MonoBehaviour
{
    // Agente objetivo.
    public Agente target = null;

    /*
     * El método GetSteering se implementará en las clases hijas de SteeringBehaviour. En este método, cada
     * uno de los SteeringBehaviour devolverá un objeto Steering calculado de diferente manera.
     */
    public abstract Steering GetSteering(AgentNPC agent);
}
