using Newtonsoft.Json;
using UnityEngine;
using Legends.Data;

namespace Legends.Services
{
    public class PlayerPrefsSaveService : ISaveService
    {
        const string SaveKey = "legends_save_v1";

        public void Save(GameState state)
        {
            string json = JsonConvert.SerializeObject(state);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
        }

        public GameState Load()
        {
            if (!PlayerPrefs.HasKey(SaveKey))
                return null;
            string json = PlayerPrefs.GetString(SaveKey);
            return JsonConvert.DeserializeObject<GameState>(json);
        }

        public void Delete()
        {
            PlayerPrefs.DeleteKey(SaveKey);
            PlayerPrefs.Save();
        }

        public bool Exists()
        {
            return PlayerPrefs.HasKey(SaveKey);
        }
    }
}
