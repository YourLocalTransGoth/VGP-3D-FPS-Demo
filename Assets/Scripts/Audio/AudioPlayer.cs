using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip gunFire;
    [SerializeField] private AudioClip gunReload;
    [SerializeField] private AudioClip enemyDyingSound;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void PlayGunFire()
    {
        PlayClip(gunFire);
    }

    public void PlayGunReload()
    {
        PlayClip(gunReload);
    }

    public void PlayEnemyDyingSound()
    {
        PlayClip(enemyDyingSound);
    }

    private void PlayClip(AudioClip clip)
    {
        if (audioSource == null || clip == null)
        {
            return;
        }

        audioSource.clip = clip;
        audioSource.Play();
    }

}