using Azure.AI.OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using TalkGPT.Utils;

namespace TalkGPT.Pages
{
    public partial class Settings
    {
        private string JsonText { get 
            {
                return JsonSerializer.Serialize(_sdkSettings.SDKSettings, new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                    }
                );
            } 
            set
            {
                _sdkSettings.SDKSettings = JsonSerializer.Deserialize<SDKSettings>(value);
            }
        }
        void SaveAsync()
        {
            // change order for illegal settings will make program crash but not save to file
            _gptSDK.RefreshSettings();
            _sdkSettings.SaveSettings();
        }
    }
}
