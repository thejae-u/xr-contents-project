using System;
using System.Collections;
using System.Collections.Generic;
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
    
    public bool IsAttacked { get; private set; }
    
    // 외부에서 실행중임을 확인
    public bool IsEnded { get; private set; }
    
    private void Start()
    {
        curValue = 0.0f;
        IsEnded = false;
        IsAttacked = false;
        weakImage = gameObject.GetComponent<Image>();
    }

    private void Update()
    {
        if (isRunning && normalizedValue <= 1.0f)
        {
            curValue += Time.deltaTime;

            normalizedValue = curValue / waitTime;
            weakImage.fillAmount = normalizedValue;

            myColor = Color.Lerp(Color.white, Color.red, normalizedValue);
            weakImage.color = myColor;
        }
        else if (normalizedValue > 1.0f)
        {
            IsAttacked = false;
            isRunning = false;
            IsEnded = true;
        }
    }

    public void Init()
    {
        isRunning = true;
    }

    public void Checked()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }

    public void Hit()
    {
        isRunning = false;
        IsAttacked = true;
        IsEnded = true;
    }
}
