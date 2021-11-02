#if UNITY_EDITOR
using UnityEditor;

namespace FrameWork
{
    //[InitializeOnLoad]
    public class SheetDownloadConfigCore
    {
        private static SheetDownloadConfig _config;

        static SheetDownloadConfigCore()
        {
            _config = SheetDownloadConfig.Instance;
        }
    }
}
#endif