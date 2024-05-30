using System;
/// <summary>
/// �ʲ�����������
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
    // ������ɵ��¼�����
    public delegate void OnLoadFinishEventHandler(UnityEngine.Object asset);
    // ����ID
    private static int idGenerator;
    
    public int ID { get; set; }
    // ����״̬
    public TaskState State { get; internal set; }
    // �ʲ�����
    public Type AssetType { get; private set; }
    // �ʲ�����
    public AssetData Data { get; set; }
    // һ��ʱ�䴦�����͵��¼�
    public event OnLoadFinishEventHandler OnLoadFinish;
    // ���캯����ֵ
    public AssetLoadTask(AssetData data, Type assetType)
    {
        ID = ++idGenerator;
        Data = data;
        AssetType = assetType;
    }

    public void LoadFinish()
    {
        //Ulog.Log($"Load Asset Finish:{Data.Path}");
        // �������ִ��
        OnLoadFinish?.Invoke(Data.Asset);
    }
}

