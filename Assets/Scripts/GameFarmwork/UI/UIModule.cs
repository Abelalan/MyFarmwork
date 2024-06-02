using Config;
using QFSW.QC;
using System;
using System.Collections;
using System.Collections.Generic;
using TGame.Asset;
using UnityEngine;
using UnityEngine.UI;


public partial class UIModule : BaseGameModule
{
   
    // 打开UI的根节点
    public Transform normalUIRoot;

    public Transform modalUIRoot;
    // 关闭UI的根节点
    public Transform closeUIRoot;

    // 遮罩图片
    public Image imgMask;

    // QFSW.QC 是一个在 Unity 中常用的工具库，叫做 Quantum Console。这是一个用于调试和开发的命令行控制台。
    // Quantum Console 提供了一个在游戏运行时可以输入和执行命令的界面，可以极大地方便开发和调试工作。
    // Quantum Console 允许你在游戏运行时输入并执行命令，这对于调试和测试非常有用。你可以动态调用方法、修改变量、执行脚本等。
    // 量子控制台
    public QuantumConsole prefabQuantumConsole;
    
    // 静态的字典 中介映射  映射UI视图I和相应的类型，字典的作用是用于查找和管理UI中介器
    private static Dictionary<UIViewID, Type> MEDIATOR_MAPPING;

    // 资产映射
    private static Dictionary<UIViewID, Type> ASSET_MAPPING;


    // 只读修饰符，用于声明只读字段，表示字段的值一旦初始化后就不能再修改。
    // 声明一个只读的使用中介列表
    private readonly List<UIMediator> usingMediators = new List<UIMediator>();
    // 存放空闲中介的字典，通过类型进行分类和管理，每个类型对应一个中介队列
    private readonly Dictionary<Type, Queue<UIMediator>> freeMediators = new Dictionary<Type, Queue<UIMediator>>();
    // 用于管理UI对象的对象池。
    private readonly GameObjectPool<GameObjectAsset> uiObjectPool = new GameObjectPool<GameObjectAsset>();

    // 用于存储QuantumConsole实例的字段。
    private QuantumConsole quantumConsole;

    protected internal override void OnModuleInit()
    {
        base.OnModuleInit();
        //quantumConsole = Instantiate(prefabQuantumConsole);
        //quantumConsole.transform.SetParentAndResetAll(transform);
        //quantumConsole.OnActivate += OnConsoleActive;
        //quantumConsole.OnDeactivate += OnConsoleDeactive;
    }

    protected internal override void OnModuleStop()
    {
        //base.OnModuleStop();
        //quantumConsole.OnActivate -= OnConsoleActive;
        //quantumConsole.OnDeactivate -= OnConsoleDeactive;
    }

