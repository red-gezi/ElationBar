using Best.SignalR;
using Best.SignalR.Encoders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public static class NetManager
{
    public static bool IsLocal { get; set; } = true;
    static string ip => IsLocal ? "localhost:233" : "106.15.38.165:233";
    static HubConnection ServerHub { get; set; } = null;
    public static async Task Init(bool isHotFixedLoad = false)
    {
        try
        {
            if (ServerHub == null)
            {
                //ServerHub = new HubConnectionBuilder().WithUrl($"http://{ip}/GameHub").Build();
                ServerHub = new HubConnection(new Uri($"http://{ip}/GameHub"), new JsonProtocol(new LitJsonEncoder()));
                await ServerHub.ConnectAsync();
                //await ServerHub.StartAsync();
                ServerHub.On<Room>("NotifyJoinRoom", room => GameManager.NotifyJoinRoom(room));

                ServerHub.On<int, List<UserInfo>>("NotifyGameInit", (chairID, playerInfos) => GameManager.NotifyGameInit(chairID, playerInfos));
                ServerHub.On("NotifyTurnInit", () => GameManager.NotifyTurnInit());
                
                ServerHub.On<CardType>("NotifyShowTargetCard", cardType => GameManager.NotifyShowTargetCard(cardType));
                ServerHub.On("NotifyLoadBullet", () => GameManager.NotifyLoadBullet());
                ServerHub.On<List<CardType>>("NotifyDraw5Cards", cardsType => GameManager.NotifyDraw5Cards(cardsType));
                ServerHub.On<int, List<int>>("NotifyPlayCard", (currentPlayerIndex, selectCardIndexs) => GameManager.NotifyPlayCard(currentPlayerIndex, selectCardIndexs));
                ServerHub.On<int>("NotifyQuestion", currentPlayerIndex => GameManager.NotifyQuestion(currentPlayerIndex));
                ServerHub.On<int,float>("NotifyWaitForPlayer", (currentPlayerIndex, second )=> GameManager.NotifyWaitForPlayer(currentPlayerIndex, second));
                ServerHub.On<int, bool>("NotifyPlayerShot", (currentPlayerIndex, isSurvival) => GameManager.NotifyPlayerShot(currentPlayerIndex, isSurvival));
                ServerHub.On<int>("NotifyPlayerWin", currentPlayerIndex => GameManager.NotifyPlayerWin(currentPlayerIndex));
                ServerHub.On("NotifyPlayerAgain", () => GameManager.NotifyPlayerAgain());
                //向当前正在校准的角色发送语音
                ServerHub.On<byte[]>("ReceiveVoiceToSelf", (x) => Debug.Log($"{DateTime.Now}接收完成3，时间间隔为{DateTime.Now - now}"));
                //向指定座位的角色发送语音
                ServerHub.On<int, byte[]>("ReceiveVoiceToPlayer", (id, x) => Debug.Log($"{DateTime.Now}接收完成3，时间间隔为{DateTime.Now - now}"));
            }
            else
            {
                Debug.Log("服务器已有初始化实例");
            }

        }
        catch (Exception e)
        {
            Debug.Log("无法链接到服务器,请点击重连");

            //await NoticeCommand.ShowAsync("无法链接到服务器,请点击重连\n" + e.Message, NotifyBoardMode.Ok, okAction: async () => { await Init(); });
        }
    }
    public static async Task CheckHubState()
    {
        if (ServerHub == null)
        {
            await Init();
        }
    }
    public static async void Dispose()
    {
        Debug.Log("释放网络资源");
        //await ServerHub.StopAsync();
        await ServerHub.CloseAsync();
    }
    public static async Task<bool> LoginAsync(string name)
    {
        try
        {
            Debug.Log("登陆请求");
            await CheckHubState();
            var a = ServerHub.State;

            await ServerHub.SendAsync("Login", name);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            Debug.Log("账号登录失败");
            return false;
        }
    }
    public static async Task<string> CreatRoom(string roomName, string roomPassword)
    {
        try
        {
            Debug.Log("创建房间请求");
            await CheckHubState();
            string roomID = await ServerHub.InvokeAsync<string>("CreatRoom", roomName, roomPassword);
            return roomID;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            Debug.Log("创建房间失败");
            return "";
        }
    }
    public static async Task<bool> JoinRoom(string roomID)
    {
        try
        {
            await CheckHubState();
            bool isJoinSuccess = await ServerHub.InvokeAsync<bool>("JoinRoom", roomID, GameManager.PlayerName, GameManager.PlayerChara);
            return isJoinSuccess;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            Debug.Log("加入房间失败");
            return false;
        }
    }
    public static async Task<bool> LeaveRoom(string roomID)
    {
        try
        {
            await CheckHubState();
            bool isJoinSuccess = await ServerHub.InvokeAsync<bool>("LeaveRoom", roomID);
            return isJoinSuccess;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            Debug.Log("离开房间失败");
            return false;
        }
    }
    internal static async Task GameStartMockAsync()
    {
        await CheckHubState();
        GameManager.RoomID = await ServerHub.InvokeAsync<string>("GameStartMock");
        Debug.Log("创建房间成功");
    }
    static DateTime now;
    public static async Task SendVoice(float[] voice)
    {
        await CheckHubState();

        Debug.Log($"传输{voice.Count()}数据");
        //Debug.Log("开始传输音频" + DateTime.Now);

        //now = DateTime.Now;
        //await ServerHub.SendAsync("SendVoice1", voice);
        //Debug.Log($"{DateTime.Now}传输完成1，时间间隔为{DateTime.Now - now}");

        now = DateTime.Now;
        var data = await ServerHub.InvokeAsync<string>("SendVoice2", voice);
        Debug.Log($"{DateTime.Now}传输完成2，时间间隔为{DateTime.Now - now}");

        //now = DateTime.Now;
        //await ServerHub.SendAsync("SendVoice3", voice);
        //Debug.Log($"{DateTime.Now}发送完成3，时间间隔为{DateTime.Now - now}");
    }

    internal static async void PlayCard(List<Card> selectCards)
    {
        Debug.Log("打出卡牌");
        await ServerHub.SendAsync("PlayerPlayCard", GameManager.RoomID, GameManager.ClientChairID, selectCards);
    }

    internal static async void Question()
    {
        Debug.Log("发出质疑");
        await ServerHub.SendAsync("PlayerQuestion", GameManager.RoomID, GameManager.ClientChairID);
    }
    ///////////////////////////////////////////////////语音////////////////////////////////////////////////////////////////
    public static async Task SendVoiceToSelf(byte[] voice)
    {
        await CheckHubState();
        Debug.Log($"传输{voice.Count()}数据");
        DateTime now = DateTime.Now;
        var data = await ServerHub.InvokeAsync<byte[]>("SendVoiceToSelf", voice.ToList());
        Debug.Log($"{DateTime.Now}传输完成2，时间间隔为{DateTime.Now - now}");
    }
    public static async Task SendVoiceToRoom(byte[] voice)
    {
        await CheckHubState();
        Debug.Log($"传输{voice.Count()}数据");
        DateTime now = DateTime.Now;
        var data = await ServerHub.InvokeAsync<string>("SendVoiceToRoom", voice.ToJson());
        Debug.Log($"{DateTime.Now}传输完成2，时间间隔为{DateTime.Now - now}");
    }
    ///////////////////发送聊天信息///////////////
    //请求发送消息
    //public static async Task SendMessage(string chatID, ChatMessageInfo.ChatMessage message, string speakerUID, string targetChaterUID)
    //{
    //    await CheckHubState();
    //    await ServerHub.SendAsync("SendMessage", PlayerPassWord, chatID, message, speakerUID, targetChaterUID);
    //}
    ///////////////////////////////////////////////////房间操作////////////////////////////////////////////////////////////////
}
