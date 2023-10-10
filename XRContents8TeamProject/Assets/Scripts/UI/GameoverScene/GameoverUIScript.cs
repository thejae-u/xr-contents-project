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

    public void OnRestartButtonClick()
    {
        SceneManager.LoadScene("TestScene");
    }

    public void OnMenuButtonClick()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
