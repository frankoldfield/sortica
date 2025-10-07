using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : Face
{
    private new void Awake()
    {
        base.Awake();
        GameObject objeto = new GameObject();
        objeto.SetActive(false);
        targetDelegado = objeto.AddComponent<Agente>();
    }

    new void OnDestroy()
    {
        if(targetDelegado != null)
            Destroy(targetDelegado.gameObject);
        base.OnDestroy();
    }

    /*
     * Atributos:
     *      - wanderOffset: distancia entre el Agente y el círculo "de decisión".
     *      - wanderRadius: radio del círculo.
     *      - wanderRate: rate máximo de cambio para la orientación. Entre 0 y 1 (porcentaje).
     *      - wanderOrientation: la orientación actual del target Wander.
     */

    [SerializeField] private float wanderRadius = 5;
    //[SerializeField] private float wanderOffset = 0.5f;
    private float wanderOffset;
    [SerializeField] private float wanderRate = 5f;
    private float wanderOrientation = 0f;

    /*
     * Wander imagina un círculo delante del Agente a una distancia wanderOffset (distancia desde el agente
     * al centro del círculo). En cada frame, se toma una orientación aleatoria en el rango [-wanderRate, wanderRate],
     * de forma que, en cada frame, el cambio de orientación sea, como mucho, de wanderRateº. Con esa orientación, se
     * calcula, usando el radio del círculo, el punto al que ha de mirar el personaje. Una vez hallado ese punto, se
     * utiliza Face para hallar el steering que hará que el agente mire a ese punto, y se aplicará aceleración máxima
     * en la dirección en la que mira el personaje.
     * 
     * Los pasos a seguir son:
     * 
     *      (1) Actualizar el atributo wanderOrientation. Para ello, se toma un real aleatorio en el rango
     *      [-wanderRate, wanderRate] y se suma al anterior wanderOrientaion.
     *      
     *      (2) Se calcula la orientación objetivo del agente, que es la suma de su orientación actual y
     *      wanderOrientation.
     *      
     *      (3) Se calcula el centro del círculo:
     *                                    _______
     *                                   |       |
     *      agent.Position * ----------------*   |
     *                      wanderOffset |_______|
     *                      
     *      (4) Se calcula la nueva posición a la que se dirigirá agent.
     *      
     *      (5) Se actualiza el targetDelegado.
     *      
     *      (6) Se delega en Face y se devuelve un Steering con:
     *          - angular calculado en Face.
     *          - lineal un vector con:
     *              - Dirección en la que va agent (como un forward).
     *              - Magnitud igual a la aceleración máxima de agent.
     */
    public override Steering GetSteering(AgentNPC agent)
    {

        // Para mantener el WanderOffset de modo que el círculo esté justo delante del Agente. Se supone que el radio del Agente es 1.
        wanderOffset = wanderRadius + 1f;

        // (1)
        wanderOrientation += Random.Range(-wanderRate, wanderRate);

        // (2)
        float targetOrientation = agent.Orientacion + wanderOrientation;

        // (3)
        Vector3 centroCirculo = agent.Posicion + agent.OrientationToVector().normalized * wanderOffset;

        // (4)
        Vector3 nuevaPosicion = centroCirculo + wanderRadius * Bodi.OrientationToVector(targetOrientation).normalized;

        // (5)
        targetDelegado.Posicion = nuevaPosicion;

        // (6)
        Steering steering = base.GetSteering(agent);
        steering.Lineal = agent.AceleracionMaxima * agent.OrientationToVector().normalized;
        return steering;
    }

    public bool dibujarCirculo = false;

    private void OnDrawGizmos()
    {
        if(!dibujarCirculo)
        {
            return;
        }

        Agente agent = GetComponent<Agente>();
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(agent.Posicion + agent.OrientationToVector().normalized * wanderOffset, wanderRadius);
    }
}
