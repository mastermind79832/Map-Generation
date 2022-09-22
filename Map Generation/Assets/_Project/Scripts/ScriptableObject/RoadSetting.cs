using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGeneration
{
	[CreateAssetMenu(fileName = "new Road", menuName = "Map/Create Road Setting")]
    public class RoadSetting : ScriptableObject
    {
        public RoadTile roadPrefab;
        public Vector2Int[] Cordinates;
    }
}
