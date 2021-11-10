using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapStuff : MonoBehaviour
{
    public Tile ThisTile;

    public Sprite forNewTile;

    IEnumerator Start()
    {
        var subGO = transform.GetChild(0).gameObject;

		var tm = subGO.GetComponent<Tilemap>();

        var pos = new Vector3Int(-3, 1, 0);

        yield return new WaitForSeconds(2.0f);

        tm.SetTile(pos, ThisTile);

        var t = tm.GetTile( pos);

        Debug.Log(t.ToString());

        yield return new WaitForSeconds(2.0f);

        var t2 = ScriptableObject.CreateInstance<Tile>();
        t2.sprite = forNewTile;

        tm.SetTile(pos, t2);
    }
}
