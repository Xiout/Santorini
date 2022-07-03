using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerManager
{
   public enum God{
        None,
        //Apollo,     //Your Move: Your Worker may move into an opponent Worker's space by forcing their Worker to the space yours just vacated.
        //Artemis,      //Your Move: Your Worker may move one additional time, but not back to its initial space.
        Athena,       //Opponent's Turn: If one of your Workers moved up on your last turn, opponent Workers cannot move up this turn.
        //Atlas,      //Your Build: Your Worker may build a dome at any level.
        //Demeter,    //Your Build: Your Worker may build one additional time, but not on the same space.
        //Hephaestus, //Your Build: Your Worker may build one additional block (not dome) on top of your first block.
        //Hermes,     //Your Turn: If your Workers do not move up or down, they may each move any number of times (even zero), and then either builds
        //Minotaur,   //Your Move: Your Worker may move into an opponent Worker's space, if their Worker can be forced one space straight backwards to an unoccupied space at any level.
        //Pan,        //Win Condition: You also win if your Worker moves down two or more levels.
        //Prometheus  //Your Turn: If your Worker does not move up, it may build both before and after moving.
    }

    /// <summary>
    /// Define if the pBuilder is allowed to move into pCell according to Athena move restriction rule
    /// Note : Other moving conditions are not taking in account in this method
    /// </summary>
    /// <param name="pGM"></param>
    /// <param name="pBuilder"></param>
    /// <param name="pCell"></param>
    /// <returns></returns>
    public static bool AthenaMoveRestriction(GameManager pGM, Builder pBuilder, Cell pCell)
    {
        Player AthenaPlayer = pGM.mPlayers.Find(player => player.mGod == God.Athena);

        
        if (AthenaPlayer == null)
        {
            //Athena is not played this game
            return true;

        }

        if(AthenaPlayer.mIndex == pBuilder.mPlayer)
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
}
