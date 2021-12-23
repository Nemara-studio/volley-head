using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VollyHead.Online
{
    public struct ServerMatchMessage : NetworkMessage
    {
        public ServerMatchOperation serverMatchOperation;
        public string matchId;
        public PlayerInfo playerInfo;
    }

    public struct ClientMatchMessage : NetworkMessage
    {
        public ClientMatchOperation clientMatchOperation;
        public PlayerInfo player;
        public MatchInfo matchInfo;
    }

    public enum ServerMatchOperation : byte
    {
        None,
        Connect,
        ChangeName,
        Create,
        Cancel,
        Start,
        Join,
        Leave,
        Ready,
        ChangeTeam,
    }

    public enum ClientMatchOperation : byte
    {
        None,
        ChangeName,
        Created,
        Cancelled,
        Joined,
        Departed,
        UpdateRoom,
        ChangedTeam,
        Started
    }
}