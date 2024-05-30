using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class LoginUIMediator : UIMediator<LoginUIView>
{
    protected override void OnInit(LoginUIView view)
    {
        base.OnInit(view);
        view.CloseButton.onClick.AddListener(() =>
        {
            Close();
        });
    }

    protected override void OnShow(object arg)
    {
        
    }


    protected override void OnHide()
    {
        
    }
    protected override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
    }
}
