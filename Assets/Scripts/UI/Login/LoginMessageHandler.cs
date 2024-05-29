using System;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class LoginMessageHandler : MessageHandler<MessageType.Login>
{
   
    public override async Task HandleMessage(MessageType.Login arg)
    {
        UnityLog.Info("´ò¿ªLoinÃæ°å");


        
    }
}
