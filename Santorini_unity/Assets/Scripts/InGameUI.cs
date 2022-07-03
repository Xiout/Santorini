using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    public GameObject mActivePayerLine;
    public Button mPower;

    public Material mMaterialPowerOn;
    public Material mMaterialPowerOff;

    public void UpdateActivePlayer(int pNumPlayer, Material pPlayerMaterial)
    {
        GameManager lGM = GameManager.sGetInstance();

        Transform transSwatcheCurrPlayer = mActivePayerLine.transform.Find("Swatche");
        Image imgCurrPlayer = transSwatcheCurrPlayer.GetComponent<Image>();

        imgCurrPlayer.color = pPlayerMaterial.color;

        Transform transTextCurrPlayer = mActivePayerLine.transform.Find("TxtActivePlayer");
        transTextCurrPlayer.GetComponent<Text>().text = $"Player {pNumPlayer + 1}";
    }

    public void SwitchOnOffPower()
    {
        GameManager lGM = GameManager.sGetInstance();

        lGM.mIsPowerOn = !lGM.mIsPowerOn;

        Debug.Log("Switch On/Off Power : "+ lGM.mIsPowerOn);

        SetPower(lGM.mIsPowerOn);
        lGM.mPowerOnOffEvent.Invoke();
    }

    public void SetPower(bool pIsOn)
    {
        Image imgCurrPlayer = mPower.transform.GetComponent<Image>();
        if (pIsOn)
        {
            imgCurrPlayer.color = mMaterialPowerOn.color;
        }
        else
        {
            imgCurrPlayer.color = mMaterialPowerOff.color;
        }
    }
}
