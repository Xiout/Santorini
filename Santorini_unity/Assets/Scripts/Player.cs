using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static PowerManager;

public class Player
{
    public int mIndex;
    public Material mMaterial;

    public God mGod;
    private List<Builder> mBuilders;

    public Player(int pIndex, Material pMat)
    {
        mIndex = pIndex;
        mMaterial = pMat;
        mBuilders = new List<Builder>();
        mGod = God.None;
    }

    public bool SetGod(God pGod)
    {
        if (mGod == God.None) 
        {
            mGod = pGod;
            return true;
        }

        return false;
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

    public bool CanOneBuilderMoveUp()
    {
        bool lCanMoveUp = false;
        for(int i=0; i<mBuilders.Count; ++i)
        {
            lCanMoveUp = lCanMoveUp || mBuilders[i].CanMoveUp();
        }
        return lCanMoveUp;
    }

    public int GetBuildersCount()
    {
        return mBuilders.Count;
    }

    /// <summary>
    /// Reset mPreviousTurnCell and mHasMovedThisTurn for all builders if necessary
    /// </summary>
    public void EndTurnPlayer()
    {
        for(int i=0; i<mBuilders.Count; ++i)
        {
            if (!mBuilders[i].mHasMovedThisTurn)
            {
                mBuilders[i].mPreviousTurnCell = mBuilders[i].mCurrentCell;
            }

            mBuilders[i].mHasMovedThisTurn = false;
        }
    }
}
