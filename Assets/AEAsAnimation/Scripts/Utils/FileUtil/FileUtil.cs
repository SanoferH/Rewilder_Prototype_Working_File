namespace AEAsAnimation
{
    public class FileUtil
    {
        private static FileUtil _instance = null;
        public static FileUtil Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FileUtil();

                return _instance;
            }
        }
        private FileUtil()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            _loader = new FileLoaderStandard();
            _accessor = new FileAccessorStandard();
#elif UNITY_IOS && !UNITY_EDITOR
            _loader = new FileLoaderStandard();
            _accessor = new FileAccessorStandard();
#elif UNITY_WEBGL && !UNITY_EDITOR
            _loader = new FileLoaderStandard();
            _accessor = new FileAccessorStandard();
#else
            _loader = new FileLoaderStandard();
            _accessor = new FileAccessorStandard();
#endif
        }

        
        private FileLoader _loader = null;
        private FileAccessor _accessor = null;


        public void Initialize()
        {
        }

        public void Tick()
        {
            _loader?.Tick();
            _accessor?.Tick();
        }


        public FileLoader loader
        {
            get
            {
                return _loader;
            }
        }

        public FileAccessor accessor
        {
            get
            {
                return _accessor;
            }
        }
    }
}

