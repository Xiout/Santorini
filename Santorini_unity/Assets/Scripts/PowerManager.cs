using System;
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
        //Prometheus, //Your Turn: If your Builder does not move up, it may build both before and after moving.
        //Zeus,       //Your Build: Your Builder may build a block under them.
        //Triton,     //Your Move: Each time your Builder moves into a perimeter space, it may immediately move again.
        //Poseidon,   //End of Your Turn: If your unmoved Builder is on the ground level, it may build up to three times.
        Persephone,   //Opponent’s Turn: If possible, at least one Builder must move up this turn.
        //Limus,      //Opponent’s Turn: Opponent Builders cannot build on spaces neighboring your Builders, unless building a dome to create a Complete Tower.
        //Hypnus,     //Start of Opponent’s Turn: If one of your opponent’s Builders is higher than all of their others, it cannot move.
        //Hestia,     //Your Build: Your Builder may build one additional time, but this cannot be on a perimeter space.
        Hera,         //Opponent’s Turn: An opponent cannot win by moving into a perimeter space.
        //Dionysus,   //Your Build: Each time a Builder you control creates a Complete Tower, you may take an additional turn using an opponent Builder instead of your own. No player can win during these additional turns.
        Chronus,      //Win Condition: You also win when there are at least five Complete Towers on the board.
        //Charon,     //Your Move: Before your Builder moves, you may force a neighboring opponent Builder to the space directly on the other side of your Builder, if that space is unoccupied.
        //Bia,        //Any Move: If an opponent Builder starts its turn neighboring one of your Builders, its last move must be to a space neighboring one of your Builders.
        //Ares,       //End of Your Turn: You may remove an unoccupied block (not dome) neighboring your unmoved Builder.
        //Aphrodite,  //Any Move: If an opponent Builder starts its turn neighboring one of your Builders, its last move must be to a space neighboring one of your Builders.
    }

    /// <summary>
    /// Define if the pBuilder is allowed to move into pCell according to Athena's move restriction rule
    /// Opponent cannot move up this turn if Athena moved up on their last turn
    /// Note : Other moving conditions are not taking in account in this method
    /// </summary>
    /// <param name="pGM"></param>
    /// <param name="pBuilder"></param>
    /// <param name="pCell"></param>
    /// <returns></returns>
    public static bool AthenaMoveRestriction(Builder pBuilder, Cell pCell)
    {
        GameManager lGM = GameManager.sGetInstance();
        Player athenaPlayer = lGM.mPlayers.Find(player => player.mGod == God.Athena);

        if (athenaPlayer == null)
        {
            //Athena is not played this game
            return true;
        }

        if(athenaPlayer.mIndex == pBuilder.mPlayerIndex)
        {
            //The current player has Athena's power, the restriction does not apply to them
            return true;
        }

        bool hasMovedUp = false;
        for(int i=0; i<athenaPlayer.GetBuildersCount(); ++i)
        {
            hasMovedUp = hasMovedUp || athenaPlayer.GetBuilder(i).mCurrentCell.GetBuildingLevel()> athenaPlayer.GetBuilder(i).mPreviousTurnCell.GetBuildingLevel();
        }

        if (!hasMovedUp)
        {
            //The player with Athena's power has not moved up this turn, no movement restriction
            return true;
        }

        //The buider is allowed to move only on cell at the same level as him or lower
        return pBuilder.mCurrentCell.GetBuildingLevel() >= pCell.GetBuildingLevel();
    }

    /// <summary>
    /// Define if the given builder is allowed to move into the given cell according to Artemis's move restriction rule.
    /// This restriction must concern only the second move of the player that has Artemis Power
    /// Artemis cannot move back to their initial cell
    /// </summary>
    /// <param name="pBuilder"></param>
    /// <param name="pCell"></param>
    /// <returns></returns>
    public static bool ArtemisSecondMoveRestriction(Builder pBuilder, Cell pCell)
    {
        if(pBuilder.mHasMovedThisTurn && pBuilder.mPreviousTurnCell == pCell)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Define if the builder can win on this cell position according to Hera's win restriction rule.
    /// Opponent cannot win on perimeter cells
    /// </summary>
    /// <param name="pBuilder"></param>
    /// <returns></returns>
    public static bool HeraWinRestriction(Builder pBuilder)
    {
        GameManager lGM = GameManager.sGetInstance();
        Player heraPlayer = lGM.mPlayers.Find(player => player.mGod == God.Hera);

        if (heraPlayer == null)
        {
            //Hera is not played this game
            return true;
        }

        if(pBuilder.mPlayerIndex == heraPlayer.mIndex)
        {
            //The current player has Hera's power, the restriction does not apply to them
            return true;
        }

        return !pBuilder.mCurrentCell.IsPerimeter();
    }

    /// <summary>
    /// Define if the given builder is allowed to move into the given cell according to Persephone's move restriction rule. 
    /// Persephone forces their opponent to move up everytime it is possible
    /// </summary>
    /// <param name="pBuilder"></param>
    /// <param name="pCell"></param>
    /// <returns></returns>
    public static bool PersephoneMoveRestriction(Builder pBuilder, Cell pCell)
    {
        GameManager lGM = GameManager.sGetInstance();
        Player persephonePlayer = lGM.mPlayers.Find(player => player.mGod == God.Persephone);

        if (persephonePlayer == null)
        {
            //Persephone is not played this game
            return true;
        }

        if (persephonePlayer.mIndex == pBuilder.mPlayerIndex)
        {
            //The current player has Persephone's power, the restriction does not apply to them
            return true;
        }

        Player currentPlayer = lGM.mPlayers.Find(p => p.mIndex == pBuilder.mPlayerIndex);
        if (currentPlayer.CanOneBuilderMoveUp() && !pBuilder.mHasMovedThisTurn)
        {
            return pCell.GetBuildingLevel() > pBuilder.mCurrentCell.GetBuildingLevel();
        }

        return true;
    }
}
