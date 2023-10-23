using UnityEngine;
using UnityEngine.UI;

public class BalletUIController : MonoBehaviour
{
    public Transform BulletPos;
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

    private void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        BulletPos.transform.position = new Vector3(mousePosition.x - 70, mousePosition.y, 0);
    }

    public void SetAmmo(bool isDiscount )
    {
        if(isDiscount)
        {
            Ammo--;
        }
        else
        {
            Ammo = Ammo + maxAmmo;
        }

        Ammo = Mathf.Clamp(Ammo, 0, maxAmmo);

        for (int i = 0; i < maxAmmo; i++)
            bullet[i].sprite = drawBack;

        for (int i = 0; i < maxAmmo; i++)
            if (Ammo > i)
                bullet[i].sprite = drawFront;
    }
}
