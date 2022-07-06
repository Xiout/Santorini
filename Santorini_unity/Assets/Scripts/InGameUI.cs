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

    /// <summary>
    /// Display which player is playing this turn
    /// </summary>
    /// <param name="pNumPlayer"></param>
    /// <param name="pPlayerMaterial"></param>
    public void UpdateActivePlayer(int pNumPlayer, Material pPlayerMaterial)
    {
        GameManager lGM = GameManager.sGetInstance();

        Transform transSwatcheCurrPlayer = mActivePayerLine.transform.Find("Swatche");
        Image imgCurrPlayer = transSwatcheCurrPlayer.GetComponent<Image>();

        imgCurrPlayer.color = pPlayerMaterial.color;

        Transform transTextCurrPlayer = mActivePayerLine.transform.Find("TxtActivePlayer");
        transTextCurrPlayer.GetComponent<Text>().text = $"Player {pNumPlayer + 1}";
    }

    /// <summary>
    /// Method linked to the button "Power".
    /// This button allow to perform a specific action depending on the God power the current player has.
    /// </summary>
    public void SwitchOnOffPower()
    {
        GameManager lGM = GameManager.sGetInstance();

        lGM.mIsPowerOn = !lGM.mIsPowerOn;

        Debug.Log("Switch On/Off Power : "+ lGM.mIsPowerOn);

        SetColorButtonPower(lGM.mIsPowerOn);
        lGM.mPowerOnOffEvent.Invoke();
    }

    /// <summary>
    /// Change the color of the button Power
    /// </summary>
    /// <param name="pIsOn"></param>
    public void SetColorButtonPower(bool pIsOn)
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
