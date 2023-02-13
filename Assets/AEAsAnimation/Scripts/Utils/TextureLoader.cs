using System.Collections.Generic;

using UnityEngine;

namespace AEAsAnimation
{
    public class TextureLoader
    {
        private static TextureLoader _instance = null;
        public static TextureLoader Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TextureLoader();

                return _instance;
            }
        }
        private TextureLoader() { }


        private static Dictionary<string, Sprite> _cache = new Dictionary<string, Sprite>();

        public void GetSprite(
            string filePath,
            System.Action<Sprite> callback)
        {
            if (_cache.ContainsKey(filePath))
            {
                callback(_cache[filePath]);
                return;
            }
            GetTexture(
                filePath,
                texture => {
                    var sprite = Sprite.Create(
                        texture,
                        new Rect(0, 0, texture.width, texture.height),
                        Vector2.one / 2);
                    if (!_cache.ContainsKey(filePath))
                        _cache.Add(filePath, sprite);
                    callback(sprite);
                });
        }
        
        private void GetTexture(
            string filePath,
            System.Action<Texture2D> callback)
        {
            FileUtil.Instance.loader.LoadBinary(filePath, bytes => {
                long pos = 0;

                if (bytes.Length == 0) Debug.LogError(filePath + " not found.");

                byte[] fileSignature = new byte[8];
                for (int i = 0; i < 8; i++, pos++)
                {
                    fileSignature[i] = bytes[pos];
                }

                byte[] chunkSize = new byte[4];
                byte[] chunkType = new byte[4];
                byte[] imageWidth = new byte[4];
                byte[] imageHeight = new byte[4];

                if (System.BitConverter.IsLittleEndian)
                {
                    for (int i = 4; i > 0; i--, pos++)
                    {
                        chunkSize[i - 1] = bytes[pos];
                    }
                    for (int i = 4; i > 0; i--, pos++)
                    {
                        chunkType[i - 1] = bytes[pos];
                    }
                    for (int i = 4; i > 0; i--, pos++)
                    {
                        imageWidth[i - 1] = bytes[pos];
                    }
                    for (int i = 4; i > 0; i--, pos++)
                    {
                        imageHeight[i - 1] = bytes[pos];
                    }
                }
                else
                {
                    for (int i = 0; i < 4; i++, pos++)
                    {
                        chunkSize[i] = bytes[pos];
                    }
                    for (int i = 0; i < 4; i++, pos++)
                    {
                        chunkType[i] = bytes[pos];
                    }
                    for (int i = 0; i < 4; i++, pos++)
                    {
                        imageWidth[i] = bytes[pos];
                    }
                    for (int i = 0; i < 4; i++, pos++)
                    {
                        imageHeight[i] = bytes[pos];
                    }
                }

                int width = System.BitConverter.ToInt32(imageWidth, 0);
                int height = System.BitConverter.ToInt32(imageHeight, 0);

                Texture2D texture = new Texture2D(width, height);
                texture.LoadImage(bytes);
                callback(texture);
            });
        }
    }
}
