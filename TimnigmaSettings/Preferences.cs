// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using MadMilkman.Ini;

namespace TimnigmaSettings
{
    [Serializable]
    public class Preferences : INotifyPropertyChanged
    {
        private string fontface;

        [IniSerialization("Font")]
        public string FontFace
        {
            get { return fontface; }
            set
            {
                fontface = value;
                OnPropertyChanged();
            }
        }

        private string fontsize;

        [IniSerialization("FontSize")]
        public string FontSize
        {
            get { return fontsize; }
            set
            {
                fontsize = value;
                OnPropertyChanged();
            }
        }

        private string ttType;

        [IniSerialization("TTType")]
        public string TooltipType
        {
            get { return ttType; }
            set
            {
                ttType = value;
                OnPropertyChanged();
            }
        }

        private string powershell;

        [IniSerialization("PowerShell")]
        public string PowerShell
        {
            get { return powershell; }
            set
            {
                powershell = value;
                OnPropertyChanged();
            }
        }

        private string hwversion;

        [IniSerialization("HWVersion")]
        public string HWVersion
        {
            get { return hwversion; }
            set
            {
                hwversion = value;
                OnPropertyChanged();
            }
        }

        private string barTranparency;

        [IniSerialization("BarTransparency")]
        public string BarTransparency
        {
            get { return barTranparency; }
            set
            {
                barTranparency = value;
                OnPropertyChanged();
            }
        }

        private string barHeight;

        [IniSerialization("BarHeight")]
        public string BarHeight
        {
            get { return barHeight; }
            set
            {
                barHeight = value;
                OnPropertyChanged();
            }
        }

        private string menuNormalAlpha;

        [IniSerialization("MenuNormalAlpha")]
        public string MenuNormalAlpha
        {
            get { return menuNormalAlpha; }
            set
            {
                menuNormalAlpha = value;
                OnPropertyChanged();
            }
        }

        private string menuHoverAlpha;

        [IniSerialization("MenuHoverAlpha")]
        public string MenuHoverAlpha
        {
            get { return menuHoverAlpha; }
            set
            {
                menuHoverAlpha = value;
                OnPropertyChanged();
            }
        }

        private string tjMaxTemp;

        [IniSerialization("TjMaxTemp")]
        public string TjMaxTemp
        {
            get { return tjMaxTemp; }
            set
            {
                tjMaxTemp = value;
                OnPropertyChanged();
            }
        }

        private string barColor;

        [IniSerialization("BarColor")]
        public string BarColor
        {
            get { return barColor; }
            set
            {
                barColor = value;
                OnPropertyChanged();
            }
        }

        private string colorText;

        [IniSerialization("ColorText")]
        public string ColorText
        {
            get { return colorText; }
            set
            {
                colorText = value;
                OnPropertyChanged();
            }
        }

        private string colorRed;

        [IniSerialization("ColorRed")]
        public string ColorRed
        {
            get { return colorRed; }
            set
            {
                colorRed = value;
                OnPropertyChanged();
            }
        }

        private string colorGreen;

        [IniSerialization("ColorGreen")]
        public string ColorGreen
        {
            get { return colorGreen; }
            set
            {
                colorGreen = value;
                OnPropertyChanged();
            }
        }

        private string colorYellow;

        [IniSerialization("ColorYellow")]
        public string ColorYellow
        {
            get { return colorYellow; }
            set
            {
                colorYellow = value;
                OnPropertyChanged();
            }
        }

        private string netMaxDown;

        [IniSerialization("NetMaxDown")]
        public string NetMaxDown
        {
            get { return netMaxDown; }
            set
            {
                netMaxDown = value;
                OnPropertyChanged();
            }
        }

        private string netMaxUp;

        [IniSerialization("NetMaxUp")]
        public string NetMaxUp
        {
            get { return netMaxUp; }
            set
            {
                netMaxUp = value;
                OnPropertyChanged();
            }
        }

        private string myOWMApiKey;
        [IniSerialization("MyOWMApiKey")]

        public string MyOWMApiKey
        {
            get { return myOWMApiKey; }
            set
            {
                myOWMApiKey = value;
                OnPropertyChanged();
            }
        }


        #region Handle property change event
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            Debug.WriteLine($"{propertyName} changed");
        }
        #endregion Handle property change event
    }
}
