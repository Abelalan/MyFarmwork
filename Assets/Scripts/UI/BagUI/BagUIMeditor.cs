public class BagUIMeditor : UIMediator<BagUIView>
{
    protected override void OnInit(BagUIView view)
    {
        base.OnInit(view);
        view.CloseButton.onClick.AddListener(() =>
        {
            Close();
        });
    }

    
}
