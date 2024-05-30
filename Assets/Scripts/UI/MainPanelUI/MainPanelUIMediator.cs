using UnityEngine.UI;

public class MainPanelUIMediator : UIMediator<MainPanelUIView>
{
    bool isShowMask = true;
    protected override void OnInit(MainPanelUIView view)
    {
        base.OnInit(view);
        view.LoginUIButton.onClick.AddListener(() =>
        {
            GameManager.UI.OpenUI(UIViewID.LoginUI);
        });
        view.BagUIButton.onClick.AddListener(() =>
        {
            GameManager.UI.OpenUI(UIViewID.BagUI);
        });
        view.MaskButton.onClick.AddListener(() =>
        {
            if (isShowMask)
            {
                GameManager.UI.ShowMask(0.5f);
                isShowMask = false;
            }
            else
            {
                GameManager.UI.HideMask();
                isShowMask = true;
            }
           
        });
    }
}