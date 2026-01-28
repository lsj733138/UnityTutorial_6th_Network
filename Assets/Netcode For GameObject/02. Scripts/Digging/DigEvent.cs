using Unity.Netcode;
using UnityEngine;

public class DigEvent : NetworkBehaviour
{
    private TilemapManager tilemapManager;
    public LayerMask tileLayer;
    
    public Transform[] hitPoints;

    void Start()
    {
        tilemapManager = FindFirstObjectByType<TilemapManager>();
    }

    public void OnDig()
    {
        for (int i = 0; i < hitPoints.Length; i++)
        {
            Collider2D coll = Physics2D.OverlapCircle(hitPoints[i].position, 0.5f, tileLayer);

            if (coll != null)
            {
                // 월드 포지션 값을 타일의 셀 포지션 값으로 변경하는 기능
                Vector3Int cellPos = tilemapManager.TileMap.WorldToCell(hitPoints[i].position);

                if (tilemapManager.TileMap.GetTile(cellPos) != null)
                {
                    if (IsOwner)
                    {
                        tilemapManager.RemoveTileServerRpc(cellPos);
                        break;
                    }
                }
            }
        }
    }
}