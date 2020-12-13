using System;
using System.Net;
using UnityEngine;


public class PlayerObject
{
    public ushort ID;
    public ushort PrefabID;
    public byte OwnerID;
    public Vector3 Position;
    public Quaternion Rotation;
}

public class PlayerInstance
{
    public ushort ID { get; set; }
    public byte OwnerID { get; set; }
    public ushort SkinID { get; set; }

    public ushort WeaponID_First { get; set; }
    public string WeaponFisrt_Modifications { get; set; }

    public ushort WeaponID_Secondary { get; set; }
    public string WeaponSecondary_Modifications { get; set; }

    public ushort WeaponID_Third { get; set; }
    public string WeaponThird_Modifications { get; set; }

    public byte WeaponID_Grenage { get; set; }
    public byte WeaponID_Knife { get; set; }
    public byte SpecializationID { get; set; }

    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }

    public byte Current_Health { get; set; }
    public byte Class_Speed { get; set; }

    public byte Selected_Class { get; set; }
    public byte Team { get; set; }
}

public class MapObject
{
    public ushort ID { get; set; }
    public bool State { get; set; }
}

public class PlayerLocalServer
{
    public string Guid { get; set; }
    public byte Local_ID { get; set; }
    public string Nickname { get; set; }
    public ushort Local_Kills { get; set; }
    public ushort Local_Deaths { get; set; }
    public byte Current_Team { get; set; }
    public byte Selected_Class { get; set; }
}

[Serializable]
public class ServerConfig
{
    public string ServerName { get; set; }
    public string OnLoadMessage { get; set; }
    public string Map { get; set; }
    public string Gamemode { get; set; }
    public int Port { get; set; }
    public byte Players { get; set; }
    public ushort ScoreTeamNATO { get; set; }
    public ushort ScoreTeamWP { get; set; }
    public int GameTime { get; set; }
    public int AwaitNewSession { get; set; }
    public byte Tickrate { get; set; }
    public bool UPnP { get; set; }
}

