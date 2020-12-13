using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class AdminCommands
{
    public enum AdminCommandsEnum : byte
    {
        Ban,
        ReloadBalanceData,
        Register,
        Shutdown
    }

    [OFRPC((ushort)ReceivedMessageID.AdminCommands)]
    public static void AdminCommandsHandler(NetIncomingMessage message)
    {
        switch ((AdminCommandsEnum)message.ReadByte())
        {
            case AdminCommandsEnum.Ban:
                Ban(message);
                break;
            case AdminCommandsEnum.ReloadBalanceData:
                ReloadBalanceData(message);
                break;
            case AdminCommandsEnum.Register:
                bool res = RegisterPlayer(message);
                NetOutgoingMessage msg = Program.peer.CreateMessage();
                msg.Write((ushort)ReceivedMessageID.AdminCommands);
                msg.Write((byte)AdminCommandsEnum.Register);
                msg.Write(res);
                Program.peer.SendMessage(msg, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                break;
            case AdminCommandsEnum.Shutdown:
                Shutdown(message);
                break;
        }
    }

    public static void Ban(NetIncomingMessage message)
    {
        string AdminLogin = message.ReadString();
        string AdminPassword = message.ReadString();
        string BanNickname = message.ReadString();

        try
        {
            PlayerInfo request = MemoryOperations.PlayersData.Keys.FirstOrDefault(x => x.Nickname == AdminLogin);
            if (request.Password == AdminPassword && request.AccessLevel >= 4)
            {
                PlayerInfo banned = MemoryOperations.PlayersData.Keys.First(x => x.Nickname == BanNickname);
                banned.IsBanned = true;
            }
        }
        catch { }
    }

    public static void ReloadBalanceData(NetIncomingMessage message)
    {
        string AdminLogin = message.ReadString();
        string AdminPassword = message.ReadString();
        string BanNickname = message.ReadString();

        try
        {
            PlayerInfo request = MemoryOperations.PlayersData.Keys.FirstOrDefault(x => x.Nickname == AdminLogin);
            if (request.Password == AdminPassword && request.AccessLevel >= 16)
            {
                MemoryOperations.LoadGameBalance(Program.PATH_BALANCE_FILE);
                Commands.BroadcastBalanceData();
            }
        }
        catch { }
    }

    public static bool RegisterPlayer(NetIncomingMessage message)
    {
        string AdminLogin = message.ReadString();
        string AdminPassword = message.ReadString();
        string RegisterNickname = message.ReadString();
        string RegisterPassword = message.ReadString();

        PlayerInfo request = MemoryOperations.PlayersData.Keys.FirstOrDefault(x => x.Nickname == AdminLogin);
        if (request.Password == AdminPassword && request.AccessLevel >= 4)
        {
            PlayerInfo newplayer = new PlayerInfo
            {
                Guid = Guid.NewGuid().ToString(),
                Nickname = RegisterNickname,
                Password = RegisterPassword,
                Played_Time = 0,
                AccessLevel = 0,
                Deaths = 0,
                Kills = 0,
                Score = 0,
                Level = 0,
                IsBanned = false,
                Unlocked_Count = 0
            };
            newplayer.Unlocked_Weapons = new Dictionary<string, ItemWeapon>();
            newplayer.Unlocked_Skin = new Dictionary<string, ItemSkin>();
            newplayer.Unlocked_Melee = new Dictionary<string, ItemMelee>();
            newplayer.Unlocked_Grenages = new Dictionary<string, ItemGrenage>();

            if (MemoryOperations.BalanceData.Weapons != null)
            {
                foreach (var item in MemoryOperations.BalanceData.Weapons)
                {
                    if (item.Value.Unlock_Price == 0 & item.Value.LevelToUnlock == 0)
                    {
                        ItemWeapon addedWeapon = new ItemWeapon
                        {
                            Kills = 0
                        };
                        addedWeapon.Unlocked_Magazines = new List<string>();
                        addedWeapon.Unlocked_Rails = new List<string>();
                        addedWeapon.Unlocked_Scopes = new List<string>();
                        addedWeapon.Unlocked_Skins = new List<string>();
                        addedWeapon.Unlocked_Special = new List<string>();

                        if (item.Value.Rails != null)
                        {
                            foreach (var rail in item.Value.Rails)
                            {
                                if (rail.Value.Unlock_Price == 0)
                                {
                                    addedWeapon.Unlocked_Rails.Add(rail.Key);
                                }
                            }
                        }
                        
                        if (item.Value.Scopes != null)
                        {
                            foreach (var scope in item.Value.Scopes)
                            {
                                if (scope.Value.Unlock_Price == 0)
                                {
                                    addedWeapon.Unlocked_Scopes.Add(scope.Key);
                                }
                            }
                        }
                        
                        if (item.Value.Magazines != null)
                        {
                            foreach (var magazine in item.Value.Magazines)
                            {
                                if (magazine.Value.Unlock_Price == 0)
                                {
                                    addedWeapon.Unlocked_Magazines.Add(magazine.Key);
                                }
                            }
                        }
                        
                        if (item.Value.Special != null)
                        {
                            foreach (var special in item.Value.Special)
                            {
                                if (special.Value.Unlock_Price == 0)
                                {
                                    addedWeapon.Unlocked_Special.Add(special.Key);
                                }
                            }
                        }
                        
                        if (item.Value.Skins != null)
                        {
                            foreach (var skin in item.Value.Skins)
                            {
                                if (skin.Value.Unlock_Price == 0)
                                {
                                    addedWeapon.Unlocked_Skins.Add(skin.Key);
                                }
                            }
                        }
                        
                        newplayer.Unlocked_Weapons.Add(item.Key, addedWeapon);
                        
                    }
                }
            }
            
            if (MemoryOperations.BalanceData.Skins != null)
            {
                foreach (var item in MemoryOperations.BalanceData.Skins)
                {

                    if (item.Value.Unlock_Price == 0 & item.Value.LevelToUnlock == 0)
                    {
                        ItemSkin addSkin = new ItemSkin
                        {
                            Kills = 0
                        };
                        addSkin.Unlocked_Skins = new List<string>();

                        if (item.Value.Skins != null)
                        {
                            foreach (var skin in item.Value.Skins)
                            {
                                if (skin.Value.Unlock_Price == 0)
                                {
                                    addSkin.Unlocked_Skins.Add(skin.Key);
                                }
                            }
                        }
                        newplayer.Unlocked_Skin.Add(item.Key, addSkin);
                    }
                }
            }
            /*
            if (MemoryOperations.BalanceData.Knifes != null)
            {
                foreach (var item in MemoryOperations.BalanceData.Knifes)
                {
                    if (item.Value.Unlock_Price == 0 & item.Value.LevelToUnlock == 0)
                    {
                        ItemMelee addMelee = new ItemMelee
                        {
                            Kills = 0
                        };
                        addMelee.Unlocked_Skins = new List<string>();

                        if (item.Value.Skins != null)
                        {
                            foreach (var skin in item.Value.Skins)
                            {
                                if (skin.Value.Unlock_Price == 0)
                                {
                                    addMelee.Unlocked_Skins.Add(skin.Key);
                                }
                            }
                        }

                        newplayer.Unlocked_Melee.Add(item.Key, addMelee);
                    }
                }
            }
            *//*
            if (MemoryOperations.BalanceData.Knifes != null)
            {
                foreach (var item in MemoryOperations.BalanceData.Grenages)
                {
                    if (item.Value.Unlock_Price == 0 & item.Value.LevelToUnlock == 0)
                    {
                        ItemGrenage addGrenage = new ItemGrenage
                        {
                            Kills = 0
                        };
                        addGrenage.Unlocked_Skins = new List<string>();

                        if (item.Value.Skins != null)
                        {
                            foreach (var skin in item.Value.Skins)
                            {
                                if (skin.Value.Unlock_Price == 0)
                                {
                                    addGrenage.Unlocked_Skins.Add(skin.Key);
                                }
                            }
                        }
                        newplayer.Unlocked_Grenages.Add(item.Key, addGrenage);
                    }
                }
            }*/
            
            bool success = false;
            while (!success)
            {
                success = MemoryOperations.PlayersData.TryAdd(newplayer, true);
            }
            Console.WriteLine("Registered {0} {1}", newplayer.Nickname, newplayer.Guid);
            return true;

        }
        else
        {
            return false;
        }
    }

    private static void Shutdown(NetIncomingMessage message)
    {
        string AdminLogin = message.ReadString();
        string AdminPassword = message.ReadString();

        Console.WriteLine("Shutdown");
        try
        {
            PlayerInfo request = MemoryOperations.PlayersData.Keys.FirstOrDefault(x => x.Nickname == AdminLogin);
            if (request.Password == AdminPassword && request.AccessLevel >= 16)
            {
                MemoryOperations.RefreshPlayersAccountsInMemory(0);
                ServerLoop.CloseLooping();
            }
        }
        catch { Environment.Exit(0); }
    }
}

