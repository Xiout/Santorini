using UnityEngine;
using UnityEngine.Events;
using static PowerManager;

public partial class GameManager
{
    #region Events classes declaration
    //In Game Events
    //Event placing phase complete
    public class PlacingPhaseCompletedEvent : UnityEvent { }
    //Event moving phase complete
    public class MovingPhaseCompletedEvent : UnityEvent { }
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

    public PlacingPhaseCompletedEvent mPlacingEvent;
    public MovingPhaseCompletedEvent mMovingEvent;
    public BuildingPhaseCompletedEvent mBuildingEvent;
    public TurnCompletedEvent mTurnCompleted;
    public VictoryEvent mVictoryEvent;
    public BoardResetEvent mBoardResetEvent;
    public ModificationNbPlayersEvent mModNbPlayersEvent;
    public PowerOnOffEvent mPowerOnOffEvent;
    public PowerExecutedEvent mPowerExecutedEvent;

    private static void SetUpEvents(GameManager pInstance)
    {
        pInstance.mPlacingEvent = new PlacingPhaseCompletedEvent();
        pInstance.mPlacingEvent.AddListener(sInstance.PlacingPhaseCompleted);
        
        pInstance.mMovingEvent = new MovingPhaseCompletedEvent();
        pInstance.mMovingEvent.AddListener(sInstance.MovingPhaseCompleted);
        
        pInstance.mBuildingEvent = new BuildingPhaseCompletedEvent();
        pInstance.mBuildingEvent.AddListener(sInstance.BuildingPhaseCompleted);
        
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

        mInGamePhase = InGamePhase.MOVE;
    }

    private void MovingPhaseCompleted()
    {
        mInGameGO.GetComponent<InGameUI>().mPower.interactable = false;
        mInGameGO.GetComponent<InGameUI>().SetPower(false);
        sInstance.mIsPowerOn = false;

        if (mPlayers[mCurrentPlayer].mGod == God.Artemis)
        {
            mInGameGO.GetComponent<InGameUI>().mPower.interactable = true;
        }

        mInGamePhase = InGamePhase.BUILD;
    }

    private void BuildingPhaseCompleted()
    {
        mInGameGO.GetComponent<InGameUI>().mPower.interactable = false;
        mInGameGO.GetComponent<InGameUI>().SetPower(false);
        sInstance.mIsPowerOn = false;

        mInGamePhase = InGamePhase.MOVE;
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
        Debug.Log("POWER EVENT : "+mIsPowerOn);

        if (mIsPowerOn)
        {
            mInGamePhase = InGamePhase.POWER;
        }
        else 
        { 
            if(mPlayers[mCurrentPlayer].mGod == God.Artemis)
            {
                mInGamePhase = InGamePhase.BUILD;
            }
        }
    }

    private void PowerExecuted()
    {
        if (mPlayers[mCurrentPlayer].mGod == God.Artemis)
        {
            mInGamePhase = InGamePhase.BUILD;
        }
    }
}