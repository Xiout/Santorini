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

    // Update is called once per frame
    void Update()
    {
        
    }

    public int getBuildingLevel()
    {
        return mBuildingLevel;
    }
}
