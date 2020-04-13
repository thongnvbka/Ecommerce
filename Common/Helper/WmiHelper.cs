namespace Common.Helper
{
    public static class WmiHelper
    {
        public static string GetMotherBoardSerial()
        {
            var mosMotherBoardSerial = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
            var motherBoardSerial = string.Empty;
            foreach (var o in mosMotherBoardSerial.Get())
            {
                var mo = (ManagementObject)o;
                motherBoardSerial = mo["SerialNumber"].ToString();
                break;
            }
            return motherBoardSerial.Trim();
        }

        public static string GetCpuProcessorId()
        {
            var mosCpuProcessorId = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            var cpuProcessorId = string.Empty;
            foreach (var o in mosCpuProcessorId.Get())
            {
                var mo = (ManagementObject)o;
                cpuProcessorId = mo["processorID"].ToString();
                break;
            }
            return cpuProcessorId.Trim();
        }

        public static string GetDiskDriveSerial()
        {
            var mosDiskDriveSerial = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            var diskDriveSerial = string.Empty;
            foreach (var o in mosDiskDriveSerial.Get())
            {
                var mo = (ManagementObject)o;
                diskDriveSerial = mo["SerialNumber"].ToString();
                break;
            }
            return diskDriveSerial.Trim();
        }
    }
}