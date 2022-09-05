using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public InputField createInput;
    public InputField joinInput;
    public GameObject panel;
    public GameObject waitingText;


    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    //szoba l�trehoz�sa
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createInput.text);
    }
    //csatlakoz�s a szob�hoz
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
    }
    public override void OnJoinedRoom()
    {
        this.waitingText.SetActive(true);
        this.panel.SetActive(false);
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        PhotonNetwork.LoadLevel("MP");
    }
}
