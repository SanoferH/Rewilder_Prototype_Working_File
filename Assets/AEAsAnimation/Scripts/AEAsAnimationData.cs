using System.Collections.Generic;


namespace AEAsAnimation
{
    public class CompositionData
    {
        public int id;
        public float width;
        public float height;
        public List<string> layerIds;
        public string metaData;
        public AEAsAnimationLayerPropertyData<string> markers = null;
    }

    public class LayerData
    {
        public string id;
        public string uniqueId;
        public FootageData footage;
        public int compositionId;
        public AVLayerPropertiesData properties = null;
        public int parentCompositionId;

        public int layerIndex;
        public int parentIndex;
        public string name;
        public float width;
        public float height;
        public AVLayerBlendingMode blendMode;
        public string metaData;
        public float inSeconds;
        public float outSeconds;
        public bool isNullLayer;
    }


    public class MetaData
    {
        public string path;
        public string versionString;
        public float fps;
        public bool asLayout;
    }

    public class FootageData
    {
        public string sourceName;
    }

    public class SolidSourceFootageData : FootageData
    {
        public List<float> color;
    }

    public class FileSourceFootageData : FootageData
    {
    }

    public class AVLayerTransformsData
    {
        public AEAsAnimationLayerPropertyData<float> position = null;
        public AEAsAnimationLayerPropertyData<float> rotation = null;
        public AEAsAnimationLayerPropertyData<float> scale = null;
        public AEAsAnimationLayerPropertyData<float> anchor = null;
        public AEAsAnimationLayerPropertyData<float> opacity = null;


        public int totalFrane
        {
            get
            {
                var result = 0;
                if (position != null && result < position.maxFrame) result = position.maxFrame;
                if (rotation != null && result < rotation.maxFrame) result = rotation.maxFrame;
                if (scale != null && result < scale.maxFrame) result = scale.maxFrame;
                if (anchor != null && result < anchor.maxFrame) result = anchor.maxFrame;
                if (opacity != null && result < opacity.maxFrame) result = opacity.maxFrame;
                return result;
            }
        }

        public static AVLayerTransformsData Empty()
        {
            return new AVLayerTransformsData
            {
                position = AEAsAnimationLayerPropertyData<float>.Empty(),
                rotation = AEAsAnimationLayerPropertyData<float>.Empty(),
                scale = AEAsAnimationLayerPropertyData<float>.Empty(),
                anchor = AEAsAnimationLayerPropertyData<float>.Empty(),
                opacity = AEAsAnimationLayerPropertyData<float>.Empty()
            };
        }
    }

    public class GlobalColorData
    {
        public float overlayR = 0;
        public float overlayG = 0;
        public float overlayB = 0;
        public float overlayA = 0;

        public float multiplyR = 1;
        public float multiplyG = 1;
        public float multiplyB = 1;
        public float multiplyA = 1;

        public float opacity = 1;
    }

    public class AVLayerColorOverlaysData
    {
        public AEAsAnimationLayerPropertyData<float> color;
        public AEAsAnimationLayerPropertyData<float> opacity;
    }

    public class AVLayerMaskData
    {
        public AEAsAnimationLayerPropertyData<float> opacity;
        public AEAsAnimationLayerPropertyData<float> offset;
        public AEAsAnimationLayerShapePropertyData shape;
        public bool inverted;
    }

    public class AVLayerMarkerData
    {
        public AEAsAnimationLayerPropertyData<string> marker;
    }

    public class AVLayerPropertiesData
    {
        public AVLayerTransformsData transforms;
        public AVLayerColorOverlaysData colorOverlays;
        public AVLayerMaskData masks;
        public AVLayerMarkerData markers;

        public int totalFrane
        {
            get
            {
                var result = 0;
                if (transforms != null && result < transforms.totalFrane) result = transforms.totalFrane;
                return result;
            }
        }

        public static AVLayerPropertiesData Empty()
        {
            return new AVLayerPropertiesData
            {
                transforms = new AVLayerTransformsData { },
                colorOverlays = null,
                masks = null,
            };
        }
    }

    public enum AVLayerPropertyType
    {
        None = 0,
        Position,
        Rotation,
        Scale,
        Anchor_Point,
        Opacity,
        ColorOverlay,
    }

    public enum AVLayerMaskMode
    {
        None = 0,
        ADD,
        SUBTRACT,
        INTERSECT,
        LIGHTEN,
        DARKEN,
        DIFFERENCE
    }

    public enum AVLayerBlendingMode
    {
        None = 0,
        ADD,
        ALPHA_ADD,
        CLASSIC_COLOR_BURN,
        CLASSIC_COLOR_DODGE,
        CLASSIC_DIFFERENCE,
        COLOR,
        COLOR_BURN,
        COLOR_DODGE,
        DANCING_DISSOLVE,
        DARKEN,
        DARKER_COLOR,
        DIFFERENCE,
        DISSOLVE,
        EXCLUSION,
        HARD_LIGHT,
        HARD_MIX,
        HUE,
        LIGHTEN,
        LIGHTER_COLOR,
        LINEAR_BURN,
        LINEAR_DODGE,
        LINEAR_LIGHT,
        LUMINESCENT_PREMUL,
        LUMINOSITY,
        MULTIPLY,
        NORMAL,
        OVERLAY,
        PIN_LIGHT,
        SATURATION,
        SCREEN,
        SILHOUETE_ALPHA,
        SILHOUETTE_LUMA,
        SOFT_LIGHT,
        STENCIL_ALPHA,
        STENCIL_LUMA,
        SUBTRACT,
        VIVID_LIGHT,
    }
}