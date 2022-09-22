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
		private List<Vector2Int> roadTiles;

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
			roadTiles.Clear();
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
			roadTiles = m_CurrentSetting.roadSetting.Cordinates;
			for (int i = 0; i < roadTiles.Count; i++)
			{
				if (CheckInBounds(roadTiles[i]))
				{
					Debug.LogError($"Road cordinate: {roadTiles[i]} does not match total index");
					roadTiles.Remove(roadTiles[i]);
					continue;
				}

				if (m_Tiles[roadTiles[i].x, roadTiles[i].y] != null)
				{
					Debug.LogWarning($"Duplicate {roadTiles[i]} Road cordinate");
					roadTiles.Remove(roadTiles[i]);
					continue;
				}

				SpawnTile(prefab, roadTiles[i]);
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
			if (m_CurrentSetting.itemSetting == null)
			{
				Debug.LogError("Item Setting not Set");
				return;
			}
			int itemCount;
			Vector2Int randomIndex = Vector2Int.zero;
			OnTileItem newItem;
			foreach (ItemSetting setting in m_CurrentSetting.itemSetting)
			{
				itemCount = setting.maxCount;
				if (itemCount > m_NormalEmptyTileCount)
					continue;

				while (itemCount > 0)
				{
					randomIndex.x = UnityEngine.Random.Range(0, m_CurrentSetting.MaxCount.x);
					randomIndex.y = UnityEngine.Random.Range(0, m_CurrentSetting.MaxCount.y);

					if (CheckTileAvailability(ref randomIndex, setting.spawnTile))
					{
						newItem = Instantiate(setting.itemPrefab);
						m_Tiles[randomIndex.x, randomIndex.y].SetOnTileItem(newItem);
						itemCount--;
					}
				}
			}
		}

		private bool CheckTileAvailability(ref Vector2Int randomIndex, TileType spawnTile)
		{
			return m_Tiles[randomIndex.x, randomIndex.y].GetType() == spawnTile &&
				!m_Tiles[randomIndex.x, randomIndex.y].IsItemOccupied();
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
