using System;
using System.IO;
using System.Linq.Expressions;

public class ConfigManager
{
    public static ConfigData PlayerLocalData { get; set; }

    public static void LoadLocalUserData()
    {
        if (!File.Exists("UserData.ini"))
        {
            SaveLocalUserData(new ConfigData() { });
        }
        if (PlayerLocalData == null)
        {
            PlayerLocalData = File.ReadAllText("UserData.ini").ToObject<ConfigData>();
        }
    }
    public static void SetConfigData<TProperty>(Expression<Func<ConfigData, TProperty>> propertyExpression, TProperty newValue)
    {
        var memberExpression = propertyExpression.Body as MemberExpression;
        typeof(ConfigData).GetProperty(memberExpression.Member.Name).SetValue(PlayerLocalData, newValue);
        SaveLocalUserData(PlayerLocalData);
    }
    public static void SaveLocalUserData(ConfigData configData)
    {
        File.WriteAllText("UserData.ini", configData.ToJson());
    }
}