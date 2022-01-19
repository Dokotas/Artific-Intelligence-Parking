using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class FinishButton : MonoBehaviour
{
    [SerializeField] private Text WinText;

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(WinMessage(other.gameObject.GetComponent<PhotonView>().Owner.ToString()));
    }

    IEnumerator WinMessage(string name)
    {
        int coldown = 5;
        WinText.text = name + " win the game!";
        while (coldown > 0)
        {
            coldown--;
            yield return new WaitForSeconds(1f);
        }

        PhotonNetwork.LeaveRoom();
    }
}