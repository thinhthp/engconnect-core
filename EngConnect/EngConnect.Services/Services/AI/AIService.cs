using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EngConnect.Services.Services.AI
{
    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _geminiApiKey;

        public AIService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _geminiApiKey = configuration["Gemini:ApiKey"]!;
        }

        public async Task<string> ChatAsync(string message)
        {
            var systemInstruction = "You are an expert English language assistant for the EngConnect platform. Your role is to help users learn and practice English. Please only respond to questions related to English language learning, grammar, vocabulary, pronunciation, and conversation practice. If a question is not related to learning English, politely decline to answer and state that your purpose is to assist with English learning. Ensure your responses are accurate, clear, and helpful for an English learner. Try to match your language with the users languages. Answer in plain text.";

            var apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_geminiApiKey}";

            var payload = new
            {
                systemInstruction = new { parts = new[] { new { text = systemInstruction } } },
                contents = new[]
                {
                    new { parts = new[] { new { text = message } } }
                }
            };

            var response = await _httpClient.PostAsJsonAsync(apiUrl, payload);

            if (!response.IsSuccessStatusCode)
            {
                return "Sorry, I'm having trouble connecting to the AI service right now.";
            }

            var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();

            try
            {
                var text = jsonResponse.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();
                return text!;
            }
            catch
            {
                return "Sorry, I couldn't process the response from the AI service.";
            }
        }

        public async Task<string> GetHintAsync(string question)
        {
            var systemInstruction = "You are an English tutor on the EngConnect platform. A user will provide an English question, and your task is to provide a helpful hint without giving away the direct answer. The hint should guide the user toward figuring out the answer on their own. For example, if the question is 'What is the past tense of go?', a good hint would be 'It's an irregular verb that starts with the letter w.' Do not answer the question directly. Try to match your language with the users languages. Answer in plain text.";

            var apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_geminiApiKey}";

            var payload = new
            {
                systemInstruction = new { parts = new[] { new { text = systemInstruction } } },
                contents = new[]
                {
                    new { parts = new[] { new { text = question } } }
                }
            };

            var response = await _httpClient.PostAsJsonAsync(apiUrl, payload);

            if (!response.IsSuccessStatusCode)
            {
                return "Sorry, I'm having trouble connecting to the AI service right now.";
            }

            var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();

            try
            {
                var text = jsonResponse.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();
                return text!;
            }
            catch
            {
                return "Sorry, I couldn't process the response from the AI service.";
            }
        }
    }
}