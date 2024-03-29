﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject mMenuOptions;
    public GameObject mVictory;
    public GameObject mInGame;

    void Start()
    {
        mMenuOptions.SetActive(false);
        mVictory.SetActive(false);
        mInGame.SetActive(false);
    }

    public void PlayGame()
    {
        Debug.Log("Play Game");
        GameManager.sGetInstance().SetUpGame();
    }
}
