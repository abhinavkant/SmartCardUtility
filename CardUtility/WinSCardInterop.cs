using System.Runtime.InteropServices;
using System.Text;
using System;

namespace CardUtility
{
    internal class WinSCardInterop
    {
        [DllImport("WinScard.dll", EntryPoint = "SCardEstablishContext")]
        public static extern uint SCardEstablishContext(
            uint dwScope,
            IntPtr nNotUsed1,
            IntPtr nNotUsed2,
            ref IntPtr phContext);

        [DllImport("WinScard.dll", EntryPoint = "SCardReleaseContext")]
        public static extern uint SCardReleaseContext(
            IntPtr hContext);

        [DllImport("winscard.dll", EntryPoint = "SCardGetStatusChangeW", CharSet = CharSet.Unicode)]
        public static extern uint SCardGetStatusChangeW(
            IntPtr hContext,
            uint dwTimeout,
            [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)]
                WinSCardInterop.ReaderState[] rgReaderState,
            uint cReaders);

        [DllImport("winscard.dll", EntryPoint = "SCardListReadersW", CharSet = CharSet.Unicode)]
        public static extern uint SCardListReadersW(
            IntPtr hContext,
            string groups,
            string readers,
            ref int size);

        [DllImport("WinSCard.dll", CharSet = CharSet.Auto)]
        public static extern uint SCardConnect(
            IntPtr hContext, 
            string szReader, 
            int dwShareMode, 
            int dwPreferredProtocols, 
            out IntPtr phCard, 
            out int pdwActiveProtocol);

        [DllImport("WinSCard.dll")]
        public static extern uint SCardTransmit(
            IntPtr hCard, 
            ref IO_REQUEST pioSendPci, 
            byte[] pbSendBuffer, 
            int cbSendLength, 
            IntPtr pioRecvPci, 
            byte[] pbRecvBuffer, 
            ref int pcbRecvLength);

        [DllImport("WinSCard.dll")]
        public static extern uint SCardDisconnect(
            IntPtr hCard, 
            int dwDisposition);


        #region Error codes
        public const uint S_SUCCESS = 0x00000000;
        public const uint F_INTERNAL_ERROR = 0x80100001;
        public const uint E_CANCELLED = 0x80100002;
        public const uint E_INVALID_HANDLE = 0x80100003;
        public const uint E_INVALID_PARAMETER = 0x80100004;
        public const uint E_INVALID_TARGET = 0x80100005;
        public const uint E_NO_MEMORY = 0x80100006;
        public const uint F_WAITED_TOO_LONG = 0x80100007;
        public const uint E_INSUFFICIENT_BUFFER = 0x80100008;
        public const uint E_UNKNOWN_READER = 0x80100009;
        public const uint E_TIMEOUT = 0x8010000A;
        public const uint E_SHARING_VIOLATION = 0x8010000B;
        public const uint E_NO_SMARTCARD = 0x8010000C;
        public const uint E_UNKNOWN_CARD = 0x8010000D;
        public const uint E_CANT_DISPOSE = 0x8010000E;
        public const uint E_PROTO_MISMATCH = 0x8010000F;
        public const uint E_NOT_READY = 0x80100010;
        public const uint E_INVALID_VALUE = 0x80100011;
        public const uint E_SYSTEM_CANCELLED = 0x80100012;
        public const uint F_COMM_ERROR = 0x80100013;
        public const uint F_UNKNOWN_ERROR = 0x80100014;
        public const uint E_INVALID_ATR = 0x80100015;
        public const uint E_NOT_TRANSACTED = 0x80100016;
        public const uint E_READER_UNAVAILABLE = 0x80100017;
        public const uint P_SHUTDOWN = 0x80100018;
        public const uint E_PCI_TOO_SMALL = 0x80100019;
        public const uint E_READER_UNSUPPORTED = 0x8010001A;
        public const uint E_DUPLICATE_READER = 0x8010001B;
        public const uint E_CARD_UNSUPPORTED = 0x8010001C;
        public const uint E_NO_SERVICE = 0x8010001D;
        public const uint E_SERVICE_STOPPED = 0x8010001E;
        public const uint E_UNEXPECTED = 0x8010001F;
        public const uint E_ICC_INSTALLATION = 0x80100020;
        public const uint E_ICC_CREATEORDER = 0x80100021;
        public const uint E_UNSUPPORTED_FEATURE = 0x80100022;
        public const uint E_DIR_NOT_FOUND = 0x80100023;
        public const uint E_FILE_NOT_FOUND = 0x80100024;
        public const uint E_NO_DIR = 0x80100025;
        public const uint E_NO_FILE = 0x80100026;
        public const uint E_NO_ACCESS = 0x80100027;
        public const uint E_WRITE_TOO_MANY = 0x80100028;
        public const uint E_BAD_SEEK = 0x80100029;
        public const uint E_INVALID_CHV = 0x8010002A;
        public const uint E_UNKNOWN_RES_MNG = 0x8010002B;
        public const uint E_NO_SUCH_CERTIFICATE = 0x8010002C;
        public const uint E_CERTIFICATE_UNAVAILABLE = 0x8010002D;
        public const uint E_NO_READERS_AVAILABLE = 0x8010002E;
        public const uint E_COMM_DATA_LOST = 0x8010002F;
        public const uint E_NO_KEY_CONTAINER = 0x80100030;
        public const uint W_UNSUPPORTED_CARD = 0x80100065;
        public const uint W_UNRESPONSIVE_CARD = 0x80100066;
        public const uint W_UNPOWERED_CARD = 0x80100067;
        public const uint W_RESET_CARD = 0x80100068;
        public const uint W_REMOVED_CARD = 0x80100069;
        public const uint W_SECURITY_VIOLATION = 0x8010006A;
        public const uint W_WRONG_CHV = 0x8010006B;
        public const uint W_CHV_BLOCKED = 0x8010006C;
        public const uint W_EOF = 0x8010006D;
        public const uint W_CANCELLED_BY_USER = 0x8010006E;
        public const uint W_CARD_NOT_AUTHENTICATED = 0x8010006F;
        #endregion

