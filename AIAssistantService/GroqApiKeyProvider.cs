namespace AIAssistantService
{
    public interface IApiKeyProvider
    {
        string GetApiKey();
        void MarkAsFailed(string key);
    }

    public class GroqApiKeyProvider : IApiKeyProvider
    {
        private readonly List<string> _keys;
        private int _currentIndex = 0;
        private readonly object _lock = new();

        public GroqApiKeyProvider(IConfiguration config)
        {
            _keys = config.GetSection("GroqSettings:ApiKeys").Get<List<string>>() ?? new List<string>();
        }

        public string GetApiKey()
        {
            lock (_lock)
            {
                if (_keys.Count == 0) throw new Exception("Không có API Key nào!");
                var key = _keys[_currentIndex];
                _currentIndex = (_currentIndex + 1) % _keys.Count;
                return key;
            }
        }

        public void MarkAsFailed(string key)
        {
            lock (_lock)
            {
                Console.WriteLine($"[WARNING]: API Key {key.Substring(0, 10)}... gặp lỗi!");
            }
        }
    }
}
