﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager
{
    //In Game Events
    //Event placing phase complete
    public class PlacingPhaseCompletedEvent : UnityEvent{}
    //Event moving phase complete
    public class MovingPhaseCompletedEvent : UnityEvent{}
    //Event building phase complete
    public class BuildingPhaseCompletedEvent : UnityEvent{}
    //Event Victory
    public class VictoryEvent : UnityEvent<int>{}

    //Menu Events
    //Event Modification Nb Players
    public class ModificationNbPlayersEvent : UnityEvent<int> { }

    private static GameManager sInstance = null;

    public enum GameState{MENU, PLAY, VICTORY}
    private GameState mCurrentState;

    ///Placing phase : 0
    ///Building phase : 1
    ///Moving phase : 2
    private int mInGamePhase;

    private int mNbPlayers;
    public const int sMIN_NBPLAYERS = 2;
    public const int sMAX_NBPLAYERS = 3;
    ///players' builder material
    ///the size of this list MUST be equal to mNbPlayers
    public List<Material> mPlayersMaterial;

    //Events
    public PlacingPhaseCompletedEvent mPlacingEvent;
    public MovingPhaseCompletedEvent mMovingEvent;
    public BuildingPhaseCompletedEvent mBuildingEvent;
    public VictoryEvent mVictoryEvent;
    public ModificationNbPlayersEvent mModNbPlayersEvent;

    private GameObject mMenuGO;


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

            sInstance.mModNbPlayersEvent = new ModificationNbPlayersEvent();
            sInstance.mModNbPlayersEvent.AddListener(sInstance.addPlayers);

            sInstance.mCurrentState = GameState.MENU;
            sInstance.mMenuGO = GameObject.Find("Canvas_Menu");
            sInstance.mMenuGO.SetActive(true);

            sInstance.mInGamePhase = 0;
            sInstance.mNbPlayers = 2;
            sInstance.mPlayersMaterial = new List<Material>();
        }

        return sInstance;
    }

    public int getNbPlayers()
    {
        return mNbPlayers;
    }

    private void addPlayers(int pNb)
    {
        mNbPlayers += pNb;
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

    public void Play()
    {
        mCurrentState = GameState.PLAY;
        mMenuGO.SetActive(false);
    }

    private void Victory(int pIdWinner)
    {
        Debug.Log("VICTOIRE DU JOUEUR " + pIdWinner);
        mCurrentState = GameState.VICTORY;
    }
}
