using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class LockPuzzle : MonoBehaviour, IPuzzle
{
    public string requiredKeyName;
    public GameObject door;

    private bool isUnlocked = false;

    public void Interact(InventorySystem inventory)
    {
        if (!isUnlocked) {
            if (!door.GetComponent<DoorObject>().isOpening && inventory.GetItemCount(requiredKeyName) > 0)
            {
                inventory.RemoveItem(requiredKeyName);
                OpenLock();
            }
            else if (door.GetComponent<DoorObject>().isOpening)
            {
                Debug.Log("El candado ya se está abriendo.");
            }
            else
            {
                Debug.Log("Necesitas la llave correcta.");
            }
        }
    }

    private void OpenLock()
    {   
        //isOpening = true;

        door.GetComponent<DoorObject>().OpenDoor();

        //isOpening = false;
        isUnlocked = true;
    }
}