using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;
using System.Threading;
using Newtonsoft.Json;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

public class GlobalServerListener : MonoBehaviour
{
    static NetClient client;
    public PlayerGlobal player;
    public ItemsData balance;

    public delegate void Connected(string version);
    public event Connected AcceptVersion;
    public event Connected DeniedVersion;

    public delegate void Accept();
    public event Accept LoadMainMenu;
    public event Accept LoginDenied;

    public delegate void Server(string name, int port, byte playersMax, byte playersNow, string onLoadMessage, string map, string gamemode);
    public event Server ServerList;

    public delegate void Data();
    public event Data GetNewData;

    private void Awake()
    { 
        Application.targetFrameRate = 120;
    }

    void Start()
    {
        player = new PlayerGlobal();
        if (transform.name != "Main Canvas" || transform.name == "Network")
        {
            DontDestroyOnLoad(this.gameObject);
        }
        var config = new NetPeerConfiguration("of_masterserver")
        {
            ReceiveBufferSize = 4096,
            SendBufferSize = 4096
        };
        client = new NetClient(config);
        client.Start();
        client.Connect("79.174.12.222", 57125);

        StartCoroutine("CheckVersion");
    }

    void Update()
    {
        NetIncomingMessage message;
        if (client != null)
        {
            if ((message = client.ReadMessage()) != null)
            {
                switch (message.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        switch ((GlobalServerTransmissionEnum)message.ReadByte())
                        {/*
                            case GlobalServerTransmissionEnum.CheckVersionOnLoad:
                                {
                                    string msg = message.ReadString();
                                    CompleteCheckVersion(msg);  
                                }
                                break;
                            case GlobalServerTransmissionEnum.Login:
                                {
                                    bool result = message.ReadBoolean();
                                    if (result == false)
                                    {
                                        LoginDenied?.Invoke();
                                        break;
                                    }
                                    string _data = message.ReadString();
                                    player = JsonConvert.DeserializeObject<PlayerLocalServer>(_data);
                                    LoadMainMenu?.Invoke();
                                }
                                break;
                            case GlobalServerTransmissionEnum.RequestServerList:
                                {
                                    string name = message.ReadString();
                                    int port = message.ReadInt32();
                                    byte playersMax = message.ReadByte();
                                    byte playersConnected = message.ReadByte();
                                    string onLoadMessage = message.ReadString();
                                    string gamemode = message.ReadString();
                                    string map = message.ReadString();
                                    ServerList?.Invoke(name, port, playersMax, playersConnected, onLoadMessage, map, gamemode);
                                }
                                break;
                            case GlobalServerTransmissionEnum.ResendPlayerInfo:
                                {
                                    string data = message.ReadString();
                                    player = JsonConvert.DeserializeObject<PlayerLocalServer>(data);
                                    GetNewData?.Invoke();
                                    break;
                                }
                            case GlobalServerTransmissionEnum.GetServerData:
                                {
                                    string data = message.ReadString();
                                    balance = JsonConvert.DeserializeObject<ItemsData>(data);
                                }*/
                        }
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        switch ((NetConnectionStatus)message.ReadByte())
                        {
                            case NetConnectionStatus.Disconnected:
                                {
                                    DeniedVersion?.Invoke("Problems with connection to global server!");
                                }
                                break;
                        }
                        break;
                }
                client.Recycle(message);
            }
        }
    }

    IEnumerator CheckVersion()
    {
        yield return new WaitForSeconds(1f);
        if (client != null & client.ServerConnection != null)
        {
            NetOutgoingMessage msg = client.CreateMessage();
          //  msg.Write((byte)GlobalServerTransmissionEnum.CheckVersionOnLoad);
            client.SendMessage(msg, client.ServerConnection, NetDeliveryMethod.ReliableUnordered);
            client.FlushSendQueue();
        }
        else
        {
            yield return new WaitForSeconds(5);
            StartCoroutine("CheckVersion");
        }
    }

    void CompleteCheckVersion(string version)
    {
        if( version == Application.version )
        {
            AcceptVersion?.Invoke("");
        }
        else
        {
            DeniedVersion?.Invoke(version);
        }
    }

    public void Login(string nickname, string password)
    {
        if (client != null & client.ServerConnection != null)
        {
            NetOutgoingMessage msg = client.CreateMessage();
         //   msg.Write((byte)GlobalServerTransmissionEnum.Login);
            msg.Write(nickname);
            msg.Write(password);
            client.SendMessage(msg, client.ServerConnection, NetDeliveryMethod.ReliableUnordered);
            client.FlushSendQueue();
        }
    }

