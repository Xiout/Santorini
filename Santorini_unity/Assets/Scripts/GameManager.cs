using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager
{
    //Event placing phase complete
    public class PlacingPhaseCompletedEvent : UnityEvent{}
    //Event moving phase complete
    public class MovingPhaseCompletedEvent : UnityEvent{}
    //Event building phase complete
    public class BuildingPhaseCompletedEvent : UnityEvent{}
    //Event Victory
    public class VictoryEvent : UnityEvent<int>{}


    private static GameManager sInstance = null;

    public enum GameState{MENU, PLAY, VICTORY}
    private GameState mCurrentState;

    ///Placing phase : 0
    ///Building phase : 1
    ///Moving phase : 2
    private int mInGamePhase = 0;

    //Events
    public PlacingPhaseCompletedEvent mPlacingEvent;
    public MovingPhaseCompletedEvent mMovingEvent;
    public BuildingPhaseCompletedEvent mBuildingEvent;
    public VictoryEvent mVictoryEvent;


    public static GameManager sGetInstance()
    {
        if (sInstance == null)
        {
            sInstance = new GameManager();

            sInstance.mPlacingEvent = new PlacingPhaseCompletedEvent();
            sInstance.mPlacingEvent.AddListener(sInstance.placingPhaseCompleted);

            sInstance.mMovingEvent = new MovingPhaseCompletedEvent();
            sInstance.mMovingEvent.AddListener(sInstance.movingPhaseCompleted);

            sInstance.mBuildingEvent = new BuildingPhaseCompletedEvent();
            sInstance.mBuildingEvent.AddListener(sInstance.buildingPhaseCompleted);

            sInstance.mVictoryEvent = new VictoryEvent();
            sInstance.mVictoryEvent.AddListener(sInstance.Victory);

            sInstance.mCurrentState = GameState.PLAY;
        }

        return sInstance;
    }

    public GameState getGameState()
    {
        return mCurrentState;
    }

    public int GetInGamePhase()
    {
        return mInGamePhase;
    }

    private void placingPhaseCompleted()
    {
        mInGamePhase = 1;
    }

    private void movingPhaseCompleted()
    {
        mInGamePhase = 2;
    }

    private void buildingPhaseCompleted()
    {
        mInGamePhase = 1;
    }

    private void Victory(int pIdWinner)
    {
        Debug.Log("VICTOIRE DU JOUEUR " + pIdWinner);
        mCurrentState = GameState.VICTORY;
    }
}
