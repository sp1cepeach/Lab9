using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour, ISaveable
{
    [SerializeField] float thrust;
    [SerializeField] Bullet bulletPrefab;
    [SerializeField] int maxBullets;
    [SerializeField] float reloadTime;
    [SerializeField] int startHealth;
    int health;
    new Rigidbody2D rigidbody;
    BulletSpawner bulletSpawner;
    float timeSinceLastShot;

    void Start()
    {
        // Health
        health = startHealth;
        AlienSpawner alienSpawner = FindAnyObjectByType<AlienSpawner>();
        alienSpawner.onAlienSpawn += (alien) =>
        {
            alien.OnAlienReachPlayer += () => TakeDamage(alien.damage);
        };

        // Movement
        rigidbody = GetComponent<Rigidbody2D>();

        // Shooting
        bulletSpawner = new BulletSpawner(maxBullets, () => Instantiate(bulletPrefab));
        timeSinceLastShot = reloadTime;
    }

    void Update()
    {
        // Movement
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector2 impulse = Vector2.right * thrust * horizontalInput;
        rigidbody.AddRelativeForce(impulse, ForceMode2D.Impulse);

        // Shooting
        timeSinceLastShot += Time.deltaTime;
        bool isShootPressed = Input.GetButtonDown("Fire1");
        bool isReloading = (timeSinceLastShot < reloadTime);
        if (isShootPressed && !isReloading)
        {
            bulletSpawner.SpawnBullet(transform);
            timeSinceLastShot = 0;
        }
    }

    public void TakeDamage(int damageAmount)
    {
        health = Mathf.Max(0, health - damageAmount);

        if (health <= 0)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        Debug.Log("Score = " + FindAnyObjectByType<Score>().score);
        Debug.Log("Game Over");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    private void OnGUI()
    {
        string hearts = "";
        for (int i = 0; i < health; ++i)
        {
            hearts += "❤ ";
        }
        GUI.Label(new Rect(10, 30, 100, 20), hearts);
    }

    void ISaveable.Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(Application.dataPath + "/SaveData/" + name + ".binary", FileMode.Create);
        TransformSaver data = new TransformSaver
        {
            health = this.health,
            x = this.transform.position.x
        };

        formatter.Serialize(stream, data);
        stream.Close();
    }

    void ISaveable.Load()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(Application.dataPath + "/SaveData/" + name + ".binary", FileMode.Open);
        TransformSaver data = formatter.Deserialize(stream) as TransformSaver;
        stream.Close();

        this.health = data.health;
        this.rigidbody.MovePosition(
            transform.position + Vector3.right * (data.x - transform.position.x)
        );
    }

    [System.Serializable]
    private class TransformSaver
    {
        public int health;
        public float x;
    }
}
