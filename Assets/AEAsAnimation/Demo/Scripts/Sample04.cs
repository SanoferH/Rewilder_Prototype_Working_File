using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AEAsAnimation;

public class Sample04 : MonoBehaviour
{
    private string FilePath = "AEAsAnimation/Samples/Button/layout.json";

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
        var animationRoot = AEAsAnimationRoot
            .Attach(transform)
            .Show(FilePath);

        animationRoot.OnClick += tag => {
            Debug.Log(tag + " clicked.");
        };
    }


    private void Start()
    {
        Preload(() =>
        {
            CreateAnimation();
        });
    }
}
