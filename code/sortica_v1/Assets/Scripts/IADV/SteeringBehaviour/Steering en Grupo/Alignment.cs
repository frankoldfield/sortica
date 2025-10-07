using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alignment : SteeringEnGrupo
{

    /*
     * Atributos:
     *
     *  (1) threshold: el umbral de distancia (m�nima) para tener en cuenta un target. Si la distancia entre agente
     *  y target es mayor a threshold, la orientaci�n del target no se tiene en cuenta.
     *  
     */
    public float threshold = 20f;


    /*
     * La idea de Alignment es que, dada una lista de Agentes (targets), el agente intente tener la misma orientaci�n
     * que el promedio (en este caso, la media) de los targets. Adem�s, se pone como condici�n que el target est� a una
     * distancia menor o igual que threshold.
     * El procedimiento es el siguiente:
     * 
     *  (1) Se inicializan un contador (count) y una variable, orientacionPromedio, que se ir�n actualizando con los targets.
     *  
     *  (2) Para cada target:
     *      
     *      (2.1) Se halla la direcci�n y la distancia. Si la distancia es mayor que el threshold, se pasa al siguiente.
     *      
     *      (2.2) Si la distancia es menor o igual que el threshold, se suma, a la orientaci�n promedio, la orientaci�n
     *      del target y se actualiza count. (Comentario 1)
     *      
     *  (3) Se inicializa un Steering, steer, que ser� lo devuelto por el m�todo.
     *  
     *  (4) En caso de que se haya contado alg�n target:
     *  
     *      (4.1) Se halla la aceleraci�n angular que queremos aplicar sobre el agente. (Comentario 2)
     *      
     *      (4.2) Se actualiza el campo "angular" de steer a dicha aceleraci�n.
     *      
     *  (5) Si no se hab�a contado ning�n target, no se acelera al agente.
     *      
     *  
     *  (Comentario 1) V�ase que los agentes tienen una orientaci�n entre 0 y 360. Por tanto:
     *      
     *      (1 / n) * sum ( 0, i = 1, ..., n ) <= (1 / n) * sum ( orientacion[i], i = 1, ..., n )
     *      Y 
     *      (1 / n) * sum (0, i = 1, ..., n ) = 1 / n * 0 * n = 0
     *      
     *      Adem�s
     *      
     *      (1 / n) * sum ( orientacion[i], i = 1, ..., n ) <= (1 / n) * sum ( 360, i = 1, ..., n)
     *      Y 
     *      (1 / n) * sum ( 360, i = 1, ..., n) = 1 / n * 360 * n = 360
     *      
     *  Por tanto, sabemos que, haciendo la media de las orientaciones, esta media estar� entre 0 y 360 (es coherente).
     *  
     *  (Comentario 2) Los c�lculos son an�logos a los que se hacen en Align, por lo que no se explican aqu�.
     */
    public override Steering GetSteering(AgentNPC agent)
    {
        // (1)
        int count = 0;
        float orientacionPromedio = 0;

        // (2)
        foreach (Agente target in Targets)
        {
            // (2.1)
            Vector3 direccion = target.Posicion - agent.Posicion;
            float distancia = direccion.magnitude;

            // (2.2)
            if (distancia <= threshold)
            {
                orientacionPromedio += Bodi.MapToRange(target.Orientacion, 0, 360);
                count++;
            }
        }

        // (3)
        Steering steer = new Steering();
        steer.Lineal = new Vector3(0, 0, 0);

        // (4)
        if (count > 0)
        {
            orientacionPromedio = orientacionPromedio / count;

            // (4.1)
            float theta_objetivo = orientacionPromedio;
            float theta_actual = agent.Orientacion;

            float theta_objetivo_relativo = Bodi.MapToRange(
                theta_objetivo - theta_actual,
                0,
                360
                );

            float sentidoRotacion = 1;
            float distancia_entre_angulos = theta_objetivo_relativo;

            if(theta_objetivo_relativo > 180)
            {
                sentidoRotacion = -1;
                distancia_entre_angulos = 360 - distancia_entre_angulos;
            }

            // Quiero recorrer "distancia_entre_angulos" en Time.deltaTime. w = distancia_entre_angulos / Time.deltaTime.
            float rotacionObjetivo = sentidoRotacion * distancia_entre_angulos / Time.deltaTime;
            
            if(rotacionObjetivo > agent.RotacionMaxima)
            {
                rotacionObjetivo = agent.RotacionMaxima;
            } 

            // w = w0 + alpha * t --> alpha = (w - w0) / t
            float angularObjetivo = (rotacionObjetivo - agent.Rotacion) / Time.deltaTime;

            // (4.2)
            if(Mathf.Abs(angularObjetivo) > agent.AngularMaxima)
            {
                angularObjetivo = angularObjetivo / Mathf.Abs(angularObjetivo) * agent.AngularMaxima;
            }
            steer.Angular = angularObjetivo;
            return steer;
        }

        // (5)
        steer.Angular = 0;
        return steer;
    }

}
