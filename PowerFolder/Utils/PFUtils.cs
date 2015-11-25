using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConfigManager = PowerFolder.Configuration.ConfigurationManager;

namespace PowerFolder.Utils
{
    public class PFUtils
    {
        /// <summary>
        /// Reads data from a stream until the end is reached. The
        /// data is returned as a byte array. An IOException is
        /// thrown if any of the underlying IO calls fail.
        /// </summary>
        /// <param name="stream">The stream to read data from</param>
        /// <param name="initialLength">The initial buffer length</param>
        public static byte[] ReadFileChunked(Stream stream, int initialLength)
        {
            if (initialLength < 1)
            {
                initialLength = 32768;
            }

            byte[] buffer = new byte[initialLength];
            int read = 0;

            int chunk;
            while ((chunk = stream.Read(buffer, read, buffer.Length - read)) > 0)
            {
                read += chunk;

                if (read == buffer.Length)
                {
                    int nextByte = stream.ReadByte();

                    if (nextByte == -1)
                    {
                        return buffer;
                    }

                    byte[] newBuffer = new byte[buffer.Length * 2];
                    Array.Copy(buffer, newBuffer, buffer.Length);
                    newBuffer[read] = (byte)nextByte;
                    buffer = newBuffer;
                    read++;
                }
            }
            byte[] ret = new byte[read];
            Array.Copy(buffer, ret, read);
            return ret;
        }

        public static bool TryToParseInteger(string value)
        {
            int number;

            if (value == null)
            {
                return false;
            }
            bool result = int.TryParse(value, out number);

            if (result)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool TryToParseBytes(string value)
        {
            if (value == null)
            {
                return false;
            }

            byte outValue;

            bool result = byte.TryParse(value, out outValue);

            if (result)
            {
                return true;
            }
            return false;
        }

        public static bool TryToParseLong(string value)
        {
            if (value == null)
            {
                return false;
            }

            long outerResult;

            bool result = long.TryParse(value, out outerResult);
            if (result && outerResult > 0)
            {
                return true;
            }
            return false;
        }
        public static string FormatBytes(long bytes)
        {

            const int scale = 1024;
            string[] orders = new string[] { "GB", "MB", "KB", "Bytes" };
            long max = (long)Math.Pow(scale, orders.Length - 1);

            foreach (string order in orders)
            {
                if (bytes > max)
                    return string.Format("{0:##.##} {1}", decimal.Divide(bytes, max), order);

                max /= scale;
            }
            return "0 Bytes";
        }
    }
}
