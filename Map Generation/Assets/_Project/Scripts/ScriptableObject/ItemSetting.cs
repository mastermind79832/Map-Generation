using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGeneration
{
    [CreateAssetMenu(fileName = "new Item", menuName = "Map/Create Item Setting")]
    public class ItemSetting : ScriptableObject
    {
        public OnTileItem itemPrefab;
        public int maxCount;
    }
}
