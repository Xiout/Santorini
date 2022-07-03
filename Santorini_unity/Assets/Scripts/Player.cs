using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class Player
{
    public int mIndex;
    public Material mMaterial;

    private List<Builder> mBuilders;

    public Player(int pIndex, Material pMat)
    {
        mIndex = pIndex;
        mMaterial = pMat;
        mBuilders = new List<Builder>();
    }


    public bool AddBuilder(Builder pBuilder)
    {
        GameManager lGM = GameManager.sGetInstance();
        if(lGM.GetInGamePhase() != InGamePhase.PLACE)
        {
            Debug.Log($"Cannot add Builder {pBuilder.name} to player {mIndex}");
            return false;
        }

        Debug.Log($"Builder {pBuilder.name} sucessfully added to player {mIndex}");
        mBuilders.Add(pBuilder);
        return true;
    }

    public Builder GetBuilder(int pIndex)
    {
        if(pIndex>=0 && pIndex<mBuilders.Count)
            return mBuilders[pIndex];

        return null;
    }

    public int GetBuildersCount()
    {
        return mBuilders.Count;
    }

}
