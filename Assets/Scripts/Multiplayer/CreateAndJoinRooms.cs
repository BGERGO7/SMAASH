using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public TMP_InputField createInput;
    public TMP_InputField joinInput;

    
    //Szoba letrehozasa
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createInput.text);
    }

    //Szobahoz csatlakozas
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    //Miutan csatlakozott a szobahoz, belep a palyara
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("sc_main");
    }
}
