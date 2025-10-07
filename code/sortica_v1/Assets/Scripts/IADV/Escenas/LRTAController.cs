using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Distances;

public class LRTAController : MonoBehaviour
{
    public GameObject npc;
    public GameObject scene;
    public GameObject pathPrefab;
    public GameObject obstaclePrefab;
    public GameObject goalPrefab;
    public GameObject goal;
    public bool debug;
    public float localSpaceDepth;
    public Distancia Distance_metric;

    private void Start()
    {
        StartCoroutine(EsperarYAsignarTargets());
    }

    private IEnumerator EsperarYAsignarTargets()
    {
        yield return new WaitForSeconds(0.1f);

        npc.GetComponent<ArbitroLRTA>().setTargets(pathPrefab, obstaclePrefab, goalPrefab, goal, debug, localSpaceDepth, Distance_metric, scene.GetComponent<Grid>());
        npc.GetComponent<LRTAStar>().Empezar();
    }
}
