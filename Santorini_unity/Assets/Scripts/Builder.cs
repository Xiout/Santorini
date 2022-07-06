using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static PowerManager;

public class Builder : BoardGameComponent
{
    ///refers to the cell on wich the builder is located
    public Cell mCurrentCell;

    ///refers to the cell the builder was located during the previous turn
    public Cell mPreviousTurnCell;

    public bool mHasMovedThisTurn;

    ///Id of the player owner of the builder
    public int mPlayerIndex;

    // Start is called before the first frame update
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

    ///Values for Y depending on the number of floor a cell have (from 0-1)
    public static readonly float[] sPresetY = { 1.0f, 2.95f, 5.25f, 6.50f };

    // Update is called once per frame
    void Update()
    {
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

    ///get the list of all cells the builder can currently move to
    public List<Cell> getAllCellsAvailableForMoving()
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

    ///get the list of all cells the builder can currently build on
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

    ///return true if the builder is currently able to move to the cell as parameter
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

    ///return true if the builder is currently able to move to the cell as parameter
    private bool IsCellAvailableForBuilding(Cell pCell)
    {
        return mCurrentCell.mAdjoiningCells.Contains(pCell) //Can build only on Adjoining cells
            && pCell.mIsFree //Cannot build on occupied cells
            && pCell.GetBuildingLevel() < 4; //Cannot build above 4th level
    }

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

    ///If the builder is able to move to pCell, do it and return true
    ///If not, return false
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

    ///Reset the position of the builder's GameObject based on the position of its cell and the number of floor it has
    public void UpdatePhysicalPosition()
    {
        Vector3 lCellPos = mCurrentCell.gameObject.transform.position;
        float yPos = sPresetY[mCurrentCell.GetBuildingLevel()];
        Vector3 lNewPos = new Vector3(lCellPos.x, yPos, lCellPos.z);
        gameObject.transform.SetPositionAndRotation(lNewPos, gameObject.transform.rotation);
    }
}
