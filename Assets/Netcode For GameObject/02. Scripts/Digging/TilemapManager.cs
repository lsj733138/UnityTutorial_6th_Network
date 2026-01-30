using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : NetworkBehaviour
{
    public Tilemap TileMap { get; private set; }

    public GameObject[] minerals;

    private NetworkList<Vector3Int> destroyedTiles = new NetworkList<Vector3Int>();
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        TileMap = GetComponent<Tilemap>();
        
        destroyedTiles.OnListChanged += OnTileDestroyed;
        
        // 추후에 들어온 플레이어가 이미 파괴된 타일들의 내역을 찾아서 똑같이 파괴
        foreach (Vector3Int pos in destroyedTiles)
            TileMap.SetTile(pos, null);
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
        
        destroyedTiles.Add(cellPos); // DestroyTiles에 cellPos를 Add
    }

    private void OnTileDestroyed(NetworkListEvent<Vector3Int> changedEvent)
    {
        if (changedEvent.Type == NetworkListEvent<Vector3Int>.EventType.Add) // 매개변수로 들어온 changedEvent가 Add로 들어왔다면
        {
            TileMap.SetTile(changedEvent.Value, null);
        }
    }
}
