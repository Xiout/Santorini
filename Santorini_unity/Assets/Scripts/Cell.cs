using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : BoardGameComponent
{
    /// <summary>
    /// List of all cells that are next to this cell
    /// </summary>
    public List<Cell> mAdjoiningCells;

    /// <summary>
    /// Number of floor built on this cell
    /// </summary>
    private int mBuildingLevel;

    /// <summary>
    /// Define is there is already a builder on this cell
    /// </summary>
    public bool mIsFree;

    private void Awake()
    {
        mIsFree = true;
        mBuildingLevel = 0;
    }

    /// <summary>
    /// Getter of the number of floor the cell currently have
    /// </summary>
    /// <returns></returns>
    public int GetBuildingLevel()
    {
        return mBuildingLevel;
    }

    /// <summary>
    /// Return true is the cell is on the side of the board
    /// </summary>
    /// <returns></returns>
    public bool IsPerimeter()
    {
        return mAdjoiningCells.Count < 8;
    }

    //If it's possible to build on this cell, do it and return true
    //If not, return false
    /// <summary>
    /// Perform a build ff it's possible to build on this cell and return true 
    /// If not, return false
    /// </summary>
    /// <returns></returns>
    public bool TryBuild()
    {
        if(mBuildingLevel<4 && mIsFree)
        {
            ++mBuildingLevel;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Reset the cell and remove physical Building level that is on it
    /// </summary>
    public void ClearCell()
    {
        mIsFree = true;

        //Remove all buildingLevel
        //while (transform.childCount > 0)
        for(int i=0; i<transform.childCount; ++i)
        {
            Debug.Log("Detroy " + gameObject.name +" "+ transform.GetChild(0).name);
            Destroy(transform.GetChild(i).gameObject);
        }
        mBuildingLevel = 0;

        ResetMaterial();
    }
}
