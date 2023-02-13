using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AEAsAnimation
{
    public class AEAsAnimationLoader : MonoBehaviour
    {
        void Start()
        {
            GameObject.DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            FileUtil.Instance.loader.Tick();
        }

        private static AEAsAnimationLoader _instance = null;
        public static AEAsAnimationLoader Initialize()
        {
            if (_instance != null) return _instance;

            var root = new GameObject("AEAsAnimationLoader");
            var loader = root.AddComponent<AEAsAnimationLoader>();

            var pool = new GameObject("pool");
            pool.transform.parent = root.transform;

            FileUtil.Instance.Initialize();
            AEAsAnimationUtils.pool.SetUp(pool.transform);

            return _instance = loader;
        } 
    }
}