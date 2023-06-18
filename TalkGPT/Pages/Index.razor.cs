using AntDesign;
using AntDesign.Charts;
using AntDesign.Core.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TalkGPT.Utils;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace TalkGPT.Pages
{
    public partial class Index
    {
        public string RecognitionText { get; set; }

        public string ProblemText { get; set; }
        public string AnswerText { get; set; }

        bool loading = false;

        /*public Index(ISpeechToText speechToText)
        {
            this.speechToText = speechToText;
        }*/

        /*
        //private readonly ISpeechToText speechToText;

        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        
        void Click()
        {
            cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            Listen(cancellationToken);
        }

        void Stop()
        {
            cancellationTokenSource?.Cancel();
        }

        public async void Listen(CancellationToken cancellationToken)
        {
            var isAuthorized = await _speechToText.RequestPermissions();
            if (isAuthorized)
            {
                try
                {
                    RecognitionText = await _speechToText.Listen(CultureInfo.GetCultureInfo("zh-cn"),
                        new Progress<string>(partialText =>
                        {
                            if (DeviceInfo.Platform == DevicePlatform.Android)
                            {
                                RecognitionText = partialText;
                            }
                            else
                            {
                                RecognitionText += partialText + " ";
                            }
                        }), cancellationToken);
                }
                catch (Exception ex)
                {
                    await _notice.Open(new NotificationConfig()
                    {
                        Message = "Error",
                        Description = ex.Message,
                        NotificationType = NotificationType.Error
                    });
                }
            }
            else
            {
                await _notice.Open(new NotificationConfig()
                {
                    Message = "Permission Error",
                    Description = "No microphone access",
                    NotificationType = NotificationType.Error
                });
            }
        }*/
        async void GetAnswer()
        {
            loading = true;
            StateHasChanged();
            try
            {
                AnswerText = await _gptSDK.Answer(ProblemText);
                if (_sdkSettings.SDKSettings.EnableTTS)
                {
                    _speechSDK.SpeechAsync(AnswerText);
                }
            }
            catch (Exception ex)
            {
                await _notice.Open(new NotificationConfig()
                {
                    Message = "Error",
                    Description = ex.Message,
                    NotificationType = NotificationType.Error
                });
            }
            ProblemText = string.Empty;
            loading = false;
            StateHasChanged();
        }
        // TODO: Save current chat to json file
    }
}
