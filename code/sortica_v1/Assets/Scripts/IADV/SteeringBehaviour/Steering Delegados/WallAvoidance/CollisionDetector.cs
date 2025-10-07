using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Un Bigote tiene una orientaci�n y una distancia. Para el Raycast, se usar� la orientaci�n para hallar
* la direcci�n del vector y la distancia como distancia m�xima para procesar el rayo.
*/
public class Bigote
{
    public float orientacion;
    public float distancia;

    public Bigote(float orientacion, float distancia)
    {
        this.orientacion = orientacion;
        this.distancia = distancia;
    }
 };

public struct Colision
{
    public Bigote bigote;
    public RaycastHit hit;
}

/*
 * Clase detector de colisiones.
 */
[System.Serializable]
public class CollisionDetector 
{
    /*
     * Atributos est�ticos:
     *      
     *      (1) ANGULO_MINIMO_OBSERVACION: �ngulo m�nimo que hay que observar cuando hay un n�mero par de bigotes.
     *      Cuando el n�mero de bigotes es impar, siempre hay uno en el centro; sin embargo, cuando es par, hay dos
     *      que podemos considerar centrales. Para estos, es bueno establecer un �ngulo m�nimo que tienen que formar,
     *      pues son los que van m�s cercanos a la direcci�n de la velocidad y, por tanto, del movimiento del agente.
     *      
     *      (2) ANGULO_MAXIMO_OBSERVACION: el �ngulo que tienen que formar los bigotes a lo sumo. Es decir, el �ngulo
     *      m�ximo que va a cubrir el WallAvoidance. No tiene sentido, por ejemplo, buscar colisiones por detr�s del
     *      agente, pues no va a ir en esa direcci�n.
     *      
     *      (3) DISTANCIA_BIGOTES_PRIORITARIOS: la distancia de el/los bigote/s centrales. Estos, al ser m�s prioritarios,
     *      cubren una mayor distancia (tiene sentido que, los que sean m�s parecidos al sentido del movimiento, tengan
     *      m�s importancia a la hora de buscar la colisi�n).
     *      
     *      (4) DISTANCIA_BIGOTES_NO_PRIORITARIOS: la distancia de los bigotes no centrales.
     *      
     *      (5) BIGOTES_POR_DEFECTO: n�mero de bigotes por defecto.
     *      
     *      (6) ELEVACION_BIGOTE: altura mínima a la que mira el bigote, para no mirar en el (0, 0, 0).
     */
    private static float ANGULO_MINIMO_OBSERVACION = 5f;
    private static float ANGULO_MAXIMO_OBSERVACION = 30f;
    private static float DISTANCIA_BIGOTES_PRIORITARIOS = 5f;
    private static float DISTANCIA_BIGOTES_NORMALES = 4f;
    private static int BIGOTES_POR_DEFECTO = 3;
    private static float ELEVACION_BIGOTE = 0.5f;

    /*
     * Atributos de clase:
     * 
     *      (1) bigotesPrioritarios: array de Bigotes que contiene los bigotes prioritarios. Al buscar colisiones,
     *      mirar� primero en este array.
     *      
     *      (2) bigotesNoPrioritarios: array de Bigotes que contiene aquellos bigotes no prioritarios. Si ha buscado
     *      en bigotesPrioritarios y no ha encontrado colisi�n, pasar� a buscar en estos.
     *      
     *      (3) numBigotes: el n�mero de bigotes que hay (quitar?).
     */
    private Bigote[] bigotesPrioritarios = null;
    private Bigote[] bigotesNoPrioritarios = null;
    private int numBigotes = 0;
    [SerializeField] private float anguloMinimoObservacion, anguloMaximoObservacion, distanciaBigotesPrioritarios, distanciaBigotesNormales;

    public float getElevacionBigotes()
    {
        return ELEVACION_BIGOTE;
    }

    /*
     * M�todo que devuelve un array con todos los Bigotes.
     */
    public Bigote[] getBigotes()
    {
        Bigote[] bigotes = new Bigote[bigotesPrioritarios.Length + bigotesNoPrioritarios.Length];

        for(int i = 0; i < bigotesPrioritarios.Length; i++)
        {
            bigotes[i] = bigotesPrioritarios[i];
        }

        for(int i = 0; i < bigotesNoPrioritarios.Length; i++)
        {
            bigotes[i + bigotesPrioritarios.Length] = bigotesNoPrioritarios[i];
        }

        return bigotes;
    }

    public CollisionDetector(int numBigotes, float anguloMinimo, float anguloMaximo, 
        float distanciaBigotesPrioritarios, float distanciaBigotesNormales)
    {
        this.numBigotes = numBigotes > 0 ? numBigotes : BIGOTES_POR_DEFECTO;
        anguloMinimoObservacion = anguloMinimo;
        anguloMaximoObservacion = anguloMaximo;
        this.distanciaBigotesPrioritarios = distanciaBigotesPrioritarios;
        this.distanciaBigotesNormales = distanciaBigotesNormales;

        if (this.numBigotes % 2 == 0)
        {
            bigotesPrioritarios = new Bigote[2];
            bigotesNoPrioritarios = new Bigote[this.numBigotes - 2];
        }
        else
        {
            bigotesPrioritarios = new Bigote[1];
            bigotesNoPrioritarios = new Bigote[this.numBigotes - 1];
        }

        inicializarBigotes();
    }


    /*
     * Inicializador. Crea los arrays de bigotes y llama a inicializarBigotes().
     */
    public CollisionDetector(int numBigotes) : this(numBigotes, ANGULO_MINIMO_OBSERVACION, ANGULO_MAXIMO_OBSERVACION, DISTANCIA_BIGOTES_PRIORITARIOS, DISTANCIA_BIGOTES_NORMALES)
    {}

