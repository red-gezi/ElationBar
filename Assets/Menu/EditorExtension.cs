#if UNITY_EDITOR
//using Microsoft.AspNetCore.SignalR.Client;
using Best.SignalR.Encoders;
using Best.SignalR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using Concentus;
//using Concentus.Structures;
namespace Hotfix
{
    public class EditorExtension : MonoBehaviour
    {
        //服务器路径
        static string ServerIP => $"{AssetBundleUpdateManager.ServerIP}:233";
        static string VersionsServerIP { get; } = @"http://106.15.38.165:49514";
        //游戏热更资源放置路径
        static string HotfixAssetPath { get; } = @"Assets\HotFixResources";
        //游戏AB包资源路径
        static string ABUpLoadPath { get; } = @"AB_Upload";
        const string projectName = "ElationBar";
        //该集合中的文件将以耳机子文件夹名称打包，否则以主文件名打包
        static List<string> subDireList = new() { "Chara" };
        static string CommandPassword
        {
            get
            {
                if (!File.Exists("password.txt"))
                {
                    File.WriteAllLines("password.txt", new string[] { "1234" });
                }
                return File.ReadAllLines("password.txt")[0];
            }
        }
        /////////////////////////////////////////////////////////////////打开///////////////////////////////////////////////////////////////////////////////////////////
        [MenuItem(projectName + "/打开/打开服务端", false, 1)]
        static void OpenServer() => System.Diagnostics.Process.Start(@"Server\HotfixServer\bin\Debug\net6.0\HotFixServer.exe");
        [MenuItem(projectName + "/打开/打开游戏客户端", false, 2)]
        static void OpenClient() => System.Diagnostics.Process.Start(@"Pc\TouHouMachineLearningSummary.exe");
        [MenuItem(projectName + "/打开/打开配置文件", false, 2)]
        static void OpenConfig() => System.Diagnostics.Process.Start(@"Assets\Resources\HotFix.txt");
        
        
        //[MenuItem(projectName + "/打开/打开数据表格（云端）", false, 50)]
        //static void OpenCloudXls() => System.Diagnostics.Process.Start(@"https://kdocs.cn/l/cfS6F51QxqGd");
        //[MenuItem(projectName + "/打开/打开数据表格", false, 51)]
        //static void OpenXls() => System.Diagnostics.Process.Start(@"Assets\GameResources\GameData\GameData.xlsx");
        //[MenuItem(projectName + "/打开/打开表格数据实时同步工具", false, 52)]
        //static void UpdateXls() => System.Diagnostics.Process.Start(@"OtherSolution\xls检测更新\bin\Debug\net6.0\xls检测更新.exe");

        /////////////////////////////////////////////////////////////////场景///////////////////////////////////////////////////////////////////////////////////////////
        #region 场景
        [MenuItem(projectName + "/Scene/切换为热更场景", priority = 151)]
        static void LoadHotFixScene() => System.Diagnostics.Process.Start(@"Assets\Scenes\0_Menu.unity");
        [MenuItem(projectName + "/Scene/切换为载入场景", priority = 152)]
        static void LoadLoginScene() => System.Diagnostics.Process.Start(@"Assets\Scenes\1_Load.unity");
        [MenuItem(projectName + "/Scene/切换为对战场景", priority = 153)]
        static void LoaBattleScene() => System.Diagnostics.Process.Start(@"Assets\Scenes\2_Game.unity");
        #endregion
        /////////////////////////////////////////////////////////////////人物新增///////////////////////////////////////////////////////////////////////////////////////////
        #region 人物新增
        [MenuItem(projectName + "/人物新增/新增人物列表", priority = 10)]
        static void AddCharaEnum() => System.Diagnostics.Process.Start(@"Assets\Script\Enum\Chara.cs");
        [MenuItem(projectName + "/人物新增/模型下载", priority = 11)]
        static void DownloadChara() => System.Diagnostics.Process.Start(@"https://www.aplaybox.com/u/516827875/model");
        [MenuItem(projectName + "/人物新增/语音下载", priority = 12)]
        static void OpenVoiceTools() => System.Diagnostics.Process.Start(@"Tool\语音下载\bin\Debug\net6.0-windows\语音下载.exe");

