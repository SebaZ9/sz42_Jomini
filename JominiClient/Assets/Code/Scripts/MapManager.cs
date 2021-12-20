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
    public TMP_Text province;
    public TMP_Text provOwner;

    [Header("ProvinceEdges")]
    private Dictionary<string, string> fiefProvince;
    private Dictionary<string, string> provinceName;
    private Dictionary<string, string> provinceOwner;
    public Texture2D TR;
    public Texture2D R;
    public Texture2D BR;
    public Texture2D BL;
    public Texture2D L;
    public Texture2D TL;


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
        fiefProvince = new Dictionary<string, string>();
        provinceName = new Dictionary<string, string>();
        provinceOwner = new Dictionary<string, string>();

        fiefNames.Clear();
        fiefOwners.Clear();
        ownerColours.Clear();

        for (int i = 0; i < worldMap.fiefIDNames.Length - 2; i+=3) {
            fiefNames.Add(worldMap.fiefIDNames[i], worldMap.fiefIDNames[i + 1]);
            fiefOwners.Add(worldMap.fiefIDNames[i], worldMap.fiefIDNames[i + 2]);
            fiefProvince.Add(worldMap.fiefIDNames[i], worldMap.provinceID[i/3]);
            provinceName.Add(worldMap.fiefIDNames[i], worldMap.provinceName[i/3]);
            provinceOwner.Add(worldMap.fiefIDNames[i], worldMap.provinceOwner[i/3]);

            if(!ownerColours.ContainsKey(worldMap.fiefIDNames[i + 2])) {
                ownerColours.Add(worldMap.fiefIDNames[i + 2], colours[ownerColours.Count]);
            }
        }
        

        worldX = worldMap.dimensionX + 1;
        worldY = worldMap.dimensionY + 1;
        mapLocationsFiefIDs = new string[worldX, worldY];
        for (int x = 0; x < mapLocationsFiefIDs.Length / worldY; x++)
        {
            for (int y = 0; y < mapLocationsFiefIDs.Length / worldX; y++)
            {
               mapLocationsFiefIDs[x, y] = "";
            }
        }
        textTags = new Dictionary<string, TextMeshProUGUI>();
        Debug.Log("Current Family ID: " + protoClient.activeChar.familyID);
        for (int x = 0; x < worldMap.dimensionX; x++) {
            for (int y = 0; y < worldMap.dimensionY; y++) {

                if (worldMap.gameMapLayout[x + (y * worldMap.dimensionX)].Equals("")) {

                } else {

                    if(fiefOwners[worldMap.gameMapLayout[x + (y * worldMap.dimensionX)]] == protoClient.activeChar.familyID) {
                        Tile test = (Tile)ScriptableObject.CreateInstance("Tile");
                        test.sprite = dirt.sprite;
                        test.color = new Color(0.3f, 0.6f, 0.4f, 1f);
                        map.SetTile(new Vector3Int(x, -y + worldMap.dimensionY - 1, 0), test);
                        
                    } else {
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

        for (int x = 0; x < mapLocationsFiefIDs.Length / worldY; x++)
        {
            for (int y = 0; y < mapLocationsFiefIDs.Length / worldX; y++)
            {
                
                if (!mapLocationsFiefIDs[x, y].Equals(""))
                {
                    ProcessFiefProvince(x, y);
                }
            }
        }

    }

    private Sprite GetProvinceBorders(bool TR, bool R, bool BR, bool BL, bool L, bool TL)
    {
        Texture2D testTexture = new Texture2D(120, 140);
        Color[] coloursTR = this.TR.GetPixels();
        Color32[] coloursR = this.R.GetPixels32(0);
        Color32[] coloursBR = this.BR.GetPixels32(0);
        Color32[] coloursBL = this.BL.GetPixels32(0);
        Color32[] coloursL = this.L.GetPixels32(0);
        Color32[] coloursTL = this.TL.GetPixels32(0);

        Color[] finalColours = new Color[coloursBL.Length];

        for (int i = 0; i < coloursBL.Length; i++)
        {
            if (TR)
            {
                finalColours[i].r += coloursTR[i].r;
                finalColours[i].g += coloursTR[i].g;
                finalColours[i].b += coloursTR[i].b;
                finalColours[i].a += coloursTR[i].a;
            }
            if (R)
            {
                finalColours[i].r += coloursR[i].r;
                finalColours[i].g += coloursR[i].g;
                finalColours[i].b += coloursR[i].b;
                finalColours[i].a += coloursR[i].a;
            }
            if (BR)
            {
                finalColours[i].r += coloursBR[i].r;
                finalColours[i].g += coloursBR[i].g;
                finalColours[i].b += coloursBR[i].b;
                finalColours[i].a += coloursBR[i].a;
            }
            if (BL)
            {
                finalColours[i].r += coloursBL[i].r;
                finalColours[i].g += coloursBL[i].g;
                finalColours[i].b += coloursBL[i].b;
                finalColours[i].a += coloursBL[i].a;
            }
            if (L)
            {
                finalColours[i].r += coloursL[i].r;
                finalColours[i].g += coloursL[i].g;
                finalColours[i].b += coloursL[i].b;
                finalColours[i].a += coloursL[i].a;
            }
            if (TL)
            {
                finalColours[i].r += coloursTL[i].r;
                finalColours[i].g += coloursTL[i].g;
                finalColours[i].b += coloursTL[i].b;
                finalColours[i].a += coloursTL[i].a;
            }
        }

        testTexture.SetPixels(finalColours);
        testTexture.Apply();
        return Sprite.Create(testTexture, new Rect(0f, 0f, testTexture.width, testTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
    }

    private void ProcessFiefProvince(int x, int y)
    {
        string currentProvince = fiefProvince[mapLocationsFiefIDs[x, y]];
        bool provinceTR = true;
        bool provinceR = true;
        bool provinceBR = true;
        bool provinceBL = true;
        bool provinceL = true;
        bool provinceTL = true;
        if (y % 2 == 0)             // TR
        {
            if (fiefProvince.ContainsKey(mapLocationsFiefIDs[x, y + 1]) && ((x) * (y + 1) < mapLocationsFiefIDs.Length))
            {
                provinceTR = fiefProvince[mapLocationsFiefIDs[x, y + 1]] != currentProvince;
            }
        }
        else
        {
            if (fiefProvince.ContainsKey(mapLocationsFiefIDs[x + 1, y + 1]))
            {
                provinceTR = fiefProvince[mapLocationsFiefIDs[x + 1, y + 1]] != currentProvince;

            }
        }
        if (fiefProvince.ContainsKey(mapLocationsFiefIDs[x + 1, y]))
            provinceR = fiefProvince[mapLocationsFiefIDs[x + 1, y]] != currentProvince;
        if (y % 2 == 0)            // BR
        {
            if (fiefProvince.ContainsKey(mapLocationsFiefIDs[x, y - 1]))
                provinceBR = fiefProvince[mapLocationsFiefIDs[x, y - 1]] != currentProvince;
        }
        else
        {
            if (fiefProvince.ContainsKey(mapLocationsFiefIDs[x + 1, y - 1]))
                provinceBR = fiefProvince[mapLocationsFiefIDs[x + 1, y - 1]] != currentProvince;
        }
        if (y % 2 == 0)             // BL
        {
            if (fiefProvince.ContainsKey(mapLocationsFiefIDs[x - 1, y - 1]))
                provinceBL = fiefProvince[mapLocationsFiefIDs[x - 1, y - 1]] != currentProvince;
        }
        else
        {
            if (fiefProvince.ContainsKey(mapLocationsFiefIDs[x, y - 1]))
                provinceBL = fiefProvince[mapLocationsFiefIDs[x, y - 1]] != currentProvince;
        }
        if (fiefProvince.ContainsKey(mapLocationsFiefIDs[x - 1, y]))
            provinceL = fiefProvince[mapLocationsFiefIDs[x - 1, y]] != currentProvince;
        if (y % 2 == 0)             // TL
        {
            if (fiefProvince.ContainsKey(mapLocationsFiefIDs[x - 1, y + 1]))
                provinceTL = fiefProvince[mapLocationsFiefIDs[x - 1, y + 1]] != currentProvince;
        }
        else
        {
            if (fiefProvince.ContainsKey(mapLocationsFiefIDs[x, y + 1]))
                provinceTL = fiefProvince[mapLocationsFiefIDs[x, y + 1]] != currentProvince;
        }
        


        Sprite provinceSprite = GetProvinceBorders(provinceTR, provinceR, provinceBR, provinceBL, provinceL, provinceTL);
        Tile provinceTile = (Tile)ScriptableObject.CreateInstance("Tile");
        provinceTile.sprite = provinceSprite;
        overlayMap.SetTile(new Vector3Int(x, y, 0), provinceTile);

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
        province.text = "Province: " + provinceName[fiefToViewID];
        provOwner.text = "Owner: " + GetCharName(provinceOwner[fiefToViewID]);
        //ProtoProvince result = GetProtoProvince(fiefProvince[fiefToViewID]);
        //if (result != null)
        //{
        //     Debug.Log($"Fiefs Province: {result.name}");
        //}
    }

    private ProtoProvince GetProtoProvince(string provinceName)
    {
        Debug.Log($"Fiefs Province: {provinceName}");
        ProtoMessage msg = GetProvince(provinceName, tclient);
        switch (msg.ResponseType)
        {
            case DisplayMessages.GetProvince:
                {
                    return (ProtoProvince)msg;
                }
            case DisplayMessages.Error:
            default:
                {
                    return null;
                }
            
        }
    }

    private void SelectTiles(string charId) {

        foreach (Vector3Int selected in selectedTiles) {
            map.SetTile(selected, dirt);
        }
        selectedTiles.Clear();

        if (charId != protoClient.activeChar.familyID) {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            for (int x = 0; x < worldX; x++) {
                for (int y = 0; y < worldY; y++) {
                    if(!mapLocationsFiefIDs[x, y].Equals("")) {
                        if (fiefOwners[mapLocationsFiefIDs[x, y]] == charId) {
                            Tile colouredTile = (Tile)ScriptableObject.CreateInstance("Tile");
                            colouredTile.sprite = dirt.sprite;
                            colouredTile.color = new Color(0.8f, 0.4f, 0.4f, 1f);
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
        base.Update();
        if (Input.GetMouseButtonDown(0)) {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPosition = map.WorldToCell(mousePosition);

            if (!MouseRayCast()) {

                if (gridPosition.x >= 0 && gridPosition.x <= worldX && gridPosition.y >= 0 && gridPosition.y <= worldY) {
                    if (string.IsNullOrWhiteSpace(mapLocationsFiefIDs[gridPosition.x, gridPosition.y])) {

                    } else {
                        fiefToViewID = mapLocationsFiefIDs[gridPosition.x, gridPosition.y];
                        SelectTiles(fiefOwners[fiefToViewID]);
                        UpdateFiefPanel();

                        

                    }
                }

            }
        }
    }
}
