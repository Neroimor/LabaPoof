using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabAnd
{
    internal class StringRemot
    {

        public string ConcatByteString(byte[] a)
        {
            string[] ab = new string[a.Length];
            //richTextBox3.Clear();
            for (int i = 0; i < a.Length; i++)
            {

                ab[i] = Convert.ToString(a[i], 16);
                if (ab[i].Length < 2)
                {
                    ab[i] = "0" + ab[i];
                }

            }
            string add = string.Join("", ab);
            // Добавляем пробелы через каждые два символа
            string formattedAdd = add;
            return formattedAdd;
        }
    }
}
