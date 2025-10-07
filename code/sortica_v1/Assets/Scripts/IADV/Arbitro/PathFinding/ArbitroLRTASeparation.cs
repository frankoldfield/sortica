using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Distances;

// Árbitro que controla el comportamiento de Flee
public class ArbitroLRTASeparation : ArbitroPonderado
{
    public override void create() {
        LRTAStar lrtastar = gameObject.AddComponent<LRTAStar>();
        Separation separation = gameObject.AddComponent<Separation>();
        behaviours = new BehaviourPeso[2];
        behaviours[0].steeringBehaviour = lrtastar;
        behaviours[0].peso = 1f;
        separation.threshold = 10;
        separation.decayCoefficient = 100;
        behaviours[1].steeringBehaviour = separation;
        behaviours[1].peso = 2f;
    }

    public override void delete() {
        Destroy(gameObject.GetComponent<LRTAStar>());
        behaviours = null;
    }

    public override void setTargets(params object[] targets)
    {
        LRTAStar LRTAbehaviour = ((LRTAStar)behaviours[0].steeringBehaviour);
        LRTAbehaviour.pathPrefab = (GameObject) targets[0];
        LRTAbehaviour.obstaclePrefab = (GameObject)targets[1];
        LRTAbehaviour.goalPrefab = (GameObject) targets[2];
        LRTAbehaviour.goal = (GameObject) targets[3];
        LRTAbehaviour.debug = (bool) targets[4];
        LRTAbehaviour.localSpaceDepth = (float) targets[5];
        LRTAbehaviour.Distance_metric = (Distancia) targets[6];
        LRTAbehaviour.grid = (Grid)targets[7];

        Separation separation = ((Separation)behaviours[1].steeringBehaviour);
        Agente[] subordinados = new Agente[targets.Length - 8];
        for(int i = 8; i < targets.Length; i++)
        {
            subordinados[i - 8] = targets[i] as Agente;
        }
        separation.Targets = subordinados;
    }
}