    public void RequestServerList()
    {
        NetOutgoingMessage msg = client.CreateMessage();
      //  msg.Write((byte)GlobalServerTransmissionEnum.RequestServerList);
        client.SendMessage(msg, client.ServerConnection, NetDeliveryMethod.ReliableUnordered);
        client.FlushSendQueue();
    }

    public void UpdateServerInList(string serverName, byte playersCon)
    {
        NetOutgoingMessage msg = client.CreateMessage();
        msg.Write((byte)GlobalServerTransmissionEnum.UpdateServerInList);
        msg.Write(serverName);
        msg.Write(playersCon);
        client.SendMessage(msg, client.ServerConnection, NetDeliveryMethod.ReliableUnordered);
        client.FlushSendQueue();
    }
    
    public IPAddress GetInternal()
    {
        IPAddress localIP;
        using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
        {
            socket.Connect("8.8.8.8", 65530);
            IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
            localIP = endPoint.Address;
        }
        print(localIP);
        return localIP;
    }
    
    public void OnApplicationQuit()
    {
        client.Shutdown("disconnect");
    }

    public void RequestNewData()
    {
        NetOutgoingMessage outgoingMessage = client.CreateMessage();
     //   outgoingMessage.Write((byte)GlobalServerTransmissionEnum.ResendPlayerInfo);
        outgoingMessage.Write(player.Nickname);
        client.SendMessage(outgoingMessage, client.ServerConnection, NetDeliveryMethod.ReliableUnordered);
        client.FlushSendQueue();
    }

    public void RequestBalanceData()
    {
        NetOutgoingMessage msg = client.CreateMessage();
      //  msg.Write((byte)GlobalServerTransmissionEnum.GetServerData);
        client.SendMessage(msg, client.ServerConnection, NetDeliveryMethod.ReliableUnordered);
        client.FlushSendQueue();
    }
}

[SerializeField]
public class PlayerGlobal
{
    public string Nickname { get; set; }
    public string Password { get; set; }
    public byte AccessLevel { get; set; }

    public bool IsBanned { get; set; }

    public int Kills { get; set; }
    public int Deaths { get; set; }

    public int Score { get; set; }

    public Dictionary<string, ItemWeapons> Weapons_Unlocked { get; set; }
    public Dictionary<string, ItemCharacter> Skins_Unlocked { get; set; }
    public Dictionary<string, ItemSpecial> Specials_Unlocked { get; set; }
}

public class ItemWeapons
{
    public int Kills { get; set; }
    public List<string> Camo { get; set; }
}

public class ItemCharacter
{
    public int Kills { get; set; }
    public List<string> Camo { get; set; }
}

public class ItemSpecial
{
    public int Kills { get; set; }
}

[SerializeField]
public class ItemsData
{
    public string GameVersion { get; set; }

    public string[] Weapons_At_Start { get; set; }
    public string[] Skins_At_Start { get; set; }
    public string[] Special_Weapons_At_Start { get; set; }

    public Dictionary<string, Weapons> Weapons { get; set; }
    public Dictionary<string, Skins> Skins { get; set; }
    public Dictionary<string, Classes> Classes { get; set; }
    public Dictionary<string, Special> Special { get; set; }
}

public class Weapons
{
    public byte Damage { get; set; }
    public ushort Ammo_Magazine { get; set; }
    public ushort Ammo_All { get; set; }
    public float Recoil_Vertical { get; set; }
    public float Recoil_Left { get; set; }
    public float Recoil_Right { get; set; }
    public float Dispersion { get; set; }
    public float Dispersion_InZoom { get; set; }
    public ushort Range { get; set; }
    public string How_Unlock { get; set; }
    public string[] Camo { get; set; }
}

public class Skins
{
    public string How_Unlock { get; set; }
    public string[] Camo { get; set; }
}

public class Classes
{
    public byte Health { get; set; }
    public byte Speed { get; set; }
    public string[] WP_Weapons { get; set; }
    public string[] NATO_Weapons { get; set; }
    public string[] WP_Secondary { get; set; }
    public string[] NATO_Secondary { get; set; }
    public string[] WP_Skins { get; set; }
    public string[] NATO_Skins { get; set; }
}

public class Special
{
    public string How_Unlock { get; set; }
    public int Damage { get; set; }
}