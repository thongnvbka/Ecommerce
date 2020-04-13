namespace Common.Constant
{

    public class PrefixSystem
    {
        public static string PrefixUser = "USE";
        public static string PrefixCourse = "COU";
        public static string PrefixClass = "CLA";
        public static string PrefixClassFree = "CLF";
        public static string PrefixOrder = "ORD";

        public static string PrefixTokenCode = "TOK";
    }

    public class ConstantSystem
    {
        public static string Date = "dd/MM/yyyy";
        public static string DateTime = "dd/mm/yyyy - hh:mm:ss tt";


        public static int RandomNumblerUserCode = 7;
        public static int RandomNumblerCourseCode = 7;
        public static int RandomNumblerClassCode = 7;
        public static int RandomNumblerClassFreeCode = 7;
        public static int RandomNumblerOrderCode = 7;

        public static int RandomNumberLicenseKeyParam = 4;

        public static int GeneratePasswordLength = 10;

        public static int RandomNumblerTokenCode = 100;
        public static int RandomNumblerResetPassworCode = 5;
    }

    public static class Result
    {
        public static bool Succeed = true;
        public static bool Failed = false;
    }

    public static class MsgType
    {
        public static int Success = 1;
        public static int Info = 2;
        public static int Warning = 3;
        public static int Error = -1;
    }

    public class ReturnFunc
    {
        public bool status;
        public string msg;
    }

    public static class SessionCostants
    {
        public static string UserSession = "USER_SESSION";
    }
}
