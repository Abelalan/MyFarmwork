using System.Collections.Generic;

/// <summary>
/// ECS场景
/// </summary>
public class ECSScene : ECSEntity
{

    // 存放ECSEntity的字典
    private Dictionary<long, ECSEntity> entities;


    /// <summary>
    /// 构造函数，初始化字典
    /// </summary>
    public ECSScene()
    {
        entities = new Dictionary<long, ECSEntity>();
    }

    /// <summary>
    /// 继承了Entity，需要实现Dispose接口
    /// </summary>
    public override void Dispose()
    {
        if (Disposed)
            return;
        // 从集合对象池中获取一个集合
        List<long> entityIDList = ListPool<long>.Obtain();
        // 遍历字典的键，将键加入集合
        foreach (var entityID in entities.Keys)
        {
            entityIDList.Add(entityID);
        }
        // 遍历所有的键
        foreach (var entityID in entityIDList)
        {
            // 获取对应的Entity
            ECSEntity entity = entities[entityID];
            // 进行释放
            entity.Dispose();
        }
        // 将对象放回对象池
        ListPool<long>.Release(entityIDList);

        base.Dispose();
    }

    /// <summary>
    /// 添加一个实体
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
    /// 移除一个实体
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
    /// 找到实体
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
    /// 查找带有组件的实体
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
    /// 获取所有的实体
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
