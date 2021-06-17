// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

#region Using declarations
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MadMilkman.Ini;
using NLog;
using TKUtils;
#endregion Using declarations

namespace TimnigmaSettings
{
    public partial class MainWindow : Window
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private readonly IniFile preferencesIni = new IniFile();
        private Preferences prefs;
        private string configFile;
        private string hwinfoFile;

        public MainWindow()
        {
            UserSettings.Init(UserSettings.AppFolder, UserSettings.DefaultFilename, true);

            InitializeComponent();

            ReadSettings();

            AddMissingKeys();
        }

        #region Settings
        private void ReadSettings()
        {
            // Change the log file filename when debugging
            string env = Debugger.IsAttached ? "debug" : "temp";
            GlobalDiagnosticsContext.Set("TempOrDebug", env);

            // Startup message in the temp file
            log.Info($"{AppInfo.AppName} {AppInfo.TitleVersion} is starting up");

            // Window position & Size
            Top = UserSettings.Setting.WindowTop;
            Left = UserSettings.Setting.WindowLeft;

            string mydocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            configFile = Path.Combine(mydocs, @"Rainmeter\Skins\Timnigma\_IncludeFiles\Preferences.inc");
            hwinfoFile = Path.Combine(mydocs, @"Rainmeter\Skins\Timnigma\_IncludeFiles\HWiNFO.inc");
            log.Info($"Preferences file is {configFile}.");
            log.Info($"HWiNFO configuration file is {hwinfoFile}.");

            if (File.Exists(configFile))
            {
                preferencesIni.Load(configFile);
                prefs = preferencesIni.Sections["Variables"].Deserialize<Preferences>();
                DataContext = prefs;
                //tbpath.Text = $"Settings file: {configFile}";
            }
            else
            {
                //tbpath.Text = $"Settings file: {configFile} NOT FOUND";
                _ = MessageBox.Show($"Cannot find\n{configFile}\n\nUnable to continue.",
                    "ERROR",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                log.Fatal($"{configFile} not found.");
                Application.Current.Shutdown();
            }
        }
        #endregion Settings

        #region Find any keys missing from Preferences.inc
        private List<string> FindMissingKeys()
        {
            // Create list of preferences
            Preferences pref = new Preferences();
            List<string> prefList = new List<string>();
            foreach (PropertyInfo p in pref.GetType().GetProperties())
            {
                prefList.Add(p.Name);
            }

            // Create list of keys in ini file
            IniSection section = preferencesIni.Sections["Variables"];
            IniKeyCollection iniKeys = section.Keys;
            List<string> iniList = new List<string>();
            foreach (IniKey item in iniKeys)
            {
                iniList.Add(item.Name);
            }

            // return items in preferences list that do not have a key in preferences.ini
            return prefList.Except(iniList, StringComparer.OrdinalIgnoreCase).ToList();
        }
        #endregion Find any keys missing from Preferences.inc

        #region Add missing keys
        private void AddMissingKeys()
        {
            if (FindMissingKeys().Count > 0)
            {
                foreach (string item in FindMissingKeys())
                {
                    try
                    {
                        _ = preferencesIni.Sections["Variables"].Keys.Add(item);
                        log.Debug($"Added key: {item} to Preferences.inc");
                    }
                    catch (Exception ex)
                    {
                        log.Debug($"Failed to add missing key [{item}] in Preferences.inc");
                        _ = MessageBox.Show($"Failed to add missing key [{item}] in Preferences.inc\n{ex}",
                        "Error",
                                            MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    }
                }
                preferencesIni.Save(configFile);
            }
        }
        #endregion Add missing keys

        #region Save Preferences.inc file
        private void SaveIni()
        {
            preferencesIni.Sections["Variables"].Keys["Font"].Value = prefs.Font;
            preferencesIni.Sections["Variables"].Keys["FontSize"].Value = prefs.FontSize;
            preferencesIni.Sections["Variables"].Keys["TTType"].Value = prefs.TTType;
            preferencesIni.Sections["Variables"].Keys["PowerShell"].Value = prefs.PowerShell;
            preferencesIni.Sections["Variables"].Keys["HWversion"].Value = prefs.HWVersion;
            preferencesIni.Sections["Variables"].Keys["BarTransparency"].Value = prefs.BarTransparency;
            preferencesIni.Sections["Variables"].Keys["BarHeight"].Value = prefs.BarHeight;
            preferencesIni.Sections["Variables"].Keys["MenuNormalAlpha"].Value = prefs.MenuNormalAlpha;
            preferencesIni.Sections["Variables"].Keys["MenuHoverAlpha"].Value = prefs.MenuHoverAlpha;
            preferencesIni.Sections["Variables"].Keys["BarColor"].Value = prefs.BarColor;
            preferencesIni.Sections["Variables"].Keys["ColorText"].Value = prefs.ColorText;
            preferencesIni.Sections["Variables"].Keys["NetMaxDown"].Value = prefs.NetMaxDown;
            preferencesIni.Sections["Variables"].Keys["NetMaxUp"].Value = prefs.NetMaxUp;
            preferencesIni.Sections["Variables"].Keys["ColorGreen"].Value = prefs.ColorGreen;
            preferencesIni.Sections["Variables"].Keys["ColorRed"].Value = prefs.ColorRed;
            preferencesIni.Sections["Variables"].Keys["ColorYellow"].Value = prefs.ColorYellow;
            preferencesIni.Sections["Variables"].Keys["MyOWMApiKey"].Value = prefs.MyOWMApiKey;
            preferencesIni.Save(configFile);
            log.Debug("preferences saved");
        }
        #endregion Save Preferences.inc file

        #region Window events
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            log.Info("{0} is shutting down.", AppInfo.AppName);

            // Shut down NLog
            LogManager.Shutdown();

            // save the property settings
            UserSettings.Setting.WindowLeft = Left;
            UserSettings.Setting.WindowTop = Top;
            UserSettings.SaveSettings();
        }
        #endregion Window events

        #region Button events
        private void Font_Click(object sender, RoutedEventArgs e)
        {
            FontSelector fs = new FontSelector(prefs.Font)
            {
                Owner = this
            };
            _ = fs.ShowDialog();
            prefs.Font = string.IsNullOrWhiteSpace(fs.FontName) ? "Segoe UI" : fs.FontName;
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            SaveIni();

            try
            {
                using (Process ps = new Process())
                {
                    string pf = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                    ps.StartInfo.FileName = Path.Combine(pf, @"Rainmeter\Rainmeter.exe");
                    ps.StartInfo.Arguments = "!RefreshApp";
                    _ = ps.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to launch Rainmeter",
                    "ERROR",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                log.Error(ex, "Error while trying to refresh Rainmeter");
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (Process ps = new Process())
                {
                    ps.StartInfo.FileName = configFile;
                    _ = ps.Start();
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Failed to open\n{configFile} ",
                    "ERROR",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                log.Error(ex, "Error while trying to open Preferences.inc");
            }
        }

        private void BtnEditHW_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (Process ps = new Process())
                {
                    ps.StartInfo.FileName = hwinfoFile;
                    _ = ps.Start();
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Failed to open\n{hwinfoFile} ",
                    "ERROR",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                log.Error(ex, "Error while trying to open HWiNFO.inc");
            }
        }
        #endregion Button events

        #region Mouse events
        private void Image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            About a = new About
            {
                Owner = this
            };
            a.ShowDialog();
        }
        #endregion Mouse events

        #region Hyper-link click
        private void Hyp_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            log.Debug($"Opening {e.Uri.AbsoluteUri}");
            if (!string.IsNullOrWhiteSpace(e.Uri.AbsoluteUri))
            {
                _ = Process.Start(e.Uri.AbsoluteUri);
                e.Handled = true;
            }
        }
        #endregion Hyper-link click

