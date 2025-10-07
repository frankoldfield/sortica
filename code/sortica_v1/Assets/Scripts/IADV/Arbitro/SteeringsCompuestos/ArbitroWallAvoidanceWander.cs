using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ArbitroWallAvoidanceWander : ArbitroPonderado
{
    public override void create()
    {
        behaviours = new BehaviourPeso[2];
        WallAvoidance wallAvoidance = gameObject.AddComponent<WallAvoidance>();
        Wander wander = gameObject.AddComponent<Wander>();
        //MirarHaciaDelante mirarHaciaDelante = gameObject.AddComponent<MirarHaciaDelante>();
        behaviours[0].steeringBehaviour = wander;
        behaviours[0].peso = 1f;
        behaviours[1].steeringBehaviour = wallAvoidance;
        behaviours[1].peso = 2f;
        //behaviours[2].steeringBehaviour = mirarHaciaDelante;
        //behaviours[2].peso = 10f;
    }

    public override void delete()
    {
        Destroy(gameObject.GetComponent<WallAvoidance>());
        Destroy(gameObject.GetComponent<Wander>());
        //Destroy(gameObject.GetComponent<MirarHaciaDelante>());
    }

    public override void setTargets(params object[] targets) {}
}
