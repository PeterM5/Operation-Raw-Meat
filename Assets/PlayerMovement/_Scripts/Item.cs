using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType {
    Gun,
    Shield
}

public class Item: MonoBehaviour
{
    public ItemType type;
}
