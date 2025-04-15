using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using static Cinemachine.DocumentationSortingAttribute;

public class MainMenuMusic : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource mainMenuMusic;

    private void Start()
    {
        audioMixer.SetFloat("MasterVolume", -10f);
        mainMenuMusic.Play();
        
    }

    public void MusicLevel(float level) 
    {
        audioMixer.SetFloat("MusicVolume", level);
    }
    public void MusicPitch(float pitch) 
    {
        audioMixer.SetFloat("MusicPitch", pitch);
    }

    public void MasterLevel(float level) 
    {
        audioMixer.SetFloat("MasterVolume", level);
    }


}
