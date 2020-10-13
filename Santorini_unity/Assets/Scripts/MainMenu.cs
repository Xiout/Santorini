using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject mMenuOptions;
    void Start()
    {
        mMenuOptions.SetActive(false);
    }
        public void PlayGame()
    {
        GameManager.sGetInstance().Play();
    }
}
