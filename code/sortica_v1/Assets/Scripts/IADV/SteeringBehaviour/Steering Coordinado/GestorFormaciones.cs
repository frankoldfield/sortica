using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TipoFormacion
{
    FORMACION_EN_W,
    FORMACION_LOGO
}

public class GestorFormaciones
{
    private static Dictionary<TipoFormacion, Type> TIPO_A_PATTERN = new Dictionary<TipoFormacion, Type>()
    {
        {TipoFormacion.FORMACION_EN_W, typeof(FormacionW)},
        {TipoFormacion.FORMACION_LOGO, typeof(FormacionLogo) }
    };

    /*
     * Dado un tipo de formación (TipoFormacion), devuelve una instancia de ese tipo.
     */
    public static FormationPattern getFormacionFromTipo(TipoFormacion tipo)
    {
        Type tipoFormacion = TIPO_A_PATTERN[tipo];

        if(tipoFormacion == null)
        {
            return null;
        }

        return (FormationPattern) Activator.CreateInstance(tipoFormacion);
    }

    /*
     * Dada una instancia de un FormationPattern, devuelve qué TipoFormacion es.
     */
    public static TipoFormacion getTipoFromFormacion(FormationPattern patron)
    {
        Type tipo = patron.GetType();
        
        foreach(TipoFormacion tipoFormacion in TIPO_A_PATTERN.Keys)
        {
            if (TIPO_A_PATTERN[tipoFormacion].Equals(tipo))
            {
                return tipoFormacion;
            }
        }

        throw new System.Exception("El tipo " + tipo.ToString() + " no tiene asociada ningún TipoFormacion.");
    }

    /*
     * Dado el tipo actual de formación, devuelve el siguiente (es decir, recorre los tipos de formaciones
     * circularmente para poder cambiar entre estas).
     */
    public static FormationPattern getSiguienteFormacion(TipoFormacion tipo)
    {
        TipoFormacion[] tipos = (TipoFormacion[])Enum.GetValues(typeof(TipoFormacion));

        for(int i = 0; i < tipos.Length; i++)
        {
            if (tipos[i] == tipo)
            {
                TipoFormacion siguiente = tipos[(i + 1)% tipos.Length];
                return getFormacionFromTipo(siguiente);
            }
        }

        return null;
    }

    /*
     * Dada una instancia de FormationPattern, devuelve otra instancia de FormationPattern que correspondería
     * con la siguiente según TipoFormacion.
     */
    public static FormationPattern getSiguienteFormacion(FormationPattern patron)
    {
        TipoFormacion tipo = getTipoFromFormacion(patron);
        return getSiguienteFormacion(tipo);
    }

}
