//Este script se encarga de destruir el tablón cuando el jugador interactúa con él y notifica a PlankPuzzle
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlankObject : MonoBehaviour, IPuzzle{
    public static event Action<PlankObject> PlankRemoved = delegate {}; //envía la información necesaria al script PlankPuzzle
    private bool coroutineAllowed = true; //controla si la interacción es posible
    public string requiredToolName; // Nombre del objeto necesario para interactuar (palanca)



    public void Interact(InventorySystem inventory) { //Llama a RemovePlank solo cuando esté en la interfaz IPuzzle
        if (inventory.GetItem(requiredToolName))
        {
            if (coroutineAllowed)
            {
                StartCoroutine(RemovePlank());
            }
        }
        else
        {
            Debug.Log("Necesitas una palanca para quitar este tablón.");
        }
    }

    public void StopInteract()
    {
        //Vacío
    }
    
    private IEnumerator RemovePlank()
    {
        coroutineAllowed = false; // Evita interacciones repetidas
        yield return new WaitForSeconds(0.5f); // Simula un pequeño retraso para la acción

        
        PlankRemoved.Invoke(this); // Notifica al `PlankPuzzle` que este tablón ha sido eliminado

        // Destruye el objeto
        Destroy(gameObject);
    }

}
