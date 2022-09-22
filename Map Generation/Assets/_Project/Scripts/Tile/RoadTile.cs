using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGeneration
{
	public class RoadTile : Tile
	{
		public override void Initialize()
		{
			SetType(TileType.Road);
		}
	}
}