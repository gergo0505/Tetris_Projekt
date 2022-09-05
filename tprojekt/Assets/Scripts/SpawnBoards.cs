using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnBoards : MonoBehaviour
{
    //mindkét játékosnak board létrehozása
    public GameObject boardPrefab;
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        Vector3Int Spawn1 = new Vector3Int(-7, 0, 0);
        Vector3Int Spawn2 = new Vector3Int(8, 0, 0);
        
        if (PhotonNetwork.LocalPlayer.ActorNumber==1)
        {
            PhotonNetwork.Instantiate(boardPrefab.name, Spawn1, Quaternion.identity);
            
        }
        else
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == 2)

            PhotonNetwork.Instantiate(this.boardPrefab.name, Spawn2, Quaternion.identity);
        }

    }
}
