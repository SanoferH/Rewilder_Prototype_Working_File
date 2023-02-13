using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AEAsAnimation
{
    public class AEAsAnimationRoot : MonoBehaviour
    {
        public event UnityEngine.Events.UnityAction<string> OnClick;

        private AEAsAnimationRootData _data = null;
        private Dictionary<AEComposition, GameObject> _compositionRoots = new Dictionary<AEComposition, GameObject>();
        private Dictionary<AELayer, GameObject> _layerRoots = new Dictionary<AELayer, GameObject>();
        private Dictionary<AELayer, AELayerReflection> _layerReflectionMap = new Dictionary<AELayer, AELayerReflection>();

        private AEComposition _rootNode = null;

        private GlobalColorData _globalColor = null;

        public class SwapImageData
        {
            public string from;
            public string to;
            public string directory = "";
            public bool overrideSize = false;
        }
        private List<SwapImageData> _swapImages = new List<SwapImageData>();
        private bool _asLayout = false;

        public static AEAsAnimationRoot Attach(
            Transform parent = null,
            string name = "AEAsAnimationRoot",
            bool asLayout = false,
            bool useGlobalColor = false
        ) {
            AEAsAnimationLoader.Initialize();

            var root = new GameObject(name);
            root.transform.parent = parent;
            var rectTransform = root.AddComponent<RectTransform>();
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localScale = Vector3.one;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            var aeRoot = root.AddComponent<AEAsAnimationRoot>();
            if (asLayout) aeRoot.SetAsLayout();
            if (useGlobalColor) aeRoot.SetGlobalColor();
            return aeRoot;
        }

        private string _swapImageRootDirectory = "";
        public AEAsAnimationRoot AddSwapImages(
          string from,
          string to,
          bool overrideSize = false,
          string directory = ""
        )
        {
            _swapImages.Add(new SwapImageData
            {
                from = from,
                to = to,
                directory = directory != "" ? directory : _swapImageRootDirectory,
                overrideSize = overrideSize
            });
            return this;
        }

        public AEAsAnimationRoot SetSwapImagesRootDirectory(
          string directory
        )
        {
            _swapImageRootDirectory = directory;
            foreach(var swapData in _swapImages)
            {
                swapData.directory = directory;
            }
            return this;
        }


        public AEAsAnimationRoot SetAsLayout()
        {
            _asLayout = true;
            return this;
        }

        public AEAsAnimationRoot SetGlobalColor(
            Color overlay = default(Color),
            Color multiply = default(Color),
            float opacity = 1)
        {
            var multiply_ = multiply != default(Color)
                ? multiply
                : new Color(1, 1, 1, 1);
            _globalColor = new GlobalColorData
            {
                overlayR = overlay.r,
                overlayG = overlay.g,
                overlayB = overlay.b,
                overlayA = overlay.a,
                multiplyR = multiply_.r,
                multiplyG = multiply_.g,
                multiplyB = multiply_.b,
                multiplyA = multiply_.a,
                opacity = opacity
            };
            return this;
        }

        public AEAsAnimationRoot Show(
            string dataPath,
            System.Action<AEAsAnimationRoot> OnReady = null,
            System.Action<AEAsAnimationRoot> OnFailure = null,
            bool loop = false)
        {
            System.Func<string> GetDirectory = () => {
                return AEAsAnimationUtils.GetFileDirectory(dataPath);
            };

            AEAsAnimationUtils.LoadData(dataPath, data => {
                _data = data;

                var mainComposition = GetComposition(data.mainCompositionId);
                if (mainComposition == null)
                {
                    if (OnFailure != null) OnFailure(this);
                    return;
                }

                var rootNode = _rootNode = AEComposition.Attach(
                    gameObject.transform,
                    mainComposition,
                    new AEItemsAccessor
                    {
                        GetComposition = GetComposition,
                        GetLayer = GetLayer,
                        GetMetaData = () => {
                            _data.metaData.asLayout = _asLayout;
                            return _data.metaData;
                        },
                        GetFileDirectory = GetDirectory,
                        GetSwapImageMap = () => {
                            return _swapImages;
                        },
                        GetSwapImage = from => {
                            foreach (var swapData in _swapImages)
                            {
                                if (swapData.from != from) continue;
                                return swapData;
                            }
                            return null;
                        },
                        OnClick = metaData => {
                            if (OnClick != null) OnClick(metaData);
                        },
                        RegisterComposition = (composition, root) => {
                            _compositionRoots.Add(composition, root);
                        },
                        RegisterLayer = (layer, root) => {
                            _layerRoots.Add(layer, root);
                        },
                        RegisterLayerReflection = (layer, reflection) => {
                            _layerReflectionMap.Add(layer, reflection);
                        },
                        GetGlobalColorData = () =>
                        {
                            return _globalColor;
                        },
                    });

                var rectTransform = rootNode.gameObject.GetComponent<RectTransform>();
                rectTransform.localPosition = new Vector3(
                        -1 * mainComposition.width / 2,
                        mainComposition.height / 2,
                        0
                    );

                if (loop) rootNode.GoToStart(true);

                if (OnReady != null) OnReady(this);
            });

            return this;
        }

        public IEnumerator ShowAsync(
            string dataPath,
            System.Action<AEAsAnimationRoot> OnReady = null,
            System.Action<AEAsAnimationRoot> OnFailure = null,
            bool loop = false)
        {
            System.Func<string> GetDirectory = () =>
            {
                return AEAsAnimationUtils.GetFileDirectory(dataPath);
            };

            bool isLoaded = false;
            AEAsAnimationUtils.LoadData(dataPath, data =>
            {
                isLoaded = true;
                _data = data;
            });

            while(!isLoaded)
            {
                yield return null;
            }

            var mainComposition = GetComposition(_data.mainCompositionId);
            if (mainComposition == null)
            {
                if (OnFailure != null) OnFailure(this);
            }
            else
            {
                yield return AEComposition.AttachAsync(
                    gameObject.transform,
                    mainComposition,
                    new AEItemsAccessor
                    {
                        GetComposition = GetComposition,
                        GetLayer = GetLayer,
                        GetMetaData = () =>
                        {
                            _data.metaData.asLayout = _asLayout;
                            return _data.metaData;
                        },
                        GetFileDirectory = GetDirectory,
                        GetSwapImageMap = () =>
                        {
                            return _swapImages;
                        },
                        GetSwapImage = from => {
                            foreach (var swapData in _swapImages)
                            {
                                if (swapData.from != from) continue;
                                return swapData;
                            }
                            return null;
                        },
                        OnClick = metaData =>
                        {
                            if (OnClick != null) OnClick(metaData);
                        },
                        RegisterComposition = (composition, root) =>
                        {
                            _compositionRoots.Add(composition, root);
                        },
                        RegisterLayer = (layer, root) =>
                        {
                            _layerRoots.Add(layer, root);
                        },
                        RegisterLayerReflection = (layer, reflection) =>
                        {
                            _layerReflectionMap.Add(layer, reflection);
                        },
                        GetGlobalColorData = () =>
                        {
                            return _globalColor;
                        },
                    },
                    rootNode =>
                    {
                        _rootNode = rootNode;

                        var rectTransform = rootNode.gameObject.GetComponent<RectTransform>();
                        rectTransform.localPosition = new Vector3(
                                -1 * mainComposition.width / 2,
                                mainComposition.height / 2,
                                0
                            );

                        if (loop) rootNode.GoToStart(true);

                        if (OnReady != null) OnReady(this);
                    });
            }

            yield return null;
        }


        public int GoToMarker(string markerTag, bool loop = false)
        {
            return _rootNode.GoToMarker(markerTag, loop);
        }

        public int GoToStart(bool loop = false, int lastOffset = 0)
        {
            return _rootNode.GoToStart(loop, lastOffset);
        }

        public void KeepFrame(int frame)
        {
            _rootNode.KeepFrame(frame);
        }

        public void SetLoop()
        {
            _rootNode.SetLoop();
        }

        public void SetLoop(
            int from,
            int to)
        {
            _rootNode.SetLoop(from, to);
        }

        public void Restart()
        {
            _rootNode.Restart();
        }

        public void StopAnimation()
        {
            _rootNode.StopAnimation();
        }

        public GameObject GetCompositionGameObjectByName(string name)
        {
            foreach (var key in _compositionRoots.Keys)
            {
                if (key.name == name)
                {
                    return _compositionRoots[key];
                }
            }
            return null;
        }

        public GameObject GetLayerGameObjectByName(
            string name,
            bool activeOnly = true,
            int index = 0)
        {
            var resuls = new List<GameObject>();

            foreach (var key in _layerReflectionMap.Keys)
            {
                if (key.data.name == name)
                {
                    var result = _layerReflectionMap[key].gameObject;
                    if (activeOnly && !key.isInTime) continue;
                    resuls.Add(result);
                }
            }

            return resuls.Count > index ? resuls[index] : null;
        }

        public void RegisterLayerObserver(
            string name,
            System.Action<AELayer> callback,
            bool activeOnly = true,
            int index = 0)
        {
            var target = GetLayerByName(name, activeOnly, index);
            if (target == null) return;
            target.OnUpdated += layer => {
                callback(layer);
            };
        }

        public AELayer GetLayerByName(
            string name,
            bool activeOnly = true,
            int index = 0)
        {
            var resuls = new List<AELayer>();

            foreach (var key in _layerReflectionMap.Keys)
            {
                if (key.data.name == name)
                {
                    if (activeOnly && !key.isInTime) continue;
                    resuls.Add(key);
                }
            }

            return resuls.Count > index ? resuls[index] : null;
        }

        public GameObject GetImageNodeByName(
            string name,
            bool activeOnly = true,
            int index = 0)
        {
            var resuls = new List<GameObject>();

            foreach (var pair in _layerReflectionMap)
            {
                if (pair.Key.data.name == name)
                {
                    if (activeOnly && !pair.Key.isInTime) continue;
                    resuls.Add(pair.Value.gameObject);
                }
            }

            return resuls.Count > index ? resuls[index] : null;
        }

        public List<GameObject> GetAllLayerGameObjects(
            string name,
            bool activeOnly = false)
        {
            var resuls = new List<GameObject>();

            foreach (var key in _layerReflectionMap.Keys)
            {
                if (key.data.name == name)
                {
                    var result = _layerReflectionMap[key].gameObject;
                    if (activeOnly && !key.isInTime) continue;
                    resuls.Add(result);
                }
            }

            return resuls;
        }

        public GameObject GetLayerGameObjectByImageFile(string imageFile)
        {
            foreach (var key in _layerReflectionMap.Keys)
            {
                if (key.imageFile == imageFile)
                {
                    return _layerReflectionMap[key].gameObject;
                }
            }

            return null;
        }

        public void SwapImage(
            string from,
            string to
            )
        {
            foreach (var key in _layerReflectionMap.Keys)
            {
                if (key.imageFile != from) continue;

                var reflection = _layerReflectionMap[key];
                reflection.SwapImage(to);
            }
        }

        public float fps
        {
            get
            {
                return _data.metaData.fps;
            }
        }

        public int totalFrame
        {
            get
            {
                return _rootNode.totalFrame;
            }
        }


        public static void Preload(
            string dataPath,
            System.Action OnReady
        ) {
            AEAsAnimationUtils.Preload(dataPath, OnReady);
        }


        public AEAsAnimationRootData animationData
        {
            get {
                return _data;
            }
        }

        public string markerInfromations
        {
            get
            {
                return _rootNode.markerInfromations;
            }
        }

        public List<string> allMarkers
        {
            get
            {
                return _rootNode.allMarkers;
            }
        }

        public List<string> allImageFiles
        {
            get
            {
                return _rootNode.allImageFiles;
            }
        }

        public List<string> allLayerMetaData
        {
            get
            {
                return _rootNode.allLayerMetaData;
            }
        }

        public List<string> allLayerNames
        {
            get
            {
                return _rootNode.allLayerNames;
            }
        }

        // ------------------------------------- private

        private CompositionData GetComposition(int id)
        {
            foreach(var composition in _data.allCompositions)
            {
                if (composition.id == id) return composition;
            }
            return null;
        }

        private LayerData GetLayer(string id)
        {
            foreach(var layer in _data.allLayers)
            {
                if (layer.id == id) return layer;
            }
            return null;
        }
    }

    public class AEItemsAccessor {
        public System.Func<int, CompositionData> GetComposition;
        public System.Func<string, LayerData> GetLayer;
        public System.Func<MetaData> GetMetaData;
        public System.Func<string> GetFileDirectory;
        public System.Func<List<AEAsAnimationRoot.SwapImageData>> GetSwapImageMap;
        public System.Func<string, AEAsAnimationRoot.SwapImageData> GetSwapImage;
        public System.Action<string> OnClick;
        public System.Action<AEComposition, GameObject> RegisterComposition;
        public System.Action<AELayer, GameObject> RegisterLayer;
        public System.Action<AELayer, AELayerReflection> RegisterLayerReflection;
        public System.Func<GlobalColorData> GetGlobalColorData;
    }

    public class AEAsAnimationRootData
    {
        public int mainCompositionId;
        public List<CompositionData> allCompositions = null;
        public List<LayerData> allLayers = null;
        public MetaData metaData;

        public static Dictionary<string, AEAsAnimationRootData> Cache = new Dictionary<string, AEAsAnimationRootData>();
    }
}