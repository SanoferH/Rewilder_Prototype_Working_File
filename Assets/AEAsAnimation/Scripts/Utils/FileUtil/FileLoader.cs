using System.Collections.Generic;

namespace AEAsAnimation
{
    public class FileLoader
    {
        public enum StorageType
        {
            None = 0,
            StreamingAssets,
            Persistent,
            Temporary
        }

        protected StorageType _readStorageType = StorageType.StreamingAssets;
        protected StorageType _writeStorageType = StorageType.Persistent;

        public void SetStorageType(StorageType type)
        {
            _readStorageType = _writeStorageType = type;
        }

        public void SetStorageType(StorageType read, StorageType write)
        {
            _readStorageType = read;
            _writeStorageType = write;
        }

        protected Dictionary<StorageType, string> _storagePathMap = null;
        virtual public string readStoragePath
        {
            get
            {
                return _storagePathMap[_readStorageType];
            }
        }
        virtual public string writeStoragePath
        {
            get
            {
                return _storagePathMap[_writeStorageType];
            }
        }

        protected class Filer
        {
            public string path;
            public System.Action callback;
            public System.Func<bool> IsFinished;
        }
        protected Queue<Filer> _loadingFileQueue = new Queue<Filer>();


        virtual public void Tick()
        {
            if (_loadingFileQueue.Count > 0)
            {
                if (_loadingFileQueue.Peek().IsFinished())
                {
                    _loadingFileQueue.Dequeue().callback();
                }
            }
        }

        virtual public void LoadText(
            string path,
            System.Action<string> callback)
        {
            LoadTextRaw(readStoragePath + "/" + path, callback);
        }

        virtual public void LoadBinary(
            string path,
            System.Action<byte[]> callback)
        {
            LoadBinaryRaw(readStoragePath + "/" + path, callback);
        }

        virtual public void SaveText(
            string path,
            string text)
        {
            var writer = new System.IO.StreamWriter(writeStoragePath + "/" + path);
            writer.Write(text);
            writer.Close();
        }

        virtual public void SaveBinary(
            string path,
            byte[] binary)
        {
            using (var fileStream = new System.IO.FileStream(writeStoragePath + "/" + path, System.IO.FileMode.Create))
            {
                var writer = new System.IO.BinaryWriter(fileStream);
                writer.Write(binary);
                writer.Close();
            }
        }




        protected void LoadTextRaw(
            string fullPath,
            System.Action<string> callback)
        {
            var request = UnityEngine.Networking.UnityWebRequest.Get(fullPath);
            _loadingFileQueue.Enqueue(new Filer
            {
                path = fullPath,
                callback = () =>
                {
                    callback(request.downloadHandler.text);
                },
                IsFinished = () =>
                {
                    return request.isDone;
                }
            });
            request.SendWebRequest();
        }

        protected void LoadBinaryRaw(
            string fullPath,
            System.Action<byte[]> callback)
        {
            var request = UnityEngine.Networking.UnityWebRequest.Get(fullPath);
            _loadingFileQueue.Enqueue(new Filer
            {
                path = fullPath,
                callback = () =>
                {
                    callback(request.downloadHandler.data);
                },
                IsFinished = () =>
                {
                    return request.isDone;
                }
            });
            request.SendWebRequest();
        }


        protected void CopyStreamingAssetsFileToParsistant(
            string path,
            System.Action<byte[]> callback)
        {

            var request = UnityEngine.Networking.UnityWebRequest.Get(_storagePathMap[StorageType.StreamingAssets] + "/" + path);
            request.downloadHandler = new UnityEngine.Networking.DownloadHandlerFile(_storagePathMap[StorageType.Persistent] + "/" + path);
            _loadingFileQueue.Enqueue(new Filer
            {
                path = path,
                callback = () =>
                {
                    callback(request.downloadHandler.data);
                },
                IsFinished = () =>
                {
                    return request.isDone;
                }
            });
            request.SendWebRequest();
        }

        protected void CopyStreamingAssetsTextToParsistant(
            string path,
            System.Action<string> callback)
        {

            var request = UnityEngine.Networking.UnityWebRequest.Get(_storagePathMap[StorageType.StreamingAssets] + "/" + path);
            request.downloadHandler = new UnityEngine.Networking.DownloadHandlerFile(_storagePathMap[StorageType.Persistent] + "/" + path);
            _loadingFileQueue.Enqueue(new Filer
            {
                path = path,
                callback = () =>
                {
                    callback(request.downloadHandler.text);
                },
                IsFinished = () =>
                {
                    return request.isDone;
                }
            });
            request.SendWebRequest();
        }
    }
}

