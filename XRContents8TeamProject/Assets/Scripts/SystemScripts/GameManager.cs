using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private PlayerManager playerManager;
    public List<Transform> stages;

    private int remainMonster;

    private void Start()
    {
        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
    }

    private void Update()
    {
        remainMonster = 0;
        foreach (var stage in stages)
        {
            remainMonster += stage.childCount;
        }
        
        if (playerManager.GetPlayerHp() <= 0 || remainMonster == 0)
        {
            SceneManager.LoadScene("GameoverScene");
        }
    }
}
