using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    public Image blackImage;

    public GameObject globalLight;
    public GameObject spotLight;
    
    private CutSceneCounter Inst = CutSceneCounter.Inst;

    private SkeletonAnimation anim;

    private int soundCount;

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

    private void DeleteMixAnimation()
    {
        for (int i = 0; i < names.Length; i++)
        {
            for (int j = 0; j < names.Length; j++)
            {
                if (j == i) continue;
                anim.AnimationState.Data.SetMix(names[i], names[j], 0);
            }
        }
    }

    private void Awake()
    {
        anim = gameObject.GetComponent<SkeletonAnimation>();
        DeleteMixAnimation();
    }

    private void Start()
    {
        SoundManager.Inst.Play("BgmMenu");
        AnimationCall();
    }


    private void Update()
    {
        ControlNightIntensity();
        
        if (Inst.IsEndingOn)
        {
            Inst.IsEndingOn = false;
            OffAllButtons();
        }

        if (!Inst.IsEndFirstAnim)
        {
            CheckShowMenu();
            return;
        }

        if (!Inst.IsEndSecondAnim)
        {
            SetStartAnimation();
            return;
        }
        
        ShowPageButton();
        
        
        if(Inst.IsStart)
            GameStart();
    }

    public void OnNextButtonClick()
    {
        nextButton.SetActive(false);
        
        if (anim.AnimationState.GetCurrent(0).IsComplete)
        {
            switch (soundCount)
            {
                case 0:
                    soundCount++;
                    SoundManager.Inst.Play("BookPage1");
                    break;
                case 1:
                    soundCount++;
                    SoundManager.Inst.Play("BookPage2");
                    break;
                case 2:
                    soundCount++;
                    SoundManager.Inst.Play("BookPage3");
                    break;
                case 3:
                    soundCount++;
                    SoundManager.Inst.Play("BookPage4");
                    break;
                case 4:
                    soundCount = 0;
                    SoundManager.Inst.Play("BookPage5");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            switch (anim.AnimationName)
            {
                case "Page6":
                    Inst.IsStart = true;
                    return;
                case "Page10":
                    Sequence sequence = DOTween.Sequence();
                    sequence.Append(blackImage.DOFade(1.0f, speed / 4.0f)).OnComplete(() =>
                    {
                        CutSceneCounter.Inst.CurState = -1;
                        CutSceneCounter.Inst.SettingGameOver();
                        AnimationCall();
                        sequence.Append(blackImage.DOFade(0.0f, speed / 4.0f));
                    });
                    return;
                default:
                    AnimationCall();
                    break;
            }
        }
    }

    private void ControlNightIntensity()
    {
        switch (anim.AnimationName)
        {
            case "Page9":
                spotLight.GetComponent<Light2D>().intensity = 5.0f;
                return;
            default:
                spotLight.GetComponent<Light2D>().intensity = 10.0f;       
                return;        
        }
    }

    private void AnimationCall()
    {
        CutSceneCounter.Inst.CurState += 1;
        anim.AnimationState.SetAnimation(0, names[CutSceneCounter.Inst.CurState], false);
    }

    private void SetStartAnimation()
    {
        if (anim.AnimationState.GetCurrent(0).IsComplete)
        {
            AnimationCall();
            Inst.IsEndSecondAnim = true;
            SoundManager.Inst.Play("BookOpen");
        }
    }

    private void ShowPageButton()
    {
        if (Inst.IsStart)
        {
            skipButton.SetActive(false);
            nextButton.SetActive(false);
            return;
        }

        if (!anim.AnimationState.GetCurrent(0).IsComplete) return;

        switch (anim.AnimationName)
        {
            case "Start1":
            case "Start2":
                nextButton.SetActive(false);
                return;
            case "Book_Open_1" when anim.AnimationState.GetCurrent(0).IsComplete:
                nextButton.SetActive(true);
                skipButton.SetActive(true);
                return;
            case "Book_Open_1":
                return;
            default:
                nextButton.SetActive(anim.AnimationState.GetCurrent(0).IsComplete);
                return;
        }
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
        Inst.IsEndFirstAnim = true;
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
        CutSceneCounter.Inst.CurState = 6;
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

    private void OffAllButtons()
    {
        startButton.SetActive(false);
        settingButton.SetActive(false);
        exitButton.SetActive(false);
        nextButton.SetActive(false);
        skipButton.SetActive(false);
    }
    
    private void GameStart()
    {
        if (!Inst.IsFade)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(blackImage.DOFade(1.0f, speed / 4.0f));
            SoundManager.Inst.SoundFadeOut();
            
            sequence.Play();
            Inst.IsFade = true;
        }
        
        if (Camera.main.orthographicSize > range)
        {
            Camera.main.orthographicSize -= Time.deltaTime * speed;
            return;
        }

        spotLight.GetComponent<Light2D>().intensity = 0;
        globalLight.GetComponent<Light2D>().intensity = 0;

        SceneManager.LoadScene("Stage1");
    }
}