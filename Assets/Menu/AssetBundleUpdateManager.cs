using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//该脚本无法被热更，修改需要重新打包
public class AssetBundleUpdateManager : GeziBehaviour<AssetBundleUpdateManager>
{

    public enum GameStartMode
    {
        Editor,
        PC_Release,
        PC_Test,
        Android
    }
    #region 需要配置项目
    public Text loadText;
    public Text processText;
    public Slider slider;
    public static bool isLocalMode;
    public static List<string> Configs { get; set; }
    static string ServerTag => "PC_Release";
    static string HotFixSceneABFileName => "1_loadscene.gezi";
    static string HotFixAssetFileName => "load.gezi";
    static string HotFixExeName => "ElationBar.exe";
    static string HotFixAPKName => "ElationBar.apk";
    static string HotFixDllName => "GameLogic.dll";

    public static string ProjectName => Configs[1];
    public static string ServerIP => isLocalMode ? "127.0.0.1" : Configs[3];
    public static string ServerDownloadUrl => $"{ServerIP}:7777/AB_Upload/{ProjectName}";

    #endregion
    public static Action EndAction = null;
    static MD5 md5 = new MD5CryptoServiceProvider();
    public static GameStartMode CurrentGameStartMode;
    bool isTestMode;


    static string localHotFixSceneBundlePath = "";
    static string localHotFixAssetBundlePath = "";
    static string localDllOrApkPath = "";
    static string onlineHotFixSceneBundlePath = "";
    static string onlineHotFixAssetBundlePath = "";
    static string onlineDllOrApkPath = "";

    static string onlineAB_MD5sFile = "";
    static string onlineDllOrApk_MD5Path = "";
    //配置文件路径
    static string ConfigFileSavePath => (Application.isMobilePlatform ? Application.persistentDataPath : Directory.GetCurrentDirectory()) + "/GameConfig.ini";
    private async void Start()
    {
        Init2();
        await DownLoadAssetBundles();
        await DownLoadDllOrApk();
        AssetBundle.UnloadAllAssetBundles(true);
        //加载热更AB包，切换到热更场景
        AssetBundle.LoadFromFile(localHotFixSceneBundlePath);
        AssetBundle.LoadFromFile(localHotFixAssetBundlePath);
        Debug.LogWarning("重新载入完成");
        SceneManager.LoadScene("1_Load");
    }

    //流程：检查ab包资源是否需要重新下载
    //检查dll或者apk是否需要下载
    private void Init2()
    {
        Configs = Resources.Load<TextAsset>("HotFix").text.Split("\r\n").ToList();
        if (Application.isEditor)
            CurrentGameStartMode = GameStartMode.Editor;
        else if (Application.isMobilePlatform)
            CurrentGameStartMode = GameStartMode.Android;
        else if (isTestMode)
            CurrentGameStartMode = GameStartMode.PC_Test;
        else
            CurrentGameStartMode = GameStartMode.PC_Release;
        switch (CurrentGameStartMode)
        {
            case GameStartMode.Editor:
            case GameStartMode.PC_Release:
            case GameStartMode.PC_Test:
                //指定热更场景和资源本地路径
                localDllOrApkPath = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles(HotFixDllName, SearchOption.AllDirectories).FirstOrDefault()?.FullName;
                localHotFixSceneBundlePath = $"AB_Download/{ServerTag}/{HotFixSceneABFileName}";
                localHotFixAssetBundlePath = $"AB_Download/{ServerTag}/{HotFixAssetFileName}";
                //指定热更场景和资源网络路径
                onlineHotFixSceneBundlePath = $"{ServerDownloadUrl}/{ServerTag}/{HotFixSceneABFileName}";
                onlineHotFixAssetBundlePath = $"{ServerDownloadUrl}/{ServerTag}/{HotFixAssetFileName}";
                onlineAB_MD5sFile = $"{ServerDownloadUrl}/{ServerTag}/MD5.json";
                Debug.Log(onlineAB_MD5sFile);
                onlineDllOrApk_MD5Path = $"{ServerDownloadUrl}/{ServerTag}_Dll/MD5.json";
                onlineDllOrApkPath = $"{ServerDownloadUrl}/{ServerTag}_Dll/{HotFixDllName}";
                break;
            case GameStartMode.Android:
                //指定热更场景和资源本地路径
                localHotFixSceneBundlePath = $"{Application.persistentDataPath}/Assetbundles/{HotFixSceneABFileName}";
                localHotFixAssetBundlePath = $"{Application.persistentDataPath}/Assetbundles/{HotFixAssetFileName}";
                localDllOrApkPath = $"{Application.persistentDataPath}/APK/{HotFixAPKName}";
                //指定热更场景和资源网络路径
                onlineHotFixSceneBundlePath = $"{ServerDownloadUrl}/Android/{HotFixSceneABFileName}";
                onlineHotFixAssetBundlePath = $"{ServerDownloadUrl}/Android/{HotFixAssetFileName}";
                onlineAB_MD5sFile = $"{ServerDownloadUrl}/Android/MD5.json";
                onlineDllOrApk_MD5Path = $"{ServerDownloadUrl}/Apk/MD5.json";
                onlineDllOrApkPath = $"{ServerDownloadUrl}/Apk/{HotFixAPKName}";
                break;
        }

    }
    //private async void Init()
    //{

