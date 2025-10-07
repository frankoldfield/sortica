using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static Distances;
public class LRTAStar : Arrive
{
    
    public GameObject pathPrefab;
    public GameObject obstaclePrefab;
    public GameObject goalPrefab;
    public GameObject goal;
    public bool debug;
    public float localSpaceDepth;
    public Distancia Distance_metric;
    public Grid grid;
    

    private GameObject pathInstance;
    private GameObject goalInstance;
    private Dictionary<Tile, float> h_values = new Dictionary<Tile, float>();
    private Dictionary<Tile, float> temp = new Dictionary<Tile, float>();
    private List<Tile> localSearchSpace;
    private Tile current;
    private Tile goalTile;
    private Agente targetDelegado;

    public void Empezar()
    {
        GameObject objectDelegado = new GameObject();
        objectDelegado.transform.position = goal.transform.position;
        targetDelegado = objectDelegado.AddComponent<AgentNPC>();

        targetDelegado.RadioInterior = (float) (grid.tileSize * 0.2);
        targetDelegado.RadioExterior = targetDelegado.RadioInterior + 2;
        
        goalTile = grid.WorldToTile(goal.transform.position);
        current = grid.WorldToTile(transform.position);

        goalInstance = GameObject.Instantiate(goalPrefab, goalTile.Center, Quaternion.identity);
        CheckDebug(goalInstance);
        
        foreach (Tile tile in grid.tiles.Values)
        {
            h_values[tile] = GetDistance(tile, goalTile, Distance_metric);
        }

        // **1. Definir espacio de b�squeda local**
        
        localSearchSpace = grid.GetLocalSearchSpace(current, goalTile, Distance_metric, localSpaceDepth);
        
        foreach (Tile tile in localSearchSpace)
        {
            temp[tile] = h_values[tile];
            h_values[tile] = float.MaxValue; // Inicializar en infinito
        }
        ValueUpdate();
        ExecuteLocalPath();
    }


    public override Steering GetSteering(AgentNPC agent)
    {
        if(grid.WorldToTile(transform.position)==current)   //Si hemos llegado al objetivo local
        {
            if(current!=goalTile)   //Hemos llegado al objetivo local pero no al objetivo final
            {
                if(!localSearchSpace.Contains(current))  //Si el tile al que hemos llegado no est� en el espacio de b�squeda local
                {
                    localSearchSpace = grid.GetLocalSearchSpace(current, goalTile, Distance_metric, localSpaceDepth);   //Actualizamos el espacio local de b�squeda
                    foreach (Tile tile in localSearchSpace) //Guardamos los valores te�ricos e inicializamos los reales
                    {
                        temp[tile] = h_values[tile];
                        h_values[tile] = float.MaxValue; // Inicializar en infinito
                    }
                    ValueUpdate();  //Actualizamos los valores reales
                }
                ExecuteLocalPath();  //Buscamos el siguiente tile al que ir
            }
        }
        targetDelegado.Posicion = current.Center;
        base.target = targetDelegado;
        Steering steer = base.GetSteering(agent);

        return steer;
    }

    private void ValueUpdate()
    {
        bool hasChanged = false;
        bool containsInf = false;
        do
        {
            hasChanged = false;
            containsInf = false;

            foreach (Tile v in localSearchSpace)
            {
                float minNeighbor = MinNeighborCost(v);
                float new_h = Mathf.Max(temp[v], minNeighbor);
                 
                if (h_values[v] > new_h)
                {
                    h_values[v] = new_h;
                    hasChanged = true; // Hubo un cambio, seguimos iterando
                }

                if (h_values[v] == float.MaxValue)
                {
                    containsInf = true;
                }
            }

        } while ((containsInf && hasChanged) || hasChanged);
        return;
    }


    private void ExecuteLocalPath()
    {
        float minCost = float.MaxValue;
        List<Tile> TileList = grid.GetNeighbors(current, Distance_metric);
        List<Tile> newTiles = new List<Tile>();
        Tile bestTile = null;
        foreach (Tile neighbor in TileList)
        {
            float estimatedCost = GetDistance(current, neighbor, Distance_metric) + h_values[neighbor];

            if (estimatedCost < minCost)
            {
                minCost = estimatedCost;
                bestTile = neighbor;
            }
        }

        // Ejecutar la acci�n: mover a la mejor Tile
        if (bestTile != null)
        {
            pathInstance = GameObject.Instantiate(pathPrefab, current.Center, Quaternion.identity);
            CheckDebug(pathInstance);
            current = bestTile;
        }
        
        return;
    }

    private void CheckDebug(GameObject instance)
    {
        if (!debug)
        {
            
            Renderer rend = instance.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.enabled = false; // Oculta el objeto sin desactivarlo completamente
            }
        }
    }

    // Calcula el menor costo entre los vecinos de un nodo
    private float MinNeighborCost(Tile tile)
    {
        float minCost = float.MaxValue;
        foreach (Tile neighbor in grid.GetNeighbors(tile, Distance_metric))
        {
            //PrintLocalSearchGrid(grid.GetNeighbors(tile));
            float cost = GetDistance(tile, neighbor, Distance_metric) + h_values[neighbor];
            if (cost < minCost)
            {
                minCost = cost;
            }
        }
        return minCost;
    }

}