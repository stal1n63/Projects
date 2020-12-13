using System;
using Lidgren.Network;
using System.Threading;

public static class Program
{
    public const string VERSION = "MS 6.0_dev";
     
    public const int PORT = 57125;

    public const string PATH_BALANCE_FILE = @"/items.json";
    public const string PATH_PLAYERS_DATA = @"/players/";
    public static NetServer peer = null;

    private static MemoryOperations memory; //avoid gc

    static void Main(string[] args)
    {
        RPCAbleMethods.Load();

        NetPeerConfiguration config = new NetPeerConfiguration("of_masterserver")
        {
            Port = PORT,
            MaximumConnections = (2^32 - 1),
            AcceptIncomingConnections = true,
        };

        peer = new NetServer(config);
        peer.Start();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("> Server started, server version: " + VERSION);
        Console.ForegroundColor = ConsoleColor.White;

        memory = new MemoryOperations();
        ServerLoop.StartServerLoop(peer);
        
        while (ServerLoop.EnabledLooping)
        {
            Thread.Sleep(1000);
        }
    }
}
