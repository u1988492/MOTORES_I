using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ImagesProjector : MonoBehaviour
{
    public Material[] materials;
    public DecalProjector decalProjector;
    public float timeSwitchImages = 5f;

    private float timer;
    private int i = 0;
    void Start()
    {
        decalProjector = GetComponent<DecalProjector>();
        decalProjector.material = materials[0];
        i++;
    }
    void Update()
    {
        timer += Time.deltaTime;

        if (timer > timeSwitchImages)
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
