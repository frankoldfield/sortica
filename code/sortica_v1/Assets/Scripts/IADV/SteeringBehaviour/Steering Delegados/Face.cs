using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face : Align
{
    /*
     * Atributos:
     *      
     *      - targetDelegado: el Target que tiene la clase Face. No es el mismo que el target que tiene la
     *      superclase, que se irá creando/actualizando según sea necesario (Face delega en Align).
     *      
     *      - explicitTarget: el target que se le pasa a la superclase para que devuelva el Steering.
     */
    public Agente targetDelegado;
    protected Agente explicitTarget;

    /*
     * Awake inicializa un GameObject para asignarle un Script Agent (explicitTarget), puesto que no se puede 
     * inicializar una instancia de esta clase (hija de MonoBehaviour). Digamos que este simula el target del padre.
     */
    protected void Awake()
    {
        GameObject objeto = new GameObject();
        objeto.SetActive(false);
        explicitTarget = objeto.AddComponent<AgentNPC>();
    }

    /// <summary>
    /// Para que se ejecute en caso de destruir el Script. Para no dejar "Basura", quiero eliminar el explicitTarget.
    /// </summary>
    public void OnDestroy()
    {
        if(explicitTarget != null)
            Destroy(explicitTarget.gameObject);
    }

    /*
     * El objetivo de Face es que el Agente agent "mire" a targetDelegado. Para esto, se puede usar Align teniendo
     * en cuenta que el ángulo objetivo (theta en el dibujo de debajo) será Mathf.Atan2(x, z).
     * 
     *                X |
     *                  |      * targetDelegado.Posicion
     *                  |    / |
     *                  |   /  | x
     *                  |  /   | 
     *                  | / theta
     *                  |/_ _ _|_________ Z
     *   agent.Position *   z
     *                      
     * Por tanto, la idea es que el nuevo target tenga ángulo theta, consiguiendo, así, que agent mire a targetDelegado.
     * Para conseguirlo:
     *  
     *      (1) Se calcula el vector que une agent con targetDelegado (direccion).
     *      
     *      (2) Se calcula y asigna la nueva orientación mediante Mathf.Atan2.
     *      
     *      (3) Se asigna al explicitTarget unos ángulos interior y exterior iguales a los de agent, consiguiendo
     *      así que Align pare cuando el targetDelegado esté dentro del ángulo interior de agent.
     *      
     *      (4) Se delega en Align y se devuelve el Steering calculado por este.
     * 
     */
    public override Steering GetSteering(AgentNPC agent)
    {
        // (1)
        Vector3 direccion = targetDelegado.Posicion - agent.Posicion;

        // (2)
        explicitTarget.Orientacion = Mathf.Atan2(direccion.x, direccion.z) * Mathf.Rad2Deg;

        // (3)
        explicitTarget.AnguloInterior = agent.AnguloInterior;
        explicitTarget.AnguloExterior = agent.AnguloExterior;
        base.target = explicitTarget;

        // (4)
        Steering steer = base.GetSteering(agent);
        return steer;
    }
}
