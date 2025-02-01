using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorObject : MonoBehaviour
{
    public float rotationSpeed = 90f;
    public float rotationAmount = 90f;

    private float currentRotation = 0f;
    private InteractionSystem interactionSystem; //Importante para poder salir de la c�mara al abrir la puerta

    public bool isOpening = false;

    private void Start()
    {
        interactionSystem = FindObjectOfType<InteractionSystem>(); //Detectar zona de Interacci�n
        if (interactionSystem == null)
        {
            Debug.LogError("No se encontr� InteractionSystem en la escena");
        }
    }

    private IEnumerator OpeningDoor() //"Animaci�n" de abrir puerta
    {
        isOpening = true;
        Debug.Log("Puerta abri�ndose...");

        if (interactionSystem != null)
        {
            interactionSystem.ExitZoom(); //Salir del zoom 
        }

        // Importante: Obtenemos el transform del padre (el pivote)
        Transform pivote = transform.parent;

        while (currentRotation < rotationAmount)
        {
            float rotationThisFrame = rotationSpeed * Time.deltaTime;
            currentRotation += rotationThisFrame;

            // Rotamos el pivote en lugar de la puerta
            pivote.Rotate(Vector3.up, rotationThisFrame);

            yield return null;
        }

        SoundManager.Instance.PlaySFX("door_open");

        isOpening = false;
        Debug.Log("Puerta completamente abierta!");
        gameObject.tag = "Untagged";
    }

    public void OpenDoor()
    {
        StartCoroutine(OpeningDoor()); //Empieza el m�todo de abrir puerta 
    }
}

