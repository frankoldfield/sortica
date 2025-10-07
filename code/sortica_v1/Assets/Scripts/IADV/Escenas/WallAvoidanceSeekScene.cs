using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallAvoidanceSeekScene : MonoBehaviour
{
    public GameObject player;
    public GameObject unBigote;
    public GameObject dosBigotes;
    public GameObject tresBigotes;

    private void Start()
    {
        StartCoroutine(EsperarYAsignarTargets());
    }

    private IEnumerator EsperarYAsignarTargets()
    {
        yield return new WaitForSeconds(0.1f);

        unBigote.GetComponent<ArbitroWallAvoidanceSeek>().setTargets(player.GetComponent<AgentPlayer>());
        dosBigotes.GetComponent<ArbitroWallAvoidanceSeek>().setTargets(player.GetComponent<AgentPlayer>());
        tresBigotes.GetComponent<ArbitroWallAvoidanceSeek>().setTargets(player.GetComponent<AgentPlayer>());

        unBigote.GetComponent<WallAvoidance>().numBigotes = 1;
        dosBigotes.GetComponent<WallAvoidance>().numBigotes = 2;
        tresBigotes.GetComponent<WallAvoidance>().numBigotes = 3; // Esto lo tiene por defecto, pero se pone por dejarlo claro.
    }
}
