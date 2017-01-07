﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccessBattle
{
    /// <summary>
    /// Board is 8x8 using Chess notation 
    /// as base for position indexes
    ///   a b c d e f g h
    /// 8                 8  7
    /// 7                 7  6
    /// ...               ...
    /// 2                 2  1
    /// 1                 1  0
    ///   a b c d e f g h
    ///   0 1 2 3 4 5 6 7
    /// 
    /// X is horizontal, Y is vertical
    /// (0,0) is a1, (7,7) is h8
    /// 
    /// Stack:
    /// For simplification, 
    /// the first 4 fields are always links.
    /// Board orientation is ignored
    /// Player1: Fields (0,8) - (7,8)
    /// Player2: Fields (0,9) - (7,9)
    /// </summary>
    public class Board : PropChangeNotifier
    {
        public BoardField[,] Fields { get; private set; }
        public List<OnlineCard> OnlineCards { get; private set; }        
        public List<FirewallCard> Firewalls { get; private set; }

        #region Little Helpers
        BoardField[] _player1DeploymentFields;
        BoardField[] _player2DeploymentFields;
        public List<BoardField> GetPlayerDeploymentFields(int playerNumber)
        {
            if (playerNumber == 1) return _player1DeploymentFields.ToList();
            if (playerNumber == 2) return _player2DeploymentFields.ToList();
            return new List<BoardField>();
        }
        BoardField[] _player1StackFields; // First 4 are link fields
        BoardField[] _player2StackFields; // First 4 are link fields
        public List<BoardField> GetPlayerStackFields(int playerNumber)
        {
            if (playerNumber == 1) return _player1StackFields.ToList();
            if (playerNumber == 2) return _player2StackFields.ToList();
            return new List<BoardField>();
        }
        #endregion

        public Board()
        {
            Fields = new BoardField[8, 10];
            for (ushort x = 0; x < 8; ++x)
                for (ushort y = 0; y < 10; ++y)
                {
                    var type = BoardFieldType.Main;
                    if (y > 7) type = BoardFieldType.Stack;
                    else if ((x > 2 && x < 5) && (y == 0 || y == 7)) type = BoardFieldType.Exit;
                    Fields[x, y] = new BoardField(x, y, type);
                }
                    

            OnlineCards = new List<OnlineCard>();
            for (int i = 0; i < 16; ++i)
                OnlineCards.Add(new OnlineCard());

            _player1DeploymentFields = new BoardField[] 
            { Fields[0,0],Fields[1,0],Fields[2,0],Fields[3,1],Fields[4,1],Fields[5,0],Fields[6,0],Fields[7,0] };
            _player2DeploymentFields = new BoardField[]
            { Fields[0,7],Fields[1,7],Fields[2,7],Fields[3,6],Fields[4,6],Fields[5,7],Fields[6,7],Fields[7,7] };

            _player1StackFields = new BoardField[]
            { Fields[0,8],Fields[1,8],Fields[2,8],Fields[3,8],Fields[4,8],Fields[5,8],Fields[6,8],Fields[7,8] };
            _player2StackFields = new BoardField[]
            { Fields[0,9],Fields[1,9],Fields[2,9],Fields[3,9],Fields[4,9],Fields[5,9],Fields[6,9],Fields[7,9] };
        }


        public bool PlaceCard(ushort x, ushort y, Card card)
        {
            if (x > 7 || y > 9 || card == null)
                return false;
            var field = Fields[x, y];
            if (field.Card != null) return false;
            field.Card = card;
            card.Location = field;
            return true;
        }
        public bool PlaceCard(Vector loc, Card card)
        {
            return PlaceCard(loc.X, loc.Y, card);
        }
    }

    public class BoardField : PropChangeNotifier
    {        
        public Vector Position { get; private set; }
        Card _card;
        public Card Card
        {
            get { return _card; }
            set { SetProp(ref _card, value); }
        }

        public BoardFieldType Type { get; private set; }

        public BoardField(ushort x, ushort y, BoardFieldType type)
        {
            Position = new Vector(x,y);
            Type = type;
        }
    }

    public enum BoardFieldType
    {
        Main,
        Exit,
        Stack
    }
}
