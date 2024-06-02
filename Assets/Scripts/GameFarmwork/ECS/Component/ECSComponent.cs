/// <summary>
/// 抽象类ECSComponent，表示ECS中的组件
/// </summary>
public abstract class ECSComponent
{
    // 每个组件都有一个唯一的ID。只有Get访问器，表示组件的唯一标识
    public long ID { get; private set; }

    // // 组件所属实体的 ID，可读可写，表示与该组件关联的实体
    public long EntityID { get; set; }

    // 组件是否已被释放的标志，可读可写，表示组件是否被销毁
    public bool Disposed { get; set; }

    // 通过EntityID 获取与该组件关联的实体
    public ECSEntity Entity
    {
        get
        {
            // 如果EntityID == 0，返回默认值null;
            if (EntityID == 0)
                return default;

            // 通过ECSModule快速查找并返回对应的实体
            return TGameFramework.Instance.GetModule<ECSModule>().FindEntity(EntityID);
        }
    }
    /// <summary>
    /// ECSComponent 的构造函数，用于初始化组件
    /// </summary>
    public ECSComponent()
    {
        // 使用IDGenerator生成一个新的ID
        ID = IDGenerator.NewInstanceID();
        // 未销毁
        Disposed = false;
    }
}

