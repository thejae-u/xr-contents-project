using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryReader : MonoBehaviour
{
    private string filePath = "TestScript.csv";
    
    private void Start()
    {
        TextAsset csvText = Resources.Load<TextAsset>(filePath);

        if (csvText != null)
        {
            string[] lines = csvText.text.Split(',');

            foreach (var line in (lines))
            {
                string[] datas = line.Split(',');

                foreach (var data in datas)
                {
                    Debug.Log(data);
                }
            }
        }
        else
        {
            Debug.LogError("파일을 불러 올 수 없습니다.");
        }
    }
}