    /// <summary>
    /// 缓存UIViewID 与 UIMediator 、 UIView 的映射信息
    /// 将每个UIView类型与其对应的Mediator和Asset进行关联
    /// 方法遍历所有继承自UIView的类型，查找并处理它们的自定义属性，将其信息存储在两个字典中
    /// </summary>
    private static void CacheUIMapping()
    {
        // 缓存检测，如果 MEDIATOR_MAPPING 已经被初始化，说明映射已经缓存过，结束方法，避免重复初始化
        if (MEDIATOR_MAPPING != null)
            return;

        // 初始化两个字典
        MEDIATOR_MAPPING = new Dictionary<UIViewID, Type>();
        ASSET_MAPPING = new Dictionary<UIViewID, Type>();

        // 获取UIView类型，所有的UI视图都继承这个基类
        Type baseViewType = typeof(UIView);
        // 遍历UIView所在程序集的所有类型
        foreach (var type in baseViewType.Assembly.GetTypes())
        {
            // 过滤出抽象类型
            if (type.IsAbstract)
                // 跳过抽象类型，因为不能被实例化
                continue;
            // IsAssignableFrom 用于检查type是否继承自 UIView
            if (baseViewType.IsAssignableFrom(type))
            {
                // 获取并检查自定义属性,false表明只搜索当前type上的自定义属性，不包括基类型
                object[] attrs = type.GetCustomAttributes(typeof(UIViewAttribute), false);
                if (attrs.Length == 0)
                {
                    // 获取当前类型上的UIViewAttribute自定义属性。如果没有找到该属性，记录错误日志并跳过该类型。
                    UnityLog.Error($"{type.FullName} 没有绑定 Mediator，请使用UIMediatorAttribute绑定一个Mediator以正确使用");
                    continue;
                }
                // 遍历找到的所有的UIViewAttribute属性，通常只有一个
                foreach (UIViewAttribute attr in attrs)
                {
                    // 将ID和MediatorType添加到MEDIATOR_MAPPING字典，中介类型
                    MEDIATOR_MAPPING.Add(attr.ID, attr.MediatorType);
                    // Debug.Log(attr.MediatorType);
                    // 将ID和视图类型添加到ASSET_MAPPING字典,继承UIView的类型 
                    ASSET_MAPPING.Add(attr.ID, type);
                    // Debug.Log(type);
                    break;
                }
            }
        }
    }
    /// <summary>
    /// 每祯调用
    /// </summary>
    /// <param name="deltaTime">表示自上一帧以来经过的时间，用于帧更新逻辑中。</param>
    protected internal override void OnModuleUpdate(float deltaTime)
    {
        base.OnModuleUpdate(deltaTime);
        // 
        uiObjectPool.UpdateLoadRequests();

        // 遍历列表中的每一个中介，调用它们的 Update 方法，将 deltaTime 作为参数传递进去。用于更新每个中介者的状态。
        foreach (var mediator in usingMediators)
        {
            // 调用中介者的UpDate方法
            mediator.Update(deltaTime);
        }

        UpdateMask(deltaTime);
    }

    private void OnConsoleActive()
    {
        //GameManager.Input.SetEnable(false);
    }

    private void OnConsoleDeactive()
    {
        //GameManager.Input.SetEnable(true);
    }
    /// <summary>
    /// 获取本类型UI面板SortingOrder的最大值
    /// </summary>
    /// <param name="mode"></param>
    /// <returns></returns>
    private int GetTopMediatorSortingOrder(UIMode mode)
    {
        // 如果没有找到合适的中介者索引，索引将保持为-1
        int lastIndexMediatorOfMode = -1;
        // 从后向前遍历中介者列表
        for (int i = usingMediators.Count - 1; i >= 0; i--)
        {
            
            UIMediator mediator = usingMediators[i];
            // 检查其 UIMode 是否与参数 mode 相同。
            if (mediator.UIMode != mode)
                continue;
            // 记录其索引到 lastIndexMediatorOfMode，然后 break 退出循环。
            lastIndexMediatorOfMode = i;
            break;
        }

        // 如果没有找到符合条件的中介者，返回一个默认的排序顺序：
        // 对于 UIMode.Normal，返回 0
        // 对于其他模式，返回 1000
        if (lastIndexMediatorOfMode == -1)
            return mode == UIMode.Normal ? 0 : 1000;
        // 如果找到符合条件的中介者，返回其 SortingOrder 值。
        return usingMediators[lastIndexMediatorOfMode].SortingOrder;
    }

    /// <summary>
    /// 获取中介
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private UIMediator GetMediator(UIViewID id)
    {
        CacheUIMapping(); // 获取中介的时候先缓存中介映射信息
        // 通过id获取中介
        if (!MEDIATOR_MAPPING.TryGetValue(id, out Type mediatorType))
        {
            UnityLog.Error($"找不到 {id} 对应的Mediator");
            return null;
        }
        // 从freeMediator获取mediatorType的中介者队列，
        // 如果不存在对应类型的队列，则创建一个新的队列，并将其添加到 freeMediators 字典中。
        if (!freeMediators.TryGetValue(mediatorType, out Queue<UIMediator> mediatorQ))
        {
            // 创建一个新的队列
            mediatorQ = new Queue<UIMediator>();
            // 添加进字典中
            freeMediators.Add(mediatorType, mediatorQ);
        }

        UIMediator mediator;
        // 如果队列为空
        if (mediatorQ.Count == 0)
        {
            // 创建一个新的中介实例
            mediator = Activator.CreateInstance(mediatorType) as UIMediator;
        }
        else
        {
            // 如果不为空，取出一个中介实例
            mediator = mediatorQ.Dequeue();
        }

        // 返回创建的中介者实例
        return mediator;
    }

