using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using HIDCtrl;
using Drivers;
using KeyboardUtils;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace KeySender
{
    //create the HIDController object
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        int cordal1x;
        int cordal1y;
        int cordal2x;
        int cordal2y;
        int kontrol = 0;

        Process[] prc1;
        Process[] prc2;

        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        

        [Flags]
        public enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }

        /// <summary>

        /// </summary>
        static Socket dinleyiciSoket = new Socket
     (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        const int PORT = 52004;
        TcpListener dinle;
        private HIDController HID = new HIDController();
        private KbUtils KUtils = new KbUtils();
      
        private uint FTimeout = 5000;  //approx five seconds
        int cnt2,cnt3= 0;
        int pbufdurum = 0;
        int ptkdurum = 0;
        bool isEnable = false;
        string data="";
        public Form1()
        {
            InitializeComponent();

        }

         private void Form1_Shown(object sender, EventArgs e)
        {
            
            //create the HIDController 
            
            HID.VendorID = (ushort)DriversConst.TTC_VENDORID;                
            HID.ProductID = (ushort)DriversConst.TTC_PRODUCTID_KEYBOARD;     
            HID.Connect();
            tmrPing.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            HID.Disconnect();
        }

 

      
        void Send(Byte Modifier, Byte Padding, Byte Key0, Byte Key1, Byte Key2, Byte Key3, Byte Key4, Byte Key5)
        {
            SetFeatureKeyboard KeyboardData = new SetFeatureKeyboard();
            KeyboardData.ReportID = 1;
            KeyboardData.CommandCode = 2;
            KeyboardData.Timeout = FTimeout / 5;
            KeyboardData.Modifier = Modifier;

            KeyboardData.Padding = Padding;
            KeyboardData.Key0 = Key0;
            KeyboardData.Key1 = Key1;
            KeyboardData.Key2 = Key2;
            KeyboardData.Key3 = Key3;
            KeyboardData.Key4 = Key4;
            KeyboardData.Key5 = Key5;

            byte[] buf = getBytesSFJ(KeyboardData, Marshal.SizeOf(KeyboardData));

            HID.SendData(buf, (uint)Marshal.SizeOf(KeyboardData));
        }


        public byte[] getBytesSFJ(SetFeatureKeyboard sfj, int size)
        {
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(sfj, ptr, false);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        private void tmrPing_Tick(object sender, EventArgs e)
        {
            tmrPing.Stop();
            Ping();
            tmrPing.Start();
        }


        void Ping()
        {
            SetFeatureKeyboard KeyboardData = new SetFeatureKeyboard();
            KeyboardData.ReportID = 1;
            KeyboardData.CommandCode = 3;

            KeyboardData.Timeout = FTimeout / 5; 
            KeyboardData.Modifier = 0;
            KeyboardData.Padding = 0;
            KeyboardData.Key0 = 0;
            KeyboardData.Key1 = 0;
            KeyboardData.Key2 = 0;
            KeyboardData.Key3 = 0;
            KeyboardData.Key4 = 0;
            KeyboardData.Key5 = 0;

            byte[] buf = getBytesSFJ(KeyboardData, Marshal.SizeOf(KeyboardData));

            HID.SendData(buf, (uint)Marshal.SizeOf(KeyboardData));
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            prc1 = Process.GetProcessesByName("Sender");
            prc2 = Process.GetProcessesByName("T2");
            isEnable = true;
            pbufdurum = 1;
            ptkdurum = 1;
            await Task.WhenAll(pbuff(), ptk(),trd(), ms());
            //timer1.Start();
            //timer2.Start(); w
        }

        private void button2_Click(object sender, EventArgs e)
        {
            isEnable = false;
            pbufdurum = 0;
            ptkdurum = 0;
            cnt2 = 0;
            cnt3 = 0;
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            dinle = new TcpListener(IPAddress.Any, PORT);
            dinle.Start();
            await tbaglan();
        }

        public void gonder(byte a,byte b, byte c, byte d, byte e, byte f)
        {
           // SetTarget();
            //press the 'a' key
            Send(0,0,a, b, c, d, e, f);
            System.Threading.Thread.Sleep(50);
            Send(0, 0, 0, 0, 0, 0, 0, 0);
        }

    

        public void rd()
        {
            while(dinleyiciSoket.Connected)
            {
                byte[] gelenData = new byte[256];
                dinleyiciSoket.Receive(gelenData);
                data = (Encoding.UTF8.GetString(gelenData)).Split('\0')[0];
                if (data == "1")
                {
                 
                    label2.Text = data;
                    if (kontrol == 1)
                    {
                        SetForegroundWindow(prc2[0].MainWindowHandle);
                        kontrol = 0;
                    }
                }

                if (data == "2")
                {
                
                    label2.Text = data;
                    if (kontrol == 1)
                    {
                        SetForegroundWindow(prc2[0].MainWindowHandle);
                        kontrol = 0;
                    }
                }

                if (data == "3")
                {
                    
                    label2.Text = "fare";
                }
            }
        }

        private void baglan()
        {
            label1.Text = "waiting...";
            dinleyiciSoket = dinle.AcceptSocket();
            label1.Text = "connected";
        }
        Task tbaglan()
        {
            return Task.Run(async () =>
            {
                await Task.Delay(100);
                baglan();      
            });
        }

        Task trd()
        {
            return Task.Run(async () =>
            {
                await Task.Delay(50);
                while (isEnable==true && dinleyiciSoket.Connected)
                {
                    rd();
                }
                
            });
        }

        Task pbuff()
        {
            return Task.Run(async () =>
            {
                while (isEnable == true) { 
                if (pbufdurum == 1 && data=="2")
                {
                      
                    await Task.Delay(1000);
                    int deger2 = Convert.ToInt32(textBox2.Text);
                    int deger3 = Convert.ToInt32(textBox3.Text);
                    cnt2++;  //b1
                    cnt3++;  //b2             
                    if (cnt2 == (deger2))
                    {
                        ptkdurum = 0;
                        await Task.Delay(2000);
                        gonder(36, 0, 0, 0, 0, 0);
                        await Task.Delay(50);
                        gonder(36, 0, 0, 0, 0, 0);
                        await Task.Delay(2000);
                        gonder(37, 0, 0, 0, 0, 0);
                        await Task.Delay(50);
                        gonder(37, 0, 0, 0, 0, 0);
                        cnt2 = 0;
                        await Task.Delay(2000);
                        ptkdurum = 1;
                    }

                    if (cnt3 == deger3)
                    {
                        gonder(38, 0, 0, 0, 0, 0);
                        cnt3 = 0;
                    }           
                }
               }
            });
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dinleyiciSoket.Close();
            dinle.Stop();
        }

        Task ptk()
        {
            return Task.Run(async () =>
            {
                while (isEnable == true)
                {
                    if (ptkdurum == 1 && data=="2")
                    {
                        await Task.Delay(250);
                        gonder(30, 0, 0, 0, 0, 0);
                        await Task.Delay(50);
                        //gonder(53, 0, 0, 0, 0, 0);
                        //await Task.Delay(50);
                        //await Task.Delay(400);
                    }
                }
            });
        }

  
        private void Form1_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.NumPad1)
            {
                cordal1x = Cursor.Position.X;
                cordal1y = Cursor.Position.Y;
                label3.Text = cordal1x.ToString() + " , " + cordal1y.ToString();
            }
            if (e.KeyCode == Keys.NumPad2)
            {
                cordal2x = Cursor.Position.X;
                cordal2y = Cursor.Position.Y;
                label4.Text = cordal2x.ToString() + " , " + cordal2y.ToString();
            }
        }

        Task ms()
        {
            return Task.Run(async () =>
            {
                while (isEnable == true)
                {
                    if (data == "3")
                    {
                        SetForegroundWindow(prc1[0].MainWindowHandle);

                        Cursor.Position = new Point(cordal1x, cordal1y);
                        mouse_event((int)(MouseEventFlags.LEFTDOWN), 0, 0, 0, 0);
                        mouse_event((int)(MouseEventFlags.MOVE), 0, 0, 0, 0);
                        await Task.Delay(200);
                        Cursor.Position = new Point(cordal2x, cordal2y);
                        await Task.Delay(200);
                        mouse_event((int)(MouseEventFlags.LEFTUP), 0, 0, 0, 0);
                        SetForegroundWindow(prc2[0].MainWindowHandle);
                        await Task.Delay(1000);
                        kontrol = 1;
                    }
                }
            });
        }
    }

}
