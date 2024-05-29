using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class InitProcedure : BaseProcedure
{
    public override async Task OnEnterProcedure(object value)
    {

        UnityLog.Info("enter init procedure");
        //GameManager.ECS.World.AddComponent<KnapsackComponent>();
        //GameManager.ECS.World.AddComponent<PlayerInfoComponent>();
        //GameManager.ECS.World.AddComponent<GameSceneComponent>();
        //  GameManager.ECS.World.AddNewComponent<PlayerComponent>();
        //GameManager.ECS.World.AddNewComponent<TaskComponent>();

        ///链接登录服务器
        //await GameManager.Net.ConnectLoginServer();

        await GameManager.UI.OpenUIAsync(UIViewID.LoginUI);
        //GameManager.Audio.PlayBGM(1);
        await Task.Yield();

    }
}
