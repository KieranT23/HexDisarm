#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("Q4BLYsh4u/YRtlEt3+njudUHBum3X+0iYr8V0EkrH+toLhpuzkln9VI39eLLNt2oT0Z2jGS2tELHhWCmzQ1KGTWJB3RBdZqqrZkxPwd4rVhgPSaHROMo1dmSLOmx3865i/3pLJNy2D7cxZvDCAnfCCJueaXppA1VXKmHuMeZgo7YLYArVxjJyX9c3NaDsw7QkfYETVgrnxLZAfxO3aic2DS3ubaGNLe8tDS3t7ZhA2KuKt9NK4JBbvOZGKQjFlELrKUX6srf3rZSKs2Ec14nXL6T78o8U62oa0V2tOhpWUTHDMve2lkE8ZHZVPnhHjJjhjS3lIa7sL+cMP4wQbu3t7eztrVf+vxMV6SaxWltqt8N2ba40sBQ1N9yEwuqqNHqfbS1t7a3");
        private static int[] order = new int[] { 10,8,3,11,9,13,11,12,8,11,13,11,13,13,14 };
        private static int key = 182;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
