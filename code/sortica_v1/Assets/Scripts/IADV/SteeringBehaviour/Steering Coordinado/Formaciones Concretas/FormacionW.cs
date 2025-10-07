using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormacionW : FormationPattern
{
    public static Dictionary<int, Location> LOCALIZACIONES_RELATIVAS = new Dictionary<int, Location>
    {
        {0, new Location(new Vector3(0, 0, 0), 0) },
        {1, new Location(new Vector3(-1, 0, -1), 330) },
        {2, new Location(new Vector3(1, 0, -1), 30) },
        {3, new Location(new Vector3(-2, 0, -2), 210) },
        {4, new Location(new Vector3(2, 0, -2), 150) },
        {5, new Location(new Vector3(-3, 0, -1), 300) },
        {6, new Location(new Vector3(3, 0, -1), 60) },
        {7, new Location(new Vector3(-4, 0, -2), 240) },
        {8, new Location(new Vector3(4, 0, -2), 120) },
        {9, new Location(new Vector3(-5, 0, -1), 270) },
        {10, new Location(new Vector3(5, 0, -1), 90) },
        {11, new Location(new Vector3(0, 0, -2), 180) },
    };

    public FormacionW()
    {
        localizacionesRelativas = LOCALIZACIONES_RELATIVAS;
    }

    public static float SEPARACION_SLOTS = 5;

    public override Location getSlotLocation(int slotNumber)
    {
        Location location = LOCALIZACIONES_RELATIVAS[slotNumber];
        Location locationRelativaReal = new Location(
            location.posicion * SEPARACION_SLOTS,
            location.orientacion);
        return locationRelativaReal;
    }

    public override bool supportSlots(int slotCount)
    {
        return slotCount <= LOCALIZACIONES_RELATIVAS.Count;
    }
}