        [MenuItem(projectName + "/人物新增/配置表情数据", priority = 13)]
        static void ConfigFaceData() => System.Diagnostics.Process.Start(@"Assets\Script\Data\GameData.cs");
        [MenuItem(projectName + "/人物新增/配置动作数据", priority = 14)]
        static void ConfigMotionData() => System.Diagnostics.Process.Start(@"Assets\Script\Data\GameData.cs");
        #endregion
        /////////////////////////////////////////////////////////////////项目配置///////////////////////////////////////////////////////////////////////////////////////////
        [MenuItem(projectName + "/Config/切换当前程序集使用线上版本（确保debug完要切回来）", priority = 1)]
        static void ChangeToOnlineCardScript()
        {
            var targetFile = new FileInfo("Assets\\Script\\9_MixedScene\\CardSpace\\GameCard.asmdef1");
            if (targetFile.Exists)
            {
                targetFile.MoveTo("Assets\\Script\\9_MixedScene\\CardSpace\\GameCard.asmdef");
                AssetDatabase.Refresh();
            }
        }
        [MenuItem(projectName + "/Config/切换当前程序集使用本地版本（可以查看更多debug细节）", priority = 2)]
        static void ChangeToLoaclCardScript()
        {
            var targetFile = new FileInfo("Assets\\Script\\9_MixedScene\\CardSpace\\GameCard.asmdef");
            if (targetFile.Exists)
            {
                targetFile.MoveTo("Assets\\Script\\9_MixedScene\\CardSpace\\GameCard.asmdef1");
                AssetDatabase.Refresh();
            }
        }
        [MenuItem(projectName + "/Config/切换当前链接网络服务端", priority = 51)]
        static void ChangeToOnlineServer()
        {
            AssetBundleUpdateManager.isLocalMode = false;
        }
        [MenuItem(projectName + "/Config/切换当前链接本地服务端", priority = 52)]
        static void ChangeToLocalServer()
        {
            AssetBundleUpdateManager.isLocalMode = true;
        }
        /////////////////////////////////////////////////////////////////发布（服务端）///////////////////////////////////////////////////////////////////////////////////////////
        [MenuItem(projectName + "/Public/发布当前服务器到正式环境", false, 0)]
        static async void UpdateServer()
        {
            //var VersionsHub = new HubConnectionBuilder().WithUrl($"{VersionsServerIP}/VersionsHub").Build();
            //await VersionsHub.StartAsync();
            //var result = await VersionsHub.InvokeAsync<string>("UpdateServer", File.ReadAllBytes(@"Server\HotfixServer\bin\Debug\net6.0\HotFixServer.dll"), CommandPassword);
            //Debug.LogWarning("上传结果" + result);
            //await VersionsHub.StopAsync();

            var VersionsHub = new HubConnection(new Uri($"{VersionsServerIP}/VersionsHub"), new JsonProtocol(new LitJsonEncoder()));
            await VersionsHub.ConnectAsync();
            var fileData = File.ReadAllBytes(@"Server\HotfixServer\bin\Debug\net6.0\HotFixServer.dll");
            var result = await VersionsHub.InvokeAsync<string>("UpdateServer", fileData, CommandPassword);
            Debug.LogWarning("上传结果" + result);
            await VersionsHub.CloseAsync();
        }
        /////////////////////////////////////////////////////////////////发布程序集版本///////////////////////////////////////////////////////////////////////////////////////////
        [MenuItem(projectName + "/Public/发布代码版本到测试版", false, 100)]
        static void UpdateCardToTest() => UpdateCard("Test");

