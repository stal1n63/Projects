using System;
using System.Collections.Generic;
using System.Net;

#region PlayerData
[Serializable]
public class PlayerInfo
{
    public string Guid { get; set; }
    public string Nickname { get; set; }
    public string Password { get; set; }
    public byte AccessLevel { get; set; }

    public bool IsBanned { get; set; }

    public int Kills { get; set; }
    public int Deaths { get; set; }

    public int Score { get; set; }
    public byte Level { get; set; }

    public int Played_Time { get; set; }
    public int Unlocked_Count { get; set; }

    public Dictionary<string, ItemWeapon> Unlocked_Weapons { get; set; }
    public Dictionary<string, ItemSkin> Unlocked_Skin { get; set; }
    public Dictionary<string, ItemGrenage> Unlocked_Grenages { get; set; }
    public Dictionary<string, ItemMelee> Unlocked_Melee { get; set; }
}

public class ItemWeapon
{
    public int Kills { get; set; }
    public List<string> Unlocked_Scopes { get; set; }
    public List<string> Unlocked_Rails { get; set; }
    public List<string> Unlocked_Magazines { get; set; }
    public List<string> Unlocked_Special { get; set; }
    public List<string> Unlocked_Skins { get; set; }
}

public class ItemSkin
{
    public int Kills { get; set; }
    public List<string> Unlocked_Skins { get; set; }
}

public class ItemSpecialization
{
    public int Kills { get; set; }
}

public class ItemGrenage
{
    public int Kills { get; set; }
    public List<string> Unlocked_Skins { get; set; }
}

public class ItemMelee
{
    public int Kills { get; set; }
    public List<string> Unlocked_Skins { get; set; }
}
#endregion

#region Game/BalanceData
public class ItemsData
{
    public string ActualGameVersion { get; set; }
    public float GlobalXPModifer { get; set; } //actually need to be 1f
    public float PerUnlockModifer { get; set; } //1.2f
    public Dictionary<string, Weapon> Weapons { get; set; }
    public Dictionary<string, Skin> Skins { get; set; }
    public Dictionary<string, Grenage> Grenages { get; set; }
    public Dictionary<string, Specialization> Specialization { get; set; }
    public Dictionary<byte, Teams> Teams { get; set; }
}

#region WeaponsSerialization
public class Weapon
{
    public string Weapon_InGameText { get; set; }
    public ushort Unlock_Price { get; set; }
    public string Weapon_ShootModes { get; set; }
    public byte LevelToUnlock { get; set; }

    public Dictionary<string, Weapon_RailAttachment> Rails { get; set; }
    public Dictionary<string, Weapon_MagazineAttachment> Magazines { get; set; }
    public Dictionary<string, Weapon_ScopeAttachment> Scopes { get; set; }
    public Dictionary<string, Weapon_SecondWeaponMode> Special { get; set; }

    public Dictionary<string, AdditionalSkin> Skins { get; set; }
}

public class Weapon_RailAttachment
{
    public string Attachment_InGameText { get; set; }
    public ushort Unlock_Price { get; set; }

    public ushort Dispersion { get; set; } //cm per 100m
    public float Recoil_Up { get; set; } //degrees per one shot
    public float Recoil_Left { get; set; }
    public float Recoil_Right { get; set; }
    public float UnZoomed_Factor { get; set; } //multipler recoil

    public ushort Damage_HightEndDistance { get; set; } //m
    public ushort Damage_LowStartDistance { get; set; } //m
    public sbyte Damage_Change { get; set; }

    public ushort AdditionalLenght { get; set; } //cm
}

public class Weapon_MagazineAttachment
{
    public string Attachment_InGameText { get; set; }
    public ushort Unlock_Price { get; set; }

    public ushort Magazine_Capacity { get; set; }
    public ushort All_Capacity { get; set; }
    public ushort Damage_Max { get; set; }
    public ushort Damage_Min { get; set; }
    public byte Penetration { get; set; }
    public ushort Explode_Force { get; set; }
    public ushort Explode_Range { get; set; }

    public float Bullet_Drop { get; set; } //m/s^2, not gravitation modifer - that all forses in system!
    public ushort Bullet_Speed { get; set; }
}

public class Weapon_ScopeAttachment
{
    public string Attachment_InGameText { get; set; }
    public ushort Unlock_Price { get; set; }

