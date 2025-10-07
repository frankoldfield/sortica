using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{

    public Vector2Int Position { get; private set; }
    public Vector3 Center { get; private set; }
    public bool IsObstacle { get; set; }
    public GameObject rayCastCube { get; set; }

    public Tile(Vector2Int position, float tileSize, GameObject gridPrefab, GameObject obstaclePrefab, bool debug)
    {
        
        Position = position;
        Center = new Vector3(position.x * tileSize, 0, position.y * tileSize);
        
        this.rayCastCube = GameObject.Instantiate(gridPrefab, Center + new Vector3(0, 1/2, 0), Quaternion.identity);
        Renderer rend = this.rayCastCube.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.enabled = false; // Oculta el objeto sin desactivarlo completamente
        }
        CheckIfObstacle(tileSize, debug);
        if(debug) {
            if(IsObstacle) {
                GameObject.Instantiate(obstaclePrefab, Center, Quaternion.identity);
            }
            else {
                GameObject.Instantiate(gridPrefab, Center, Quaternion.identity);
            }
        }
    }

    public void CheckIfObstacle(float tileSize, bool debug)
    {
        Vector3 direccion = new Vector3(0, 1, 0).normalized;
        Vector3 boxSize = new Vector3(tileSize, 1, tileSize);
        Vector3 posicion = rayCastCube.transform.position - new Vector3(0, 1, 0);
        IsObstacle = Physics.BoxCast(posicion, boxSize / 2, direccion, out RaycastHit hit, Quaternion.identity, 10f, ~LayerMask.GetMask("Grid"));
        if(debug)
        {
            Debug.DrawRay(posicion, direccion * 10f, Color.red, 10f);
        }
    }
}
