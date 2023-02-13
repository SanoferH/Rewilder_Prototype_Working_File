using System.Collections.Generic;

namespace AEAsAnimation
{
    public class FileAccessor
    {
        virtual public void Tick()
        {

        }


        virtual public bool Exists(string path)
        {
            return false;
        }

        virtual public List<string> GetFilePathsInDirectory(string directoryPath, string fileName)
        {
            var result = new List<string>
            {
                directoryPath + "/" + fileName
            };

            return result;
        }

        virtual public string GetFileName(string filePath)
        {
            var sections = filePath.Split("/".ToCharArray());
            return sections[sections.Length - 1];
        }

        virtual public string FindFilePath(string filePath)
        {
            var sections = filePath.Split("/".ToCharArray());
            var fileName = sections[sections.Length - 1];
            var directoryPath = filePath.Substring(0, filePath.LastIndexOf("/"));

            var paths = GetFilePathsInDirectory(directoryPath, fileName);

            return paths.Count > 0 ? paths[0] : "";
        }

        protected string ConvertBackSlash(string raw)
        {
            return raw.Replace("\\".ToCharArray()[0], "/".ToCharArray()[0]);
        }
    }
}

