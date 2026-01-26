using Photon.Pun;
using UnityEngine;

public class SimpleGameManager : MonoBehaviour
{
    private void Start()
    {
        int randX = Random.Range(-5, 6);
        int randZ = Random.Range(-5, 6);
        Vector3 spawnPos = new Vector3(randX, 1, randZ);
        
        PhotonNetwork.Instantiate("Player", spawnPos, Quaternion.identity);
    }
}
