using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public static readonly float[] sPresetY = { 1.0f, 2.95f, 5.25f, 6.50f };

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<Cell> getAllCellAvailableForMoving()
    {
        List<Cell> lAvailableCells = new List<Cell>(mCell.mAdjoiningCells);

        for (int i=0; i< lAvailableCells.Count; ++i)
        {
            Cell lCell = lAvailableCells[i];
            if(!lCell.mIsFree || lCell.getBuildingLevel()-mCell.getBuildingLevel() >= 2)
            {
                lAvailableCells.RemoveAt(i);
                i--;
            }
        }

        return lAvailableCells;
    }

    public List<Cell> getAllCellAvailableForBuilding()
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

    private bool isCellAvailableForMoving(Cell pCell)
    {
        return mCell.mAdjoiningCells.Contains(pCell) && pCell.mIsFree && pCell.getBuildingLevel()-mCell.getBuildingLevel()<2;
    }

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

    ///Reset the position of the builder based on the position of the cell
    public void UpdatePosition()
    {
        Vector3 lCellPos = mCell.gameObject.transform.position;
        float yPos = sPresetY[mCell.getBuildingLevel()];
        Vector3 lNewPos = new Vector3(lCellPos.x, yPos, lCellPos.z);
        gameObject.transform.SetPositionAndRotation(lNewPos, gameObject.transform.rotation);
    }
}
