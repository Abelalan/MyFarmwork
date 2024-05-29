using Config;
using Nirvana;
using System.Xml;
using UnityEngine;

/// <summary>
/// 定义了一个泛型类，继承自UIMediator，并且限制泛型T必须是UIView 的子类
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class UIMediator<T> : UIMediator where T : UIView
{
    protected T view;
    /// <summary>
    /// 重写OnShow方法
    /// </summary>
    /// <param name="arg"></param>
    protected override void OnShow(object arg)
    {
        base.OnShow(arg);
        // 获取继承UIView的组件
        view = ViewObject.GetComponent<T>();
    }
    /// <summary>
    /// 重写OnHide组件
    /// </summary>
    protected override void OnHide()
    {
        view = default;
        base.OnHide();
    }

    protected void Close()
    {
        // 调用 TGameFramework 的 UIModule 模块的 CloseUI 方法来关闭当前 UI。
        TGameFramework.Instance.GetModule<UIModule>().CloseUI(this);
    }

    public override void InitMediator(UIView view)
    {
        base.InitMediator(view);

        OnInit(view as T);
    }

    protected virtual void OnInit(T view) { }
}


public abstract class UIMediator
{
    // UI隐藏时触发的事件
    public event System.Action OnMediatorHide;
    // 表示当前的显示的UI对象
    public GameObject ViewObject { get; set; }
    // UI对象的组件，事件处理
    public UIEventTable eventTable { get; set; }
    // 命名管理
    public UINameTable nameTable { get; set; }
    // UI显示的排序顺序
    public int SortingOrder { get; set; }
    // UI模式，用于定义UI的行为或状态
    public UIMode UIMode { get; set; }
    // 初始化中介者状态
    public virtual void InitMediator(UIView view) { }
    /// <summary>
    /// 展示UI
    /// </summary>
    /// <param name="viewObject">UI面板对象</param>
    /// <param name="arg">传递过来的数据</param>
    public void Show(GameObject viewObject, object arg)
    {
        // 全局变量赋值
        ViewObject = viewObject;   
        // 获取事件处理的组件
        eventTable = ViewObject.GetComponent<UIEventTable>();
        // 获取名称处理的组件
        nameTable = viewObject.GetComponent<UINameTable>();
        // 展示UI
        OnShow(arg);
    }
    // 显示 UI 时的逻辑，供子类重写。
    protected virtual void OnShow(object arg) { }
    /// <summary>
    /// 隐藏UI的方法
    /// </summary>
    public void Hide()
    {
        // 调用OnHide方法

        OnHide();
        // 触发OnMediatorHide事件
        OnMediatorHide?.Invoke();
        // 清除订阅事件
        OnMediatorHide = null;
        // 清除UI对象
        ViewObject = default;
    }

    protected virtual void OnHide() { }

    // 更新UI方法，每祯调用一次
    public void Update(float deltaTime)
    {
        OnUpdate(deltaTime);

    }

    protected virtual void OnUpdate(float deltaTime) { }
}

