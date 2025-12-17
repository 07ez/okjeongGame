using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject itemUI;
    public TMP_Text itemName;
    public GameObject itemBar;
    public Image ResetIMG;
    public RectTransform OKrt;
    RectTransform rt;
    Animator anim;

    float barSpeed;

    bool isGetItem = false;

    public int sw = 1;

    private void Awake()
    {
        instance = this;

        rt = itemBar.GetComponent<RectTransform>();
        anim = itemUI.GetComponent<Animator>();
        StartCoroutine(Golefttext());
    }

    public void GetItem(int itemNum) // 0 == ΩØµÂ, 1 == ∆¯≈∫
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

    public void ResetMap()
    {
        OKrt.anchoredPosition = new Vector2(-1200, 0);
        StartCoroutine(Golefttext());
    }

    IEnumerator Golefttext()
    {

        sw *= -1; // 1¿Ã∏È fadeOut | -1¿Ã∏È fadeIn

        Vector2 start = OKrt.anchoredPosition;
        Vector2 end = start + new Vector2(1200, 0);
        Color colorstart = ResetIMG.color;
        Color colorend = new Color(colorstart.r, colorstart.g, colorstart.b, colorstart.a + 1f * sw);

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / 0.5f;

            Vector2 pos = Vector2.Lerp(start, end, t);
            Color color = Color.Lerp(colorstart, colorend, t);

            OKrt.anchoredPosition = pos;
            ResetIMG.color = color;

            yield return null;
        }
        if (sw == 1) // FadeOut
        {
            yield return new WaitForSecondsRealtime(0.5f);
            GameManager.instance.ResetMap();
            StartCoroutine(Golefttext());
        }
    }
}
