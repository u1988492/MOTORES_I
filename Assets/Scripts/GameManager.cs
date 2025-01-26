using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public bool isGuardianVisionUnlocked = false;
    public bool isGuardianVisionActive = false;

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
}