    public byte Scope_FOV { get; set; }
    public string[] Scope_Sprites { get; set; }
}

public class Weapon_SecondWeaponMode //if available, can be null
{
    public string Attachment_InGameText { get; set; }
    public ushort Unlock_Price { get; set; }

    public ushort Magazine_Capacity { get; set; }
    public ushort All_Capacity { get; set; }
    public ushort Damage { get; set; }
    public byte Penetration { get; set; }
    public ushort Explode_Force { get; set; }
    public ushort Explode_Range { get; set; }

    public float Bullet_Drop { get; set; } //   m/s^2
    public ushort Bullet_Speed { get; set; }
}
#endregion

#region PlayerSkinsSerialization
public class Skin
{
    public string Skin_InGameText { get; set; }
    public ushort Unlock_Price { get; set; }
    public byte LevelToUnlock { get; set; }

    public Dictionary<string, AdditionalSkin> Skins { get; set; }
}
#endregion

#region Knifes/Grenages
public class Grenage
{
    public string Grenage_InGameText { get; set; }
    public ushort Unlock_Price { get; set; }
    public byte LevelToUnlock { get; set; }

    public ushort Damage { get; set; }
    public byte Penetration { get; set; }
    public ushort Explode_Force { get; set; }
    public ushort Explode_Range { get; set; } //cm

    public float Grenage_Drop { get; set; } //m/s^2

    public Dictionary<string, AdditionalSkin> Skins { get; set; }
}
#endregion

#region Specialization
public class Specialization
{
    public string Specialization_InGameText { get; set; }
    public string Command { get; set; }
}
#endregion

public class AdditionalSkin
{
    public string Skin_InGameText { get; set; }
    public ushort Unlock_Price { get; set; }
}

#region Teams/Classes

public class Teams
{
    public List<string> TeamsAbleToThisPattern { get; set; }

    public Dictionary<string, ClassAvailable> Classes { get; set; }

    public Dictionary<string, Wehicle> LighWehicles { get; set; }
    public Dictionary<string, Wehicle> HeavyWehicles { get; set; }
}

public class ClassAvailable
{
    public byte Class_Health { get; set; }
    public ushort Class_Speed { get; set; }

    public List<string> Mains { get; set; }
    public List<string> Secondaries { get; set; }
    public List<string> ClassSpecials { get; set; }
    public List<string> Grenages { get; set; }
    public List<string> Skins { get; set; }
    public List<string> Specializations { get; set; }
}

public class Wehicle
{ 
    public int Health { get; set; }

    public ushort MaxSpeed { get; set; }
    public byte Gears { get; set; }
    public ushort MaxRPM { get; set; }
    public int Mass { get; set; }

    public string InGameText { get; set; }
    public int UnlockPrice { get; set; }
    public byte LevelToUnlock { get; set; }

    public Dictionary<string, WehicleAmmo> Ammos { get; set; }
    public Dictionary<string, AdditionalSkin> Skins { get; set; }
}

public class WehicleAmmo
{
    public string InGameText { get; set; }
    public int UnlockPrice { get; set; }

    public int Damage { get; set; }
    public ushort Damage_HightEndDistance { get; set; } //m
    public ushort Damage_LowStartDistance { get; set; } //m
    public ushort Magazine_Capacity { get; set; }
    public ushort All_Capacity { get; set; }
    public byte Penetration { get; set; }
    public ushort Explode_Force { get; set; }
    public ushort Explode_Range { get; set; }

    public string Weapon_ShootModes { get; set; }

    public float Bullet_Drop { get; set; } //m/s^2, not gravitation modifer - that all forses in system!
    public ushort Bullet_Speed { get; set; }
}
#endregion
#endregion

public class ServerInfo
{
    public double LastServerUpdateTime;
    public IPEndPoint InternalIP;
    public IPEndPoint ExternalIP;
    public byte Players_Max { get; set; }
    public byte Players_Connected { get; set; }
    public string Server_Name { get; set; }
    public string Server_OnLoadMessage { get; set; }
    public string Gamemode { get; set; }
    public string Map { get; set; }

    public string TeamLeft { get; set; }
    public string TeamRight { get; set; }
    public ushort TeamLestScore { get; set; }
    public ushort TeamRightScore { get; set; }
}