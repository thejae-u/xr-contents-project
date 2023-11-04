using System;
using System.Collections;
using System.Collections.Generic;
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
    
    // About Monster Spawn
    public List<Stage> stages;
    private int curStage;
    private int curSector;
    private int enemyCount;
    
    // About blood UI
    public Image bloodImage;
    private bool isCoroutineOn;
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
            return inst == null ? null : inst;
        }
    }
    

    private void Start()
    {
        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        isCoroutineOn = false;
    }

    private void Update()
    {
        enemyCount = 0;
        
        foreach (var sector in stages[curStage].sectors)
        {
            enemyCount += sector.transform.childCount;
        }

        Debug.Log($"{transform} : Remain Enemy Count is {enemyCount}");
        if (stages[curStage].sectors.Count > curSector)
        {
            if (stages[curStage].sectors[curSector].transform.childCount == 0)
            {
                curSector++;
                if (stages[curStage].sectors.Count > curSector)
                    EnemySpawn(curStage, curSector);
            }
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
