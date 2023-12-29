using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ConfigRegistry", menuName = "Config/ConfigRegistry")]
public class ConfigRegistry : SerializedScriptableObject
{
    [SerializeField, FolderPath] private string configPath;
    [SerializeField] private ConfigData registry;

    public bool TryGetConfig<T>(out T config) where T : class, IConfig
    {
        if (registry.TryGetValue(typeof(T).Name, out ScriptableObject asset))
        {
            config = asset as T;
            return true;
        }

        config = null;
        return false;
    }

    [System.Serializable]
    public class ConfigData : UnitySerializedDictionary<string, ScriptableObject> { }

    [Button("Refresh")]
    public void Refresh()
    {
#if UNITY_EDITOR
        if (registry == null)
        {
            registry = new ConfigData();
        }

        var types = UnityEditor.TypeCache.GetTypesDerivedFrom<IConfig>().Where(t => !t.IsGenericType);
        foreach (var item in types)
        {
            ScriptableObject asset = UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableObject>($"{configPath}/{item.Name}.asset");
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance(item);
                UnityEditor.AssetDatabase.CreateAsset(asset, $"{configPath}/{item.Name}.asset");
                registry.Add(item.Name, asset);
            }
            else
            {
                registry[item.Name] = asset;
            }
        }

        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }
}

public interface IConfig
{
    string ID { get; }
}

public interface IConfigData
{
    string ID { get; }
}

public abstract class BaseConfig<U> : SerializedScriptableObject, IConfig where U : IConfig
{
    public string ID => nameof(U);
}

public abstract class BaseSingleConfig<T, U> : BaseConfig<U> where T : class, IConfigData, new() where U : IConfig
{
    [SerializeField, InlineProperty, HideLabel] private T data = new();
    public T Data => data;
}

public abstract class BaseMultiConfig<T, U> : BaseConfig<U> where T : IConfigData where U : IConfig
{
    [SerializeField] private MultiConfigData data;
    public MultiConfigData Data => data;

    [System.Serializable]
    public class MultiConfigData : UnitySerializedDictionary<string, T> { }
}
