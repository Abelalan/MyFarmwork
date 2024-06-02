using System;
using UnityEngine;


public partial class AssetModule : BaseGameModule
{
    // readonly 变量在初始化后不能被修改，这保证了它的不可变性（除了在构造函数中）。
    // 在这个类中，gameObjectPool 是一个私有字段，用于管理游戏对象的加载和缓存。
    private readonly GameObjectPool<GameObjectAsset> gameObjectPool = new GameObjectPool<GameObjectAsset>();

    /// <summary>
    /// 加载游戏对象
    /// </summary>
    /// <param name="path"></param>
    /// <param name="createNewCallback"></param>
    /// <returns></returns>
    public GameObject LoadGameObject(string path, Action<GameObject> createNewCallback = null)   
    {
        //UnityLog.Info($"Load GameObject:{path}");
        return gameObjectPool.LoadGameObject(path, createNewCallback).gameObject;
    }
    public T LoadGameObject<T>(string path, Action<GameObject> createNewCallback = null) where T : Component
    {
        //UnityLog.Info($"Load GameObject:{path}");
        GameObject go = gameObjectPool.LoadGameObject(path, createNewCallback).gameObject;
        return go.GetComponent<T>();
    }

    public void LoadGameObjectAsync(string path, Action<GameObjectAsset> callback, Action<GameObject> createNewCallback = null)
    {
        gameObjectPool.LoadGameObjectAsync(path, callback, createNewCallback);
    }

    public void UnloadCache()
    {
        gameObjectPool.UnloadAllGameObjects();
    }

    public void UnloadGameObject(GameObject go)
    {
        gameObjectPool.UnloadGameObject(go);
    }

    private void UpdateGameObjectRequests()
    {
        gameObjectPool.UpdateLoadRequests();
    }
}

