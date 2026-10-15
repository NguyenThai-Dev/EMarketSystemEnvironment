namespace AIAssistantService
{
    public interface IApiKeyProvider
    {
        string GetApiKey();
        void MarkAsFailed(string key);
    }

    public class GeminiApiKeyProvider : IApiKeyProvider
    {
        private readonly List<string> _keys;
        private int _currentIndex = 0;
        private readonly object _lock = new();

        public GeminiApiKeyProvider(IConfiguration config)
        {
            _keys = new List<string>();

            // 1. Kiểm tra cấu hình danh sách ApiKeys trong GeminiSettings
            var geminiKeys = config.GetSection("GeminiSettings:ApiKeys").Get<List<string>>();
            if (geminiKeys != null && geminiKeys.Count > 0)
            {
                _keys.AddRange(geminiKeys.Where(k => !string.IsNullOrWhiteSpace(k)));
            }

            // 2. Kiểm tra key đơn lẻ (ApiKey hoặc GoogleAIStudioApiKey từ GeminiSettings / GroqSettings)
            var singleKey = config["GeminiSettings:ApiKey"]
                         ?? config["GeminiSettings:GoogleAIStudioApiKey"]
                         ?? config["GroqSettings:GoogleAIStudioApiKey"]
                         ?? config["GoogleAIStudioApiKey"]
                         ?? config["GEMINI_API_KEY"];

            if (!string.IsNullOrWhiteSpace(singleKey) && !_keys.Contains(singleKey))
            {
                _keys.Add(singleKey);
            }

            // 3. Dự phòng danh sách ApiKeys cũ nếu có
            if (_keys.Count == 0)
            {
                var legacyKeys = config.GetSection("GroqSettings:ApiKeys").Get<List<string>>();
                if (legacyKeys != null && legacyKeys.Count > 0)
                {
                    _keys.AddRange(legacyKeys.Where(k => !string.IsNullOrWhiteSpace(k)));
                }
            }
        }

        public string GetApiKey()
        {
            lock (_lock)
            {
                if (_keys.Count == 0)
                {
                    throw new Exception("Không có Gemini API Key nào được cấu hình trong GeminiSettings hoặc User Secrets!");
                }
                var key = _keys[_currentIndex];
                _currentIndex = (_currentIndex + 1) % _keys.Count;
                return key;
            }
        }

        public void MarkAsFailed(string key)
        {
            lock (_lock)
            {
                var masked = key.Length > 10 ? key.Substring(0, 10) : key;
                Console.WriteLine($"[WARNING]: Gemini API Key {masked}... gặp lỗi!");
            }
        }
    }

    // Alias hỗ trợ tương thích ngược
    public class GroqApiKeyProvider : GeminiApiKeyProvider
    {
        public GroqApiKeyProvider(IConfiguration config) : base(config) { }
    }
}
