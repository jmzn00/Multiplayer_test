using Photon.Pun;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerPhotonSoundManager : MonoBehaviourPun
{
    public AudioClip[] audioClips;
    public AudioMixerGroup mixerGroup;

    [PunRPC]
    public void PlaySoundAtPosition(int clipIndex, Vector3 position)
    {
        GameObject soundGo = new GameObject("SoundEmitter");
        soundGo.transform.position = position;
        AudioSource source = soundGo.AddComponent<AudioSource>();
        source.outputAudioMixerGroup = mixerGroup;
        source.clip = audioClips[clipIndex];
        source.spatialBlend = 1f;
        source.minDistance = 1f;
        source.maxDistance = 25f;
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.Play();


        Destroy(soundGo, source.clip.length);
    }
}
