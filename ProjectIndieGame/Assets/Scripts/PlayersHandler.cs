﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayersHandler : MonoBehaviour
{

    public List<GameObject> players = new List<GameObject>();

    [SerializeField] private GameObject _resolutionScreen;
    [SerializeField] private Text resolutionScreenText;
    [SerializeField] private GameObject _firstButton;

    public List<GameObject> GetPlayers()
    {
        return players;
    }

    public void EndGame(string pWinner)
    {
        resolutionScreenText.text = pWinner + resolutionScreenText.text;
        _resolutionScreen.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_firstButton);
        Time.timeScale = 0;
    }
}
