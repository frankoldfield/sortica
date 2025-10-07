using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * La clase Steering está compuesta por dos atributos: una aceleración lineal y una aceleración angular.
 */
public class Steering
{
    /*
     * Aceleración angular.
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
     * Aceleración lineal.
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
