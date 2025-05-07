using UnityEngine;

public class DoorActivate : MonoBehaviour
{
    public bool isActivated = false;
    private bool startTimer = false;
    public float cooldown;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            isActivated = true;            
        }        
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            startTimer = true;
            timer = 0f;
        }
    }

    private float timer;
    private void Update()
    {
        Debug.Log(isActivated);

        if (startTimer) 
        {            
            timer += Time.deltaTime;
            if(timer >= cooldown) 
            {
                isActivated = false;
                startTimer = false;
            }
        }
    }
}
