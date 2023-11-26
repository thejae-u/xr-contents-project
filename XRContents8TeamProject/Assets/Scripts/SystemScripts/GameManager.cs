using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class Stage
{
    public List<GameObject> sectors;
}

public class GameManager : MonoBehaviour
{
    private PlayerManager playerManager;

    private int everyMonsterCount;
    public Image blackImage;
    private float fadeTime;
    private bool isFade;
    
    // About Monster Spawn
    public List<Stage> stages;
    private int curStage;
    private int curSector;
    private int enemyCount;
    
    // About blood UI
    public Image bloodImage;
    private bool isCoroutineOn;
    public float speed;
    
    public bool IsNight { get; private set; }
    
    private static GameManager inst = null;
    
    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static GameManager Inst
    {
        get
        {
            return inst;
        }
    }
    

    private void Start()
    {
        IsNight = false;
        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        isCoroutineOn = false;
        fadeTime = 1.0f;
        isFade = false;
        blackImage.color = Color.black;
        FadeIn();
    }

    private void Update()
    {
        enemyCount = 0;
        everyMonsterCount = 0;
        
        CountRemainEnemy();
        SpawnNextEnemy();

        if (stages.Count > 0)
        {
            CheckRemainEnemy();
        }

        if (everyMonsterCount == 0)
        {
            ChangeScene();
        }
        
        Exit();
    }

    private bool CheckPlayerLife()
    {
        return PlayerManager.Instance.GetPlayerHp() > 0;
    }

    private void CountRemainEnemy()
    {
        foreach (var sector in stages[curStage].sectors)
        {
            enemyCount += sector.transform.childCount;
        }
    }

    private void SpawnNextEnemy()
    {
        if (stages[curStage].sectors.Count > curSector)
        {
            if (stages[curStage].sectors[curSector].transform.childCount == 0)
            {
                curSector++;
                if (stages[curStage].sectors.Count > curSector)
                    EnemySpawn(curStage, curSector);
            }
        }
    }

    private void CheckRemainEnemy()
    {
        foreach (var stage in stages)
        {
            if (stage.sectors.Count > 0)
            {
                foreach (var sector in stage.sectors)
                {
                    if (sector == null) return;
                    everyMonsterCount += sector.transform.childCount;
                }
            }
        }
    }

    public void FadeOut(string nextScene)
    {
        isFade = true;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(blackImage.DOFade(1.0f, fadeTime));

        sequence.OnComplete(() =>
        {
            SceneManager.LoadScene(nextScene);
            isFade = false;
        });
    }

    public void FadeIn()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(blackImage.DOFade(0.0f, fadeTime));
    }

    private void ChangeScene()
    {
        if (SceneManager.GetActiveScene().name == "Stage1" && !isFade)
        {
            isFade = true;
            FadeOut("Stage2");
        }
        else if (SceneManager.GetActiveScene().name == "Stage2" && !isFade)
        {
            isFade = true;
            FadeOut("Stage3");
        }
        else if (SceneManager.GetActiveScene().name == "Stage3" && !isFade)
        {
            isFade = true;
            FadeOut("MenuAndCutScene");
        }
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
    
    public void EnemySpawn(int stage, int sector)
    {
        curStage = stage;
        curSector = sector;
        stages[curStage].sectors[curSector].SetActive(true);
    }

    public int CheckEnemyCount()
    {
        return enemyCount;
    }

    public void HitPlayer()
    {
        Color newColor = Color.white;
        newColor.a = 1f;
        bloodImage.color = newColor;
        if (!isCoroutineOn)
            StartCoroutine(BloodTime(newColor));
    }

    public List<Stage> GetEnemies()
    {
        return stages;
    }

    private IEnumerator BloodTime(Color newColor)
    {
        isCoroutineOn = true;
        while (newColor.a > 0)
        {
            newColor.a -= speed * Time.deltaTime;
            bloodImage.color = newColor;
            yield return new WaitForEndOfFrame();
        }

        newColor.a = 0.0f;
        bloodImage.color = newColor;
        isCoroutineOn = false;
    }
}
