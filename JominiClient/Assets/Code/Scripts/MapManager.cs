using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using ProtoMessageClient;
using System;
using System.Threading;
using TMPro;
using UnityEngine.EventSystems;

public class MapManager : Controller
{

    public Tile dirt;
    public Tile rock;
    public Tile water;

    public List<Tile> overlayObjects;

    [SerializeField] private Tilemap map;
    [SerializeField] private Tilemap overlayMap;

    /// <summary>
    /// 2D array containing Fief IDs where the indices are their positions on the grid.
    /// </summary>
    private string[,] mapLocationsFiefIDs;
    private int worldX;
    private int worldY;
    private List<Vector3Int> selectedTiles;

    public GameObject WorldSpaceCanvas;
    public TextMeshProUGUI baseText;
    private Dictionary<string, TextMeshProUGUI> textTags;

    //private Dictionary<string, Color> ownerColours;
    [Header("Selected Fief")]
    public TMP_Text fiefName;
    public TMP_Text fiefCode;
    public TMP_Text fiefOwner;


    // Start is called before the first frame update
    public void Start()
    {
        selectedTiles = new List<Vector3Int>();
        CreateMap();
        fiefToViewID = protoClient.activeChar.location;
        UpdateFiefPanel();
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
                    //map.SetTile(new Vector3Int(x, -y + worldMap.dimensionY - 1, 0), water);
                } else {

                    if(fiefOwners[worldMap.gameMapLayout[x + (y * worldMap.dimensionX)]] == protoClient.activeChar.charID) {
                        Tile test = new Tile();
                        test.sprite = dirt.sprite;
                        test.color = ownerColours[fiefOwners[worldMap.gameMapLayout[x + (y * worldMap.dimensionX)]]];
                        map.SetTile(new Vector3Int(x, -y + worldMap.dimensionY - 1, 0), test);
                        //overlayMap.SetTile(new Vector3Int(x, -y + worldMap.dimensionY - 1, 0), overlayObjects[0]);
                    } else {
                        //Tile test = new Tile();
                        //test.sprite = dirt.sprite;
                        //test.color = ownerColours[fiefOwners[worldMap.gameMapLayout[x + (y * worldMap.dimensionX)]]];
                        map.SetTile(new Vector3Int(x, -y + worldMap.dimensionY - 1, 0), dirt);
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

    private bool MouseRayCast() {
        PointerEventData data = new PointerEventData(EventSystem.current);
        data.position = Input.mousePosition;

        List<RaycastResult> casts = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, casts);
        for(int i=0; i < casts.Count; i++) {
            if (casts[i].gameObject.tag != "IgnoreClick") {
                casts.RemoveAt(i);
                i--;
            }
        }
        return casts.Count > 0;
    }

    private void UpdateFiefPanel() {
        fiefCode.text = fiefToViewID;
        fiefName.text = fiefNames[fiefToViewID];
        fiefOwner.text = "Owner: " + GetCharName(fiefOwners[fiefToViewID]);
    }

    private void SelectTiles(string charId) {

        foreach (Vector3Int selected in selectedTiles) {
            map.SetTile(selected, dirt);
        }
        selectedTiles.Clear();

        if (charId != protoClient.activeChar.charID) {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPosition = map.WorldToCell(mousePosition);   

            

            for (int x = 0; x < worldX; x++) {
                for (int y = 0; y < worldY; y++) {
                    if(!mapLocationsFiefIDs[x, y].Equals("")) {
                        if (fiefOwners[mapLocationsFiefIDs[x, y]] == charId) {
                            Debug.Log("UPDATE COLOR");
                            Tile colouredTile = new Tile {
                                sprite = dirt.sprite,
                                color = new Color(.15f, 0.5f, 0.35f, 1f)
                            };
                            map.SetTile(new Vector3Int(x, y, 0), colouredTile);
                            selectedTiles.Add(new Vector3Int(x, y, 0));
                        }
                    }    
                }
            }
        }
    }

    public void ViewSelectedFief() {
        GoToScene(SceneName.ViewFief);
    }

    // Update is called once per frame
    public void Update()
    {

        if (Input.GetMouseButtonDown(0)) {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPosition = map.WorldToCell(mousePosition);

            if (!MouseRayCast()) {

                if (gridPosition.x >= 0 && gridPosition.x <= worldX && gridPosition.y >= 0 && gridPosition.y <= worldY) {
                    if (string.IsNullOrWhiteSpace(mapLocationsFiefIDs[gridPosition.x, gridPosition.y])) {
                        Debug.Log("Water");
                    } else {
                        fiefToViewID = mapLocationsFiefIDs[gridPosition.x, gridPosition.y];
                        SelectTiles(fiefOwners[fiefToViewID]);
                        UpdateFiefPanel();
                    }
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
