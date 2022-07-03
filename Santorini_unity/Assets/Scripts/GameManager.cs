using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static PowerManager;

public class GameManager
{
    //In Game Events
    //Event placing phase complete
    public class PlacingPhaseCompletedEvent : UnityEvent{}
    //Event moving phase complete
    public class MovingPhaseCompletedEvent : UnityEvent{}
    //Event building phase complete
    public class BuildingPhaseCompletedEvent : UnityEvent{}
    //Event Turn Completed
    public class TurnCompletedEvent : UnityEvent<int>{}
    //Event Victory
    public class VictoryEvent : UnityEvent<int>{}
    //Event Board Reset
    public class BoardResetEvent : UnityEvent{}

    //Menu Events
    //Event Modification Nb Players
    public class ModificationNbPlayersEvent : UnityEvent<int> { }

    private static GameManager sInstance = null;

    public enum GameState{MENU, PLAY, VICTORY, RESET}
    private GameState mCurrentState;

    public enum InGamePhase { PLACE, MOVE, BUILD}
    private InGamePhase mInGamePhase;

    private int mNbPlayers;
    public const int sMIN_NBPLAYERS = 2;
    public const int sMAX_NBPLAYERS = 3;

    public List<Player> mPlayers;

    private int mCurrentPlayer;

    //Events
    public PlacingPhaseCompletedEvent mPlacingEvent;
    public MovingPhaseCompletedEvent mMovingEvent;
    public BuildingPhaseCompletedEvent mBuildingEvent;
    public TurnCompletedEvent mTurnCompleted;
    public VictoryEvent mVictoryEvent;
    public BoardResetEvent mBoardResetEvent;
    public ModificationNbPlayersEvent mModNbPlayersEvent;

    private GameObject mMenuGO;
    private GameObject mOptionMenuGO;
    private GameObject mVictoryGO;
    private GameObject mInGameGO;


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

            sInstance.mTurnCompleted = new TurnCompletedEvent();
            sInstance.mTurnCompleted.AddListener(sInstance.turnCompleted);

            sInstance.mVictoryEvent = new VictoryEvent();
            sInstance.mVictoryEvent.AddListener(sInstance.Victory);

            sInstance.mBoardResetEvent = new BoardResetEvent();
            sInstance.mBoardResetEvent.AddListener(sInstance.startGame);

            sInstance.mModNbPlayersEvent = new ModificationNbPlayersEvent();
            sInstance.mModNbPlayersEvent.AddListener(sInstance.addPlayers);

            sInstance.mCurrentState = GameState.MENU;
            sInstance.mMenuGO = GameObject.Find("Canvas_Menu");
            sInstance.mMenuGO.SetActive(true);

            sInstance.mOptionMenuGO = sInstance.mMenuGO.GetComponent<MainMenu>().mMenuOptions;
            sInstance.mVictoryGO = sInstance.mMenuGO.GetComponent<MainMenu>().mVictory;
            sInstance.mInGameGO = sInstance.mMenuGO.GetComponent<MainMenu>().mInGame;

            sInstance.mInGamePhase = 0;
            sInstance.mNbPlayers = 2;
            sInstance.mPlayers = new List<Player>();
        }

        return sInstance;
    }

    public int getNbPlayers()
    {
        return mNbPlayers;
    }

    public int getCurrentPlayer()
    {
        return mCurrentPlayer;
    }

    private void addPlayers(int pNb)
    {
        mNbPlayers += pNb;
    }

    public GameState getGameState()
    {
        return mCurrentState;
    }

    public InGamePhase GetInGamePhase()
    {
        return mInGamePhase;
    }

    private void placingPhaseCompleted()
    {
        mInGamePhase = InGamePhase.MOVE;
    }

    private void movingPhaseCompleted()
    {
        mInGamePhase = InGamePhase.BUILD;
    }

    private void buildingPhaseCompleted()
    {
        mInGamePhase = InGamePhase.MOVE;
    }

    private void turnCompleted(int pNextPlayer)
    {
        mPlayers[mCurrentPlayer].EndTurnPlayer();
        mCurrentPlayer = pNextPlayer;
        mInGameGO.GetComponent<InGameUI>().UpdateActivePlayer(mCurrentPlayer, mPlayers[mCurrentPlayer].mMaterial);
    }

    private void startGame()
    {
        mCurrentState = GameState.PLAY;
        mInGamePhase = 0;
    }

    public void Play()
    {
        //By default, the game state is RESET so in case of Replay, the board is cleared and the game state is updated to PLAY after
        mCurrentState = GameState.RESET; 

        //Hide Menu, display In-Game UI
        mMenuGO.SetActive(false);
        mVictoryGO.SetActive(false);
        mInGameGO.SetActive(true);

        //Set First Player
        mCurrentPlayer = 0;
        mPlayers[0].SetGod(God.Athena);
        mInGameGO.GetComponent<InGameUI>().UpdateActivePlayer(mCurrentPlayer, mPlayers[mCurrentPlayer].mMaterial);
    }

    private void Victory(int pIdWinner)
    {
        mInGameGO.SetActive(false);

        string lStrVictory = $"PLAYER {pIdWinner+1} WON !";
        Debug.Log(lStrVictory);
		
		mCurrentState = GameState.VICTORY;

        mVictoryGO.SetActive(true);
		GameObject lTxtVictoryGO = mVictoryGO.transform.Find("Txt_Victory").gameObject;
		lTxtVictoryGO.GetComponent<Text>().text = lStrVictory;
    }
}
