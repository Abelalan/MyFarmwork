using System;
using System.Collections.Generic;

/// <summary>
/// ECS实体，实现了IDisposable接口，以便在不需要时进行资源的释放和清理
/// </summary>
public class ECSEntity : IDisposable
{
    // 实例ID，唯一标识实体
    public long InstanceID { get; private set; }
    // 父类ID，指向父实体
    public long ParentID { get; private set; }
    // 表示实体是否被销毁
    public bool Disposed { get; private set; }

    // 获取父实体
    public ECSEntity Parent
    {
        get
        {
            if (ParentID == 0)
                return default;

            // 找到父实体
            return TGameFramework.Instance.GetModule<ECSModule>().FindEntity(ParentID);
        }
    }
    // 场景ID指向所属场景
    public long SceneID { get; set; }
    
    // 获取场景
    public ECSScene Scene
    {
        get
        {
            if (SceneID == 0)
                return default;

            return TGameFramework.Instance.GetModule<ECSModule>().FindEntity(SceneID) as ECSScene;
        }
    }

    // 子实体列表
    private List<ECSEntity> children = new List<ECSEntity>();
    // 组件映射，键为组件类型，值为组件实例
    private Dictionary<Type, ECSComponent> componentMap = new Dictionary<Type, ECSComponent>();
    /// <summary>
    /// 构造函数
    /// </summary>
    public ECSEntity()
    {
        // 生成实例Id
        InstanceID = IDGenerator.NewInstanceID();
        // 将实体添加到 ECS 模块中
        TGameFramework.Instance.GetModule<ECSModule>().AddEntity(this);
    }
    /// <summary>
    /// 实现 IDisposable 接口，用于释放资源
    /// </summary>
    public virtual void Dispose()
    {
        if (Disposed)
            return;
        // 设置已释放标志
        Disposed = true;
        // 销毁Child
        for (int i = children.Count - 1; i >= 0; i--)
        {
            // 获取子实体
            ECSEntity child = children[i];
            // 从集合中移除
            children.RemoveAt(i);
            // 销毁
            child?.Dispose();
        }

        // 销毁Component
        List<ECSComponent> componentList = ListPool<ECSComponent>.Obtain();
        // 获取所有组件
        foreach (var component in componentMap.Values)
        {
            componentList.Add(component);
        }
        // 
        foreach (var component in componentList)
        {
            // 从字典中移除
            componentMap.Remove(component.GetType());
            // 销毁组件
            TGameFramework.Instance.GetModule<ECSModule>().DestroyComponent(component);
        }
        // 对象池回收
        ListPool<ECSComponent>.Release(componentList);

        // 从父节点移除
        Parent?.RemoveChild(this);
        // 从世界中移除
        TGameFramework.Instance.GetModule<ECSModule>().RemoveEntity(this);
    }
    /// <summary>
    /// 检查是否包含特定类型组件
    /// </summary>
    /// <typeparam name="C"></typeparam>
    /// <returns></returns>
    public bool HasComponent<C>() where C : ECSComponent
    {
        return componentMap.ContainsKey(typeof(C));
    }
    /// <summary>
    /// 获取特定组件
    /// </summary>
    /// <typeparam name="C"></typeparam>
    /// <returns></returns>
    public C GetComponent<C>() where C : ECSComponent
    {
        componentMap.TryGetValue(typeof(C), out var component);
        return component as C;
    }
    /// <summary>
    /// 添加一个新组件，如果已存在则先移除
    /// </summary>
    /// <typeparam name="C"></typeparam>
    /// <returns></returns>
    public C AddNewComponent<C>() where C : ECSComponent, new()
    {
        // 如果已包含该类型的组件，先移除
        if (HasComponent<C>())
        {
            RemoveComponent<C>();
        }

        // 创建新的组件实例
        C component = new C();
        component.EntityID = InstanceID;
        // 添加到组件映射字典中
        componentMap.Add(typeof(C), component);
        // 调用 ECS 模块的 AwakeComponent 方法初始化组件
        TGameFramework.Instance.GetModule<ECSModule>().AwakeComponent(component);
        return component;
    }
    /// <summary>
    /// 添加一个带有一个参数的新组件，如果已经存在则先移除
    /// </summary>
    /// <typeparam name="C"></typeparam>
    /// <typeparam name="P1"></typeparam>
    /// <param name="p1"></param>
    /// <returns></returns>
    public C AddNewComponent<C, P1>(P1 p1) where C : ECSComponent, new()
    {
        if (HasComponent<C>())
        {
            RemoveComponent<C>();
        }

        C component = new C();
        component.EntityID = InstanceID;
        componentMap.Add(typeof(C), component);
        TGameFramework.Instance.GetModule<ECSModule>().AwakeComponent(component, p1);
        return component;
    }
    /// <summary>
    /// 添加一个带有两个参数的新组件，如果已经存在则先移除
    /// </summary>
    /// <typeparam name="C"></typeparam>
    /// <typeparam name="P1"></typeparam>
    /// <typeparam name="P2"></typeparam>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <returns></returns>
    public C AddNewComponent<C, P1, P2>(P1 p1, P2 p2) where C : ECSComponent, new()
    {
        if (HasComponent<C>())
        {
            RemoveComponent<C>();
        }

        C component = new C();
        component.EntityID = InstanceID;
        componentMap.Add(typeof(C), component);
        TGameFramework.Instance.GetModule<ECSModule>().AwakeComponent(component, p1, p2);
        return component;
    }
    /// <summary>
    /// 添加一个组件，如果已经存在则报错并返回默认值
    /// </summary>
    /// <typeparam name="C"></typeparam>
    /// <returns></returns>
    public C AddComponent<C>() where C : ECSComponent, new()
    {
        if (HasComponent<C>())
        {
            // 已存在
            UnityLog.Error($"Duplicated Component:{typeof(C).FullName}");
            return default;
        }

        C component = new C();
        component.EntityID = InstanceID;
        componentMap.Add(typeof(C), component);
        TGameFramework.Instance.GetModule<ECSModule>().AwakeComponent(component);
        return component;
    }

