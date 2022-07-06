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

    /// <summary>
    /// Set the god power for this player (this power cannot be changed during the GameState PLAY
    /// </summary>
    /// <param name="pGod"></param>
    /// <returns></returns>
    public bool SetGod(God pGod)
    {
        GameManager lGM = GameManager.sGetInstance();
        if (lGM.GetGameState() != GameState.PLAY) 
        {
            mGod = pGod;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Register an additional Builder
    /// </summary>
    /// <param name="pBuilder"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Return the Builder correspondign to the given index
    /// </summary>
    /// <param name="pIndex"></param>
    /// <returns></returns>
    public Builder GetBuilder(int pIndex)
    {
        if(pIndex>=0 && pIndex<mBuilders.Count)
            return mBuilders[pIndex];

        return null;
    }

    /// <summary>
    /// Determine if at least of of its builder can move up
    /// </summary>
    /// <param name="pIndex"></param>
    /// <returns></returns>
    public bool CanOneBuilderMoveUp()
    {
        bool lCanMoveUp = false;
        for(int i=0; i<mBuilders.Count; ++i)
        {
            lCanMoveUp = lCanMoveUp || mBuilders[i].CanMoveUp();
        }
        return lCanMoveUp;
    }

    /// <summary>
    /// Return the number total of builders the player has
    /// </summary>
    /// <returns></returns>
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
