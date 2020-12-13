using System;

[Flags]
internal enum Permissions : byte
{
    Player = 0,
    DonateBacker = 1,
    YouTube = 2,
    Helper = 4,
    GlobalAdmin = 8,
    Developer = 16,
}

