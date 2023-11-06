using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace wps.core
{
    public class Activity : IActivity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public List<Parameter> Parameters { get; set; }
        public bool Rollback { get; set; } = true;
        public string RunAfter { get; set; }
        private IStrategy strategy;

        [JsonIgnore]
        [XmlIgnore]
        public IStrategy Strategy
        {
            get { return strategy; }
            set
            {
                strategy = value;
            }
        }

        public virtual async Task ExecuteAsync()
        {
            Console.WriteLine($"ExecuteAsync called.");
            try
            {
                await strategy.ExecuteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occured while executing {Name} activity. Exception::: {ex}");
                if (Rollback == false)
                {
                    Console.WriteLine("Since Rollback disabled rolling back changes is not done.");
                    return;
                }
                Console.WriteLine("Since Rollback enabled rolling back changes.");
                await RollbackAsync();
            }
        }

        public virtual async Task RollbackAsync()
        {
            Console.WriteLine($"Activity.RollbackAsync called.");
            await strategy.RollbackAsync();
        }
    }
}