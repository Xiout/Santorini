﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerManager
{
   public enum God{
        None,
        Apollo,       //Your Move: Your Builder may move into an opponent Builder's space by forcing their Builder to the space yours just vacated.
        Artemis,      //Your Move: Your Builder may move one additional time, but not back to its initial space.
        Athena,       //Opponent's Turn: If one of your Builders moved up on your last turn, opponent Builders cannot move up this turn.
        //Atlas,      //Your Build: Your Builder may build a dome at any level.
        //Demeter,    //Your Build: Your Builder may build one additional time, but not on the same space.
        //Hephaestus, //Your Build: Your Builder may build one additional block (not dome) on top of your first block.
        //Hermes,     //Your Turn: If your Builders do not move up or down, they may each move any number of times (even zero), and then either builds
        //Minotaur,   //Your Move: Your Builder may move into an opponent Builder's space, if their Builder can be forced one space straight backwards to an unoccupied space at any level.
        Pan,          //Win Condition: You also win if your Builder moves down two or more levels.
        //Prometheus  //Your Turn: If your Builder does not move up, it may build both before and after moving.
    }

    /// <summary>
    /// Define if the pBuilder is allowed to move into pCell according to Athena move restriction rule
    /// Note : Other moving conditions are not taking in account in this method
    /// </summary>
    /// <param name="pGM"></param>
    /// <param name="pBuilder"></param>
    /// <param name="pCell"></param>
    /// <returns></returns>
    public static bool AthenaMoveRestriction(Builder pBuilder, Cell pCell)
    {
        GameManager lGM = GameManager.sGetInstance();
        Player AthenaPlayer = lGM.mPlayers.Find(player => player.mGod == God.Athena);

        if (AthenaPlayer == null)
        {
            //Athena is not played this game
            return true;

        }

        if(AthenaPlayer.mIndex == pBuilder.mPlayerIndex)
        {
            //The current player has Athena's power, the restriction does not apply to them
            return true;
        }

        bool hasMovedUp = false;
        for(int i=0; i<AthenaPlayer.GetBuildersCount(); ++i)
        {
            Debug.Log($"Athena Builder {i} : {AthenaPlayer.GetBuilder(i)}");
            Debug.Log($"Current Cell {AthenaPlayer.GetBuilder(i).mCurrentCell}");
            Debug.Log($"Previous Cell {AthenaPlayer.GetBuilder(i).mPreviousTurnCell}");
            hasMovedUp = hasMovedUp || AthenaPlayer.GetBuilder(i).mCurrentCell.getBuildingLevel()> AthenaPlayer.GetBuilder(i).mPreviousTurnCell.getBuildingLevel();
        }

        if (!hasMovedUp)
        {
            //The player with Athena's power has not moved up this turn, no movement restriction
            return true;
        }

        //The buider is allowed to move only on cell at the same level as him or lower
        return pBuilder.mCurrentCell.getBuildingLevel() >= pCell.getBuildingLevel();
    }

    public static bool ArtemisSecondMoveRestriction(Builder pBuilder, Cell pCell)
    {
        if(pBuilder.mHasMovedThisTurn && pBuilder.mPreviousTurnCell == pCell)
        {
            return false;
        }

        return true;
    }
}
