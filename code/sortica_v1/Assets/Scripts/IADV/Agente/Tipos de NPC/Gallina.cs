using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Distances;

public class Gallina : AgentNPC
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
            if(hit.collider.gameObject.tag == "Plains") { // Movimiento normal, no tiene limitaciones
                VelocidadMaxima = 20;
                AceleracionMaxima = 200;
            }
            else if(hit.collider.gameObject.tag == "Mountains") { // Movimiento reducido, es más difícil moverse por la montaña
                VelocidadMaxima = 10;
                AceleracionMaxima = 100;
            }
            else if(hit.collider.gameObject.tag == "Waters") { // El agua moja sus plumas, por lo que se mueve más lento, las gallinas no están hechas para moverse por agua
                VelocidadMaxima = 5f;
                AceleracionMaxima = 50;
            }
            else if(hit.collider.gameObject.tag == "Forests") { // Se mueve un poco más lento por la presencia de vegetación abundante, aun así, al ser pequeña, no le afecta tanto
                VelocidadMaxima = 15f;
                AceleracionMaxima = 150;
            }
        }

        WallAvoidance wallAvoidance = GetComponent<WallAvoidance>();
        if (wallAvoidance != null)
        {
            wallAvoidance.numBigotes = 1; // Las gallinas tienen mala visión, por lo que solo tienen un bigote
        }

        LRTAStar lrtastar = GetComponent<LRTAStar>();
        if (lrtastar != null)
        {
            lrtastar.Distance_metric = Distancia.Chebyshev;
        }
    }
}