    //    var a = Resources.Load<TextAsset>("HotFix");
    //    Configs = a.text.Split("\r\n").ToList();
    //    if (Application.isMobilePlatform)
    //    {
    //        //指定热更场景和资源本地路径
    //        localHotFixSceneBundlePath = $"{Application.persistentDataPath}/Assetbundles/{HotFixSceneABFileName}";
    //        localHotFixAssetBundlePath = $"{Application.persistentDataPath}/Assetbundles/{HotFixAssetFileName}";
    //        localDllOrApkPath = $"{Application.persistentDataPath}/APK/{HotFixAPKName}";
    //        //指定热更场景和资源网络路径
    //        onlineHotFixSceneBundlePath = $"{ServerDownloadUrl}/Android/{HotFixSceneABFileName}";
    //        onlineHotFixAssetBundlePath = $"{ServerDownloadUrl}/Android/{HotFixAssetFileName}";
    //        onlineAB_MD5sFile = $"{ServerDownloadUrl}/Android/MD5.json";
    //        onlineDllOrApk_MD5Path = $"{ServerDownloadUrl}/Apk/MD5.json";
    //        onlineDllOrApkPath = $"{ServerDownloadUrl}/Apk/{HotFixAPKName}";
    //    }
    //    else
    //    {
    //        //指定热更场景和资源本地路径
    //        localDllOrApkPath = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles(HotFixDllName, SearchOption.AllDirectories).FirstOrDefault()?.FullName;
    //        localHotFixSceneBundlePath = $"AB_Download/{ServerTag}/{HotFixSceneABFileName}";
    //        localHotFixAssetBundlePath = $"AB_Download/{ServerTag}/{HotFixAssetFileName}";
    //        //指定热更场景和资源网络路径
    //        onlineHotFixSceneBundlePath = $"{ServerDownloadUrl}/{ServerTag}/{HotFixSceneABFileName}";
    //        onlineHotFixAssetBundlePath = $"{ServerDownloadUrl}/{ServerTag}/{HotFixAssetFileName}";
    //        onlineAB_MD5sFile = $"{ServerDownloadUrl}/{ServerTag}/MD5.json";
    //        Debug.Log(onlineAB_MD5sFile);
    //        onlineDllOrApk_MD5Path = $"{ServerDownloadUrl}/{ServerTag}_Dll/MD5.json";
    //        onlineDllOrApkPath = $"{ServerDownloadUrl}/{ServerTag}_Dll/{HotFixDllName}";
    //    }
    //    using (var httpClient = new HttpClient())
    //    {
    //        //对比热更场景MD5，判断是否下载
    //        byte[] data;
    //        HttpResponseMessage httpResponse;
    //        httpResponse = await httpClient.GetAsync(onlineAB_MD5sFile);
    //        if (!httpResponse.IsSuccessStatusCode) { Debug.LogError("onlineAB_MD5sFile文件下载失败"); return; }
    //        var AB_MD5s = JsonConvert.DeserializeObject<Dictionary<string, byte[]>>(await httpResponse.Content.ReadAsStringAsync());

    //        //校验热更场景
    //        if (new FileInfo(localHotFixSceneBundlePath).Exists && AB_MD5s[HotFixSceneABFileName].SequenceEqual(md5.ComputeHash(File.ReadAllBytes(new FileInfo(localHotFixSceneBundlePath).FullName))))
    //        {
    //            Debug.Log("热更场景无变动");
    //        }
    //        else
    //        {
    //            //下载热更新场景
    //            httpResponse = await httpClient.GetAsync(onlineHotFixSceneBundlePath);
    //            if (!httpResponse.IsSuccessStatusCode) { Debug.LogError("热更场景文件下载失败"); return; }
    //            data = await httpResponse.Content.ReadAsByteArrayAsync();
    //            Directory.CreateDirectory(new FileInfo(localHotFixSceneBundlePath).DirectoryName);
    //            File.WriteAllBytes(localHotFixSceneBundlePath, data);
    //        }

