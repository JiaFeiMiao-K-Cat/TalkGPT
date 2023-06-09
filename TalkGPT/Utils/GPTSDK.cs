using Azure;
using Azure.AI.OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace TalkGPT.Utils
{
    public class GPTSDK
    {
        private SDKSettingsService _settings { get; set; }
        OpenAIClient Client { get; set; }
        public IList<ChatMessage> Messages { get; set; } = new List<ChatMessage>()
        {
            new ChatMessage(ChatRole.System, @"You are an AI assistant that helps people find information.")
            #if DEBUG
            , new ChatMessage(ChatRole.User, "如何画一只猫娘"),
            new ChatMessage(ChatRole.Assistant, "要画一只猫娘，首先需要了解猫娘的特征和风格。猫娘通常具有人类和猫科动物的特征，例如猫耳朵、尾巴、爪子和柔软的毛发。\n\n以下是一些步骤，以帮助你画一只猫娘：\n\n1. 画出猫娘的轮廓：画出她的头、身体、四肢和尾巴的形状，可以参考一些猫娘的图片作为参考。\n\n2. 画出猫娘的面部特征：画出她的眼睛、鼻子和嘴巴的位置和大小。记得加上猫娘典型的猫耳朵。\n\n3. 画出猫娘的身体特征：画出她的爪子和柔软的毛发。可以参考一些猫娘的图片，看看如何表现出她们的毛发和爪子。\n\n4. 着色：根据你的喜好和参考的图片来为猫娘着色，使她看起来更加生动。\n\n记住，这只是一个简单的步骤指南。如果你想画出更复杂和详细的猫娘，需要更多的练习和实践。")
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
            Messages.Add(new ChatMessage(ChatRole.System, @"You are an AI assistant that helps people find information."));
        }

        public async Task<string> Answer(string prompt)
        {
            Messages.Add(new ChatMessage(ChatRole.User, prompt));
            //Messages.Add(new ChatMessage(ChatRole.Assistant, prompt));
            //return prompt;
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
            Messages.Add(new ChatMessage(ChatRole.Assistant, answer));
#if DEBUG
            return JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            });
#else
            return response.Value.Choices[0].Message.Content;
#endif
        }
    }
    public class ChatArchive
    {
        public string Name { get; set; }
        public IList<ChatMessage> Context { get; set; }
    }
}
