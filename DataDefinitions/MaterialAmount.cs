﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EddiDataDefinitions
{
    public class MaterialAmount : INotifyPropertyChanged
    {
        public string material { get; private set; }

        [JsonIgnore]
        private int _amount;
        public int amount { get
            {
                return _amount;
            }
            set
            {
                if (_amount != value)
                {
                    _amount = value;
                    NotifyPropertyChanged("amount");
                }
            }
        }

        [JsonIgnore]
        private int? _minimum;
        public int? minimum
        {
            get
            {
                return _minimum;
            }
            set
            {
                if (_minimum != value)
                {
                    _minimum = value;
                    NotifyPropertyChanged("minimum");
                }
            }
        }

        [JsonIgnore]
        private int? _desired;
        public int? desired
        {
            get
            {
                return _desired;
            }
            set
            {
                if (_desired != value)
                {
                    _desired = value;
                    NotifyPropertyChanged("desired");
                }
            }
        }

        [JsonIgnore]
        private int? _maximum;
        public int? maximum
        {
            get
            {
                return _maximum;
            }
            set
            {
                if (_maximum != value)
                {
                    _maximum = value;
                    NotifyPropertyChanged("maximum");
                }
            }
        }

        public MaterialAmount(Material material, int amount)
        {
            this.material = material.name;
            this.amount = amount;
        }

        public MaterialAmount(Material material, int? minimum, int? desired, int? maximum)
        {
            this.material = material.name;
            amount = 0;
            this.minimum = minimum;
            this.desired = desired;
            this.maximum = maximum;
        }

        [JsonConstructor]
        public MaterialAmount(string material, int amount, int? minimum, int? desired, int? maximum)
        {
            this.material = material;
            this.amount = amount;
            this.minimum = minimum;
            this.desired = desired;
            this.maximum = maximum;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
