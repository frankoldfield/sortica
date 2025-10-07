using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

/*
 * Struct SlotAssingment: guarda la informaci�n necesaria para los slots/ranuras de una formaci�n: el personaje 
 * que lo ocupa (null si no lo ocupa nadie) y el n�mero que lo identifica dentro de la formaci�n.
 */
public class SlotAssignment
{
    public Agente character;
    public int slotNumber;

    public SlotAssignment(Agente character)
    {
        this.character = character;
        slotNumber = -1;
    }

    public SlotAssignment()
    {
        this.character = null;
        slotNumber = -1;
    }
}

/*
 * Estructura para una localizaci�n. Una localizaci�n puede ser, por ejemplo, una posici�n y una orientaci�n.
 * Contiene un Vector3, posicion, que representa la posici�n y un real, orientacion, que representa la orientaci�n.
 */
public class Location
{
    public Vector3 posicion;
    public float orientacion;

    public Location()
    { }

    public Location(Vector3 posicion, float orientacion)
    {
        this.posicion = posicion;
        this.orientacion = orientacion;
    }
}

/*
 * 
 */
public class FormationManager : MonoBehaviour
{
    /*
     * Atributos:
     * 
     *      - slotAssignments: lista de SlotAssignments actuales.
     *      - slotLider: slot que contiene el personaje que ser� el l�der de la formaci�n.
     *      - pattern: el patr�n de la formaci�n.
     */

    public List<SlotAssignment> slotAssignments = new List<SlotAssignment>();
    private SlotAssignment slotLider = null;
    public FormationPattern pattern;

    /*
     * Actualiza los SlotAssignment. El l�der siempre tendr� slotNumber a 0 y, el resto,
     * desde 1 hasta slotAssignments.Count + 1.
     */
    public void updateSlotAssignments()
    {
        slotLider.slotNumber = 0;

        List<SlotAssignment> nuevosAssignments = new List<SlotAssignment>();

        int j = 1;

        for(int i = 0; i < slotAssignments.Count; i++)
        {
            bool encontrado = false;
            while(j < pattern.localizacionesRelativas.Keys.Count && !encontrado)
            {
                if(!hayObstaculo(j))
                {
                    slotAssignments[i].slotNumber = j;
                    nuevosAssignments.Add(slotAssignments[i]);
                    encontrado = true;
                }
                j++;
            }

            if(!encontrado)
            {
                slotAssignments[i].slotNumber = -1;
            }
        }

        slotAssignments = nuevosAssignments;

        //for (int i = 0; i < slotAssignments.Count; i++)
        //{
        //    slotAssignments[i].slotNumber = i + 1;  // Se reserva el slotNumber 0 para el l�der.
        //}
    }

    /// <summary>
    /// Comprueba si hay un obstáculo en la posición absoluta para el slot con slotNumber
    /// pasado por parámetro.
    /// </summary>
    /// <param name="slotNumber"></param>
    /// <returns></returns>
    private bool hayObstaculo(int slotNumber)
    {
        Vector3 posicionReal = getLocationAbsoluta(slotNumber).posicion;
        return hayObstaculo(posicionReal);
    }

    /// <summary>
    /// Devuelve true si hay un obstáculo en la posición pasada por argumento.
    /// </summary>
    /// <param name="posicion"></param>
    /// <returns></returns>
    private bool hayObstaculo(Vector3 posicion)
    {
        Vector3 boxSize = new Vector3(2f, 2f, 2f);
        RaycastHit hit;

        if(Physics.BoxCast(posicion + new Vector3(0, 10, 0), boxSize / 2, new Vector3(0, -1, 0), out hit, Quaternion.identity, 100f, ~LayerMask.GetMask("Suelo", "Personajes", "Grid")))
        {
                return true;
        }

        return false;
    }

    /*
     * Intenta a�adir un personaje a la formaci�n. Primero, intenta a�adir el l�der.
     * Si no ha podido, comprueba si el patr�n acepta un nuevo personaje. Si puede, lo
     * inserta y devuelve true. Si no, devuelve false.
     */
    public bool addCharacter(Agente character)
    {
        if (slotLider == null)
        {
            slotLider = new SlotAssignment();
            slotLider.character = character;
            updateSlotAssignments();
            return true;
        }

        int slotsOcupados = slotAssignments.Count + 1;  // + 1 por el l�der.

        if (pattern.supportSlots(slotsOcupados + 1))
        {
            SlotAssignment nuevoSlot = new SlotAssignment();
            nuevoSlot.character = character;
            slotAssignments.Add(nuevoSlot);
            updateSlotAssignments();

            if(nuevoSlot.slotNumber == -1)
            {
                return false;
            }

            return true;
        }

        return false;
    }

    public bool removeCharacter(Agente character)
    {
        SlotAssignment slot = encontrarSlotConCharacter(character);

        if (slot == null)
        {
            return false;
        }

        if (slot == slotLider)
        {
            return false;
        }

        slotAssignments.Remove(slot);
        updateSlotAssignments();
        return true;
    }

