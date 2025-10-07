using System.Collections;
using UnityEngine;

// Clase que se encarga de controlar la escena de prueba de Seek, Flee y Arrive.
public class SeekFleeArriveScene : MonoBehaviour
{
    public GameObject player;
    public GameObject seek;
    public GameObject flee;
    public GameObject arrive;

    private void Start()
    {
        StartCoroutine(EsperarYAsignarTargets());
    }

    private IEnumerator EsperarYAsignarTargets()
    {
        yield return new WaitForSeconds(0.1f);

        seek.GetComponent<ArbitroSeek>().setTargets(player.GetComponent<AgentPlayer>());
        flee.GetComponent<ArbitroFlee>().setTargets(player.GetComponent<AgentPlayer>());
        arrive.GetComponent<ArbitroArrive>().setTargets(player.GetComponent<AgentPlayer>());
    }
}
