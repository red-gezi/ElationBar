public class UserInfo
{
    public string Name { get; set; }
    public string ConnectionId { get; set; }
    public PlayerState CurrentState { get; set; }
    public Chara PlayerRole { get; set; }
    public int MaxBulletPoint { get; set; }
    public int CurrentBulletPoint { get; set; } = 0;
    public UserInfo()
    {
    }
    public UserInfo(string name, string connectionId)
    {
        Name = name;
        ConnectionId = connectionId;
    }

    
}
