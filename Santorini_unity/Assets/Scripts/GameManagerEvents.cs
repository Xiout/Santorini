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

    //Menu Events
    //Event Modification Nb Players
    public class ModificationNbPlayersEvent : UnityEvent<int> { }
    #endregion

    public PlacingPhaseCompletedEvent mPlacingEvent;
    public MovingPhaseCompletedEvent mMovingEvent;
    public BuildingPhaseCompletedEvent mBuildingEvent;
    public TurnCompletedEvent mTurnCompleted;
    public VictoryEvent mVictoryEvent;
    public BoardResetEvent mBoardResetEvent;
    public ModificationNbPlayersEvent mModNbPlayersEvent;

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
    }

    private void PlacingPhaseCompleted()
    {
        mInGamePhase = InGamePhase.MOVE;
    }

    private void MovingPhaseCompleted()
    {
        mInGamePhase = InGamePhase.BUILD;
    }

    private void BuildingPhaseCompleted()
    {
        mInGamePhase = InGamePhase.MOVE;
    }

    private void TurnCompleted(int pNextPlayer)
    {
        mPlayers[mCurrentPlayer].EndTurnPlayer();
        mCurrentPlayer = pNextPlayer;
        mInGameGO.GetComponent<InGameUI>().UpdateActivePlayer(mCurrentPlayer, mPlayers[mCurrentPlayer].mMaterial);
    }

    private void StartGame()
    {
        mCurrentState = GameState.PLAY;
        mInGamePhase = 0;
    }

}