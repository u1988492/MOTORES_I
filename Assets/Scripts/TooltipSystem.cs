using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipSystem : MonoBehaviour
{
    public static TooltipSystem Instance { get; private set; } //Singleton

    [SerializeField] private GameObject tooltipContainer;
    [SerializeField] private GameObject tooltipImageContainer;

    [SerializeField] private TextMeshProUGUI tooltipName;
    [SerializeField] private Image tooltipSprite;
    [SerializeField] private TextMeshProUGUI tooltipDesc;

    [SerializeField] private TextMeshProUGUI imageContent;
    [SerializeField] private Image imageObject;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        tooltipContainer.SetActive(false);
        tooltipImageContainer.SetActive(false);
    }

    public void Show(bool showtooltipWithImage, string name, string desc, Sprite spri, Sprite img)
    {
        tooltipName.text = name;
        tooltipDesc.text = desc;
        imageContent.text = desc;

        if (showtooltipWithImage)
        {
            tooltipSprite.sprite = null;
            tooltipName.enabled = false;
            tooltipDesc.enabled = false;
            imageContent.enabled = true;
            tooltipSprite.color = Color.clear;
            imageObject.sprite = img;
            imageObject.color = Color.white;
            tooltipImageContainer.SetActive(true);
            tooltipContainer.SetActive(false);
        }
        else 
        {
            tooltipContainer.SetActive(false);
            tooltipContainer.SetActive(true);
            tooltipName.enabled = true;
            tooltipDesc.enabled = true;
            imageContent.enabled = false;
            tooltipSprite.sprite = spri;
            tooltipSprite.color = Color.white;
        }

        Debug.Log("Tooltip");
        //tooltipContainer.SetActive(true);
    }

    public void Hide()
    {
        tooltipContainer.SetActive(false);
        tooltipImageContainer.SetActive(false);
    }
}