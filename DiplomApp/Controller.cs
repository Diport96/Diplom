﻿using DiplomApp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace DiplomApp
{
    abstract class Controller : INotifyPropertyChanged
    {
        private string name;

        public Guid ID { get; }
        public CType Type { get; }
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }
        abstract public string Value { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public Controller(Guid id, string name)
        {
            ID = id;
            Type = TypeInit();
            Name = name;
        }

        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        internal abstract CType TypeInit();
    }
}