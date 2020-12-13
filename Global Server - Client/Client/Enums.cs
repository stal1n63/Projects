public enum RPC : ushort
{
    ConnectionPlayerData,
    InstintiateSimpleObject,
    DestroySimpleObject,
    UpdateSimpleObject,
    InstintiatePlayer,
    DestroyPlayer,
    UpdatePlayer,
    DeliverDamage,
    ChatSend,
    ChangeTeam,
    ChangeClass,
    ConnectionEndSync,
    PlayerToRagdoll,
    DestroyMapObject,
    SessionEnded,
    NewSession_TDM,
}

public enum GlobalServerTransmissionEnum : byte
{
    AddServerInList = 253,
    UpdateServerInList,
    DestroyServerInList
}

public enum Gamemodes
{
    TDM,
    EndSession
}