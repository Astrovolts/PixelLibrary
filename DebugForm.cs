using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PixelLibrary
{
    using System;
    using System.Drawing;
    using System.Threading;
    using System.Windows.Forms;

    public partial class DebugForm : Form
    {
        private static DebugForm _instance;
        private static readonly object _lock = new object();
        private static Thread _uiThread;

        private DebugForm()
        {
            StartPosition = FormStartPosition.CenterParent;

            InitializeComponent();

            this.TopMost = true;
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        // Singleton instance getter
        public static DebugForm Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _uiThread = new Thread(() =>
                            {
                                Application.Run(new DebugForm());
                            })
                            {
                                IsBackground = true
                            };
                            _uiThread.SetApartmentState(ApartmentState.STA); // Required for Windows Forms
                            _uiThread.Start();

                            // Wait for the instance to be created
                            while (_instance == null)
                            {
                                Thread.Sleep(10);
                            }
                        }
                    }
                }
                return _instance;
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // Assign instance when the handle is created
            _instance = this;
        }

        // Method to update UI with a bitmap
        public void UpdatePicture(Bitmap bitmap)
        {
            if (InvokeRequired)
            {
                // Invoke on the UI thread
                Invoke(new Action<Bitmap>(UpdatePicture), bitmap);
            }
            else
            {
                // Actual UI update logic
                // Assume there is a PictureBox named "pictureBox1" on the form
                if (pictureBox.Image != null)
                {
                    pictureBox.Image.Dispose();
                }
                pictureBox.Image = new Bitmap(bitmap);
            }
        }

        // Cleanup to ensure thread and resources are properly disposed
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            _instance = null;
            Application.ExitThread();
        }
    }

}
