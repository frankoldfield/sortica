using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Separation : SteeringEnGrupo
{
    /*
     * Atributos:
     *      - threshold: el umbral de distancia a partir del cual empieza
     *      a aplicarse el steering.
     *      - decayCoefficient: coeficiente para aplicar la fuerza.
     */
    public float threshold = 5f;
    public float decayCoefficient = 1000f;


    /*
     * La idea del Steering Separation es que, dada una lista de targets,
     * 
     */
    public override Steering GetSteering(AgentNPC agent)
    {
        Steering steer = new Steering();

        foreach(Agente target in Targets)
        {
            Vector3 direccion = agent.Posicion - target.Posicion;
            float distancia = direccion.magnitude;

            float fuerza = 0;

            if (distancia < threshold)
            {
                // Prueba
                if(distancia <= 0)
                {
                    distancia = 0.1f;
                    direccion = Bodi.OrientationToVector(Random.Range(0, 360));
                }

                fuerza = Mathf.Min(
                    decayCoefficient / (distancia * distancia),
                    agent.AceleracionMaxima
                    );
            }

            direccion = direccion.normalized;
            steer.Lineal += fuerza * direccion;
        }

        return steer;
    }
}
