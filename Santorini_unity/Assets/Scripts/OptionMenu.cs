using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static PowerManager;

public class OptionMenu : MonoBehaviour
{
    public List<GameObject> mLinePlayers;
    public Text mTextNPlayers;
    public Button mBttLessPlayers, mBttMorePlayers;
    public List<Material> mAllMaterialForPlayers;

    private List<God> mGodsPlayers;
    void Awake()
    {
        GameManager lGM = GameManager.sGetInstance();

        mGodsPlayers = new List<God>();

        for (int i=0; i<mLinePlayers.Count; ++i)
        {
            Transform transLine = mLinePlayers[i].transform.Find("Swatche");
            Image img = transLine.GetComponent<Image>();
            img.color = mAllMaterialForPlayers[i].color;
            lGM.mPlayers.Add(new Player(i, mAllMaterialForPlayers[i]));
            mGodsPlayers.Add(God.None);
        }
    }
   
    void Update()
    {
        GameManager lGM = GameManager.sGetInstance();

        mTextNPlayers.text = lGM.GetNbPlayers().ToString();

        int i = 0;
        while(lGM.mPlayers.Count < lGM.GetNbPlayers())
        {
            lGM.mPlayers.Add(new Player(0, null));
            i++;
        }

        if(lGM.GetNbPlayers() == GameManager.sMIN_NBPLAYERS)
        {
            mBttLessPlayers.gameObject.SetActive(false);
        }
        else
        {
            mBttLessPlayers.gameObject.SetActive(true);
        }

        if (lGM.GetNbPlayers() == GameManager.sMAX_NBPLAYERS)
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
            if (iPlayer < lGM.GetNbPlayers())
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

    public void ChangeGodPlayer(int pNumPlayer)
    {
        var gods = Enum.GetNames(typeof(God)).ToList();
        Debug.Log(gods.Count);
        GameManager lGM = GameManager.sGetInstance();

        int currentGodId = (int)mGodsPlayers[pNumPlayer];

        do
        {
            currentGodId = (currentGodId + 1) % gods.Count;
        } while ((mGodsPlayers.Contains((God)currentGodId) && (God)currentGodId != God.None));

        if ((God)currentGodId == God.None)
        {
            for(int i=0; i<lGM.GetNbPlayers(); ++i)
            {
                mGodsPlayers[i] = God.None;
                UpdateGodLabel(i, gods[currentGodId]);
            }
        }
        else
        {
            mGodsPlayers[pNumPlayer] = (God)currentGodId;
            UpdateGodLabel(pNumPlayer, gods[currentGodId]);

            for (int i = 0; i < lGM.GetNbPlayers(); ++i)
            {
                if(mGodsPlayers[i] == God.None)
                {
                    int godId = 0;
                    while ((mGodsPlayers.Contains((God)godId)&& (God)godId != God.None) || godId == 0)
                    {
                        godId = (godId + 1) % gods.Count;
                    }
                    mGodsPlayers[i] = (God)godId;
                    UpdateGodLabel(i, gods[godId]);
                }
            }
        }
    }

    public void UpdateGodLabel(int pNumPlayer, string pGodString)
    {
        Transform transLabelGod = mLinePlayers[pNumPlayer].transform.Find("Txt_God");
        transLabelGod.GetComponent<Text>().text = pGodString;
    }

    public void SetAllGodPower()
    {
        GameManager lGM = GameManager.sGetInstance();
        for(int i=0; i<lGM.GetNbPlayers(); ++i)
        {
            lGM.mPlayers[i].SetGod(mGodsPlayers[i]);
        }
    }
}


