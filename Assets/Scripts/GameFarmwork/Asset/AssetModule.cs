using UnityEngine;
using UnityEngine.AddressableAssets;

public partial class AssetModule : BaseGameModule
{
    // �༭��ģʽ��
#if UNITY_EDITOR
    //[XLua.BlackList]
    // ���ذ�������
    public const string BUNDLE_LOAD_NAME = "Tools/Build/Bundle Load";
#endif
    // ʹ���еĶ���ڵ�
    public Transform usingObjectRoot;
    // �ͷŵĶ���ڵ�
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
