using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : BoardGameComponent
{
    ///List of all adjoning cells
    public List<Cell> mAdjoiningCells;

    ///Number of floor built on this cell
    private int mBuildingLevel;

    ///false : a builder is on this cell
    ///true : no builder is on this cell
    public bool mIsFree;

    private void Awake()
    {
        mIsFree = true;
        mBuildingLevel = 0;
    }

    /// getter of the number of floor the cell currently have
    public int getBuildingLevel()
    {
        return mBuildingLevel;
    }

    ///If it's possible to build on this cell, do it and return true
    ///If not, return false
    public bool TryBuild()
    {
        if(mBuildingLevel<4 && mIsFree)
        {
            ++mBuildingLevel;
            return true;
        }
        return false;
    }
}
