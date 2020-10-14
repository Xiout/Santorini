using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionMenu : MonoBehaviour
{
    public List<GameObject> mLinePlayers;
    public Text mTextNPlayers;
    public Button mBttLessPlayers, mBttMorePlayers;
    public List<Material> mAllMaterialForPlayers;

    void Awake()
    {
        GameManager lGM = GameManager.sGetInstance();
        for (int i=0; i<mLinePlayers.Count; ++i)
        {
            Transform transLine = mLinePlayers[i].transform.Find("Swatche");
            Image img = transLine.GetComponent<Image>();
            img.color = mAllMaterialForPlayers[i].color;
            lGM.mPlayersMaterial.Add(mAllMaterialForPlayers[i]);
        }
    }
   
    void Update()
    {
        GameManager lGM = GameManager.sGetInstance();

        mTextNPlayers.text = lGM.getNbPlayers().ToString();

        while(lGM.mPlayersMaterial.Count < lGM.getNbPlayers())
        {
            lGM.mPlayersMaterial.Add(null);
        }

        if(lGM.getNbPlayers() == GameManager.sMIN_NBPLAYERS)
        {
            mBttLessPlayers.gameObject.SetActive(false);
        }
        else
        {
            mBttLessPlayers.gameObject.SetActive(true);
        }

        if (lGM.getNbPlayers() == GameManager.sMAX_NBPLAYERS)
        {
            mBttMorePlayers.gameObject.SetActive(false);
        }
        else
        {
            mBttMorePlayers.gameObject.SetActive(true);
        }

        for (int i=GameManager.sMIN_NBPLAYERS; i< GameManager.sMAX_NBPLAYERS; ++i)
        {
            if (i >= mLinePlayers.Count) { break; }
            if (i < lGM.getNbPlayers())
            {
                mLinePlayers[i].SetActive(true);
            }
            else
            {
                mLinePlayers[i].SetActive(false);
            }
        }
    }

    public void MorePlayers()
    {
        GameManager.sGetInstance().mModNbPlayersEvent.Invoke(+1);
    }

    public void LessPlayers()
    {
        GameManager.sGetInstance().mModNbPlayersEvent.Invoke(-1);
    }

    public void ChangeColorPlayer(int pNumPlayer)
    {
        if (pNumPlayer >= mLinePlayers.Count) { return;  }

        GameManager lGM = GameManager.sGetInstance();
        List<Color> usedColors = new List<Color>();
        for(int i=0; i<mLinePlayers.Count; ++i)
        {
            Transform transLine = mLinePlayers[i].transform.Find("Swatche");
            Image img = transLine.GetComponent<Image>();
            usedColors.Add(img.color);
        }

        Transform transLineCurrPlayer = mLinePlayers[pNumPlayer].transform.Find("Swatche");
        Image imgCurrPlayer = transLineCurrPlayer.GetComponent<Image>();

        int ind = mAllMaterialForPlayers.IndexOf(lGM.mPlayersMaterial[pNumPlayer]);
        ind = (ind+1)%mAllMaterialForPlayers.Count;
        while (usedColors.Contains(mAllMaterialForPlayers[ind].color))
        {
            ind = (ind + 1) % mAllMaterialForPlayers.Count;
        }

        imgCurrPlayer.color = mAllMaterialForPlayers[ind].color;
        lGM.mPlayersMaterial[pNumPlayer] = mAllMaterialForPlayers[ind];
    }
}
