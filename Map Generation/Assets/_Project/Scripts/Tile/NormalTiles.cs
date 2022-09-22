using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGeneration
{
	public class NormalTiles : Tile
	{
		protected override void Initialize()
		{
			SetType(TileType.Normal);
		}
	}
}
