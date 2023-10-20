using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private PlayerManager playerManager;
    public List<Transform> stages;
    
    public Image bloodImage;
    private bool isCoroutineOn;

    private int remainMonster;
    public float speed;

    private static GameManager inst = null;

    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
        }
        else
        {
            if (inst != null)
            {
                Destroy(gameObject);
            }
        }
    }

    public static GameManager Inst
    {
        get
        {
            if (inst == null)
                return null;
            return inst;
        }
    }
    

    private void Start()
    {
        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        isCoroutineOn = false;
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

        Exit();
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

    public void HitPlayer()
    {
        Color newColor = Color.white;
        newColor.a = 1f;
        bloodImage.color = newColor;
        if (!isCoroutineOn)
            StartCoroutine(BloodTime(newColor));
    }

    private IEnumerator BloodTime(Color newColor)
    {
        Debug.Log($"Coroutine CALL");
        isCoroutineOn = true;
        while (newColor.a > 0)
        {
            //Debug.Log($"{newColor.a}");
            newColor.a -= speed * Time.deltaTime;
            bloodImage.color = newColor;
            yield return new WaitForEndOfFrame();
        }

        newColor.a = 0.0f;
        bloodImage.color = newColor;
        isCoroutineOn = false;
    }
}
