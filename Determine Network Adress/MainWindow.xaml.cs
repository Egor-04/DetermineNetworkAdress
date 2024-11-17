using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Determine_Network_Adress
{
    public partial class MainWindow : Window
    {
        private const int _maxOctetsCount = 4;
        private readonly int[] _bits = [128, 64, 32, 16, 8, 4, 2, 1];
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private string ConvertToBinary(int number)
        {
            if (number == 0) return "00000000"; // Или просто "0", если требуется

            string binary = "";

            // Выполняем преобразование
            while (number > 0)
            {
                binary = (number % 2) + binary;
                number /= 2;
            }

            return binary.PadLeft(8, '0');
        }

        private string ConvertToBinaryByOverridedBits(int number)
        {
            if (number == 0) { return "00000000"; }

            StringBuilder binary = new StringBuilder();

            for (int i = 0; i < _bits.Length; i++)
            {
                if (number >= _bits[i])
                {
                    binary.Append('1');
                    number -= _bits[i];
                }
                else
                {
                    binary.Append('0');
                }
            }

            return binary.ToString();
        }

        private string ConvertToBinaryVeryFast(int number)
        {
            StringBuilder binary = new StringBuilder(8);

            for (int i = 0; i < 8; i++)
            {
                binary.Insert(0, (number & 1) == 1 ? '1' : '0');
                number >>= 1;
            }

            return binary.ToString();
        }

        private string ConvertBinaryOctetToInt(string binaryString)
        {
            int decimalValue = Convert.ToInt32(binaryString, 2);
            return decimalValue.ToString();
        }

        private List<string> SplitOnOctets(string ip)
        {
            string[] octets = ip.Split('.');
            return new List<string>(octets);
        }


        private bool IsValidIPAddress(string ipAddress)
        {
            return IPAddress.TryParse(ipAddress, out IPAddress _);
        }

        private bool IsValidBinaryIPAddress(string binIPAddress)
        {
            try
            {
                int symbolsCount = 0;
                bool isValid = false;
                string[] octets = binIPAddress.Split(".");

                if (octets.Length != 4)
                {
                    isValid = false;
                    return isValid;
                }

                for (int i = 0; i < octets.Length; i++)
                {
                    for (int j = 0; j < octets[i].Length; j++)
                    {
                        symbolsCount++;
                    }
                }

                if (symbolsCount == 35)
                {
                    isValid = true;
                    return isValid;
                }

                return isValid;
            }
            catch (Exception ex)
            {
                TextBox_Debug.Text = ex.ToString();
                return false;
            }
        }


        private string ConvertIPAdressToBinaryCode(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                return string.Empty;
            }

            if (!IsValidIPAddress(address) || SplitOnOctets(address).Count > _maxOctetsCount)
            {
                return "Incorrect IP Address";
            }

            var octets = SplitOnOctets(address);
            var resultBinaryIPAdress = new StringBuilder();

            for (int i = 0; i < octets.Count; i++)
            {
                int convertedToInt = Convert.ToInt32(octets[i]);
                resultBinaryIPAdress.Append(ConvertToBinaryByOverridedBits(convertedToInt));

                if (i < octets.Count - 1)
                {
                    resultBinaryIPAdress.Append('.');
                }
            }

            return resultBinaryIPAdress.ToString();
        }

        private string LogicalAND(string binaryAddress1, string binaryAddress2)
        {
            if (string.IsNullOrEmpty(binaryAddress1) || string.IsNullOrEmpty(binaryAddress2) ||
                binaryAddress1.Length < 35 || binaryAddress2.Length < 35)
            {
                return "Incorrect or not full IP & Mask Addresses";
            }

            var octet = new StringBuilder();
            var resultIntIpAddress = new StringBuilder();

            for (int i = 0; i < binaryAddress1.Length; i++)
            {
                char x = binaryAddress1[i];
                char y = binaryAddress2[i];

                if (x == '.' && y == '.')
                {
                    resultIntIpAddress.Append(octet);
                    resultIntIpAddress.Append('.');
                    octet.Clear();
                    continue;
                }

                switch (x, y)
                {
                    case ('0', '0'):
                        octet.Append('0');
                        break;
                    case ('0', '1'):
                        octet.Append('0');
                        break;
                    case ('1', '0'):
                        octet.Append('0');
                        break;
                    case ('1', '1'):
                        octet.Append('1');
                        break;
                }
            }

            if (octet.Length > 0)
            {
                resultIntIpAddress.Append(octet);
            }

            return resultIntIpAddress.ToString();
        }

        private string ConvertBinaryIPAdressToInt(string binaryAddress)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(binaryAddress))
                {
                    return string.Empty;
                }

                if (IsValidBinaryIPAddress(binaryAddress) || SplitOnOctets(binaryAddress).Count > _maxOctetsCount || SplitOnOctets(binaryAddress).Count < _maxOctetsCount)
                {
                    return "Incorrect IP Address";
                }

                var octets = SplitOnOctets(binaryAddress);
                var resultBinaryIPAdress = new StringBuilder();

                for (int i = 0; i < octets.Count; i++)
                {
                    resultBinaryIPAdress.Append(ConvertBinaryOctetToInt(octets[i]));

                    if (i < octets.Count - 1)
                    {
                        resultBinaryIPAdress.Append('.');
                    }
                }

                return resultBinaryIPAdress.ToString();
            }
            catch (Exception ex)
            {
                TextBox_Debug.Text = ex.ToString();
                return string.Empty;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            TextBox_BinaryNetAdressResult.Text = ConvertIPAdressToBinaryCode(TextBox_NetworkAdress.Text);
        }

        private void TextBox_MaskAdress_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox_BinaryMaskAdressResult.Text = ConvertIPAdressToBinaryCode(TextBox_MaskAdress.Text);
            TextBox_BinaryAddressToIntResult.Text = LogicalAND(TextBox_BinaryNetAdressResult.Text, TextBox_BinaryMaskAdressResult.Text);
            TextBox_ServiceAddressResult.Text = ConvertBinaryIPAdressToInt(TextBox_BinaryAddressToIntResult.Text);
        }

        private void TextBox_BinaryNetAdressResult_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_BinaryMaskAdressResult_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_BinaryAddressToIntResult_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void TextBox_ServiceAddressResult_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}