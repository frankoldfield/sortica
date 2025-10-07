using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FormationPattern
{

    /*
     * Atributos:
     * 
     *      - numberOfSlots: el n�mero de slots ACTUALMENTE en la formaci�n.
     *      
     */
    //public int numberOfSlots;
    public Dictionary<int, Location> localizacionesRelativas;


    ///*
    // * Calcula el drift offset de la formaci�n dados los agentes que se encuentran actualmente en ella.
    // */
    //public abstract Location getDriftOffset(List<SlotAssignment> slotAssignments);
    /*
     * Devuelve la localizaci�n de un slot dado el n�mero que lo identifica.
     */
    public abstract Location getSlotLocation(int slotNumber);
    /*
     * Devuelve true en caso de que el patr�n pueda soportar el n�mero dado de slots (slotCount).
     */
    public abstract bool supportSlots(int slotCount);
}
