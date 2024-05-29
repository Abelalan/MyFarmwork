using System.Threading.Tasks;


public class LoadingUI_SceneCreatingMessageHandler : MessageHandler<MessageType.GameSceneCreating>
    {
        public override async Task HandleMessage(MessageType.GameSceneCreating arg)
        {
            if (arg.data.showLoading)
            {
                await GameManager.UI.OpenUIAsync(UIViewID.LoadingUI);
            }
        }
    }
 
