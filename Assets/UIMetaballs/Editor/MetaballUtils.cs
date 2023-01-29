using UIMetaballs.Runtime;
using UnityEngine;

namespace UIMetaballs
{
    public class MetaballUtils : UnityEditor.AssetModificationProcessor
    {
        // This is used to call initialize on metaballs after 0.1 seconds, so they don't disappear
        static string[] OnWillSaveAssets(string[] paths)
        {
            var panels = (MetaballPanel[])Object.FindObjectsOfType(typeof(MetaballPanel));

            foreach (var p in panels)
            {
                p.StartInitializationAfterSave();
            }
            
            return paths;
        }
    }
}