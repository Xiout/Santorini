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

    //2 material to do a checker board (temporary)
    public Material mMaterialChecker1;
    public Material mMaterialChecker2;

    ///List of all "cells" of the board
    ///This list is automatically filled at the generation of the cells
    ///Each cell is a plane with a script Cell attached to it
    private List<GameObject> mAllCellObjs;

    // Start is called before the first frame update
    void Start()
    {
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

                lCellObj.transform.SetParent(lRowObj.transform);

                //Paint cells for the checker board
                MeshRenderer lMeshRenderer = lCellObj.GetComponent<MeshRenderer>();
                if((iCell+iRow)%2 == 0)
                {
                    lMeshRenderer.materials[0] = mMaterialChecker1;
                    Debug.Log(mMaterialChecker1.name + " // " + lMeshRenderer.materials[0].name);
                }
                else
                {
                    lMeshRenderer.materials[0] = mMaterialChecker2;
                    Debug.Log(mMaterialChecker2.name + " // " + lMeshRenderer.materials[0].name);
                }
                
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
                Debug.Log(lMouseRayHit.transform.gameObject.name);
            }
        }

        //TODO : verifier que l'objet clicked est un builder
        //si oui : afficher les cases selectionnables

    }
}
