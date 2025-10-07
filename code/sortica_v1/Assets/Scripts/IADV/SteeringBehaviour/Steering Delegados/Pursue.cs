using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pursue : MiSeek
{
    /// <summary>
    /// Target del Steering delegado.
    /// </summary>
    public Agente targetDelegado = null;

    /// <summary>
    /// M�ximo tiempo de predicci�n.
    /// </summary>
    public float maxPrediction = 1f;

    /// <summary>
    /// El m�todo Awake() inicializa el atributo "target" de MiSeek a�adiendo un GameObject a la escena y, a este
    /// GameObject, a�adi�ndole un componente "Agente".
    /// </summary>
    protected void Awake()
    {
        GameObject objeto = new GameObject();
        base.target = objeto.AddComponent<Agente>();
        objeto.SetActive(false);
    }

    /// <summary>
    /// Destruye el target delegado.
    /// </summary>
    protected void OnDestroy()
    {
        Destroy(base.target);
    }

    /// <summary>
    /// Para Pursue, el objetivo es que agent persiga a targetDelegado, pero prediciendo su movimiento. Para ello:
    /// 
    ///     (1) Calcula la distancia que hay entre ellos.
    ///     
    ///     (2) Si, en un tiempo de "maxPrediction", no podr�a recorrer esa distancia, por lo que intentar� predecir
    ///     lo m�ximo posible (es decir, en tiempo, maxPrediction).
    ///     
    ///     (3) Si la distancia se puede recorrer en maxPrediction a la velocidad actual del personaje, entonces
    ///     toma una predicci�n m�s peque�a y proporcional a la distancia. Por tanto, cuanto menor sea la distancia,
    ///     menos tendr� que predecir.
    ///     
    ///     (4) Actualiza el target de MiSeek.
    ///     
    ///     (5) Llama al m�todo GetSteering() de la clase padre y devuelve dicho Steering.
    /// </summary>
    /// <param name="agent"></param>
    /// <returns></returns>
    public override Steering GetSteering(AgentNPC agent)
    {

        // (1)
        Vector3 direccion = targetDelegado.Posicion - agent.Posicion;
        float distancia = direccion.magnitude;

        float speed = agent.Speed;

        float prediction = 0f;

        // (2)
        if(speed * maxPrediction <= distancia)
        {
            prediction = maxPrediction;
        }
        // (3)
        else
        {
            prediction = distancia / speed; 
        }

        // (4)
        base.target.Posicion = targetDelegado.Posicion + targetDelegado.Velocidad * prediction;

        // (5)
        return base.GetSteering(agent);
    }

    /// <summary>
    /// Dibuja el rayo que va desde su posici�n a su target para que se vea que no es un seek.
    /// </summary>
    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position + new Vector3(0, 0.5f, 0), base.target.Posicion + new Vector3(0, 0.5f, 0));
    }
}
