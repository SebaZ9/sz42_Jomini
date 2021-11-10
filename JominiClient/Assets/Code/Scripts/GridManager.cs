using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField]
    private GameObject hexPrefab;
    private int rows = 5;
    private int cols = 8;
    private float tileSize = 1;

    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid() {
        GameObject referenceTile = (GameObject)Instantiate(hexPrefab);

        for(int row = 0; row < rows; row++) {
            for(int col = 0; col < cols; col++) {
                GameObject tile = (GameObject)Instantiate(referenceTile);

                float posX = col * tileSize;
                float posY = row * -tileSize;

                tile.transform.position = new Vector2(posX, posY);
            }
        }

        Destroy(referenceTile);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