        [MenuItem(projectName + "/Public/发布代码版本到正式版", false, 101)]
        static void UpdateCardToRelease() => UpdateCard("Release");
        private static void UpdateCard(string tag)
        {
            var gameCardAssembly = new DirectoryInfo(@"Library\ScriptAssemblies").GetFiles("GameCard*.dll").FirstOrDefault();

            if (gameCardAssembly != null)
            {
                //_ = Command.NetCommand.UploadCardConfigsAsync(cardConfig, drawAbleList, CommandPassword);
            }
            else
            {
                Debug.LogError("检索不到程序集dll文件");
            }
        }
        /////////////////////////////////////////////////////////////////发布热更新资源///////////////////////////////////////////////////////////////////////////////////////////
        [MenuItem(projectName + "/Public/清空AB包标签", priority = 150)]
        static void ClearABTags() => ClearAssetBundlesTags();
        [MenuItem(projectName + "/Public/生成测试AB包本地AB包资源", priority = 150)]
        static void BuildAssetBundlesToEditor()
        {
            AddAssetBundlesTags();
            string outputPath = Directory.GetCurrentDirectory() + $@"\{ABUpLoadPath}\PC_Test";
            Directory.CreateDirectory(outputPath);
            // 优化构建选项
            BuildAssetBundleOptions options = BuildAssetBundleOptions.UncompressedAssetBundle;
            BuildPipeline.BuildAssetBundles(outputPath, options, BuildTarget.StandaloneWindows64);
            Debug.LogWarning("打包完成");
        }

