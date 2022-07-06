using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static PowerManager;

public partial class GameManager
{
    private static GameManager sInstance = null;

    public enum GameState{MENU, PLAY, VICTORY, RESET}
    private GameState mCurrentState;

    public enum InGamePhase { PLACE, MOVE, BUILD, POWER}
    private InGamePhase mInGamePhase;

    private int mNbPlayers;
    public const int sMIN_NBPLAYERS = 2;
    public const int sMAX_NBPLAYERS = 3;

    /// <summary>
    /// List of all the player in game
    /// </summary>
    public List<Player> mPlayers;
    /// <summary>
    /// Id of the current player
    /// </summary>
    private int mCurrentPlayer;

    /// <summary>
    /// Reference to the board
    /// This reference is set once when the game start and cannot be modify later on.
    /// </summary>
    private Board mBoard;

    //GUI
    private GameObject mMenuGO;
    private GameObject mOptionMenuGO;
    private GameObject mVictoryGO;
    private GameObject mInGameGO;

    /// <summary>
    /// For Active power, define if the power is in use
    /// </summary>
    public bool mIsPowerOn;

    public static GameManager sGetInstance()
    {
        if (sInstance == null)
        {
            sInstance = new GameManager();
            SetUpEvents(sInstance);

            sInstance.mCurrentState = GameState.MENU;
            sInstance.mMenuGO = GameObject.Find("Canvas_Menu");
            sInstance.mMenuGO.SetActive(true);

            sInstance.mOptionMenuGO = sInstance.mMenuGO.GetComponent<MainMenu>().mMenuOptions;
            sInstance.mVictoryGO = sInstance.mMenuGO.GetComponent<MainMenu>().mVictory;
            sInstance.mInGameGO = sInstance.mMenuGO.GetComponent<MainMenu>().mInGame;

            sInstance.mInGamePhase = 0;
            sInstance.mNbPlayers = 2;
            sInstance.mPlayers = new List<Player>();
            sInstance.mIsPowerOn = false;
        }

        return sInstance;
    }

    public bool SetBoard(Board pBoard)
    {
        if (mBoard == null)
        {
            mBoard = pBoard;
            return true;
        }

        return false;
    }

    public Board GetBoard()
    {
        return mBoard;
    }

    public int GetNbPlayers()
    {
        return mNbPlayers;
    }

    public int GetCurrentPlayer()
    {
        return mCurrentPlayer;
    }

    public GameState GetGameState()
    {
        return mCurrentState;
    }

    public InGamePhase GetInGamePhase()
    {
        return mInGamePhase;
    }

    public void SetUpGame()
    {
        //By default, the game state is RESET so in case of Replay, the board is cleared and the game state is updated to PLAY after
        mCurrentState = GameState.RESET; 

        //Hide Menu, display In-Game UI
        mMenuGO.SetActive(false);
        mVictoryGO.SetActive(false);
        mInGameGO.SetActive(true);

        //Set Gods
        mOptionMenuGO.GetComponent<OptionMenu>().SetAllGodPower();

        //Set First Player
        mCurrentPlayer = 0;
        mInGameGO.GetComponent<InGameUI>().UpdateActivePlayer(mCurrentPlayer, mPlayers[mCurrentPlayer].mMaterial);
        mInGameGO.GetComponent<InGameUI>().mPower.interactable = false;
        sInstance.mIsPowerOn = false;
    }
}
