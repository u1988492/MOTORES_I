using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    private void Awake()
    {
        // Si ya existe una instancia, destruimos esta nueva
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        // Si no existe, esta es la primera instancia
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}