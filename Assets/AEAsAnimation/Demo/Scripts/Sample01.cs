using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AEAsAnimation;

public class Sample01 : MonoBehaviour
{
    private string FilePath = "AEAsAnimation/Samples/Treasure/motion_tre_001.json";

    private void Start()
    {
        AEAsAnimationRoot
            .Attach(transform)
            .Show(FilePath);
    }
}