    /// <summary>
    /// 回收中介实例
    /// </summary>
    /// <param name="mediator"></param>
    private void RecycleMediator(UIMediator mediator)
    {
        // 实例被销毁
        if (mediator == null)
            return;

        // 获取实例的类型
        Type mediatorType = mediator.GetType();
        // 从freeMediator获取mediatorType的中介者队列，
        // 如果不存在对应类型的队列，则创建一个新的队列，并将其添加到 freeMediators 字典中。
        if (!freeMediators.TryGetValue(mediatorType, out Queue<UIMediator> mediatorQ))
        {
            // 创建一个新队列
            mediatorQ = new Queue<UIMediator>();
            // 添加进字典
            freeMediators.Add(mediatorType, mediatorQ);
        }
        // 入队
        mediatorQ.Enqueue(mediator);
    }
    /// <summary>
    /// 获取打开的UI中介
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public UIMediator GetOpeningUIMediator(UIViewID id)
    {
        // 通过id获取配置
        UIConfig uiConfig = UIConfig.ByID((int)id);
        // 没有对应的配置，直接返回null
        if (uiConfig.IsNull)
            return null;
        // 通过id获取中介
        UIMediator mediator = GetMediator(id);
        // 如果获取的中介为空
        if (mediator == null)
            // 返回空
            return null;
        // 获取中介的类型
        Type requiredMediatorType = mediator.GetType();
        // 遍历集合usingMediators，这个集合应该包含了所有当前正在使用的中介对象
        foreach (var item in usingMediators)
        {
            // 遍历集合中的每一个中介对象，检查其类型是否与requiredMediatorType相同。如果找到相同类型的中介对象，则返回该对象。
            if (item.GetType() == requiredMediatorType)
                return item;
        }
        // 没有找到同类型的返回null
        return null;
    }
    /// <summary>
    /// 将指定UIViewID对应的UIMediator对象提升到界面顶层
    /// 更新中介对象的排序顺序，使其在显示层级上位于最前面
    /// </summary>
    /// <param name="id"></param>
    public void BringToTop(UIViewID id)
    {
        // 通过UIViewID获取对应的中介对象。
        UIMediator mediator = GetOpeningUIMediator(id);

        // 如果获取的中介对象为空，直接返回，不执行后续操作。
        if (mediator == null)
            return;

        // 获取当前中介模式（UIMode）下的最高排序顺序。
        int topSortingOrder = GetTopMediatorSortingOrder(mediator.UIMode);
        // 如果当前中介对象的排序顺序已经是最高，则不需要调整，直接返回。
        if (mediator.SortingOrder == topSortingOrder)
            return;
        // 设置新的排序顺序，比当前最高顺序高10。
        int sortingOrder = topSortingOrder + 10;
        // 将中介对象的排序顺序更新为新的排序顺序。
        mediator.SortingOrder = sortingOrder;
        // 先将中介对象从集合usingMediators中移除。
        usingMediators.Remove(mediator);
        // 然后将中介对象重新添加到集合末尾，使其在集合中的位置更新。
        usingMediators.Add(mediator);

        // 获取中介对象关联的视图对象上的Canvas组件。
        Canvas canvas = mediator.ViewObject.GetComponent<Canvas>();
        if (canvas != null)
        {
            // 如果Canvas组件存在，更新其sortingOrder属性，使其在UI渲染层级上处于最前。
            canvas.sortingOrder = sortingOrder;
        }
    }
    /// <summary>
    /// 判断UI是否是打开的，打开一个单例的UI时进行判断
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool IsUIOpened(UIViewID id)
    {
        return GetOpeningUIMediator(id) != null;
    }
    /// <summary>
    /// 打开一个单例的UI界面（UIMediator），如果该界面已经在打开状态，则直接返回现有的中介对象；否则，新打开该界面
    /// </summary>
    /// <param name="id"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    public UIMediator OpenUISingle(UIViewID id, object arg = null)
    {
        UIMediator mediator = GetOpeningUIMediator(id);

        if (mediator != null)
            return mediator;
        // 新打开该界面
        return OpenUI(id, arg);
    }

