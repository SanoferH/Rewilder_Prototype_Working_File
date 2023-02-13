#if UNITY_EDITOR

using System.IO;

using UnityEngine;
using UnityEditor;

using UnityEditor.Callbacks;

public class SampleInitializer
{
    [OnOpenAssetAttribute(1)]
    public static bool step1(int instanceID, int line)
    {
        string name = EditorUtility.InstanceIDToObject(instanceID).name;
        if (name == "SampleScene01"
            || name == "SampleScene02"
            || name == "SampleScene03"
            || name == "SampleScene04"
            )
        {
            var directoryPath = Application.streamingAssetsPath + "/AEAsAnimation/Samples/";
            if (!Directory.Exists(directoryPath))
            {
                var sourcePath = Application.dataPath + "/AEAsAnimation/Demo/Streaming/Samples/";
                if (Directory.Exists(sourcePath))
                {
                    System.Action<string, string> Copy = null;
                    Copy = (from, to) => {
                        var directoryInfo = new DirectoryInfo(from);

                        Directory.CreateDirectory(to);

                        var src_folders = directoryInfo.GetDirectories();
                        var src_files = directoryInfo.GetFiles();

                        foreach (FileInfo file in src_files)
                        {
                            if (file.Name.Contains(".meta")) continue;
                            string path = Path.Combine(to, file.Name);
                            file.CopyTo(path, true);
                        }
                        foreach (DirectoryInfo subfolder in src_folders)
                        {
                            string path = Path.Combine(to, subfolder.Name);
                            Copy(subfolder.FullName, path);
                        }
                    };
                    Copy(sourcePath, directoryPath);
                }
            }
        }
        return false; // we did not handle the open
    }
}
#endif