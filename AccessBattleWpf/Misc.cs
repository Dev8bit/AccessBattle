﻿using AccessBattle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessBattleWpf
{
    public class BoardFieldClickedEventArgs : EventArgs
    {
        public BoardFieldViewModel Field { get; private set; }
        public BoardFieldClickedEventArgs(BoardFieldViewModel field)
        {
            Field = field;
        }
    }

    public enum BoardFieldViewDisplayState
    {
        Empty,
        MainVirus,
        //MainVirusBoosted,      
        MainLink,
        //MainLinkBoosted,
        OnlineCardFlipped,
        MainFirewall,  
        StackVirusEmpty, // Stack fields show always symbol
        StackLinkEmpty,
        StackVirus,
        StackLink,
        LineBoost, // Use only for line boost selection field
        Firewall, // Use only for firewall selection field
        VirusCheck, // Use only for firewall selection field
        NotFound404, // Use only for firewall selection field
        ExitEmpty,
        ExitVirus,
        ExitLink,
        //ExitVirusBoosted,
        //ExitLinkBoosted,
    }
}
