using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace TalkGPT.Utils
{
    public class AzureSpeechSDK
    {
        public SDKSettingsService _settings { get; set; }

        public AzureSpeechSDK(SDKSettingsService settings)
        {
            _settings = settings;
        }

        public async Task<IEnumerable<string>> GetVoiceNamesAsync()
        {
            var speechConfig = SpeechConfig.FromSubscription(_settings.SDKSettings.SpeechKey, _settings.SDKSettings.SpeechRegion);
            using var speechSynthesizer = new SpeechSynthesizer(speechConfig);
            return (await speechSynthesizer.GetVoicesAsync()).Voices.Select(e => e.Name);
        }

        public async void SpeechAsync(string text)
        {
            var speechConfig = SpeechConfig.FromSubscription(_settings.SDKSettings.SpeechKey, _settings.SDKSettings.SpeechRegion);
            speechConfig.SpeechSynthesisVoiceName = _settings.SDKSettings.SpeechSynthesisVoiceName;
            using var speechSynthesizer = new SpeechSynthesizer(speechConfig);
            await speechSynthesizer.SpeakTextAsync(text);
        }
    }
}
