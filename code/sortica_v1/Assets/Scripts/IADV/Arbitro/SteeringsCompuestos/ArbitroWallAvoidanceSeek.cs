using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ArbitroWallAvoidanceSeek : ArbitroPonderado
{
    public override void create()
    {
        behaviours = new BehaviourPeso[2];
        WallAvoidance wallAvoidance = gameObject.AddComponent<WallAvoidance>();
        Seek seek = gameObject.AddComponent<Seek>();
        behaviours[0].steeringBehaviour = seek;
        behaviours[0].peso = 1f;
        behaviours[1].steeringBehaviour = wallAvoidance;
        behaviours[1].peso = 10f;
    }

    public override void delete()
    {
        Destroy(gameObject.GetComponent<WallAvoidance>());
        Destroy(gameObject.GetComponent<Seek>());
    }

    public override void setTargets(params object[] targets) {
        behaviours[0].steeringBehaviour.target = (Agente) targets[0];
    }
}
