using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPuzzle : MonoBehaviour, IPuzzle
{
    public GameObject door;
    public Collider interactionZone;

    private bool isUnlocked = false;

    public void Interact(InventorySystem inventory) //Función interactuar --> Abrir puerta
    {
        if (!isUnlocked) 
        {
            if (!door.GetComponent<DoorObject>().isOpening)
            {
                OpenLock();
            }
            else
            {
                Debug.Log("La puerta se está abriendo...");
            }
        }
    }

    public void StopInteract()
    {
        //Vacío
    }
    private void OpenLock()
    {
        door.GetComponent<DoorObject>().OpenDoor();
        isUnlocked = true;
        interactionZone.tag = "Untagged";
    }
}
