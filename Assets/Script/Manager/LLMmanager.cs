using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LlmManager : MonoBehaviour
{
    public Text Send;
    public Text Show;
    // Start is called before the first frame update
    public async void SendFromUiText()
    {
        Show.text = await GetAnswer(Send.text);
    }
    [Button("回答")]
    private static async Task<string> GetAnswer(string tips)
    {
        var baseUrl = "https://dashscope.aliyuncs.com/compatible-mode/v1/chat/completions";
        var apiKey = "sk-92e7d73f9884483a92a6bf01fd9965b4";
        //string tips = $"";
        Debug.Log("提示词:\n" + tips);
        var requestBody = new
        {
            model = "qwen-omni-turbo",
            messages = new object[]
            {
            new
            {
                role = "system",
                content = new[]
                {
                    new { type = "text", text = "你要跟我进行一个角色扮演，你叫砂金，是星际和平公司的一名高管，你信仰存护命途，你说话轻佻，打扮浮夸，爱好赌博，当其实这些都是你的掩饰，你已经家破人亡，仇人是你的同事施耐德" }
                }
            }
            ,
                    new
                    {
                        role = "user",
                        content = new object[]
                        {
                            new { type = "text", text = tips }
                        }
                    }
        },
            modalities = new[] { "text" },
            stream = true,
            stream_options = new
            {
                include_usage = true
            }
        };

        var json = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        try
        {
            var response = await client.PostAsync(baseUrl, content);
            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var reader = new StreamReader(stream))
            {
                string line;
                var fullContent = new StringBuilder();
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (line.StartsWith("data: "))
                    {
                        line = line.Substring(6); // Remove "data: " prefix
                        if (line == "[DONE]")
                        {
                            break;
                        }

                        LmmInfo lmmInfo = line.ToObject<LmmInfo>();
                        if (lmmInfo.choices != null&& lmmInfo.choices.Any()&& lmmInfo.choices[0].delta.content != null)
                        {
                            string result = lmmInfo.choices[0].delta.content;
                            //Debug.Log("分析结果:\n" + result);
                            fullContent.Append(result);
                        }
                        //dynamic chunk = JsonConvert.DeserializeObject<dynamic>(line);
                        //if (chunk.choices != null && chunk.choices.Count > 0 && chunk.choices[0].delta.content != null)
                        //{
                        //    fullContent.Append(chunk.choices[0].delta.content);
                        //}
                    }
                }
                Debug.Log("分析结果:\n" + fullContent.ToString());
                Debug.Log("__________________________________________________________");
                return $"{fullContent.ToString()}";
            }
        }
        catch (Exception)
        {

            Debug.Log("分析超时");
            Debug.Log("__________________________________________________________");
            return "";
        }
    }
}

public class LmmInfo
{
    public Choice[] choices { get; set; }
    public string _object { get; set; }
    public object usage { get; set; }
    public int created { get; set; }
    public object system_fingerprint { get; set; }
    public string model { get; set; }
    public string id { get; set; }
}

public class Choice
{
    public Delta delta { get; set; }
    public object finish_reason { get; set; }
    public int index { get; set; }
    public object logprobs { get; set; }
}

public class Delta
{
    public string content { get; set; }
}
