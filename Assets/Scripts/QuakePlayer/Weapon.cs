using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("WeaponStats")]
    public int damage;
    public float fireRate;
    [Space]
    public float radius;
    public float range;
    [Space]
    public ParticleSystem shootParticle;
    public ParticleSystem hitParticle;
    
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
