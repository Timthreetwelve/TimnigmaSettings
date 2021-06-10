// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

using System.ComponentModel;
using System.Runtime.CompilerServices;
using TKUtils;

namespace TimnigmaSettings
{
    public class UserSettings : SettingsManager<UserSettings>, INotifyPropertyChanged
    {
        #region Constructor
        public UserSettings()
        {
            // Set defaults
            WindowLeft = 100;
            WindowTop = 100;
        }
        #endregion Constructor

        #region Properties

        public double WindowLeft
        {
            get
            {
                if (windowLeft < 0)
                {
                    windowLeft = 0;
                }
                return windowLeft;
            }
            set => windowLeft = value;
        }

        public double WindowTop
        {
            get
            {
                if (windowTop < 0)
                {
                    windowTop = 0;
                }
                return windowTop;
            }
            set => windowTop = value;
        }
        #endregion Properties

        #region Private backing fields
        private double windowLeft;
        private double windowTop;
        #endregion Private backing fields

        #region Handle property change event
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion Handle property change event
    }
}
