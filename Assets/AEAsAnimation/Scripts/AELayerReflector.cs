using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AEAsAnimation
{
    public class AELayerReflector
    {
        public class TransformData
        {
            public Vector3 position;
            public Vector3 scale;
            public Quaternion rotation;
            public Vector2 size;
            public Vector2 pivot;
        }

        public class ColorData
        {
            public Color color;
            public Color overlayColor;
        }

        public class MaskData
        {
            public AEShapeComposer composer;
        }
        
        public class MetaData
        {
            public string name = "";
            public AEItemsAccessor accessor;
            public string buttonTag = "";
            public string imageFilePath = "";
            public bool overrideSize = false;
            public CompositionData child = null;
            public AVLayerColorOverlaysData colorOverlays = null;

            public bool hasOverlay
            {
                get
                {
                    return colorOverlays != null
                        || accessor.GetGlobalColorData() != null;
                }
            }

            public bool isButton
            {
                get
                {
                    return buttonTag != "";
                }
            }
        }

        public class FrameCommandData
        {
            public enum Type
            {
                None = 0,
                GotoFrame,
                Restart,
                Loop,
                Stop,
                Activate,
                Deactivate,
            }

            public Type command;
            public int gotoFrame = -1;
            public int periodFrame = -1;
            public int loopFrom = -1;
            public int loopTo = -1;
        }

        public delegate void TransformDataEvent(TransformData data);
        public delegate void ColorDataEvent(ColorData data);
        public delegate void MaskDataEvent(MaskData data);
        public delegate void FrameCommandDataEvent(FrameCommandData data);
        public event TransformDataEvent OnTransformUpdated;
        public event ColorDataEvent OnColorUpdated;
        public event MaskDataEvent OnMaskUpdated;
        public event FrameCommandDataEvent OnFrameCommandOrdered;

        private MetaData _metaData = null;

        public AELayerReflector(MetaData data)
        {
            _metaData = data;
        }

        public void UpdateTransform(TransformData data)
        {
            if (OnTransformUpdated != null) OnTransformUpdated(data);
        }
        public void UpdateColor(ColorData data)
        {
            if (OnColorUpdated != null) OnColorUpdated(data);
        }
        public void UpdateMask(MaskData data)
        {
            if (OnMaskUpdated != null) OnMaskUpdated(data);
        }

        public void OrderFrameCommand(FrameCommandData data)
        {
            if (OnFrameCommandOrdered != null) OnFrameCommandOrdered(data);
        }

        public MetaData metaData
        {
            get
            {
                return _metaData;
            }
        }
    }
}