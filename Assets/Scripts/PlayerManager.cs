using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    private Vector3 pendingPosition;
    private Quaternion pendingRotation;
    private bool hasPendingTransform = false;

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

    public void SetPosition(Vector3 position, Quaternion rotation)
    {
        pendingPosition = position;
        pendingRotation = rotation;
        hasPendingTransform = true;
    }

    private void LateUpdate()
    {
        if (hasPendingTransform)
        {
            transform.position = pendingPosition;
            transform.rotation = pendingRotation;
            hasPendingTransform = false;
            Debug.Log($"Posición real después de mover: {transform.position}");
        }
    }
}