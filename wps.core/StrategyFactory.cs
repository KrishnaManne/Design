using System.Reflection;

namespace wps.core
{
    public class StrategyFactory
    {
        private static readonly Dictionary<string, Type> StrategyLookup;

        static StrategyFactory()
        {
            StrategyLookup = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(IStrategy)) && !t.IsAbstract)
                .ToDictionary(t => GetTypeName(t), t => t);
        }

        public static IStrategy CreateStrategyInstance(string key)
        {
            if (StrategyLookup.ContainsKey(key))
            {
                return (IStrategy)Activator.CreateInstance(StrategyLookup[key]);
            }
            else
            {
                throw new ArgumentException($"No strategy found for the provided key: {key}");
            }
        }

        private static string GetTypeName(Type strategyType)
        {
            return (string)strategyType.GetFields(BindingFlags.Public | BindingFlags.Static)
                .First(f => f.Name == "TypeName").GetValue(null);
        }
    }
}