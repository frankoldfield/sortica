using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cohesion : SteeringEnGrupo
{
    /*
     * Atributos:
     * 
     *  (1) threshold: distancia mínima a la que tiene que encontrarse un target para tenerlo en cuenta en el
     *  método GetSteering().
     *  
     */
    public float threshold = 10f;

    public override Steering GetSteering(AgentNPC agent)
    {
        int count = 0;
        Vector3 centro_de_masas = new Vector3(0, 0, 0);

        foreach(Agente target in Targets)
        {
            Vector3 direccion = target.Posicion - agent.Posicion;
            float distancia = direccion.magnitude;

            if(distancia < threshold)
            {
                centro_de_masas += target.Posicion;
                count++;
            }
        }

        Steering steer = new Steering();
        steer.Angular = 0;

        if (count == 0)
        {
            steer.Lineal = new Vector3(0, 0, 0);
            return steer;
        }

        centro_de_masas /= count;

        // Aquí podría usarse un Seek, pero por no insertar más componentes al GameObject, hago el cálculo aquí.
        Vector3 nuevaDireccion = centro_de_masas - agent.Posicion;

        Vector3 velocidadDeseada = nuevaDireccion.normalized * agent.VelocidadMaxima;

        Vector3 nuevaAceleracion = (velocidadDeseada - agent.Velocidad) / Time.deltaTime;

        if(nuevaAceleracion.magnitude > agent.AceleracionMaxima)
        {
            nuevaAceleracion = nuevaAceleracion.normalized * agent.AceleracionMaxima;
        }

        steer.Lineal = nuevaAceleracion;
        return steer;        
    }
}
