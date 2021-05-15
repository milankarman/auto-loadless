﻿using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace unload
{
    public partial class ProgressWindow : Window
    {
        public int currentTask = 0;
        public bool finished = false;
        public CancellationTokenSource cts = new CancellationTokenSource();

        private string text = "";
        public int totalTasks = 0;
        private Action onFinishedAction = null;

        public ProgressWindow(string _text, int _totalTask, Action _onFinishedAction = null)
        {
            InitializeComponent();

            text = _text;
            totalTasks = _totalTask;
            onFinishedAction = _onFinishedAction;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.25);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        public void timer_Tick(object sender, EventArgs e)
        {
            if (finished)
            {
                onFinishedAction();
                Close();
            }

            label.Content = $"{text}: {currentTask} / {totalTasks}";

            double percentage = (double)currentTask / (double)totalTasks * 100d;
            progressBar.Value = percentage;
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            cts.Cancel();
            Close();
        }
    }
}
