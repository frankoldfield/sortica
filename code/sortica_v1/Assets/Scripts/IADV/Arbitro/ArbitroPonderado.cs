using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ArbitroPonderado : SteeringBehaviour
{

    /*
    * Estructura que contiene un SteeringBehaviour y su respectivo peso.
    */
    [System.Serializable]
    public struct BehaviourPeso
    {
        public SteeringBehaviour steeringBehaviour;
        public float peso;
    }

    // Colección de BehaviourPeso que contiene los SteeringBehaviour y sus pesos.
    public BehaviourPeso[] behaviours;

    /*
     * Métodos que tienen que ser implementados por las clases hijas.
     * En el primero se inicializa el árbitro con sus respectivos steerings y pesos, además de añadir dichos steerings al agente.
     * En el segundo, se eliminan los steerings del agente.
     * En el tercero, se asignan los targets a los steerings.
     * Estos métodos son usados por el gestor de árbitros.
     */
    public abstract void create();
    public abstract void delete();
    public abstract void setTargets(params object[] targets);

    /*
     * El objetivo es devolver un Steering que sea combinación de todos los SteeringBehaviour
     * contenidos en behaviours ponderados por su peso. Para ello, recorre la colección y, al
     * valor actual calculado, le agrega la ponderación del Steering calculado.
     */
    public override Steering GetSteering(AgentNPC agent)
    {
        // Crea el Steering que será combinación de los demás.
        Steering steer = new Steering();
        steer.Angular = 0;
        steer.Lineal = new Vector3();
        foreach (BehaviourPeso behaviourPeso in behaviours)
        {
            SteeringBehaviour steeringBehaviour = behaviourPeso.steeringBehaviour;
            float peso = behaviourPeso.peso;
            // Intenta calcular el Steering, puede fallar si no se ha configurado el SteeringBehaviour.
            try {
                // Calcula el Steering para ese SteeringBehaviour.
                Steering steerActual = steeringBehaviour.GetSteering(agent);
                // Pondera
                steer.Angular += peso * steerActual.Angular;
                steer.Lineal += peso * steerActual.Lineal;
            } catch (System.NullReferenceException) {
                Debug.Log("Falta por configurar: " + gameObject.name);
            }
        }

        if(Mathf.Abs(steer.Angular) > agent.AngularMaxima)
        {
            steer.Angular = (steer.Angular / Mathf.Abs(steer.Angular)) * agent.AngularMaxima;
        }

        if(steer.Lineal.magnitude > agent.AceleracionMaxima)
        {
            steer.Lineal = steer.Lineal.normalized * agent.AceleracionMaxima;
        }

        return steer;
    }
}