using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Application.FileRepository
{
    public interface IFileRepository
    {
        public Task WriteToJsonFile(FileRepositoryContent objectToWrite);
        public Task<FileRepositoryContent> ReadFromJsonFile();
    }
    
    public class FileRepository : IFileRepository
    {
        private readonly string _filePath;

        public FileRepository()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            _filePath = Path.Combine(currentDirectory, "file_repo.json");
        }
        
        public async Task WriteToJsonFile(FileRepositoryContent objectToWrite)
        {
            TextWriter writer = null;
            try
            {
                var contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite);
                writer = new StreamWriter(_filePath, false);
                await writer.WriteAsync(contentsToWriteToFile);
            }
            finally
            {
                writer?.Close();
            }
        }

        public async Task<FileRepositoryContent> ReadFromJsonFile()
        {
            TextReader reader = null;
            try
            {
                reader = new StreamReader(_filePath);
                var fileContents = await reader.ReadToEndAsync();
                return JsonConvert.DeserializeObject<FileRepositoryContent>(fileContents);
            }
            finally
            {
                reader?.Close();
            }
        }
    }
}