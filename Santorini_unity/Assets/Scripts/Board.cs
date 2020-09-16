using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///Managing script of the board
//TODO : Tranformer en singleton ?
public class Board : MonoBehaviour
{
    //board dimensions
    ///dimension X of the board
    public int mNbCellsPerRow;
    ///dimension Z of the board
    public int mNbCellsPerColumn;
    ///number of players
    public int mNbPlayers;

    ///material assigned for selected object
    public Material mMaterialSelectedObj;
    private Material mPreviousMaterialSelectedObj;

    ///List of all "cells" of the board
    ///This list is automatically filled at the generation of the cells
    ///Each cell is a plane with a script Cell attached to it
    private List<GameObject> mAllCellObjs;
    ///List of builders
    private List<GameObject> mAllBuilders;

    private GameObject mSelectedObject;

    // Start is called before the first frame update
    void Start()
    {
        mAllBuilders = new List<GameObject>();
        mAllCellObjs = new List<GameObject>();

        //--Generation of all the cells of the board--
        //Cells are organized by row (this make it easier to find adjoning of a newly created cell)
        GameObject lRowObj = null;
        GameObject lPreviousRowObj = null;
        for (int iRow=0; iRow<mNbCellsPerColumn; ++iRow)
        {
            lRowObj = new GameObject("Row" + iRow);
            for (int iCell=0; iCell < mNbCellsPerRow; ++iCell)
            {
                //--Creation of a cell--
                //Generate and naming of the cell
                GameObject lCellObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
                lCellObj.name = "Cell" + iRow + "x" + iCell;
                //Position, rotation scale setting
                Vector3 lCellPos = new Vector3(iCell * 5, 0, iRow*5);
                lCellObj.transform.SetPositionAndRotation(lCellPos, new Quaternion());
                lCellObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                
                //--Creation of the Cell script--
                Cell lCellScr = lCellObj.AddComponent<Cell>();
                //Fill mAdjoiningCells list with already existing cells (+ maj of other cells' mAdjoiningCells list)
                //Since cells are squared plane, a cell (a,b) have at maximum 8 adjonings : (a-1,b-1), (a, b-1), (a+1, b-1), (a-1, b), (a+1, b), (a-1, b+1), (a, b+1), (a+1, b+1)
                //At the moment of the loop only (a-1,b-1), (a, b-1), (a+1, b-1) and (a-1, b) may exist
                //The solution to fill the 4 missing is : when an adjoning is add to the list of current cell, the current cell is also add to the list of its adjoning
                lCellScr.mAdjoiningCells = new List<GameObject>();
                Transform lAdjoningTrf = null;
                Cell lAdjoningCellScr = null;
                if (iCell > 0)
                {
                    //(a-1, b)
                    lAdjoningTrf = lRowObj.transform.GetChild(iCell - 1);
                    lCellScr.mAdjoiningCells.Add(lAdjoningTrf.gameObject);
                    //update of the adjoning's adjoning list
                    lAdjoningCellScr = lAdjoningTrf.GetComponent<Cell>();
                    lAdjoningCellScr.mAdjoiningCells.Add(lCellObj);
                }

                if(iRow > 0)
                {
                    //(a, b-1)
                    lAdjoningTrf = lPreviousRowObj.transform.GetChild(iCell);
                    lCellScr.mAdjoiningCells.Add(lAdjoningTrf.gameObject);
                    //update of the adjoning's adjoning list
                    lAdjoningCellScr = lAdjoningTrf.GetComponent<Cell>();
                    lAdjoningCellScr.mAdjoiningCells.Add(lCellObj);

                    if (iCell > 0)
                    {
                        //(a-1, b-1)
                        lAdjoningTrf = lPreviousRowObj.transform.GetChild(iCell-1);
                        lCellScr.mAdjoiningCells.Add(lAdjoningTrf.gameObject);
                        //update of the adjoning's adjoning list
                        lAdjoningCellScr = lAdjoningTrf.GetComponent<Cell>();
                        lAdjoningCellScr.mAdjoiningCells.Add(lCellObj);
                    }

                    if (iCell < mNbCellsPerRow-1)
                    {
                        //(a-+1, b-1)
                        lAdjoningTrf = lPreviousRowObj.transform.GetChild(iCell + 1);
                        lCellScr.mAdjoiningCells.Add(lAdjoningTrf.gameObject);
                        //update of the adjoning's adjoning list
                        lAdjoningCellScr = lAdjoningTrf.GetComponent<Cell>();
                        lAdjoningCellScr.mAdjoiningCells.Add(lCellObj);
                    }
                }

                mAllCellObjs.Add(lCellObj);
                lCellObj.transform.SetParent(lRowObj.transform);

            }
            lRowObj.transform.SetParent(gameObject.transform);
            lPreviousRowObj = lRowObj;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Script de base permettant d'afficher le nom de l'objet selectionner
        if (Input.GetMouseButtonDown(0))
        {
            Ray lMouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit lMouseRayHit;
            if (Physics.Raycast(lMouseRay, out lMouseRayHit))
            {
                //retrieve the clickedObject
                GameObject lClickedObject = lMouseRayHit.transform.gameObject;

                //the click is "confirmed" if the previous selected object is same that the clicked object
                //in other words, we have to clic twice on the same cell to place a builder on it
                bool lIsClickConfirmed = false;

                //the clicked object is different that the currently selected object : the selection change
                if (mSelectedObject != lClickedObject)
                {
                    //This part is for reassign the original material the previously clicked object
                    Renderer lRenderer = null;
                    if (mSelectedObject != null)
                    {
                        lRenderer = mSelectedObject.GetComponent<Renderer>();
                        lRenderer.enabled = true;
                        lRenderer.sharedMaterial = mPreviousMaterialSelectedObj;
                    }

                    //the currently selected object is now the object we just clicked
                    mSelectedObject = lClickedObject;

                    //in order to distinguish this objet from the other, we change its material 
                    lRenderer = mSelectedObject.GetComponent<Renderer>();
                    lRenderer.enabled = true;
                    mPreviousMaterialSelectedObj = lRenderer.sharedMaterial;
                    lRenderer.sharedMaterial = mMaterialSelectedObj;
                }else{
                    lIsClickConfirmed = true;
                }

                //Builders placing phase
                if (mAllBuilders.Count < mNbPlayers * 2)
                {
                    if(lIsClickConfirmed && mAllCellObjs.Contains(mSelectedObject))
                    {
                        GameObject lBuilderObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                        lBuilderObj.name = "Builder" + ((int)(mAllBuilders.Count / mNbPlayers)) + "_p"+ ((mAllBuilders.Count % mNbPlayers) + 1);
                        Builder lBuilderScr = lBuilderObj.AddComponent<Builder>();
                        lBuilderScr.mCellObj = mSelectedObject;
                        lBuilderScr.mPlayer = (mAllBuilders.Count % mNbPlayers) + 1;

                        mAllBuilders.Add(lBuilderObj);
                    }
                }

                //TODO : Moving Builder phase
                //TODO : Building phase
            }
        }

    }
}
