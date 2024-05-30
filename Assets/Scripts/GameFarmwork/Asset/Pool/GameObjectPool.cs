using System;
using System.Collections.Generic;
using TGame.Asset;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// 游戏对象对象池
/// 管理GameObject对象的加载、异步加载、卸载和缓存
/// 提高资源管理的效率，减少频繁创建和销毁对象的开销
/// </summary>
/// <typeparam name="T"></typeparam>
public class GameObjectPool<T> where T : GameObjectPoolAsset
{
    // 键为资源路径的哈希值，值为对应的GameObject队列
    private readonly Dictionary<int, Queue<T>> gameObjectPool = new Dictionary<int, Queue<T>>();
    // 存储异步加载的请求
    private readonly List<GameObjectLoadRequest<T>> requests = new List<GameObjectLoadRequest<T>>();
    // 键为 GameObject 实例的 ID，值为正在使用的 GameObject 对象。
    private readonly Dictionary<int, GameObject> usingObjects = new Dictionary<int, GameObject>();
    /// <summary>
    ///  同步加载 GameObject 对象。如果对象池中有可用的对象，则直接使用，否则从资源路径加载新的对象。
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <param name="createNewCallback">回调函数，在创建新对象时调用。</param>
    /// <returns></returns>
    public T LoadGameObject(string path, Action<GameObject> createNewCallback = null)
    {
        // 计算路径的哈希值
        int hash = path.GetHashCode();
        // 检查是否能从对象池中获取队列
        if (!gameObjectPool.TryGetValue(hash, out Queue<T> q))
        {
            // 创建一个新队列
            q = new Queue<T>();
            // 添加进字典
            gameObjectPool.Add(hash, q);
        }
        // 如果队列为空
        if (q.Count == 0)
        {
            // 从资源路径加载对象
            GameObject prefab = Addressables.LoadAssetAsync<GameObject>(path).WaitForCompletion();
            // 实例化一个对象
            GameObject go = UnityEngine.Object.Instantiate(prefab);
            // 给对象添加组件
            T asset = go.AddComponent<T>();
            // 执行回调方法
            createNewCallback?.Invoke(go);
            // 设置ID
            asset.ID = hash;
            // 激活对象
            go.SetActive(false);
            // 将对象放进队列
            q.Enqueue(asset);
        }

        {
            T asset = q.Dequeue();
            OnGameObjectLoaded(asset);
            return asset;
        }
    }

    /// <summary>
    /// 异步加载请求
    /// </summary>
    /// <param name="path">需要加载的资源的路径</param>
    /// <param name="callback">每次调用LoadGameObjectAsync，无论是否从缓存里取出的，都会通过这个回调进行通知</param>
    /// <param name="createNewCallback">游戏对象第一次被克隆后调用，对象池取出的复用游戏对象，不会回调</param>
    public void LoadGameObjectAsync(string path, Action<T> callback, Action<GameObject> createNewCallback = null)
    {
        // new 一个新对象
        GameObjectLoadRequest<T> request = new GameObjectLoadRequest<T>(path, callback, createNewCallback);
        // 加入集合
        requests.Add(request);
    }
    /// <summary>
    /// 卸载所有缓存和正在使用的 GameObject 对象。
    /// </summary>
    public void UnloadAllGameObjects()
    {
        // 先将所有Request加载完毕
        while (requests.Count > 0)
        {
            //GameManager.Asset.UpdateLoader();
            // 
            UpdateLoadRequests();
        }

        // 将所有using Objects 卸载
        if (usingObjects.Count > 0)
        {
            List<int> list = new List<int>();
            foreach (var id in usingObjects.Keys)
            {
                list.Add(id);
            }
            foreach (var id in list)
            {
                GameObject obj = usingObjects[id];
                UnloadGameObject(obj);
            }
        }

        // 将所有缓存清掉
        if (gameObjectPool.Count > 0)
        {
            foreach (var q in gameObjectPool.Values)
            {
                foreach (var asset in q)
                {
                    UnityEngine.Object.Destroy(asset.gameObject);
                }
                q.Clear();
            }
            gameObjectPool.Clear();
        }
    }
    /// <summary>
    /// 将对象返回池中，并设置为不活跃状态。
    /// </summary>
    /// <param name="go"></param>
    public void UnloadGameObject(GameObject go)
    {
        if (go == null)
            return;

        T asset = go.GetComponent<T>();
        if (asset == null)
        {
            UnityLog.Warn($"Unload GameObject失败，找不到GameObjectAsset:{go.name}");
            UnityEngine.Object.Destroy(go);
            return;
        }

        if (!gameObjectPool.TryGetValue(asset.ID, out Queue<T> q))
        {
            q = new Queue<T>();
            gameObjectPool.Add(asset.ID, q);
        }

        q.Enqueue(asset);
        // 从使用字典中移除
        usingObjects.Remove(go.GetInstanceID());
        // 父对象设置为回收根目录
        go.transform.SetParent(TGameFramework.Instance.GetModule<AssetModule>().releaseObjectRoot);
        // 设置为失活
        go.gameObject.SetActive(false);
    }
    /// <summary>
    /// 更新加载请求，处理异步加载逻辑。
    /// </summary>
    public void UpdateLoadRequests()
    {
        // 处理所有异步加载请求。
        if (requests.Count > 0)
        {
            // 遍历所有的请求
            foreach (var request in requests)
            {
                // 获取路径哈希值
                int hash = request.Path.GetHashCode();
                // 检查是否能从对象池中获取队列
                if (!gameObjectPool.TryGetValue(hash, out Queue<T> q))
                {
                    // 创建一个新队列
                    q = new Queue<T>();
                    // 添加进字典
                    gameObjectPool.Add(hash, q);
                }
                // 没有可用对象时，异步加载并缓存新对象
                if (q.Count == 0)
                {
                    Addressables.LoadAssetAsync<GameObject>(request.Path).Completed += (obj) =>
                    {
                        // 实例化对象
                        GameObject go = UnityEngine.Object.Instantiate(obj.Result);
                        // 添加组件
                        T asset = go.AddComponent<T>();
                        // 回调函数
                        request.CreateNewCallback?.Invoke(go);
                        // 设置ID
                        asset.ID = hash;

                        go.SetActive(false);
                        // 处理加载之后的逻辑
                        OnGameObjectLoaded(asset);
                        // 加载完成
                        request.LoadFinish(asset);
                    };
                }
                else
                {
                    T asset = q.Dequeue();
                    OnGameObjectLoaded(asset);
                    // 加载完成
                    request.LoadFinish(asset);
                }
            }

            requests.Clear();
        }
    }
    /// <summary>
    /// 处理 GameObject 对象加载完成后的逻辑。
    /// </summary>
    /// <param name="asset"></param>
    private void OnGameObjectLoaded(T asset)
    {
        // 设置父对象
        asset.transform.SetParent(TGameFramework.Instance.GetModule<AssetModule>().usingObjectRoot);
        // 获取对象实例的唯一ID
        int id = asset.gameObject.GetInstanceID();
        // 添加到正在使用 的字典中
        usingObjects.Add(id, asset.gameObject);
    }
}

