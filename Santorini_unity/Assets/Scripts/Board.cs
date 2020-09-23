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

    private int mCurrentPlayer;
    /*private bool mIsPlacingDone;
    ///mIsBuildingPhase = true : Building phase
    ///mIsBuildingPhase = false : Moving phase
    private bool mIsBuildingPhase;*/
    ///Placing phase : 0
    ///Building phase : 1
    ///Moving phase : 2
    private int mGamePhase;

    ///material assigned for selected object
    public Material mMaterialSelectedObj;

    /*///material of the selected object just before it was selected
    ///(used to reset its own material after being deselected)
    private Material mPreviousMaterialSelectedObj;*/

    ///cell material 
    public Material mCellMaterial;
    ///players' builder material
    ///the size of this list MUST be equal to mNbPlayers
    public List<Material> mPlayersMaterial;

    ///List of all cells of the board
    ///This list is automatically filled at the generation of the cells
    ///each Cell is attached to a primitive gameobject "plane"
    private List<Cell> mAllCells;
    ///List of builders
    private List<Builder> mAllBuilders;

    private BoardGameComponent mSelectedBGComp;

    //private List<Cell> mAvailableCells;

    // Start is called before the first frame update
    void Start()
    {
        mCurrentPlayer = 1;
        //mIsPlacingDone = false;
        mGamePhase = 0;
        mAllBuilders = new List<Builder>();
        mAllCells = new List<Cell>();

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
                lCellScr.mAdjoiningCells = new List<Cell>();
                Transform lAdjoningTrf = null;
                Cell lAdjoningCellScr = null;
                if (iCell > 0)
                {
                    //(a-1, b)
                    lAdjoningTrf = lRowObj.transform.GetChild(iCell - 1);
                    lCellScr.mAdjoiningCells.Add(lAdjoningTrf.GetComponent<Cell>());
                    //update of the adjoning's adjoning list
                    lAdjoningCellScr = lAdjoningTrf.GetComponent<Cell>();
                    lAdjoningCellScr.mAdjoiningCells.Add(lCellScr);
                }

                if(iRow > 0)
                {
                    //(a, b-1)
                    lAdjoningTrf = lPreviousRowObj.transform.GetChild(iCell);
                    lCellScr.mAdjoiningCells.Add(lAdjoningTrf.GetComponent<Cell>());
                    //update of the adjoning's adjoning list
                    lAdjoningCellScr = lAdjoningTrf.GetComponent<Cell>();
                    lAdjoningCellScr.mAdjoiningCells.Add(lCellScr);

                    if (iCell > 0)
                    {
                        //(a-1, b-1)
                        lAdjoningTrf = lPreviousRowObj.transform.GetChild(iCell-1);
                        lCellScr.mAdjoiningCells.Add(lAdjoningTrf.GetComponent<Cell>());
                        //update of the adjoning's adjoning list
                        lAdjoningCellScr = lAdjoningTrf.GetComponent<Cell>();
                        lAdjoningCellScr.mAdjoiningCells.Add(lCellScr);
                    }

                    if (iCell < mNbCellsPerRow-1)
                    {
                        //(a-+1, b-1)
                        lAdjoningTrf = lPreviousRowObj.transform.GetChild(iCell + 1);
                        lCellScr.mAdjoiningCells.Add(lAdjoningTrf.GetComponent<Cell>());
                        //update of the adjoning's adjoning list
                        lAdjoningCellScr = lAdjoningTrf.GetComponent<Cell>();
                        lAdjoningCellScr.mAdjoiningCells.Add(lCellScr);
                    }
                }

                mAllCells.Add(lCellScr);
                lCellObj.transform.SetParent(lRowObj.transform);

                Renderer lRenderer = lCellObj.GetComponent<Renderer>();
                if (lCellScr.setDefaultMaterial(mCellMaterial))
                {
                    lRenderer.enabled = true;
                    lRenderer.sharedMaterial = mCellMaterial;
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
            while (Physics.Raycast(lMouseRay, out lMouseRayHit)) //BREAKABLE
            {
                //retrieve the clickedObject
                GameObject lClickedObject = lMouseRayHit.transform.gameObject;
                BoardGameComponent lClickedBGComp = lClickedObject.GetComponent<BoardGameComponent>();
                if(lClickedObject == null) { break;}
                //TODO : Empecher la selection d'un certain type de composants selon mGamePhase !

                //the click is "confirmed" if the previous selected object is same that the clicked object
                //in other words, we have to clic twice on the same cell to place a builder on it
                bool lIsClickConfirmed = false;
                Renderer lRenderer;
                Builder lBuilder = null; 

                //the clicked object is different that the currently selected object : the selection change OR an action is performed
                if (mSelectedBGComp == null || (mSelectedBGComp.gameObject != lClickedBGComp.gameObject))
                {
                    if (mGamePhase != 0)
                    {
                        lBuilder = mSelectedBGComp as Builder;
                        if (lBuilder != null && mAllBuilders.Contains(lBuilder))
                        {
                            //A builder was selected previously
                            Cell lCell = lClickedBGComp as Cell;
                            if(lCell != null && mAllCells.Contains(lCell))
                            {
                                //The just clicked object is a cell  
                                if (mGamePhase == 1)
                                {
                                    //MOVE
                                    Cell lPrevCell = lBuilder.mCell;
                                    if (lBuilder.TryMove(lCell))
                                    {
                                        //changing to building phase
                                        mGamePhase = 2;

                                        //reset the default material on all adjoning cells of the Builder's previous cell
                                        for (int i = 0; i < lPrevCell.mAdjoiningCells.Count; ++i)
                                        {
                                            lRenderer = lPrevCell.mAdjoiningCells[i].gameObject.GetComponent<Renderer>();
                                            lRenderer.enabled = true;
                                            lRenderer.sharedMaterial = lPrevCell.mAdjoiningCells[i].getDefaultMaterial();
                                        }

                                        //Painting cells for building
                                        List<Cell> lAvailableCells = lBuilder.getAllCellAvailableForBuilding();
                                        for (int i = 0; i < lAvailableCells.Count; ++i)
                                        {
                                            lRenderer = lAvailableCells[i].gameObject.GetComponent<Renderer>();
                                            lRenderer.enabled = true;
                                            lRenderer.sharedMaterial = mMaterialSelectedObj;
                                        }

                                        //the builder stay selected for the building phase
                                    }
                                    break;
                                }

                                if (mGamePhase == 2)
                                {
                                    //BUILD
                                    Debug.Log("BUILDING PHASE");
                                }
                            }
                        }
                    }

                    //The selection can't change during the building phase.
                    if (mGamePhase == 2)
                    {
                        break;
                    }

                    //THE SELECTION CHANGE 
                    //This part is for reassign the original material the previously clicked object
                    lRenderer = null;
                    if (mSelectedBGComp != null)
                    {
                        lRenderer = mSelectedBGComp.gameObject.GetComponent<Renderer>();
                        lRenderer.enabled = true;
                        //lRenderer.sharedMaterial = mPreviousMaterialSelectedObj;
                        lRenderer.sharedMaterial = mSelectedBGComp.GetComponent<BoardGameComponent>().getDefaultMaterial();
                    }

                    //We also reset material of the previous "Available Cells"
                    //(available cells are cells available for a builder to move or build on)
                    lBuilder = mSelectedBGComp as Builder;
                    if(lBuilder != null && mAllBuilders.Contains(lBuilder))
                    {
                        List<Cell> lAvailableCells = lBuilder.getAllCellAvailableForMoving();
                        for (int i = 0; i < lAvailableCells.Count; ++i)
                        {
                            lRenderer = lAvailableCells[i].gameObject.GetComponent<Renderer>();
                            lRenderer.enabled = true;
                            lRenderer.sharedMaterial = lAvailableCells[i].getDefaultMaterial();
                        }
                    }
                   

                    //the currently selected object is now the object we just clicked
                    mSelectedBGComp = lClickedBGComp;

                    //in order to distinguish the selected objet from the other, we change its material 
                    lRenderer = mSelectedBGComp.gameObject.GetComponent<Renderer>();
                    lRenderer.enabled = true;
                    //mPreviousMaterialSelectedObj = lRenderer.sharedMaterial;
                    lRenderer.sharedMaterial = mMaterialSelectedObj;
                }else{
                    lIsClickConfirmed = true;
                }

                //Builders placing phase
                if (mGamePhase == 0)
                {
                    Cell lSelectedCell = mSelectedBGComp as Cell;
                    if (lIsClickConfirmed && mAllCells.Contains(lSelectedCell))
                    {
                        Debug.Log("PLACING PHASE : player " + mCurrentPlayer+"'s turn");
                        //Instanciate and name the builder
                        GameObject lBuilderObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                        lBuilderObj.name = "Builder" + ((int)(mAllBuilders.Count / mNbPlayers)) + "_p"+ mCurrentPlayer;
                        //create the Builder script
                        Builder lBuilderScr = lBuilderObj.AddComponent<Builder>();
                        //declare its player owner
                        lBuilderScr.mPlayer = mCurrentPlayer;
                        //declare its location
                        lBuilderScr.mCell = lSelectedCell;

                        lRenderer = lBuilderObj.GetComponent<Renderer>();
                        if (lBuilderScr.setDefaultMaterial(mPlayersMaterial[mCurrentPlayer]))
                        {
                            lRenderer.enabled = true;
                            lRenderer.sharedMaterial = mPlayersMaterial[mCurrentPlayer];
                        }

                        //declare the cell as occupied
                        Cell mCellScr = lBuilderScr.mCell;
                        mCellScr.mIsFree = false;
                        
                        mAllBuilders.Add(lBuilderScr);

                        mCurrentPlayer = ((mCurrentPlayer + 1) % mNbPlayers);
                        if(mAllBuilders.Count >= mNbPlayers * 2)
                        {
                            mGamePhase = 1;
                        }
                    }
                }
                else
                {
                    if (mGamePhase == 1)
                    {
                        //Moving phase
                        Debug.Log("MOVING PHASE");
                        Builder lSelectedBuilder = mSelectedBGComp as Builder;
                        if (mAllBuilders.Contains(lSelectedBuilder))
                        {
                            List<Cell> lAvailableCells = lSelectedBuilder.getAllCellAvailableForMoving();
                            for (int i = 0; i < lAvailableCells.Count; ++i)
                            {
                                lRenderer = lAvailableCells[i].gameObject.GetComponent<Renderer>();
                                lRenderer.enabled = true;
                                lRenderer.sharedMaterial = mMaterialSelectedObj;
                            }
                        }
                    }
                }
                break;
            }
        }
    }
}
