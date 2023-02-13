using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.VectorGraphics;


namespace AEAsAnimation
{
    public class AELayer : MonoBehaviour
    {
        public delegate void SelfEventHandler(AELayer layer);
        public event SelfEventHandler OnUpdated;

        private LayerData _data = null;
        private AEItemsAccessor _accessor = null;
        private AELayerReflector _reflector = null;

        private PositionFrameProcessor _position = null;
        private AngleFrameProcessor _rotation = null;
        private SizeFrameProcessor _scale = null;
        private PositionFrameProcessor _anchor = null;
        private OpacityFrameProcessor _opacity = null;

        private ColorFrameProcessor _overlayColor = null;
        private OpacityFrameProcessor _overlayOpacity = null;
        private OpacityFrameProcessor _maskOffset = null;
        private OpacityFrameProcessor _maskOpacity = null;
        private MaskShapeFrameProcessor _maskShape = null;

        private ColorFrameProcessor _parentOverlayColor = null;
        private OpacityFrameProcessor _parentOverlayOpacity = null;

        private Transform _nodeTreeRoot = null;

        public static AELayer Attach(
            Transform parent,
            LayerData data,
            AEItemsAccessor accessor)
        {
            var container = new GameObject(data.name);
            container.transform.parent = parent;
            var rectTransform = container.AddComponent<RectTransform>();
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localScale = Vector3.one;
            var layer = container.AddComponent<AELayer>().SetUp(data, accessor);
            accessor.RegisterLayer(layer, container);
            return layer;
        }

        public void SetNodeTreeRoot(Transform root)
        {
            _nodeTreeRoot = root;
        }

        public void SetReflector(AELayerReflector reflector)
        {
            _reflector = reflector;
        }

        public AELayer SetUp(
            LayerData data,
            AEItemsAccessor accessor)
        {
            _data = data;
            _accessor = accessor;

            _spf = 1.0f / _accessor.GetMetaData().fps;

            _position = new PositionFrameProcessor(data.properties.transforms.position);
            _rotation = new AngleFrameProcessor(data.properties.transforms.rotation);
            _scale = new SizeFrameProcessor(data.properties.transforms.scale, 100);
            _anchor = new PositionFrameProcessor(data.properties.transforms.anchor,
                new List<float>
                {
                    data.width / 2,
                    data.height / 2
                });
            _opacity = new OpacityFrameProcessor(data.properties.transforms.opacity, 100);

            if (data.properties.colorOverlays != null)
            {
                _overlayColor = new ColorFrameProcessor(data.properties.colorOverlays.color, 1);
                _overlayOpacity = new OpacityFrameProcessor(data.properties.colorOverlays.opacity, 100);
            }

            if (data.properties.masks != null)
            {
                _maskShape = new MaskShapeFrameProcessor(data.properties.masks.shape);
                _maskOpacity = new OpacityFrameProcessor(data.properties.masks.opacity);
                _maskOffset = new OpacityFrameProcessor(data.properties.masks.offset);
            }

            UpdateTransforms();

            return this;
        }

        private UnityEngine.UI.Image AddImage(GameObject target)
        {
            // deprecated
            return null;
        }

        public void ApplyParentOverlay(AVLayerColorOverlaysData colorOverlays)
        {
            if (colorOverlays == null) return;
            if (colorOverlays.color.maxFrame < 0
                && colorOverlays.opacity.maxFrame < 0) return;

            _parentOverlayColor = new ColorFrameProcessor(colorOverlays.color, 1);
            _parentOverlayOpacity = new OpacityFrameProcessor(colorOverlays.opacity, 100);
        }

        public void UpdateTransforms()
        {
            if (data.properties == null) return;

            var rectTransform = gameObject.GetComponent<RectTransform>();

            rectTransform.pivot =
                new Vector2(
                    data.width == 0 ? 0.5f : anchorX / data.width,
                    data.height == 0 ? 0.5f : 1 - (anchorY / data.height));

            rectTransform.localScale = new Vector3(
                    scaleX / 100,
                    scaleY / 100,
                    1);
            rectTransform.localRotation = Quaternion.Euler(0, 0, -1 * rotation);
            rectTransform.sizeDelta = new Vector2(data.width, data.height);

            if (IsDown(data))
            {
                var height = _accessor.GetComposition(data.parentCompositionId).height;
                var diff = -1 * y - height;
                rectTransform.anchoredPosition =
                    new Vector2(
                        x,
                        -1 * (Screen.height / 2 + diff)) +
                    new Vector2(
                        positionOffset.x,
                        positionOffset.y
                        );
            }
            else
            {
                rectTransform.anchoredPosition =
                    new Vector2(
                        x,
                        y) +
                    new Vector2(
                        positionOffset.x,
                        positionOffset.y
                        );
            }

            if (_reflector != null)
            {
                var relatedPosition = GetTransformDataFrom(gameObject.transform, _nodeTreeRoot);

                _reflector.UpdateTransform(new AELayerReflector.TransformData
                {
                    position = relatedPosition.position,
                    rotation = relatedPosition.rotation,
                    scale = relatedPosition.scale,
                    pivot = rectTransform.pivot,
                    size = rectTransform.sizeDelta
                });
            }
        }

