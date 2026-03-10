using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;   

public class GunSjhoot : MonoBehaviour
{
    // public SoundPlayer SoundPlayer;
    public animator animator;
    public Transform BulletSpawnPoint;
    public GameObject BulletPrefab;
    public float BulletSpeed = 30.0f;
    public TextMeshProUGUI enemyText;
    public int enemyDestroyed = 0;
    public float firingDelay = 0f;
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            animator.GetComponent
            var bullet = Instantiate(BulletPrefab, BulletSpawnPoint.position, BulletSpawnPoint.rotation); // makes a new bullet
            bullet.SetActive(true); // inactive bullet made active.
            bullet.GetComponent<Rigidbody>().velocity = BulletSpawnPoint.forward * BulletSpeed;
            Debug.Log("we are shooting");
            
        }
    }
}