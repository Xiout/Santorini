using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

///Managing script of the board
//TODO : Tranformer en singleton ?
public class Board : MonoBehaviour
{
    //board dimensions
    ///dimension X of the board
    public int mNbCellsPerRow;
    ///dimension Z of the board
    public int mNbCellsPerColumn;

    ///material assigned for selected object
    public Material mMaterialSelectedObj;

    ///cell material 
    public Material mCellMaterial;
    ///players' builder material
    ///the size of this list MUST be equal to mNbPlayers
    //public List<Material> mPlayersMaterial;

    public List<GameObject> mBuildingPrefabs;

    ///List of all cells of the board
    ///This list is automatically filled at the generation of the cells
    ///each Cell is attached to a primitive gameobject "plane"
    private List<Cell> mAllCells;
    ///List of builders
    private List<Builder> mAllBuilders;

    private BoardGameComponent mSelectedBGComp;

    void Start()
    {
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

                if (lCellScr.setDefaultMaterial(mCellMaterial))
                {
                    lCellScr.ApplyMaterial(mCellMaterial);
                }
            }
            lRowObj.transform.SetParent(gameObject.transform);
            lPreviousRowObj = lRowObj;
        }
    }

    void Update()
    {
        GameManager lGM = GameManager.sGetInstance();
        int lCurrentPlayer = lGM.getCurrentPlayer();

        if (lGM.getGameState() == GameManager.GameState.RESET)
        {
            ClearBoard();
            Debug.Log("Board Cleared Sucessfully");
            lGM.mBoardResetEvent.Invoke();
        }

        if(lGM.getGameState() == GameManager.GameState.PLAY)
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
                    if (lClickedObject == null) { break; }

                    Debug.Log("Click On "+ lClickedObject.name);
                    BoardGameComponent lClickedBGComp = lClickedObject.GetComponent<BoardGameComponent>();

                    //Click on tower floors are treated as click on their parent cell
                    if (lClickedBGComp == null) 
                    {
                        Transform parentTrans = lClickedObject.transform.parent;
                        if(parentTrans== null){ return; }

                        BoardGameComponent lParentBGComp = parentTrans.GetComponent<BoardGameComponent>();
                        if (lParentBGComp == null) { return; }

                        lClickedObject = parentTrans.gameObject;
                        lClickedBGComp = lParentBGComp;
                    }

                    //the click is "confirmed" if the previous selected object is same that the clicked object
                    //in other words, we have to clic twice on the same cell to place a builder on it
                    bool lIsClickConfirmed = false;

                    Builder lBuilder = null;

                    //the clicked object is different that the currently selected object : the selection change OR an action is performed
                    if (mSelectedBGComp == null || (mSelectedBGComp.gameObject != lClickedBGComp.gameObject))
                    {
                        if (lGM.GetInGamePhase() != 0)
                        {
                            lBuilder = mSelectedBGComp as Builder;
                            if (lBuilder != null && mAllBuilders.Contains(lBuilder))
                            {
                                //A builder was selected previously
                                Cell lClickedCell = lClickedBGComp as Cell;
                                if (lClickedCell != null && mAllCells.Contains(lClickedCell))
                                {
                                    //The just clicked object is a cell  
                                    if (lGM.GetInGamePhase() == InGamePhase.MOVE)
                                    {
                                        //MOVE
                                        Cell lPrevCell = lBuilder.mCell;
                                        if (lBuilder.TryMove(lClickedCell))
                                        {
                                            //changing to building phase
                                            lGM.mMovingEvent.Invoke();

                                            //reset the default material on all adjoning cells of the Builder's previous cell
                                            for (int i = 0; i < lPrevCell.mAdjoiningCells.Count; ++i)
                                            {
                                                lPrevCell.mAdjoiningCells[i].GetComponent<BoardGameComponent>().ResetMaterial();
                                            }

                                            //Painting cells for building
                                            List<Cell> lAvailableCells = lBuilder.getAllCellsAvailableForBuilding();
                                            for (int i = 0; i < lAvailableCells.Count; ++i)
                                            {
                                                lAvailableCells[i].gameObject.GetComponent<BoardGameComponent>().ApplyMaterial(mMaterialSelectedObj);
                                            }

                                            //the builder stay selected for the building phase
                                        }
                                        break;
                                    }

                                    if (lGM.GetInGamePhase() == InGamePhase.BUILD)
                                    {
                                        //BUILD
                                        Debug.Log("BUILDING PHASE");
                                        Builder lSelectedBuilder = mSelectedBGComp as Builder;
                                        if (lSelectedBuilder != null && lSelectedBuilder.getAllCellsAvailableForBuilding().Contains(lClickedCell))
                                        {
                                            if (lClickedCell.TryBuild())
                                            {
                                                //generation of the building
                                                GameObject building = GameObject.Instantiate(mBuildingPrefabs[lClickedCell.getBuildingLevel() - 1], lClickedCell.transform.position, new Quaternion());
                                                building.transform.SetParent(lClickedCell.transform);

                                                //reset the default material on all available for build cells
                                                List<Cell> lAvailableCells = lBuilder.getAllCellsAvailableForBuilding();
                                                for (int i = 0; i < lAvailableCells.Count; ++i)
                                                {
                                                    lAvailableCells[i].gameObject.GetComponent<BoardGameComponent>().ResetMaterial();
                                                }
                                                //deselect the current builder
                                                mSelectedBGComp.GetComponent<BoardGameComponent>().ResetMaterial();
                                                mSelectedBGComp = null;

                                                //change of game phase and turn
                                                lClickedCell.GetComponent<BoardGameComponent>().ResetMaterial();
                                                lGM.mBuildingEvent.Invoke();
                                                lGM.mTurnCompleted.Invoke((lCurrentPlayer + 1) % lGM.getNbPlayers());
                                            }
                                        } 
                                    }
                                }
                            }
                        }

                        //The selection can't change during the building phase.
                        if (lGM.GetInGamePhase() == InGamePhase.BUILD)
                        {
                            break;
                        }

                        //THE SELECTION CHANGE 
                        Builder lClickedBuilder = lClickedBGComp as Builder;
                        if (lGM.GetInGamePhase() != InGamePhase.MOVE || (lClickedBuilder != null && lClickedBuilder.mPlayer == lCurrentPlayer))
                        {
                            //This part is for reassign the original material the previously clicked object
                            if (mSelectedBGComp != null)
                            {
                                mSelectedBGComp.gameObject.GetComponent<BoardGameComponent>().ResetMaterial();
                            }

                            //We also reset material of the previous "Available Cells"
                            //(available cells are cells available for a builder to move or build on)
                            lBuilder = mSelectedBGComp as Builder;
                            if (lBuilder != null && mAllBuilders.Contains(lBuilder))
                            {
                                List<Cell> lAvailableCells = lBuilder.getAllCellAvailableForMoving();
                                for (int i = 0; i < lAvailableCells.Count; ++i)
                                {
                                    lAvailableCells[i].GetComponent<BoardGameComponent>().ResetMaterial();
                                }
                            }


                            Cell lClickedCell = lClickedBGComp as Cell;
                            if (lGM.GetInGamePhase() == 0 && (lClickedCell == null || !lClickedCell.mIsFree))
                            {
                                mSelectedBGComp = null;
                                return;
                            }

                            //the currently selected object is now the object we just clicked
                            mSelectedBGComp = lClickedBGComp;

                            //in order to distinguish the selected objet from the other, we change its material 
                            mSelectedBGComp.gameObject.GetComponent<BoardGameComponent>().ApplyMaterial(mMaterialSelectedObj);
                        }
                    }
                    else
                    {
                        lIsClickConfirmed = true;
                    }

                    //Builders placing phase
                    if (lGM.GetInGamePhase() == InGamePhase.PLACE)
                    {
                        Cell lSelectedCell = mSelectedBGComp as Cell;
                        if (lIsClickConfirmed && mAllCells.Contains(lSelectedCell))
                        {
                            Debug.Log("PLACING PHASE : player " + lCurrentPlayer + "'s turn");
                            //Instanciate and name the builder
                            GameObject lBuilderObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                            lBuilderObj.name = "Builder" + ((int)(mAllBuilders.Count / lGM.getNbPlayers())) + "_p" + lCurrentPlayer;
                            //create the Builder script
                            Builder lBuilderScr = lBuilderObj.AddComponent<Builder>();
                            //declare its player owner
                            lBuilderScr.mPlayer = lCurrentPlayer;
                            //declare its location
                            lBuilderScr.mCell = lSelectedCell;

                            if (lBuilderScr.setDefaultMaterial(lGM.mPlayers[lCurrentPlayer].mMaterial))
                            {
                                lBuilderScr.ApplyMaterial(lGM.mPlayers[lCurrentPlayer].mMaterial);
                            }

                            //declare the cell as occupied
                            Cell mCellScr = lBuilderScr.mCell;
                            mCellScr.mIsFree = false;

                            mAllBuilders.Add(lBuilderScr);
                            lGM.mPlayers[lCurrentPlayer].AddBuilder(lBuilderScr);

                            lGM.mTurnCompleted.Invoke((lCurrentPlayer + 1) % lGM.getNbPlayers());

                            Debug.Log($"Nb Player : {lGM.getNbPlayers()}\n" +
                                      $"Expected Nb Builders : {lGM.getNbPlayers() * 2}\n" +
                                      $"Current Nb Builders : {mAllBuilders.Count}\n");

                            if (mAllBuilders.Count >= lGM.getNbPlayers() * 2)
                            {
                                //mGamePhase = 1;
                                lGM.mPlacingEvent.Invoke();
                            }

                            //reinitialisation of the aspect of the cell
                            mSelectedBGComp.gameObject.GetComponent<BoardGameComponent>().ResetMaterial();
                            mSelectedBGComp = null;
                        }
                    }
                    else
                    {
                        if (lGM.GetInGamePhase() == InGamePhase.MOVE)
                        {
                            //Moving phase
                            Debug.Log("MOVING PHASE");
                            Builder lSelectedBuilder = mSelectedBGComp as Builder;
                            if (mAllBuilders.Contains(lSelectedBuilder))
                            {
                                List<Cell> lAvailableCells = lSelectedBuilder.getAllCellAvailableForMoving();
                                for (int i = 0; i < lAvailableCells.Count; ++i)
                                {
                                    lAvailableCells[i].gameObject.GetComponent<BoardGameComponent>().ApplyMaterial(mMaterialSelectedObj);
                                }
                            }
                        }
                    }
                    break;
                }
            }
        }//End of if GameState = PLAY
    }

    private void ClearBoard()
    {
        //Clear Builders
        while(mAllBuilders.Count>0)
        {
            Debug.Log("Destroy " + mAllBuilders[0].gameObject.name);
            Builder currentBuilder = mAllBuilders[0];
            mAllBuilders.RemoveAt(0);
            Destroy(currentBuilder.gameObject);
        }

        //Clear Cells
        for (int iCell = 0; iCell < mAllCells.Count; ++iCell)
        {
            Debug.Log("Clear " + mAllCells[iCell].gameObject.name);
            mAllCells[iCell].ClearCell();
        }
    }
}
