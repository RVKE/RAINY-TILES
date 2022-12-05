using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TileLevel", menuName = "TileLevel")]

public class TileLevel : ScriptableObject
{
    public new string name;
    public int level;
    public int health;
    public int price;
    public int nextPrice;
    public bool isTile;
    public GameObject prefab;
}
