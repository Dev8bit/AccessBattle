﻿using AccessBattle.Networking;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessBattle.Wpf.Interfaces
{
    public interface IMenuViewModel : INotifyPropertyChanged
    {
        IMenuHolder ParentViewModel { get; }
    }

    public interface IMenuHolder
    {
        MenuType CurrentMenu { get; set; }
        NetworkGameClient NetworkClient { get; }
    }
}
