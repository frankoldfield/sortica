//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;

//public class FormacionPrueba : FormationPattern
//{
//    public float characterRadius = 1.0f;
//    public float radioCirculo = 10f;

//    public FormacionPrueba()
//    {
//        numberOfSlots = 10;
//    }

//    public override Location getDriftOffset(List<SlotAssignment> slotAssignments)
//    {
//        Location centro = new Location();
//        numberOfSlots = slotAssignments.Count;

//        foreach (SlotAssignment slot in slotAssignments)
//        {
//            Location location = getSlotLocation(slot.slotNumber);
//            centro.posicion += location.posicion;
//            centro.orientacion += location.orientacion;
//        }

//        centro.posicion /= numberOfSlots;
//        centro.orientacion /= numberOfSlots;
//        return centro;
//    }

//    public override Location getSlotLocation(int slotNumber)
//    {
//        float separacionEnAngulos = Mathf.Deg2Rad * (slotNumber * 360 / numberOfSlots);

//        Vector3 posicionRelativa = new Vector3(
//            Mathf.Sin(separacionEnAngulos),
//            0,
//            Mathf.Cos(separacionEnAngulos)
//            );

//        posicionRelativa *= radioCirculo;

//        Location relativa = new Location();
//        relativa.posicion = posicionRelativa;
//        relativa.orientacion = 0;
//        return relativa;
//    }

//    //public override Location getSlotLocation(int slotNumber)
//    //{
//    //    Location location;

//    //    if (numberOfSlots == 0)
//    //    {
//    //        location = new Location();
//    //        location.posicion = new Vector3(0, 0, 0);
//    //        location.orientacion = 0;
//    //        return location;
//    //    }

//    //    float anguloAlrededorCirculo = slotNumber / numberOfSlots * Mathf.PI * 2;

//    //    float radius = characterRadius / Mathf.Sin(Mathf.PI / numberOfSlots);

//    //    location = new Location();
//    //    location.posicion = new Vector3();
//    //    location.posicion.x = radius * Mathf.Sin(anguloAlrededorCirculo);
//    //    location.posicion.z = radius * Mathf.Cos(anguloAlrededorCirculo);
//    //    location.orientacion = anguloAlrededorCirculo;

//    //    return location;
//    //}

//    public override bool supportSlots(int slotCount)
//    {
//        //throw new System.NotImplementedException();
//        //return true;
//        return slotCount <= numberOfSlots;
//    }
//}
