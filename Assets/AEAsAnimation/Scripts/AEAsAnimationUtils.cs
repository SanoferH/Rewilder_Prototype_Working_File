using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AEAsAnimation
{
    public class AEAsAnimationUtils
    {
        public static string Version = "v1.0.0";

        public static bool ShowVersionStringLog = false;

        public static AEAsAnimationPool pool = new AEAsAnimationPool();


        public static string GetFileDirectory(string dataPath)
        {
            int lastIndex = dataPath.LastIndexOf("/");
            string fileDirectory = lastIndex > 0 ? dataPath.Substring(0, lastIndex + 1) : "";
            return fileDirectory;
        }

        public static void Preload(
            string dataPath,
            System.Action OnReady
        )
        {
            if (AEAsAnimationRootData.Cache.ContainsKey(dataPath))
            {
                OnReady();
                return;
            }

            AEAsAnimationLoader.Initialize();

            AEAsAnimationUtils.LoadData(dataPath, data => {
                var imageList = new List<string>();

                foreach (var layer in data.allLayers)
                {
                    if (layer.footage == null) continue;
                    var fileFootage = layer.footage as FileSourceFootageData;
                    if (fileFootage == null) continue;
                    imageList.Add(fileFootage.sourceName);
                }

                for (int i = 0; i < imageList.Count; i++)
                {
                    System.Action end = () => { };
                    if (i == imageList.Count - 1)
                    {
                        end = () => {
                            OnReady();
                        };
                    }

                    var filePath = AEAsAnimationUtils.GetFileDirectory(dataPath) + imageList[i];
                    TextureLoader.Instance.GetSprite(
                        filePath,
                        sprite => {
                            end();
                        });
                }
            });
        }


        public static void LoadData(
            string dataPath,
            System.Action<AEAsAnimationRootData> OnReady
        )
        {
            if (AEAsAnimationRootData.Cache.ContainsKey(dataPath))
            {
                OnReady(AEAsAnimationRootData.Cache[dataPath]);
                return;
            }
            FileUtil.Instance.loader.LoadText(dataPath, dataString =>
            {
                BootUpFromJsonString(dataPath, dataString, OnReady);
            });
        }

        public static void BootUpFromJsonString(
            string dataPath,
            string dataString,
            System.Action<AEAsAnimationRootData> OnReady
        )
        {
            var raw = MiniJSON.Json.Deserialize(dataString);
            var rootData = (IList)raw;
            int mainCompositionId = System.Convert.ToInt32(rootData[0]);
            var compositions = (IList)(rootData[1]);
            var layers = (IList)(rootData[2]);
            var metaData = new MetaData
            {
                path = dataPath,
                fps = float.Parse((string)rootData[3]),
                versionString = (string)(rootData[4])
            };

            if (AEAsAnimationUtils.ShowVersionStringLog && metaData.versionString != AEAsAnimationUtils.Version)
            {
                Debug.LogError("Loaded AEAsAnimation file's version was not match.\n " + "file : " + metaData.versionString + ", loader : " + AEAsAnimationUtils.Version);
            }
            var bakedData = new AEAsAnimationRootData
            {
                mainCompositionId = mainCompositionId,
                allCompositions = ParseCompositions(compositions),
                allLayers = ParseLayers(layers),
                metaData = metaData
            };
            AEAsAnimationRootData.Cache.Add(dataPath, bakedData);
            OnReady(bakedData);
        }


        private static List<CompositionData> ParseCompositions(IList compositions)
        {
            var allCompositions = new List<CompositionData>();

            foreach (var composition in compositions)
            {
                var dataList = (IList)composition;
                int dataListIndex = 0;
                var id = System.Convert.ToInt32(dataList[dataListIndex++]);
                var width = System.Convert.ToSingle(dataList[dataListIndex++]);
                var height = System.Convert.ToSingle(dataList[dataListIndex++]);
                var layerIdsRaw = (IList)dataList[dataListIndex++];
                var layerIds = new List<string>();
                for (var i = layerIdsRaw.Count - 1; i >= 0; i--) layerIds.Add(System.Convert.ToString(layerIdsRaw[i]));
                var metadata = System.Convert.ToString(dataList[dataListIndex++]);

                var markerDataList = (IList)dataList[dataListIndex++];
                AEAsAnimationLayerPropertyData<string> markers = null;
                if (markerDataList.Count > 0)
                {
                    markers = new AEAsAnimationLayerPropertyData<string>().Parse((IList)markerDataList[0]);
                }

                allCompositions.Add(new CompositionData
                {
                    id = id,
                    width = width,
                    height = height,
                    layerIds = layerIds,
                    metaData = metadata,
                    markers = markers
                });
            }

            return allCompositions;
        }


        private static AVLayerPropertiesData GetAVLayerPropertiesData(IList propertiesDataList)
        {
            if (propertiesDataList == null
                || propertiesDataList.Count == 0)
            {
                return AVLayerPropertiesData.Empty();
            }

            int dataListIndex = 0;
            var transformsDataList = (IList)propertiesDataList[dataListIndex++];
            var stylesDataList = (IList)propertiesDataList[dataListIndex++];
            var masksDataList = (IList)propertiesDataList[dataListIndex++];

            int transformsListIndex = 0;

            AVLayerTransformsData transforms = new AVLayerTransformsData
            {
                position = new AEAsAnimationLayerPropertyData<float>().Parse((IList)transformsDataList[transformsListIndex++]),
                rotation = new AEAsAnimationLayerPropertyData<float>().Parse((IList)transformsDataList[transformsListIndex++]),
                scale = new AEAsAnimationLayerPropertyData<float>().Parse((IList)transformsDataList[transformsListIndex++]),
                anchor = new AEAsAnimationLayerPropertyData<float>().Parse((IList)transformsDataList[transformsListIndex++]),
                opacity = new AEAsAnimationLayerPropertyData<float>().Parse((IList)transformsDataList[transformsListIndex++])
            };

            int layerStyleListIndex = 0;
            AVLayerColorOverlaysData colorOverlays = stylesDataList.Count > 0
                ? new AVLayerColorOverlaysData
                {
                    color = new AEAsAnimationLayerPropertyData<float>().Parse((IList)stylesDataList[layerStyleListIndex++]),
                    opacity = new AEAsAnimationLayerPropertyData<float>().Parse((IList)stylesDataList[layerStyleListIndex++])
                }
                : null;

            AVLayerMaskData masks = null;
            if (masksDataList.Count > 0)
            {
                int maskDataListIndex = 0;
                var maskShapeDataList = (IList)masksDataList[maskDataListIndex++];
                bool maskInverted = (bool)masksDataList[maskDataListIndex++];

                int maskShapeDataListIndex = 0;
                var shapeOpacity = new AEAsAnimationLayerPropertyData<float>().Parse((IList)maskShapeDataList[maskShapeDataListIndex++]);
                var shapeOffset = new AEAsAnimationLayerPropertyData<float>().Parse((IList)maskShapeDataList[maskShapeDataListIndex++]);
                var shapePoints = (IList)maskShapeDataList[maskShapeDataListIndex++];

                masks = new AVLayerMaskData
                {
                    opacity = shapeOpacity,
                    offset = shapeOffset,
                    shape = new AEAsAnimationLayerShapePropertyData().Parse(shapePoints),
                    inverted = maskInverted
                };
            }

            AVLayerMarkerData markers = null;

            return new AVLayerPropertiesData
            {
                transforms = transforms,
                colorOverlays = colorOverlays,
                masks = masks,
                markers = markers,
            };
        }

        private static List<LayerData> ParseLayers(IList layers)
        {
            var allLayers = new List<LayerData>();

            foreach (var layer in layers)
            {
                var dataList = (IList)layer;
                var id = System.Convert.ToString(dataList[0]);
                var uniqueId = System.Convert.ToString(dataList[1]);


                var footageSourceName = "";
                var footageSourceType = "";
                List<float> footageColor = null;
                var footageDataList = (IList)dataList[2];
                if (footageDataList != null)
                {
                    footageSourceName = System.Convert.ToString(footageDataList[0]);
                    footageSourceType = System.Convert.ToString(footageDataList[1]);

                    var footageColorRaw = (IList)footageDataList[2];
                    if (footageColorRaw != null)
                    {
                        footageColor = new List<float>();
                        foreach (var colorElement in footageColorRaw) footageColor.Add(System.Convert.ToSingle(colorElement));
                    }
                }

                var compositionId = System.Convert.ToInt32(dataList[3]);
                var propertiesDataList = (IList)dataList[4];
                var parentCompositionId = System.Convert.ToInt32(dataList[5]);

                var layerOptions = (IList)dataList[6];
                int layerOptionIndex = 0;
                var layerIndex = System.Convert.ToInt32(layerOptions[layerOptionIndex++]);
                var parentIndex = System.Convert.ToInt32(layerOptions[layerOptionIndex++]);
                var layerName = System.Convert.ToString(layerOptions[layerOptionIndex++]);
                var layerSize = (IList)layerOptions[layerOptionIndex++];
                var width = System.Convert.ToSingle(layerSize[0]); ;
                var height = System.Convert.ToSingle(layerSize[1]); ;
                var blendMode = System.Convert.ToInt32(layerOptions[layerOptionIndex++]);
                var comment = System.Convert.ToString(layerOptions[layerOptionIndex++]);
                var inSeconds = System.Convert.ToSingle(layerOptions[layerOptionIndex++]);
                var outSeconds = System.Convert.ToSingle(layerOptions[layerOptionIndex++]);
                var isNullLayer = System.Convert.ToBoolean(layerOptions[layerOptionIndex++]);


                FootageData footage = null;
                if (footageSourceType == "SolidSource")
                {
                    footage = new SolidSourceFootageData
                    {
                        sourceName = footageSourceName,
                        color = footageColor
                    };
                }
                else if (footageSourceType == "FileSource")
                {
                    footage = new FileSourceFootageData
                    {
                        sourceName = footageSourceName
                    };
                }


                allLayers.Add(new LayerData
                {
                    id = id,
                    uniqueId = uniqueId,
                    footage = footage,
                    compositionId = compositionId,
                    properties = GetAVLayerPropertiesData(propertiesDataList),
                    parentCompositionId = parentCompositionId,

                    name = layerName,
                    width = width,
                    height = height,
                    layerIndex = layerIndex,
                    parentIndex = parentIndex,
                    metaData = comment,
                    blendMode = (AVLayerBlendingMode)blendMode,
                    inSeconds = inSeconds,
                    outSeconds = outSeconds,
                    isNullLayer = isNullLayer,
                });
            }

            return allLayers;
        }
    }

    public class AEAsAnimationPool
    {
        private Dictionary<string, AEAsAnimationRoot> _pool = new Dictionary<string, AEAsAnimationRoot>();
        private Transform _poolRoot = null;

        public void SetUp(Transform poolRoot)
        {
            _poolRoot = poolRoot;
        }
        
        public void Push(string tag, AEAsAnimationRoot root)
        {
            if (_poolRoot == null) return;
            if (_pool.ContainsKey(tag)) return;

            root.transform.parent = _poolRoot;

            _pool.Add(tag, root);
        }

        public AEAsAnimationRoot Pop(string tag, Transform parent)
        {
            if (!_pool.ContainsKey(tag)) return null;

            var root = _pool[tag];
            _pool.Remove(tag);

            root.transform.parent = parent;
            root.transform.localPosition = Vector3.zero;
            root.transform.localRotation = Quaternion.identity;
            root.transform.localScale = Vector3.one;

            return root;
        }

        public void Clear()
        {
            foreach(var root in _pool.Values)
            {
                GameObject.Destroy(root.gameObject);
            }

            _pool.Clear();
        }
    }
}