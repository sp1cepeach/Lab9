using System.Linq;
using UnityEngine;

public class SavingService : MonoBehaviour
{
    private void Update()
    {
        bool isSavePressed = Input.GetButtonDown("Save");
        bool isLoadPressed = Input.GetButtonDown("Load");

        if (isSavePressed)
        {
            SaveGame();
        }
        else if (isLoadPressed)
        {
            LoadGame();
        }
    }

    public static void SaveGame()
    {
        foreach (ISaveable obj in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ISaveable>())
        {
            obj.Save();
        }

        Debug.Log("Saved Game");
    }

    public static void LoadGame()
    {
        foreach (ISaveable obj in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ISaveable>())
        {
            obj.Load();
        }

        Debug.Log("Loaded Game");
    }
}

public interface ISaveable
{
    void Save();
    void Load();
}
