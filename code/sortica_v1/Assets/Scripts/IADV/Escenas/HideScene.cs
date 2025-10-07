using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideScene : MonoBehaviour
{
    public GameObject agente;
    public GameObject hide;
    
    private void Start()
    {
        StartCoroutine(EsperarYAsignarTargets());
    }

    private IEnumerator EsperarYAsignarTargets()
    {
        yield return new WaitForSeconds(0.1f);

        hide.GetComponent<ArbitroHide>().setTargets(agente.GetComponent<AgentPlayer>());
    }
}
