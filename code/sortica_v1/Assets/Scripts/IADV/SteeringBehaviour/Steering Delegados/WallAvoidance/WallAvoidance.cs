using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WallAvoidance : MiSeek
{
    private void Awake()
    {
        GameObject objeto = new GameObject();
        target = objeto.AddComponent<Agente>();
        objeto.SetActive(false);
        collisionDetector = new CollisionDetector(numBigotes);
    }

    private void OnDestroy()
    {
        if(target != null)
            Destroy(target.gameObject);
    }

    /*
     *  Atributos:
     *      
     *      (1) avoidDistance: la distancia a la que estar� el punto que sigue la normal en caso de que haya
     *      colisi�n.
     *      
     *      (2) lookAhead: la distancia a la que se comprueban las colisiones.
     *      
     *      (3) collisionDetector: un detector de colisiones.
     *      
     *      (4) numBigotes: el n�mero de bigotes que tiene el detector de colisiones.
     */

    [SerializeField] private float avoidDistance= 2f;
    //[SerializeField] private float lookAhead = 4f;
    private CollisionDetector collisionDetector = null;
    public int numBigotes = 3;

    /*
     * Atributos para poder tocar el detector de colisiones desde el inspector.
     */
    public float anguloMinimo = 5f;
    public float anguloMaximo = 90f;
    public float distanciaPrioritarios = 6f;
    public float distanciaNormales = 4f;

    /*
     * El objetivo de WallAvoidance es que, si el agente se encuentra con un obst�culo, no lo atraviese. Para ello:
     * 
     *      (1) Instancia un Detector de Colisiones con:
     *          - Un n�mero de bigotes.
     *          - Un �ngulo m�nimo de observaci�n (ver comentarios de la clase CollisionDetector).
     *          - Un �ngulo m�ximo de observaci�n (para no tener en cuenta toda la circunferencia).
     *          - Una distancia para los bigotes prioritarios.
     *          - Una distancia para los bigotes normales.
     *          
     *      (2) Se toma, como origen de la colisi�n, la posici�n del agente y, como direcci�n, la velocidad de este
     *      (que es la direcci�n de movimiento).
     *      
     *      (3) Se llama al Detector de Colisiones, que devuelve un Struct Colision con:
     *          - El bigote que ha detectado la colisi�n (si no se detecta, ser� null).
     *          - El RaycastHit devuelto en Physics.Raycast.
     *          
     *      (4) Se procesa la colisi�n:
     *      
     *          (4.1) Si no hubo colisi�n, se devuelve aceleraci�n 0 para no interferir con el movimiento del agente.
     *          
     *          (4.2) Si hubo colisi�n, el target toma, como posici�n, un punto que sigue la normal del objeto detectado
     *          en la colisi�n y cuyo origen es el punto de colisi�n. La distancia es de "avoidDistance". Se llama
     *          a la base (MiSeek) y se devuelve el Steering.
     */
    public override Steering GetSteering(AgentNPC agent)
    {
        // (1)
        collisionDetector = new CollisionDetector(numBigotes, anguloMinimo, anguloMaximo, distanciaPrioritarios, distanciaNormales);

        // (2)
        Vector3 posicion = agent.Posicion;
        Vector3 direccion = agent.Velocidad.normalized;

        // (3)
        Colision colision = collisionDetector.getCollision(posicion, direccion);
        RaycastHit infoColision = colision.hit;
        Bigote bigoteColision = colision.bigote;

        // (4)

        // (4.1)
        if(infoColision.collider == null)
        {
            Steering steer = new Steering();
            steer.Angular = 0;
            steer.Lineal = new Vector3(0, 0, 0);
            return steer;
        }

        // (4.2)
        Vector3 nuevaPosicionTarget = infoColision.point + infoColision.normal * avoidDistance; 
        nuevaPosicionTarget.y = agent.Posicion.y;
        target.Posicion = nuevaPosicionTarget;
        return base.GetSteering(agent);        
    }

    /*
     * PARA PRUEBAS. DIBUJA LOS BIGOTES.
     */
    //private void OnDrawGizmos()
    //{
    //    Agente agente = GetComponent<Agente>();
    //    if(agente == null)
    //    {
    //        return;
    //    }

    //    if(collisionDetector == null)
    //    {
    //        return;
    //    }

    //    Bigote[] bigotes = collisionDetector.getBigotes();

    //    foreach(Bigote bigote in bigotes)
    //    {
    //        Vector3 nuevaDireccion = Geometria.hallarNuevaDireccion(bigote.orientacion, agente.Velocidad);
    //        Vector3 posicionFinalRecta = Geometria.posicionFinalRecta(agente.Posicion, nuevaDireccion, bigote.distancia);
    //        posicionFinalRecta.y += transform.position.y;
    //        Gizmos.DrawLine(transform.position, posicionFinalRecta);
    //    }
    //}
}