    //        //校验热更场景素材
    //        if (new FileInfo(localHotFixAssetBundlePath).Exists && AB_MD5s[HotFixAssetFileName].SequenceEqual(md5.ComputeHash(File.ReadAllBytes(new FileInfo(localHotFixAssetBundlePath).FullName))))
    //        {
    //            Debug.Log("热更场景素材无变动");
    //        }
    //        else
    //        {
    //            //下载热更新场景
    //            httpResponse = await httpClient.GetAsync(onlineHotFixAssetBundlePath);
    //            if (!httpResponse.IsSuccessStatusCode) { Debug.LogError("热更场景素材文件下载失败"); return; }
    //            data = await httpResponse.Content.ReadAsByteArrayAsync();
    //            Directory.CreateDirectory(new FileInfo(localHotFixAssetBundlePath).DirectoryName);
    //            File.WriteAllBytes(localHotFixAssetBundlePath, data);
    //        }

    //        httpResponse = await httpClient.GetAsync(onlineDllOrApk_MD5Path);
    //        if (!httpResponse.IsSuccessStatusCode) { Debug.LogError("dll或者apk的md5文件下载失败"); return; }
    //        data = await httpResponse.Content.ReadAsByteArrayAsync();
    //        //如果是手机端，检查apk变更，否则检查dll变更，若发生变更，则重启
    //        if (data.SequenceEqual(md5.ComputeHash(File.ReadAllBytes(new FileInfo(localDllOrApkPath).FullName))))
    //        {
    //            Debug.Log("文件无改动");
    //        }
    //        else
    //        {
    //            httpResponse = await httpClient.GetAsync(onlineDllOrApkPath);
    //            if (!httpResponse.IsSuccessStatusCode) { Debug.LogError("DllOrApk文件下载失败"); return; }
    //            //保存相关的dll或者apk文件
    //            if (!Application.isEditor)
    //            {
    //                Directory.CreateDirectory(new FileInfo(localDllOrApkPath).DirectoryName);
    //                File.WriteAllBytes(localDllOrApkPath, await httpResponse.Content.ReadAsByteArrayAsync());
    //                if (Application.isMobilePlatform)
    //                {
    //                    InstallApk(localDllOrApkPath);

    //                    void InstallApk(string apkFilePath)
    //                    {
    //                        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
    //                        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent", intentClass.GetStatic<string>("ACTION_VIEW"));
    //                        AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
    //                        AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + apkFilePath);
    //                        intentObject.Call<AndroidJavaObject>("setDataAndType", uriObject, "application/vnd.android.package-archive");

    //                        AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    //                        AndroidJavaObject currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
    //                        currentActivity.Call("startActivity", intentObject);
    //                    }
    //                    //安卓端重启重新安装
    //                    //AndroidJavaClass intentObj = new AndroidJavaClass("android.content.Intent");
    //                    //AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", intentObj.GetStatic<string>("ACTION_INSTALL_PACKAGE"));
    //                    //Application.Quit();

    //                    //// 设置 APK 文件的 Uri
    //                    //AndroidJavaClass uriObj = new AndroidJavaClass("android.net.Uri");
    //                    //AndroidJavaObject uri = uriObj.CallStatic<AndroidJavaObject>("parse", "file://" + "待修改");
    //                    //intent.Call<AndroidJavaObject>("setData", uri);

