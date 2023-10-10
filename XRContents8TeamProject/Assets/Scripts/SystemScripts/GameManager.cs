using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private PlayerManager playerManager;

    private void Start()
    {
        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
    }

    private void Update()
    {
        if (playerManager.GetPlayerHp() <= 0)
        {
            SceneManager.LoadScene("GameoverScene");
        }
    }
}
