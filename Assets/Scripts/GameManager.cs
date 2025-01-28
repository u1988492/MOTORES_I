using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public bool isGuardianVisionUnlocked = false;
    public bool isGuardianVisionActive = false;

    public Vector3 positionBunker;
    public Vector3 positionGuardian;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        } 
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public bool IsVisionUnlocked()
    {
        return isGuardianVisionUnlocked;
    }

    public bool IsVisionActive()
    {
        return isGuardianVisionActive;
    }

    public void ActiveVision()
    {
        isGuardianVisionActive = true;
    }

    public void DesactiveVision()
    {
        isGuardianVisionActive = false;
    }

    public void SaveBunkerCoords(Vector3 pos)
    {
        positionBunker = pos;
    }

    public Vector3 GetBunkerCoords()
    {
        return positionBunker;
    }

    public void SaveGuardianCoords(Vector3 pos)
    {
        positionGuardian = pos;
    }

    public Vector3 GetGuardianCoords()
    {
        return positionGuardian;
    }
}
