using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class LoginUIMediator : UIMediator<LoginUIView>
{
    protected override void OnInit(LoginUIView view)
    {
        base.OnInit(view);
    }

    protected override void OnShow(object arg)
    {
        
    }


    private void onLogin(object[] args)
    {
        
    }

  

    //private void onLogin()
    //{
    //    Debug.Log("¿ªÊ¼µÇÂ¼");
    //    GameManager.Message.Post<MessageType.Login>(new MessageType.Login()).Coroutine();
    //}

    protected override void OnHide()
    {
        
    }
    protected override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
    }
}
