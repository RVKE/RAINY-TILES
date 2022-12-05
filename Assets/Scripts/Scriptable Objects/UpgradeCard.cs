using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New UpgradeCard", menuName = "UpgradeCard")]

public class UpgradeCard : ScriptableObject
{
    public new string name;
    public string description;
    public int price;
    public int researchOrder;
}
