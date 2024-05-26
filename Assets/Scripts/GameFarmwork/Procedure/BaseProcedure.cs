using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 游戏流程基类
/// BaseProcedure类是一种状态或流程管理系统。它定义了一个基本的状态或流程类，并提供了一些方法来处理状态的转换和进入/离开状态的行为。
/// </summary>
public class BaseProcedure
{
    /// <summary>
    /// 改变流程
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public async Task ChangeProcedure<T>(object value = null) where T : BaseProcedure
    {
        await GameManager.Procedure.ChangeProcedure<T>(value);
    }
    /// <summary>
    /// 进入流程的行为
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual async Task OnEnterProcedure(object value)
    {
        await Task.Yield();
    }
    /// <summary>
    /// 离开流程的行为
    /// </summary>
    /// <returns></returns>
    public virtual async Task OnLeaveProcedure()
    {
        await Task.Yield();
    }
}
