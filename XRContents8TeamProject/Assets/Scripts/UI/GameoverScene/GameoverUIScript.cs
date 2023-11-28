using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class GameoverUIScript : MonoBehaviour
{
    public GameObject arrowImage;
    private void Start()
    {
        Cursor.visible = true;
    }

    private void Update()
    {
        ChangeCursorPosition();
        Exit();
    }

    private void ChangeCursorPosition()
    {
        Vector3 mPos = Input.mousePosition;
        var mVPos = Camera.main.ScreenToViewportPoint(mPos);
        arrowImage.GetComponent<RectTransform>().anchoredPosition = mVPos.x > 0.5f ? 
            new Vector2(64.0f, -240.0f) : new Vector2(-185.0f, -240.0f);
    }

    private void Exit()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    public void OnRestartButtonClick()
    {
        CutSceneCounter.Inst.SettingRestartScene();
        SceneManager.LoadScene("Stage1");
    }

    public void OnMenuButtonClick()
    {
        CutSceneCounter.Inst.SettingGameOver();
        SceneManager.LoadScene("MenuAndCutScene");
    }
}
