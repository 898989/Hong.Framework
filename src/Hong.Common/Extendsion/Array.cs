namespace Hong.Common.Extendsion
{
    public static class Array
    {
        /// <summary>截取数组
        /// </summary>
        /// <param name="array">被截取的数组</param>
        /// <param name="startIndex">截取开始位置</param>
        /// <param name="length">截取个数</param>
        /// <returns>返回截取的新数组</returns>
        public static T[] Cut<T>(this T[] array, int startIndex, int length)
        {
            T[] temp = new T[length];

            for (int index = 0; index < length; index++)
                temp[index] = array[startIndex + index];

            return temp;
        }

        /// <summary>合并数组
        /// </summary>
        /// <param name="fromArray">原数组</param>
        /// <param name="intoArray">合并到数组</param>
        /// <param name="startIndex">从数组<see cref="intoArray"/>的什么位置开始放入</param>
        /// <returns>返回最终放入的个数</returns>
        public static int Merge<T>(T[] fromArray, T[] intoArray, int startIndex = 0)
        {
            int length = fromArray.Length + startIndex <= intoArray.Length ? fromArray.Length : intoArray.Length - fromArray.Length - startIndex;

            for (int index = 0; index < length; index++)
                intoArray[startIndex + index] = fromArray[index];

            return length;
        }
    }
}