        [MenuItem(projectName + "/Public/发布电脑游戏热更资源为测试版", priority = 151)]
        static void BuildDAssetBundlesToTest() => BuildAssetBundles("PC_Test");
        [MenuItem(projectName + "/Public/发布电脑游戏热更资源为正式版", priority = 152)]
        static void BuildAssetBundlesToRelease() => BuildAssetBundles("PC_Release");
        //[MenuItem(projectName + "/Public/发布安卓端游戏热更资源为正式版", priority = 153)]
        //static void BuildAssetBundlesToAndroid() => BuildAssetBundles("Android");
        private static void ClearAssetBundlesTags()
        {
            //清空标签
            new DirectoryInfo(HotfixAssetPath).GetDirectories().ToList()
                .ForEach(dire =>
                {
                    dire.GetFiles("*.*", SearchOption.AllDirectories)
                            .Where(file => file.Extension != ".meta" && file.Extension != ".cs")
                            .ToList()
                            .ForEach(file =>
                            {
                                string path = file.FullName.Replace(Directory.GetCurrentDirectory() + @"\", "");
                                try
                                {
                                    AssetImporter.GetAtPath(path).assetBundleName = $"";
                                }
                                catch (Exception e)
                                {
                                    Debug.LogError(path + e.Message);
                                }
                            });
                });
        }
        //添加AB包标签
        private static void AddAssetBundlesTags()
        {
            var ignoreExtensions = new List<string>() { ".meta", ".hlsl", ".asset", ".pmx" };
            //打标签
            new DirectoryInfo(HotfixAssetPath)
                .GetDirectories()
                .ToList()
                .ForEach(dire =>
                {
                    if (subDireList.Contains(dire.Name))
                    {
                        dire.GetDirectories()
                        .ToList()
                        .ForEach(subDire =>
                        {
                            subDire.GetFiles("*.*", SearchOption.AllDirectories)
                            .Where(file => !ignoreExtensions.Contains(file.Extension))
                            .ToList()
                            .ForEach(file =>
                            {
                                string path = file.FullName.Replace(Directory.GetCurrentDirectory() + @"\", "");
                                AssetImporter.GetAtPath(path).assetBundleName = $"{subDire.Name}.gezi";
                            });
                        });
                    }
                    else
                    {
                        dire.GetFiles("*.*", SearchOption.AllDirectories)
                            .Where(file => !ignoreExtensions.Contains(file.Extension))
                            .ToList()
                            .ForEach(file =>
                            {
                                string path = file.FullName.Replace(Directory.GetCurrentDirectory() + @"\", "");
                                AssetImporter.GetAtPath(path).assetBundleName = $"{dire.Name}.gezi";
                            });
                    }

                });
            Debug.LogWarning("标签修改完毕，开始打包");


        }
        //打包AB包并上传
        private static async void BuildAssetBundles(string tag)
        {
            //打标签
            AddAssetBundlesTags();
            //将AB打包到AB文件夹下，并上传
            string outputPath = Directory.GetCurrentDirectory() + $@"\{ABUpLoadPath}\{tag}";
            Directory.CreateDirectory(outputPath);

            BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, tag == "Android" ? BuildTarget.Android : BuildTarget.StandaloneWindows64);
            Debug.LogWarning($"{tag}打包完毕");
            Debug.LogWarning("开始生成MD5值校验文件");

            //创建md5文件
            MD5 md5 = new MD5CryptoServiceProvider();
            Dictionary<string, byte[]> MD5s = new();
            new DirectoryInfo(outputPath).GetFiles("*.*").ToList().ForEach(file =>
            {
                //只上传代码和gezi文件
                if (file.Extension == ".gezi" || file.Extension == ".dll")
                {
                    byte[] result = md5.ComputeHash(File.ReadAllBytes(file.FullName));
                    MD5s[file.Name] = result;
                }
            });
            File.WriteAllText(outputPath + @"\MD5.json", MD5s.ToJson());
            var localMD5Dict = MD5s;
            Debug.LogWarning("MD5值校验生成完毕,开始上传文件");
            //var hotFixHub = new HubConnectionBuilder().WithUrl($"{ServerIP}/HotFixHub").Build();

            //hotFixHub.ServerTimeout = new TimeSpan(0, 5, 0);
            //await hotFixHub.StartAsync();
            var hotFixHub = new HubConnection(new Uri($"{ServerIP}/HotFixHub"), new JsonProtocol(new LitJsonEncoder()));
            await hotFixHub.ConnectAsync();


            string result = "";
            string OnlieMD5FiIeDatas = "{}";
            try
            {
                OnlieMD5FiIeDatas = await hotFixHub.InvokeAsync<string>("GetAssetBundlesMD5", tag);
            }
            catch (Exception e)
            {
                Debug.LogError("无法下载网络上MD5.json文件" + e.Message);
            }
            var onlineMD5Dict = OnlieMD5FiIeDatas.ToObject<Dictionary<string, byte[]>>();
            //上传AB包

            foreach (var item in localMD5Dict)
            {
                //如果文件不存在或者md5值不相等才上传
                if (!onlineMD5Dict.ContainsKey(item.Key) || !onlineMD5Dict[item.Key].SequenceEqual(item.Value))
                {
                    Debug.LogWarning(item.Key + "开始传输");
                    result = await hotFixHub.InvokeAsync<string>("UploadAssetBundles", @$"{ABUpLoadPath}/{projectName}/{tag}/{item.Key}", File.ReadAllBytes(@$"{ABUpLoadPath}/{tag}/{item.Key}"), CommandPassword);
                    Debug.LogWarning(item.Key + "传输" + CommandPassword);

                    Debug.LogWarning(item.Key + "传输" + result);

                }
                else
                {
                    Debug.LogWarning(item.Key + "无更改，无需上传");
                }
            }
            //传输完成后上传AB包MD5文件
            result = await hotFixHub.InvokeAsync<string>("UploadAssetBundles", @$"{ABUpLoadPath}/{projectName}/{tag}/MD5.json", File.ReadAllBytes(@$"{ABUpLoadPath}/{tag}/MD5.json"), CommandPassword);
            Debug.LogWarning($"{tag}的MD5.json的传输结果为{result}");

            Debug.LogWarning("dll开始传输");
            result = await hotFixHub.InvokeAsync<string>("UploadAssetBundles", @$"{ABUpLoadPath}/{projectName}/{tag}_Dll//GameLogic.dll", File.ReadAllBytes($@"{Directory.GetCurrentDirectory()}/Library/ScriptAssemblies/GameLogic.dll"), CommandPassword);
            Debug.LogWarning("dll传输" + result);

            byte[] dllMd5 = md5.ComputeHash(File.ReadAllBytes($@"{Directory.GetCurrentDirectory()}/Library/ScriptAssemblies/GameLogic.dll"));
            result = await hotFixHub.InvokeAsync<string>("UploadAssetBundles", @$"{ABUpLoadPath}/{projectName}/{tag}_Dll/MD5.json", dllMd5, CommandPassword);
            Debug.LogWarning("dll的MD5码更新" + result);
            //await hotFixHub.StopAsync();
            await hotFixHub.CloseAsync();
            md5.Dispose();
        }
    }
}
#endif