    public C AddComponent<C, P1>(P1 p1) where C : ECSComponent, new()
    {
        if (HasComponent<C>())
        {
            UnityLog.Error($"Duplicated Component:{typeof(C).FullName}");
            return default;
        }

        C component = new C();
        component.EntityID = InstanceID;
        componentMap.Add(typeof(C), component);
        TGameFramework.Instance.GetModule<ECSModule>().AwakeComponent(component, p1);
        return component;
    }

    public C AddComponent<C, P1, P2>(P1 p1, P2 p2) where C : ECSComponent, new()
    {
        if (HasComponent<C>())
        {
            UnityLog.Error($"Duplicated Component:{typeof(C).FullName}");
            return default;
        }

        C component = new C();
        component.EntityID = InstanceID;
        componentMap.Add(typeof(C), component);
        TGameFramework.Instance.GetModule<ECSModule>().AwakeComponent(component, p1, p2);
        return component;
    }

    public void RemoveComponent<C>() where C : ECSComponent, new()
    {
        Type componentType = typeof(C);
        if (!componentMap.TryGetValue(componentType, out var component))
            return;

        componentMap.Remove(componentType);
        TGameFramework.Instance.GetModule<ECSModule>().DestroyComponent((C)component);
    }

    public void RemoveComponent<C, P1>(P1 p1) where C : ECSComponent, new()
    {
        Type componentType = typeof(C);
        if (!componentMap.TryGetValue(componentType, out var component))
            return;

        componentMap.Remove(componentType);
        TGameFramework.Instance.GetModule<ECSModule>().DestroyComponent((C)component, p1);
    }

    public void RemoveComponent<C, P1, P2>(P1 p1, P2 p2) where C : ECSComponent, new()
    {
        Type componentType = typeof(C);
        if (!componentMap.TryGetValue(componentType, out var component))
            return;

        componentMap.Remove(componentType);
        TGameFramework.Instance.GetModule<ECSModule>().DestroyComponent((C)component, p1, p2);
    }
    /// <summary>
    /// 添加子实体
    /// </summary>
    /// <param name="child"></param>
    public void AddChild(ECSEntity child)
    {
        if (child == null)
            return;

        if (child.Disposed)
            return;
        // 获取父实体
        ECSEntity oldParent = child.Parent;
        if (oldParent != null)
        {
            // 解除父子关系
            oldParent.RemoveChild(child);
        }
        // 添加父子关系
        children.Add(child);
        // 改变父实体ID
        child.ParentID = InstanceID;
    }

    public void RemoveChild(ECSEntity child)
    {
        if (child == null)
            return;

        children.Remove(child);
        child.ParentID = 0;
    }

    public T FindChild<T>(long id) where T : ECSEntity
    {
        foreach (var child in children)
        {
            if (child.InstanceID == id)
                return child as T;
        }

        return default;
    }

    public T FindChild<T>(Predicate<T> predicate) where T : ECSEntity
    {
        foreach (var child in children)
        {
            T c = child as T;
            if (c == null)
                continue;

            if (predicate.Invoke(c))
            {
                return c;
            }
        }

        return default;
    }

    public void FindChildren<T>(List<T> list) where T : ECSEntity
    {
        foreach (var child in children)
        {
            if (child is T)
            {
                list.Add(child as T);
            }
        }
    }
}

