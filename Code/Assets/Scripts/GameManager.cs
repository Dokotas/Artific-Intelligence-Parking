using System;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine;


public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform[] spawnPositions;

    private void Start()
    {
        var p = PhotonNetwork.LocalPlayer;
        var index = p.GetPlayerNumber();
        object playerModel;
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(AsteroidsGame.PLAYER_MODEL, out playerModel);
        PhotonNetwork.Instantiate(playerModel.ToString(), spawnPositions[index].position,
                spawnPositions[index].rotation);
    }

    public void Leave()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");
    }
}