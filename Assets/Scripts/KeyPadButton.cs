using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonScript : MonoBehaviour
{
    public int keyPadNumber = 1;
    public UnityEvent KeyPadClicked;


    private void OnMouseDown(){
        KeyPadClicked.Invoke();
    }
}
