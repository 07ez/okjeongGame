using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject itemUI;
    public TMP_Text itemName;
    public GameObject itemBar;
    RectTransform rt;
    Animator anim;

    float barSpeed;

    bool isGetItem = false;

    private void Awake()
    {
        instance = this;

        rt = itemBar.GetComponent<RectTransform>();
        anim = itemUI.GetComponent<Animator>();
    }
    public void GetItem(int itemNum) // 0 == ½¯µå, 1 == ÆøÅº
    {
        anim.SetBool("GetItem", true);
        isGetItem = true;

        if (itemNum == 0)
        {
            itemName.text = "Shield";
            itemName.color = new Color(92 / 255f, 1, 251 / 255f);
            itemBar.GetComponent<Image>().color = new Color(92 / 255f, 1, 251 / 255f);
            rt.sizeDelta = new Vector2(480, 8);
            barSpeed = 10f;
        }
        else if (itemNum == 1)
        {
            itemName.text = "Bomb";
            itemName.color = new Color(178 / 255f, 0, 0);
            itemBar.GetComponent<Image>().color = new Color(178 / 255f, 0, 0);
            rt.sizeDelta = new Vector2(480, 8);
            barSpeed = 1f;
        }
        else if (itemNum == 2)
        {
            itemName.text = "Slow";
            itemName.color = new Color(42 / 255f, 163 / 255f, 42 / 255f);
            itemBar.GetComponent<Image>().color = new Color(42 / 255f, 163 / 255f, 42 / 255f);
            rt.sizeDelta = new Vector2(480, 8);
            barSpeed = 1.4f;
        }
        else if (itemNum == 3)
        {
            itemName.text = "Star";
            itemName.color = new Color(200 / 255f, 163 / 255f, 41 / 255f);
            itemBar.GetComponent<Image>().color = new Color(200 / 255f, 163 / 255f, 41 / 255f);
            rt.sizeDelta = new Vector2(480, 8);
            barSpeed = 3f;
        }
    }

    public void UseItem()
    {
        isGetItem = false;
        anim.SetBool("GetItem", false);
    }

    private void Update()
    {
        if (isGetItem)
        {
            rt.sizeDelta = new Vector2(rt.sizeDelta.x - Time.deltaTime * (480f / barSpeed),
            rt.sizeDelta.y);
        }
    }
}
