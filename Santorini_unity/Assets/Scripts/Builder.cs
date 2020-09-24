using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Builder : BoardGameComponent
{
    ///refers to the cell on wich the builder is located
    public Cell mCell;
    
    ///Id of the player owner of the builder
    public int mPlayer;

    // Start is called before the first frame update
    void Start()
    {
        if (mCell != null)
        {
            UpdatePosition();

            //declare the cell as occupied
            mCell.mIsFree = false;
        }
    }

    ///Values for Y depending on the number of floor a cell have (from 0-1)
    public static readonly float[] sPresetY = { 1.0f, 2.95f, 5.25f, 6.50f };

    // Update is called once per frame
    void Update()
    {
        GameManager lGM = GameManager.sGetInstance();

        if (lGM.getGameState() == GameManager.GameState.PLAY && mCell.getBuildingLevel() == 3)
        {
            GameManager.sGetInstance().mVictoryEvent.Invoke(mPlayer);
        }
    }

    ///get the list of all cells the builder can currently move to
    public List<Cell> getAllCellAvailableForMoving()
    {
        List<Cell> lAvailableCells = new List<Cell>(mCell.mAdjoiningCells);

        for (int i=0; i< lAvailableCells.Count; ++i)
        {
            Cell lCell = lAvailableCells[i];
            if(!lCell.mIsFree || lCell.getBuildingLevel()-mCell.getBuildingLevel() >= 2 || lCell.getBuildingLevel() >= 4)
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
        List<Cell> lAvailableCells = new List<Cell>(mCell.mAdjoiningCells);

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
        return mCell.mAdjoiningCells.Contains(pCell) && (pCell.mIsFree && pCell.getBuildingLevel()-mCell.getBuildingLevel()<2) && pCell.getBuildingLevel() < 4;
    }

    ///If the builder is able to move to pCell, do it and return true
    ///If not, return false
    public bool TryMove(Cell pCell)
    {
        if (!isCellAvailableForMoving(pCell))
        {
            return false;
        }

        mCell.mIsFree = true;
        mCell = pCell;
        mCell.mIsFree = false;
        UpdatePosition();
        return true;
    }

    ///Reset the position of the builder based on the position of its cell and the number of floor it has
    public void UpdatePosition()
    {
        Vector3 lCellPos = mCell.gameObject.transform.position;
        float yPos = sPresetY[mCell.getBuildingLevel()];
        Vector3 lNewPos = new Vector3(lCellPos.x, yPos, lCellPos.z);
        gameObject.transform.SetPositionAndRotation(lNewPos, gameObject.transform.rotation);
    }
}
