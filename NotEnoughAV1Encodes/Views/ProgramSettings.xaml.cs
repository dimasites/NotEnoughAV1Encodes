﻿using ControlzEx.Theming;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Navigation;
using WPFLocalizeExtension.Engine;

namespace NotEnoughAV1Encodes.Views
{
    public partial class ProgramSettings : MetroWindow
    {
        public SettingsDB settingsDBTemp = new();
        
        public ProgramSettings(SettingsDB settingsDB)
        {
            InitializeComponent();
            ToggleSwitchDeleteTempFiles.IsOn = settingsDB.DeleteTempFiles;
            ToggleSwitchShutdown.IsOn = settingsDB.ShutdownAfterEncode;
            ToggleSwitchOverrideWorkerCount.IsOn = settingsDB.OverrideWorkerCount;
            ToggleSwitchLogging.IsOn = settingsDB.Logging;
            ComboBoxAccentTheme.SelectedIndex = settingsDB.AccentTheme;
            ComboBoxBaseTheme.SelectedIndex = settingsDB.BaseTheme;
            TextBoxTempPath.Text = settingsDB.TempPath;
            settingsDBTemp.BGImage = settingsDB.BGImage;
            ComboBoxProcessPriority.SelectedIndex = settingsDB.PriorityNormal ? 0 : 1;
            
            try
            {
                ThemeManager.Current.ChangeTheme(this, settingsDB.Theme);
            }
            catch { }


            ComboBoxLanguage.SelectedIndex = settingsDB.CultureInfo.Name switch
            {
                "en" => 0,
                "de" => 1,
                "zh-CN" => 2,
                "ru-RU" => 3,
                _ => 0,
            };
        }

        private void ButtonSelectBGImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                settingsDBTemp.BGImage = openFileDialog.FileName;
            }
        }

        private void ButtonSelectTempPath_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.SelectedPath = TextBoxTempPath.Text;
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                TextBoxTempPath.Text = dialog.SelectedPath + "\\";
            }
        }

        private void ButtonSelectTempPathReset_Click(object sender, RoutedEventArgs e)
        {
            TextBoxTempPath.Text = Path.GetTempPath();
        }

        private void ButtonUpdater_Click(object sender, RoutedEventArgs e)
        {
            Updater updater = new(ComboBoxBaseTheme.Text, ComboBoxAccentTheme.Text);
            updater.ShowDialog();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            settingsDBTemp.DeleteTempFiles = ToggleSwitchDeleteTempFiles.IsOn;
            settingsDBTemp.ShutdownAfterEncode = ToggleSwitchShutdown.IsOn;
            settingsDBTemp.OverrideWorkerCount = ToggleSwitchOverrideWorkerCount.IsOn;
            settingsDBTemp.Logging = ToggleSwitchLogging.IsOn;
            settingsDBTemp.BaseTheme = ComboBoxBaseTheme.SelectedIndex;
            settingsDBTemp.AccentTheme = ComboBoxAccentTheme.SelectedIndex;
            settingsDBTemp.Theme = ComboBoxBaseTheme.Text + "." + ComboBoxAccentTheme.Text;
            settingsDBTemp.TempPath = TextBoxTempPath.Text;
            settingsDBTemp.PriorityNormal = ComboBoxProcessPriority.SelectedIndex == 0;
        }

        private void ButtonResetBGImage_Click(object sender, RoutedEventArgs e)
        {
            settingsDBTemp.BGImage = null;
        }

        private void ComboBoxLanguage_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ComboBoxLanguage == null) return;
            settingsDBTemp.CultureInfo = ComboBoxLanguage.SelectedIndex switch
            {
                0 => new CultureInfo("en"),
                1 => new CultureInfo("de"),
                2 => new CultureInfo("zh-CN"),
                3 => new CultureInfo("ru-RU"),
                _ => new CultureInfo("en"),
            };
            LocalizeDictionary.Instance.Culture = settingsDBTemp.CultureInfo;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            ProcessStartInfo psi = new()
            {
                FileName = "cmd",
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = $"/c start {e.Uri}"
            };
            Process.Start(psi);
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}