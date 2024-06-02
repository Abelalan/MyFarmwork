using System;
using UnityEngine;


public partial class AssetModule : BaseGameModule
{
    // readonly �����ڳ�ʼ�����ܱ��޸ģ��Ᵽ֤�����Ĳ��ɱ��ԣ������ڹ��캯���У���
    // ��������У�gameObjectPool ��һ��˽���ֶΣ����ڹ�����Ϸ����ļ��غͻ��档
    private readonly GameObjectPool<GameObjectAsset> gameObjectPool = new GameObjectPool<GameObjectAsset>();

    /// <summary>
    /// ������Ϸ����
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

