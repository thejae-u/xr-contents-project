using UnityEngine;
using UnityEngine.UI;

public class BalletUIController : MonoBehaviour
{
    public Transform bulletPos;
    public Image[] bullet;

    public int Ammo { get; private set; }

    private int maxAmmo;

    public Sprite drawBack;
    public Sprite drawFront;

    public Sprite drawMouseBack;
    public Sprite drawMouseFront;

    private void Awake()
    {
        maxAmmo = bullet.Length / 2;
        Ammo = maxAmmo;

        for(int i = 0; i < maxAmmo; i++)
        {
            if(Ammo > i)
            {
                bullet[i].sprite = drawMouseFront;
                bullet[i + 6].sprite = drawFront;
            }
        }
    }

    private void Update()
    {
        if (bulletPos != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            bulletPos.transform.position = new Vector3(mousePosition.x - 70, mousePosition.y, 0);
        }
    }

    public void SetAmmo(bool isDiscount)
    {
        if (isDiscount)
        {
            Ammo--;
        }
        else
        {
            Ammo = maxAmmo;
        }

        Ammo = Mathf.Clamp(Ammo, 0, maxAmmo);

        for (int i = 0; i < maxAmmo; i++)
        {
            bullet[i].sprite = drawMouseBack;
            bullet[i + 6].sprite = drawBack;
        }

        for (int i = 0; i < maxAmmo; i++)
        {
            if (Ammo > i)
            {
                bullet[i].sprite = drawMouseFront;
                bullet[i + 6].sprite = drawFront;
            }
        }
    }
}
