using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; } // public singleton instance

    [System.Serializable]
    public class SoundSet
    {
        public string name;
        public AudioClip[] clips;
        [Range(0f, 1f)] // volume
        public float volume = 1f;
        public bool loop = false;
        public float minTimeBetweenPlays = 0.1f;
        private float lastPlayTime;

        public bool CanPlay()
        {
            return Time.time - lastPlayTime >= minTimeBetweenPlays; // limit to avoid sound spamming
        }

        public void UpdatePlayTime()
        {
            lastPlayTime = Time.time;
        }
    }
    
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Sound Sets")]
    public SoundSet[] footstepSets; // different for each surface type
    public SoundSet[] ambientSets;  // different according to the scene
    public SoundSet[] sfxSets; // effects for interactions


   [Header("Audio Sources")]
    public AudioSource footstepSource;
    public AudioSource primaryAmbientSource;    // main ambient sound
    public AudioSource secondaryAmbientSource;  // for smooth transitions
    public AudioSource sfxSource;

    [Header("Scene Settings")]
    public string bunkerSceneName = "Bunker"; 
    public string beachSceneName = "AstralPlane";
    public float ambientFadeDuration = 2.0f; 

    private Dictionary<string, SoundSet> soundSetLookup = new Dictionary<string, SoundSet>();
    private string currentScene;
    private bool isTransitioningAmbient = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSoundSets();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy(){
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void InitializeSoundSets()
    {
        // initialize footstep sounds
        foreach (var set in footstepSets){
            soundSetLookup[$"footstep_{set.name}"] = set;
        }

        // initialize ambient sounds
        foreach (var set in ambientSets){
            soundSetLookup[$"ambient_{set.name}"] = set;
        }

        // initialize sfx sounds
        foreach (var set in sfxSets){
            soundSetLookup[$"sfx_{set.name}"] = set;
        }
    }

// handles transitions between scenes
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        currentScene = scene.name;
        
        // play the ambient sound associated to the scene type
        string ambientType = GetAmbientTypeForScene(scene.name);
        TransitionAmbientSound(ambientType);
        
        Debug.Log($"Scene loaded: {scene.name}, playing ambient type: {ambientType}");
    }

    private string GetAmbientTypeForScene(string sceneName){
        if (sceneName == bunkerSceneName) return "bunker";
        if (sceneName == beachSceneName) return "astral";
        return "mainMenu"; // main menu ambience as default
    }

    public string GetCurrentSurfaceType(){
        return currentScene == bunkerSceneName ? "metal" : "sand";
    }


// plays a footstep sound according the surface type
    public void PlayFootstep(){
        string surface = GetCurrentSurfaceType();
        string key = $"footstep_{surface}";
        
        if (soundSetLookup.TryGetValue(key, out SoundSet soundSet) && soundSet.CanPlay()){
            PlayRandomClip(footstepSource, soundSet);
            soundSet.UpdatePlayTime();
        }
    }

// handles changing ambient sounds depending on the scene
    private void TransitionAmbientSound(string ambientType){
        if (isTransitioningAmbient) return;
        
        string key = $"ambient_{ambientType}";
        if (!soundSetLookup.TryGetValue(key, out SoundSet soundSet)) return;

        StartCoroutine(CrossfadeAmbientSound(soundSet));
    }

// smoothly changes between ambient sounds
    private System.Collections.IEnumerator CrossfadeAmbientSound(SoundSet newAmbientSet){
        isTransitioningAmbient = true;

        // set up the new ambient sound in the secondary source
        AudioClip newClip = newAmbientSet.clips[Random.Range(0, newAmbientSet.clips.Length)];
        secondaryAmbientSource.clip = newClip;
        secondaryAmbientSource.loop = true;
        secondaryAmbientSource.volume = 0f;
        secondaryAmbientSource.Play();

        // crossfade
        float timeElapsed = 0f;
        float startVolume = primaryAmbientSource.volume;
        float targetVolume = newAmbientSet.volume;

        while (timeElapsed < ambientFadeDuration){
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / ambientFadeDuration;

            primaryAmbientSource.volume = Mathf.Lerp(startVolume, 0f, t);
            secondaryAmbientSource.volume = Mathf.Lerp(0f, targetVolume, t);

            yield return null;
        }

        // stop the old ambient sound
        primaryAmbientSource.Stop();

        // swap the sources
        (primaryAmbientSource, secondaryAmbientSource) = (secondaryAmbientSource, primaryAmbientSource);

        isTransitioningAmbient = false;
    }


// plays a sound effect according to te effect type
    public void PlaySFX(string effectName){
        string key = $"sfx_{effectName}";
        if (soundSetLookup.TryGetValue(key, out SoundSet soundSet) && soundSet.CanPlay()){
            PlayRandomClip(sfxSource, soundSet);
            soundSet.UpdatePlayTime();
        }
    }

// plays a random clip from the available sounds
    private void PlayRandomClip(AudioSource source, SoundSet soundSet){
        if (soundSet.clips.Length > 0){
            AudioClip clip = soundSet.clips[Random.Range(0, soundSet.clips.Length)];
            source.PlayOneShot(clip, soundSet.volume);
        }
    }

// updates the volume based on the pause system settings
    public void UpdateMasterVolume(float volume){
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Max(0.0001f, volume)) * 20f);
    }


}
