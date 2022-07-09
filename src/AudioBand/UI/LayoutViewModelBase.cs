﻿using AudioBand.Messages;
using AudioBand.Models;
using System.Collections.Generic;

namespace AudioBand.UI
{
    /// <summary>
    /// Base view model for <see cref="LayoutModelBase"/> with an instance of the model.
    /// </summary>
    /// <typeparam name="TModel">The model type.</typeparam>
    public class LayoutViewModelBase<TModel> : ViewModelBase
        where TModel : LayoutModelBase, new()
    {
        private readonly List<ButtonContentViewModel> _contentViewModels = new List<ButtonContentViewModel>();

        private readonly TModel _backup = new TModel();
        private LikeDislikeButton likeDislikeButton;
        private IDialogService dialogService;
        private IMessageBus messageBus;
        private AlbumArt albumArt;
        private ProgressBar progressBar;
        private object source;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutViewModelBase{TModel}"/> class
        /// and maps the data from <paramref name="source"/> to the <see cref="Model"/> and registers <see cref="Model"/> for change tracking.
        /// </summary>
        /// <param name="source">The initial model data.</param>
        /// <param name="messageBus">The message bus.</param>
        public LayoutViewModelBase(LikeDislikeButton likeDislikeButton, IMessageBus messageBus, TModel source)
        {
            MapSelf(source, Model);
            UseMessageBus(messageBus);
        }

        public LayoutViewModelBase(LikeDislikeButton likeDislikeButton, IDialogService dialogService, IMessageBus messageBus)
        {
            this.likeDislikeButton = likeDislikeButton;
            this.dialogService = dialogService;
            this.messageBus = messageBus;
        }

        public LayoutViewModelBase(IMessageBus messageBus, AlbumArt albumArt)
        {
            this.messageBus = messageBus;
            this.albumArt = albumArt;
        }

        public LayoutViewModelBase(IMessageBus messageBus, CustomLabel source)
        {
            this.messageBus = messageBus;
        }

        public LayoutViewModelBase(IMessageBus messageBus, ProgressBar progressBar)
        {
            this.messageBus = messageBus;
            this.progressBar = progressBar;
        }

        public LayoutViewModelBase(IMessageBus messageBus, object source)
        {
            this.messageBus = messageBus;
            this.source = source;
        }

        /// <summary>
        /// Gets or sets a value indicating whether it is visible.
        /// </summary>
        [TrackState]
        public bool IsVisible
        {
            get => Model.IsVisible;
            set => SetProperty(Model, nameof(Model.IsVisible), value);
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        [TrackState]
        public double Width
        {
            get => Model.Width;
            set => SetProperty(Model, nameof(Model.Width), value);
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        [TrackState]
        public double Height
        {
            get => Model.Height;
            set => SetProperty(Model, nameof(Model.Height), value);
        }

        /// <summary>
        /// Gets or sets the x position.
        /// </summary>
        [TrackState]
        public double XPosition
        {
            get => Model.XPosition;
            set => SetProperty(Model, nameof(Model.XPosition), value);
        }

        /// <summary>
        /// Gets or sets the y position.
        /// </summary>
        [TrackState]
        public double YPosition
        {
            get => Model.YPosition;
            set => SetProperty(Model, nameof(Model.YPosition), value);
        }

        /// <summary>
        /// Gets or sets the relative positioning.
        /// </summary>
        [TrackState]
        public PositionAnchor Anchor
        {
            get => Model.Anchor;
            set => SetProperty(Model, nameof(Model.Anchor), value);
        }

        /// <summary>
        /// Gets the instance of the model for this view model.
        /// </summary>
        protected TModel Model { get; } = new TModel();

        /// <inheritdoc />
        protected override void OnReset()
        {
            base.OnReset();
            ResetObject(Model);
        }

        /// <inheritdoc />
        protected override void OnBeginEdit()
        {
            base.OnBeginEdit();
            MapSelf(Model, _backup);
        }

        /// <inheritdoc />
        protected override void OnCancelEdit()
        {
            base.OnCancelEdit();
            MapSelf(_backup, Model);
        }

        /// <inheritdoc />
        protected override void OnEndEdit()
        {
            base.OnEndEdit();
            foreach (var buttonContentViewModel in _contentViewModels)
            {
                buttonContentViewModel.EndEdit();
            }
        }
    }
}
