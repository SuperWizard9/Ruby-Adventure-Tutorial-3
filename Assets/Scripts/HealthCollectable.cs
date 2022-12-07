using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCollectable : MonoBehaviour
{
    public AudioClip collectedClip;
    public GameObject pickupParticlePrefab;

    void OnTriggerEnter2D(Collider2D other)
    {
    RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            if(controller.health < controller.maxHealth)
            {
                controller.ChangeHealth(1);
                GameObject pickupParticleObject= Instantiate(pickupParticlePrefab, transform.position, Quaternion.identity);
                pickupParticleObject.GetComponent<ParticleSystem>().Play();
                Destroy(gameObject);
            }
        }

    }
}