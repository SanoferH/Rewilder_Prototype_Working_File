using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace UIMetaballs.Runtime
{
    [RequireComponent(typeof(CanvasRenderer))]
    [AddComponentMenu("UIMetaballs/Metaball Panel")]
    [ExecuteInEditMode] 
    public class MetaballPanel : RawImage
    {
        [Tooltip("Smoothness of the metaballs edges")] 
        [HideInInspector] [SerializeField]
        [Range(0.00f, 0.04f)] float _antiAliasing = 0.0012f;
        public float AntiAliasing
        {
            get => _antiAliasing;
            set => _antiAliasing = value;
        }
        [HideInInspector] [SerializeField]
        [Range(0.01f, 100.0f)] float _colorBlending = 4.6f;
        public float ColorBlending
        {
            get => _colorBlending;
            set => _colorBlending = value;
        }
        [Tooltip("Activating this option will make the colors add correctly instead of an object ordered color mode. REMEMBER: THIS IS A HEAVY OPTION AND MAY DECREASE THE PERFORMANCE SEVERELY")] 
        [HideInInspector] [SerializeField]
        bool _correctColoring = true;
        public bool CorrectColoring
        {
            get => _correctColoring;
            set => _correctColoring = value;
        }
        [HideInInspector] [SerializeField] 
        Color _backgroundColor = new Color(0,0,0, 1);
        public Color BackgroundColor
        {
            get => _backgroundColor;
            set => _backgroundColor = value;
        }
        [HideInInspector] [SerializeField] 
        bool _useOutline = false;
        public bool UseOutline
        {
            get => _useOutline;
            set => _useOutline = value;
        }
        [HideInInspector] [SerializeField] 
        Color _outlineColor = new Color(1,1,1, 1);
        public Color OutlineColor
        {
            get => _outlineColor;
            set => _outlineColor = value;
        }
        [HideInInspector] [SerializeField] 
        Vector2Int _resolution = new Vector2Int(1920, 1080);
        public Vector2Int Resolution
        {
            get => _resolution;
            set => _resolution = value;
        }

        [Tooltip("Use parent canvas size (If you have a canvas that is set to fill the screen, it will utilize the screen size, ex: 1920x1080)")] [HideInInspector] [SerializeField] private bool _useCanvasSize = true;
        
        [Tooltip("Divide the canvas resolution")]
        [Range(1, 20)] [HideInInspector] [SerializeField] public float _resolutionDivider = 1;
        [Tooltip("Multiply the canvas resolution")]
        [Range(1, 20)] [HideInInspector] [SerializeField] public float _resolutionMultiplier = 1;

        UIMetaball[] _components;
        MetaballStruct[] _data;
        ComputeBuffer _buffer;
        [SerializeField] CustomRenderTexture _metaballTexture;
        Material _shader;
        int _count = 0;

        static readonly int Res = Shader.PropertyToID("Resolution");
        static readonly int BGColor = Shader.PropertyToID("BGColor");
        static readonly int OColor = Shader.PropertyToID("OutlineColor");
        static readonly int MetaballBuffer = Shader.PropertyToID("MetaballBuffer");
        static readonly int UICount = Shader.PropertyToID("UICount");
        static readonly int Correctcoloring  = Shader.PropertyToID("CorrectColoring");


        protected override void OnEnable()
        {
            base.OnEnable();
            
            Initialize();
        }
        
        public void Initialize()
        {
            // BUG - Enabling and disabling alot of time creates alot of unused instances of this script, Resources.UnloadUnusedAssets() fixes this.
            Resources.UnloadUnusedAssets();
            
            InitializeTexture();
            RebuildData();
        }

        public void InitializeTexture()
        {
            if(_shader == null)
                _shader = new Material(Shader.Find("UIMetaballs/MetaballRendering"));

            // Using CustomRenderTexture for custom resolutions
            if (_useCanvasSize)
            {
                _metaballTexture = new CustomRenderTexture((int)(rectTransform.rect.width*_resolutionMultiplier/_resolutionDivider), (int)(rectTransform.rect.height*_resolutionMultiplier/_resolutionDivider));
                _metaballTexture.width = (int)(rectTransform.rect.width*_resolutionMultiplier/_resolutionDivider);
                _metaballTexture.height = (int)(rectTransform.rect.height*_resolutionMultiplier/_resolutionDivider);
            }
            else
            {
                _metaballTexture = new CustomRenderTexture((int)(Resolution.x*_resolutionMultiplier/_resolutionDivider), (int)(Resolution.y*_resolutionMultiplier/_resolutionDivider));
                _metaballTexture.width = (int)(Resolution.x*_resolutionMultiplier/_resolutionDivider);
                _metaballTexture.height = (int)(Resolution.y*_resolutionMultiplier/_resolutionDivider);
            }
            
            _metaballTexture.initializationMaterial = _shader;
            _metaballTexture.initializationMode = CustomRenderTextureUpdateMode.Realtime;
            _metaballTexture.initializationSource = CustomRenderTextureInitializationSource.Material;

            _metaballTexture.material = _shader;

            _metaballTexture.Create();
            
            this.texture = _metaballTexture;
            
            // Also setting the material color and texture to make sure it's not transparent and not getting textures from other shaders
            this.material.color = Color.white;
            this.material.mainTexture = _metaballTexture;
        }
        
        void Update()
        {
            UpdateData();
            UpdateShaderParameters();
        }
        
        public void UpdateData()
        {
            if (_count == 0)
                return;

            for(int i = 0; i < _count; i++)
            {
                _data[i] = _components[i].MBall;
            }

            _buffer.SetData(_data);
        }
        
        public void UpdateShaderParameters()
        {
            _shader.SetInt(Correctcoloring, CorrectColoring ? 1 : 0);
            _shader.SetColor(BGColor, BackgroundColor);
            if(_useOutline)
                _shader.SetColor(OColor, OutlineColor);
            
            if(_useCanvasSize)
                _shader.SetVector(Res, new Vector4(rectTransform.rect.width, rectTransform.rect.height, AntiAliasing, ColorBlending));
            else
                _shader.SetVector(Res, new Vector4(Resolution.x, Resolution.y, AntiAliasing, ColorBlending));
            
            _shader.SetInt(UICount, _count);
        }

        public void RebuildData()
        {
            _components = GetComponentsInChildren<UIMetaball>();
            _count = _components.Length;

            if (_count == 0)
                return;
            
            CreateBuffer();
            _data = new MetaballStruct[_count];
            
            UpdateData();
            
            if(_shader != null && _buffer != null)
                _shader.SetBuffer(MetaballBuffer, _buffer);
        }

        void CreateBuffer()
        {
            if (_buffer != null)
                _buffer.Dispose();
            
            _buffer = new ComputeBuffer(_count, Marshal.SizeOf(typeof(MetaballStruct)));
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            
            if(_metaballTexture != null)
                _metaballTexture.Release();
            
            if(_buffer != null)
                _buffer.Dispose();
        }

        public void StartInitializationAfterSave()
        {
            StartCoroutine(InitializeSaveCoroutine());
        }

        public IEnumerator InitializeSaveCoroutine()
        {
            yield return new WaitForSeconds(0.01f);
            Initialize();
        }
    }
}
