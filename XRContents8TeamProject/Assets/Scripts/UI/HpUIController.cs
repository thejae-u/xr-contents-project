using UnityEngine;
using UnityEngine.UI;

public class hpUIController : MonoBehaviour
{
    public Image[] heart;

    public float hp { get; private set; }

    private float hpMax;

    private void Start()
    {
        for(int i = 0; i < heart.Length; i++) 
        {
            heart[i] = heart[i].transform.GetChild(0).GetComponent<Image>();
        }

        hpMax = heart.Length;
        hp = hpMax;
    }

    public void ExInit()
    {
        heart = new Image[transform.childCount];

        for(int i = 0;i < heart.Length;i++)
            heart[i] = transform.GetChild(i).GetChild(0).GetComponent<Image>();

            hpMax = heart.Length;
            hp = Mathf.Clamp(hp, 0, hpMax); // 최대생명 넘지 못하도록 처리    
        
        for (int i = 0; i < hpMax; i++)
        {
            heart[i].fillAmount = 0;

            if ((int)hp > i)
                heart[i].fillAmount = 1;

            if ((int)hp == i)
                heart[i].fillAmount = hp - (int)hp;
        }
    }

    public void Sethp(float damage)
    {

    }
}
