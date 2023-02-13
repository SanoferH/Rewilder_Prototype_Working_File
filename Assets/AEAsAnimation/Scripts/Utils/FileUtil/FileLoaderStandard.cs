using System.Collections.Generic;

using UnityEngine;

namespace AEAsAnimation
{
    public class FileLoaderStandard : FileLoader
    {
        public FileLoaderStandard()
        {
            _storagePathMap = new Dictionary<StorageType, string>
            {
                {StorageType.None,  Application.dataPath},
                {StorageType.Persistent,  Application.persistentDataPath},
                {StorageType.StreamingAssets,  Application.streamingAssetsPath},
                {StorageType.Temporary,  Application.temporaryCachePath},
            };
        }
    }
}