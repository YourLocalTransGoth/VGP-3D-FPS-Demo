using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip gunFireSound;

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void PlayGunFire()
    {
        if (audioSource != null && gunFireSound != null)
        {
            audioSource.PlayOneShot(gunFireSound);
        }
    }
}
