using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGeneration
{
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] private GridSetting[] m_GridSetting;
		
		private Tile[,] m_Tiles;
		private GridSetting m_CurrentSetting;
		private Vector3 m_SpawnTilePos;

		private int m_NormalEmptyTileCount;

		public void CreateGrid()
		{
			if(m_Tiles != null)
				CleanTiles();
			m_CurrentSetting = GetRandomSetting();
			CreateNewTiles();
		}
		private GridSetting GetRandomSetting() => m_GridSetting[UnityEngine.Random.Range(0, m_GridSetting.Length)];
		
		// Destroy tiles
		private void CleanTiles()
		{
			for (int x = 0; x < m_CurrentSetting.MaxCount.x; x++)
			{
				for (int y = 0; y < m_CurrentSetting.MaxCount.y; y++)
				{
					Destroy(m_Tiles[x,y].gameObject);
				}
			}
			m_NormalEmptyTileCount = 0;
			m_Tiles = null;
		}
		
		// Creation
		private void CreateNewTiles()
		{
			m_Tiles = new Tile[m_CurrentSetting.MaxCount.x, m_CurrentSetting.MaxCount.y];
			CreateRoadsTiles();
			CreateNormalTiles();
			CreateItems();
		}
		private void CreateRoadsTiles()
		{
			if (m_CurrentSetting.roadSetting == null)
			{
				Debug.LogError("Road Setting not Set");
				return;
			}

			RoadTile prefab = m_CurrentSetting.roadSetting.roadPrefab;
			Vector2Int[] cordinates = m_CurrentSetting.roadSetting.Cordinates;
			for (int i = 0; i < cordinates.Length; i++)
			{
				if (CheckInBounds(cordinates[i]))
				{
					Debug.LogError($"Road cordinate: {cordinates[i]} does not match total index");
					continue;
				}

				if (m_Tiles[cordinates[i].x, cordinates[i].y] != null)
				{
					Debug.LogWarning($"Duplicate {cordinates[i]} Road cordinate");;
					continue;
				}

				SpawnTile(prefab, cordinates[i]);
			}
		}

		private bool CheckInBounds(Vector2Int index)
		{
			return index.x < 0 ||
				index.y < 0 ||
				index.x >= m_CurrentSetting.MaxCount.x ||
				index.y >= m_CurrentSetting.MaxCount.y;
		}

		private void CreateNormalTiles()
		{
			Vector2Int index = Vector2Int.zero;
			for (index.x = 0; index.x < m_CurrentSetting.MaxCount.x; index.x++)
			{
				for (index.y = 0; index.y < m_CurrentSetting.MaxCount.y; index.y++)
				{
					if (m_Tiles[index.x, index.y] != null)
						continue;
					SpawnTile(m_CurrentSetting.normalPrefab, index);
					m_NormalEmptyTileCount++;
				}
			}
		}

		private void CreateItems()
		{
			// Spawn Items Here
			if (m_CurrentSetting.itemSetting == null || m_CurrentSetting.roadSetting == null || m_CurrentSetting.roadSetting.Cordinates.Length <= 0)
			{
				Debug.LogError("Item Setting not Set");
				return;
			}

			int itemCount;
			Vector2Int randomIndex = Vector2Int.zero;
			OnTileItem newItem;
			int loopCount;

			foreach (ItemSetting setting in m_CurrentSetting.itemSetting)
			{
				itemCount = setting.maxCount;
				if (itemCount > m_NormalEmptyTileCount)
					continue;
				loopCount = 0;
				while (itemCount > 0)
				{
					randomIndex = m_CurrentSetting.roadSetting.Cordinates[UnityEngine.Random.Range(0, m_CurrentSetting.roadSetting.Cordinates.Length)];

					if(GetAdjacentFreeTile(ref randomIndex, setting.spawnTile, out Vector2Int freeIndex, out Vector3 direction))
					{
						newItem = Instantiate(setting.itemPrefab);
						newItem.transform.forward = -direction;
						m_Tiles[freeIndex.x, freeIndex.y].SetOnTileItem(newItem);
						m_NormalEmptyTileCount--;
						itemCount--;
					}
					else
						loopCount++;

					if (loopCount >= 5000)
					{
						Debug.LogWarning($"Loop exceeded limit {randomIndex}, {itemCount}");
						itemCount--;
					}
				}
			}
		}

		//  for checking vertical and horizontal adjacent tile
		Vector2Int[] adjacentIndexArray = { new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0) };
		private bool GetAdjacentFreeTile(ref Vector2Int index, TileType spawnTile, out Vector2Int freeIndex , out Vector3 direction)
		{
			Vector2Int currIndex = index;
			
			direction = Vector3.zero;
			freeIndex = Vector2Int.zero;

			for (int i = 0; i < adjacentIndexArray.Length; i++)
			{
				currIndex.x = index.x + adjacentIndexArray[i].x;
				currIndex.y = index.y + adjacentIndexArray[i].y;
				if (CheckInBounds(currIndex))
					continue;
				if (CheckTileAvailability(ref currIndex, spawnTile))
				{
					freeIndex = currIndex;
					direction.x = adjacentIndexArray[i].x;
					direction.z = adjacentIndexArray[i].y;
					return true;
				}
			}
			return false;
		}

		private float GetAngle(Vector2Int direction)
		{
			if (direction.x == 0 && direction.y == 1)
			{
				return 90f;
			}

			return 0;
		}

		private bool CheckTileAvailability(ref Vector2Int index, TileType spawnTile)
		{
			return m_Tiles[index.x, index.y].GetType() == spawnTile &&
				!m_Tiles[index.x, index.y].IsItemOccupied();
		}

		private void SpawnTile(Tile prefab, Vector2Int index)
		{
			m_SpawnTilePos = Vector3.zero;
			m_Tiles[index.x, index.y] = Instantiate(prefab);
			m_Tiles[index.x, index.y].Initialize();
			m_Tiles[index.x, index.y].transform.parent = transform;
			m_SpawnTilePos.x = index.x;
			m_SpawnTilePos.z = index.y;
			m_Tiles[index.x, index.y].transform.position = m_CurrentSetting.tileSize * m_SpawnTilePos;
		}
	}
}
