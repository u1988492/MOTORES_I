using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

// made with the help of Small Hedge Game's tutorial on youtube

//[RequireComponent(typeof(AudioSource))] // always have to have an audio source
[System.Serializable]
    public class Sound{
        public string name; // name of the sound
        public AudioClip clip; 
        [Range(0f, 1f)] public float volume = 1f; // volume for the sound, default 100%
        [Range(0.1f, 3f)] public float pitch = 1f; 
        public bool loop = false; // true if the sound should loop, for ambience
        public bool is3D = false; // true for spacialized sounds
    }


public class SoundManager : MonoBehaviour
{
    public AudioMixerGroup masterMixerGroup; // to handle settings changes
    public List<Sound> soundList; // list of sound effects
    public static SoundManager Instance {get; private set;}
    private Dictionary<string, Sound> soundDictionary; // for quick lookup by sound name

    private void Awake(){
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }

        // initialize sound dictionary
        soundDictionary = new Dictionary<string, Sound>();
        foreach(var sound in soundList){
            soundDictionary[sound.name] = sound;
        }
    }

// create audio source to handle settings changes
    private AudioSource CreateAudioSource(GameObject parent, Sound sound){
        AudioSource audioSource = parent.AddComponent<AudioSource>();
        audioSource.clip = sound.clip;
        audioSource.volume = sound.volume;
        audioSource.pitch = sound.pitch;
        audioSource.loop = sound.loop;
        audioSource.spatialBlend = sound.is3D ? 1f : 0f; // depending on whether the sound is 3d or 2d
        audioSource.outputAudioMixerGroup = masterMixerGroup; // assign the mixer group
        return audioSource;
    }

// plays a sound by its name at a specific position
    public void PlaySound(string soundName, Vector3 position){
        if(soundDictionary.TryGetValue(soundName, out Sound sound)){
            // crear objeto de sonido y posicionar 
            GameObject soundObject = new GameObject("TempAudio"); 
            soundObject.transform.position = position;
            AudioSource audioSource = CreateAudioSource(soundObject, sound);
            audioSource.Play();

            if(!sound.loop){
                Destroy(soundObject, sound.clip.length); // remove the object after it has finished playing 
            }
            else{
                DontDestroyOnLoad(soundObject); // keep object alive if it should loop
            }
       }
       else{
            Debug.LogWarning($"El sonido '{soundName}' no se encuentra en la lista.");
       }
    }

    public void PlayAmbientSound(string soundName){
        if(soundDictionary.TryGetValue(soundName, out Sound sound)){
            GameObject ambientObject = new GameObject("AmbientSound");
            AudioSource audioSource = ambientObject.AddComponent<AudioSource>();
            audioSource.clip = sound.clip;
            audioSource.volume = 0f; // start at 0 for fade-in
            audioSource.loop = true;
            audioSource.spatialBlend = 0f; // make it a 2D sound
            audioSource.Play();

            // fade in
            StartCoroutine(FadeAudioSource(audioSource, sound.volume, 2f));
            DontDestroyOnLoad(ambientObject);
        }
    }

// stops a looping sound
    public void StopAmbientSound(string soundName, float fadeDuration = 2f){
        // find the audio source and destroy it
        var ambientObject = GameObject.Find("AmbientSound");
        if(ambientObject != null){
            AudioSource audioSource = ambientObject.GetComponent<AudioSource>();
            if(audioSource!= null && audioSource.clip.name == soundName){
                StartCoroutine(FadeAudioSource(audioSource, 0f, fadeDuration, true)); // fade out and destroy
            }
        }
    }

// fade audio in or out for a duration of time
    private IEnumerator FadeAudioSource(AudioSource audioSource, float targetVolume, float duration, bool destroyAfterFade = false){
        float startVolume = audioSource.volume;
        float elapsed = 0f;

        while(elapsed < duration){
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        audioSource.volume = targetVolume;
        
        if(destroyAfterFade){
            Destroy(audioSource.gameObject);
        }
    }

// looping walking sound
    public void PlayFootstep(Vector3 position, string surfaceType){
        string footstepSoundName = surfaceType == "Metal" ? "Footstep_Metal" : "Footstep_Sound"; // sound type depending on the surface
        if(!GameObject.Find("FootstepSound")){ // check if there is already a sound playing
            GameObject footstepObject = new GameObject("FootstepSound");
            footstepObject.transform.position = position;
            AudioSource audioSource = footstepObject.AddComponent<AudioSource>();

            if(soundDictionary.TryGetValue(footstepSoundName, out Sound sound)){
                 audioSource.clip = sound.clip;
                audioSource.volume = sound.volume;
                audioSource.pitch = sound.pitch;
                audioSource.loop = true; // loop sound
                audioSource.spatialBlend = sound.is3D ? 1f : 0f; // 3D or 2D sound
                audioSource.Play();
                DontDestroyOnLoad(footstepObject); // keep the sound alive across scenes
            }
            else{
                Debug.LogWarning($"Sonido de caminar '{footstepSoundName}' no encontrado.");
                Destroy(footstepObject);
            }
        }
    }

// stop sound when player is not walking
    public void StopFootstep(){
        var footstepObject = GameObject.Find("FootstepSound");
        if(footstepObject != null){
            AudioSource audioSource = footstepObject.GetComponent<AudioSource>();
            if(audioSource != null){
                audioSource.Stop();
                Destroy(footstepObject);
            }
        }
    }

}