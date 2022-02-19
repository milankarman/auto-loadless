﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace unload
{
    public partial class LoadCheckWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly Project project;

        private int loadIndex;
        private int startFrame;
        private int endFrame;

        public int LoadNumber
        {
            get => loadIndex + 1;
            set
            {
                loadIndex = Math.Clamp(value, 1, project.DetectedLoads.Count) - 1;
                ShowLoadScreen(loadIndex);
                ShowLoadInfo(loadIndex);
                OnPropertyChanged();
            }
        }

        public int StartFrame
        {
            get => startFrame;
            set
            {
                startFrame = Math.Clamp(value, 1, project.totalFrames);
                ShowStartFrame(startFrame);

                DetectedLoad load = project.DetectedLoads[loadIndex];
                load.StartFrame = startFrame;
                UpdateDetectedLoads();

                if (LoadNumber != load.Number) LoadNumber = load.Number;

                ShowLoadInfo(loadIndex);
                OnPropertyChanged();
            }
        }

        public int EndFrame
        {
            get => endFrame;
            set
            {
                endFrame = Math.Clamp(value, 1, project.totalFrames);
                ShowEndFrame(endFrame);

                DetectedLoad load = project.DetectedLoads[loadIndex];
                load.EndFrame = endFrame;
                UpdateDetectedLoads();

                if (LoadNumber != load.Number) LoadNumber = load.Number;

                ShowLoadInfo(loadIndex);
                OnPropertyChanged();
            }
        }

        public LoadCheckWindow()
        {
            string videoPath = "C:\\Users\\Milan\\Downloads\\[WR] The Hobbit(PC) World Record in 11_48 _ Any% NMG[ikrXK1HM4u0].webm";
            string framesDirectory = "C:\\Users\\Milan\\Downloads\\WRTheHobbitPCWorldRecordin1148AnyNMGikrXK1HM4u0webm_frames";

            project = new Project(videoPath, framesDirectory, 44505, 60);

            List<DetectedLoad> loads = new()
            {
                new DetectedLoad(1, 176, 191),
                new DetectedLoad(2, 1289, 1310),
                new DetectedLoad(3, 4005, 4024),
                new DetectedLoad(4, 5059, 5081),
                new DetectedLoad(5, 5848, 5876),
                new DetectedLoad(6, 15093, 15116),
                new DetectedLoad(7, 20918, 20937),
                new DetectedLoad(8, 25887, 25921),
                new DetectedLoad(9, 26696, 26725),
                new DetectedLoad(10, 31314, 31331),
                new DetectedLoad(11, 33856, 33882),
                new DetectedLoad(12, 35743, 35775),
            };

            project.TEST_SetDetectedLoads(loads);

            InitializeComponent();
            DataContext = this;

            txtLoadNumber.PreviewTextInput += TextBoxValidator.ForceInteger;
            txtStartFrame.PreviewTextInput += TextBoxValidator.ForceInteger;
            txtEndFrame.PreviewTextInput += TextBoxValidator.ForceInteger;
            txtStepSize.PreviewTextInput += TextBoxValidator.ForceInteger;

            UpdateDetectedLoads();

            DetectedLoad firstLoad = project.DetectedLoads[0];
            LoadNumber = 1;
            StartFrame = firstLoad.StartFrame;
            EndFrame = firstLoad.EndFrame;
        }

        private void ShowLoadScreen(int loadIndex)
        {
            DetectedLoad load = project.DetectedLoads[loadIndex];
            StartFrame = load.StartFrame;
            EndFrame = load.EndFrame;
        }


        private void ShowStartFrame(int frameIndex)
        {
            Uri startFrame = new(Path.Join(project.framesDirectory, $"{frameIndex}.jpg"));
            imgStartFrame.Source = new BitmapImage(startFrame);

            Uri startFrameBefore = new(Path.Join(project.framesDirectory, $"{frameIndex - 1}.jpg"));

            if (frameIndex > 1)
            {
                imgStartFrameBefore.Source = new BitmapImage(startFrameBefore);
            }
            else
            {
                imgStartFrameBefore.Source = null;
            }
        }

        private void ShowEndFrame(int frameIndex)
        {
            Uri endFrame = new(Path.Join(project.framesDirectory, $"{frameIndex}.jpg"));
            imgEndFrame.Source = new BitmapImage(endFrame);

            Uri endFrameAfter = new(Path.Join(project.framesDirectory, $"{frameIndex + 1}.jpg"));

            if (frameIndex < project.totalFrames)
            {
                imgEndFrameAfter.Source = new BitmapImage(endFrameAfter);
            }
            else
            {
                imgEndFrameAfter.Source = null;
            }
        }

        private void ShowLoadInfo(int frameIndex)
        {
            DetectedLoad load = project.DetectedLoads[frameIndex];
            int frames = load.EndFrame - load.StartFrame + 1;
            TimeSpan loadTime = TimeSpan.FromSeconds(Math.Round(frames / project.fps, 3));
            txtLoadInfo.Text = $"Frames: {frames}, Duration: {loadTime:mm\\:ss\\.fff}";
        }

        private void UpdateDetectedLoads()
        {
            project.OrderLoads();
            lbxLoads.ItemsSource = new ObservableCollection<DetectedLoad>(project.DetectedLoads);
            sliderTimeline.Maximum = project.DetectedLoads.Count;
        }

        private int GetStepSizeFrames() => (int)(project.fps / (1000 / int.Parse(txtStepSize.Text)));

        private void btnStartFrameBackFar_Click(object sender, RoutedEventArgs e) => StartFrame -= GetStepSizeFrames();

        private void btnStartFrameBack_Click(object sender, RoutedEventArgs e) => StartFrame--;

        private void btnStartFrameForward_Click(object sender, RoutedEventArgs e) => StartFrame++;

        private void btnStartFrameForwardFar_Click(object sender, RoutedEventArgs e) => StartFrame += GetStepSizeFrames();

        private void btnEndFrameBackFar_Click(object sender, RoutedEventArgs e) => EndFrame -= GetStepSizeFrames();

        private void btnEndFrameBack_Click(object sender, RoutedEventArgs e) => EndFrame--;

        private void btnEndFrameForward_Click(object sender, RoutedEventArgs e) => EndFrame++;

        private void btnEndFrameForwardFar_Click(object sender, RoutedEventArgs e) => EndFrame += GetStepSizeFrames();

        private void btnBack_Click(object sender, RoutedEventArgs e) => LoadNumber--;

        private void btnForward_Click(object sender, RoutedEventArgs e) => LoadNumber++;

        private void btnDLoadAdd_Click(object sender, RoutedEventArgs e)
        {
            project.DetectedLoads.Add(new DetectedLoad(0, 1, 1));
            UpdateDetectedLoads();
        }

        private void btnDLoadNumber_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).DataContext is DetectedLoad load) LoadNumber = load.Number;
        }

        private void btnDLoadDelete_Click(object sender, RoutedEventArgs e)
        {
            Button cmd = (Button)sender;
            if (cmd.DataContext is DetectedLoad load)
            {
                project.DetectedLoads.Remove(load);

                if (project.DetectedLoads.Count == 0) project.DetectedLoads.Add(new DetectedLoad(0, 1, 1));
                if (load.Number == LoadNumber) LoadNumber--;

                UpdateDetectedLoads();
            }
        }

        private void btnRemoveLoad_Click(object sender, RoutedEventArgs e)
        {
            project.DetectedLoads.RemoveAt(loadIndex);

            if (project.DetectedLoads.Count == 0) project.DetectedLoads.Add(new DetectedLoad(0, 1, 1));
            LoadNumber--;

            UpdateDetectedLoads();
        }

        protected void OnPropertyChanged(string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
