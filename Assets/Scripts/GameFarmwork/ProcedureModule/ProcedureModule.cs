using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 处理游戏总流程，控制游戏过程
/// ProcedureModule流程模块，用于管理游戏中的不同流程也可以说是状态
/// 包括初始化、启动、停止和切换流程
/// 通过字典管理流程实例，通过队列处理流程切换请求，并使用对象池优化性能。
/// 允许添加、切换和管理不同的游戏或应用程序状态。
/// </summary>
public class ProcedureModule : BaseGameModule
{
    // proceduresNames用于存储流程类名的字符串数组
    [SerializeField]
    private string[] proceduresNames = null;
    // 默认流程的类名
    [SerializeField]
    private string defaultProcedureName = null;
    // 当前正在运行的流程
    public BaseProcedure CurrentProcedure { get; private set; }
    // 记录流程是否正在运行的标记
    public bool IsRunning { get; private set; }
    // 记录是否正在切换流程的标记
    public bool IsChangingProcedure { get; private set; }

    // 存储流程类型和实例的字典
    private Dictionary<Type, BaseProcedure> procedures;
    // 默认流程实例
    private BaseProcedure defaultProcedure;
    // 使用对象池来管理ChangeProcedureRequest
    private ObjectPool<ChangeProcedureRequest> changeProcedureRequestPool = new ObjectPool<ChangeProcedureRequest>(null);
    // 切换改变流程请求的队列
    private Queue<ChangeProcedureRequest> changeProcedureQ = new Queue<ChangeProcedureRequest>();

    /// <summary>
    /// 初始化模块时调用，负责初始化流程的字典和默认流程
    /// </summary>
    protected internal override void OnModuleInit()
    {
        base.OnModuleInit();
        // 初始化字典
        procedures = new Dictionary<Type, BaseProcedure>();
        // 标记是否找到了默认流程
        bool findDefaultState = false;
        for (int i = 0; i < proceduresNames.Length; i++)
        {
            // 流程的类型名字
            string procedureTypeName = proceduresNames[i];
            // 判空操作，如果为空，则跳出此次循环
            if (string.IsNullOrEmpty(procedureTypeName))
                continue;
            // Type.GetType()是用于获取类型的静态方法
            // 作用是尝试获取名称为 procedureTypeName 的类型，并将其赋值给 procedureType 变量。如果找不到该类型，将引发 TypeLoadException 异常。
            // 参数true，用来指示方法在找不到指定类型时是否引发异常
            Type procedureType = Type.GetType(procedureTypeName, true);
            // 没有获取到相应的类型
            if (procedureType == null)
            {
                // 输出错误
                Debug.LogError($"Can't find procedure:`{procedureTypeName}`");
                // 跳出循环
                continue;
            }
            // 通过反射创建流程的实例，并转换为BaseProcedure类型
            BaseProcedure procedure = Activator.CreateInstance(procedureType) as BaseProcedure;
            // 如果此流程为默认流程，isDefaultState = true
            bool isDefaultState = procedureTypeName == defaultProcedureName;
            // 将此流程添进入字典
            procedures.Add(procedureType, procedure);
            // 如果找到默认流程，将默认流程赋值
            if (isDefaultState)
            {
                defaultProcedure = procedure;
                // 将标记转找到默认流程
                findDefaultState = true;
            }
        }
        // 如果没有找到默认流程，输出错误
        if (!findDefaultState)
        {
            Debug.LogError($"You have to set a correct default procedure to start game");
        }
    }

    protected internal override void OnModuleStart()
    {
        base.OnModuleStart();
    }
    /// <summary>
    /// 流程模块停止时，释放资源
    /// </summary>
    protected internal override void OnModuleStop()
    {
        base.OnModuleStop();
        changeProcedureRequestPool.Clear();
        changeProcedureQ.Clear();

        IsRunning = false;
    }

    protected internal override void OnModuleUpdate(float deltaTime)
    {
        base.OnModuleUpdate(deltaTime);
    }

    /// <summary>
    /// 启动默认流程的方法
    /// </summary>
    /// <returns></returns>
    public async Task StartProcedure()
    {
        // 如果有流程在运行，结束方法
        if (IsRunning)
            return;
        // 没有流程在运行，启动默认流程，将IsRunning = true
        IsRunning = true;
        // 从对象池中获取一个改变流程的请求改变流程的请求
        ChangeProcedureRequest changeProcedureRequest = changeProcedureRequestPool.Obtain();
        // 将请求中的目标流程改为默认流程
        changeProcedureRequest.TargetProcedure = defaultProcedure;
        // 将改变的过的流程放入队列
        changeProcedureQ.Enqueue(changeProcedureRequest);
        // 等待改变流程的自行结果
        await ChangeProcedureInternal();
    }
    /// <summary>
    /// 切换流程，有两个重载
    /// 无参数版
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task ChangeProcedure<T>() where T : BaseProcedure
    {
        await ChangeProcedure<T>(null);
    }

    /// <summary>
    /// 切换流程，有参数版
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public async Task ChangeProcedure<T>(object value) where T : BaseProcedure
    {
        // 没有流程在运行，退出
        if (!IsRunning)
            return;
        // 判断字典中是否有此流程
        if (!procedures.TryGetValue(typeof(T), out BaseProcedure procedure))
        {
            // 没有此流程，输出错误日志
            UnityLog.Error($"Change Procedure Failed, Can't find Proecedure:${typeof(T).FullName}");
            return;
        }
        // 从对象池中获取一个改变流程的请求改变流程的请求
        ChangeProcedureRequest changeProcedureRequest = changeProcedureRequestPool.Obtain();
        // 给请求赋值
        changeProcedureRequest.TargetProcedure = procedure;
        changeProcedureRequest.Value = value;
        // 添加进队列
        changeProcedureQ.Enqueue(changeProcedureRequest);
        // 没有正在
        if (!IsChangingProcedure)
        {
            await ChangeProcedureInternal();
        }
    }

    private async Task ChangeProcedureInternal()
    {
        // 如果正在改变流程，结束方法
        if (IsChangingProcedure)
            return;

        // 标记为正在改变流程
        IsChangingProcedure = true;
        // 遍历队列
        while (changeProcedureQ.Count > 0)
        {
            // 获取需要改变的流程
            ChangeProcedureRequest request = changeProcedureQ.Dequeue();
          
            if (request == null || request.TargetProcedure == null)
                // 跳出本次循环
                continue;
            // 当前正在执行的流程不为空
            if (CurrentProcedure != null)
            {
                // 等待当前正在执行的流程关闭
                await CurrentProcedure.OnLeaveProcedure();
            }
            // 切换当前流程
            CurrentProcedure = request.TargetProcedure;
            // 等待进入当前执行的流程
            await CurrentProcedure.OnEnterProcedure(request.Value);
        }
        // 标记为没有改变流程
        IsChangingProcedure = false;
    }
}
