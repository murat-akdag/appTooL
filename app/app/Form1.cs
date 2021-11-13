using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using byteapp;
using Jupiter;

namespace app
{
    public partial class Form1 : Form
    {
        static Socket soket = new Socket
             (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        MemoryModule memoryModule;
        int hizdurum = 0;
        int potdurum = 0;
        float[] cord1, cord2, cord3, cord4;
        int corddurum = 0;
        int start = 0;
        int mapindex1, mapindex2;
        int kontrol1 = 0;
        int kontrol2 = 0;
        int ta_ra = 0;
        List<int> itemindex = new List<int>();
        int result;
        IntPtr cordAddr,hizAddr,potAddr,seyAddr,sellAddr,bagAddr,itemAddr,mapAddr,mapkont;
        int micont = 0;

        public Form1()
        {
            InitializeComponent();
            for (int i = 0; i < 64; i++)
            {
                itemindex.Add(0);
            }

        }

        private  void button1_Click(object sender, EventArgs e)
        {
            memoryModule = new MemoryModule(Convert.ToInt32(textBox1.Text));

            //var baseAdress = new IntPtr(Convert.ToInt64("F", 16));

        }

        private void button3_Click(object sender, EventArgs e)
        {
            corddurum = 1;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            corddurum = 0;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            cord1 = kordal2();
            label4.Text = cord1[0].ToString() + " " + cord1[1].ToString() + " " + cord1[2].ToString();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            cord2 = kordal2();
            mapindex1 = memoryModule.ReadVirtualMemory<int>(mapAddr);
            label5.Text = mapindex1.ToString() + "||" + cord2[0].ToString() + " " + cord2[1].ToString() + " " + cord2[2].ToString();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            cord3 = kordal2();
            label6.Text = cord3[0].ToString() + " " + cord3[1].ToString() + " " + cord3[2].ToString();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            cord4 = kordal2();
            mapindex2 = memoryModule.ReadVirtualMemory<int>(mapAddr);
            label11.Text = mapindex2.ToString() + "||" + cord4[0].ToString() + " " + cord4[1].ToString() + " " + cord4[2].ToString();
        }

        private async void button8_Click(object sender, EventArgs e)
        {
            potdurum = 1;
            await potOto();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            potdurum = 0;
            textBox2.Text = "0";
            memoryModule.WriteVirtualMemory<int>(potAddr, 0);
            memoryModule.WriteVirtualMemory<int>(potAddr + 0x4, 0);
            memoryModule.WriteVirtualMemory<int>(potAddr + 0x8, 0);

        }

        private async void button10_Click(object sender, EventArgs e)
        {
            hizdurum = 1;
            await hiz();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            hizdurum = 0;
            textBox3.Text = "0";
            memoryModule.WriteVirtualMemory<int>(hizAddr, 0);
        }


        private async void button2_Click(object sender, EventArgs e)
        {
            start = 1;
            sgonder("2");
            await Task.WhenAll(bt(),tara());
        }

        private void button12_Click(object sender, EventArgs e)
        {
            start = 0;
            sgonder("1");
            kontrol1 = 0;
            kontrol2 = 0;
        }


        Task kordAl()
        {
            return Task.Run(async () =>
            {

                while (true)
                {
                    if (corddurum == 1)
                    {
                        await Task.Delay(1000);
                        var cordx = memoryModule.ReadVirtualMemory<float>(cordAddr);
                        var cordy = memoryModule.ReadVirtualMemory<float>(cordAddr + 0x8);
                        var cordz = memoryModule.ReadVirtualMemory<float>(cordAddr + 0x4);
                        label1.Invoke((MethodInvoker)(() => label1.Text = cordx.ToString()));
                        label2.Invoke((MethodInvoker)(() => label2.Text = cordy.ToString()));
                        label3.Invoke((MethodInvoker)(() => label3.Text = cordz.ToString()));
                        //.Text = cordz.ToString();
                    }

                    else
                    {
                        //MessageBox.Show("Durdu");
                    }
                }
            });
        }

        private void button19_Click(object sender, EventArgs e)
        {
            sgonder("2");
        }

        private void button20_Click(object sender, EventArgs e)
        {
            sgonder("1");
            ta_ra = 0;
        }

        private async void button17_Click(object sender, EventArgs e)
        {
            ta_ra = 1;
            await tara();
        }

        private void button21_Click(object sender, EventArgs e)
        {
            memoryModule.WriteVirtualMemory<int>(mapAddr, mapindex1);
            memoryModule.WriteVirtualMemory<float>(cordAddr, cord2[0]);
            memoryModule.WriteVirtualMemory<float>(cordAddr + 0x8, cord2[1]);
            memoryModule.WriteVirtualMemory<float>(cordAddr + 0x4, cord2[2]);
        }

        private async void button22_Click(object sender, EventArgs e)
        {

            memoryModule.WriteVirtualMemory<float>(cordAddr, cord3[0]);
            memoryModule.WriteVirtualMemory<float>(cordAddr + 0x8, cord3[1]);
            memoryModule.WriteVirtualMemory<float>(cordAddr + 0x4, cord3[2]);
            await Task.Delay(3000);
            memoryModule.WriteVirtualMemory<int>(bagAddr, 1);
            memoryModule.WriteVirtualMemory<int>(seyAddr, 1);
            memoryModule.WriteVirtualMemory<int>(sellAddr, 8);
        }

        private async void button23_Click(object sender, EventArgs e)
        {
            sgonder("3");
            ta_ra = 1;
            await tara();
        }

        private void button24_Click(object sender, EventArgs e)
        {
            sgonder("1");
            memoryModule.WriteVirtualMemory<int>(mapAddr, mapindex2);
            memoryModule.WriteVirtualMemory<float>(cordAddr, cord4[0]);
            memoryModule.WriteVirtualMemory<float>(cordAddr + 0x8, cord4[1]);
            memoryModule.WriteVirtualMemory<float>(cordAddr + 0x4, cord4[2]);
        }

        private async void button25_Click(object sender, EventArgs e)
        {
            memoryModule.WriteVirtualMemory<float>(cordAddr, cord1[0]);
            memoryModule.WriteVirtualMemory<float>(cordAddr + 0x8, cord1[1]);
            memoryModule.WriteVirtualMemory<float>(cordAddr + 0x4, cord1[2]);
            await Task.Delay(2000);
            sgonder("2");
        }

        Task potOto()
        {
            return Task.Run(async () =>
            {

                while (potdurum == 1)
                {
                    await Task.Delay(1000);

                    var pot = memoryModule.ReadVirtualMemory<int>(potAddr);
                    if (pot == 0)
                    {
                        double temp = Convert.ToDouble(textBox2.Text) / 5.0;
                        int valpot = (int)Math.Ceiling(temp);
                        memoryModule.WriteVirtualMemory<int>(potAddr, valpot);
                        memoryModule.WriteVirtualMemory<int>(potAddr + 0x4, Convert.ToInt32(textBox2.Text));
                        memoryModule.WriteVirtualMemory<int>(potAddr + 0x8, Convert.ToInt32(textBox2.Text));
                    }
                }
            });
        }

        private void button15_Click(object sender, EventArgs e)
        {
            try
            {
                soket.Connect(new IPEndPoint(IPAddress.Parse(textBox7.Text), Convert.ToInt32(textBox6.Text)));

            }
            catch (Exception ex)
            {

                MessageBox.Show("Hata");
            }

        }

        private void button16_Click(object sender, EventArgs e)
        {
            soket.Close();
        }

        private async void Button13_Click(object sender, EventArgs e)
        {
            corddurum = 1;
            await kordAl();
        }


        private  void button18_Click(object sender, EventArgs e)
        {
            string[] pattern = System.IO.File.ReadAllLines(@"..\\Debug\test.txt");        
            cordAddr = (IntPtr)Convert.ToInt64(pattern[0], 16)+ 0xE0;
            hizAddr= (IntPtr)Convert.ToInt64(pattern[1], 16)-0x364;
            potAddr = hizAddr - 0x274;
            seyAddr = (IntPtr)Convert.ToInt64(pattern[2], 16) - 0x235;
            sellAddr = seyAddr + 0x235 + 0xF;
            bagAddr = (IntPtr)Convert.ToInt64(pattern[3], 16) + 0x38;
            itemAddr = (IntPtr)Convert.ToInt64(pattern[4], 16) + 0x158;
            mapAddr = (IntPtr)Convert.ToInt64(pattern[5], 16)-0x24C;
            mapkont = mapAddr - 0x10;

            //cordAddr = await PatternScan(scanValue, "64 00 00 00 E8 03 00 00 E8 03 00 00 E8 03 00 00 FF FF FF FF FF FF") + 0xE0;
            //hizAddr = await PatternScan(scanValue, "00 00 00 00 85 05 00 00 88 05 00 00 90 05 00 00 A1 07") -0x364;
            //potAddr = hizAddr - 0x274;
            //seyAddr= await PatternScan(scanValue, "00 00 00 00 00 00 00 00 00 00 00 03 00 00 00 ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? 00 00 00 FF FF FF FF") - 0x235;
            //sellAddr = seyAddr + 0x235 + 0xF;
            //bagAddr= await PatternScan(scanValue, "C9 00 00 00 8C 00 00 00 8D 00 00 00 8E 00 00 00 8F 00 00") +0x38;
            //itemAddr= await PatternScan(scanValue, "D2 01 00 00 A5 01 00 00 46 02 00 00 C3 01") + 0x2D0;
            //mapAddr = await PatternScan(scanValue, "01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 30 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 85 05 00 00 88 05 00 00 90 05 00 00 A1 07 00 00 A0 07") - 0x24C;
            //mapkont = mapAddr - 0x10;
            MessageBox.Show("1-" + cordAddr.ToString() + "\n"+ "2-" + hizAddr.ToString() + "\n"+ "3-" + potAddr.ToString() + "\n"+ "4-" + seyAddr.ToString() + "\n"+ "5-" + sellAddr.ToString() + "\n"+ "6-" + bagAddr.ToString() + "\n"+"7-" + itemAddr.ToString() + "\n"+ "8-" + mapAddr.ToString() + "\n"+ "9-" + mapkont.ToString() + "\n");
            //scanValue = null;
        }

        Task hiz()
        {
            return Task.Run(async () =>
            {
                while (hizdurum == 1)
                {
                    await Task.Delay(1000);
                    var hiz = memoryModule.ReadVirtualMemory<int>(hizAddr);
                    if (hiz == 0)
                    {
                        memoryModule.WriteVirtualMemory<int>(hizAddr, Convert.ToInt32(textBox3.Text));
                    }
                }
            });
        }


        Task bt()
        {
            return Task.Run(async () =>
            {
                while (start == 1)
                {
                    await Task.Delay(500);

                    if (kontrol1 == 1 && checkBox2.Enabled == true && start == 1)
                    {       //ileri
                        if (kontrol2 == 0 && start == 1)
                        {   //stop
                            sgonder("1");
                            //cordinat 
                            memoryModule.WriteVirtualMemory<int>(mapAddr, mapindex1);
                            memoryModule.WriteVirtualMemory<float>(cordAddr, cord2[0]);
                            memoryModule.WriteVirtualMemory<float>(cordAddr + 0x8, cord2[1]);
                            memoryModule.WriteVirtualMemory<float>(cordAddr + 0x4, cord2[2]);
                            await Task.Delay(3000);
                            kontrol2 = 1;
                        }

                        if (kontrol2 == 1 && start == 1)
                        {
                            //cordinat kontrol
                            var kontrol_map = memoryModule.ReadVirtualMemory<int>(mapkont);
                            if (kontrol_map == 0)
                            {
                                await Task.Delay(15000);
                                kontrol2 = 2;
                            }
                        }

                        if (kontrol2 == 2 && start == 1)
                        {
                            //tel sey
                            memoryModule.WriteVirtualMemory<float>(cordAddr, cord3[0]);
                            memoryModule.WriteVirtualMemory<float>(cordAddr + 0x8, cord3[1]);
                            memoryModule.WriteVirtualMemory<float>(cordAddr + 0x4, cord3[2]);
                            await Task.Delay(3000);
                            memoryModule.WriteVirtualMemory<int>(bagAddr, 1);
                            memoryModule.WriteVirtualMemory<int>(seyAddr, 1);
                            memoryModule.WriteVirtualMemory<int>(sellAddr, 8);
                            kontrol2 = 3;
                        }
                        ////////////////*****************************************************************
                        if (kontrol2 == 3 && start == 1)
                        {
                            if (micont == 0) {
                                
                                
                                sgonder("3");
                                micont = 1;
                                stk();
                            }
                            
                            //sell 64x
                            if (result == 0 && start == 1)
                            {
                                await Task.Delay(2000);
                                kontrol2 = 4;
                                sgonder("1");
                                micont = 0;
                            }
                            
                        }
                        ////////////////
                        ///geri
                        if (kontrol2 == 4 && start == 1)
                        {
                            //map index
                            memoryModule.WriteVirtualMemory<int>(mapAddr, mapindex2);
                            memoryModule.WriteVirtualMemory<float>(cordAddr, cord4[0]);
                            memoryModule.WriteVirtualMemory<float>(cordAddr + 0x8, cord4[1]);
                            memoryModule.WriteVirtualMemory<float>(cordAddr + 0x4, cord4[2]);
                            await Task.Delay(3000);
                            kontrol2 = 5;
                        }

                        if (kontrol2 == 5 && start == 1)
                        {
                            //cordinat kontrol
                            var kontrol_map = memoryModule.ReadVirtualMemory<int>(mapkont);
                            if (kontrol_map == 0)
                            {
                                kontrol2 = 6;
                            }
                        }
                        if (kontrol2 == 6 && start==1)
                        {
                            //tel alan
                            //attack
                            memoryModule.WriteVirtualMemory<float>(cordAddr, cord1[0]);
                            memoryModule.WriteVirtualMemory<float>(cordAddr + 0x8, cord1[1]);
                            memoryModule.WriteVirtualMemory<float>(cordAddr + 0x4, cord1[2]);
                            kontrol1 = 0;
                            kontrol2 = 0;
                            sgonder("2");
                        }

                    }
                }

                if (start == 0)
                {
                    MessageBox.Show("Stop");
                }
            });
        }
       
        Task tara()
        {
            return Task.Run(async () =>
            {
                while (ta_ra==1)
                {

                    await Task.Delay(500);
                    for (int i = 0; i < 64; i++)
                    {
                        var item = memoryModule.ReadVirtualMemory<int>(itemAddr + 24 * i);

                        if (item == 0)
                        {
                            itemindex[i] = -1;
                        }
                        if (item != 0)
                        {
                            itemindex[i] = item;
                        }
                    }

                    var noresult = Enumerable.Range(0, itemindex.Count)
                     .Where(i => itemindex[i] == -1)
                     .ToList();
                    result = itemindex.Count - noresult.Count;
                    label9.Invoke((MethodInvoker)(() => label9.Text = (result).ToString()));
                    
                    if (checkBox1.Enabled == true)
                    {
                            stk();
                    }
                    
                    //if (result == 64) { kontrol1 = 1; }
                }

                //if (start == 0)
                //{
                //    MessageBox.Show("Stop");
                //}
            });
        }
        public void stk()
        {
            for (int i = 0; i < itemindex.Count; i++)
            {
                if (itemindex[i] > 0)
                {
                    var stackx = memoryModule.ReadVirtualMemory<int>(itemAddr + 24 * i + 0x4);
                    var stacky = memoryModule.ReadVirtualMemory<int>(itemAddr + 24 * i + 0x8);
                    if (stackx != 6 || stacky != 6)
                    {
                        memoryModule.WriteVirtualMemory<int>(itemAddr + 24 * i + 0x4, 6);
                        memoryModule.WriteVirtualMemory<int>(itemAddr + 24 * i + 0x8, 6);
                    }
                }
            }
        }
        public float[] kordal2()
        {
            var cordx = memoryModule.ReadVirtualMemory<float>(cordAddr);
            var cordy = memoryModule.ReadVirtualMemory<float>(cordAddr + 0x8);
            var cordz = memoryModule.ReadVirtualMemory<float>(cordAddr + 0x4);
            float[] temp = new float[3];
            temp[0] = cordx; temp[1] = cordy; temp[2] = cordz;
            return temp;
        }


        public void sgonder(string deger){
            if (soket.Connected)
            {
                string gonder = deger;
                soket.Send(Encoding.UTF8.GetBytes(gonder));
            }
        }

















        //Task<IntPtr[]> cordPattern()
        //{
        //    Task<IntPtr[]> pattern = Task.Run(() =>
        //    {
        //        string cord = "00 00 00 01 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 " + strtohex(textBox2.Text);
        //        var patterncord = memoryModule.PatternScan(cord);
        //        //var deger = sharp.ReadString(game[0] + 0x7, false,40);
        //        IntPtr[] temp = patterncord.ToArray();
        //        //MessageBox.Show(result);
        //        return temp;
        //    });
        //    return pattern;
        //}

        //Task<IntPtr[]> cordPattern()
        //{
        //    Task<IntPtr[]> pattern = Task.Run(() =>
        //    {
        //        string cord = "00 00 00 01 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 " + strtohex(textBox2.Text);
        //        var patterncord = memoryModule.PatternScan(cord);
        //        //var deger = sharp.ReadString(game[0] + 0x7, false,40);
        //        IntPtr[] temp = patterncord.ToArray();
        //        //MessageBox.Show(result);
        //        return temp;
        //    });
        //    return pattern;
        //}
















        public string strtohex(string s)
        {
            var bytes = Encoding.ASCII.GetBytes(s);
            var hexString = BitConverter.ToString(bytes);
            hexString = hexString.Replace("-", " ");
            //MessageBox.Show(hexString.ToString());
            return hexString;
        }


    }
}
