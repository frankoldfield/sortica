using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestorArbitros : MonoBehaviour
{
    // Aquí se enumeran los tipos de árbitro disponibles, sí se desea añadir uno más, se debe añadir aquí y en el diccionario de este script, justo debajo de esta primera línea.

    public enum TipoArbitro { WallAvoidanceSeek, Flee, Seek, Arrive, Align, AntiAlign, Wander, PathFollowingFace, Vacio, Flocking, Formacion, AndarRecto, WallAvoidanceWander, LeaderFollowing, Pursue, Leave, Hide, Quieto, LRTA, LRTASeparation }


    private static Dictionary<TipoArbitro, Type> TIPO_ARBITRO = new Dictionary<TipoArbitro, Type>()
    {
        {TipoArbitro.WallAvoidanceSeek, typeof(ArbitroWallAvoidanceSeek)},
        {TipoArbitro.Flee, typeof(ArbitroFlee)},
        {TipoArbitro.Seek, typeof(ArbitroSeek)},
        {TipoArbitro.Arrive, typeof(ArbitroArrive)},
        {TipoArbitro.Align, typeof(ArbitroAlign)},
        {TipoArbitro.AntiAlign, typeof(ArbitroAntiAlign)},
        {TipoArbitro.Wander, typeof(ArbitroWander)},
        {TipoArbitro.PathFollowingFace, typeof(ArbitroPathFollowingFace)},
        {TipoArbitro.Vacio, typeof(ArbitroVacio)},
        {TipoArbitro.Flocking, typeof(ArbitroFlocking)},
        {TipoArbitro.Formacion, typeof(ArbitroFormacion)},
        {TipoArbitro.AndarRecto, typeof(ArbitroAndarRecto)},
        {TipoArbitro.WallAvoidanceWander, typeof(ArbitroWallAvoidanceWander)},
        {TipoArbitro.LeaderFollowing, typeof(ArbitroLeaderFollowing)},
        {TipoArbitro.Pursue, typeof(ArbitroPursue) },
        {TipoArbitro.Leave, typeof(ArbitroLeave) },
        {TipoArbitro.Hide, typeof(ArbitroHide) },
        {TipoArbitro.Quieto, typeof(ArbitroQuieto) },
        {TipoArbitro.LRTA, typeof(ArbitroLRTA) },
        {TipoArbitro.LRTASeparation, typeof(ArbitroLRTASeparation) }

    };

    public static ArbitroPonderado getArbitroFromTipo(TipoArbitro tipo)
    {
        Type arbitro = TIPO_ARBITRO[tipo];

        if (arbitro == null)
        {
            return null;
        }

        return (ArbitroPonderado) Activator.CreateInstance(arbitro);
    }

    public TipoArbitro tipoArbitro;
    private TipoArbitro tipoArbitroActual; // Guarda el árbitro actual
    private ArbitroPonderado arbitroActual; // Referencia al árbitro actual
    public Agente[] targets = null;
    private AgentNPC agente = null;

    void Awake()
    {
        CambiarArbitro();
        tipoArbitroActual = tipoArbitro;
        agente = GetComponent<AgentNPC>();
    }

    public void Update()
    {
        // Solo actualizar si el tipo de árbitro cambió
        if (tipoArbitro != tipoArbitroActual)
        {
            CambiarArbitro();
            agente.Velocidad = Vector3.zero;
            agente.Aceleracion = Vector3.zero;
            agente.Rotacion = 0;
            agente.Angular = 0;
            tipoArbitroActual = tipoArbitro;
        }
    }

    void CambiarArbitro()
    {
        // Eliminar el árbitro anterior si existía
        if (arbitroActual != null)
        {
            arbitroActual.delete();
            Destroy(arbitroActual);
        }

        // Crear el nuevo árbitro según el tipo seleccionado.
        if(TIPO_ARBITRO.ContainsKey(tipoArbitro))
        {
            arbitroActual = (ArbitroPonderado)gameObject.AddComponent(TIPO_ARBITRO[tipoArbitro]);
            arbitroActual.create();
            return;
        }
        else
        {
            arbitroActual = null;
        }
    }
}