    /// <summary>
    /// 打开界面
    /// </summary>
    /// <param name="id"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    public UIMediator OpenUI(UIViewID id, object arg = null)
    {
        // 获取配置
        UIConfig uiConfig = UIConfig.ByID((int)id);
        
        if (uiConfig.IsNull)
            return null;
        // 获取中介
        UIMediator mediator = GetMediator(id);
        if (mediator == null)
            return null;

        // 加载资源
        GameObject uiObject = (uiObjectPool.LoadGameObject(uiConfig.Asset, (obj) =>
        {
            // 获取组件
            UIView newView = obj.GetComponent<UIView>();
            // 初始化中介者状态
            mediator.InitMediator(newView);
        })).gameObject;

        return OnUIObjectLoaded(mediator, uiConfig, uiObject, arg);
    }
    /// <summary>
    /// 异步打开一个单例UI
    /// </summary>
    /// <param name="id"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    public IEnumerator OpenUISingleAsync(UIViewID id, object arg = null)
    {
        // 如果这个面板没有在打开状态，进行下一步操作
        if (!IsUIOpened(id))
        {
            yield return OpenUIAsync(id, arg);
        }
    }
    /// <summary>
    /// 异步打开
    /// </summary>
    /// <param name="id"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    public IEnumerator OpenUIAsync(UIViewID id, object arg = null)
    {
        UIConfig uiConfig = UIConfig.ByID((int)id);
        if (uiConfig.IsNull)
            yield break;

        UIMediator mediator = GetMediator(id);
        if (mediator == null)
            yield break;

        bool loadFinish = false;

        uiObjectPool.LoadGameObjectAsync(uiConfig.Asset, (asset) =>
        {
            GameObject uiObject = asset.gameObject;

            OnUIObjectLoaded(mediator, uiConfig, uiObject, arg);

            loadFinish = true;

        }, (obj) =>
        {
            // 获取面板
            UIView newView = obj.GetComponent<UIView>();
            // 初始化中介
            mediator.InitMediator(newView);
        });
        // 等待加载完成 while 循环每一帧都检查 loadFinish 的值：
        // 如果 loadFinish 是 false，则 yield return null 暂停协程，等待下一帧。
        // 如果 loadFinish 是 true，则跳出循环，继续执行后续代码。
        while (!loadFinish)
        {
            // 非阻塞性: yield return null 暂停协程直到下一帧，不会阻塞主线程。这种非阻塞行为是协程的核心特性之一，确保主线程可以继续处理其他任务
            yield return null;
        }
        // 两次 yield return null 确保协程完全执行完毕并返回控制权。
        yield return null;
        yield return null;
    }
    /// <summary>
    /// 处理UI对象加载完成后的逻辑，包括初始化中介对象（UIMediator）、设置UI对象的层级和排序顺序，并将UI对象显示在屏幕上
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="uiConfig"></param>
    /// <param name="uiObject"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    private UIMediator OnUIObjectLoaded(UIMediator mediator, UIConfig uiConfig, GameObject uiObject, object obj)
    {
        // 如果uiObject为空，记录错误日志，并回收中介对象，然后返回null。
        if (uiObject == null)
        {
            UnityLog.Error($"加载UI失败:{uiConfig.Asset}");
            RecycleMediator(mediator);
            return null;
        }
        // 获取UIView组件。如果UIView组件不存在，记录错误日志，回收中介对象并卸载UI对象，然后返回null。
        UIView view = uiObject.GetComponent<UIView>();
        if (view == null)
        {
            UnityLog.Error($"UI Prefab不包含UIView脚本:{uiConfig.Asset}");
            RecycleMediator(mediator);

            uiObjectPool.UnloadGameObject(view.gameObject);
            return null;
        }
        // 设置中介对象的UIMode为配置中的模式，并计算新的排序顺序。然后将中介对象添加到usingMediators集合中。
        mediator.UIMode = uiConfig.Mode;
        int sortingOrder = GetTopMediatorSortingOrder(uiConfig.Mode) + 10;

        // Debug.Log(sortingOrder);
        usingMediators.Add(mediator);

        // 获取Canvas组件，并将其渲染模式设置为ScreenSpaceCamera。注释掉的代码需要设置UI相机。
        Canvas canvas = uiObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;

        // 根据UIMode，将UI对象设置到不同的根节点，并设置Canvas的排序层。
        //canvas.worldCamera = GameManager.Camera.uiCamera;

        if (uiConfig.Mode == UIMode.Normal)
        {
            // 设置父对象
            uiObject.transform.SetParentAndResetAll(normalUIRoot);
            // 设置排序层名称
            canvas.sortingLayerName = "NormalUI";
        }
        else
        {
            // 设置父对象
            uiObject.transform.SetParentAndResetAll(modalUIRoot);
            // 设置排序层名称
            canvas.sortingLayerName = "ModalUI";
        }
        // 设置中介对象和Canvas的排序顺序，
        mediator.SortingOrder = sortingOrder;
        canvas.sortingOrder = sortingOrder;
        // 激活UI对象，并调用mediator.Show方法显示UI对象。最后返回中介对象。
        uiObject.SetActive(true);
        mediator.Show(uiObject, obj);
        return mediator;
    }

