using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class LockPuzzle : MonoBehaviour, IPuzzle
{
    public string requiredKeyName;
    public GameObject door;
    public Collider interactionZone;

    private bool isUnlocked = false;

    public void Interact(InventorySystem inventory)
    {
        if (!isUnlocked) {
            if (!door.GetComponent<DoorObject>().isOpening && inventory.GetItem(requiredKeyName))
            {
                inventory.RemoveItem(requiredKeyName);
                OpenLock();
                SoundManager.Instance.PlaySFX("beep");
            }
            else if (door.GetComponent<DoorObject>().isOpening)
            {
                Debug.Log("El candado ya se est� abriendo.");
            }
            else
            {
                Debug.Log("Necesitas la llave correcta.");
            }
        }
    }

    public void StopInteract()
    {
        //Vac�o
    }

    private void OpenLock()
    {   
        door.GetComponent<DoorObject>().OpenDoor();
        isUnlocked = true;
        interactionZone.tag = "Untagged";
    }
}