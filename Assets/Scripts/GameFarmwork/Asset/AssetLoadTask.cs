using System;
/// <summary>
/// 资产加载任务类
/// </summary>

public class AssetLoadTask
{
    public enum TaskState
    {
        Suspend = 0,
        Waiting = 1,
        Loading = 2,
        Finish = 3,
    }
    // 加载完成的事件处理
    public delegate void OnLoadFinishEventHandler(UnityEngine.Object asset);
    // 创建ID
    private static int idGenerator;
    
    public int ID { get; set; }
    // 人物状态
    public TaskState State { get; internal set; }
    // 资产类型
    public Type AssetType { get; private set; }
    // 资产数据
    public AssetData Data { get; set; }
    // 一个时间处理类型的事件
    public event OnLoadFinishEventHandler OnLoadFinish;
    // 构造函数赋值
    public AssetLoadTask(AssetData data, Type assetType)
    {
        ID = ++idGenerator;
        Data = data;
        AssetType = assetType;
    }

    public void LoadFinish()
    {
        //Ulog.Log($"Load Asset Finish:{Data.Path}");
        // 加载完成执行
        OnLoadFinish?.Invoke(Data.Asset);
    }
}

