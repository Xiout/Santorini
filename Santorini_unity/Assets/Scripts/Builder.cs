using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static PowerManager;

public class Builder : BoardGameComponent
{
    /// <summary>
    /// Refers to the cell on wich the builder is located
    /// </summary>
    public Cell mCurrentCell;

    /// <summary>
    /// Refers to the cell the builder was located during the previous turn
    /// </summary>
    public Cell mPreviousTurnCell;

    /// <summary>
    /// Indicate if this builder has moved this turn
    /// This is reset when the turn or the next player start
    /// </summary>
    public bool mHasMovedThisTurn;

    /// <summary>
    /// Id of the player owner of the builder
    /// </summary>
    public int mPlayerIndex;

    void Start()
    {
        if (mCurrentCell != null)
        {
            UpdatePhysicalPosition();
            mHasMovedThisTurn = false;

            //declare the cell as occupied
            mCurrentCell.mIsFree = false;
        }
    }

    /// <summary>
    /// Values for Y to set when the Builder move to a cell depending on the number of floor a cell have
    /// </summary>
    public static readonly float[] sPresetY = { 1.0f, 2.95f, 5.25f, 6.50f };

    void Update()
    {
        //This update handle Victory Conditions
        GameManager lGM = GameManager.sGetInstance();
        
        if (mPlayerIndex != lGM.GetCurrentPlayer())
            return;//This builder does not belong to the current Player

        int lCurrentLvl = mCurrentCell.GetBuildingLevel();
        int lPreviousLvl = mPreviousTurnCell.GetBuildingLevel();
        Player lPlayer = lGM.mPlayers.Find(p => p.mIndex == mPlayerIndex);

        if (lGM.GetGameState() == GameManager.GameState.PLAY)
        {
            //Normal Win condition
            if (lCurrentLvl > lPreviousLvl && lCurrentLvl == 3 && HeraWinRestriction(this))
            {
                GameManager.sGetInstance().mVictoryEvent.Invoke(mPlayerIndex);
            }

            //Pan Win Condition
            if (lPlayer.mGod == God.Pan && lCurrentLvl <= lPreviousLvl-2 && HeraWinRestriction(this)) 
            {
                GameManager.sGetInstance().mVictoryEvent.Invoke(mPlayerIndex);
            }
        }


        
    }

    /// <summary>
    /// Get the list of all cells the builder can currently move to
    /// </summary>
    /// <returns></returns>
    public List<Cell> GetAllCellsAvailableForMoving()
    {
        List<Cell> lAvailableCells = new List<Cell>(mCurrentCell.mAdjoiningCells);

        for (int i=0; i< lAvailableCells.Count; ++i)
        {
            Cell lCell = lAvailableCells[i];
            if(!(IsCellAvailableForMoving(lCell)&&PowerManager.PersephoneMoveRestriction(this, lCell)))
            {
                lAvailableCells.RemoveAt(i);
                i--;
            }
        }

        return lAvailableCells;
    }

    /// <summary>
    /// Get the list of all cells the builder can currently build on
    /// </summary>
    /// <returns></returns>
    public List<Cell> GetAllCellsAvailableForBuilding()
    {
        List<Cell> lAvailableCells = new List<Cell>(mCurrentCell.mAdjoiningCells);

        for (int i = 0; i < lAvailableCells.Count; ++i)
        {
            Cell lCell = lAvailableCells[i];
            if (!IsCellAvailableForBuilding(lCell))
            {
                lAvailableCells.RemoveAt(i);
                i--;
            }
        }

        return lAvailableCells;
    }

    /// <summary>
    /// Return true if the builder is currently able to move to the given cell
    /// </summary>
    /// <param name="pCell"></param>
    /// <returns></returns>
    private bool IsCellAvailableForMoving(Cell pCell)
    {
        GameManager lGM = GameManager.sGetInstance();
        Player lPlayer = lGM.mPlayers.Find(p => p.mIndex == mPlayerIndex);

        return mCurrentCell.mAdjoiningCells.Contains(pCell) && //Can move only on Adjoining Cells
            (pCell.mIsFree || lPlayer.mGod == God.Apollo) && //Can move only on cell not occupied by another builder (expect with specific god power)
            pCell.GetBuildingLevel() - mCurrentCell.GetBuildingLevel() < 2 &&  //Cannot Got Up from more than 1 level at the time
            pCell.GetBuildingLevel() < 4 //Cannot move on floor 4 or above
            && PowerManager.AthenaMoveRestriction(this, pCell) //Check Athena's power restriction (If a player with Athena has move up during his last turn, no other player can move up this turn)
            && PowerManager.ArtemisSecondMoveRestriction(this, pCell); //Check Artemis's power restriction (Player with Artemis power may move twice before building but second movement cannot be initial position)
            //&& PowerManager.PersephoneMoveRestriction(this, pCell); //Check Persephone's Power restriction (Opponent must go up if possible)
    }

    /// <summary>
    /// Return true if the builder is currently able to build on the given cell
    /// </summary>
    /// <param name="pCell"></param>
    /// <returns></returns>
    private bool IsCellAvailableForBuilding(Cell pCell)
    {
        return mCurrentCell.mAdjoiningCells.Contains(pCell) //Can build only on Adjoining cells
            && pCell.mIsFree //Cannot build on occupied cells
            && pCell.GetBuildingLevel() < 4; //Cannot build above 4th level
    }

    /// <summary>
    /// Return true is the builder has at least one option to move up from its current cell
    /// </summary>
    /// <returns></returns>
    public bool CanMoveUp()
    {
        for(int i=0; i<mCurrentCell.mAdjoiningCells.Count; ++i)
        {
            Cell lCell = mCurrentCell.mAdjoiningCells[i];
            if (IsCellAvailableForMoving(lCell) && lCell.GetBuildingLevel() > mCurrentCell.GetBuildingLevel())
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// If the builder is able to move to the given cell, perform the movement and return true
    /// If not, return false
    /// </summary>
    /// <param name="pCell"></param>
    /// <returns></returns>
    public bool TryMove(Cell pCell)
    {
        if (!(IsCellAvailableForMoving(pCell) && PowerManager.PersephoneMoveRestriction(this, pCell)))
        {
            return false;
        }

        GameManager lGM = GameManager.sGetInstance();
        Player lPlayer = lGM.mPlayers.Find(p => p.mIndex == mPlayerIndex);

        if (lPlayer.mGod == God.Apollo && !pCell.mIsFree)
        {
            Builder lBuilder = lGM.GetBoard().GetBuilderAtCell(pCell);
            lBuilder.mCurrentCell = this.mCurrentCell;
            lBuilder.UpdatePhysicalPosition();
        }
        else
        {
            mCurrentCell.mIsFree = true;
        }

        mPreviousTurnCell = mCurrentCell;
        mCurrentCell = pCell;
        mHasMovedThisTurn = true;
        mCurrentCell.mIsFree = false;

        UpdatePhysicalPosition();
        return true;
    }

    /// <summary>
    /// Reset the position of the builder's GameObject based on the position of its cell and the number of floor it has
    /// </summary>
    public void UpdatePhysicalPosition()
    {
        Vector3 lCellPos = mCurrentCell.gameObject.transform.position;
        float yPos = sPresetY[mCurrentCell.GetBuildingLevel()];
        Vector3 lNewPos = new Vector3(lCellPos.x, yPos, lCellPos.z);
        gameObject.transform.SetPositionAndRotation(lNewPos, gameObject.transform.rotation);
    }
}
