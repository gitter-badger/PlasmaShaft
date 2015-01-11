using System;
using System.Drawing;
using System.Windows.Forms;
using PlasmaShaft;

namespace PlasmaShaft.GUI
{
    public partial class Window : Form
    {
        public Window() {
            InitializeComponent();
        }

        private void Window_Load(object sender, EventArgs e) {
            Server.OnLog += Write;
            Server.Init();
            System.Timers.Timer timer = new System.Timers.Timer(1000);
            timer.Elapsed += delegate {
                UpdateCPU();
            };
            timer.Start();
        }

        private void Write(string message, LogMessage MSG) {
            if(rtbChat.InvokeRequired) {
                rtbChat.BeginInvoke(new MethodInvoker(() => Write(message, MSG)));
                return;
            }
            rtbChat.SelectionStart = rtbChat.TextLength;
            rtbChat.SelectionLength = 0;
            rtbChat.SelectionColor = Color.Gray;
            rtbChat.AppendText("[" + DateTime.Now.ToString("hh:mm:ss") + "] ");
            rtbChat.SelectionColor = Color.Black;
            bool colorized = true;
            foreach (string part in message.Split('&'))
            {
                if (!message.Contains("&"))
                {
                    colorized = false;
                    break;
                }
                string tmp = part;
                if (part.Length >= 1)
                {
                    switch (part[0])
                    {
                        case '0':
                            rtbChat.SelectionStart = rtbChat.TextLength;
                            rtbChat.SelectionLength = 0;
                            rtbChat.SelectionColor = WindowColor.Black;
                            tmp = part.Substring(1);
                            break;
                        case '1':
                            rtbChat.SelectionStart = rtbChat.TextLength;
                            rtbChat.SelectionLength = 0;
                            rtbChat.SelectionColor = WindowColor.Navy;
                            tmp = part.Substring(1);
                            break;
                        case '2':
                            rtbChat.SelectionStart = rtbChat.TextLength;
                            rtbChat.SelectionLength = 0;
                            rtbChat.SelectionColor = WindowColor.Green;
                            tmp = part.Substring(1);
                            break;
                        case '3':
                            rtbChat.SelectionStart = rtbChat.TextLength;
                            rtbChat.SelectionLength = 0;
                            rtbChat.SelectionColor = WindowColor.Teal;
                            tmp = part.Substring(1);
                            break;
                        case '4':
                            rtbChat.SelectionStart = rtbChat.TextLength;
                            rtbChat.SelectionLength = 0;
                            rtbChat.SelectionColor = WindowColor.Maroon;
                            tmp = part.Substring(1);
                            break;
                        case '5':
                            rtbChat.SelectionStart = rtbChat.TextLength;
                            rtbChat.SelectionLength = 0;
                            rtbChat.SelectionColor = WindowColor.Purple;
                            tmp = part.Substring(1);
                            break;
                        case '6':
                            rtbChat.SelectionStart = rtbChat.TextLength;
                            rtbChat.SelectionLength = 0;
                            rtbChat.SelectionColor = WindowColor.Gold;
                            tmp = part.Substring(1);
                            break;
                        case '7':
                            rtbChat.SelectionStart = rtbChat.TextLength;
                            rtbChat.SelectionLength = 0;
                            rtbChat.SelectionColor = WindowColor.Silver;
                            tmp = part.Substring(1);
                            break;
                        case '8':
                            rtbChat.SelectionStart = rtbChat.TextLength;
                            rtbChat.SelectionLength = 0;
                            rtbChat.SelectionColor = WindowColor.Gray;
                            tmp = part.Substring(1);
                            break;
                        case '9':
                            rtbChat.SelectionStart = rtbChat.TextLength;
                            rtbChat.SelectionLength = 0;
                            rtbChat.SelectionColor = WindowColor.Blue;
                            tmp = part.Substring(1);
                            break;
                        case 'a':
                            rtbChat.SelectionStart = rtbChat.TextLength;
                            rtbChat.SelectionLength = 0;
                            rtbChat.SelectionColor = WindowColor.Lime;
                            tmp = part.Substring(1);
                            break;
                        case 'b':
                            rtbChat.SelectionStart = rtbChat.TextLength;
                            rtbChat.SelectionLength = 0;
                            rtbChat.SelectionColor = WindowColor.Teal;
                            tmp = part.Substring(1);
                            break;
                        case 'c':
                            rtbChat.SelectionStart = rtbChat.TextLength;
                            rtbChat.SelectionLength = 0;
                            rtbChat.SelectionColor = WindowColor.Red;
                            tmp = part.Substring(1);
                            break;
                        case 'd':
                            rtbChat.SelectionStart = rtbChat.TextLength;
                            rtbChat.SelectionLength = 0;
                            rtbChat.SelectionColor = WindowColor.Pink;
                            tmp = part.Substring(1);
                            break;
                        case 'e':
                            rtbChat.SelectionStart = rtbChat.TextLength;
                            rtbChat.SelectionLength = 0;
                            rtbChat.SelectionBackColor = WindowColor.Black;
                            rtbChat.SelectionColor = WindowColor.Yellow;
                            tmp = part.Substring(1);
                            break;
                        case 'f':
                            rtbChat.SelectionStart = rtbChat.TextLength;
                            rtbChat.SelectionLength = 0;
                            rtbChat.SelectionColor = WindowColor.Black;
                            tmp = part.Substring(1);
                            break;
                    }
                    rtbChat.AppendText(tmp);
                    rtbChat.SelectionBackColor = WindowColor.White;
                }
            }
            rtbChat.AppendText((colorized ? String.Empty : message) + Environment.NewLine);

        }

        private void UpdateCPU() {
            if (lblCPU.InvokeRequired) {
                lblCPU.Invoke(new MethodInvoker(UpdateCPU));
                return;
            }
            lblCPU.Text = Server.GetMemoryUsage();
        }
    }
}
