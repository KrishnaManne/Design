namespace wps.core
{
    public interface IStrategy
    {
        IStrategy SetActivity(Activity activity);
        IStrategy Initialize();
        IStrategy Validate();
        Task ExecuteAsync();
        Task RollbackAsync();
    }
}