        // -----------
        public class TransformPack
        {
            public Transform node;
            public Vector3 position;
            public Vector3 scale;
            public Quaternion rotation;

            public override string ToString()
            {

                return "position:" + position + "\nscale:" + scale + "\nrotation:" + rotation;
            }
        }

        public TransformPack GetParentTransformData(Transform child)
        {
            var parent = child.parent;
            if (parent == null)
            {
                return null;
            }

            var result = new TransformPack();

            result.node = parent;
            result.position = parent.localPosition;
            result.rotation = parent.localRotation;
            result.scale = parent.localScale;

            return result;
        }

        public TransformPack GetTransformDataFrom(
            Transform child,
            Transform root)
        {
            var result = new TransformPack
            {
                node = child,
                position = child.localPosition,
                rotation = child.localRotation,
                scale = child.localScale
            };
            TransformPack currentNode = result;
            TransformPack parent = null;
            while (true)
            {
                if (currentNode.node == root)
                {
                    break;
                }

                parent = GetParentTransformData(currentNode.node);

                if (parent == null)
                {
                    break;
                }

                var mat = Matrix4x4.identity;
                mat.SetTRS(parent.position, parent.rotation, parent.scale);
                result.position = mat.MultiplyPoint3x4(result.position);
                result.rotation *= parent.rotation;

                var xyRadian = Mathf.PI * result.rotation.eulerAngles.z / 180;
                var rotatedParentScale =
                    scaleEffectedByRotation()
                    ? new Vector3(
                    Mathf.Cos(xyRadian) * parent.scale.x - Mathf.Sin(xyRadian) * parent.scale.y,
                    Mathf.Sin(xyRadian) * parent.scale.x + Mathf.Cos(xyRadian) * parent.scale.y,
                    parent.scale.z)
                    : parent.scale;
                result.scale = new Vector3(
                    result.scale.x * rotatedParentScale.x,
                    result.scale.y * rotatedParentScale.y,
                    result.scale.z * rotatedParentScale.z
                    );

                currentNode = parent;
            }

            return result;
        }
        
        private bool scaleEffectedByRotation()
        {
            // kari
            return data.footage is SolidSourceFootageData;
        }
        
        // -------------


        private void UpdateColors()
        {
            var composition = _accessor.GetComposition(data.compositionId);
            if (composition != null)
            {
            }
            else if (data.footage != null)
            {
                var color = new Color();

                if (data.footage is SolidSourceFootageData)
                {
                    var solidFootage = data.footage as SolidSourceFootageData;

                    color = new Color(
                        solidFootage.color[0],
                        solidFootage.color[1],
                        solidFootage.color[2],
                        opacity / 100);
                }
                else if (data.footage is FileSourceFootageData)
                {
                    var global = _accessor.GetGlobalColorData();
                    var multiply = global != null
                        ? new Color(
                            global.multiplyR,
                            global.multiplyG,
                            global.multiplyB,
                            global.multiplyA
                            )
                        : new Color(1, 1, 1, 1);
                    color = new Color(
                        1.0f * multiply.r,
                        1.0f * multiply.g,
                        1.0f * multiply.b,
                        opacity / 100.0f * multiply.a);
                }

                if (_reflector != null)
                {
                    _reflector.UpdateColor(new AELayerReflector.ColorData
                    {
                        color = color,
                        overlayColor = overlayColor
                    });
                }
            }
        }

        private void UpdateMask()
        {
            if (_maskShape == null) return;

            bool inverted = data.properties.masks.inverted;
            var shapeComposer = new AEShapeComposer(
                _maskShape.shape,
                data.width,
                data.height,
                inverted);

            if (_reflector != null)
            {
                _reflector.UpdateMask(new AELayerReflector.MaskData
                {
                    composer = shapeComposer
                });
            }
        }

        private float _elaspedSeconds = 0;
        private float _spf = 1.0f / 60;
        // called by unity system.
        private void Update()
        {
            if (_isStop
                || _accessor.GetMetaData().asLayout) return;

            if ((_elaspedSeconds += Time.deltaTime) < _spf) return;
            _elaspedSeconds -= _spf;

            GoNextFrame(updated => {
            });

            UpdateActive();
        }

