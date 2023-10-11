using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameoverUIScript : MonoBehaviour
{
    private void Start()
    {
        Cursor.visible = true;
    }

    private void Update()
    {
        Exit();
    }

    private void Exit()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit(0);
#endif
        }
    }

    public void OnRestartButtonClick()
    {
        SceneManager.LoadScene("TestScene");
    }

    public void OnMenuButtonClick()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
