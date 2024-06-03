using System.Collections.Generic;

/// <summary>
/// ECS����
/// </summary>
public class ECSScene : ECSEntity
{

    // ���ECSEntity���ֵ�
    private Dictionary<long, ECSEntity> entities;


    /// <summary>
    /// ���캯������ʼ���ֵ�
    /// </summary>
    public ECSScene()
    {
        entities = new Dictionary<long, ECSEntity>();
    }

    /// <summary>
    /// �̳���Entity����Ҫʵ��Dispose�ӿ�
    /// </summary>
    public override void Dispose()
    {
        if (Disposed)
            return;
        // �Ӽ��϶�����л�ȡһ������
        List<long> entityIDList = ListPool<long>.Obtain();
        // �����ֵ�ļ����������뼯��
        foreach (var entityID in entities.Keys)
        {
            entityIDList.Add(entityID);
        }
        // �������еļ�
        foreach (var entityID in entityIDList)
        {
            // ��ȡ��Ӧ��Entity
            ECSEntity entity = entities[entityID];
            // �����ͷ�
            entity.Dispose();
        }
        // ������Żض����
        ListPool<long>.Release(entityIDList);

        base.Dispose();
    }

    /// <summary>
    /// ���һ��ʵ��
    /// </summary>
    /// <param name="entity"></param>
    public void AddEntity(ECSEntity entity)
    {
        if (entity == null)
            return;

        ECSScene oldScene = entity.Scene;
        if (oldScene != null)
        {
            oldScene.RemoveEntity(entity.InstanceID);
        }

        entities.Add(entity.InstanceID, entity);
        entity.SceneID = InstanceID;
        UnityLog.Info($"Scene Add Entity, Current Count:{entities.Count}");
    }
    /// <summary>
    /// �Ƴ�һ��ʵ��
    /// </summary>
    /// <param name="entityID"></param>
    public void RemoveEntity(long entityID)
    {
        if (entities.Remove(entityID))
        {
            UnityLog.Info($"Scene Remove Entity, Current Count:{entities.Count}");
        }
    }

    /// <summary>
    /// �ҵ�ʵ��
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>

    public void FindEntities<T>(List<long> list) where T : ECSEntity
    {
        foreach (var item in entities)
        {
            if (item.Value is T)
            {
                list.Add(item.Key);
            }
        }
    }
    /// <summary>
    /// ���Ҵ��������ʵ��
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public void FindEntitiesWithComponent<T>(List<long> list) where T : ECSComponent
    {
        foreach (var item in entities)
        {
            if (item.Value.HasComponent<T>())
            {
                list.Add(item.Key);
            }
        }
    }
    /// <summary>
    /// ��ȡ���е�ʵ��
    /// </summary>
    /// <param name="list"></param>
    public void GetAllEntities(List<long> list)
    {
        foreach (var item in entities)
        {
            list.Add(item.Key);
        }
    }
}
