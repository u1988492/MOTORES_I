using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ImagesProjector : MonoBehaviour
{
    public Material[] materials; //Lista de materiales con las imágenes
    public DecalProjector decalProjector; //Objeto que genera la imagen en la pared
    public float timeSwitchImages = 5f;

    private float timer;
    private int i = 0;
    void Start()
    {
        decalProjector = GetComponent<DecalProjector>(); //Conseguir materiales
        decalProjector.material = materials[0];
        i++; 
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > timeSwitchImages) //Al llegar al tiempo se cambia material
        {
            switchMaterial();
            i++;
            timer = 0f;
        }
    }

    void switchMaterial()
    {
        if(i >= materials.Length)
        {
            i = 0;
        }
        decalProjector.material = materials[i];
    }
}
