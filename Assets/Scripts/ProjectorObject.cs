using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectorObject : MonoBehaviour, IUsable
{
    public string requiredProjectorImages;
    public GameObject projector;

    private bool isObtained = false;

    void Start()
    {
        projector.SetActive(false);
    }

    public void Usar(InventorySystem inventory)
    {
        SoundManager.Instance.PlaySFX("click");
        if (!isObtained)
        {
            if(inventory.GetItem(requiredProjectorImages))
            {
                inventory.RemoveItem(requiredProjectorImages);
                projector.SetActive(true);
                gameObject.tag = "Untagged";
            }
        }
    }
}
