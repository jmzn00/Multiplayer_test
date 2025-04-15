using UnityEngine;

public class AudioScript : MonoBehaviour
{

    [SerializeField] private PlayerControllerQuake playerController;
    [SerializeField] private AudioSource movementSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayMovementSound();
    }

    private void PlayMovementSound() 
    {
        if (playerController.GetMagnitude() > 2.5f)
        {
            if (!movementSound.isPlaying)
            {
                //movementSound.Play();
            }
        }
        else
        {
            if (movementSound.isPlaying)
            {
                //movementSound.Stop();               
            }
        }
    }
    
}
