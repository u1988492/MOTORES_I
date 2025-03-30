using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BunkerState : MonoBehaviour
{
    public static BunkerState Instance { get; private set; }
    private GameObject bunkerContent;
    private Vector3 originalPosition; // Guardamos la posición original

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SaveBunkerState()
    {
        bunkerContent = GameObject.Find("BunkerContent");
        if (bunkerContent != null)
        {
            // Guardamos la posición original
            originalPosition = bunkerContent.transform.position;

            // Lo hacemos hijo y lo preservamos
            bunkerContent.transform.SetParent(transform);
            DontDestroyOnLoad(bunkerContent);

            // Lo ocultamos
            bunkerContent.SetActive(false);
        }
    }

    public void RestoreBunkerState()
    {
        if (bunkerContent != null)
        {
            // Desvinculamos del padre
            bunkerContent.transform.SetParent(null);

            // Restauramos la posición original
            bunkerContent.transform.position = originalPosition;

            // Lo mostramos de nuevo
            bunkerContent.SetActive(true);
        }
    }
}