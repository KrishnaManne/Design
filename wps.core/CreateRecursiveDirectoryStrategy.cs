namespace wps.core
{
    public class CreateRecursiveDirectoryStrategy : StrategyBase
    {
        public static string TypeName = "CreateDirectory";
        private string path;

        public override IStrategy Initialize()
        {
            //throw new NotImplementedException();
            Console.WriteLine($"{nameof(CreateRecursiveDirectoryStrategy)}.InitializeAsync called.");
            path = GetParameter("path");
            return this;
        }

        public override IStrategy Validate()
        {
            Console.WriteLine($"{nameof(CreateRecursiveDirectoryStrategy)}.ValidateAsync called.");
            ArgumentNullException.ThrowIfNull(GetParameter("path"), "path");
            return this;
        }

        public override Task ExecuteAsync()
        {
            Console.WriteLine($"{nameof(CreateRecursiveDirectoryStrategy)}.ExecuteAsync called.");

            CreateDirectoryIfNotExists(path);

            return Task.CompletedTask;
        }

        public override Task RollbackAsync()
        {
            Console.WriteLine($"{nameof(CreateRecursiveDirectoryStrategy)}.RollbackAsync called.");



            DeleteDirectoryIfExists(path);

            return Task.CompletedTask;
        }

        private void CreateDirectoryIfNotExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Console.WriteLine("Directory created: " + path);
            }
            else
            {
                Console.WriteLine("Directory already exists: " + path);
            }
        }

        private void DeleteDirectoryIfExists(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                Console.WriteLine("Directory deleted: " + path);
            }
            else
            {
                Console.WriteLine("Directory does not exist: " + path);
            }
        }
    }
}