using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormacionLogo : FormationPattern
{
    private static Dictionary<int, Location> POSICIONES_RELATIVAS = new Dictionary<int, Location>()
    {
        { 0, new Location(new Vector3(0, 0, 0), 0) },
        { 1, new Location(new Vector3(-1, 0, -2), 0) },
        { 2, new Location(new Vector3(1, 0, -2), 0) },
        { 3, new Location(new Vector3(-2, 0, -4), 0) },
        { 4, new Location(new Vector3(2, 0, -4), 0) },
        { 5, new Location(new Vector3(-3, 0, -6), 0) },
        { 6, new Location(new Vector3(3, 0, -6), 0) },
        { 7, new Location(new Vector3(-5, 0, -6), 0) },
        { 8, new Location(new Vector3(5, 0, -6), 0) },
        { 9, new Location(new Vector3(-4, 0, -7), 0) },
        { 10, new Location(new Vector3(4, 0, -7), 0) },
        { 11, new Location(new Vector3(-5, 0, -8), 0) },
        { 12, new Location(new Vector3(5, 0, -8), 0) },
        { 13, new Location(new Vector3(-3, 0, -8), 0) },
        { 14, new Location(new Vector3(3, 0, -8), 0) },
        { 15, new Location(new Vector3(-2, 0, -9), 0) },
        { 16, new Location(new Vector3(2, 0, -9), 0) },
    };

    public static float SEPARACION_SLOTS = 3f;
    
    public FormacionLogo()
    {
        localizacionesRelativas = POSICIONES_RELATIVAS;
    }

    public override Location getSlotLocation(int slotNumber)
    {
        Location relativa = POSICIONES_RELATIVAS[slotNumber];
        Location relativaReal = new Location(relativa.posicion * SEPARACION_SLOTS, relativa.orientacion);
        return relativaReal;
    }

    public override bool supportSlots(int slotCount)
    {
        return slotCount <= POSICIONES_RELATIVAS.Count;
    }
}
