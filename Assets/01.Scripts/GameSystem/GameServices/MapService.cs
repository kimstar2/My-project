using System;
using _TevLib.ServiceLocatorSystem;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _01.Scripts.GameSystem.GameServices
{
    public class MapService : MonoBehaviour , IMapService
    {
        [SerializeField] private Grid grid;
        [SerializeField] private Tilemap soundTile;

        private void Awake()
        {
            ServiceLocator.RegisterService<IMapService>(this);
            if (soundTile != null)
                soundTile.GetComponent<TilemapRenderer>().enabled = false;
        }

        public Vector3 GetCallCenterToWorld(Vector3Int cellPos) => grid.GetCellCenterWorld(cellPos);

        public Vector3Int GetWorldToCell(Vector3 worldPos) => grid.WorldToCell(worldPos);

        public Vector3 GetCellToWorld(Vector3Int cellPos) => grid.CellToWorld(cellPos);

        public void EnterSoundTile(Vector3 worldPos)
        {
            
        }
    }
}