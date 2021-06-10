// Copyright(c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;

namespace TimnigmaSettings
{
    public partial class FontSelector : Window
    {
        public string FontName { get; set; }
        public FontSelector(string ff)
        {
            InitializeComponent();
            LoadListbox(ff);
        }

        private void LoadListbox(string ff)
        {
            System.Collections.Generic.ICollection<FontFamily> fontlist = Fonts.SystemFontFamilies;
            lb1.ItemsSource = fontlist.OrderBy(x => x.Source);
            lb1.SelectedValuePath = "Source";
            lb1.SelectedValue = ff;
            lb1.ScrollIntoView(lb1.SelectedItem);
            _ = lb1.Focus();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                FontName = lb1.SelectedValue.ToString();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
