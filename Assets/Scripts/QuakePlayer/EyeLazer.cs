using UnityEngine;

public class EyeLazer : MonoBehaviour
{
    private float damage = 50;
    private float timeBetweenShots = 5f;
    
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public float GetDamage() 
    {
        return damage;
    }

    public float GetTimeBetweenShots() 
    {
        return timeBetweenShots;
    }
}
