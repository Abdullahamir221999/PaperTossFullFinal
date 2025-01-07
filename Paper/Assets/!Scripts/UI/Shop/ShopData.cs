using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "BallData", menuName = "ScriptableObjects/BallData", order = 1)]
public class ShopData : ScriptableObject
{
    public ShopItem[] shopItems;
}

[System.Serializable]
public class ShopItem
{
    public string ballName;
    public Sprite ballSprite;
    public Material ballMaterial;
    public BallType ballType;
    public BallState ballState;
}

public enum BallType
{
    Common = 500,
    Rare = 1000
}

public enum BallState
{
    Bought,
    NotBought,
    Equipped,
}