using System;
using System.Collections.Generic;

class Room
{
    public string id;
    public string roomNames;
    public bool isFull;
    public bool isInTheGame;
    public List<UserInfo> PlayerInfos = new();
}