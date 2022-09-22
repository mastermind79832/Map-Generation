using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MapGeneration
{
    public enum TileType
	{
        Normal  = 0,
        Road    = 1
	}

    public abstract class Tile : MonoBehaviour
    {
        private TileType m_Type;
        private OnTileItem m_Item;

		protected abstract void Initialize();      
        // Setter
        protected void SetType(TileType type) => m_Type = type;
        protected void SetOnTileItem(OnTileItem item)
        {
            m_Item = item;
            item.transform.parent = transform;
            item.transform.position = Vector3.zero;
        }
        public new TileType GetType() => m_Type;
    }
}