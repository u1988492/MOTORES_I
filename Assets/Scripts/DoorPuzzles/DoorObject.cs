using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorObject : MonoBehaviour
{
    public float rotationSpeed = 90f;
    public float rotationAmount = 90f;

    private float currentRotation = 0f;
    private InteractionSystem interactionSystem; //Importante para poder salir de la cámara al abrir la puerta

    public bool isOpening = false;

    private void Start()
    {
        interactionSystem = FindObjectOfType<InteractionSystem>(); //Detectar zona de Interacción
        if (interactionSystem == null)
        {
            Debug.LogError("No se encontró InteractionSystem en la escena");
        }
    }

    private IEnumerator OpeningDoor(Transform pivote, Vector3? pos = null) //"Animación" de abrir puerta
    {
        isOpening = true;
        Debug.Log("Puerta abriéndose...");

        Vector3 finalPos = pos ?? Vector3.up;

        if (interactionSystem != null)
        {
            interactionSystem.ExitZoom(); //Salir del zoom 
        }

        while (currentRotation < rotationAmount)
        {
            float rotationThisFrame = rotationSpeed * Time.deltaTime;
            currentRotation += rotationThisFrame;

            // Rotamos el pivote en lugar de la puerta
            pivote.Rotate(finalPos, rotationThisFrame);

            yield return null;
        }

        isOpening = false;
        Debug.Log("Puerta completamente abierta!");
        gameObject.tag = "Untagged";
    }

    public void OpenDoor()
    {
        Transform pivote = transform.parent; // Importante: Obtenemos el transform del padre (el pivote)
        StartCoroutine(OpeningDoor(pivote)); //Empieza el método de abrir puerta 
    }

    public void OpenDoorWithoutPivot(Vector3? pos = null)
    {
        Transform pivote = transform;
        StartCoroutine(OpeningDoor(pivote, pos)); //Empieza el método de abrir puerta 
    }
}

