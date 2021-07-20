namespace AudioBand.Models
{
    /// <summary>
    /// Model for the volume button.
    /// </summary>
    public class VolumeButton : ButtonModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VolumeButton"/> class.
        /// </summary>
        public VolumeButton()
        {
            XPosition = 251;
            YPosition = 6;
            Width = 18;
            Height = 18;
        }

        /// <summary>
        /// Gets or sets the button with no volume.
        /// </summary>
        public ButtonContent NoVolumeContent { get; set; } = new ButtonContent
        {
            Text = "",
        };

        /// <summary>
        /// Gets or sets the button with low volume.
        /// </summary>
        public ButtonContent LowVolumeContent { get; set; } = new ButtonContent
        {
            Text = "",
        };

        /// <summary>
        /// Gets or sets the button with high volume.
        /// </summary>
        public ButtonContent HighVolumeContent { get; set; } = new ButtonContent
        {
            Text = "",
        };
    }
}