using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArbitroLeaderFollowing : ArbitroPonderado
{

    /*
     * Estructura de los behaviours:
     * {
     *      0: Arrive,
     *      1: Separation,
     *      2: Flee
     * }
     */


    /*
     * El l�der following debe tener los siguientes steering behaviours:
     * 
     *      - Arrive para moverse hacia el l�der y reducir la velocidad
     *      gradualmente.
     *      
     *      - Evade/Flee para que, si el personaje est� en el camino del
     *      l�der, se quite.
     *      
     *      - Separation: para que los subordinados no se den.
     *      
     *      
     * Orden de los targets:
     * 
     *      (1) L�der: targets[0].
     *      (2) Subordinados: targets[1:].
     *      
     */
    public override void create()
    {
        behaviours = new BehaviourPeso[3];

        /*
         * Arrive: el target es el l�der.
         */
        Arrive arrive = gameObject.AddComponent<Arrive>();
        behaviours[0].steeringBehaviour = arrive;
        behaviours[0].peso = 5;

        /*
         * Separation: los targets son los subordinados para no empujarse.
         */
        Separation separation = gameObject.AddComponent<Separation>();
        separation.threshold = 10;
        separation.decayCoefficient = 10000;
        behaviours[1].steeringBehaviour = separation;
        behaviours[1].peso = 4;

        /*
         * Flee: el target es el l�der para no ponerse en su camino.
         */
        Flee flee = gameObject.AddComponent<Flee>();
        behaviours[2].steeringBehaviour = flee;
        behaviours[2].peso = 3f;
    }

    public override void delete()
    {
        foreach (BehaviourPeso behaviour in behaviours)
        {
            Destroy(behaviour.steeringBehaviour);
        }
    }

    public override void setTargets(params object[] targets) {
        Agente lider = targets[0] as Agente;

        Agente[] subordinados = new Agente[targets.Length - 1];
        for(int i = 1; i < targets.Length; i++)
        {
            subordinados[i - 1] = targets[i] as Agente;
        }

        Arrive arrive = behaviours[0].steeringBehaviour as Arrive;
        arrive.target = lider;

        Separation separation = behaviours[1].steeringBehaviour as Separation;
        separation.Targets = subordinados;

        Flee flee = behaviours[2].steeringBehaviour as Flee;
        flee.target = lider;
    }
}
