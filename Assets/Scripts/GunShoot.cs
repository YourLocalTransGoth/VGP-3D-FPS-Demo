using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShoot : MonoBehaviour
{
    public static int enemyDestroyed = 0;

    public Transform BulletSpawnPoint;
    public GameObject BulletPrefab;
    public AudioPlayer audioPlayer;

    public float BulletSpeed = 30.0f;

    private void Start()
    {
        if (audioPlayer == null)
        {
            audioPlayer = FindObjectOfType<AudioPlayer>();
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        if (GameStateController.IsGamePaused)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (audioPlayer != null)
            {
                audioPlayer.PlayGunReload();
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (BulletPrefab == null || BulletSpawnPoint == null)
            {
                return;
            }

            Vector3 spawnPosition = BulletSpawnPoint.position + BulletSpawnPoint.forward * 0.2f;
            var bullet = Instantiate(BulletPrefab, spawnPosition, BulletSpawnPoint.rotation);
            bullet.SetActive(true);

            Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
            if (bulletRigidbody != null)
            {
                bulletRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                bulletRigidbody.velocity = BulletSpawnPoint.forward * BulletSpeed;
            }

            Collider bulletCollider = bullet.GetComponent<Collider>();
            Transform shooterRoot = BulletSpawnPoint.root;
            if (bulletCollider != null && shooterRoot != null)
            {
                Collider[] shooterColliders = shooterRoot.GetComponentsInChildren<Collider>();
                foreach (Collider shooterCollider in shooterColliders)
                {
                    if (shooterCollider != null && shooterCollider != bulletCollider)
                    {
                        Physics.IgnoreCollision(bulletCollider, shooterCollider);
                    }
                }
            }

            if (audioPlayer != null)
            {
                audioPlayer.PlayGunFire();
            }
        }
    }
}