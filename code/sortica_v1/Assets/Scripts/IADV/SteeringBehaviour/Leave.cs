using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Leave es, esencialmente, el movimiento contrario a Arrive. En nuestro caso, queremos que el personaje huya
/// a la m�xima velocidad posible cuando est� dentro del radio interior del target y que vaya decelerando cuando
/// est� fuera de este. Cuando est� fuera del radio exterior, adem�s, queremos que su velocidad sea 0.
/// </summary>
public class Leave : SteeringBehaviour
{
    float timeToTarget = 0;

    /// <summary>
    /// El m�todo GetSteering() de Leave tiene, como objetivo, hacer que el personaje huya hasta estar lo suficientemente
    /// lejos de "target". Para ello:
    /// 
    ///     (1) Se comprueba que el atributo timeToTarget sea mayor o igual que 0 (no tendr�a sentido acelerar en
    ///     0 unidades de tiempo, pues tendr�a que hacerlo con magnitud infinita). En caso de no serlo, se le asigna
    ///     Time.deltaTime (tiempo desde el anterior frame).
    ///     
    ///     (2) Se halla el vector de huida (el contrario al vector que une la posici�n de agent y la de target) y
    ///     la distancia que los separa (magnitud o m�dulo del vector).
    ///     
    ///     (3) Se distingue seg�n la distancia para establecer la velocidad de huida objetivo (target speed):
    ///     
    ///         (3.1) Si agent se encuentra fuera del radio exterior de target, targetSpeed = 0.
    ///         
    ///         (3.2) Si agent se encuentra entre el radio exterio y el interior, se toma una proporci�n de la velocidad
    ///         m�xima de agent. Como queremos que aumente cuanto m�s cerca est� de target, se multiplica agent.VelocidadMaxima
    ///         por 1 - distancia / radioExterior (la velocidad aumenta proporcionalmente al decrecimiento de la distancia).
    ///         
    ///         (3.3) Si agent se encuentra dentro del radio interior, querr� huir con targetSpeed = agent.VelocidadMaxima.
    ///         
    ///     (4) Se halla el vector velocidad de huida, multiplicando targetSpeed por la direcci�n normalizada.
    ///     
    ///     (5) Se halla la diferencia entre el vector velocidad de huida y la velocidad actual del personaje para acelerarlo
    ///     ( v = v0 + at --> a = (v - v0) / t).
    ///     
    ///     (6) Se halla la aceleraci�n objetivo dividiendo la diferencia de las velocidades entre el tiempo (time).
    ///     En caso de ser esta aceleraci�n muy grande, se acota a la aceleraci�n lineal m�xima de agent.
    ///     
    ///     (7) Se devuelve un Steering cuya aceleraci�n lineal es la calculada en (6) y aceleraci�n angular = 0.
    ///     
    /// </summary>
    /// <param name="agent"></param>
    /// <returns></returns>
    public override Steering GetSteering(AgentNPC agent)
    {
        // (1)
        float time = timeToTarget <= 0 ? Time.deltaTime : timeToTarget;

        // (2)
        Vector3 direccion = agent.Posicion - target.Posicion;
        float distancia = direccion.magnitude;

        // (3)
        float targetSpeed = 0f;

        // (3.1)
        if(distancia > target.RadioExterior)    // Se deja este if, aunque innecesario, por diferenciar los tres casos.
        {
            targetSpeed = 0f;
        }
        // (3.2)
        else if (distancia < target.RadioExterior && distancia > target.RadioInterior)
        {
            targetSpeed = agent.VelocidadMaxima * (1 - distancia / target.RadioExterior);
        }
        // (3.3)
        else if (distancia < target.RadioInterior)
        {
            targetSpeed = agent.VelocidadMaxima;
        }

        // (4)
        Vector3 targetVelocity = targetSpeed * direccion.normalized;

        // (5)
        Vector3 diferenciaVelocidad = targetVelocity - agent.Velocidad;

        // (6)
        Vector3 aceleracion = diferenciaVelocidad / time;

        if(aceleracion.magnitude > agent.AceleracionMaxima)
        {
            aceleracion = aceleracion.normalized * agent.AceleracionMaxima;
        }

        // (7)
        Steering steer = new Steering();
        steer.Lineal = aceleracion;
        steer.Angular = 0f;
        return steer;
    }

}
