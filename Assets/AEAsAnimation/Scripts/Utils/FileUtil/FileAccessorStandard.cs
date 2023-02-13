using System.Collections.Generic;

namespace AEAsAnimation
{
    public class FileAccessorStandard : FileAccessor
    {
        override public bool Exists(string path)
        {
            return System.IO.File.Exists(path);
        }

        override public List<string> GetFilePathsInDirectory(string directoryPath, string fileName)
        {
            var result = new List<string>();

            var directory = new System.IO.DirectoryInfo(directoryPath);
            foreach (var file in directory.GetFiles())
            {
                if (file.Name == fileName) result.Add(ConvertBackSlash(directoryPath + "/" + fileName));
            }

            foreach (var subDirectory in directory.GetDirectories())
            {
                result.AddRange(GetFilePathsInDirectory(subDirectory.ToString(), fileName));
            }

            return result;
        }
    }
}

