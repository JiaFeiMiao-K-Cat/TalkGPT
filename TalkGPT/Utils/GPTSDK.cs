using Azure;
using Azure.AI.OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace TalkGPT.Utils
{
    public class GPTSDK
    {
        private SDKSettingsService _settings { get; set; }
        OpenAIClient Client { get; set; }
        public IList<NewChatMessage> Messages { get; set; } = new List<NewChatMessage>()
        {
            new NewChatMessage(ChatRole.System, @"You are an AI assistant that helps people find information.")
            #if DEBUG
            , new NewChatMessage(ChatRole.User, "如何画一只猫娘"),
            new NewChatMessage(ChatRole.Assistant, "要画一只猫娘，首先需要了解猫娘的特征和风格。猫娘通常具有人类和猫科动物的特征，例如猫耳朵、尾巴、爪子和柔软的毛发。\n\n以下是一些步骤，以帮助你画一只猫娘：\n\n1. 画出猫娘的轮廓：画出她的头、身体、四肢和尾巴的形状，可以参考一些猫娘的图片作为参考。\n\n2. 画出猫娘的面部特征：画出她的眼睛、鼻子和嘴巴的位置和大小。记得加上猫娘典型的猫耳朵。\n\n3. 画出猫娘的身体特征：画出她的爪子和柔软的毛发。可以参考一些猫娘的图片，看看如何表现出她们的毛发和爪子。\n\n4. 着色：根据你的喜好和参考的图片来为猫娘着色，使她看起来更加生动。\n\n记住，这只是一个简单的步骤指南。如果你想画出更复杂和详细的猫娘，需要更多的练习和实践。")
            #endif
        };

        public IList<ChatArchive> Archives { get; set; }

        public GPTSDK(SDKSettingsService settings)
        {
            _settings = settings;
            Client = new(new Uri(_settings.SDKSettings.GPTEndpoint), new AzureKeyCredential(_settings.SDKSettings.GPTKey));
            Archives = FileHelper.ReadJsonFile<IList<ChatArchive>>("archives.json");
        }

        public void SaveArchives()
        {
            FileHelper.WriteJsonFile("archives.json", Archives);
        }

        public void RefreshSettings()
        {
            Client = new(new Uri(_settings.SDKSettings.GPTEndpoint), new AzureKeyCredential(_settings.SDKSettings.GPTKey));
            ResetMeaages();
        }
        public void ResetMeaages()
        {
            Messages.Clear();
            Messages.Add(new NewChatMessage(ChatRole.System, @"You are an AI assistant that helps people find information."));
        }

        public async Task<string> Answer(string prompt)
        {
            Messages.Add(new NewChatMessage(ChatRole.User, prompt));
#if DEBUG
            Messages.Add(new NewChatMessage(ChatRole.Assistant, prompt));
            return prompt;
#endif
            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                Temperature = _settings.SDKSettings.Temperature,
                MaxTokens = _settings.SDKSettings.MaxTokens,
                NucleusSamplingFactor = _settings.SDKSettings.NucleusSamplingFactor,
                FrequencyPenalty = _settings.SDKSettings.FrequencyPenalty,
                PresencePenalty = _settings.SDKSettings.PresencePenalty
            };
            foreach (ChatMessage message in Messages)
            {
                chatCompletionsOptions.Messages.Add(message);
            }
            Response<ChatCompletions> response =
                await Client.GetChatCompletionsAsync(
                    deploymentOrModelName: "JiaFeiChat",
                    chatCompletionsOptions
                );
            string answer = response.Value.Choices[0].Message.Content;
            Messages.Add(new NewChatMessage(ChatRole.Assistant, answer));

            /*return JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            });*/

            return response.Value.Choices[0].Message.Content;
        }
    }
    public class ChatArchive
    {
        public string Name { get; set; }
        public IList<ChatMessage> Context { get; set; }
    }

    public class NewChatMessage : ChatMessage
    {
        [JsonIgnore]
        public string NewContent
        {
            get
            {
                return Content;
            }
            set 
            { 
                return; 
            }
        }

        public DateTime CreateTime { get; set; }

        public NewChatMessage(ChatRole role, string content) : base(role, content)
        {
            CreateTime = DateTime.UtcNow;
        }
    }
}
