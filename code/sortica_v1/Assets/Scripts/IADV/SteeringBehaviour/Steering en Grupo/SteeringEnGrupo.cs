using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SteeringEnGrupo : SteeringBehaviour
{

    /*
     * Atributos: una lista de targets.
     */
    private Agente[] _targets = null;
    public Agente[] Targets
    {
        /*
         * Devuelve una copia de la lista de targets para que no se pueda modificar desde otro lugar.
         * 
         *  (1) Crea la copia.
         *  (2) Añade cada agente a la copia.
         *  (3) Devuelve la copia.
         */
        get
        {
            if(_targets == null)
            {
                return null;
            }

            // (1)
            Agente[] copia = new Agente[_targets.Length];
            
            // (2)
            for(int i = 0; i < _targets.Length; i++)
            {
                copia[i] = _targets[i];
            }

            // (3)
            return copia;
        }

        /*
         * Añade los agentes que se le pasan como argumento ÚNICAMENTE si el Agente no es la componente Agente
         * del gameObject (no se quiere tener en cuenta a uno mismo).
         * 
         *  (1) Crea una lista (agentesValidos) vacía.
         *  
         *  (2) Para cada Agente agente en el argumento:
         *  
         *      (2.1) Si es diferente al componente Agente del gameObject, lo inserta en la lista.
         *      
         *      (2.2) Si no, no hace nada.
         *      
         *  (3) Actualiza _targets con la lista.
         */
        set
        {
            Agente[] argumento = value;

            List<Agente> agentesValidos = new List<Agente>();

            foreach(Agente agente in argumento)
            {
                if(agente != gameObject.GetComponent<Agente>())
                {
                    agentesValidos.Add(agente);
                }
            }

            _targets = agentesValidos.ToArray();
        }
    }

}
