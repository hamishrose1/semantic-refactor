using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OllamaSharp;
using OllamaSharp.Models.Chat;

namespace SemanticRefactor
{
    /// <summary>
    /// Custom ollama chat completion service. Allows for using locally deployed models from ollama.
    /// </summary>
    public class CustomChatCompletionService : IChatCompletionService
    {
        public string ModelUrl { get; set; }

        public string ModelName { get; set; }

        public IReadOnlyDictionary<string, object?> Attributes => throw new NotImplementedException();

        private HttpClient _httpClient;
        private HttpClient HttpClient
        {
            get
            {
                if (_httpClient is null)
                {
                    _httpClient = new HttpClient()
                    {
                        BaseAddress = new Uri(ModelUrl),
                        Timeout = TimeSpan.FromMinutes(5)
                    };
                }

                return _httpClient;
            }
        }

        public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
        {
            var ollama = new OllamaApiClient(HttpClient, ModelName);

            var chat = new Chat(ollama, _ => { });


            // iterate though chatHistory Messages
            foreach (var message in chatHistory)
            {
                if (message.Role == AuthorRole.System)
                {
                    await chat.SendAs(ChatRole.System, message.Content);
                    continue;
                }
            }

            var lastMessage = chatHistory.LastOrDefault();

            string question = lastMessage.Content;
            var chatResponse = "";
            var history = (await chat.Send(question, CancellationToken.None)).ToArray();

            var last = history.Last();
            chatResponse = last.Content;

            chatHistory.AddAssistantMessage(chatResponse);

            return chatHistory;
        }

        public IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

    }
}