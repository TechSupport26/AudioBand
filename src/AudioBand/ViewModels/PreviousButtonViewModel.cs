﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AudioBand.AudioSource;
using AudioBand.Commands;
using AudioBand.Models;
using AudioBand.Settings;

namespace AudioBand.ViewModels
{
    /// <summary>
    /// View model for the previous button.
    /// </summary>
    public class PreviousButtonViewModel : ButtonViewModelBase<PreviousButton>
    {
        private readonly IAppSettings _appSettings;
        private readonly IAudioSession _audioSession;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreviousButtonViewModel"/> class.
        /// </summary>
        /// <param name="appSettings">The app settings.</param>
        /// <param name="dialogService">The dialog service.</param>
        /// <param name="audioSession">The audio session.</param>
        public PreviousButtonViewModel(IAppSettings appSettings, IDialogService dialogService, IAudioSession audioSession)
            : base(appSettings.PreviousButton, dialogService)
        {
            _appSettings = appSettings;
            _audioSession = audioSession;
            _appSettings.ProfileChanged += AppsSettingsOnProfileChanged;
            PreviousTrackCommand = new AsyncRelayCommand<object>(PreviousTrackCommandOnExecute);
            Content = new ButtonContentViewModel(Model.Content, new PreviousButton().Content, dialogService);
            TrackContentViewModel(Content);
        }

        /// <summary>
        /// Gets the button content.
        /// </summary>
        public ButtonContentViewModel Content { get; }

        /// <summary>
        /// Gets the previous track command.
        /// </summary>
        public IAsyncCommand PreviousTrackCommand { get; }

        private async Task PreviousTrackCommandOnExecute(object arg)
        {
            if (_audioSession.CurrentAudioSource == null)
            {
                return;
            }

            await _audioSession.CurrentAudioSource.PreviousTrackAsync();
        }

        private void AppsSettingsOnProfileChanged(object sender, EventArgs e)
        {
            Debug.Assert(IsEditing == false, "Should not be editing");
            ReplaceModel(_appSettings.PreviousButton);
        }
    }
}