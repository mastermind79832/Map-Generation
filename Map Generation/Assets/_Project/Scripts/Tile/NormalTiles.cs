using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGeneration
{
	public class NormalTiles : Tile
	{
		public override void Initialize()
		{
			SetType(TileType.Normal);
		}
	}
}
