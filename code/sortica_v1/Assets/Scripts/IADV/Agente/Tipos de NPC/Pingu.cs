using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Distances;

public class Pingu : AgentNPC
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
            if(hit.collider.gameObject.tag == "Plains") { // Movimiento reducido, los pingüinos se mueven lentos por la tierra
                VelocidadMaxima = 10;
                AceleracionMaxima = 100;
            }
            else if(hit.collider.gameObject.tag == "Mountains") { // Movimiento muy reducido, los pingüinos no están hechos para moverse por montañas
                VelocidadMaxima = 5;
                AceleracionMaxima = 50;
            }
            else if(hit.collider.gameObject.tag == "Waters") { // El agua es su hábitat natural, por lo que se mueve más rápido
                VelocidadMaxima = 25;
                AceleracionMaxima = 250;
            }
            else if(hit.collider.gameObject.tag == "Forests") { // Se mueve más lento por la presencia de tantos obstáculos
                VelocidadMaxima = 7.5f;
                AceleracionMaxima = 75;
            }
        }

        WallAvoidance wallAvoidance = GetComponent<WallAvoidance>();
        if (wallAvoidance != null)
        {
            wallAvoidance.numBigotes = 2; // Los pingüinos tienen una visión moderada, por lo que tienen dos bigotes
        }

        LRTAStar lrtastar = GetComponent<LRTAStar>();
        if (lrtastar != null)
        {
            lrtastar.Distance_metric = Distancia.Euclidea;
        }
    }
}