        public bool isInTime
        {
            get
            {
                if (_accessor.GetMetaData().asLayout) return true;

                var currentSeconds = _position.currentFrame / _accessor.GetMetaData().fps;
                return currentSeconds >= data.inSeconds
                    && currentSeconds < data.outSeconds;
            }
        }


        private void UpdateActive()
        {
            if (isInTime)
            {
                _reflector.OrderFrameCommand(new AELayerReflector.FrameCommandData
                {
                    command = AELayerReflector.FrameCommandData.Type.Activate
                });
            }
            else
            {
                _reflector.OrderFrameCommand(new AELayerReflector.FrameCommandData
                {
                    command = AELayerReflector.FrameCommandData.Type.Deactivate
                });
            }
        }


        // ------------------------------ methods
        public void GoNextFrame(System.Action<bool> callback = null)
        {
            bool isUpdated = true;

            _position.GoNextFrame();
            _rotation.GoNextFrame();
            _scale.GoNextFrame();
            _anchor.GoNextFrame();
            _opacity.GoNextFrame();
            if (_overlayColor != null) _overlayColor.GoNextFrame();
            if (_overlayOpacity != null) _overlayOpacity.GoNextFrame();
            if (_parentOverlayColor != null) _parentOverlayColor.GoNextFrame();
            if (_parentOverlayOpacity != null) _parentOverlayOpacity.GoNextFrame();
            if (_maskShape != null) _maskShape.GoNextFrame();
            if (_maskOffset != null) _maskOffset.GoNextFrame();
            if (_maskOpacity != null) _maskOpacity.GoNextFrame();

            if (callback != null) callback(isUpdated);

            UpdateTransforms();
            UpdateColors();
            UpdateMask();

            if (isUpdated
                && OnUpdated != null) OnUpdated(this);
        }

        public void GoToFrame(int frame, int periodFrame = -1)
        {
            _isStop = false;

            _position.GoToFrame(frame, periodFrame);
            _rotation.GoToFrame(frame, periodFrame);
            _scale.GoToFrame(frame, periodFrame);
            _anchor.GoToFrame(frame, periodFrame);
            _opacity.GoToFrame(frame, periodFrame);
            if (_overlayColor != null) _overlayColor.GoToFrame(frame, periodFrame);
            if (_overlayOpacity != null) _overlayOpacity.GoToFrame(frame, periodFrame);
            if (_parentOverlayColor != null) _parentOverlayColor.GoToFrame(frame, periodFrame);
            if (_parentOverlayOpacity != null) _parentOverlayOpacity.GoToFrame(frame, periodFrame);
            if (_maskShape != null) _maskShape.GoToFrame(frame, periodFrame);
            if (_maskOffset != null) _maskOffset.GoToFrame(frame, periodFrame);
            if (_maskOpacity != null) _maskOpacity.GoToFrame(frame, periodFrame);

            if (_reflector != null)
            {
                _reflector.OrderFrameCommand(new AELayerReflector.FrameCommandData
                {
                    command = AELayerReflector.FrameCommandData.Type.GotoFrame,
                    gotoFrame = frame,
                    periodFrame = periodFrame
                });
            }

            UpdateTransforms();
            UpdateColors();
            UpdateMask();
            UpdateActive();

            if (OnUpdated != null) OnUpdated(this);
        }

        public void Restart()
        {
            _isStop = false;

            _position.Restart();
            _rotation.Restart();
            _scale.Restart();
            _anchor.Restart();
            _opacity.Restart();
            if (_overlayColor != null) _overlayColor.Restart();
            if (_overlayOpacity != null) _overlayOpacity.Restart();
            if (_parentOverlayColor != null) _parentOverlayColor.Restart();
            if (_parentOverlayOpacity != null) _parentOverlayOpacity.Restart();
            if (_maskShape != null) _maskShape.Restart();
            if (_maskOffset != null) _maskOffset.Restart();
            if (_maskOpacity != null) _maskOpacity.Restart();

            if (_reflector != null)
            {
                _reflector.OrderFrameCommand(new AELayerReflector.FrameCommandData
                {
                    command = AELayerReflector.FrameCommandData.Type.Restart
                });
            }

            UpdateTransforms();
            UpdateColors();
            UpdateMask();
            UpdateActive();

            if (OnUpdated != null) OnUpdated(this);
        }

        public void SetLoop()
        {
            _position.SetLoop();
            _rotation.SetLoop();
            _scale.SetLoop();
            _anchor.SetLoop();
            _opacity.SetLoop();
            if (_overlayColor != null) _overlayColor.SetLoop();
            if (_overlayOpacity != null) _overlayOpacity.SetLoop();
            if (_parentOverlayColor != null) _parentOverlayColor.SetLoop();
            if (_parentOverlayOpacity != null) _parentOverlayOpacity.SetLoop();
            if (_maskShape != null) _maskShape.SetLoop();
            if (_maskOffset != null) _maskOffset.SetLoop();
            if (_maskOpacity != null) _maskOpacity.SetLoop();

            if (_reflector != null)
            {
                _reflector.OrderFrameCommand(new AELayerReflector.FrameCommandData
                {
                    command = AELayerReflector.FrameCommandData.Type.Loop
                });
            }
        }

