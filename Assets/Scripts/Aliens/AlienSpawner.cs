using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class AlienSpawner : MonoBehaviour, ISaveable
{
    public Action<Alien> onAlienSpawn;
    [SerializeField] Alien alienPrefab;
    float delay = 2;

    void Start()
    {
        Invoke("SpawnAlien", 1);
    }

    void SpawnAlien()
    {
        AlienBuilder alienBuilder = null;
        Func<Alien> instantiateAlien = () =>
            Instantiate(alienPrefab, transform.position, transform.rotation);

        switch (UnityEngine.Random.Range(0, 3))
        {
            case 0:
                alienBuilder = new GreenAlienBuilder(instantiateAlien);
                break;
            case 1:
                alienBuilder = new YellowAlienBuilder(instantiateAlien);
                break;
            case 2:
                alienBuilder = new PinkAlienBuilder(instantiateAlien);
                break;
        }

        alienBuilder.BuildSprite();
        alienBuilder.BuildSpeed();
        alienBuilder.BuildScore();
        alienBuilder.BuildDamage();

        onAlienSpawn?.Invoke(alienBuilder.alien);

        Invoke("SpawnAlien", delay);
        delay = 1 + (0.9f * (delay - 1));
    }

    void ISaveable.Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(Application.dataPath + "/SaveData/" + name + ".binary", FileMode.Create);
        Alien[] aliens = FindObjectsByType<Alien>(FindObjectsSortMode.None);
        float[] xPositions = new float[aliens.Length];
        float[] yPositions = new float[aliens.Length];
        float[] velocities = new float[aliens.Length];
        int[] types = new int[aliens.Length];

        for (int i = 0; i < aliens.Length; i++)
        {
            xPositions[i] = aliens[i].transform.position.x;
            yPositions[i] = aliens[i].transform.position.y;
            velocities[i] = aliens[i].rigidbody.velocity.x;
            types[i] = aliens[i].damage;
        }

        TransformSaver data = new TransformSaver
        {
            xPositions = xPositions,
            yPositions = yPositions,
            velocities = velocities,
            types = types,
        };

        formatter.Serialize(stream, data);
        stream.Close();
    }

    void ISaveable.Load()
    {
        foreach (Alien alien in FindObjectsByType<Alien>(FindObjectsSortMode.None))
        {
            Destroy(alien.gameObject);
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(Application.dataPath + "/SaveData/" + name + ".binary", FileMode.Open);
        TransformSaver data = formatter.Deserialize(stream) as TransformSaver;
        stream.Close();

        for (int i = 0; i < data.types.Length; i++)
        {
            AlienBuilder alienBuilder = null;
            Func<Alien> instantiateAlien = () =>
                Instantiate(alienPrefab, new Vector2(data.xPositions[i], data.yPositions[i]), transform.rotation);
            switch (data.types[i])
            {
                case 1:
                    alienBuilder = new GreenAlienBuilder(instantiateAlien);
                    break;
                case 2:
                    alienBuilder = new YellowAlienBuilder(instantiateAlien);
                    break;
                case 3:
                    alienBuilder = new PinkAlienBuilder(instantiateAlien);
                    break;
            }

            alienBuilder.BuildSprite();
            alienBuilder.BuildSpeed();
            alienBuilder.BuildScore();
            alienBuilder.BuildDamage();
            alienBuilder.alien.rigidbody.velocity = new Vector2(data.velocities[i], 0);

            onAlienSpawn?.Invoke(alienBuilder.alien);
        }
    }

    [System.Serializable]
    private class TransformSaver
    {
        public float[] xPositions;
        public float[] yPositions;
        public float[] velocities;
        public int[] types;
    }
}
