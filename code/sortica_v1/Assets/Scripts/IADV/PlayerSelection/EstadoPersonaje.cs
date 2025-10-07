using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Representa aquellos atributos que queremos guardar de un personaje. Particularmente útil para guardar el árbitro que tenía anteriormente.
 */
public class EstadoPersonaje
{
    private GestorArbitros.TipoArbitro tipoArbitro;
    private float velocidadMaxima, aceleracionMaxima;
    private float angularMaxima, rotacionMaxima;
    private Agente[] targets;

    public EstadoPersonaje(Agente agent)
    {
        tipoArbitro = agent.GetComponent<GestorArbitros>().tipoArbitro;
        velocidadMaxima = agent.VelocidadMaxima;
        aceleracionMaxima = agent.AceleracionMaxima;
        angularMaxima = agent.AngularMaxima;
        rotacionMaxima = agent.RotacionMaxima;
    }

    public void restaurarEstado(Agente agent)
    {
        agent.VelocidadMaxima = velocidadMaxima;
        agent.AceleracionMaxima = aceleracionMaxima;
        agent.AngularMaxima = angularMaxima;
        agent.RotacionMaxima = rotacionMaxima;
        agent.GetComponent<GestorArbitros>().tipoArbitro = tipoArbitro;
    }
}
