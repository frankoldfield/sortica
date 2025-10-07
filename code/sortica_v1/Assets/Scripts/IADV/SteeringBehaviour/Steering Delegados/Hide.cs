using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hide : Arrive
{
    public float offSetDistance = 4f; // Distancia extra desde el objeto para esconderse
    public Agente targetHide; // Agente del que hay que esconderse
    public List<GameObject> escondites = new List<GameObject>();

    private void Start()
    {
        // Buscar todos los objetos con la etiqueta "Hide" como posibles escondites
        GameObject[] objetosEscondite = GameObject.FindGameObjectsWithTag("Hide");

        foreach (var obj in objetosEscondite)
        {
            escondites.Add(obj);
        }

        // Si target no existe, crearlo dinámicamente para que Arrive pueda usarlo
        if (target == null)
        {
            GameObject obj = new GameObject("TargetEscondite");
            target = obj.AddComponent<Agente>();
            target.RadioExterior = 4;
            target.RadioInterior = 2;
        }
    }

    /*
     * Método que devuelve la mejor posición de ocultación posible detrás de un objeto.
     * Se calcula una posición que está "detrás" del escondite respecto al targetHide.
     */
    private Vector3 ObtenerPuntoDeOcultacion(GameObject escondite, Agente targetHide)
    {
        if (targetHide == null || escondite == null)
        {
            return escondite != null ? escondite.transform.position : Vector3.zero;
        }

        Vector3 vectorOcultacion = (escondite.transform.position - targetHide.Posicion).normalized;
        float distanciaDesdeObjeto = 2f + offSetDistance; // Se usa un valor fijo en lugar del radio del collider
        return escondite.transform.position + vectorOcultacion * distanciaDesdeObjeto;
    }

    /*
     * Método que determina cuál es el mejor escondite disponible respecto al targetHide.
     * Se calcula la distancia manualmente en 3D (sin ignorar Y).
     */
    private Vector3 DeterminarMejorEscondite(AgentNPC agente)
    {
        Vector3 mejorEscondite = Vector3.zero;
        float distanciaMinima = Mathf.Infinity;

        foreach (var escondite in escondites)
        {
            Vector3 posibleEscondite = ObtenerPuntoDeOcultacion(escondite, targetHide);

            // Cálculo manual de la distancia en 3D (NO ignora el eje Y)
            float distancia = Mathf.Sqrt(
                Mathf.Pow(posibleEscondite.x - agente.Posicion.x, 2) +
                Mathf.Pow(posibleEscondite.y - agente.Posicion.y, 2) +
                Mathf.Pow(posibleEscondite.z - agente.Posicion.z, 2)
            );

            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                mejorEscondite = posibleEscondite;
            }
        }

        return mejorEscondite;
    }

    /*
     * Obtiene el Steering para esconderse.
     * Se ejecuta en cada frame, buscando siempre el escondite más cercano.
     */
    public override Steering GetSteering(AgentNPC agent)
    {
        Steering steer = new Steering();

        if (targetHide == null)
        {
            Debug.LogWarning("No hay objetivo del que esconderse.");
            return steer; // Si no hay objetivo del que esconderse, no hacer nada
        }

        if (escondites.Count == 0)
        {
            // Si no hay escondites disponibles, activar una huida con Flee
            Flee flee = gameObject.GetComponent<Flee>();
            if (flee != null)
            {
                return flee.GetSteering(agent);
            }
            return steer; // No hacer nada si no hay huida
        }

        // Determinar el escondite más cercano en cada frame
        Vector3 mejorEscondite = DeterminarMejorEscondite(agent);

        // Comparar la posición actual con la nueva antes de actualizar
        float diferenciaPosicion = Mathf.Sqrt(
            Mathf.Pow(target.Posicion.x - mejorEscondite.x, 2) +
            Mathf.Pow(target.Posicion.y - mejorEscondite.y, 2) +
            Mathf.Pow(target.Posicion.z - mejorEscondite.z, 2)
        );

        if (diferenciaPosicion > 0.1f) 
        {
            target.Posicion = mejorEscondite;
        }

        // Usar la lógica de Arrive para moverse al escondite
        steer = base.GetSteering(agent);
        float magnitud = steer.Lineal.magnitude;
        
        Steering newSteer = new Steering();
        Vector3 direccion = steer.Lineal;
        direccion.y = 0;
        newSteer.Lineal = direccion.normalized * magnitud;
        newSteer.Angular = steer.Angular;
        return newSteer;
    }
}
