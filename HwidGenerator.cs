using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveRoulette
{
    static class HwIdGenerator
    {
        [StructLayout(LayoutKind.Sequential, Size = 8)]
        public class IDEREGS
        {
            public byte Features;
            public byte SectorCount;
            public byte SectorNumber;
            public byte CylinderLow;
            public byte CylinderHigh;
            public byte DriveHead;
            public byte Command;
            public byte Reserved;
        }

        [StructLayout(LayoutKind.Sequential, Size = 32)]
        public class SENDCMDINPARAMS
        {
            public int BufferSize;
            public IDEREGS DriveRegs;
            public byte DriveNumber;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] Reserved;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public int[] Reserved2;
            public SENDCMDINPARAMS()
            {
                DriveRegs = new IDEREGS();
                Reserved = new byte[3];
                Reserved2 = new int[4];
            }
        }
        [StructLayout(LayoutKind.Sequential, Size = 12)]
        public class DRIVERSTATUS
        {
            public byte DriveError;
            public byte IDEStatus;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] Reserved;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public int[] Reserved2;
            public DRIVERSTATUS()
            {
                Reserved = new byte[2];
                Reserved2 = new int[2];
            }
        }



        [DllImport("kernel32.dll", CharSet = CharSet.Auto,
    CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern SafeFileHandle CreateFile(
       string lpFileName,
       uint dwDesiredAccess,
       uint dwShareMode,
       IntPtr SecurityAttributes,
       uint dwCreationDisposition,
       uint dwFlagsAndAttributes,
       IntPtr hTemplateFile);


        //for IOCTL_STORAGE_QUERY_PROPERTY
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool DeviceIoControl(
                SafeHandle hDevice,
                uint dwIoControlCode,
                IntPtr lpInBuffer,
                uint nInBufferSize,
                [Out] IntPtr lpOutBuffer,
                uint nOutBufferSize,
                ref uint lpBytesReturned,
                IntPtr lpOverlapped);

        [Obfuscation(Feature = "inline", Exclude = false)]
        public static string GetHardwareID()
        {

            MD5 md5 = MD5.Create();
            string r = "";
            byte[] cpuid = HwIdGenerator.CpuID(1);
            cpuid[3] = 0;
            cpuid[7] = 0;
            cpuid[12] = 0;
            cpuid = md5.ComputeHash((byte[])cpuid.Clone());
            string hddtext = HwIdGenerator.GetHddSerial();
            bool flag = hddtext.Length < 5;
            if (flag)
            {
                MessageBox.Show("Can't generate machine id on this computer.", "Notification");
                return "Error";
            }

            for (int i = 0; i < cpuid.Length; i++)
            {
                hddtext += cpuid[i].ToString("X2");
            }
            hddtext += "bruceJuli";
            byte[] hddid = Encoding.ASCII.GetBytes(hddtext);
            hddid = md5.ComputeHash((byte[])hddid.Clone());
            for (int j = 0; j < 16; j++)
            {
                r += hddid[j].ToString("X2");
            }
            return r;

        }






        [Obfuscation(Feature = "inline", Exclude = false)]
        private static string GetHddSerial()
        {
            string volume = string.Format("\\\\.\\PhysicalDrive{0}", 0);
            SafeFileHandle hndl = HwIdGenerator.CreateFile(volume, 0u, 3u, IntPtr.Zero, 3u, 1u, IntPtr.Zero);
            SafeFileHandle deviceHandle = hndl;
            string serialNumber = "";
            HwIdGenerator.STORAGE_PROPERTY_QUERY query = new HwIdGenerator.STORAGE_PROPERTY_QUERY
            {
                PropertyId = 0u,
                QueryType = 0u
            };
            int inputBufferSize = Marshal.SizeOf(query.GetType());
            IntPtr inputBuffer = Marshal.AllocHGlobal(inputBufferSize);
            Marshal.StructureToPtr<HwIdGenerator.STORAGE_PROPERTY_QUERY>(query, inputBuffer, true);
            uint ioControlCode = 2954240u;
            int headerBufferSize = Marshal.SizeOf(typeof(HwIdGenerator.STORAGE_DESCRIPTOR_HEADER));
            IntPtr headerBuffer = Marshal.AllocHGlobal(headerBufferSize);
            uint headerBytesReturned = 0u;
            bool result = HwIdGenerator.DeviceIoControl(deviceHandle, ioControlCode, inputBuffer, (uint)inputBufferSize, headerBuffer, (uint)headerBufferSize, ref headerBytesReturned, IntPtr.Zero);
            bool flag = (long)headerBufferSize == (long)((ulong)headerBytesReturned) && result;
            if (flag)
            {
                HwIdGenerator.STORAGE_DESCRIPTOR_HEADER header = (HwIdGenerator.STORAGE_DESCRIPTOR_HEADER)Marshal.PtrToStructure(headerBuffer, typeof(HwIdGenerator.STORAGE_DESCRIPTOR_HEADER));
                uint descriptorBufferSize = header.Size;
                IntPtr descriptorBufferPointer = Marshal.AllocHGlobal((int)descriptorBufferSize);
                uint descriptorBytesReturned = 0u;
                result = HwIdGenerator.DeviceIoControl(deviceHandle, ioControlCode, inputBuffer, (uint)inputBufferSize, descriptorBufferPointer, descriptorBufferSize, ref descriptorBytesReturned, IntPtr.Zero);
                bool flag2 = result;
                if (flag2)
                {
                    HwIdGenerator.STORAGE_DEVICE_DESCRIPTOR descriptor = (HwIdGenerator.STORAGE_DEVICE_DESCRIPTOR)Marshal.PtrToStructure(descriptorBufferPointer, typeof(HwIdGenerator.STORAGE_DEVICE_DESCRIPTOR));
                    byte[] descriptorBuffer = new byte[descriptorBufferSize];
                    Marshal.Copy(descriptorBufferPointer, descriptorBuffer, 0, descriptorBuffer.Length);
                    string vendorId = HwIdGenerator.GetData(descriptorBuffer, (int)descriptor.VendorIdOffset, false);
                    string productId = HwIdGenerator.GetData(descriptorBuffer, (int)descriptor.ProductIdOffset, false);
                    string productRevision = HwIdGenerator.GetData(descriptorBuffer, (int)descriptor.ProductRevisionOffset, false);
                    serialNumber = HwIdGenerator.GetData(descriptorBuffer, (int)descriptor.SerialNumberOffset, false);
                }
            }
            return serialNumber;

        }






        private static readonly byte[] x86CodeBytes = new byte[]
        {
            85,
            139,
            236,
            83,
            87,
            139,
            69,
            8,
            15,
            162,
            139,
            125,
            12,
            137,
            7,
            137,
            95,
            4,
            137,
            79,
            8,
            137,
            87,
            12,
            95,
            91,
            139,
            229,
            93,
            195
        };


        private static readonly byte[] x64CodeBytes = new byte[]
        {
            83,
            73,
            137,
            208,
            137,
            200,
            15,
            162,
            65,
            137,
            64,
            0,
            65,
            137,
            88,
            4,
            65,
            137,
            72,
            8,
            65,
            137,
            80,
            12,
            91,
            195
        };






        [Obfuscation(Feature = "inline", Exclude = false)]
        private static string GetData(byte[] array, int index, bool reverse = false)
        {
            bool flag = array == null || array.Length == 0 || index <= 0 || index >= array.Length;
            string result;
            if (flag)
            {
                result = "";
            }
            else
            {
                int i;
                for (i = index; i < array.Length; i++)
                {
                    bool flag2 = array[i] == 0;
                    if (flag2)
                    {
                        break;
                    }
                }
                bool flag3 = index == i;
                if (flag3)
                {
                    result = "";
                }
                else
                {
                    byte[] valueBytes = new byte[i - index];
                    Array.Copy(array, index, valueBytes, 0, valueBytes.Length);
                    if (reverse)
                    {
                        Array.Reverse(valueBytes);
                    }
                    result = Encoding.ASCII.GetString(valueBytes).Trim();
                }
            }
            return result;
        }



        [Obfuscation(Feature = "inline", Exclude = false)]
        private static byte[] CpuID(int level)
        {
            IntPtr codePointer = IntPtr.Zero;
            byte[] result;
            try
            {
                bool flag = IntPtr.Size == 4;
                byte[] codeBytes;
                if (flag)
                {
                    codeBytes = HwIdGenerator.x86CodeBytes;
                }
                else
                {
                    codeBytes = HwIdGenerator.x64CodeBytes;
                }
                codePointer = HwIdGenerator.VirtualAlloc(IntPtr.Zero, new UIntPtr((uint)codeBytes.Length), HwIdGenerator.AllocationType.COMMIT | HwIdGenerator.AllocationType.RESERVE, HwIdGenerator.MemoryProtection.EXECUTE_READWRITE);
                Marshal.Copy(codeBytes, 0, codePointer, codeBytes.Length);
                HwIdGenerator.CpuIDDelegate cpuIdDelg = (HwIdGenerator.CpuIDDelegate)Marshal.GetDelegateForFunctionPointer(codePointer, typeof(HwIdGenerator.CpuIDDelegate));
                GCHandle handle = default(GCHandle);
                byte[] buffer = new byte[16];
                try
                {
                    handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                    cpuIdDelg(level, buffer);
                }
                finally
                {
                    bool flag2 = handle != default(GCHandle);
                    if (flag2)
                    {
                        handle.Free();
                    }
                }
                result = buffer;
            }
            finally
            {
                bool flag3 = codePointer != IntPtr.Zero;
                if (flag3)
                {
                    HwIdGenerator.VirtualFree(codePointer, 0u, 32768u);
                    codePointer = IntPtr.Zero;
                }
            }
            return result;
        }






        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void CpuIDDelegate(int level, byte[] buffer);


        [Flags]
        private enum AllocationType : uint
        {
            // Token: 0x0400016B RID: 363
            COMMIT = 4096u,
            // Token: 0x0400016C RID: 364
            RESERVE = 8192u,
            // Token: 0x0400016D RID: 365
            RESET = 524288u,
            // Token: 0x0400016E RID: 366
            LARGE_PAGES = 536870912u,
            // Token: 0x0400016F RID: 367
            PHYSICAL = 4194304u,
            // Token: 0x04000170 RID: 368
            TOP_DOWN = 1048576u,
            // Token: 0x04000171 RID: 369
            WRITE_WATCH = 2097152u
        }


        [Flags]
        private enum MemoryProtection : uint
        {

            EXECUTE = 16u,

            EXECUTE_READ = 32u,

            EXECUTE_READWRITE = 64u,

            EXECUTE_WRITECOPY = 128u,

            NOACCESS = 1u,

            READONLY = 2u,

            READWRITE = 4u,

            WRITECOPY = 8u,

            GUARD_Modifierflag = 256u,

            NOCACHE_Modifierflag = 512u,

            WRITECOMBINE_Modifierflag = 1024u
        }


        private struct STORAGE_PROPERTY_QUERY
        {

            public uint PropertyId;


            public uint QueryType;


            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public byte[] AdditionalParameters;
        }


        private struct STORAGE_DESCRIPTOR_HEADER
        {

            public uint Version;


            public uint Size;
        }


        private enum STORAGE_BUS_TYPE
        {

            BusTypeUnknown,

            BusTypeScsi,

            BusTypeAtapi,

            BusTypeAta,

            BusType1394,

            BusTypeSsa,

            BusTypeFibre,

            BusTypeUsb,

            BusTypeRAID,

            BusTypeiScsi,

            BusTypeSas,

            BusTypeSata,

            BusTypeSd,

            BusTypeMmc,

            BusTypeVirtual,

            BusTypeFileBackedVirtual,

            BusTypeMax,

            BusTypeMaxReserved = 127
        }


        private struct STORAGE_DEVICE_DESCRIPTOR
        {

            public uint Version;


            public uint Size;


            public byte DeviceType;


            public byte DeviceTypeModifier;


            [MarshalAs(UnmanagedType.U1)]
            public bool RemovableMedia;


            [MarshalAs(UnmanagedType.U1)]
            public bool CommandQueueing;


            public uint VendorIdOffset;


            public uint ProductIdOffset;


            public uint ProductRevisionOffset;


            public uint SerialNumberOffset;


            public HwIdGenerator.STORAGE_BUS_TYPE BusType;


            public uint RawPropertiesLength;


            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
            public byte[] RawDeviceProperties;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr VirtualAlloc(IntPtr lpAddress, UIntPtr dwSize, HwIdGenerator.AllocationType flAllocationType, HwIdGenerator.MemoryProtection flProtect);



        [DllImport("kernel32")]
        private static extern bool VirtualFree(IntPtr lpAddress, uint dwSize, uint dwFreeType);



        //private static uint MAX_NUMBER_OF_DRIVES { get; set; }




    }
}