        public void SetLoop(
            int from,
            int to)
        {
            _position.SetLoop(from, to);
            _rotation.SetLoop(from, to);
            _scale.SetLoop(from, to);
            _anchor.SetLoop(from, to);
            _opacity.SetLoop(from, to);
            if (_overlayColor != null) _overlayColor.SetLoop(from, to);
            if (_overlayOpacity != null) _overlayOpacity.SetLoop(from, to);
            if (_parentOverlayColor != null) _parentOverlayColor.SetLoop(from, to);
            if (_parentOverlayOpacity != null) _parentOverlayOpacity.SetLoop(from, to);
            if (_maskShape != null) _maskShape.SetLoop(from, to);
            if (_maskOffset != null) _maskOffset.SetLoop(from, to);
            if (_maskOpacity != null) _maskOpacity.SetLoop(from, to);

            if (_reflector != null)
            {
                _reflector.OrderFrameCommand(new AELayerReflector.FrameCommandData
                {
                    command = AELayerReflector.FrameCommandData.Type.Loop,
                    loopFrom = from,
                    loopTo = to
                });
            }
        }

        private bool _isStop = false;
        public void StopAnimation()
        {
            _isStop = true;

            if (_reflector != null)
            {
                _reflector.OrderFrameCommand(new AELayerReflector.FrameCommandData
                {
                    command = AELayerReflector.FrameCommandData.Type.Stop
                });
            }
        }

        public bool IsDown(LayerData data)
        {
            var metaData = data.metaData;
            return metaData.ToLower().Contains("down");
        }

        public bool IsButton(LayerData data)
        {
            var metaData = data.metaData;
            return metaData.ToLower().Contains("button");
        }

        public LayerData data
        {
            get
            {
                return _data;
            }
        }


        public float x
        {
            get
            {
                return _position.x;
            }
        }
        public float y
        {
            get
            {
                return _position.y * -1;
            }
        }
        public float rotation
        {
            get
            {
                return _rotation.angle;
            }
        }
        public float scaleX
        {
            get
            {
                return _scale.x;
            }
        }
        public float scaleY
        {
            get
            {
                return _scale.y;
            }
        }
        public float anchorX
        {
            get
            {
                return _anchor.x;
            }
        }
        public float anchorY
        {
            get
            {
                return _anchor.y;
            }
        }
        public float opacity
        {
            get
            {
                var global = _accessor.GetGlobalColorData();
                return _opacity.opacity * (global != null ? global.opacity : 1);
            }
        }

        public Color overlayColor
        {
            get
            {
                var global = _accessor.GetGlobalColorData();
                if (global != null
                    && global.overlayA > 0) return new Color(global.overlayR, global.overlayG, global.overlayB, global.overlayA);
                if (_overlayColor == null
                    || _overlayOpacity == null) return Color.clear;

                var overlayColor = _parentOverlayColor != null
                    ? _parentOverlayColor
                    : _overlayColor;

                var overlayOpacity = _parentOverlayOpacity != null
                    ? _parentOverlayOpacity
                    : _overlayOpacity;

                return new Color(
                    overlayColor.r,
                    overlayColor.g,
                    overlayColor.b,
                    1.0f * overlayOpacity.opacity / 100);
            }
        }

        public Vector2 leftTop
        {
            get
            {
                var left = -1 * data.width / 2;
                var top = data.height / 2;
                return new Vector2(left, top);
            }
        }

        private Vector3 _positionOffset = Vector3.zero;
        public Vector3 positionOffset
        {
            set
            {
                _positionOffset = value;
            }

            get
            {
                return _positionOffset;
            }
        }

        public int totalFrame
        {
            get
            {
                return data.properties.totalFrane;
            }
        }


        public string imageFile
        {
            get
            {
                if (data.footage != null
                    && data.footage is FileSourceFootageData)
                {
                    return data.footage.sourceName;
                }
                return "";
            }
        }

        public List<string> allImageFiles
        {
            get
            {
                var result = new List<string>();

                if (imageFile != "")
                {
                    result.Add(imageFile);
                }

                return result;
            }
        }

        public List<string> allLayerMetaData
        {
            get
            {
                var result = new List<string>();

                if (data.properties != null
                    && data.metaData != null
                    && data.metaData.Length > 0)
                {
                    result.Add(data.metaData);
                }
                return result;
            }
        }
    }
}