using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGeneration
{
    [CreateAssetMenu(fileName = "new Grid", menuName = "Map/Create Grid Setting")]
    public class GridSetting : ScriptableObject
    {
		[Header("Main Setting")]
        public NormalTiles normalPrefab;
        public float tileSize;
        public Vector2Int MaxCount;

		[Header("Other Settings")]
        public RoadSetting roadSetting;
		[Tooltip("Put important ones first")]
        public ItemSetting[] itemSetting;
    }
}
