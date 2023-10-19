using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WeakTimeController : MonoBehaviour
{
    [Header("대기 시간 조정")] 
    [SerializeField] private float waitTime;
    

    private float curValue;
    private float curColorValue;
    
    // 내부에서 실행중임을 확인
    private bool isRunning;
    
    private Image weakImage;
    private Color myColor;

    private float normalizedValue;
    private bool isSequenceOn;
    
    public bool IsAttacked { get; private set; }
    
    // 외부에서 실행중임을 확인
    public bool IsEnded { get; private set; }
    
    private void Start()
    {
        curValue = 0.0f;
        IsEnded = false;
        IsAttacked = false;
        weakImage = gameObject.GetComponent<Image>();
        myColor = Color.white;
        isSequenceOn = false;
    }

    private void Update()
    {
        if (!isRunning)
        {
            if (isSequenceOn) return;

            var sequence = DOTween.Sequence();
            LogPrintSystem.SystemLogPrint(transform, "Sequence Active", ELogType.EnemyAI);
            myColor = Color.red;
            weakImage.color = myColor;
            isSequenceOn = true;
            
            sequence.SetDelay(1.0f).OnComplete(() =>
            {
                IsEnded = true;
            }).SetId(this);
        }
        else
        {
            Execute();
        }
    }

    private void Execute()
    {
        if (normalizedValue <= 1.0f)
        {
            curValue += Time.deltaTime;

            normalizedValue = curValue / waitTime;
            weakImage.fillAmount = normalizedValue;

            weakImage.color = myColor;
        }
        else if (normalizedValue > 1.0f)
        {
            LogPrintSystem.SystemLogPrint(transform, "Gauge Full", ELogType.EnemyAI);
            IsAttacked = false;
            isRunning = false;
        }
    }

    public void Init()
    {
        isRunning = true;
    }

    public void Checked()
    {
        if(DOTween.IsTweening(DOTween.Sequence(this)))
            DOTween.Sequence(this).Kill();
        Destroy(gameObject.transform.parent.gameObject);
    }

    public void Hit()
    {
        isRunning = false;
        IsAttacked = true;
        IsEnded = true;
    }
}
