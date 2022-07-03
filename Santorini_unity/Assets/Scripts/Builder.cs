using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Builder : BoardGameComponent
{
    ///refers to the cell on wich the builder is located
    public Cell mCurrentCell;

    ///refers to the cell the builder was located during the previous turn
    public Cell mPreviousTurnCell;

    public bool mHasMovedThisTurn;

    ///Id of the player owner of the builder
    public int mPlayer;

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

        if (lGM.GetGameState() == GameManager.GameState.PLAY && mCurrentCell.getBuildingLevel() == 3)
        {
            GameManager.sGetInstance().mVictoryEvent.Invoke(mPlayer);
        }
    }

    ///get the list of all cells the builder can currently move to
    public List<Cell> getAllCellAvailableForMoving()
    {
        List<Cell> lAvailableCells = new List<Cell>(mCurrentCell.mAdjoiningCells);

        for (int i=0; i< lAvailableCells.Count; ++i)
        {
            Cell lCell = lAvailableCells[i];
            if(!isCellAvailableForMoving(lCell))
            {
                lAvailableCells.RemoveAt(i);
                i--;
            }
        }

        return lAvailableCells;
    }

    ///get the list of all cells the builder can currently build on
    public List<Cell> getAllCellsAvailableForBuilding()
    {
        List<Cell> lAvailableCells = new List<Cell>(mCurrentCell.mAdjoiningCells);

        for (int i = 0; i < lAvailableCells.Count; ++i)
        {
            Cell lCell = lAvailableCells[i];
            if (!lCell.mIsFree || lCell.getBuildingLevel() == 4 )
            {
                lAvailableCells.RemoveAt(i);
                i--;
            }
        }

        return lAvailableCells;
    }

    ///return true if the builder is currently able to move to the cell as parameter
    private bool isCellAvailableForMoving(Cell pCell)
    {
        GameManager lGM = GameManager.sGetInstance();

        return mCurrentCell.mAdjoiningCells.Contains(pCell) && //Can move only on AdjoiningCell
            pCell.mIsFree && //Can move only on cell not occupied by another builder
            pCell.getBuildingLevel()-mCurrentCell.getBuildingLevel()<2 &&  //Cannot Got Up from more than 1 level at the time
            pCell.getBuildingLevel() < 4 //Cannot move on floor 4 or above
            && PowerManager.AthenaMoveRestriction(this, pCell) //Check Athena's power restriction (If a player with Athena has move up during his last turn, no other player can move up this turn)
            && PowerManager.ArtemisSecondMoveRestriction(this, pCell); //Check Artemis's power restriction (Player with Artemis power may move twice before building but second movement cannot be initial position)
    }

    ///return true if the builder is currently able to move to the cell as parameter
    private bool isCellAvailableForBuilding(Cell pCell)
    {
        return mCurrentCell.mAdjoiningCells.Contains(pCell) && (pCell.mIsFree  && pCell.getBuildingLevel() < 4);
    }

    ///If the builder is able to move to pCell, do it and return true
    ///If not, return false
    public bool TryMove(Cell pCell)
    {
        if (!isCellAvailableForMoving(pCell))
        {
            return false;
        }

        mCurrentCell.mIsFree = true;
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
        float yPos = sPresetY[mCurrentCell.getBuildingLevel()];
        Vector3 lNewPos = new Vector3(lCellPos.x, yPos, lCellPos.z);
        gameObject.transform.SetPositionAndRotation(lNewPos, gameObject.transform.rotation);
    }
}
