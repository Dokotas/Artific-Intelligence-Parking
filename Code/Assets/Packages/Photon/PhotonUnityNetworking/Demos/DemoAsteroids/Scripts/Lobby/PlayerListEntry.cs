// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerListEntry.cs" company="Exit Games GmbH">
//   Part of: Asteroid Demo,
// </copyright>
// <summary>
//  Player List Entry
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Photon.Pun.Demo.Asteroids
{
    public class PlayerListEntry : MonoBehaviour
    {
        [Header("UI References")] public Text PlayerNameText;

        public Image PlayerColorImage;
        public Button PlayerReadyButton;
        public Button ChangePlayerModelButton;
        public Image PlayerReadyImage;
        public Text ReadyTimer;

        private int ownerId;
        private bool isPlayerReady;
        private PlayerModel _playerModel;

        #region UNITY

        enum PlayerModel
        {
            Bear,
            Dog,
            Duck
        }

        public void OnEnable()
        {
            PlayerNumbering.OnPlayerNumberingChanged += OnPlayerNumberingChanged;
        }

        public void Start()
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber != ownerId)
            {
                PlayerReadyButton.gameObject.SetActive(false);
            }
            else
            {
                Hashtable initialProps = new Hashtable()
                {
                    {AsteroidsGame.PLAYER_READY, isPlayerReady},
                    {AsteroidsGame.PLAYER_LIVES, AsteroidsGame.PLAYER_MAX_LIVES},
                    {AsteroidsGame.PLAYER_MODEL, PlayerModel.Bear.ToString()}
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
                PhotonNetwork.LocalPlayer.SetScore(0);

                PlayerReadyButton.onClick.AddListener(() =>
                {
                    isPlayerReady = !isPlayerReady;
                    SetPlayerReady(isPlayerReady);

                    Hashtable props = new Hashtable() {{AsteroidsGame.PLAYER_READY, isPlayerReady}};
                    PhotonNetwork.LocalPlayer.SetCustomProperties(props);

                    if (PhotonNetwork.IsMasterClient)
                    {
                        FindObjectOfType<LobbyMainPanel>().LocalPlayerPropertiesUpdated();
                    }
                });
                ChangePlayerModelButton.gameObject.SetActive(true);
                ChangePlayerModelButton.onClick.AddListener(() =>
                {
                    ChangePlayerModel();
                    Hashtable props = new Hashtable() {{AsteroidsGame.PLAYER_MODEL, _playerModel.ToString()}};
                    PhotonNetwork.LocalPlayer.SetCustomProperties(props);
                });
            }
        }

        public void OnDisable()
        {
            PlayerNumbering.OnPlayerNumberingChanged -= OnPlayerNumberingChanged;
        }

        #endregion

        public void Initialize(int playerId, string playerName)
        {
            ownerId = playerId;
            PlayerNameText.text = playerName;
        }

        private void OnPlayerNumberingChanged()
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (p.ActorNumber == ownerId)
                {
                    PlayerColorImage.color = AsteroidsGame.GetColor(p.GetPlayerNumber());
                }
            }
        }

        public void SetPlayerReady(bool playerReady)
        {
            PlayerReadyButton.GetComponentInChildren<Text>().text = playerReady ? "Ready!" : "Ready?";
            PlayerReadyImage.enabled = playerReady;
        }

        public void ChangePlayerModel()
        {
            _playerModel = (PlayerModel) (((int) _playerModel + 1) % 3);
            ChangePlayerModelButton.GetComponentInChildren<Text>().text = _playerModel.ToString();
        }

        public IEnumerator Timer()
        {
            int coldown = 10;
            while (coldown > 0)
            {
                ReadyTimer.text = coldown.ToString();
                coldown--;
                yield return new WaitForSeconds(1f);
            }
        }
    }
}