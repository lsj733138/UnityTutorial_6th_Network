using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class FinderGameManager : NetworkBehaviour
{
    public static FinderGameManager Instance;

    public GameObject npcPrefab;

    public Transform[] spawnPoints;
    public int spawnNpcAmount = 10;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void NpcSpawnServerRpc()
    {
        for (int i = 0; i < spawnNpcAmount; i++)
        {
            GameObject npc = Instantiate(npcPrefab);
            
            int index = Random.Range(0, spawnPoints.Length);
            var randomPos = spawnPoints[index].position;

            npc.transform.position = randomPos;

            npc.GetComponent<NetworkObject>().Spawn();
        }
    }
}
