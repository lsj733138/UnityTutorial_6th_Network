using Photon.Pun;
using Unity.Cinemachine;
using UnityEngine;

public class FightGameManager : MonoBehaviour
{
    private void Start()
    {
        float randX = Random.Range(-10, 11);
        float randZ = Random.Range(-10, 11);
        Vector3 spawnPos = new Vector3(randX, 0, randZ);

        int randomIndex = Random.Range(0, 4);

        string characterName = "Player_" + randomIndex;
        GameObject character = PhotonNetwork.Instantiate(characterName, spawnPos, Quaternion.identity);

        CinemachineCamera followCamera = FindFirstObjectByType<CinemachineCamera>();
        followCamera.Follow = character.transform.GetChild(0);
    }
}
