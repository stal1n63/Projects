using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Lidgren.Network;

class TDM : Server
{
    public enum TDMData : byte { SendStart = 200, SendScore, SendAllScore, SendTime }

    public ushort TeamBlueScore;
    public ushort TeamRedScore;

    public delegate void GameStatus();
    public event GameStatus SessionFinished;

    public int Time;
    private Timer timer;

    public TDM()
    {
        TeamBlueScore = server_config.ScoreTeamNATO;
        TeamRedScore = server_config.ScoreTeamWP;

        Killed += KillCheck;

        Time = server_config.GameTime;
        TimerCallback tm = new TimerCallback(UpdateGameTime);
        timer = new Timer(tm, null, 0, 1000);

        foreach (var conn in _server.Connections)
        {
            NetOutgoingMessage mesg = _server.CreateMessage();
            mesg.Write((byte)TDMData.SendScore);
            mesg.Write(TeamRedScore);
            mesg.Write(TeamBlueScore);
            _server.SendMessage(mesg, conn, NetDeliveryMethod.ReliableOrdered);
        }
        SessionStatus = true;
    }

    public void KillCheck(byte team)
    {
        if (team == 0)
        {
            TeamRedScore -= 1;
        }
        else
        {
            TeamBlueScore -= 1;
        }
        foreach (var conn in _server.Connections)
        {
            NetOutgoingMessage msg = _server.CreateMessage();
            msg.Write((byte)TDMData.SendScore);
            msg.Write(TeamRedScore);
            msg.Write(TeamBlueScore);
            _server.SendMessage(msg, conn, NetDeliveryMethod.ReliableOrdered);
        }
        if (TeamRedScore == 0 || TeamBlueScore == 0)
        {
            SessionFinished?.Invoke();
        }
    }

    public void UpdateGameTime(object obj)
    {
        Time -= 1;
        foreach (var conn in _server.Connections)
        {
            NetOutgoingMessage msg = _server.CreateMessage();
            msg.Write((byte)TDMData.SendTime);
            msg.Write(Time);
            _server.SendMessage(msg, conn, NetDeliveryMethod.UnreliableSequenced);
        }
        if (Time <= 0)
        {
            Killed -= KillCheck;
            SessionFinished?.Invoke();
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }
}

