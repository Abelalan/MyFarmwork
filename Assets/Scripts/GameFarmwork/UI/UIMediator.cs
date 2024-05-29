using Config;
using Nirvana;
using System.Xml;
using UnityEngine;

/// <summary>
/// ������һ�������࣬�̳���UIMediator���������Ʒ���T������UIView ������
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class UIMediator<T> : UIMediator where T : UIView
{
    protected T view;
    /// <summary>
    /// ��дOnShow����
    /// </summary>
    /// <param name="arg"></param>
    protected override void OnShow(object arg)
    {
        base.OnShow(arg);
        // ��ȡ�̳�UIView�����
        view = ViewObject.GetComponent<T>();
    }
    /// <summary>
    /// ��дOnHide���
    /// </summary>
    protected override void OnHide()
    {
        view = default;
        base.OnHide();
    }

    protected void Close()
    {
        // ���� TGameFramework �� UIModule ģ��� CloseUI �������رյ�ǰ UI��
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
    // UI����ʱ�������¼�
    public event System.Action OnMediatorHide;
    // ��ʾ��ǰ����ʾ��UI����
    public GameObject ViewObject { get; set; }
    // UI�����������¼�����
    public UIEventTable eventTable { get; set; }
    // ��������
    public UINameTable nameTable { get; set; }
    // UI��ʾ������˳��
    public int SortingOrder { get; set; }
    // UIģʽ�����ڶ���UI����Ϊ��״̬
    public UIMode UIMode { get; set; }
    // ��ʼ���н���״̬
    public virtual void InitMediator(UIView view) { }
    /// <summary>
    /// չʾUI
    /// </summary>
    /// <param name="viewObject">UI������</param>
    /// <param name="arg">���ݹ���������</param>
    public void Show(GameObject viewObject, object arg)
    {
        // ȫ�ֱ�����ֵ
        ViewObject = viewObject;   
        // ��ȡ�¼���������
        eventTable = ViewObject.GetComponent<UIEventTable>();
        // ��ȡ���ƴ�������
        nameTable = viewObject.GetComponent<UINameTable>();
        // չʾUI
        OnShow(arg);
    }
    // ��ʾ UI ʱ���߼�����������д��
    protected virtual void OnShow(object arg) { }
    /// <summary>
    /// ����UI�ķ���
    /// </summary>
    public void Hide()
    {
        // ����OnHide����

        OnHide();
        // ����OnMediatorHide�¼�
        OnMediatorHide?.Invoke();
        // ��������¼�
        OnMediatorHide = null;
        // ���UI����
        ViewObject = default;
    }

    protected virtual void OnHide() { }

    // ����UI������ÿ������һ��
    public void Update(float deltaTime)
    {
        OnUpdate(deltaTime);

    }

    protected virtual void OnUpdate(float deltaTime) { }
}

