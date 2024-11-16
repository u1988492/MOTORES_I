using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonScript : MonoBehaviour, IPuzzle
{
    public int keyPadNumber = 1;
    public UnityEvent KeyPadClicked;


    public void Interact(InventorySystem inventory){
        KeyPadClicked.Invoke();
    }

    public void StopInteract()
    {
        //Vacío
    }
}
