﻿namespace AccessBattle.Networking
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    /// <summary>
    /// This class contains constants for the packet types that are exchanged.
    /// </summary>
    public class NetworkPacketType
    {
        public const byte PublicKey = 0x01;
        public const byte ClientLogin = 0x02;
        // Packets above 0x02 require user to be logged in
        public const byte ListGames = 0x03;
        public const byte CreateGame = 0x04;
        public const byte JoinGame = 0x05;
        public const byte GameInit = 0x06;
        public const byte GameSync = 0x07;
        public const byte GameCommand = 0x08;
        public const byte ExitGame = 0x09;
        public const byte Rematch = 0x0A;

        /// <summary>Information about server.</summary>
        public const byte ServerInfo = 0xFE;
        /// <summary>Sent by the server to check if the client is still connected.</summary>
        public const byte KeepAlive = 0xFF; // TODO: Implement
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
