﻿using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using unload.Properties;

namespace unload
{
    public partial class StartSettingsWindow : Window
    {
        private readonly StartWindow startWindow;

        private string? workingDirectory = null;

        private const string NO_WORKING_DIRECTORY = "Same as video";

        public StartSettingsWindow(StartWindow _startWindow, string? _workingDirectory)
        {
            Owner = _startWindow;
            startWindow = _startWindow;
            workingDirectory = _workingDirectory;

            InitializeComponent();

            txtWorkingDirectory.Text = string.IsNullOrEmpty(workingDirectory) ? NO_WORKING_DIRECTORY : workingDirectory;

            if (_workingDirectory != null) btnClear.IsEnabled = true;
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new()
            {
                Title = "Select working directory for creating frames",
                IsFolderPicker = true,
                EnsureFileExists = true,
                EnsurePathExists = true,
                EnsureValidNames = true,
                Multiselect = false,
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                workingDirectory = dialog.FileName;
                txtWorkingDirectory.Text = workingDirectory;
                btnClear.IsEnabled = true;
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtWorkingDirectory.Text = NO_WORKING_DIRECTORY;
            workingDirectory = null;
            btnClear.IsEnabled = false;
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            startWindow.workingDirectory = workingDirectory;
            Settings.Default.WorkingDirectory = workingDirectory;
            Settings.Default.Save();

            startWindow.IsEnabled = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            startWindow.IsEnabled = true;
            Close();
        }
    }
}
