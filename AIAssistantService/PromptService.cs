namespace AIAssistantService
{
    public interface IPromptService
    {
        string GetPrompt(string fileName);
    }

    public class PromptService : IPromptService
    {
        private readonly string _promptPath;
        public PromptService(IWebHostEnvironment env)
        {
            _promptPath = Path.Combine(env.ContentRootPath, "Prompts");
        }

        public string GetPrompt(string fileName)
        {
            var path = Path.Combine(_promptPath, fileName + ".md");
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Không tìm thấy file prompt tại: {Path.GetFullPath(path)}");
            }
            return File.ReadAllText(path);
        }
    }
}
