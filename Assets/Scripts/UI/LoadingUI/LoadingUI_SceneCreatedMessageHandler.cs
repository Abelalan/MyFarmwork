using System.Threading.Tasks;
using UnityEngine;

namespace Koakuma.Game.UI
{
    public class LoadingUI_SceneCreatedMessageHandler : MessageHandler<MessageType.GameSceneCreated>
    {
        public override async Task HandleMessage(MessageType.GameSceneCreated arg)
        {
            //// 激活刷怪点
            //await GameManager.Message.Post(new MessageType.ActiveMonsterSpawner());

            //UnityLog.Info($"before scene object id:{IDGenerator.CurrentInstanceID()}");
            //// 实例化交互物体
            //GameSceneComponent gameSceneComponent = GameManager.ECS.World.GetComponent<GameSceneComponent>();
            //if (gameSceneComponent != null)
            //{
            //    while (gameSceneComponent.waitingForCreateSceneObjects.Count > 0)
            //    {
            //        CreateSceneObjectData createSeneObjectData = gameSceneComponent.waitingForCreateSceneObjects.Dequeue();
            //        SceneFactory.CreateSceneObject(createSceneObjectData);
            //    }
            //}

            UnityLog.Info($"after scene object id:{IDGenerator.CurrentInstanceID()}");

            if (arg.data.showLoading)
            {
                //await new  WaitForSeconds(3);
                GameManager.UI.CloseUI(UIViewID.LoadingUI);
            }
            await Task.Yield();
        }
    }
}