        #region Combo boxes

        private void CbxToolTipType_Loaded(object sender, RoutedEventArgs e)
        {
            cbxToolTipType.ItemsSource = new List<string>
            {
                "Normal",
                "Balloon"
            };
            cbxToolTipType.SelectedIndex = int.Parse(prefs.TTType);
        }

        private void CbxToolTipType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            string value = combo.SelectedItem as string;
            if (value == "Balloon")
            {
                prefs.TTType = "1";
            }
            else
            {
                prefs.TTType = "0";
            }
        }

        private void CbxPosh_Loaded(object sender, RoutedEventArgs e)
        {
            cbxPosh.ItemsSource = new List<string>
            {
                "pwsh.exe",
                "powershell.exe"
            };
            cbxPosh.SelectedItem = prefs.PowerShell;
        }

        private void CbxPosh_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            prefs.PowerShell = combo.SelectedItem as string;
        }

        private void CbxHWver_Loaded(object sender, RoutedEventArgs e)
        {
            cbxHWver.ItemsSource = new List<string>
            {
                "HWiNFO64",
                "HWiNFO32"
            };
            cbxHWver.SelectedItem = prefs.HWVersion;
        }

        private void CbxHWver_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            prefs.HWVersion = combo.SelectedItem as string;
        }

        private void CbxTextColor_Loaded(object sender, RoutedEventArgs e)
        {
            SetBoxColor(cbxTextColor, prefs.ColorText);
        }

        private void CbxTextColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                Color selectedColor = (Color)(cbxTextColor.SelectedItem as PropertyInfo)?.GetValue(null, null);
                prefs.ColorText = RGBValue(selectedColor);
            }
        }

        private void CbxBGColor_Loaded(object sender, RoutedEventArgs e)
        {
            SetBoxColor(cbxBGColor, prefs.BarColor);
        }

        private void CbxBGColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                Color selectedColor = (Color)(cbxBGColor.SelectedItem as PropertyInfo)?.GetValue(null, null);
                prefs.BarColor = RGBValue(selectedColor);
            }
        }

        private void CbxColorGreen_Loaded(object sender, RoutedEventArgs e)
        {
            SetBoxColor(cbxColorGreen, prefs.ColorGreen);
        }

        private void CbxColorGreen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                Color selectedColor = (Color)(cbxColorGreen.SelectedItem as PropertyInfo)?.GetValue(null, null);
                prefs.ColorGreen = RGBValue(selectedColor);
            }
        }

        private void CbxColorRed_Loaded(object sender, RoutedEventArgs e)
        {
            SetBoxColor(cbxColorRed, prefs.ColorRed);
        }

        private void CbxColorRed_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                Color selectedColor = (Color)(cbxColorRed.SelectedItem as PropertyInfo)?.GetValue(null, null);
                prefs.ColorRed = RGBValue(selectedColor);
            }
        }

        private void CbxColorYellow_Loaded(object sender, RoutedEventArgs e)
        {
            SetBoxColor(cbxColorYellow, prefs.ColorYellow);
        }

        private void CbxColorYellow_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                Color selectedColor = (Color)(cbxColorYellow.SelectedItem as PropertyInfo)?.GetValue(null, null);
                prefs.ColorYellow = RGBValue(selectedColor);
            }
        }
        #endregion Combo boxes

        #region Numeric up/down events
        private void NumFontSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            prefs.FontSize = e.NewValue.ToString();
        }

        private void NumnumBarHeight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            prefs.BarHeight = e.NewValue.ToString();
        }

        private void NumBarTransparency_ValueChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            prefs.BarTransparency = e.NewValue.ToString();
        }

        private void NumMenuNormalAlpha_ValueChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            prefs.MenuNormalAlpha = e.NewValue.ToString();
        }

        private void NumMenuHoverAlpha_ValueChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            prefs.MenuHoverAlpha = e.NewValue.ToString();
        }

        private void NumNetMaxDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            prefs.NetMaxDown = e.NewValue.ToString();
        }

        private void NumNetMaxUp_ValueChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            prefs.NetMaxUp = e.NewValue.ToString();
        }
        #endregion Numeric up/down events

        #region Color helper methods
        private string RGBValue(Color selectedColor)
        {
            return string.Format("{0},{1},{2}",
                            selectedColor.R,
                            selectedColor.G,
                            selectedColor.B);
        }

        private void SetBoxColor(ComboBox box, string prefColor)
        {
            string[] rgb = prefColor.Split(',');
            byte r = byte.Parse(rgb[0]);
            byte g = byte.Parse(rgb[1]);
            byte b = byte.Parse(rgb[2]);

            PropertyInfo[] colors = typeof(Colors).GetProperties();
            box.ItemsSource = colors;

            for (int i = 0; i < colors.Length; i++)
            {
                Color cc = (Color)colors[i].GetValue(null, null);
                // Check Alpha = 0 to exclude "Transparent"
                if ((cc.R == r) && (cc.G == g) && (cc.B == b) && (cc.A != 0))
                {
                    box.SelectedIndex = i;
                    break;
                }
            }
        }
        #endregion Color helper methods
    }
}
