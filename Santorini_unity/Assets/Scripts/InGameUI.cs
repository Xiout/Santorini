using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    public GameObject mActivePayerLine;

    public void UpdateActivePlayer(int pNumPlayer, Material pPlayerMaterial)
    {
        GameManager lGM = GameManager.sGetInstance();

        Transform transSwatcheCurrPlayer = mActivePayerLine.transform.Find("Swatche");
        Image imgCurrPlayer = transSwatcheCurrPlayer.GetComponent<Image>();

        imgCurrPlayer.color = pPlayerMaterial.color;

        Transform transTextCurrPlayer = mActivePayerLine.transform.Find("TxtActivePlayer");
        transTextCurrPlayer.GetComponent<Text>().text = $"Player {pNumPlayer + 1}";
    }
}
