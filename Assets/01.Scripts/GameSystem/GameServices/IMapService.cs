using UnityEngine;

namespace _01.Scripts.GameSystem.GameServices
{
    public interface IMapService
    {
        Vector3 GetCallCenterToWorld(Vector3Int cellPos);
        Vector3Int GetWorldToCell(Vector3 worldPos);
        Vector3 GetCellToWorld(Vector3Int cellPos);
        void EnterSoundTile(Vector3 worldPos);
    }
}