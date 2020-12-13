using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network; 
using System.Threading;
using System;
using System.Net;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Concurrent;

public class Client : MonoBehaviour
{
    public static IPEndPoint GlobalServerIP = new IPEndPoint(IPAddress.Parse("79.174.12.222"), 57125);
    public GameObject spawnpoint;
    GameObject spawnpoint_created;

    public byte playerID;
    public string nickname; 

    public NetClient _client;
    public MapPrefs mpPrefs;
    GlobalServerListener global;
    
    public bool isServer = false;
    public bool isOffline = false;
    public string serverName;
    public string loadMessage;
    public int port;
    public string map;
    public string gamemode;

    public byte SelectedGamemode;
    public enum Gamemodes { TDM, Offline }

    public byte Tickrate;

    public ushort TeamBlueScore;
    public ushort TeamRedScore;

    public int GameTime;

    public ConcurrentDictionary<ushort, GameObject> spawnedPlayers = new ConcurrentDictionary<ushort, GameObject>();
    public ConcurrentDictionary<ushort, GameObject> spawnedSync = new ConcurrentDictionary<ushort, GameObject>();
    public ConcurrentDictionary<uint, GameObject> spawnedNoSync = new ConcurrentDictionary<uint, GameObject>();

    Transform parentForObj, mapCam;

    public delegate void Connect();
    public event Connect Connected;
    public event Connect Disconneted;
    public event Connect SessionEnded;

    public bool SessionStatus;
    public delegate void Gamemode(byte selected);
    public event Gamemode NewGamemode;

    public delegate void TDM();
    public event TDM GetTimeTDM;
    public event TDM GetScoreTDM;

    public delegate void Offline();

    public byte SelectedTeam;

    public delegate void SelectedWeapon(ushort id_model, ushort id_material);             //revork
    public event SelectedWeapon OnClassChange_SelectedFirst;
    public event SelectedWeapon OnClassChange_SelectedSecondary;
    public event SelectedWeapon OnClassChange_SelectedThird;
    public event SelectedWeapon OnClassChange_SelectedFourth;
    public event SelectedWeapon OnClassChange_SelectedSkin;

    public delegate void Team(byte newTeam);
    public event Team ChangeTeamEvent;

    public NetConnection ServerConnection;

    public delegate void SpawnEvent();
    public event SpawnEvent PlayerSpawn;

    public Documents_IO OfflineDocuments;

    private void Awake()
    {
        DontDestroyOnLoad(this.transform.root);
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;

        GameObject globalobj = GameObject.Find("DontDestroyOnLoadInstance");
        if (globalobj != null)
        {
            global = globalobj.GetComponent<GlobalServerListener>();
        }
        else 
        {
            OfflineDocuments = gameObject.AddComponent<Documents_IO>() as Documents_IO;
            global = gameObject.AddComponent<GlobalServerListener>() as GlobalServerListener;
            global.player = new PlayerGlobal
            {
                Nickname = "Offline player",
                Password = "",
                AccessLevel = 0,
                IsBanned = false,
                Score = 0,
                Kills = 0,
                Deaths = 0
            };
            global.player.Skins_Unlocked = new Dictionary<string, ItemCharacter>();
            global.player.Weapons_Unlocked = new Dictionary<string, ItemWeapons>();
            global.player.Specials_Unlocked = new Dictionary<string, ItemSpecial>();
        }
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if(arg0.name == map)
        {
            mapCam = GameObject.Find("Map Camera").transform;
            parentForObj = GameObject.Find("InstansesEmpty").transform;
            mpPrefs = mapCam.GetComponent<MapPrefs>();
            ConnectToServer();
        }   
        else
        {
            Debug.LogWarning("WRONG MAP!");
            Disconneted?.Invoke();
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
            SceneManager.LoadSceneAsync("Main", LoadSceneMode.Single);
        }
    }

    public void ConnectToServer()
    {
        spawnedSync.Clear();
        spawnedPlayers.Clear();

        nickname = global.player.Nickname;
        var config = new NetPeerConfiguration("Ostfront")
        {
            EnableUPnP = true,
            ReceiveBufferSize = 16035,
            SendBufferSize = 16035,
            AcceptIncomingConnections = true
        };
        config.SetMessageTypeEnabled(NetIncomingMessageType.NatIntroductionSuccess, true);
        config.SetMessageTypeEnabled(NetIncomingMessageType.UnconnectedData, true);
        config.SetMessageTypeEnabled(NetIncomingMessageType.ConnectionApproval, true);

        _client = new NetClient(config);
        _client.Start();

        new OF_GameServer.LoopMessages(_client);

        if (!isOffline)
        {
            NetOutgoingMessage connectionMessage = _client.CreateMessage();
            //connectionMessage.Write((byte)GlobalServerTransmissionEnum.IntroduceToPublicServer);
            connectionMessage.Write(serverName);
            connectionMessage.Write(new IPEndPoint(global.GetInternal(),_client.Port));
            _client.SendUnconnectedMessage(connectionMessage, GlobalServerIP);
        }
        else
        {
            StartCoroutine("OfflineConnection");
        }

        StartCoroutine("CheckConnect");
    }
    
    IEnumerator OfflineConnection()
    {
        yield return new WaitForSeconds(1);
        ServerConnection = _client.Connect("127.0.0.1", 27015);
        print("Connecting");
    }

    IEnumerator CheckConnect()
    {
        yield return new WaitForSeconds(10f);
        if (ServerConnection == null)
        {
            Disconneted?.Invoke();
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
            SceneManager.LoadSceneAsync("Main", LoadSceneMode.Single);
        }
        else
        {
            StartCoroutine("CheckConnect");
        }
    }

    [OFRPC(OFRPC.RPCType.Clientside,(ushort)RPC.ConnectionPlayerData)]
    public void Test(NetIncomingMessage message)
    {
        print("client recierved");
    }
}
