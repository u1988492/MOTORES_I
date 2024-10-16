using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorObject : MonoBehaviour
{
    public float rotationSpeed = 90f; // Velocidad de rotación en grados por segundo
    public float rotationAmount = 90f; // Cantidad total de rotación en grados

    private float currentRotation = 0f;
    private InteractionSystem interactionSystem;

    public bool isOpening = false;

    private void Start()
    {
        // Busca el InteractionSystem en la escena
        interactionSystem = FindObjectOfType<InteractionSystem>();
        if (interactionSystem == null)
        {
            Debug.LogError("No se encontró InteractionSystem en la escena");
        }
    }

    public void OpenDoor()
    {
        StartCoroutine(OpeningDoor());
    }

    private IEnumerator OpeningDoor()
    {
        isOpening = true;
        Debug.Log("Puerta abriéndose...");

        if (interactionSystem != null)
        {
            interactionSystem.ExitZoom();
        }

        while (currentRotation < rotationAmount)
        {
            float rotationThisFrame = rotationSpeed * Time.deltaTime;
            currentRotation += rotationThisFrame;

            // Rotamos alrededor del eje Y (ajusta esto si necesitas otro eje)
            transform.Rotate(Vector3.up, rotationThisFrame);

            yield return null;
        }

        isOpening = false;

        Debug.Log("Puerta completamente abierta!");

        gameObject.tag = "Untagged";
    }
}
