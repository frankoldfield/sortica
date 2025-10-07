using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Árbitro que controla el comportamiento de ArbitroPathFollower+Face
public class ArbitroPathFollowingFace : ArbitroPonderado
{
    public override void create() {
        PathFollowing pathFollowing = gameObject.AddComponent<PathFollowing>();
        Face face = gameObject.AddComponent<Face>();
        behaviours = new BehaviourPeso[2];
        behaviours[0].steeringBehaviour = pathFollowing;
        behaviours[0].peso = 1f;
        behaviours[1].steeringBehaviour = face;
        behaviours[1].peso = 1f;
    }

    public override void delete() {
        Destroy(gameObject.GetComponent<PathFollowing>());
        Destroy(gameObject.GetComponent<Face>());
        behaviours = null;
    }

    public override void setTargets(params object[] targets)
    {
        Face faceBehaviour = behaviours[1].steeringBehaviour as Face;
        PathFollowing pathFollowingBehaviour = behaviours[0].steeringBehaviour as PathFollowing;

        faceBehaviour.targetDelegado = targets[0] as Agente;

        pathFollowingBehaviour.nodes = new GameObject[targets.Length - 1];

        for (int i = 1; i < targets.Length; i++)
        {
            if (targets[i] is GameObject go)
            {
                pathFollowingBehaviour.nodes[i - 1] = go;
            }
        }
    }
}
