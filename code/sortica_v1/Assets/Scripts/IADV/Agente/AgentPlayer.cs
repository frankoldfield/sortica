using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentPlayer : Agente
{
    private new void Awake()
    {
        base.Awake();
    }

    /*
     * El método Update puede dividirse en tres pasos:
     * 
     *  (1) Recoger el Input del jugador (Input.GetAxis(...)).
     *  (2) Mover al personaje en relación a dicho Input a la velocidad máxima permitida.
     *  (3) Rotar al personaje para que mire en dirección al movimiento.
     */
    void Update()
    {
        // (1) Recoger Input.
        float movimientoX = Input.GetAxis("Horizontal");
        float movimientoZ = Input.GetAxis("Vertical");

        // (2) Mover al personaje.
        Velocidad = new Vector3(movimientoX, 0, movimientoZ).normalized * VelocidadMaxima;
        transform.Translate(Velocidad * Time.deltaTime, Space.World);

        // (3) Rotar al personaje.
        transform.LookAt(transform.position + Velocidad);
        Orientacion = transform.rotation.eulerAngles.y;
    }
}