    /// <summary>
    /// 关闭界面
    /// </summary>
    /// <param name="mediator"></param>
    public void CloseUI(UIMediator mediator)
    {
        if (mediator != null)
        {
            // 回收View
            uiObjectPool.UnloadGameObject(mediator.ViewObject);
            mediator.ViewObject.transform.SetParentAndResetAll(closeUIRoot);

            // 回收Mediator
            mediator.Hide();
            RecycleMediator(mediator);
            // 从使用集合中移除
            usingMediators.Remove(mediator);
        }
    }
    /// <summary>
    /// 关闭所有UI
    /// </summary>
    public void CloseAllUI()
    {
        for (int i = usingMediators.Count - 1; i >= 0; i--)
        {
            CloseUI(usingMediators[i]);
        }
    }
    /// <summary>
    /// 通过Id关闭UI
    /// </summary>
    /// <param name="id"></param>
    public void CloseUI(UIViewID id)
    {
        UIMediator mediator = GetOpeningUIMediator(id);
        if (mediator == null)
            return;

        CloseUI(mediator);
    }

    public void SetAllNormalUIVisibility(bool visible)
    {
        normalUIRoot.gameObject.SetActive(visible);
    }

    public void SetAllModalUIVisibility(bool visible)
    {
        modalUIRoot.gameObject.SetActive(visible);
    }

    public void ShowMask(float duration = 0.5f)
    {
        destMaskAlpha = 1;
        maskDuration = duration;
    }

    public void HideMask(float? duration = null)
    {
        destMaskAlpha = 0;
        if (duration.HasValue)
        {
            maskDuration = duration.Value;
        }
    }
    // 目标透明度 Destination
    private float destMaskAlpha = 0;
    // 遮罩持续时间
    private float maskDuration = 0;
    /// <summary>
    /// 更新一个UI遮罩的透明度
    /// 
    /// </summary>
    /// <param name="deltaTime"></param>
    private void UpdateMask(float deltaTime)
    {
        // 获取遮罩的颜色
        Color c = imgMask.color;
        // 根据 maskDuration 和 deltaTime 计算新的透明度值
        // 当遮罩持续时间maskDuration>0时，使用Mathf.MoveTowards将当前透明度逐步移动到目标透明度，destMaskAlpha,移动的步长是1/maskDration *deltaTime
        // 如果maskDuration <= 0，直接将透明度设置为目标透明度
        c.a = maskDuration > 0 ? Mathf.MoveTowards(c.a, destMaskAlpha, 1f / maskDuration * deltaTime) : destMaskAlpha;
        // 限制透明度值在0-1之间
        c.a = Mathf.Clamp01(c.a);
        // 将透明度赋值
        imgMask.color = c;
        // 检查 imgMask 的透明度值，如果大于 0，则启用 imgMask；否则禁用它。
        // 目的是优化性能，在遮罩完全透明时将其禁用。
        imgMask.enabled = imgMask.color.a > 0;
    }

    public void ShowConsole()
    {
        quantumConsole.Activate();
    }
}


[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
sealed class UIViewAttribute : Attribute
{
    public UIViewID ID { get; }
    public Type MediatorType { get; }

    public UIViewAttribute(Type mediatorType, UIViewID id)
    {
        ID = id;
        MediatorType = mediatorType;
    }

    
}




