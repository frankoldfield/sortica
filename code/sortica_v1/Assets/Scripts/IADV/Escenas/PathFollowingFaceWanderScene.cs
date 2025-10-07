using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Clase que se encarga de controlar la escena de prueba de PathFollowing+Face y Wander.
public class PathFollowingFaceWanderScene : MonoBehaviour
{
    public GameObject[] nodos;
    public GameObject pathFollowingFace;
    public GameObject wander;

    private void Start()
    {
        StartCoroutine(EsperarYAsignarTargets());
    }

    private IEnumerator EsperarYAsignarTargets()
    {
        yield return new WaitForSeconds(0.1f);

        object[] objetos = new object[1 + nodos.Length];
        objetos[0] = wander.GetComponent<AgentNPC>();
        for (int i = 0; i < nodos.Length; i++)
        {
            objetos[i + 1] = nodos[i];
        }

        pathFollowingFace.GetComponent<ArbitroPathFollowingFace>().setTargets(objetos);
    }
}
