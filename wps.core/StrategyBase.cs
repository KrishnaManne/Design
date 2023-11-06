namespace wps.core
{
    public abstract class StrategyBase : IStrategy
    {
        public Activity Activity { get; private set; }

        public IStrategy SetActivity(Activity activity)
        {
            Activity = activity;
            return this;
        }

        public string GetParameter(string parameterName)
        {
            return Activity.Parameters.Exists(x => x.Name == parameterName)
            ? Activity.Parameters.FirstOrDefault(x => x.Name == parameterName).Value
            : null;
        }

        public abstract IStrategy Initialize();
        public abstract IStrategy Validate();
        public abstract Task ExecuteAsync();
        public abstract Task RollbackAsync();
    }
}