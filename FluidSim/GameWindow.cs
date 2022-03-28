using System;
using System.ComponentModel;
using System.Drawing;
using SFML.Graphics;
using SFML.Window;

namespace FluidSim
{
    public class GameWindow : RenderWindow
    {
        #region Private Variables

        private string _title;

        #endregion

        #region Protected Constructors

        public GameWindow(string title, uint width, uint height)
            : base(new VideoMode(800, 600), title, Styles.Close)
        {
            _title = title;
        }

        #endregion

        #region Public Properties

        [DefaultValue(false)] public bool AllowUserResizing { get; set; }

        [DefaultValue(typeof(Rectangle), "800, 600")]
        public Rectangle ClientBounds { get; }

        [DefaultValue("Emix Game")] public string ScreenDeviceName { get; }

        [DefaultValue("Emix Game")]
        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    SetTitle(value);
                    _title = value;
                }
            }
        }

        /*public Color ClearColor
        {
            get
            {
                return 
            }
        }
        */

        #endregion

        #region Events

        public event EventHandler<EventArgs> ClientSizeChanged;
        public event EventHandler<EventArgs> OrientationChanged;
        public event EventHandler<EventArgs> ScreenDeviceNameChanged;

        #endregion

        #region Protected Methods

        protected void OnActivated()
        {
        }

        protected void OnClientSizeChanged()
        {
            if (ClientSizeChanged != null) ClientSizeChanged(this, EventArgs.Empty);
        }

        protected void OnDeactivated()
        {
        }

        protected void OnOrientationChanged()
        {
            if (OrientationChanged != null) OrientationChanged(this, EventArgs.Empty);
        }

        protected void OnPaint()
        {
        }

        protected void OnScreenDeviceNameChanged()
        {
            if (ScreenDeviceNameChanged != null) ScreenDeviceNameChanged(this, EventArgs.Empty);
        }


        protected void SetTitle(string title)
        {
            Title = title;
        }

        #endregion
    }
}