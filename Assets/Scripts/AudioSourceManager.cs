using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// audio sources to simulate audio listeners

public class AudioSourceManager : MonoBehaviour
{
    public static AudioSourceManager Instance { get; private set; } // public singleton instance

    [Header("Player Audio")]
    public AudioSource playerBodySource; // this source will be attached to the player for footsteps
    public AudioSource[] proximityListeners; // these will be sources around the player

     [Header("Environment Audio")]
    public float globalAudioRange = 20f; // this is how far sounds can be heard
    public LayerMask audioOcclusionLayers; // these are layers that might block sound 

     private void Awake(){
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeProximityListeners();
        }
        else{
            Destroy(gameObject);
        }
    }

// initialize each listener's properties
    private void InitializeProximityListeners(){
        foreach (var source in proximityListeners){
            SetupProximitySource(source);
        }
    }

    private void SetupProximitySource(AudioSource source){
        // audio source for spacial audio
        source.spatialBlend = 1f; // 3d audio
        source.rolloffMode = AudioRolloffMode.Custom;
        source.maxDistance = globalAudioRange;
        source.dopplerLevel = 0f; // disable doppler effect
        
        // custom curve for smooth distance attenuation
        AnimationCurve customCurve = new AnimationCurve(
            new Keyframe(0f, 1f),
            new Keyframe(0.5f, 0.75f),
            new Keyframe(1f, 0f)
        );
        source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, customCurve);
    }

    // this is used when switching cameras or changing the players position
    public void UpdateProximityPositions(Vector3 playerPosition){
        if (proximityListeners.Length >= 4) {
            // place the sources in a diamond pattern around the player
            proximityListeners[0].transform.position = playerPosition + Vector3.forward * 2f;
            proximityListeners[1].transform.position = playerPosition + Vector3.right * 2f;
            proximityListeners[2].transform.position = playerPosition + Vector3.back * 2f;
            proximityListeners[3].transform.position = playerPosition + Vector3.left * 2f;
        }
    }

    // create an audio source for environmental sounds (like doors, machines, etc.)
    public AudioSource CreateEnvironmentSource(Vector3 position){
        GameObject sourceObj = new GameObject("EnvironmentSource");
        sourceObj.transform.position = position;
        AudioSource source = sourceObj.AddComponent<AudioSource>();
        SetupProximitySource(source);
        return source;
    }

    // play a sound from the proximity listener closest to the sound's origin
    public void PlayProximitySound(Vector3 position, AudioClip clip, float volume = 1f){
        AudioSource nearestSource = GetNearestProximitySource(position);
        if (nearestSource != null)
        {
            // check for audio occlusion
            if (!Physics.Linecast(Camera.main.transform.position, position, audioOcclusionLayers)){
                nearestSource.transform.position = position;
                nearestSource.PlayOneShot(clip, volume);
            }
            else
            {
                // if sound is occluded, reduce volume
                nearestSource.transform.position = position;
                nearestSource.PlayOneShot(clip, volume * 0.5f);
            }
        }
    }
 
    private AudioSource GetNearestProximitySource(Vector3 position){
        AudioSource nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (var source in proximityListeners)
        {
            float distance = Vector3.Distance(position, source.transform.position);
            if (distance < nearestDistance)
            {
                nearest = source;
                nearestDistance = distance;
            }
        }

        return nearest;
    }


}
