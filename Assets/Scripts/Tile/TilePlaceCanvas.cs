using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TilePlaceCanvas : MonoBehaviour
{
    [SerializeField] private GameObject model;
    [SerializeField] private GameObject levelTextObject;
    [SerializeField] private GameObject costTextObject;

    [SerializeField] private Mesh[] tileMeshes;
    [SerializeField] private TileLevel[] tileLevels;

    public void SetTilePlaceCanvas(int tileLevel)
    {
        int newTileLevel = tileLevel + 1;
        levelTextObject.GetComponent<TextMeshPro>().text = "LEVEL: " + tileLevel + ">" + newTileLevel;
        costTextObject.GetComponent<TextMeshPro>().text = "COST: " + tileLevels[tileLevel].price + "C";
        model.GetComponent<MeshFilter>().mesh = tileMeshes[tileLevel];
    }
}