    //                    //// 调用安装程序
    //                    //AndroidJavaClass unityObj = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    //                    //AndroidJavaObject context = unityObj.GetStatic<AndroidJavaObject>("currentActivity");
    //                    //context.Call("startActivity", intent);
    //                }
    //                else
    //                {
    //                    var game = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles(HotFixExeName, SearchOption.AllDirectories).FirstOrDefault();
    //                    if (game != null)
    //                    {
    //                        System.Diagnostics.Process.Start(game.FullName);
    //                    }
    //                    Application.Quit();
    //                }
    //            }
    //        }
    //        DownLoadAssetBundles();
    //    }
    //    AssetBundle.UnloadAllAssetBundles(true);
    //    //加载热更AB包，切换到热更场景
    //    AssetBundle.LoadFromFile(localHotFixSceneBundlePath);
    //    AssetBundle.LoadFromFile(localHotFixAssetBundlePath);
    //    Debug.LogWarning("重新载入完成");
    //    SceneManager.LoadScene("1_Load");
    //}
    public async Task DownLoadDllOrApk()
    {
        using (var httpClient = new HttpClient())
        {
            byte[] data;
            HttpResponseMessage httpResponse = await httpClient.GetAsync(onlineDllOrApk_MD5Path);
            if (!httpResponse.IsSuccessStatusCode) { Debug.LogError("dll或者apk的md5文件下载失败"); return; }
            data = await httpResponse.Content.ReadAsByteArrayAsync();
            //如果是手机端，检查apk变更，否则检查dll变更，若发生变更，则重启
            if (data.SequenceEqual(md5.ComputeHash(File.ReadAllBytes(new FileInfo(localDllOrApkPath).FullName))))
            {
                Debug.Log("apk或dll文件无改动，不用更改");
            }
            else
            {
                httpResponse = await httpClient.GetAsync(onlineDllOrApkPath);
                if (!httpResponse.IsSuccessStatusCode) { Debug.LogError("DllOrApk文件下载失败"); return; }
                //保存相关的dll或者apk文件
                switch (CurrentGameStartMode)
                {
                    case GameStartMode.Editor:
                        Debug.Log("编辑器下不做处理");
                        break;
                    case GameStartMode.PC_Release:

                    case GameStartMode.PC_Test:
                        var game = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles(HotFixExeName, SearchOption.AllDirectories).FirstOrDefault();
                        if (game != null)
                        {
                            System.Diagnostics.Process.Start(game.FullName);
                        }
                        Application.Quit();
                        break;
                    case GameStartMode.Android:
                        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
                        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent", intentClass.GetStatic<string>("ACTION_VIEW"));
                        AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
                        AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + localDllOrApkPath);
                        intentObject.Call<AndroidJavaObject>("setDataAndType", uriObject, "application/vnd.android.package-archive");

                        AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                        AndroidJavaObject currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
                        currentActivity.Call("startActivity", intentObject);
                        break;
                    default:
                        break;
                }
            }
        }
    }
    public async Task DownLoadAssetBundles()
    {
        loadText.text = "开始本地AB包资源检测";
        //获得AB包来源标签
        loadText.text = "开始下载文件";
        Debug.LogWarning("开始下载文件" + System.DateTime.Now);

        string downLoadPath = $"{Directory.GetCurrentDirectory()}/AB_Download/{ServerTag}/";
        Directory.CreateDirectory(downLoadPath);
        using (var httpClient = new HttpClient())
        {
            var responseMessage = await httpClient.GetAsync($"{ServerDownloadUrl}/{ServerTag}/MD5.json");
            if (!responseMessage.IsSuccessStatusCode) { loadText.text = "MD5文件获取出错"; return; }
            var OnlieMD5FiIeDatas = await responseMessage.Content.ReadAsStringAsync();
            var Md5Dict = OnlieMD5FiIeDatas.ToObject<Dictionary<string, byte[]>>();
            Debug.Log("MD5文件已加载完成" + OnlieMD5FiIeDatas);
            loadText.text = "MD5文件已加载完成：";
            //已下好任务数
            int downloadTaskCount = 0;
            //开始遍历校验并更新本地AB包文件
            foreach (var MD5FiIeData in Md5Dict)
            {
                //当前校验的本地文件
                FileInfo localFile = new FileInfo(downLoadPath + MD5FiIeData.Key);
                if (localFile.Exists && MD5FiIeData.Value.SequenceEqual(md5.ComputeHash(File.ReadAllBytes(localFile.FullName))))
                {
                    loadText.text = MD5FiIeData.Key + "校验成功，无需下载";
                    Debug.LogWarning(MD5FiIeData.Key + "校验成功，无需下载");
                }
                else
                {
                    loadText.text = MD5FiIeData.Key + "有新版本，开始重新下载";
                    Debug.LogWarning(MD5FiIeData.Key + "有新版本，开始重新下载");
                    await DownLoadFile(MD5FiIeData, localFile);
                    async Task DownLoadFile(KeyValuePair<string, byte[]> MD5FiIeData, FileInfo localFile)
                    {
                        loadText.text = $"正在下载:{MD5FiIeData.Key},进度 {downloadTaskCount}/{Md5Dict.Count}";
                        using (WebClient webClient = new WebClient())
                        {
                            webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
                            Debug.LogWarning("下载文件" + $"{ServerDownloadUrl}/{ServerTag}/{MD5FiIeData.Key}");
                            try
                            {
                                await webClient.DownloadFileTaskAsync(new System.Uri($"{ServerDownloadUrl}/{ServerTag}/{MD5FiIeData.Key}"), localFile.FullName);
                                Debug.LogWarning(MD5FiIeData.Key + "下载完成");
                                Debug.LogWarning("结束下载文件" + localFile.Name + " " + System.DateTime.Now);

                            }
                            catch (Exception e)
                            {

                                Debug.LogWarning(MD5FiIeData.Key + "下载完失败" + e.Message);
                            }
                            void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
                            {
                                processText.text = $"{e.BytesReceived / 1024 / 1024}MB/{e.TotalBytesToReceive / 1024 / 1024}MB";
                                slider.value = e.BytesReceived * 1f / e.TotalBytesToReceive;
                            }
                        }
                    }
                }
                downloadTaskCount++;
            }
            Debug.LogWarning("全部AB包下载完成");
            loadText.text = "全部AB包下载完成";
        }
        //md5.Dispose();
    }
}