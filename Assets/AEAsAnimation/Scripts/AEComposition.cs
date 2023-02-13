using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AEAsAnimation
{
    public class AEComposition : MonoBehaviour
    {
        private CompositionData _data = null;
        private AEItemsAccessor _accessor = null;

        private Transform _nodeTreeRoot = null;
        private Transform _reflectionRoot = null;

        private List<AELayer> _layers = new List<AELayer>();
        private List<AELayerReflection> _reflections = new List<AELayerReflection>();

        public static AEComposition Attach(
            Transform parent,
            CompositionData data,
            AEItemsAccessor accessor,
            bool isChild = false)
        {
            var container = new GameObject("composition-" + data.id);
            container.transform.parent = parent;
            var rectTransform = container.AddComponent<RectTransform>();
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localScale = Vector3.one;
            rectTransform.localRotation = Quaternion.Euler(0, 0, 0);
            var composition = container.AddComponent<AEComposition>().SetUp(data, accessor, isChild);
            accessor.RegisterComposition(composition, container);
            return composition;
        }

        public static IEnumerator AttachAsync(
            Transform parent,
            CompositionData data,
            AEItemsAccessor accessor,
            System.Action<AEComposition> callback,
            bool isChild = false)
        {
            var container = new GameObject("composition-" + data.id);
            container.transform.parent = parent;
            var rectTransform = container.AddComponent<RectTransform>();
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localScale = Vector3.one;
            rectTransform.localRotation = Quaternion.Euler(0, 0, 0);
            var composition = container.AddComponent<AEComposition>().SetUp(data, accessor, isChild);
            accessor.RegisterComposition(composition, container);

            callback(composition);

            yield return null;
        }



        public AEComposition SetUp(
            CompositionData data,
            AEItemsAccessor accessor,
            bool isChild = false)
        {
            _data = data;
            _accessor = accessor;

            var nodeTree = new GameObject("node tree");
            nodeTree.transform.parent = gameObject.transform;
            nodeTree.transform.localPosition = Vector3.zero;
            nodeTree.transform.localScale = Vector3.one;
            nodeTree.transform.localRotation = Quaternion.Euler(0, 0, 0);
            _nodeTreeRoot = nodeTree.transform;
            var reflection = new GameObject("reflection");
            reflection.transform.parent = gameObject.transform;
            reflection.transform.localPosition = Vector3.zero;
            reflection.transform.localScale = Vector3.one;
            reflection.transform.localRotation = Quaternion.Euler(0, 0, 0);
            _reflectionRoot = reflection.transform;

            var rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(data.width, data.height);

            if (data.metaData.ToLower().Contains("full"))
            {
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
            }

            // create layers
            var indexMap = new Dictionary<int, AELayer>();
            foreach (var layerId in data.layerIds)
            {
                var layerData = accessor.GetLayer(layerId);

                var layer = AELayer.Attach(
                    nodeTree.transform,
                    layerData,
                    accessor);
                layer.SetNodeTreeRoot(_nodeTreeRoot);

                if (isChild)
                {
                    var offset = Vector2.zero;
                    offset += new Vector2(-1 * data.width / 2, data.height / 2);
                    layer.positionOffset = offset;
                }

                _layers.Add(layer);

                indexMap.Add(layerData.layerIndex, layer);
            }

            // apply siblings
            foreach (var layer in _layers)
            {
                if (!indexMap.ContainsKey(layer.data.parentIndex)) continue;
                var parent = indexMap[layer.data.parentIndex];
                layer.transform.SetParent(parent.transform);
                layer.positionOffset += new Vector3(parent.leftTop.x, parent.leftTop.y);
            }

            // create reflection nodes
            int maxIndex = 0;
            foreach (var index in indexMap.Keys)
            {
                if (index > maxIndex) maxIndex = index;
            }
            for (var i = maxIndex; i >= 0; i--)
            {
                if (!indexMap.ContainsKey(i)) continue;
                var rawNode = indexMap[i];
                var layerData = rawNode.data;

                // image file
                var imageFilePath = "";
                bool overrideSize = false;
                if (layerData.footage != null
                    && layerData.footage is FileSourceFootageData)
                {
                    var fileName = layerData.footage.sourceName;
                    var swapImageData = _accessor.GetSwapImage(fileName);
                    if (swapImageData != null)
                    {
                        fileName = swapImageData.to;
                        overrideSize = swapImageData.overrideSize;
                        imageFilePath = (swapImageData.directory != "" ? swapImageData.directory : _accessor.GetFileDirectory()) + fileName;
                    }
                    else
                    {
                        imageFilePath = _accessor.GetFileDirectory() + fileName;
                    }
                }

                // child composition
                var composition = accessor.GetComposition(layerData.compositionId);

                // color overlay
                var colorOverlays = layerData.properties.colorOverlays;

                // button
                var buttonTag = "";
                var isButton = layerData.metaData.ToLower().Contains("button");
                if (isButton)
                {
                    buttonTag = layerData.metaData;
                }
                else
                {
                    // todo : 整理
                    isButton = layerData.name.ToLower().Contains("button");
                    if (isButton)
                    {
                        buttonTag = layerData.name;
                    }
                }

                var reflector = new AELayerReflector(
                    new AELayerReflector.MetaData
                    {
                        name = "ref-" + rawNode.data.name,
                        accessor = accessor,
                        buttonTag = isButton ? buttonTag : "",
                        imageFilePath = imageFilePath,
                        overrideSize = overrideSize,
                        child = composition,
                        colorOverlays = colorOverlays
                    });

                rawNode.SetReflector(reflector);
                var reflectionNode = AELayerReflection.Attach(
                    _reflectionRoot,
                    reflector);
                rawNode.UpdateTransforms();

                accessor.RegisterLayerReflection(rawNode, reflectionNode);
                _reflections.Add(reflectionNode);
            }

            return this;
        }


        public IEnumerator SetUpAsync(
            CompositionData data,
            AEItemsAccessor accessor,
            System.Action<AEComposition> callback,
            bool isChild = false)
        {
            _data = data;
            _accessor = accessor;

            var nodeTree = new GameObject("node tree");
            nodeTree.transform.parent = gameObject.transform;
            nodeTree.transform.localPosition = Vector3.zero;
            nodeTree.transform.localScale = Vector3.one;
            nodeTree.transform.localRotation = Quaternion.Euler(0, 0, 0);
            _nodeTreeRoot = nodeTree.transform;
            var reflection = new GameObject("reflection");
            reflection.transform.parent = gameObject.transform;
            reflection.transform.localPosition = Vector3.zero;
            reflection.transform.localScale = Vector3.one;
            reflection.transform.localRotation = Quaternion.Euler(0, 0, 0);
            _reflectionRoot = reflection.transform;

            var rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(data.width, data.height);

            if (data.metaData.ToLower().Contains("full"))
            {
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
            }

            // create layers
            var indexMap = new Dictionary<int, AELayer>();
            foreach (var layerId in data.layerIds)
            {
                var layerData = accessor.GetLayer(layerId);

                var layer = AELayer.Attach(
                    nodeTree.transform,
                    layerData,
                    accessor);
                layer.SetNodeTreeRoot(_nodeTreeRoot);

                layer.gameObject.transform.localPosition += new Vector3(
                    -1 * data.width / 2,
                    layer.IsDown(layerData) ? 0 : data.height / 2,
                    0
                );

                if (isChild)
                {
                    var offset = Vector2.zero;
                    offset += new Vector2(-1 * data.width / 2, data.height / 2);
                    layer.positionOffset = offset;
                }

                _layers.Add(layer);

                indexMap.Add(layerData.layerIndex, layer);

                yield return null;
            }

            // apply siblings
            foreach (var layer in _layers)
            {
                if (!indexMap.ContainsKey(layer.data.parentIndex)) continue;
                var parent = indexMap[layer.data.parentIndex];
                layer.transform.parent = parent.transform;
                layer.positionOffset += new Vector3(parent.leftTop.x, parent.leftTop.y);
                yield return null;
            }

            // create reflection nodes
            int maxIndex = 0;
            foreach (var index in indexMap.Keys)
            {
                if (index > maxIndex) maxIndex = index;
            }
            for (var i = maxIndex; i >= 0; i--)
            {
                if (!indexMap.ContainsKey(i)) continue;
                var rawNode = indexMap[i];
                var layerData = rawNode.data;

                // image file
                var imageFilePath = "";
                bool overrideSize = false;
                if (layerData.footage != null
                    && layerData.footage is FileSourceFootageData)
                {
                    var fileName = layerData.footage.sourceName;
                    var swapImageData = _accessor.GetSwapImage(fileName);
                    if (swapImageData != null)
                    {
                        fileName = swapImageData.to;
                        overrideSize = swapImageData.overrideSize;
                        imageFilePath = (swapImageData.directory != "" ? swapImageData.directory : _accessor.GetFileDirectory()) + fileName;
                    }
                    else
                    {
                        imageFilePath = _accessor.GetFileDirectory() + fileName;
                    }
                }

                // child composition
                var composition = accessor.GetComposition(layerData.compositionId);

                // color overlay
                var colorOverlays = layerData.properties.colorOverlays;

                // button
                var buttonTag = "";
                var isButton = layerData.metaData.ToLower().Contains("button");
                if (isButton)
                {
                    buttonTag = layerData.metaData;
                }
                else
                {
                    // todo : 整理
                    isButton = layerData.name.ToLower().Contains("button");
                    if (isButton)
                    {
                        buttonTag = layerData.name;
                    }
                }

                var reflector = new AELayerReflector(
                    new AELayerReflector.MetaData
                    {
                        name = "ref-" + rawNode.data.name,
                        accessor = accessor,
                        buttonTag = isButton ? buttonTag : "",
                        imageFilePath = imageFilePath,
                        overrideSize = overrideSize,
                        child = composition,
                        colorOverlays = colorOverlays
                    });

                rawNode.SetReflector(reflector);

                yield return AELayerReflection.AttachAsync(
                    _reflectionRoot,
                    reflector,
                    reflectionNode => {
                        rawNode.UpdateTransforms();
                        accessor.RegisterLayerReflection(rawNode, reflectionNode);
                        _reflections.Add(reflectionNode);
                    });
            }

            callback(this);

            yield return null;
        }


        public AEComposition SetButton(string tag)
        {
            foreach(var reflection in _reflections)
            {
                reflection.SetButton(tag);
            }
            return this;
        }

        public void ApplyParentOverlay(AVLayerColorOverlaysData colorOverlays)
        {
            foreach (var layer in _layers)
            {
                layer.ApplyParentOverlay(colorOverlays);
            }
        }

        public int GoToMarker(string markerTag, bool loop = false)
        {
            var startFrame = GetMarkerStartFrame(markerTag);
            var endFrame = GetMarkerEndFrame(markerTag);
            var duration = endFrame - startFrame;
            if (duration < 0) duration = 0;

            foreach (var layer in _layers)
            {
                if (loop)
                {
                    layer.SetLoop(GetMarkerStartFrame(markerTag), GetMarkerEndFrame(markerTag));
                }
                else
                {
                    layer.SetLoop(-1, -1);
                }

                layer.GoToFrame(GetMarkerStartFrame(markerTag), GetMarkerEndFrame(markerTag));
            }

            return duration;
        }

        public void KeepFrame(int frame)
        {
            foreach (var layer in _layers)
            {
                layer.GoToFrame(frame, frame + 1);
            }
        }

        public int GoToStart(bool loop = false, int lastOffset = 0)
        {
            var endFrame = HasMarker() ? GetFirstMarkerFrame() : (totalFrame - lastOffset);
            var duration = endFrame;

            foreach (var layer in _layers)
            {
                if (loop)
                {
                    layer.SetLoop(0, endFrame);
                }
                else
                {
                    layer.SetLoop(-1, -1);
                }

                layer.GoToFrame(0, endFrame);
            }

            return duration;
        }

        public void SetLoop()
        {
            foreach (var layer in _layers)
            {
                layer.SetLoop();
            }
        }

        public void SetLoop(
            int from,
            int to)
        {
            foreach (var layer in _layers)
            {
                layer.SetLoop(from, to);
            }
        }

        public void Restart()
        {
            foreach (var layer in _layers)
            {
                layer.Restart();
            }
        }

        public void StopAnimation()
        {
            foreach (var layer in _layers)
            {
                layer.StopAnimation();
            }
        }

        public void GoToFrame(int frame, int periodFrame = -1)
        {
            foreach (var layer in _layers)
            {
                layer.GoToFrame(frame, periodFrame);
            }
        }

        public int totalFrame
        {
            get
            {
                int result = 0;

                foreach(var layer in _layers)
                {
                    if (result < layer.totalFrame) result = layer.totalFrame;
                }

                return result;
            }
        }
        
        public string markerInfromations
        {
            get
            {
                var informations = "";

                var markers = _data.markers;
                if (markers == null) return informations;

                var markerInformation = new Dictionary<string, int>();

                foreach (var element in markers.elements)
                {
                    var key = element.args[0];
                    if (markerInformation.ContainsKey(key)) continue;
                    markerInformation.Add(key, element.frame);
                }

                foreach (var pair in markerInformation)
                {
                    informations += pair.Key + "(" + pair.Value + " frame)\n";
                }

                return informations;
            }
        }

        public List<string> allMarkers
        {
            get
            {
                var result = new List<string>();

                var markers = _data.markers;
                if (markers == null) return result;

                foreach (var element in markers.elements)
                {
                    var key = element.args[0];
                    if (result.Contains(key)) continue;
                    result.Add(key);
                }

                return result;
            }
        }

        public List<string> allImageFiles
        {
            get
            {
                var result = new List<string>();

                foreach (var layer in _layers)
                {
                    result.AddRange(layer.allImageFiles);
                }
                foreach (var reflection in _reflections)
                {
                    result.AddRange(reflection.allImageFiles);
                }

                return result;
            }
        }

        public List<string> allLayerNames
        {
            get
            {
                var result = new List<string>();

                foreach (var layer in _layers)
                {
                    result.Add(layer.data.name);
                }
                foreach (var reflection in _reflections)
                {
                    result.AddRange(reflection.allLayerNames);
                }

                return result;
            }
        }

        public List<string> allLayerMetaData
        {
            get
            {
                var result = new List<string>();

                foreach (var layer in _layers)
                {
                    result.AddRange(layer.allLayerMetaData);
                }
                foreach (var reflection in _reflections)
                {
                    result.AddRange(reflection.allLayerMetaData);
                }

                return result;
            }
        }


        // ------------ private
        private int GetFirstMarkerFrame()
        {
            var markers = _data.markers;

            if (markers.elements.Count > 0 
                && markers.elements[0].args[0] != "")
            {
                return GoToMarker(markers.elements[0].args[0]);
            }

            foreach (var element in markers.elements)
            {
                if (element.args[0] == "") continue;
                return element.frame;
            }
            return 0;
        }

        private int GetMarkerStartFrame(string markerTag)
        {
            var markers = _data.markers;
            foreach (var element in markers.elements)
            {
                if (element.args[0] == markerTag) return element.frame;
            }
            return 0;
        }

        private int GetMarkerEndFrame(string markerTag)
        {
            var markers = _data.markers;
            bool onTargetMarker = false;
            foreach (var element in markers.elements)
            {
                if (onTargetMarker)
                {
                    if (DeleteNewLineCode(element.args[0]) != markerTag) return element.frame;
                }

                if (DeleteNewLineCode(element.args[0]) == markerTag) onTargetMarker = true;
            }
            return markers.maxFrame;
        }

        private bool HasMarker()
        {
            foreach (var element in _data.markers.elements)
            {
                if (element.args[0] == "") continue;
                return true;
            }
            return false;
        }

        private string DeleteNewLineCode(string raw)
        {
            return raw.Replace("\n", "").Replace("\r", "");
        }
    }
}