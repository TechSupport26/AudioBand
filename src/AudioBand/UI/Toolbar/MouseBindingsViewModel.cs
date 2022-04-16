﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using AudioBand.AudioSource;
using AudioBand.Commands;
using AudioBand.Extensions;
using AudioBand.Messages;
using AudioBand.Models;
using AudioBand.Settings;
using MouseBinding = AudioBand.Models.MouseBinding;

namespace AudioBand.UI
{
    /// <summary>
    /// The Mouse Binding ViewModel.
    /// </summary>
    public class MouseBindingsViewModel : ViewModelBase
    {
        private IAppSettings _appSettings;
        private IAudioSession _audioSession;
        private IMessageBus _messageBus;

        private ObservableCollection<MouseBinding> _mouseBindings;

        private List<MouseBinding> _model = new List<MouseBinding>();
        private List<MouseBinding> _backup = new List<MouseBinding>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseBindingsViewModel"/> class.
        /// </summary>
        /// <param name="appSettings">The app settings.</param>
        /// <param name="audioSession">The audio session.</param>
        /// <param name="messageBus">The message bus.</param>
        public MouseBindingsViewModel(IAppSettings appSettings, IAudioSession audioSession, IMessageBus messageBus)
        {
            MapSelf(appSettings.AudioBandSettings.MouseBindings, _model);

            _appSettings = appSettings;
            _audioSession = audioSession;
            _messageBus = messageBus;

            MouseBindings = new ObservableCollection<MouseBinding>(appSettings.AudioBandSettings.MouseBindings);
            StartEditCommand = new RelayCommand(StartEditCommandOnExecute);

            LeftClickCommand = new RelayCommand(LeftClickCommandOnExecute);
            DoubleLeftClickCommand = new RelayCommand(DoubleLeftClickCommandOnExecute);
            MiddleClickCommand = new RelayCommand(MiddleClickCommandOnExecute);
            DoubleMiddleClickCommand = new RelayCommand(DoubleMiddleClickCommandOnExecute);
            MouseWheelCommand = new RelayCommand<MouseWheelEventArgs>(MouseWheelCommandOnExecute);

            UseMessageBus(messageBus);
        }

        /// <summary>
        /// Gets the Mouse Input Types.
        /// </summary>
        public IEnumerable<EnumDescriptor<MouseInputType>> MouseInputTypes { get; } = typeof(MouseInputType).GetEnumDescriptors<MouseInputType>();

        /// <summary>
        /// Gets the Mouse Binding Command types.
        /// </summary>
        public IEnumerable<EnumDescriptor<MouseBindingCommandType>> CommandTypes { get; } = typeof(MouseBindingCommandType).GetEnumDescriptors<MouseBindingCommandType>();

        /// <summary>
        /// Gets or sets the MouseBindings collection.
        /// </summary>
        [TrackState]
        public ObservableCollection<MouseBinding> MouseBindings
        {
            get => _mouseBindings;
            set => SetProperty(ref _mouseBindings, value);
        }

        /// <summary>
        /// Gets the command to start editing.
        /// </summary>
        public ICommand StartEditCommand { get; }

        /// <summary>
        /// Gets the command to handle left clicks.
        /// </summary>
        public ICommand LeftClickCommand { get; }

        /// <summary>
        /// Gets the command to handle double left clicks.
        /// </summary>
        public ICommand DoubleLeftClickCommand { get; }

        /// <summary>
        /// Gets the command to handle middle clicks.
        /// </summary>
        public ICommand MiddleClickCommand { get; }

        /// <summary>
        /// Gets the command to handle double middle clicks.
        /// </summary>
        public ICommand DoubleMiddleClickCommand { get; }

        /// <summary>
        /// Gets the command to handle mouse scrolling.
        /// </summary>
        public ICommand MouseWheelCommand { get; }

        /// <inheritdoc />
        protected override void OnBeginEdit()
        {
            base.OnBeginEdit();
            MapSelf(_model, _backup);
        }

        /// <inheritdoc />
        protected override void OnCancelEdit()
        {
            base.OnCancelEdit();
            MapSelf(_backup, _model);
        }

        /// <inheritdoc />
        protected override void OnEndEdit()
        {
            base.OnEndEdit();
            MapSelf(_model, _appSettings.AudioBandSettings.MouseBindings);
        }

        private void StartEditCommandOnExecute()
        {
            BeginEdit();
        }

        private void LeftClickCommandOnExecute()
        {
            CheckBindingsPerInputType(MouseInputType.LeftClick);
        }

        private void DoubleLeftClickCommandOnExecute()
        {
            CheckBindingsPerInputType(MouseInputType.LeftDoubleClick);
        }

        private void MiddleClickCommandOnExecute()
        {
            CheckBindingsPerInputType(MouseInputType.MiddleClick);
        }

