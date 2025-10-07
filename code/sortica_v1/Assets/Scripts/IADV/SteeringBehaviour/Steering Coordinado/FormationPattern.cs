using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FormationPattern
{

    /*
     * Atributos:
     * 
     *      - numberOfSlots: el número de slots ACTUALMENTE en la formación.
     *      
     */
    //public int numberOfSlots;
    public Dictionary<int, Location> localizacionesRelativas;


    ///*
    // * Calcula el drift offset de la formación dados los agentes que se encuentran actualmente en ella.
    // */
    //public abstract Location getDriftOffset(List<SlotAssignment> slotAssignments);
    /*
     * Devuelve la localización de un slot dado el número que lo identifica.
     */
    public abstract Location getSlotLocation(int slotNumber);
    /*
     * Devuelve true en caso de que el patrón pueda soportar el número dado de slots (slotCount).
     */
    public abstract bool supportSlots(int slotCount);
}
