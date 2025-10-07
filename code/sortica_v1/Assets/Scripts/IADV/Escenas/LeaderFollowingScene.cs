using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderFollowingScene : MonoBehaviour
{
    public GameObject[] followers = new GameObject[9];
    public GameObject leader;

    private void Start()
    {
        StartCoroutine(EsperarYAsignarTargets());
    }

    private IEnumerator EsperarYAsignarTargets()
    {
        yield return new WaitForSeconds(0.1f);

        Agente[] targets = new Agente[followers.Length + 1];
        targets[0] = leader.GetComponent<AgentPlayer>();
        for(int i=0; i<followers.Length; i++)
        {
            targets[i+1] = followers[i].GetComponent<AgentNPC>();
        }

        for(int i=0; i<followers.Length; i++)
        {
            followers[i].GetComponent<ArbitroLeaderFollowing>().setTargets(targets);
        }
    }
}