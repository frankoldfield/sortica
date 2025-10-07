using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public float velocidadAngular = 1000;

    void Update()
    {
        transform.Rotate(Vector3.up * velocidadAngular * Time.deltaTime, Space.World);
    }
}