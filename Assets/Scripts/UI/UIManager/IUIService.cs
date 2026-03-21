namespace UI
{
    public interface IUIService
    {
        T Show<T>() where T : UIView;
        void Hide<T>() where T : UIView;
        void HideAll();
    }
}