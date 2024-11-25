using System.IO;
using UnityEngine;

public class Score : MonoBehaviour, ISaveable
{
    public int score { get { return _score; } }
    [SerializeField] private int _score;

    void Start()
    {
        _score = 0;
        AlienSpawner alienSpawner = FindAnyObjectByType<AlienSpawner>();
        alienSpawner.onAlienSpawn += (alien) =>
        {
            alien.OnAlienHit += () => IncreaseScore(alien.score);
        };
    }

    public void IncreaseScore(int amount)
    {
        _score += amount;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), "Score: " + score);
    }

    void ISaveable.Save()
    {
        string json = JsonUtility.ToJson(this);
        File.WriteAllText(Application.dataPath + "/SaveData/" + name + ".json", json);
    }

    void ISaveable.Load()
    {
        string json = File.ReadAllText(Application.dataPath + "/SaveData/" + name + ".json");
        JsonUtility.FromJsonOverwrite(json, this);
    }
}
