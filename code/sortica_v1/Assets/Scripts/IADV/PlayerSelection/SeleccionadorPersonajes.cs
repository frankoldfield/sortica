using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Distances;

public class SeleccionadorPersonajes : MonoBehaviour
{
    private static string TARGET_TAG = "NPC";
    private static string PLAINS_TAG = "Plains";
    private static string WATERS_TAG = "Waters";
    private static string MOUNTAINS_TAG = "Mountains";
    private static string FORESTS_TAG = "Forests";
    public List<GameObject> personajes = null;
    public GameObject prefabMarca = null;
    private bool alternarArbitro = false;

    private FormationManager formationManager = null;

    // Cosas del LRTA
    public GameObject pathPrefab;
    public GameObject obstaclePrefab;
    public GameObject goalPrefab;
    public bool debug;
    public GameObject scene;

    // Intervalo de tiempo en segundos para alternar el árbitro del líder
    public float intervaloCambioArbitro = 5f;

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0)) // Seleccionar (o deseleccionar) a un personaje
        {
            SeleccionarPersonaje();
        }
        else if (Input.GetMouseButtonDown(0)) // Mover a los personajes seleccionados al punto pulsado
        {
            MoverAPulsado();
        }
        else if (Input.GetKeyDown(KeyCode.F)) // Iniciar una formación con los personajes seleccionados
        {
            Formar();
        }
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.C)) // Cambiar de tipo formación
        {
            CambiarFormacion();
        }
        else if (Input.GetKeyDown(KeyCode.Q)) // Deseleccionar a todos los personajes de golpe
        {
            for (int i = personajes.Count - 1; i >= 0; i--)
            {
                sacarPersonaje(personajes[i]);
            }
        }
    }

    /*
     * Lanza un rayo y, en caso de que ese rayo haya impactado con un personaje, lo selecciona (le añade una marca
     * y lo inserta en la lista "personajes").
     */
    void SeleccionarPersonaje()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            string tagPulsado = hit.transform.root.tag;
            if (tagPulsado.Equals(TARGET_TAG))
            {
                procesarPersonaje(hit);
            }
        }
    }

    /*
     * Lanza un rayo y, en caso de impactar en el suelo, envía a los personajes seleccionados al punto de impacto.
     */
    void MoverAPulsado()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            string tagPulsado = hit.transform.tag;
            if ((tagPulsado.Equals(PLAINS_TAG) || tagPulsado.Equals(MOUNTAINS_TAG) || tagPulsado.Equals(WATERS_TAG) || tagPulsado.Equals(FORESTS_TAG)) && personajes.Count > 0)
            {
                // Sacamos la posición a la que hay que ir
                Vector3 posicionClick = hit.point;
                
                // Si hay una formación en marcha, se puede producir el caso 1 o 3. Si no, solo el caso 2.
                if(formationManager != null) {
                    // Si todos los personajes seleccionados están en la formación, se produce el caso 1. Si no, el caso 3.
                    if(FormacionCompleta()) {
                        Caso1(hit.point);
                    } else {
                        Caso3(hit.point);
                    }
                } else {
                    Caso2(hit.point);
                }

                // Activa la alternancia del árbitro solo si se ha hecho clic en el suelo
                alternarArbitro = true;
            }
        }
    }


    private Coroutine wanderCorrutina;
    /*
     * Caso 1: Todos los personajes son parte de una formación, por lo que el líder hace un pathfinding al punto pulsado y el resto hace un leader following.
     */
    void Caso1(Vector3 posicionClick) {
        DisolverFormacion();

        GameObject goal = new GameObject("Goal");
        goal.transform.position = posicionClick;

        foreach(GameObject personaje in personajes) {
            if(personaje == personajes[0]) {
                personaje.GetComponent<GestorArbitros>().tipoArbitro = GestorArbitros.TipoArbitro.LRTA;
                personaje.GetComponent<GestorArbitros>().Update();
                ArbitroLRTA arbitro = personaje.GetComponent<ArbitroLRTA>();
                object[] targets = new object[8];
                targets[0] = pathPrefab;
                targets[1] = obstaclePrefab;
                targets[2] = goalPrefab;
                targets[3] = goal;
                targets[4] = debug;
                targets[5] = 1f;
                targets[6] = Distancia.Euclidea;
                targets[7] = scene.GetComponent<Grid>();
                arbitro.setTargets(targets);
                personaje.GetComponent<LRTAStar>().Empezar();
            } else {
                personaje.GetComponent<GestorArbitros>().tipoArbitro = GestorArbitros.TipoArbitro.LeaderFollowing;
                personaje.GetComponent<GestorArbitros>().Update();
                ArbitroLeaderFollowing arbitro = personaje.GetComponent<ArbitroLeaderFollowing>();
                Agente[] targets = new Agente[personajes.Count];
                targets[0] = personajes[0].GetComponent<Agente>();
                for(int i = 1; i < targets.Length; i++)
                {
                    targets[i] = personajes[i].GetComponent<Agente>();
                }
                arbitro.setTargets(targets);
            }
        }

        StartCoroutine(EsperarYFormar(posicionClick));
    }

    /*
     * Corrutina que espera a que todos los personajes lleguen a su destino antes de formar la formación.
     */
    IEnumerator EsperarYFormar(Vector3 destino)
    {
        float umbral = 1.5f;
        GameObject lider = personajes[0];
        Agente agenteLider = lider.GetComponent<Agente>();

        bool haLlegado = false;

        while (!haLlegado)
        {
            Vector3 pos = agenteLider.Posicion;

            float dx = pos.x - destino.x;
            float dz = pos.z - destino.z;
            float distancia = Mathf.Sqrt(dx * dx + dz * dz);

            if (distancia <= umbral)
            {
                haLlegado = true;
            }

            yield return new WaitForSeconds(0.5f); // Espera medio segundo antes de comprobar de nuevo
        }

        // Cuando el líder ha llegado, recrear la formación
        Formar();

        if(formationManager != null)
        {
            wanderCorrutina = StartCoroutine(AlternarEntreWanderYQuieto(5f)); // 5 segundos por cada estado
        }
    }

    /*
    * Corrutina que alterna entre Wander y estado Quieto (Vacio) en el líder cada cierto tiempo.
    */
    IEnumerator AlternarEntreWanderYQuieto(float intervalo)
    {
        GameObject lider = personajes[0];
        GestorArbitros gestor = lider.GetComponent<GestorArbitros>();

        while (true)
        {
            // Wander
            gestor.tipoArbitro = GestorArbitros.TipoArbitro.Wander;
            yield return new WaitForSeconds(intervalo);

            // Quieto
            gestor.tipoArbitro = GestorArbitros.TipoArbitro.Quieto;
            yield return new WaitForSeconds(intervalo);
        }
    }

    /*
     * Caso 2: Todos los personajes hacen un pathfinding individual al punto pulsado, junto con un separation para no solaparse entre ellos.
     */
    void Caso2(Vector3 posicionClick) {
        GameObject goal = new GameObject("Goal");
        goal.transform.position = posicionClick;

        foreach(GameObject personaje in personajes) {
            personaje.GetComponent<GestorArbitros>().tipoArbitro = GestorArbitros.TipoArbitro.LRTASeparation;
            personaje.GetComponent<GestorArbitros>().Update();
            ArbitroLRTASeparation arbitro = personaje.GetComponent<ArbitroLRTASeparation>();
            int cuantosPersonajes = personajes.Count;
            object[] targets = new object[8+cuantosPersonajes];
            targets[0] = pathPrefab;
            targets[1] = obstaclePrefab;
            targets[2] = goalPrefab;
            targets[3] = goal;
            targets[4] = debug;
            targets[5] = 1f;
            targets[6] = Distancia.Euclidea;
            targets[7] = scene.GetComponent<Grid>();
            for(int i=0; i<cuantosPersonajes; i++) {
                targets[i+8] = personajes[i].GetComponent<Agente>();
            }
            arbitro.setTargets(targets);
            personaje.GetComponent<LRTAStar>().Empezar();
        }
    }

    /*
     * Caso 3: Algunos personajes están en la formación y otros no. Los que no están en la formación hacen un pathfinding individual al punto pulsado.
     * Los que están en la formación siguen al líder.
     */
    void Caso3(Vector3 posicionClick) {
        DisolverFormacion();
        Caso2(posicionClick);
    }

    bool FormacionCompleta()
    {
        if(formationManager == null)
        {
            return false;
        }

        foreach(GameObject personaje in personajes)
        {
            Agente agente = personaje.GetComponent<Agente>();

            if(formationManager.encontrarSlotConCharacter(agente) == null)
            {
                return false;
            }
        }

        return true;
    }

    /*
     * Si no había, previamente, una formación, creará una. Si la había, la disuelve.
     */
    void Formar()
    {
        if (formationManager == null && personajes.Count > 0)
        {
            CrearFormacion(GestorFormaciones.getFormacionFromTipo((TipoFormacion)0));
        }
        else
        {
            DisolverFormacion();
        }
    }

    /*
     * Crea una formación dado un patrón. Para ello, intenta insertar los personajes en la lista de seleccionados.
     * Si ha podido insertar un personaje, cambia su árbitro al de formaciones y guarda su árbitro anterior.
     */
    void CrearFormacion(FormationPattern patron)
    {
        formationManager = gameObject.AddComponent<FormationManager>();
        formationManager.pattern = patron;

        GameObject lider = personajes[0];

        formationManager.addCharacter(lider.GetComponent<Agente>());

        for (int i = 1; i < personajes.Count; i++)
        {
            Agente agente = personajes[i].GetComponent<Agente>();
            GestorArbitros gestor = personajes[i].GetComponent<GestorArbitros>();

            bool anadido = formationManager.addCharacter(agente);

            if (anadido)
            {
                gestor.tipoArbitro = GestorArbitros.TipoArbitro.Formacion;
            }
        }
    }

    /*
     * Disuelve una formación, es decir, quita los personajes que estaban. En caso de haber podido eliminar al personaje,
     * se le asigna el árbitro que tenía antes de entrar en la formación.
     */
    void DisolverFormacion()
    {
        if (wanderCorrutina != null) {
            StopCoroutine(wanderCorrutina);
            wanderCorrutina = null;
        }

        foreach (GameObject personaje in personajes)
        {
            Agente agente = personaje.GetComponent<Agente>();
            GestorArbitros gestor = personaje.GetComponent<GestorArbitros>();

            if (formationManager.removeCharacter(agente))
            {
                gestor.tipoArbitro = GestorArbitros.TipoArbitro.Vacio;
            }
        }

        // Detener la corrutina si estaba activa
        if (alternarArbitro)
        {
            StopCoroutine(AlternarArbitroLider());
            alternarArbitro = false;
        }

        Destroy(formationManager);
        formationManager = null;
    }

    /*
     * Procesa el personaje pulsado. Si estaba en la lista, lo saca. Si no, lo inserta.
     */
    void procesarPersonaje(RaycastHit hit)
    {
        GameObject seleccionado = hit.transform.root.gameObject;

        if (personajes.Contains(seleccionado))
        {
            sacarPersonaje(seleccionado);
        }
        else
        {
            insertarMarca(seleccionado);
            personajes.Add(seleccionado);
        }
    }

    /*
     * Sacar un personaje de la selección. Además de quitarle la marca, podría hacer otras cosas (como quitarlo
     * de la formación).
     */
    void sacarPersonaje(GameObject personaje)
    {
        quitarMarca(personaje);

        // Lo saca de la formación.
        if (formationManager != null)
        {
            formationManager.removeCharacter(personaje.GetComponent<Agente>());
        }

        personajes.Remove(personaje);
    }

    /*
     * Inserta una marca a un personaje.
     */
    void insertarMarca(GameObject objeto)
    {
        GameObject marca = Instantiate(prefabMarca, objeto.transform);

        float altura = obtenerAltura(objeto);

        marca.transform.localPosition = Vector3.up * (altura + 0.5f); // +0.5 para darle un margen

        Vector3 escalaPadre = objeto.transform.lossyScale;
        marca.transform.localScale = new Vector3(1 / escalaPadre.x, 1 / escalaPadre.y, 1 / escalaPadre.z);

        float tamañoDeseado = 1.0f;
        marca.transform.localScale *= tamañoDeseado;

        marca.name = "Mark";
    }

    /*
     * Obtiene la altura real de un objeto con Collider.
     */
    float obtenerAltura(GameObject objeto)
    {
        Collider col = objeto.GetComponent<Collider>();
        if (col != null)
        {
            float alturaReal = col.bounds.size.y / objeto.transform.lossyScale.y; // Se compensa la escala
            return alturaReal;
        }

        return 2.0f; // Altura por defecto si no hay Collider
    }

    /*
     * Quita la marca de un personaje.
     */
    void quitarMarca(GameObject objeto)
    {
        GameObject marca = objeto.transform.Find("Mark").gameObject;
        Destroy(marca);
    }

    /*
     * Cambia de formación usando el Gestor de Formaciones.
     */
    void CambiarFormacion()
    {
        if (formationManager == null)
        {
            return;
        }

        FormationPattern nuevoPatron = GestorFormaciones.getSiguienteFormacion(formationManager.pattern);
        DisolverFormacion();
        CrearFormacion(nuevoPatron);
    }

    /*
     * Corrutina que alterna cada "intervaloCambioArbitro" segundos entre el arbitro "Wander" y "Vacio" en el líder de la formación.
     */
    IEnumerator AlternarArbitroLider()
    {
        while (formationManager != null) // Solo se ejecuta mientras haya una formación activa
        {
            if (personajes.Count > 0 && personajes[0].GetComponent<GestorArbitros>() != null)
            {
                GameObject lider = personajes[0]; // El líder siempre es el primer personaje de la lista
                GestorArbitros gestor = lider.GetComponent<GestorArbitros>();

                gestor.tipoArbitro = GestorArbitros.TipoArbitro.Wander;
                yield return new WaitForSeconds(intervaloCambioArbitro);

                gestor.tipoArbitro = GestorArbitros.TipoArbitro.Vacio;
                yield return new WaitForSeconds(intervaloCambioArbitro);
            }
            else
            {
                yield break; // Si no hay personajes, salir de la corrutina
            }
        }
    }
}
