using System;
using System.Collections.Generic;

using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;




namespace 计数软件
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    /// 

    public abstract class test
    {
        [DllImport("kernel32.dll", EntryPoint = "ReadProcessMemory")]
        public static extern int _MemoryReadByteSet(int hProcess, int lpBaseAddress, byte[] lpBuffer, int nSize, int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", EntryPoint = "ReadProcessMemory")]
        public static extern int _MemoryReadInt32(int hProcess, int lpBaseAddress, ref int lpBuffer, int nSize, int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", EntryPoint = "WriteProcessMemory")]
        public static extern int _MemoryWriteByteSet(int hProcess, int lpBaseAddress, byte[] lpBuffer, int nSize, int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", EntryPoint = "WriteProcessMemory")]
        public static extern int _MemoryWriteInt32(int hProcess, int lpBaseAddress, ref int lpBuffer, int nSize, int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", EntryPoint = "GetCurrentProcess")]
        public static extern int GetCurrentProcess();

        [DllImport("kernel32.dll", EntryPoint = "OpenProcess")]
        public static extern int OpenProcess(int dwDesiredAccess, int bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", EntryPoint = "CloseHandle")]
        public static extern int CloseHandle(int hObject);

        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
        public static extern int _CopyMemory_ByteSet_Float(ref float item, ref byte source, int length);


        const int PROCESS_POWER_MAX = 2035711;

        public float inkuse = 0.0f;


        /// <summary>
        /// 读内存整数型
        /// </summary>
        /// <param name="pID">进程ID</param>
        /// <param name="bAddress">0x地址</param>
        /// <returns>0失败</returns>
        public static int ReadMemoryInt32(int pID, int bAddress)
        {
            int num = 0;
            int handle = GetProcessHandle(pID);
            int num3 = test._MemoryReadInt32(handle, bAddress, ref num, 4, 0);
            test.CloseHandle(handle);
            if (num3 == 0)
            {
                return 0;
            }
            else
            {
                return num;
            }
        }

        /// <summary>
        /// 写内存整数型
        /// </summary>
        /// <param name="pID">进程ID</param>
        /// <param name="bAddress">0x地址</param>
        /// <param name="value">写入值</param>
        /// <returns>false失败 true成功</returns>
        public static bool WriteMemoryInt32(int pID, int bAddress, int value)
        {
            int handle = GetProcessHandle(pID);
            int num2 = test._MemoryWriteInt32(handle, bAddress, ref value, 4, 0);
            test.CloseHandle(handle);
            return num2 != 0;
        }

        /// <summary>
        /// 读内存小数型
        /// </summary>
        /// <param name="pID">进程ID</param>
        /// <param name="bAddress">0x地址</param>
        /// <returns>0失败</returns>
        public static float ReadMemoryFloat(int pID, int bAddress)
        {
            //byte[] array = test.GetVoidByteSet(4);
            byte[] array = new byte[4];//不取空字节集也可以正确转换成单精度小数型
            int handle = GetProcessHandle(pID);
            int temp = test._MemoryReadByteSet(handle, bAddress, array, 4, 0);
            if (temp == 0)
            {
                return 0f;
            }
            else
            {
                return test.GetFloatFromByteSet(array, 0);
            }
        }

        public static float ReadMemoryDouble(int pID, int bAddress)
        {
            //byte[] array = test.GetVoidByteSet(4);
            byte[] array = new byte[8];//不取空字节集也可以正确转换成单精度小数型
            int handle = GetProcessHandle(pID);
            int temp = test._MemoryReadByteSet(handle, bAddress, array, 8, 0);
            if (temp == 0)
            {
                return 0f;
            }
            else
            {
                return test.GetFloatFromByteSet(array, 0);
            }
        }

        /// <summary>
        /// 写内存小数型
        /// </summary>
        /// <param name="pID">进程ID</param>
        /// <param name="bAddress">0x地址</param>
        /// <param name="value">写入数据</param>
        /// <returns>false失败</returns>
        public static bool WriteMemoryFloat(int pID, int bAddress, float value)
        {
            //byte[] byteSet = test.GetByteSet(value);
            byte[] byteSet = BitConverter.GetBytes(value);//https://msdn.microsoft.com/en-us/library/yhwsaf3w
                                                          //byte[] byteSet = Encoding.GetEncoding("gb2312").GetBytes(value.ToString());
            return test.WriteMemoryByteSet(pID, bAddress, byteSet, 0);
        }

        /// <summary>
        /// 写内存字节集
        /// </summary>
        /// <param name="pID">进程ID</param>
        /// <param name="bAddress">0x地址</param>
        /// <param name="value">字节数据</param>
        /// <param name="length">写入长度 0代表字节数据的长度</param>
        /// <returns>false失败</returns>
        private static bool WriteMemoryByteSet(int pID, int bAddress, byte[] value, int length = 0)
        {
            int handle = test.GetProcessHandle(pID);
            int nSize = (length == 0) ? value.Length : length;
            int tmp = test._MemoryWriteByteSet(handle, bAddress, value, nSize, 0);//byte[]属于引用类型 引用类型不用ref也是以传址方式进行运算
                                                                                  //test.CloseHandle(pID);
            return tmp != 0;
        }

        /// <summary>
        /// 取空白字节集
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static byte[] GetVoidByteSet(int num)
        {
            if (num <= 0)
            {
                num = 1;
            }
            string text = "";
            for (int i = 0; i < num; i++)
            {
                text += "0";
            }
            return Encoding.UTF8.GetBytes(text);
        }

        /// <summary>
        /// 取进程句柄
        /// </summary>
        /// <param name="pID">进程ID</param>
        /// <returns>进程句柄</returns>
        public static int GetProcessHandle(int pID)
        {
            if (pID == -1)
            {
                return test.GetCurrentProcess();
            }
            else
            {
                return test.OpenProcess(PROCESS_POWER_MAX, 0, pID);
            }
        }

        /// <summary>
        /// 字节集转小数型
        /// </summary>
        /// <param name="sourceValue">字节集</param>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public static float GetFloatFromByteSet(byte[] sourceValue, int index)
        {
            float result = 0f;
            test._CopyMemory_ByteSet_Float(ref result, ref sourceValue[index], 4);
            return result;
        }

        /// <summary>
        /// 获取字节集
        /// </summary>
        /// <param name="data">需要转换到字节集的数据</param>
        /// <returns></returns>
        public static byte[] GetByteSet(float data)
        {
            return Encoding.UTF8.GetBytes(data.ToString());
        }
    }

    public partial class MainWindow : Window
    {
        public double b, cN = 0;
        public int type, coo,iuc = 0;
        public bool inkcstate = false;

        public double inkuseC, inkuseM, inkuseY, inkuseK = 0.0f;
        public double materuse = 0.0f;
        public double distan = 0.0f;
        public MainWindow()
        {
            InitializeComponent();

        }

        public static void Delay(int milliSecond)
        {
            int start = Environment.TickCount;
            while (Math.Abs(Environment.TickCount - start) < milliSecond)//毫秒
            {
                System.Windows.Forms.Application.DoEvents();//可执行某无聊的操作
            }
        }


        public void saveAndStart(object sender, RoutedEventArgs e)
        {

            string macProName = ProgramName.Text;
            string macProAddress = ProgramAddress.Text;

            double a;
            if (ProgramName.Text == "" & ProgramAddress.Text == "")
            {
                System.Windows.MessageBox.Show("错误", "信息未填写！");
            }
            else
            {
                int macProAdd = int.Parse(macProAddress, System.Globalization.NumberStyles.HexNumber);
                Process[] localByName = Process.GetProcessesByName(macProName);
                int programPid = localByName[0].Id;
                if (type == 0)
                {
                    button.Content = "暂停";
                    type = 1;
                    double distance_cache = distan / 1000.0;
                    double useinkC, useinkM, useinkY, useinkK = 0.0f;
                    while (true)
                    {
                        a = test.ReadMemoryFloat(programPid, macProAdd);
                        b = (a / 60) + b;
                        
                        calc.Text = b.ToString("f2");
                        if (coo == 1)
                        {
                            cN = (a / 60) + cN;
                            can.Text = cN.ToString("f2");
                            materuse = (a / 60) + materuse;
                        }
                        if(inkcstate == true)
                        {
                            useinkC = ((materuse + 0.0001) / distance_cache) * inkuseC;
                            useinkM = ((materuse + 0.0001) / distance_cache) * inkuseM;
                            useinkY = ((materuse + 0.0001) / distance_cache) * inkuseY;
                            useinkK = ((materuse + 0.0001) / distance_cache) * inkuseK;

                            INK_C_USE.Text = useinkC.ToString("F5");
                            INK_M_USE.Text = useinkM.ToString("F5");
                            INK_Y_USE.Text = useinkY.ToString("F5");
                            INK_K_USE.Text = useinkK.ToString("F5");
                        }
                        Delay(1000);
                        if (type == 0)
                        {
                            break;
                        }


                    }
                }

                else
                {
                    type = 0;
                    button.Content = "开始";
                }
            }

        }
        public void resetNum(object sender, RoutedEventArgs e)
        {
            b = 0;
        }

        public void CanresetNum(object sender, RoutedEventArgs e)
        {
            cN = 0;
        }

        private void INKREALTIMECALC_Click(object sender, RoutedEventArgs e)
        {
            if (inkcstate == false)
            {
                

                if ((INK_C.Text != "") &&
                    (INK_M.Text != "") &&
                    (INK_Y.Text != "") &&
                    (INK_K.Text != "") &&
                    (distance.Text != ""))
                {
                    if (double.TryParse(INK_C.Text, out inkuseC) &&
                       double.TryParse(INK_M.Text, out inkuseM) &&
                       double.TryParse(INK_Y.Text, out inkuseY) &&
                       double.TryParse(INK_K.Text, out inkuseK) &&
                       double.TryParse(distance.Text,out distan))
                    {
                        if ((inkuseC > 0)
                            && (inkuseM > 0)
                            && (inkuseY > 0)
                            && (inkuseK > 0)
                            && (distan > 0))
                        {
                            inkcstate = true;
                            INKREALTIMECALCbt.Content = "油墨实时计算暂停";

                            /*while (true)
                            {
                                if (type == 0)
                                {
                                    break;
                                }

                                useinkC = ((materuse + 0.0001) / distan) * inkuseC;
                                useinkM = ((materuse + 0.0001) / distan) * inkuseM;
                                useinkY = ((materuse + 0.0001) / distan) * inkuseY;
                                useinkK = ((materuse + 0.0001) / distan) * inkuseK;

                                INK_C_USE.Text = useinkC.ToString("F2");
                                INK_M_USE.Text = useinkM.ToString("F2");
                                INK_Y_USE.Text = useinkY.ToString("F2");
                                INK_K_USE.Text = useinkK.ToString("F2");
                            }*/
                        }
                        else
                        {
                            MessageBox.Show("检查输入是否正确！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("检查输入是否正确！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }
                else
                {
                    MessageBox.Show("检查油墨是否填写！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            else
            {
                inkcstate = false;
                INKREALTIMECALCbt.Content = "油墨实时计算开始";
            }
        }

        public void canOnOFF(object sender, RoutedEventArgs e)
        {
            if (coo == 0)
            {
                coo = 1;
                button_Copy.Content = "暂停";
            }
            else
            {
                coo = 0;
                button_Copy.Content = "开始";
            }
        }
        public void TOFF(object sender, RoutedEventArgs e)
        {
            type = 0;
            Close();
        }

    }
}
