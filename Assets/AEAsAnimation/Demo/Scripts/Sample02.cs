using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AEAsAnimation;

public class Sample02 : MonoBehaviour
{
    private string FilePath = "AEAsAnimation/Samples/Arrow/wana_00.json";

    private void Preload(System.Action callback)
    {
        AEAsAnimationRoot.Preload(
                FilePath,
                () => {
                    callback();
                }
            );
    }

    private void CreateAnimation()
    {
        AEAsAnimationRoot
            .Attach(transform)
            .Show(FilePath, loop:true);
    }


    private void Start()
    {
        Preload(() =>
        {
            CreateAnimation();
        });
    }
}
