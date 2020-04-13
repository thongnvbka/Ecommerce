using Common.Constant;

namespace Common.RandomString
{
    public class RandomStringEzi
    {
        public static string RandomUserCodeEzi(int length)
        {
            var rsg = new RandomStringGenerator(false, false, true, false) { UniqueStrings = true };
            return PrefixSystem.PrefixUser + rsg.Generate(length);
        }

        public static string RandomCourseCodeEzi(int length)
        {
            var rsg = new RandomStringGenerator(false, false, true, false) { UniqueStrings = true };
            return PrefixSystem.PrefixCourse + rsg.Generate(length);
        }

        public static string RandomClassCodeEzi(int length)
        {
            var rsg = new RandomStringGenerator(false, false, true, false) { UniqueStrings = true };
            return PrefixSystem.PrefixClass + rsg.Generate(length);
        }

        public static string RandomClassFreeCodeEzi(int length)
        {
            var rsg = new RandomStringGenerator(false, false, true, false) { UniqueStrings = true };
            return PrefixSystem.PrefixClassFree + rsg.Generate(length);
        }
        public static string RandomOrderCodeEzi(int length)
        {
            var rsg = new RandomStringGenerator(false, false, true, false) { UniqueStrings = true };
            return PrefixSystem.PrefixOrder + rsg.Generate(length);
        }

        public static string RandomTokenCode(int length)
        {
            var rsg = new RandomStringGenerator(true, true, true, false) { UniqueStrings = true };
            return PrefixSystem.PrefixTokenCode + rsg.Generate(length);
        }

        public static string RandomResetPasswordCode(int length)
        {
            var rsg = new RandomStringGenerator(false, false, true, false) { UniqueStrings = true };
            return rsg.Generate(length);
        }

        public static string RandomLicenseParamEzi(int length)
        {
            var rsg = new RandomStringGenerator(false, false, true, false)
            {
                UseUpperCaseCharacters = true,
                UseLowerCaseCharacters = false
            };
            return rsg.Generate(length);
        }

        public static string RandomLicenseKeyEzi(int length)
        {
            var licensKey = "";
            for (var i = 0; i < length; i++)
            {
                if (i == length - 1)
                {
                    licensKey = licensKey + RandomLicenseParamEzi(4);
                }
                else
                {
                    licensKey = licensKey + RandomLicenseParamEzi(4) + "-";
                }
            }
            return licensKey;
        }
    }
}
