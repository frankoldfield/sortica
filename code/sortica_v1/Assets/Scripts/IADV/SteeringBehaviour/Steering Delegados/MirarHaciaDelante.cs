using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirarHaciaDelante : Face
{
    /// <summary>
    /// Crea el target delegado. Es virtual porque, cada vez que se llame a GetSteering(), se actualizará su posición
    /// para que se mire hacia él usando el vector velocidad del personaje.
    /// </summary>
    protected new void Awake()
    {
        base.Awake();
        GameObject objeto = new GameObject();
        targetDelegado = objeto.AddComponent<Agente>();
        objeto.SetActive(false);
    }
    
    /// <summary>
    /// Elimina el targetDelegado virtual creado en el método Awake().
    /// </summary>
    new void OnDestroy()
    {
        base.OnDestroy();
        if(base.targetDelegado != null)
            Destroy(base.targetDelegado.gameObject);
    }

    /// <summary>
    /// En este Steering el objetivo es acelerar angularmente al agente para que mire en dirección de su velocidad.
    /// De esta forma, da la sensación de andar recto, pues mira hacia delante.
    /// El procedimiento es:
    /// 
    ///     (1) Se crea un objeto Steering con la componente lineal a (0, 0, 0), pues solo queremos acelerarlo
    ///     angularmente.
    ///     
    ///     (2) Se actualiza el targetDelegado de la superclase para que su posición sea la resultante de la suma
    ///     de la posición de agent y su vector velocidad normalizado.
    ///     
    ///     (3) Se llama al método GetSteering() de Face (superclase) y se devuelve lo que esta calcula.
    ///     
    /// </summary>
    /// <param name="agent">El Agente al que queremos aplicarle Steering.</param>
    /// <returns>Un Steering con aceleración angular y lineal.</returns>
    public override Steering GetSteering(AgentNPC agent)
    {
        // (1)
        Steering steer = new Steering();
        steer.Lineal = new Vector3(0, 0, 0);

        if (agent.Speed == 0)
        {
            steer.Angular = 0;
            return steer;
        }

        // (2)
        base.targetDelegado.Posicion = agent.Posicion + agent.Velocidad.normalized;

        // (3)
        return base.GetSteering(agent);
    }
}
