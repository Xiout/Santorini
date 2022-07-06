using UnityEngine;
using UnityEngine.Events;
using static PowerManager;

public partial class GameManager
{
    #region Events classes declaration
    //In Game Events
    //Event placing phase complete
    public class PlacingPhaseCompletedEvent : UnityEvent { }
    //Event moving phase start
    public class MovingPhaseStartEvent : UnityEvent { }
    //Event moving phase complete
    public class MovingPhaseCompletedEvent : UnityEvent { }
    //Event building phase start
    public class BuildingPhaseStartEvent : UnityEvent { }
    //Event building phase complete
    public class BuildingPhaseCompletedEvent : UnityEvent { }
    //Event Turn Completed
    public class TurnCompletedEvent : UnityEvent<int> { }
    //Event Victory
    public class VictoryEvent : UnityEvent<int> { }
    //Event Board Reset
    public class BoardResetEvent : UnityEvent { }
    //Event Power Action Realised
    public class PowerExecutedEvent : UnityEvent { }

    //Menu Events
    //Event Modification Nb Players
    public class ModificationNbPlayersEvent : UnityEvent<int> { }
    //Event Power On
    public class PowerOnOffEvent : UnityEvent { }
    #endregion

    public PlacingPhaseCompletedEvent mPlacingEvent { get; private set; }
    public MovingPhaseStartEvent mMovingEventStart { get; private set; }
    public BuildingPhaseStartEvent mBuildingEventStart { get; private set; }
    public MovingPhaseCompletedEvent mMovingEventComplet { get; private set; }
    public BuildingPhaseCompletedEvent mBuildingEventComplet { get; private set; }
    public TurnCompletedEvent mTurnCompleted { get; private set; }
    public VictoryEvent mVictoryEvent { get; private set; }
    public BoardResetEvent mBoardResetEvent { get; private set; }
    public ModificationNbPlayersEvent mModNbPlayersEvent { get; private set; }
    public PowerOnOffEvent mPowerOnOffEvent { get; private set; }
    public PowerExecutedEvent mPowerExecutedEvent { get; private set; }

    private static void SetUpEvents(GameManager pInstance)
    {
        pInstance.mPlacingEvent = new PlacingPhaseCompletedEvent();
        pInstance.mPlacingEvent.AddListener(sInstance.PlacingPhaseCompleted);

        pInstance.mMovingEventStart = new MovingPhaseStartEvent();
        pInstance.mMovingEventStart.AddListener(sInstance.MovingPhaseStart);
        
        pInstance.mMovingEventComplet = new MovingPhaseCompletedEvent();
        pInstance.mMovingEventComplet.AddListener(sInstance.MovingPhaseCompleted);

        pInstance.mBuildingEventStart = new BuildingPhaseStartEvent();
        pInstance.mBuildingEventStart.AddListener(sInstance.BuildingPhaseStart);

        pInstance.mBuildingEventComplet = new BuildingPhaseCompletedEvent();
        pInstance.mBuildingEventComplet.AddListener(sInstance.BuildingPhaseCompleted);
        
        pInstance.mTurnCompleted = new TurnCompletedEvent();
        pInstance.mTurnCompleted.AddListener(sInstance.TurnCompleted);
        
        pInstance.mVictoryEvent = new VictoryEvent();
        pInstance.mVictoryEvent.AddListener(sInstance.Victory);
        
        pInstance.mBoardResetEvent = new BoardResetEvent();
        pInstance.mBoardResetEvent.AddListener(sInstance.StartGame);
        
        pInstance.mModNbPlayersEvent = new ModificationNbPlayersEvent();
        pInstance.mModNbPlayersEvent.AddListener(sInstance.AddPlayers);

        pInstance.mPowerOnOffEvent = new PowerOnOffEvent();
        pInstance.mPowerOnOffEvent.AddListener(sInstance.PowerOnOff);

        pInstance.mPowerExecutedEvent = new PowerExecutedEvent();
        pInstance.mPowerExecutedEvent.AddListener(sInstance.PowerExecuted);
    }

    private void PlacingPhaseCompleted()
    {
        mInGameGO.GetComponent<InGameUI>().mPower.interactable = false;
        mInGameGO.GetComponent<InGameUI>().SetPower(false);
        sInstance.mIsPowerOn = false;
    }

   
    private void MovingPhaseStart()
    {
        mInGamePhase = InGamePhase.MOVE;
    }

    private void MovingPhaseCompleted()
    {
        mBoard.ResetHighlightCellsBoard();

        mInGameGO.GetComponent<InGameUI>().mPower.interactable = false;
        mInGameGO.GetComponent<InGameUI>().SetPower(false);
        sInstance.mIsPowerOn = false;

        if (mPlayers[mCurrentPlayer].mGod == God.Artemis)
        {
            mInGameGO.GetComponent<InGameUI>().mPower.interactable = true;
        }
    }

    private void BuildingPhaseStart()
    {
        mInGamePhase = InGamePhase.BUILD;
        mBoard.HighlightCellsAvailableBuilding();
    }

    private void BuildingPhaseCompleted()
    {
        mBoard.ResetHighlightCellsBoard();

        mInGameGO.GetComponent<InGameUI>().mPower.interactable = false;
        mInGameGO.GetComponent<InGameUI>().SetPower(false);
        sInstance.mIsPowerOn = false;
    }

    private void TurnCompleted(int pNextPlayer)
    {
        mPlayers[mCurrentPlayer].EndTurnPlayer();
        mCurrentPlayer = pNextPlayer;
        mInGameGO.GetComponent<InGameUI>().UpdateActivePlayer(mCurrentPlayer, mPlayers[mCurrentPlayer].mMaterial);

        mInGameGO.GetComponent<InGameUI>().mPower.interactable = false;
        sInstance.mIsPowerOn = false;
    }

    private void StartGame()
    {
        mCurrentState = GameState.PLAY;
        mInGamePhase = 0;
    }

    private void PowerOnOff()
    {
        mBoard.ResetHighlightCellsBoard();

        if (mIsPowerOn)
        {
            mInGamePhase = InGamePhase.POWER;
            if (mPlayers[mCurrentPlayer].mGod == God.Artemis)
            {
                mBoard.HightlightCellsAvailableMoving();
            }
        }
        else 
        { 
            if(mPlayers[mCurrentPlayer].mGod == God.Artemis)
            {
                mBuildingEventStart.Invoke();
            }
        }
    }

    private void PowerExecuted()
    {
        mBoard.ResetHighlightCellsBoard();

        if (mPlayers[mCurrentPlayer].mGod == God.Artemis)
        {
            mBuildingEventStart.Invoke();
        }
    }
}