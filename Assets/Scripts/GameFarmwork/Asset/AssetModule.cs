using UnityEngine;
using UnityEngine.AddressableAssets;

public partial class AssetModule : BaseGameModule
{
    // 编辑器模式下
#if UNITY_EDITOR
    //[XLua.BlackList]
    // 加载包的名称
    public const string BUNDLE_LOAD_NAME = "Tools/Build/Bundle Load";
#endif
    // 使用中的对象节点
    public Transform usingObjectRoot;
    // 释放的对象节点
    public Transform releaseObjectRoot;

    protected internal override void OnModuleUpdate(float deltaTime)
    {
        base.OnModuleUpdate(deltaTime);
        UpdateGameObjectRequests();
    }

    public T LoadAsset<T>(string path) where T : Object
    {
        return Addressables.LoadAssetAsync<T>(path).WaitForCompletion();
    }

    public void LoadObjectAsync<T>(string path, AssetLoadTask.OnLoadFinishEventHandler onLoadFinish) where T : UnityEngine.Object
    {
        Addressables.LoadAssetAsync<T>(path).Completed += (obj) =>
        {
            onLoadFinish?.Invoke(obj.Result);
        };
    }
}
