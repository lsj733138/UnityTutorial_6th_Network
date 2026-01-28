using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    public Tilemap TileMap { get; private set; }

    public GameObject[] minerals;
    
    private void Awake()
    {
        TileMap = GetComponent<Tilemap>();
    }

    [ServerRpc(RequireOwnership = false)]
    public void RemoveTileServerRpc(Vector3Int cellPos)
    {
        RemoveTile(cellPos);
    }
    
    // 타일을 삭제하는 기능
    public void RemoveTile(Vector3Int cellPos)
    {
        int randomValue = Random.Range(0, 101);

        if (randomValue >= 70) // 드롭율 30퍼센트
        {
            int randomIndex = Random.Range(0, minerals.Length);
            GameObject mineral = Instantiate(minerals[randomIndex], cellPos, Quaternion.identity);

            mineral.GetComponent<NetworkObject>().Spawn(); // 동기화
        }
        
        TileMap.SetTile(cellPos, null);
    }
}
