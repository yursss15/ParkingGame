using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SelectLevel : MonoBehaviour
{
    public Text CountMoves;

    [Serializable]
    public struct Levels
    {
        public GameObject level;
        public int moves;
    }

    public Levels[] levels;

    void Start()
    {
        if(!PlayerPrefs.HasKey("Game Level")) PlayerPrefs.SetInt("Game Level", 1);

        if (PlayerPrefs.GetInt("Game Level") > levels.Length) PlayerPrefs.SetInt("Game Level", levels.Length - 1);
        Debug.Log(levels.Length);
        Debug.Log(PlayerPrefs.GetInt("Game Level"));
        Levels now = levels[PlayerPrefs.GetInt("Game Level") - 1];
        now.level.SetActive(true);
        CountMoves.text = now.moves.ToString();
    }
}