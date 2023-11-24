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

    public GameObject uiCanvas;

    private GameObject globalLight = null;
    private GameObject spotLight = null;

    private bool isInitialized;
    
    private int curState;
    private bool isEndFirstAnim;

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

        for (int i = 0; i < names.Length - 1; i++)
        {
            if (i < 2)
                continue;
            anim.AnimationState.Data.SetMix(names[i], names[i + 1], 0);
        }
        
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        curState = 0;
        isInitialized = false;
        isEndFirstAnim = false;
        isStart = false;

        ShowMenu();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "CutScene") return;
        
        
        
        CheckShowMenu();
        
        
        if (!isStart)
        {
            AnimationUpdate();
            return;
        }
        


        GameStart();
    }

    private void InitObject()
    {
        if (SceneManager.GetActiveScene().name != "MenuAndCutScene") return;
        
        globalLight = GameObject.Find("Global");
        spotLight = GameObject.Find("Spot");
        isInitialized = true;
    }

    private void AnimationCall()
    {
        anim.AnimationState.SetAnimation(0, names[curState++], false);
    }

    private void AnimationUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            switch (anim.AnimationName)
            {
                // First CutScene
                case "Book_Open_1" when anim.AnimationState.GetCurrent(0).IsComplete:
                    AnimationCall();
                    break;
                case "Book_Open_1":
                    break;
                
                case "Page2" when anim.AnimationState.GetCurrent(0).IsComplete:
                    AnimationCall();
                    break;
                case "Page2":
                    break;
                
                case "Page3" when anim.AnimationState.GetCurrent(0).IsComplete:
                    AnimationCall();
                    break;
                case "Page3":
                    break;
                
                case "Page4" when anim.AnimationState.GetCurrent(0).IsComplete:
                    AnimationCall();
                    break;
                case "Page4":
                    break;
                
                case "Page5" when anim.AnimationState.GetCurrent(0).IsComplete:
                    AnimationCall();
                    break;
                case "Page5":
                    break;
                
                case "Page6" when anim.AnimationState.GetCurrent(0).IsComplete:
                    isStart = true;
                    break;
                case "Page6":
                    break;
                
                // Second CutScene
                case "Book_Open_7" when anim.AnimationState.GetCurrent(0).IsComplete:
                    AnimationCall();
                    break;
                case "Book_Open_7":
                    break;
                case "Page8" when anim.AnimationState.GetCurrent(0).IsComplete:
                    AnimationCall();
                    break;
                case "Page8":
                    break;
                case "Page9" when anim.AnimationState.GetCurrent(0).IsComplete:
                    AnimationCall();
                    break;
                case "Page9":
                    break;
                case "Page10" when anim.AnimationState.GetCurrent(0).IsComplete:
                    break;
                case "Page10":
                    break;
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
        if (anim.AnimationName == "Start1" && anim.AnimationState.GetCurrent(0).IsComplete)
        {
            uiCanvas.SetActive(true);
            isStart = true;
        }
    }

    public void OnStartButtonClick()
    {
        AnimationCall();
        uiCanvas.SetActive(false);
    }

    private void SecondStartAnimation()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = true;
        AnimationCall();
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