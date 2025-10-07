using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Distances;

public class Bloodskull : AgentNPC
{
    public float distanciaRaycast = 2f; // Distancia del Raycast
    private LayerMask capaDetectada; // Se declarará en Awake

    protected new void Awake()
    {
        base.Awake();
        capaDetectada = LayerMask.GetMask("Grid");
    }

    protected new void Update()
    {
        base.Update();

        // Definir el punto de inicio del Raycast y la dirección (hacia abajo)
        Vector3 origen = transform.position;
        Vector3 direccion = Vector3.down;

        // Lanzar el Raycast
        if (Physics.Raycast(origen, direccion, out RaycastHit hit, distanciaRaycast, capaDetectada))
        {
            if(hit.collider.gameObject.tag == "Plains") { // Movimiento normal, aunque es bastante lento por su tamaño
                VelocidadMaxima = 10;
                AceleracionMaxima = 100;
            }
            else if(hit.collider.gameObject.tag == "Mountains") { // Movimiento extremadamente reducido, su poca movilidad le impide moverse por la montaña
                VelocidadMaxima = 0.5f;
                AceleracionMaxima = 5;
            }
            else if(hit.collider.gameObject.tag == "Waters") { // El agua lo ralentiza, pero su masa muscular le permite no verse tan afectado
                VelocidadMaxima = 7.5f;
                AceleracionMaxima = 75;
            }
            else if(hit.collider.gameObject.tag == "Forests") { // Se mueve más lento por la presencia de tantos obstáculos
                VelocidadMaxima = 2.5f;
                AceleracionMaxima = 25;
            }
        }

        WallAvoidance wallAvoidance = GetComponent<WallAvoidance>();
        if (wallAvoidance != null)
        {
            wallAvoidance.numBigotes = 3; // Bloodskull tiene una visión moderada, por lo que tiene tres bigotes
        }

        LRTAStar lrtastar = GetComponent<LRTAStar>();
        if (lrtastar != null)
        {
            lrtastar.Distance_metric = Distancia.Manhattan;
        }
    }
}
