namespace MyLibrary
{
    public static class Utility
    {
        /**
         * <summary>配列の最大値を返す。空配列が渡された場合、デフォルト値を返す。</summary>
         * <param name="arr">解析対象の配列</param>
         * <returns>最大値</returns>
         */
        public static Type ArrayMax<Type>(Type[] arr) where Type : System.IComparable
        {
            if (arr.Length > 0)
            {
                Type max = arr[0];
                foreach (Type a in arr)
                {
                    if (max.CompareTo(a) < 0)   max = a;
                }
                return max;
            }
            else
            {
                return default(Type);
            }
        }


        /**
         * <summary>配列の最小値を返す。空配列が渡された場合、デフォルト値を返す。</summary>
         * <param name="arr">解析対象の配列</param>
         * <returns>最小値</returns>
         */
        public static Type ArrayMin<Type>(Type[] arr) where Type : System.IComparable
        {
            if (arr.Length > 0)
            {
                Type min = arr[0];
                foreach (Type a in arr)
                {
                    if (min.CompareTo(a) > 0) min = a;
                }
                return min;
            }
            else
            {
                return default(Type);
            }
        }


        /**
         * <summary>配列の最大値を持つインデックスを返す。複数ある場合、若い番号を優先して返す。空配列が渡された場合、-1を返す。</summary>
         * <param name="arr">解析対象の配列</param>
         * <returns>最大値を持つインデックス</returns>
         */
        public static int ArrayArgmax<Type>(Type[] arr) where Type : System.IComparable
        {
            if (arr.Length > 0)
            {
                Type max = arr[0];
                int index = 0;
                for (int i = 1; i < arr.Length; i++ )
                {
                    if (max.CompareTo(arr[i]) < 0)
                    {
                        max = arr[i];
                        index = i;
                    }
                }
                return index;
            }
            else
            {
                return -1;
            }
        }


        /**
         * <summary>配列の最小値を持つインデックスを返す。複数ある場合、若い番号を優先して返す。空配列が渡された場合、-1を返す。</summary>
         * <param name="arr">解析対象の配列</param>
         * <returns>最小値を持つインデックス</returns>
         */
        public static int ArrayArgmin<Type>(Type[] arr) where Type : System.IComparable
        {
            if (arr.Length > 0)
            {
                Type min = arr[0];
                int index = 0;
                for (int i = 1; i < arr.Length; i++)
                {
                    if (min.CompareTo(arr[i]) > 0)
                    {
                        min = arr[i];
                        index = i;
                    }
                }
                return index;
            }
            else
            {
                return -1;
            }
        }    
    }
}