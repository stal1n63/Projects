using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lidgren.Network;

public class MemoryOperations
{
    public static ConcurrentDictionary<PlayerInfo, bool> PlayersData;
    private static List<PlayerInfo> PlayersData_IO = new List<PlayerInfo>();

    public static ItemsData BalanceData = new ItemsData();

    #region Initialization
    public MemoryOperations()
    {
        LoadGameBalance(Program.PATH_BALANCE_FILE);

        var files = Directory.EnumerateFiles(Program.PATH_PLAYERS_DATA, "*.json");

        PlayersData = new ConcurrentDictionary<PlayerInfo, bool>();

        foreach (var file in files)
        {
            bool success = false;
            while (!success)
                success = PlayersData.TryAdd(JsonConvert.DeserializeObject<PlayerInfo>(File.ReadAllText(file)), false);
        }
        Console.WriteLine("Loaded players");

        TimerCallback tm = new TimerCallback(RefreshPlayersAccountsInMemory);
        Timer timer = new Timer(tm, null, 0, 300000); //5 mins
    }

    public static void LoadGameBalance(string path)
    {
        if(File.Exists(path))
        {
            BalanceData = JsonConvert.DeserializeObject<ItemsData>(File.ReadAllText(path));
        }
        else
        {
            throw new Exception("No real items.json");
        }
    }

    public async static void RefreshPlayersAccountsInMemory(object obj)
    {
        PlayersData_IO.Clear();
        try
        {
            foreach (var pl in PlayersData)
            {
                if (pl.Value)
                {
                    PlayersData_IO.Add(pl.Key);
                    PlayersData[pl.Key] = false;
                }
            }
        }
        catch
        {
            throw new Exception("Cant save players!");
        }

        if (obj == null) //who invoker?
        {
            Task.Factory.StartNew(SaveToData);
        }
        else
        {
            await Task.Factory.StartNew(SaveToData);
        }
    }
    #endregion

    private static void SaveToData()
    {
        Console.WriteLine("Save players to HDD");
        foreach (var data in PlayersData_IO)
        {
            string str = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(Program.PATH_PLAYERS_DATA + "/" + data.Guid + ".json", str);
            Console.WriteLine("Saved: " + data.Guid);
        }
    }
}

