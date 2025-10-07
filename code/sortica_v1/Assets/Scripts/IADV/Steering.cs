using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * La clase Steering est� compuesta por dos atributos: una aceleraci�n lineal y una aceleraci�n angular.
 */
public class Steering
{
    /*
     * Aceleraci�n angular.
     */
    private float _angular;
    public float Angular
    {
        get
        {
            return _angular;
        }

        set
        {
            _angular = value;
        }
    }

    /*
     * Aceleraci�n lineal.
     */
    private Vector3 _lineal;
    public Vector3 Lineal
    {
        get
        {
            return _lineal;
        }

        set
        {
            if(value.magnitude < 0)
            {
                _lineal = new Vector3(0, 0, 0);
            }
            else
            {
                _lineal = value;
            }
        }
    }
}
