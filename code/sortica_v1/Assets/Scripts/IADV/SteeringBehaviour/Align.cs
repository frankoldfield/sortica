using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Align : SteeringBehaviour
{

    [SerializeField] private float timeToTarget = 0;

    /*
     * Para el Steering Behaviour Align, el objetivo es que el Agente agent mire en, aproximadamente, la misma
     * dirección que el Agente target (es decir, que su orientación esté dentro del ángulo interior de visión de
     * target). Para ello, se realizan los siguientes pasos:
     * 
     *      (1) Se calcula la orientación relativa del target con respecto al agente. Es decir, si la orientación
     *      del agente fuera el punto origen de la rotación (el 0), qué ángulo tendría el target. Por ejemplo, si
     *      agent.Orientation = 45º y target.Orientation = 30º, la orientación relativa del target sería -15º que,
     *      en el rango [0, 360], es 345º.
     *      
     *      (2) Se calculan
     *      
     *          (2.1) La diferencia real de orientación (diferenciaRealOrientacion), que es la distancia más pequeña entre
     *          las orientaciones: min{|agent.Orientacion - target.Orientacion|, 360 - |agent.Orientacion - target.Orientacion|}.
     *          
     *          (2.2) El sentido de la rotación. Si la orientación relativa está en el rango [0, 180], tendrá que girar en sentido
     *          antihorario, mientras que, si se encuentra en el rango [180, 360), tendrá que girar en sentido horario.
     *      
     *      (3) Se calcula la rotación objetivo distinguiendo los siguientes casos:
     *          
     *          (3.1) Si la orientación de agent ya coincide con el ángulo interior de target (es decir, la diferencia
     *          en la orientación es menor que la mitad del ángulo interior), la rotación objetivo es 0.
     *          
     *          (3.2) Si no, si la orientación de agent coincide con el ángulo exterior de target, la rotación objetivo
     *          es una proporción de la orientación máxima de agent:
     *              rotacionObjetivo = sentidoRotacion * agent.RotacionMaxima / target.AnguloExterior
     *              
     *          (3.3) Si no, la rotación objetivo es la rotación máxima de agent.
     *          
     *      (4) ** Se calcula la aceleración angular (aceleracionAngular) siguiendo las fórmulas de Newton:
     *          rotacionObjetivo = rotacion_0 + aceleracionAngular * timeToTarget --> ...
     *          --> aceleracionAngular = (rotacionObjetivo - rotacion_0) / timeToTarget.
     *          
     *      (5) Se construye y devuelve un objeto Steering con:
     *          - lineal = 0.
     *          - angular igual a la aceleración angular calculada en (4)
     *          
     *      ** Véase que se comprueba si timeToTarget es <= 0 para, en ese caso, que se use Time.deltaTime.
     *      Esto dota de coherencia, pues no tiene sentido establecernos llegar a un objetivo en tiempo menor
     *      o igual a 0, así que, como mínimo, tendría que ser en un frame (Time.deltaTime).
     */
    public override Steering GetSteering(AgentNPC agent)
    {
        // (1)
        //float targetOrientationRelativa = Bodi.MapToRange(agent.Orientacion - target.Orientacion, 0, 360);
        float targetOrientationRelativa = Bodi.MapToRange(target.Orientacion - agent.Orientacion, 0, 360);

        // (2)
        float diferenciaRealOrientacion;
        float sentidoRotacion;

        if (targetOrientationRelativa < 180)
        {
            diferenciaRealOrientacion = targetOrientationRelativa;
            sentidoRotacion = 1;
        }
        else
        {
            diferenciaRealOrientacion = 360 - targetOrientationRelativa;
            sentidoRotacion = -1;
        }

        // (3)
        float rotacionObjetivo;

        // (3.1)
        if (diferenciaRealOrientacion < Mathf.Abs(target.AnguloInterior / 2))
        {
            rotacionObjetivo = 0;
        }
        // (3.2)
        else if (diferenciaRealOrientacion < Mathf.Abs(target.AnguloExterior / 2))
        {

            rotacionObjetivo = sentidoRotacion * agent.RotacionMaxima * diferenciaRealOrientacion / target.AnguloExterior;
        }
        // (3.3)
        else
        {
            rotacionObjetivo = agent.RotacionMaxima * sentidoRotacion;
        }

        // (4)
        float timeToTarget = this.timeToTarget;

        if (timeToTarget <= 0)
        {
            timeToTarget = Time.deltaTime;
        }

        float aceleracionAngular = (rotacionObjetivo - agent.Rotacion) / timeToTarget;

        if(Mathf.Abs(aceleracionAngular) > agent.AngularMaxima)
        {
            aceleracionAngular = aceleracionAngular / Mathf.Abs(aceleracionAngular) * agent.AngularMaxima;
        }

        // (5)
        Steering steer = new Steering();
        steer.Angular = aceleracionAngular;
        steer.Lineal = new Vector3(0, 0, 0);
        return steer;
    }
}
