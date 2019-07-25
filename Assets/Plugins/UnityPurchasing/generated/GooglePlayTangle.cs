#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("Tu/MjJwY3Q5h+9kQhl2+dnnugoHfleMgeNJfjAQ+d/0wOhuIiAz4MMa0rNgtqHqy7uqLcrnik9dLS3Sva6zoX4aNbYWykfzP9oPFioTIsl6zGcVzv5trqdPHG3nMtxQlQu9MIvjoPmX7M0E6IHwwOFOLuA/vese50AtX1JPAZWIQCcb6qHP1ogigvcLQ/eIY/hpADCIHcU6YHj5Ej4i+A+x3ot62RLEwF3rw7B5Hc2oIjttYeblY+f/e63MneEZiVSHxkE427r+AMrGSgL22uZo2+DZHvbGxsbWwsyYja1zl7ynrCvlsZ1C3v7uC3OU1MrG/sIAysbqyMrGxsAPmYinUknmaPn5tA236/oI5fypoLwtGttAAAJJBwUR+Nf7ORbKzsbCx");
        private static int[] order = new int[] { 10,11,5,7,12,9,12,7,12,9,10,13,13,13,14 };
        private static int key = 176;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
