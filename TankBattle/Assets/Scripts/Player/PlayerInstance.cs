using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayerInstance : MonoBehaviour
{
    public static PlayerInstance Instance;

    public PlayerData PlayerData;

    public Action<int> OnChangeMoney;

    public Action OnDataLoaded;

    public Action OnDataPartsUpdated;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }

        Instance = this;

        Load();
    }

    #region  Save and Load
    public void Save()
    {
        var path = Path.Combine(Application.persistentDataPath, "Saves", "PlayerSave.json");

        JsonDataLoader.SaveAsync(path, PlayerData).Forget();

    }

    public void Load()
    {
        LoadGameAsync().Forget();
    }

    private async UniTask<PlayerData> LoadGameAsync()
    {
        var path = Path.Combine(Application.persistentDataPath, "Saves", "PlayerSave.json");

        PlayerData = await JsonDataLoader.LoadAsync<PlayerData>(path);

        if (PlayerData == null)
        {
            PlayerData = new();
            PlayerData.MoneyCount = 100;
        }
        OnDataLoaded?.Invoke();

        return PlayerData;
    }
    #endregion

    public void ChangeMoney(int money)
    {
        PlayerData.MoneyCount += money;
        OnChangeMoney?.Invoke(PlayerData.MoneyCount);
    }

    public void UpdateData()
    {
        OnDataPartsUpdated?.Invoke();
    }
}
