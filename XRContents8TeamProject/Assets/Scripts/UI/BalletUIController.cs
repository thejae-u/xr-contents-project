using UnityEngine;
using UnityEngine.UI;

public class BalletUIController : MonoBehaviour
{
    public Image[] bullet;

    public int Ammo { get; private set; }

    private int maxAmmo;

    public Sprite drawBack;
    public Sprite drawFront;

    private void Awake()
    {
        maxAmmo = bullet.Length;
        Ammo = maxAmmo;

        for(int i = 0; i < maxAmmo; i++)
        {
            if(Ammo > i)
            {
                bullet[i].sprite = drawFront;
            }
        }
    }

    public void SetAmmo(bool isDiscount )
    {
        if(isDiscount)
        {
            Ammo--;
        }
        else
        {
            Ammo++;
        }

        Ammo = Mathf.Clamp(Ammo, 0, maxAmmo);

        for (int i = 0; i < maxAmmo; i++)
            bullet[i].sprite = drawBack;

        for (int i = 0; i < maxAmmo; i++)
            if (Ammo > i)
                bullet[i].sprite = drawFront;
    }
}
