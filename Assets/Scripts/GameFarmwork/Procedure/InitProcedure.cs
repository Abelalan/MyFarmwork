using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class InitProcedure : BaseProcedure
{
    public override async Task OnEnterProcedure(object value)
    {

        await base.OnEnterProcedure(value);

        Debug.Log("�л�Ϊ��ʼ��״̬");

        await Task.Yield();

    }
}
