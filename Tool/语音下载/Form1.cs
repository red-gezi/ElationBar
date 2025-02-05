using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace 语音下载
{
    public partial class Form1 : Form
    {
        List<Voice> voices = new();
        public Form1()
        {
            InitializeComponent();
        }
        private void Btn_OpenWeb_Click(object sender, EventArgs e)
        {
            string url = "https://bbs.mihoyo.com/sr/wiki/channel/map/17/18?bbs_presentation_style=no_header";
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            //Process.Start("https://bbs.mihoyo.com/sr/wiki/channel/map/17/18?bbs_presentation_style=no_header");
        }
        private void Btn_Filiter_Click(object sender, EventArgs e)
        {
            string pattern = "<source src=\"(.*?)\">.*<span class=\"obc-tmpl-character__voice-content\">([\\s\\S]*?)</span>";
            MatchCollection matches = Regex.Matches(Text_Web.Text, pattern);
            voices.Clear();
            foreach (Match match in matches)
            {
                if (match.Groups.Count == 3)
                {
                    string audioSrc = match.Groups[1].Value.Trim();
                    string chineseText = match.Groups[2].Value.Trim();
                    voices.Add(new Voice(audioSrc, chineseText));
                }
            }
            VoiceItem.Items.Clear();
            voices.ForEach(voice => VoiceItem.Items.Add(voice.name));
        }
        private void Btn_Download_Click(object sender, EventArgs e)
        {
            foreach (int index in VoiceItem.CheckedIndices)
            {
                // index 是选中的项的索引
                Console.WriteLine("选中的索引: " + index);
                voices[index].DownLoad(CharaTag.Text);
            }
            MessageBox.Show("全部下完！");
        }
        private void OpenDire_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo($"{CharaTag.Text}\\Voice") { UseShellExecute = true });

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
    class Voice
    {
        public string name;
        public string url;

        public Voice(string audioSrc, string chineseText)
        {
            url = audioSrc;
            name = chineseText;
        }

        public async void DownLoad(string tag)
        {
            Directory.CreateDirectory($"{tag}/Voice");

            using (HttpClient client = new HttpClient())
            {
                // 发送GET请求以获取文件内容
                HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                // 读取响应内容并将其写入文件
                using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                              fileStream = new FileStream($"{tag}/Voice/V_{name}.wav", FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                {
                    await contentStream.CopyToAsync(fileStream);
                }
                Console.WriteLine($"{name}下载完成");
            }
        }
    }
}
