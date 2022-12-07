using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoCollectible : MonoBehaviour
{
  public AudioClip SourceCollectable;  

    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            if(controller.ammo  <= controller.currentAmmo)
            {
                controller.ChangeAmmo(4);
                controller.AmmoText();
                controller.DamageAudio.clip=SourceCollectable;
                controller.DamageAudio.Play();
                Destroy(gameObject);

                
            }
        }
    }
}