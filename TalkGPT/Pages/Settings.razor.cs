using Azure.AI.OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkGPT.Utils;

namespace TalkGPT.Pages
{
    public partial class Settings
    {
        void SaveAsync()
        {
            _sdkSettings.SaveSettings();
            _gptSDK.RefreshSettings();
        }
    }
}
