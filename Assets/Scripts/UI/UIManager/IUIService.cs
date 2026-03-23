namespace UI
{
    /// <summary>Abstraction over screen stack: show/hide <see cref="UIView"/> derivatives by type.</summary>
    public interface IUIService
    {
        #region Methods

        T Show<T>() where T : UIView;
        void Hide<T>() where T : UIView;
        void HideAll();

        #endregion
    }
}
