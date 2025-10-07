using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArbitroAndarRecto : ArbitroPonderado
{

    /*
     * Steerings:
     * 
     *  (1) Wander, peso = 1.
     *  (2) Wall Avoidance, peso = 1000.
     *  (3) MirarHaciaDelante, peso = 1000.
     *  
     */

    public override void create()
    {
        Wander wander = gameObject.AddComponent<Wander>();
        WallAvoidance wallAvoidance = gameObject.AddComponent<WallAvoidance>();
        MirarHaciaDelante mirar = gameObject.AddComponent<MirarHaciaDelante>();

        behaviours = new BehaviourPeso[3];

        behaviours[0].steeringBehaviour = wander;
        behaviours[0].peso = 10;

        behaviours[1].steeringBehaviour = wallAvoidance;
        behaviours[1].peso = 1000;

        behaviours[2].steeringBehaviour = mirar;
        behaviours[2].peso = 10;
    }

    public override void delete()
    {
        for(int i = 0; i< behaviours.Length; i++)
        {
            Destroy(behaviours[i].steeringBehaviour);
        }

        behaviours = null;
    }

    public override void setTargets(params object[] targets) {}
}
