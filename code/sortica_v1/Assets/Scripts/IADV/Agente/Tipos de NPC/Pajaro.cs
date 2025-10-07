using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pajaro : AgentNPC
{
    protected new void Awake()
    {
        base.Awake();
        GestorArbitros gestor = GetComponent<GestorArbitros>();
        gestor.tipoArbitro = GestorArbitros.TipoArbitro.Flocking;
    }

    private void Start()
    {
        GameObject[] pajaros = GameObject.FindGameObjectsWithTag("Pajaro");
        Agente[] agentesPajaro = new Agente[pajaros.Length];
        ArbitroFlocking arbitro = GetComponent<ArbitroFlocking>();

        for (int i = 0; i < pajaros.Length; i++)
        {
            agentesPajaro[i] = pajaros[i].GetComponent<Pajaro>();
        }
        arbitro.setTargets(agentesPajaro);
    }
}
