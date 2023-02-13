using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AEAsAnimation
{
    public class AELayerReflection : MonoBehaviour
    {
        public static AELayerReflection Attach(
            Transform parent,
            AELayerReflector reflector)
        {
            var container = new GameObject(reflector.metaData.name);
            container.transform.parent = parent;
            var rectTransform = container.AddComponent<RectTransform>();
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localScale = Vector3.one;

            return container.AddComponent<AELayerReflection>().SetUp(reflector);
        }

        public static IEnumerator AttachAsync(
            Transform parent,
            AELayerReflector reflector,
            System.Action<AELayerReflection> callback)
        {
            var container = new GameObject(reflector.metaData.name);
            container.transform.parent = parent;
            var rectTransform = container.AddComponent<RectTransform>();
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localScale = Vector3.one;

            yield return container.AddComponent<AELayerReflection>().SetUpAsync(reflector, reflection =>
            {
                callback(reflection);
            });
        }

        private LayerData _data;
        private AELayerReflector _reflector;

        private UnityEngine.UI.Image _image = null;
        private UnityEngine.UI.Image _overlayImage = null;
        private UnityEngine.UI.Image _maskImage = null;
        private AEComposition _childComposition = null;

        private bool _overrideSize = false;
        private Vector2 _overrideSizeDelta = Vector2.zero;

        public AELayerReflection SetUp(
            AELayerReflector reflector)
        {
            SetUpEvents(reflector);

            var childData = reflector.metaData.child;
            if (childData != null)
            {
                SetUpChild(reflector.metaData);
            }
            else
            {
                _image = AddImage(reflector.metaData.imageFilePath, gameObject, reflector.metaData.overrideSize);
                if (reflector.metaData.hasOverlay) SetUpOverlay(reflector.metaData.imageFilePath, reflector.metaData.overrideSize);
            }

            if (reflector.metaData.isButton)
            {
                SetButton(reflector.metaData.buttonTag);
            }

            return this;
        }

        public IEnumerator SetUpAsync(
            AELayerReflector reflector,
            System.Action<AELayerReflection> callback)
        {
            SetUpEvents(reflector);

            var childData = reflector.metaData.child;
            if (childData != null)
            {
                yield return SetUpChildAsync(reflector.metaData);
            }
            else
            {
                _image = AddImage(reflector.metaData.imageFilePath, gameObject, reflector.metaData.overrideSize);
                if (reflector.metaData.hasOverlay) SetUpOverlay(reflector.metaData.imageFilePath, reflector.metaData.overrideSize);
            }

            if (reflector.metaData.isButton)
            {
                SetButton(reflector.metaData.buttonTag);
            }

            callback(this);

            yield return null;
        }

        private AELayerReflection SetUpEvents(
            AELayerReflector reflector)
        {
            _reflector = reflector;

            bool isStop = false;

            reflector.OnTransformUpdated += transformData => {
                if (isStop) return;

                transform.localPosition = transformData.position;
                transform.localScale = transformData.scale;
                transform.localRotation = transformData.rotation;

                var rectTransform = gameObject.GetComponent<RectTransform>();
                rectTransform.pivot = transformData.pivot;
                rectTransform.sizeDelta = _overrideSize ? _overrideSizeDelta : transformData.size;

                if (_overlayImage != null)
                {
                    var overLayRectTransform = _overlayImage.GetComponent<RectTransform>();
                    overLayRectTransform.sizeDelta = _overrideSize ? _overrideSizeDelta : transformData.size;
                }
            };
            reflector.OnColorUpdated += colorData => {
                if (_image != null)
                {
                    _image.color = colorData.color;
                }

                if (_overlayImage != null)
                {
                    _overlayImage.material.color = new Color(
                        colorData.overlayColor.r,
                        colorData.overlayColor.g,
                        colorData.overlayColor.b,
                        1
                        );
                    _overlayImage.color = new Color(
                        1, 1, 1,
                        Mathf.Min(colorData.overlayColor.a , colorData.color.a)
                        );
                }
            };
            reflector.OnMaskUpdated += maskData => {
                if (_maskImage == null)
                {
                    SetUpMask(maskData);
                }

                _maskImage.sprite = maskData.composer.GetSprite();
                _maskImage.gameObject.GetComponent<RectTransform>().sizeDelta = maskData.composer.size;
            };
            reflector.OnFrameCommandOrdered += commandData => {
                if (commandData.command == AELayerReflector.FrameCommandData.Type.Activate)
                {
                    gameObject.SetActive(true);
                }
                else if (commandData.command == AELayerReflector.FrameCommandData.Type.Deactivate)
                {
                    gameObject.SetActive(false);
                }

                if (_childComposition == null) return;
                if (commandData.command == AELayerReflector.FrameCommandData.Type.GotoFrame)
                {
                    isStop = false;
                    _childComposition.GoToFrame(commandData.gotoFrame, commandData.periodFrame);
                }
                else if (commandData.command == AELayerReflector.FrameCommandData.Type.Restart)
                {
                    isStop = false;
                    _childComposition.Restart();
                }
                else if (commandData.command == AELayerReflector.FrameCommandData.Type.Loop)
                {
                    _childComposition.SetLoop(commandData.loopFrom, commandData.loopTo);
                }
                else if (commandData.command == AELayerReflector.FrameCommandData.Type.Stop)
                {
                    isStop = true;
                    _childComposition.StopAnimation();
                }
            };

            return this;
        }

            public void SetButton(string tag)
        {
            if (_image == null
                || _reflector.metaData.accessor == null) return;

            var button = gameObject.GetComponent<UnityEngine.UI.Button>();
            if (button == null) button = gameObject.AddComponent<UnityEngine.UI.Button>();
            button.image = _image;
            var onClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            onClick.AddListener(() => {
                _reflector.metaData.accessor.OnClick(tag);
            });
            button.onClick = onClick;
        }

        public void SwapImage(string newImageFile, bool overrideSize = false)
        {
            if (_overlayImage != null)
            {
                GameObject.Destroy(_overlayImage.gameObject);
                _overlayImage = null;
            }

            _image = AddImage(newImageFile, gameObject, overrideSize);
            if (_reflector.metaData.hasOverlay) SetUpOverlay(newImageFile, overrideSize);
        }

        public List<string> allImageFiles
        {
            get
            {
                return _childComposition != null
                    ? _childComposition.allImageFiles
                    : new List<string>();
            }
        }

        public List<string> allLayerNames
        {
            get
            {
                return _childComposition != null
                    ? _childComposition.allLayerNames
                    : new List<string>();
            }
        }

        public List<string> allLayerMetaData
        {
            get
            {
                return _childComposition != null
                    ? _childComposition.allLayerMetaData
                    : new List<string>();
            }
        }


        // ------------------------- private 
        private void SetUpChild(AELayerReflector.MetaData metaData)
        {
            var aeComposition = _childComposition = AEComposition.Attach(
                gameObject.transform,
                metaData.child,
                metaData.accessor,
                true);

            _childComposition.ApplyParentOverlay(metaData.colorOverlays);
        }

        private IEnumerator SetUpChildAsync(AELayerReflector.MetaData metaData)
        {
            yield return AEComposition.AttachAsync(
                gameObject.transform,
                metaData.child,
                metaData.accessor,
                aeComposition => {
                    _childComposition = aeComposition;
                    _childComposition.ApplyParentOverlay(metaData.colorOverlays);
                },
                true);
        }

        private void SetUpOverlay(string imageFilePath, bool overrideSize = false)
        {
            var overlay = new GameObject("overlay");
            overlay.transform.parent = gameObject.transform;
            overlay.transform.localPosition = Vector3.zero;
            overlay.transform.localRotation = Quaternion.Euler(0, 0, 0);
            overlay.transform.localScale = Vector3.one;

            _overlayImage = AddImage(imageFilePath, overlay, overrideSize);

            var parentRectTransform = gameObject.GetComponent<RectTransform>();
            var rectTransform = overlay.GetComponent<RectTransform>();
            rectTransform.sizeDelta = parentRectTransform.sizeDelta;

            bool hasMask = GetComponentsInParent<UnityEngine.UI.Mask>().Length > 0;
            var raw = Resources.Load<Material>(hasMask ? "Materials/ColorOverlay" : "Materials/ColorOverlayWithoutMask");
            var material = new Material(raw);
            _overlayImage.material = material;
        }

        private void SetUpMask(AELayerReflector.MaskData maskData)
        {
            if (_childComposition == null) return;

            var maskNode = new GameObject("mask");
            maskNode.transform.parent = gameObject.transform;
            maskNode.transform.localPosition = Vector3.zero;
            maskNode.transform.localRotation = Quaternion.Euler(0, 0, 0);
            maskNode.transform.localScale = Vector3.one;

            var sprite = maskData.composer.GetSprite();

            var rectTransform = maskNode.AddComponent<RectTransform>();

            var maskImage = _maskImage = maskNode.AddComponent<UnityEngine.UI.Image>();
            maskImage.sprite = sprite;
            maskImage.useSpriteMesh = true;

            var uiMask = maskNode.AddComponent<UnityEngine.UI.Mask>();
            uiMask.showMaskGraphic = false;
            rectTransform.sizeDelta = maskData.composer.size;

            _childComposition.gameObject.transform.SetParent(maskNode.transform);
        }

        private UnityEngine.UI.Image AddImage(string imageFilePath, GameObject target, bool overrideSize = false)
        {
            var image = target.GetComponent<UnityEngine.UI.Image>();
            if (image == null) image = target.AddComponent<UnityEngine.UI.Image>();

            if (imageFilePath == null 
                || imageFilePath == "")
            {
                image.sprite = null;
            }
            else
            {
                TextureLoader.Instance.GetSprite(
                    imageFilePath,
                    sprite => {
                        image.sprite = sprite;

                        if (overrideSize)
                        {
                            _overrideSize = true;
                            image.gameObject.GetComponent<RectTransform>().sizeDelta = _overrideSizeDelta = new Vector2(sprite.texture.width, sprite.texture.height);
                        }
                    });
            }
            return image;
        }
    }
}