    /*
     * Este m�todo se encarga de inicializar los bigotes. Para ello:
     * 
     *      (1) Halla el n�mero de bigotes prioritarios (nBigotesPrioritarios) y el �ngulo a cubrir (
     *      anguloACubrir). Este �ngulo se refiere al trozo donde se tendr�n que situar los bigotes no
     *      prioritarios o normales.
     *      
     *          (1.1) Si nBigotesPrioritarios es 1, inicializa dicho bigote con:
     *              - orientaci�n = 0 --> El bigote va en la misma direcci�n que el movimiento/velocidad.
     *          
     *          (1.2) Si nBigotesPrioritarios es 2, inicializa los dos bigotes de la siguiente forma:
     *              
     *              - El primero tiene orientaci�n = -(ANGULO_MINIMO_OBSERVACION) / 2. Por ejemplo, si el
     *              �ngulo m�nimo fuera 10, este bigote tendr�a que estar a -5� del centro (direcci�n del
     *              movimiento).
     *              
     *              - El segundo tiene orientaci�n = (ANGULO_MINIMO_OBSERVACION) / 2 por la misma raz�n
     *              que el anterior.
     *          
     *          Adem�s, en este caso rectifica el �ngulo a cubrir. (Ver (Comentario 1)).
     *          
     *      (2) Inicializa los bigotes normales teniendo en cuenta el �ngulo que tienen que cubrir
     *      y el n�mero de estos.
     *         
     *         
     *         
     *     (Comentario 1)    
     *         Cubierto por "ANGULO_MINIMO_OBSERVACION"
     *         <------->
     *          \  |  /
     *           \ | /   ANGULO_MAXIMO_OBSERVACION / 2 - ANGULO_MINIMO_OBSERVACION / 2 = anguloACubrir.
     *      _____(\|/)_____  
     *      
     */
    private void inicializarBigotes()
    {
        // (1)
        int nBigotesPrioritarios = bigotesPrioritarios.Length;
        float anguloACubrir = anguloMaximoObservacion / 2;

        // (1.1)
        if (nBigotesPrioritarios == 1)
        {
            bigotesPrioritarios[0] = new Bigote(0, distanciaBigotesPrioritarios);
        }
        // (1.2)
        else
        { 
            bigotesPrioritarios[0] = new Bigote(-(anguloMinimoObservacion) / 2, distanciaBigotesPrioritarios);
            bigotesPrioritarios[1] = new Bigote(anguloMinimoObservacion / 2, distanciaBigotesPrioritarios);
            anguloACubrir -= anguloMinimoObservacion / 2;
        }

        // (2)

        int nBigotesNoPrioritarios = numBigotes - nBigotesPrioritarios;
        int bigotesPorLado = nBigotesNoPrioritarios / 2;
        float tamHueco = anguloACubrir / (bigotesPorLado + 1);

        for (int i = 0; i < bigotesPorLado; i++)
        {
            bigotesNoPrioritarios[i] = new Bigote(
                -(anguloMaximoObservacion / 2) + tamHueco * i,
                distanciaBigotesNormales
                );
        }

        for (int i = 0; i < bigotesPorLado; i++)
        {
            bigotesNoPrioritarios[bigotesPorLado + i] = new Bigote(
                anguloMaximoObservacion / 2 - tamHueco * i,
                distanciaBigotesNormales
                );
        }
    }

    /*
     * Devuelve la informaci�n de la colisi�n (RaycastHit) en caso de que haya. Para ello:
     * 
     *      (1) Crea un array con los bigotes prioritarios al inicio para procesarlos antes y los bigotes
     *      normales despu�s.
     *      
     *      (2) Recorre los bigotes y:
     *          
     *          (2.1) Halla el nuevo vector de direcci�n (hallarNuevaDireccion) teniendo en cuenta la
     *          orientaci�n del bigote. Cuando la tiene, crea un rayo desde el origen y con el nuevo
     *          vector direcci�n.
     *          
     *          (2.2) Comprueba si hay colisi�n para ese bigote (con el rayo creado en el paso anterior).
     *          Si hay colisi�n, devuelve la informaci�n de esta.
     *          
     *      (3) Si no ha encontrado colisi�n, devuelve un RaycastHit "vac�o". El atributo collider ser� "null", por
     *      lo que se puede comprobar si ha habido colisi�n o no.
     */
    public Colision getCollision(Vector3 posicion, Vector3 rayVector)
    {
        Colision colision = new Colision();
        RaycastHit info;

        // (1)
        Bigote[] bigotesOrdenados = new Bigote[numBigotes];
        for (int i = 0; i < bigotesPrioritarios.Length; i++) bigotesOrdenados[i] = bigotesPrioritarios[i];
        for (int i = 0; i < bigotesNoPrioritarios.Length; i++) bigotesOrdenados[bigotesPrioritarios.Length + i] = bigotesNoPrioritarios[i];

        // (2)
        foreach (Bigote bigote in bigotesOrdenados)
        {
            // (2.1)
            Vector3 nuevaDireccion = Geometria.hallarNuevaDireccion(bigote.orientacion, rayVector);
            Vector3 posicionElevada = posicion;
            posicionElevada.y += ELEVACION_BIGOTE;
            Ray rayo = new Ray(posicionElevada, nuevaDireccion);

            // Debug
            Debug.DrawRay(posicionElevada, nuevaDireccion * bigote.distancia, Color.black);

            // (2.2)
            if (Physics.Raycast(rayo, out info, bigote.distancia))
            {
                colision.hit = info;
                colision.bigote = bigote;
                return colision;
            }
        }

        // (3)
        colision.hit = new RaycastHit();
        colision.bigote = null;
        return colision;
    }

}
