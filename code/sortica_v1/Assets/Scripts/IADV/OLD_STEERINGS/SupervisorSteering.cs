using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupervisorSteering : MiSeek
{
    /*
     * Atributos:
     *      
     *      - targetDelegado: el Target que tiene la clase PathFollowing. No es el mismo que el target que tiene la
     *      superclase, que se ir� creando/actualizando seg�n sea necesario (PathFollowing delega en Seek).
     *      
     *      - explicitTarget: el target que se le pasa a la superclase para que devuelva el Steering.
     *      
     *      - nodePositions: array de posiciones que forman el camino, se seleccionan desde el inspector
     *      
     *      - nodes: lista de nodos (targets) que se ir�n recorriendo
     *      
     *      - currentNode: nodo actual
     *      
     *      - pathDir: 1 o -1, es la direcci�n en la que se recorre el camino, cuando se llegue a un extremo se cambia de direcci�n
     */
    public Agente targetDelegado;
    protected Agente explicitTarget;
    public Vector3[] nodePositions;
    private Agente[] nodes;
    public int currentNode;
    private int pathDir = 1;

    /*
     * Awake lee la lista de posiciones del camino, y crea los nodos correspondientes, para que se pueda ir recorriendo la lista
     */
    protected void Awake()
    {
        GameObject objeto; 


        nodes = new Agente[nodePositions.Length];
        for (int i = 0; i < nodes.Length; i++)
        {
            objeto = new GameObject();
            objeto.SetActive(false);
            explicitTarget = objeto.AddComponent<AgentNPC>();
            explicitTarget.Posicion = nodePositions[i];
            nodes[i] = explicitTarget;
        }
    }

    /*
     *      (1) Cogemos el nodo actual
     *      
     *      (2) Calculamos la distancia a este nodo
     *      
     *      (3) Ahora veremos si hemos llegado al nodo actual y, en ese caso, cambiaremos de nodo (Todo esto para el siguiente frame)
     *      
     *          (3.1) Vemos si ha el agente ha llegado al nodo actual y en ese caso avanzamos al siguiente nodo en el recorrido (en direcci�n pathDir)
     *          
     *          (3.2) Si hemos llegado al �ltimo nodo entonces tenemos que cambiar de direcci�n, lo mismo pasa si hemos llegado al primero (al dar la vuelta) y nos hemos pasado
     *                  * Primero cambiamos de direcci�n (ya que hemos llegado a uno de los extremos del camino)
     *                  * Despu�s avanzamos en esa direcci�n
     *                  
     *      (4) Le pasamos el nodo actual como target al steering padre (seek), para que calcule un steering de seek hacia esa posici�n
     */
    public override Steering GetSteering(AgentNPC agent)
    {
        //(1)
        explicitTarget = nodes[currentNode];

        // (2)
        Vector3 newDirection = explicitTarget.Posicion - agent.Posicion;
        float distance = newDirection.magnitude;

        // (3)
        // (3.1)
        if (distance <= Mathf.Max(agent.RadioInterior, 1))
        {
            currentNode += pathDir;
        }

        // (3.2)
        if (currentNode >= nodes.Length || currentNode < 0)
        {
            pathDir = -pathDir;
            currentNode += pathDir;
        }
        // (4)
        base.target = explicitTarget;
        Steering steer = base.GetSteering(agent);
        return steer;
    }
}