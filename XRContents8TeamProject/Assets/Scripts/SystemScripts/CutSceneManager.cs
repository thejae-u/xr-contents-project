using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class CutSceneManager : MonoBehaviour
{
    [Header("거리 조정")][Range(0.0f, 3.0f)]
    public float range;

    [Header("속도 조정")][Range(0.0f, 5.0f)] 
    public float speed;

    public GameObject startButton;
    public GameObject settingButton;
    public GameObject exitButton;
    public GameObject skipButton;
    
    public GameObject nextButton;
    public GameObject prevButton;

    private GameObject globalLight = null;
    private GameObject spotLight = null;

    private bool isInitialized;
    
    private int curState;
    private bool isEndFirstAnim;
    private bool isEndFirstAnim2;

    private SkeletonAnimation anim;

    private readonly string[] names =
    {
        "Start1",
        "Start2",
        "Book_Open_1",
        "Page2",
        "Page3",
        "Page4",
        "Page5",
        "Page6",
        "Book_Open_7",
        "Page8",
        "Page9",
        "Page10"
    };

    private static CutSceneManager inst;

    private bool isStart;

    public static CutSceneManager Inst
    {
        get
        {
            return inst;
        }
    }

    private void Awake()
    {
        if (inst == null)
            inst = this;
        else
            Destroy(gameObject);
        
        anim = gameObject.GetComponent<SkeletonAnimation>();

        DeleteMixAnimation();
        
        DontDestroyOnLoad(gameObject);
    }

    private void DeleteMixAnimation()
    {
        for (int i = 0; i < names.Length - 1; i++)
        {
            for (int j = 0; j < names.Length - 1; j++)
            {
                if (j == i) continue;
                anim.AnimationState.Data.SetMix(names[i], names[j], 0);
            }
        }
    }

    private void Start()
    {
        curState = -1;
        isInitialized = false;
        isEndFirstAnim = false;
        isEndFirstAnim2 = false;
        isStart = false;

        ShowMenu();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "MenuAndCutScene") return;

        if (!isEndFirstAnim)
        {
            CheckShowMenu();
            return;
        }

        ShowPageButton();
        
        if(!isEndFirstAnim2)
            AnimationUpdate();
        
        if(isStart)
            GameStart();
    }

    private void InitObject()
    {
        if (SceneManager.GetActiveScene().name != "MenuAndCutScene") return;
        
        globalLight = GameObject.Find("Global");
        spotLight = GameObject.Find("Spot");
        isInitialized = true;
    }

    public void OnNextButtonClick()
    {
        if (anim.AnimationState.GetCurrent(0).IsComplete)
        {
            switch (anim.AnimationName)
            {
                case "Page6":
                    isStart = true;
                    return;
                case "Page10":
                    return;
                default:
                    AnimationCall();
                    break;
            }
        }
    }

    public void OnPrevButtonClick()
    {
        if (anim.AnimationState.GetCurrent(0).IsComplete)
        {
            curState -= 1;
            anim.AnimationState.SetAnimation(0, names[curState], false);
        }
    }

    private void AnimationCall()
    {
        curState += 1;
        anim.AnimationState.SetAnimation(0, names[curState], false);
    }

    private void ShowPageButton()
    {
        if (isStart)
        {
            skipButton.SetActive(false);
            prevButton.SetActive(false);
            nextButton.SetActive(false);
            return;
        }
        
        switch (anim.AnimationName)
        {
            case "Start1":
            case "Start2":
                skipButton.SetActive(false);
                prevButton.SetActive(false);
                nextButton.SetActive(false);
                return;
            case "Book_Open_1" when anim.AnimationState.GetCurrent(0).IsComplete:
                skipButton.SetActive(true);
                prevButton.SetActive(false);
                nextButton.SetActive(true);
                return;
            case "Book_Open_1":
                prevButton.SetActive(false);
                return;
            case "Page10" when anim.AnimationState.GetCurrent(0).IsComplete:
                skipButton.SetActive(false);
                prevButton.SetActive(true);
                nextButton.SetActive(false);
                return;
            case "Page10":
                nextButton.SetActive(false);
                return;
            default:
                prevButton.SetActive(true);
                nextButton.SetActive(true);
                return;
        }
    }

    private void AnimationUpdate()
    {
        if (anim.AnimationName == "Start2")
        {
            if (anim.AnimationState.GetCurrent(0).IsComplete)
            {
                AnimationCall();
                isEndFirstAnim2 = true;
            }
        }
    }

    private void ShowMenu()
    {
        InitObject();
        AnimationCall();
    }

    private void CheckShowMenu()
    {
        if (anim.AnimationState.GetCurrent(0).IsComplete)
        {
            ShowMenuButtons();
        }
    }

    public void OnStartButtonClick()
    {
        OffMenuButtons();
        AnimationCall();
        isEndFirstAnim = true;
    }

    public void OnExitButtonClick()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public void OnSkipButtonClick()
    {
        curState = 6;
        AnimationCall();
        skipButton.SetActive(false);
    }

    private void ShowMenuButtons()
    {
        startButton.SetActive(true);
        settingButton.SetActive(true);
        exitButton.SetActive(true);
    }

    private void OffMenuButtons()
    {
        startButton.SetActive(false);
        settingButton.SetActive(false);
        exitButton.SetActive(false);
    }

    private void SecondStartAnimation()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = true;
        isStart = false;
    }

    private void GameStart()
    {
        if (Camera.main.orthographicSize > range)
        {
            Camera.main.orthographicSize -= Time.deltaTime * speed;
            return;
        }

        spotLight.GetComponent<Light2D>().intensity = 0;
        globalLight.GetComponent<Light2D>().intensity = 0;
        gameObject.GetComponent<MeshRenderer>().enabled = false;

        SceneManager.LoadScene("Stage1");
    }
}