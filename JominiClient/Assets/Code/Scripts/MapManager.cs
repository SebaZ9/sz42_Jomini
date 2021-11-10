using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using ProtoMessageClient;
using System;
using System.Threading;
using TMPro;

public class MapManager : Controller
{

    public Tile dirt;
    public Tile rock;
    public Tile water;
    public Color testColour;

    [SerializeField] private Tilemap map;

    [SerializeField] private TileBase whiteTile;

    [SerializeField] private GameObject lblTest;

    [SerializeField] private Sprite whiteSprite;

    [SerializeField] private GameObject hexagonPrefab;

    [SerializeField] private Text lblPageTitle;
    [SerializeField] private Text lblMessageForUser;

    /// <summary>
    /// 2D array containing Fief IDs where the indices are their positions on the grid.
    /// </summary>
    private string[,] mapLocationsFiefIDs;
    private int worldX;
    private int worldY;

    public GameObject WorldSpaceCanvas;
    public TextMeshProUGUI baseText;
    private Dictionary<string, TextMeshProUGUI> textTags;

    //private Dictionary<string, Color> ownerColours;
    

    // Start is called before the first frame update
    public void Start()
    {
        CreateMap();
    }

    private void CreateMap() {
        ProtoWorldMap worldMap = (ProtoWorldMap)GetWorldMap(tclient);

        fiefNames.Clear();
        fiefOwners.Clear();
        ownerColours.Clear();

        for (int i = 0; i < worldMap.fiefIDNames.Length - 2; i+=3) {
            fiefNames.Add(worldMap.fiefIDNames[i], worldMap.fiefIDNames[i + 1]);
            fiefOwners.Add(worldMap.fiefIDNames[i], worldMap.fiefIDNames[i + 2]);

            if(!ownerColours.ContainsKey(worldMap.fiefIDNames[i + 2])) {
                ownerColours.Add(worldMap.fiefIDNames[i + 2], colours[ownerColours.Count]);
            }
        }

        mapLocationsFiefIDs = new string[worldMap.dimensionX, worldMap.dimensionY];
        worldX = worldMap.dimensionX;
        worldY = worldMap.dimensionY;
        textTags = new Dictionary<string, TextMeshProUGUI>();

        for (int x = 0; x < worldMap.dimensionX; x++) {
            for (int y = 0; y < worldMap.dimensionY; y++) {

                if (worldMap.gameMapLayout[x + (y * worldMap.dimensionX)].Equals("")) {
                    map.SetTile(new Vector3Int(x, -y + worldMap.dimensionY - 1, 0), water);
                } else {

                    if(fiefOwners[worldMap.gameMapLayout[x + (y * worldMap.dimensionX)]] == protoClient.activeChar.charID) {
                        map.SetTile(new Vector3Int(x, -y + worldMap.dimensionY - 1, 0), dirt);
                    } else {
                        Tile test = new Tile();
                        test.sprite = rock.sprite;
                        test.color = ownerColours[fiefOwners[worldMap.gameMapLayout[x + (y * worldMap.dimensionX)]]];
                        map.SetTile(new Vector3Int(x, -y + worldMap.dimensionY - 1, 0), test);
                    }

                    TextMeshProUGUI cellUI = Instantiate(baseText);
                    cellUI.transform.SetParent(WorldSpaceCanvas.transform);
                    cellUI.transform.SetPositionAndRotation(map.CellToWorld(new Vector3Int(x, -y + worldMap.dimensionY - 1, 0)) + new Vector3(4.925f, -2.525f, 0), Quaternion.identity);
                    cellUI.SetText(worldMap.gameMapLayout[x + (y * worldMap.dimensionX)]);
                    textTags.Add(worldMap.gameMapLayout[x + (y * worldMap.dimensionX)], cellUI);

                }
                mapLocationsFiefIDs[x, -y + worldMap.dimensionY - 1] = worldMap.gameMapLayout[x + y * worldMap.dimensionX];

            }
        }

    }

    // Update is called once per frame
    public void Update()
    {

        if (Input.GetMouseButtonDown(0)) {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPosition = map.WorldToCell(mousePosition);

            if(gridPosition.x >= 0 && gridPosition.x <= worldX && gridPosition.y >= 0 && gridPosition.y <= worldY) {
                if (string.IsNullOrWhiteSpace(mapLocationsFiefIDs[gridPosition.x, gridPosition.y])) {
                    Debug.Log("Water");
                } else {
                    Debug.Log($"{gridPosition.x} | {gridPosition.y} | {mapLocationsFiefIDs[gridPosition.x, gridPosition.y]}");
                    fiefToViewID = mapLocationsFiefIDs[gridPosition.x, gridPosition.y];
                    GoToScene(SceneName.ViewFief);
                }
            }        
            /*
            try {
                string selectedFiefID = mapLocationsFiefIDs[-gridPosition.y, gridPosition.x];

                print(gridPosition.x + " " + gridPosition.y + " " + selectedFiefID);

                // Clicked on water.
                if (string.IsNullOrWhiteSpace(selectedFiefID)) {
                    return;
                }

                fiefToViewID = selectedFiefID;
                //GoToScene(SceneName.ViewFief);
            } catch (IndexOutOfRangeException) {
                // IndexOutOfRangeException
            }*/


        }
    }
}