        private void DoubleMiddleClickCommandOnExecute()
        {
            CheckBindingsPerInputType(MouseInputType.MiddleDoubleClick);
        }

        private void MouseWheelCommandOnExecute(MouseWheelEventArgs args)
        {
            if (args.Delta > 0)
            {
                CheckBindingsPerInputType(MouseInputType.ScrollUp);
                return;
            }

            CheckBindingsPerInputType(MouseInputType.ScrollDown);
        }

        private void CheckBindingsPerInputType(MouseInputType inputType)
        {
            var altDown = Keyboard.GetKeyStates(Key.LeftAlt) == KeyStates.Down || Keyboard.GetKeyStates(Key.RightAlt) == KeyStates.Down;
            var ctrlDown = Keyboard.GetKeyStates(Key.LeftCtrl) == KeyStates.Down || Keyboard.GetKeyStates(Key.RightCtrl) == KeyStates.Down;
            var shiftDown = Keyboard.GetKeyStates(Key.LeftShift) == KeyStates.Down || Keyboard.GetKeyStates(Key.RightShift) == KeyStates.Down || Keyboard.GetKeyStates(Key.CapsLock) == KeyStates.Toggled;

            for (int i = 0; i < _appSettings.AudioBandSettings.MouseBindings.Count; i++)
            {
                var mouseBinding = _appSettings.AudioBandSettings.MouseBindings[i];

                // If keys arent down correctly, continue
                if ((mouseBinding.WithAlt && !altDown) || (mouseBinding.WithCtrl && !ctrlDown) || (mouseBinding.WithShift && !shiftDown))
                {
                    continue;
                }

                if (mouseBinding.MouseInputType == inputType)
                {
                    ExecuteCommandBasedOnInputType(mouseBinding.CommandType);
                }
            }
        }

        private void ExecuteCommandBasedOnInputType(MouseBindingCommandType inputType)
        {
            switch (inputType)
            {
                case MouseBindingCommandType.NextProfile:
                    SwitchProfile();
                    break;
                case MouseBindingCommandType.PreviousProfile:
                    SwitchProfile(true);
                    break;
                case MouseBindingCommandType.Play:
                    _audioSession.CurrentAudioSource?.PlayTrackAsync();
                    break;
                case MouseBindingCommandType.Pause:
                    _audioSession.CurrentAudioSource?.PauseTrackAsync();
                    break;
                case MouseBindingCommandType.NextSong:
                    _audioSession.CurrentAudioSource?.NextTrackAsync();
                    break;
                case MouseBindingCommandType.PreviousSong:
                    _audioSession.CurrentAudioSource?.PreviousTrackAsync();
                    break;
                case MouseBindingCommandType.NextRepeatMode:
                    break;
                case MouseBindingCommandType.PreviousRepeatMode:
                    break;
                case MouseBindingCommandType.ToggleShuffleMode:
                    break;
                case MouseBindingCommandType.VolumeHigher:
                    break;
                case MouseBindingCommandType.VolumeLower:
                    break;
                case MouseBindingCommandType.OpenAssociatedApp:
                    OpenAssociatedApp();
                    break;
                default:
                    break;
            }
        }

        private void OpenAssociatedApp()
        {
            if (_audioSession.CurrentAudioSource == null)
            {
                return;
            }

            var windowPtr = NativeMethods.FindWindow(_audioSession.CurrentAudioSource.WindowClassName, null);

            // Spotify has some weird shenanigans with their windows, doing it like normal
            // results in the wrong window handle being returned.
            if (_audioSession.CurrentAudioSource.Name == "Spotify")
            {
                var spotifyProcesses = Process.GetProcessesByName("spotify");
                var title = spotifyProcesses.FirstOrDefault(x => !string.IsNullOrEmpty(x.MainWindowTitle))?.MainWindowTitle;

                if (string.IsNullOrEmpty(title))
                {
                    try
                    {
                        var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                        Process.Start($"{path}/Spotify/Spotify.exe");
                    }
                    catch (Exception)
                    {
                        Logger.Debug("Failed to open Spotify through path.");
                    }
                }

                windowPtr = NativeMethods.FindWindow(null, title);
            }

            if (windowPtr == IntPtr.Zero)
            {
                Logger.Warn("Could not find the associated window to open with double click.");
                return;
            }

            if (NativeMethods.IsIconic(windowPtr))
            {
                NativeMethods.ShowWindow(windowPtr, 9);
            }

            NativeMethods.SetForegroundWindow(windowPtr);
        }

        private void SwitchProfile(bool previous = false)
        {
            var index = Array.FindIndex(_appSettings.Profiles.ToArray(), x => x.Name == _appSettings.CurrentProfile.Name);
            index = previous ? index - 1 : index + 1;

            _appSettings.SelectProfile(_appSettings.Profiles.ElementAt(index).Name);
        }
    }
}
