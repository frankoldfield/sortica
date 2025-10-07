using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeavePursue : MonoBehaviour
{
    public GameObject personaje_leave,
        perseguidor_leave,
        personaje_pursue,
        personaje_wander;

    /// <summary>
    /// Asigna...
    ///     - A personaje_evade, target = "perseguidor_evade".
    ///     - A perseguidor_evade, target = "personaje_evade".
    ///     - A personaje_pursue, target = "personaje_wander".
    ///     - A personaje_wander, nada.
    /// </summary>
    void Start()
    {
        ArbitroLeave leave = personaje_leave.GetComponent<ArbitroLeave>();
        leave.setTargets(perseguidor_leave.GetComponent<Agente>());

        ArbitroSeek seek = perseguidor_leave.GetComponent<ArbitroSeek>();
        seek.setTargets(personaje_leave.GetComponent<Agente>());

        ArbitroPursue pursue = personaje_pursue.GetComponent<ArbitroPursue>();
        ((Pursue)(pursue.behaviours[0].steeringBehaviour)).maxPrediction = 20f;
        pursue.setTargets(personaje_wander.GetComponent<Agente>());

        
    }
}
