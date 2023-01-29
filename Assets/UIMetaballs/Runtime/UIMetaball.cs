using UnityEngine;
using UnityEngine.EventSystems;

namespace UIMetaballs.Runtime
{
    [AddComponentMenu("UIMetaballs/UI Metaball")]
    [ExecuteInEditMode]
    public class UIMetaball : UIBehaviour
    {
        [ColorUsage(true, true)]
        [HideInInspector] [SerializeField]
        Color _color;
        [Tooltip("Set the blending with the next metaball (this setting is order based on the metaball creation)")] [Range(0f, 1.0f)] [HideInInspector] [SerializeField]
        float _blending;
        [Tooltip("Set the outline width")] [Range(0f, 1.0f)] [HideInInspector] [SerializeField]
        float _outlineWidth;
        [HideInInspector] [SerializeField]
        Vector4 _roundness;

        [HideInInspector] [SerializeField] public bool round;

        [HideInInspector] public RectTransform rectTransform;
        [HideInInspector] public MetaballPanel parentPanel;
        // [HideInInspector] public CanvasScaler canvasScaler; - not utilized anymore

        public Color Color
        {
            get => _color;
            set => _color = value;
        }
        public float Blending
        {
            get => _blending;
            set => _blending = value;
        }

        public float OutlineWidth
        {
            get => _outlineWidth;
            set => _outlineWidth = value;
        }
        public Vector4 Roundness
        {
            get => _roundness;
            set => _roundness = value;
        }

        MetaballStruct _mBall;
        public MetaballStruct MBall
        {
            get => _mBall;
        }

        protected override void OnEnable()
        {
            _mBall = new MetaballStruct();
            rectTransform = GetComponent<RectTransform>();
            parentPanel = GetComponentInParent<MetaballPanel>();
            //canvasScaler = GetComponentInParent<CanvasScaler>();
            
            if (parentPanel != null)
                parentPanel.RebuildData();
        }

        void Update()
        {
            // Update metaball structure
            //_mBall = new MetaballStruct() { position = -rectTransform.anchoredPosition, size = rectTransform.rect.size, blending = Blending, color = Color, roundness = Roundness, angle = -rectTransform.eulerAngles.z, round = round ? 1 : 0 };
            _mBall.position = -rectTransform.anchoredPosition;
            _mBall.size = rectTransform.rect.size;
            _mBall.blending = Blending;
            _mBall.outlineWidth = OutlineWidth;
            _mBall.color = Color;
            _mBall.roundness = Roundness;
            _mBall.angle = -rectTransform.eulerAngles.z;
            _mBall.round = round ? 1 : 0;
        }

        protected override void OnDisable()
        {
            if(parentPanel != null)
                parentPanel.RebuildData();
        }
    }
}
