using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PowerManager;

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
            lGM.mPlayers.Add(new Player(i, mAllMaterialForPlayers[i]));
        }
    }
   
    void Update()
    {
        GameManager lGM = GameManager.sGetInstance();

        mTextNPlayers.text = lGM.getNbPlayers().ToString();

        int i = 0;
        while(lGM.mPlayers.Count < lGM.getNbPlayers())
        {
            lGM.mPlayers.Add(new Player(0, null));
            i++;
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

        for (int iPlayer=GameManager.sMIN_NBPLAYERS; iPlayer< GameManager.sMAX_NBPLAYERS; ++iPlayer)
        {
            if (iPlayer >= mLinePlayers.Count) { break; }
            if (iPlayer < lGM.getNbPlayers())
            {
                mLinePlayers[iPlayer].SetActive(true);
            }
            else
            {
                mLinePlayers[iPlayer].SetActive(false);
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

        int ind = mAllMaterialForPlayers.FindIndex(mat => mat == lGM.mPlayers[pNumPlayer].mMaterial);
        ind = (ind+1)%mAllMaterialForPlayers.Count;
        while (usedColors.Contains(mAllMaterialForPlayers[ind].color))
        {
            ind = (ind + 1) % mAllMaterialForPlayers.Count;
        }

        imgCurrPlayer.color = mAllMaterialForPlayers[ind].color;
        lGM.mPlayers[pNumPlayer].mMaterial = mAllMaterialForPlayers[ind];
    }
}
