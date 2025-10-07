using System.Collections;
using UnityEngine;

// Clase que se encarga de controlar la escena de prueba de Align y AntiAlign.
public class AlignAntiAlignScene : MonoBehaviour
{
    public GameObject player;
    public GameObject align;
    public GameObject antiAlign;

    private void Start()
    {
        StartCoroutine(EsperarYAsignarTargets());
    }

    private IEnumerator EsperarYAsignarTargets()
    {
        yield return new WaitForSeconds(0.1f);

        align.GetComponent<ArbitroAlign>().setTargets(player.GetComponent<AgentPlayer>());
        antiAlign.GetComponent<ArbitroAntiAlign>().setTargets(player.GetComponent<AgentPlayer>());
    }
}
