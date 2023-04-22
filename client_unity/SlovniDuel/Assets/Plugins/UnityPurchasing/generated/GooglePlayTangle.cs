#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("2IxukNdnvoN56glE1nd9dopo/LFqgIk5XdqX+HsKLaQi+kWBVv+TA/wdUHX35nJg9pty1PJeePxX4bbnScrEy/tJysHJScrKywzkhk/o2XDUjGuPCtH2En7UBuvK2Q7LAaxQI0Je+KkwJxRA/Bog//mnC+bGExm7bfAfrY5wuoi7/JIYkGFa2RVn/bOVNrp5iIBNw4e2anvhnSZAIvIpoNF6XDiHttUYyIdRvzGiB1bcX5LM+0nK6fvGzcLhTYNNPMbKysrOy8jtzb/J2uvaP8BxRDWkaPCZhJlLCUDE0JynGNAEcSZdCFtMTLFnL9gNVSr5YCF1gAl3v3QkomjLtD7b7v2RXfNHf3DAED7IjTGP9Hbyh5Gsno32il8uGJHxlsnIysvK");
        private static int[] order = new int[] { 5,2,7,7,10,7,6,12,11,12,13,11,12,13,14 };
        private static int key = 203;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
