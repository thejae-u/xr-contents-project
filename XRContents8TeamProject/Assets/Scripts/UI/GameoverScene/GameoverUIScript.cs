using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameoverUIScript : MonoBehaviour
{
    public void OnRestartButtonClick()
    {
        SceneManager.LoadScene("TestScene");
    }

    public void OnMenuButtonClick()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