        public const uint SCOPE_USER = 0;
        public const uint SCOPE_TERMINAL = 1;
        public const uint SCOPE_SYSTEM = 2;

        public const int SHARE_SHARED = 2;
        public const int PROTOCOL_T0 = 1;
        public const int PROTOCOL_T1 = 2;


        public const int LEAVE_CARD = 0;

        public const string GROUP_ALL_READERS = "SCard$AllReaders\0\0";
        public const string GROUP_DEFAULT_READERS = "SCard$DefaultReaders\0\0";
        public const string GROUP_LOCAL_READERS = "SCard$LocalReaders\0\0";
        public const string GROUP_SYSTEM_READERS = "SCard$SystemReaders\0\0";

        public const uint STATE_UNAWARE = 0x00000000;
        public const uint STATE_IGNORE = 0x00000001;
        public const uint STATE_CHANGED = 0x00000002;
        public const uint STATE_UNKNOWN = 0x00000004;
        public const uint STATE_UNAVAILABLE = 0x00000008;
        public const uint STATE_EMPTY = 0x00000010;
        public const uint STATE_PRESENT = 0x00000020;
        public const uint STATE_ATRMATCH = 0x00000040;
        public const uint STATE_EXCLUSIVE = 0x00000080;
        public const uint STATE_INUSE = 0x00000100;
        public const uint STATE_MUTE = 0x00000200;
        public const uint STATE_UNPOWERED = 0x00000400;

        [StructLayout(LayoutKind.Sequential)]
        public struct IO_REQUEST
        {
            public int dwProtocol;
            public int cbPciLength;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct ReaderState
        {
            public ReaderState(string sName)
            {
                szReader = sName;
                pvUserData = IntPtr.Zero;
                dwCurrentState = 0;
                dwEventState = 0;
                cbATR = 0;
                rgbATR = null;
            }

            internal string szReader;
            internal IntPtr pvUserData;
            internal uint dwCurrentState;
            internal uint dwEventState;
            internal uint cbATR;    // count of bytes in rgbATR
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x24, ArraySubType = UnmanagedType.U1)]
            internal byte[] rgbATR;
        }

        public static string ToHex(byte[] ab, string sDelim)
        {
            if (ab == null) return "<NULL>";
            return ToHex(ab, 0, ab.Length, sDelim);
        }

        public static string ToHex(byte[] ab, int offset, int len, string sDelim)
        {
            if (ab == null) return "<NULL>";
            StringBuilder sb = new StringBuilder();
            len = Math.Min(offset + len, ab.Length);
            for (int i = offset; i < len; i++)
                sb.Append(String.Format("{0:x02}", ab[i]).ToUpper() + sDelim);
            return sb.ToString();
        }
    }


}
