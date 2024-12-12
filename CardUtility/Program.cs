using System.Runtime.InteropServices;
using System;
using CardUtility;
using System.Collections.Generic;
using System.Linq;
using System.IO;

internal class Program
{
    private static int Main(string[] args)
    {
        Console.WriteLine("Card Utility");

        nint hContext = nint.Zero;

        uint result = WinSCardInterop.SCardEstablishContext(
            WinSCardInterop.SCOPE_SYSTEM,
            nint.Zero,
            nint.Zero,
            ref hContext);

        if (result != WinSCardInterop.S_SUCCESS)
        {
            Console.WriteLine("Failed to establish context: " + result);
            return 1;
        }



        var readers = GetReaderList(hContext);
        Console.WriteLine("Connected Smart Card Readers:");
        for (int i = 0; i < readers.Count(); i++)
        {
            Console.WriteLine($"[{i}] {readers.ElementAt(i)}");
        }

        if (readers.Count() == 0)
        {
            Console.WriteLine("No smart card readers found.");
            return 1;
        }

        Console.Write("Select a reader by index: ");
        int readerIndex = int.Parse(Console.ReadLine());
        if (readerIndex < 0 || readerIndex >= readers.Count())
        {
            Console.WriteLine("Invalid selection.");
            return 1;
        }

        // Connect to the selected reader
        string selectedReader = readers.ElementAt(readerIndex);
        nint hCard;
        int activeProtocol;
        result = WinSCardInterop.SCardConnect(
            hContext,
            selectedReader,
            WinSCardInterop.SHARE_SHARED,
            WinSCardInterop.PROTOCOL_T0 | WinSCardInterop.PROTOCOL_T1,
            out hCard,
            out activeProtocol);
        if (result != WinSCardInterop.S_SUCCESS)
        {
            Console.WriteLine("Failed to connect to reader: " + result);
            return 1;
        }

        Console.WriteLine("Connected to reader.");
        Console.WriteLine($"Send APDU Command prefixed by '<'{Environment.NewLine}\tenter h for history {Environment.NewLine}\tenter cls to clear console{Environment.NewLine}\tenter r to select reader {Environment.NewLine}\tpress q to exit");
        var commandHistory = new List<string>();
        do
        {
            var data = Console.ReadLine();

            if (data.StartsWith("<"))
            {
                var commandString = data
                    .Replace("<", string.Empty)
                    .Trim();
                commandHistory.Add(commandString);
                var commnadByteArray = Convert.FromHexString(commandString.Replace(" ", string.Empty));
                SendAPDUCommand(
                    commnadByteArray,
                    hCard,
                    activeProtocol);
            }
            else if (data.StartsWith("h", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine("Command History:");
                for (var i = 0; i < commandHistory.Count(); i++)
                {
                    Console.WriteLine("{0} {1}", i, commandHistory[i]);
                }
            }
            else if (data.StartsWith("cls", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.Clear();
            }
            else if (data.StartsWith("q", StringComparison.InvariantCultureIgnoreCase))
            {
                break;
            }
        } while (true);

        // Disconnect from the reader
        WinSCardInterop.SCardDisconnect(hCard, WinSCardInterop.LEAVE_CARD);
        Console.WriteLine("Disconnected from reader.");
        return 0;
    }

    static IEnumerable<string> GetReaderList(nint hContext)
    {
        var list = new List<string>();
        int nStringLength = 0;
        uint result = WinSCardInterop.SCardListReadersW(
            hContext,
            WinSCardInterop.GROUP_ALL_READERS,
            null,
            ref nStringLength);
        if (result != WinSCardInterop.S_SUCCESS)
        {
            Console.WriteLine("Failed to get reader list length: " + result);
            return list;
        }

        string sReaders = new string(' ', nStringLength);
        result = WinSCardInterop.SCardListReadersW(
            hContext,
            WinSCardInterop.GROUP_ALL_READERS,
            sReaders,
            ref nStringLength);
        if (result != WinSCardInterop.S_SUCCESS)
        {
            Console.WriteLine("Failed to list readers: " + result);
            return list;
        }

        list = new List<string>(sReaders.Split('\0'));
        for (int i = 0; i < list.Count;)
        {
            if (list[i].Trim().Length > 0)
                i++;
            else
                list.RemoveAt(i);
        }
        return list;
    }

    static uint SendAPDUCommand(byte[] apduCommand, nint hCard, int activeProtocol)
    {
        uint result;
        LogToFile(string.Format("< {0}", BitConverter.ToString(apduCommand).Replace("-", " ")));

        byte[] responseBuffer = new byte[256];
        int responseLength = responseBuffer.Length;

        var ioRequest = new WinSCardInterop.IO_REQUEST
        {
            dwProtocol = activeProtocol,
            cbPciLength = Marshal.SizeOf(typeof(WinSCardInterop.IO_REQUEST))
        };

        //Console.WriteLine($"< {BitConverter.ToString(apduCommand)}");

        result = WinSCardInterop.SCardTransmit(
            hCard,
            ref ioRequest,
            apduCommand,
            apduCommand.Length,
            nint.Zero,
            responseBuffer,
            ref responseLength);

        if (result != WinSCardInterop.S_SUCCESS)
        {
            Console.WriteLine("Failed to transmit APDU: " + result);
        }
        else
        {
            var response = string.Format("> {0} \n", BitConverter.ToString(responseBuffer, 0, responseLength).Replace("-", " "));
            Console.WriteLine(response);
            LogToFile(response);
        }

        return result;
    }

    static void LogToFile(string log)
    {
        File.AppendAllText(@"commandlog.txt", $"{DateTime.Now.ToString()} {log} {Environment.NewLine}");
    }
}