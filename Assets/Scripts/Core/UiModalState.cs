namespace HollowStyleMVP.Core
{
    public static class UiModalState
    {
        private static int openCount;
        public static bool HasOpenModal => openCount > 0;
        public static void Open() => openCount++;
        public static void Close()
        {
            if (openCount > 0) openCount--;
        }
        public static void Reset() => openCount = 0;
    }
}
