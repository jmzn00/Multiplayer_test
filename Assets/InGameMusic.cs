using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class InGameMusic : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioClip[] backgroundMusic;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private TextMeshProUGUI currentTrackText;

    private int currentTrack = 0;
    private void Awake()
    {
        PlayNextTrack();
    }

    public void MasterLevel(float level) 
    {
        audioMixer.SetFloat("MasterVolume", level);
    }
    public void MusicLevel(float level) 
    {
        audioMixer.SetFloat("MusicVolume", level);
    }

    public void MusicPitch(float pitch) 
    {
        audioMixer.SetFloat("MusicPitch", pitch);
    }

    private void Update()
    {
        if (!audioSource.isPlaying) 
        {
            PlayNextTrack();
        }
        
    }
    
    public void PlayNextTrack() 
    {
        if(currentTrack >= backgroundMusic.Length) 
        {
            currentTrack = 0;
            return;
        }

        audioSource.clip = backgroundMusic[currentTrack];
        currentTrackText.text = "Current Track: " + audioSource.clip.name;
        audioSource.Play();
        currentTrack++;        
        
    }
}

 
