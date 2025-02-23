﻿using System;
using System.Threading.Tasks;
using UnityEngine;

public class CustomThread : MonoBehaviour
{
    /// <summary>
    /// 等待指定值函数
    /// </summary>
    public static async Task UnitllAcync(Func<bool> cond, Action runAction = null)
    {
        while (true)
        {
            if (cond())
            {
                runAction();
                break;
            }
            await Task.Delay(10);
        }
    }
    /// <summary>
    /// 定时任务模块
    /// </summary>
    public static async Task TimerAsync(float stopTime, Action<float> runAction = null)
    {
        int currentMs = 0;
        //DateTime time = DateTime.Now;
        //Debug.Log("开始打印");
        int stopMs = (int)(stopTime * 1000);
        while (currentMs <= stopMs)
        {
            //Debug.Log("当前" + (currentMs));
            //如果任务瞬间停止则进度直接返回100%，否则返回百分比
            runAction(stopTime == 0 ? 1 : currentMs * 1f / stopMs);
            currentMs += 50;
            await Task.Delay(50);
        }
        //Debug.Log("结束打印"+( time - DateTime.Now));
    }
}
