using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkGPT.Utils
{
    public class SDKSettingsService
    {
        private SDKSettings _settings;
        public SDKSettings SDKSettings { 
            get 
            { 
                if (_settings == null)
                {
                    _settings = new SDKSettings();
                }
                return _settings;
            }
            set
            {
                _settings = value;
            }
        }

        public SDKSettingsService()
        {
            _settings = FileHelper.ReadJsonFile<SDKSettings>("settings.json");
        }

        public void SaveSettings()
        {
            FileHelper.WriteJsonFile("settings.json", _settings);
        }
    }
    public class SDKSettings
    {
        #region GPT
        public string GPTEndpoint { get; set; } = "https://REPLACE.openai.azure.com/";
        public string GPTKey { get; set; } = "REPLACE";
        public string GPTEngine { get; set; } = "REPLACE";
        public float Temperature { get; set; } = 0.7f;
        public int MaxTokens { get; set; } = 800;
        public float NucleusSamplingFactor { get; set; } = 0.95f;
        public int FrequencyPenalty { get; set; } = 0;
        public int PresencePenalty { get; set; } = 0;
        #endregion
        #region TTS
        public bool EnableTTS { get; set; } = true;
        public string SpeechKey { get; set; } = "REPLACE";
        public string SpeechRegion { get; set; } = "REPLACE";
        public string SpeechSynthesisVoiceName { get; set; } = "REPLACE";
        #endregion
    }
}
