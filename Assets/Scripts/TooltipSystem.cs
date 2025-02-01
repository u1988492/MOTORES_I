using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipSystem : MonoBehaviour
{
    public static TooltipSystem Instance { get; private set; } //Singleton

    [SerializeField] private GameObject tooltipContainer;

    [SerializeField] private TextMeshProUGUI tooltipName;
    [SerializeField] private Image tooltipSprite;
    [SerializeField] private Image tooltipImage;
    [SerializeField] private TextMeshProUGUI tooltipDesc;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        tooltipContainer.SetActive(false);
    }

    public void Show(bool showtooltipWithImage, string name, string desc, Sprite spri, Sprite img)
    {
        tooltipName.text = name;
        tooltipDesc.text = desc;

        if (showtooltipWithImage)
        {
            tooltipImage.sprite = img;
            tooltipImage.color = Color.white;
            tooltipSprite.sprite = null;
            tooltipSprite.color = Color.clear;
        }
        else 
        {
            tooltipImage.sprite = null;
            tooltipImage.color = Color.clear;
            tooltipSprite.sprite = spri;
            tooltipSprite.color = Color.white;
        }

        Debug.Log("Tooltip");
        tooltipContainer.SetActive(true);
    }

    public void Hide()
    {
        tooltipContainer.SetActive(false);
    }
}