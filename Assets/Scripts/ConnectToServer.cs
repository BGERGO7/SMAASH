using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    
    //Elkezd csatlakozni
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();   
    }

    //Miutan csatlakozott a szerverre elkezd csatlakozni a lobbyba 
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    //Betolti a lobbit miutan csatlakozott a szerverre
    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("sc_lobby");
    }
}
