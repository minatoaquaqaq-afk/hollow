using System.IO;
using UnityEngine;

namespace HollowStyleMVP.Save
{
    public static class SaveSystem
    {
        private static string Path => System.IO.Path.Combine(Application.persistentDataPath, "save.json");
        public static bool HasSave() => File.Exists(Path);
        public static void Save(SaveData data) => File.WriteAllText(Path, JsonUtility.ToJson(data, true));
        public static SaveData Load() => HasSave() ? JsonUtility.FromJson<SaveData>(File.ReadAllText(Path)) : new SaveData();
        public static void DeleteSave() { if (HasSave()) File.Delete(Path); }
    }
}