    /*
     * Intenta encontrar un SlotAssignment en slotAssignments cuyo personaje sea el que
     * se pasa por argumento. Si lo encuentra, devuelve el SlotAssignment. Si no, devuelve
     * null.
     */
    public SlotAssignment encontrarSlotConCharacter(Agente character)
    {
        if (slotLider.character == character)
        {
            return slotLider;
        }

        foreach (SlotAssignment slot in slotAssignments)
        {
            if (slot.character == character)
            {
                return slot;
            }
        }

        return null;
    }

    /*
     * Intenta encontrar un SlotAssignment en slotAssignments cuyo slotNumber sea el que
     * se pasa por argumento. Si lo encuentra, devuelve el SlotAssignment. Si no, devuelve
     * null.
     */
    private SlotAssignment encontrarSlotConSlotNumber(int slotNumber)
    {
        if (slotLider.slotNumber == slotNumber)
        {
            return slotLider;
        }

        foreach (SlotAssignment slot in slotAssignments)
        {
            if (slot.slotNumber == slotNumber)
            {
                return slot;
            }
        }

        return null;
    }

    /*
     * M�todo para actualizar los targets de los Agentes en los slots. Los pasos son:
     * 
     *      (1) Se toma la Location (posici�n y orientaci�n) del l�der, a partir de la que
     *      se calcular�n los targets.
     *      
     *      (2) Se itera para cada agente en los SlotAssignments:
     *      
     *          (2.1) Se halla su Location relativa.
     *          
     *          (2.2) Se calcula su LocationReal mediante la matriz de rotaci�n (ver clase
     *          geometr�a).
     *          
     *          (2.3) Se actualiza su target.
     */
    public void updateSlots()
    {
        // (1)
        Location locationLider = getLocationLider();

        // (2)
        for (int i = 0; i < slotAssignments.Count; i++)
        {
            Agente character = slotAssignments[i].character;
            int slotNumber = slotAssignments[i].slotNumber;

            // (2.1)
            Location relativeLocation = pattern.getSlotLocation(slotNumber);

            // (2.2)
            Location locationReal = getLocationAbsoluta(slotNumber);

            //RaycastHit hit;
            //Ray rayo = new Ray(character.Posicion + new Vector3(0, 0.5f, 0), locationReal.posicion - character.Posicion);

            //if ((locationReal.posicion - character.Posicion).magnitude < lookUpDistance &&
            //    Physics.Raycast(rayo, out hit, distanciaRayo) &&
            //    hit.transform.root.gameObject.GetComponent<Agente>() == null)
            //{
            //    setTarget(character, new Location(character.Posicion, locationReal.orientacion));
            //}
            //else
            //{
            //    // (2.3)
            //    setTarget(character, locationReal);
            //}
            //if (Physics.Raycast(rayo, out hit, distanciaRayo) &&
            //    hit.transform.root.gameObject.GetComponent<Agente>() == null)
            //{
            //    setTarget(character, new Location(character.Posicion, locationReal.orientacion));
            //}
            //else
            //{
            //    // (2.3)
            //    setTarget(character, locationReal);
            //}
            // (2.3)
            setTarget(character, locationReal);
        }
    }

    public Location getLocationAbsoluta(int slotNumber)
    {
        Location relativeLocation = pattern.getSlotLocation(slotNumber);
        Location locationReal = new Location();
        Location locationLider = getLocationLider();

        locationReal.posicion = Geometria.hallarPosicionDesdeRelativa(
                relativeLocation.posicion,
                locationLider.posicion,
                slotLider.character.Orientacion
                );

        locationReal.orientacion = relativeLocation.orientacion + locationLider.orientacion;

        return locationReal;
    }

    /*
     * Intenta devolver una instancia de Location con la localizaci�n del l�der. Si no
     * hay l�der en la formaci�n, devuelve null. Si no, devuelve la Location con la posici�n
     * y la orientaci�n del l�der en el mundo.
     */
    private Location getLocationLider()
    {
        if (slotLider == null)
        {
            return null;
        }

        Vector3 posicionLider = slotLider.character.Posicion;
        float orientacionLider = slotLider.character.Orientacion;

        Location locationLider = new Location();
        locationLider.posicion = posicionLider;
        locationLider.orientacion = orientacionLider;
        return locationLider;
    }

    /*
     * Dado un Agente, se busca establecer su target (para el �rbitro de Formacion)
     * como la Location pasada por argumento.
     */
    private void setTarget(Agente agent, Location location)
    {
        ArbitroFormacion componenteFormacion = agent.GetComponent<ArbitroFormacion>();

        if (componenteFormacion == null)
        {
            Debug.LogWarning("El agente " + agent.name + " no tiene componente Formacion.");
        }
        else
        {
            componenteFormacion.setTarget(location);
        }
    }

    private void Update()
    {
        updateSlots();
    }
}