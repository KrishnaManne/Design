using System.Reflection.Metadata.Ecma335;

namespace wps.core
{
    public class CopyfilesStrategy : StrategyBase
    {
        public static string TypeName = "CopyFiles";
        private List<string> copiedFiles;
        private string source;
        private string destination;
        public CopyfilesStrategy()
        {
            copiedFiles = new List<string>();
        }

        public override IStrategy Initialize()
        {
            //throw new NotImplementedException();
            Console.WriteLine($"{nameof(CopyfilesStrategy)}.Initialize called.");
            source = GetParameter("source");
            destination = GetParameter("destination");
            return this;
        }        

        public override IStrategy Validate()
        {
            Console.WriteLine($"{nameof(CopyfilesStrategy)}.Validate called.");

            ArgumentNullException.ThrowIfNullOrEmpty(GetParameter("source"), "source");
            ArgumentNullException.ThrowIfNullOrEmpty(GetParameter("destination"), "destination");

            return this;
        }

        public override Task ExecuteAsync()
        {
            Console.WriteLine($"{nameof(CopyfilesStrategy)}.ExecuteAsync called.");
            CopyFiles(source, destination);
            return Task.CompletedTask;
        }

        public override Task RollbackAsync()
        {
            Console.WriteLine($"Copy files strategy is being rolledback.");

            if(copiedFiles.Count == 0) return Task.CompletedTask;           

            foreach (string copiedFile in copiedFiles)
            {
                try
                {
                    File.Delete(copiedFile);
                    Console.WriteLine($"File deleted: {copiedFile}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting file {copiedFile}: {ex.Message}");                    
                }
            }
            return Task.CompletedTask;
        }

        private void CopyFiles(string sourceFolderPath, string destinationFolderPath)
        {
            if (!Directory.Exists(destinationFolderPath))
            {
                Directory.CreateDirectory(destinationFolderPath);
            }

            foreach (string filePath in Directory.GetFiles(sourceFolderPath))
            {
                string fileName = Path.GetFileName(filePath);
                string destinationFilePath = Path.Combine(destinationFolderPath, fileName);
                File.Copy(filePath, destinationFilePath, true);
                copiedFiles.Add(destinationFilePath);
                Console.WriteLine($"File copied: {filePath} -> {destinationFilePath}"); 
            }
        }
    }
}