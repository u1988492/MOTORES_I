//Este script controla la lista de tablones y abre la puerta una vez que todos los tablones han sido eliminados
using UnityEngine;
using System.Collections.Generic;

public class PlankPuzzle : MonoBehaviour{
    public GameObject door; // Referencia a la puerta
    public Collider interactionZone;
    public List<GameObject> planks; // Lista de tablones que bloquean la puerta

    private void OnEnable(){// Escucha el evento de eliminación de tablones
        
        PlankObject.PlankRemoved += HandlePlankRemoved;
    }

    private void OnDisable(){// Deja de escuchar el evento cuando este script se desactive
        
        PlankObject.PlankRemoved -= HandlePlankRemoved;
    }
    
    private void HandlePlankRemoved(PlankObject removedPlank){
        // Convierte el PlankObject a su GameObject asociado
        GameObject plankGameObject = removedPlank.gameObject;
        
        // Elimina el GameObject de la lista
        if (planks.Contains(plankGameObject))
        {
            planks.Remove(plankGameObject);
            Debug.Log($"Un tablón ha sido eliminado. Restan {planks.Count} tablones.");

            // Si no quedan tablones, abre la puerta
            if (planks.Count == 0)
            {
            door.GetComponent<DoorObject>().OpenDoor();
            interactionZone.tag = "Untagged";
            }
        }
    }
}