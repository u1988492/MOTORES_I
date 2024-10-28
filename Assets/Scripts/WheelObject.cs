/*This script controls the rotation of the individual object 'WHEEL'
The numberShown maximum will depend on the number of the faces of the wheel - change if needed
The Rotation of the wheel will depend on the Transform attributes of the wheel - change if needed
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Wheel : MonoBehaviour {

    public static event Action<string, int> Rotated = delegate {}; //sends the information needed to the wheel puzzle control script
    private bool coroutineAllowed; //if the wheel is not rotating, it will be true
    private int numberShown;

    private void Start() {
        coroutineAllowed = true;
        numberShown = 0;
    }

    private void OnMouseDown() {
        if(coroutineAllowed){
            StartCoroutine("RotateWheel");
        }
    }

    private IEnumerator RotateWheel() {
        coroutineAllowed = false;

        for (int i = 0; i <= 11; i++){
            transform.Rotate( 0f, 3f, 0f);
            yield return new WaitForSeconds(0.01f);
        }

        coroutineAllowed = true;
        numberShown += 1;

        if (numberShown > 9){
            numberShown = 0;
        }

        Rotated(name, numberShown);
    }
}
