using System.Collections.Generic;

namespace AEAsAnimation
{
    public class PositionFrameProcessor : FrameProcessor<float>
    {
        private List<float> _defaultValues = null;
        private float _defaultValue = 0;
        public PositionFrameProcessor(
            AEAsAnimationLayerPropertyData<float> data,
            float defaultValue = 0)
        {
            _defaultValue = defaultValue;
            base.SetUp(data);
        }

        public PositionFrameProcessor(
            AEAsAnimationLayerPropertyData<float> data,
            List<float> defaultValues)
        {
            _defaultValues = defaultValues;
            base.SetUp(data);
        }

        public float x
        {
            get
            {
                if (isInvalid 
                    || _data.maxFrame < 0) return GetDefaultValue(0);
                return _data.elements[_currentFrame].args[0];
            }
        }
        public float y
        {
            get
            {
                if (isInvalid
                    || _data.maxFrame < 0) return GetDefaultValue(1);
                return _data.elements[_currentFrame].args[1];
            }
        }
        public float z
        {
            get
            {
                if (isInvalid
                    || _data.maxFrame < 0) return GetDefaultValue(2);
                return _data.elements[_currentFrame].args[2];
            }
        }

        private float GetDefaultValue(int index)
        {
            if (_defaultValues != null
                && index < _defaultValues.Count) return _defaultValues[index];

            return _defaultValue;
        }
    }

    public class SizeFrameProcessor : FrameProcessor<float>
    {
        private List<float> _defaultValues = null;
        private float _defaultValue = 0;
        public SizeFrameProcessor(
            AEAsAnimationLayerPropertyData<float> data,
            float defaultValue = 0)
        {
            _defaultValue = defaultValue;
            base.SetUp(data);
        }

        public SizeFrameProcessor(
            AEAsAnimationLayerPropertyData<float> data,
            List<float> defaultValues)
        {
            _defaultValues = defaultValues;
            base.SetUp(data);
        }


        public float x
        {
            get
            {
                if (isInvalid
                    || _data.maxFrame < 0) return GetDefaultValue(0);
                return _data.elements[_currentFrame].args[0];
            }
        }
        public float y
        {
            get
            {
                if (isInvalid
                    || _data.maxFrame < 0) return GetDefaultValue(1);
                return _data.elements[_currentFrame].args[1];
            }
        }
        public float z
        {
            get
            {
                if (isInvalid
                    || _data.maxFrame < 0) return GetDefaultValue(2);
                return _data.elements[_currentFrame].args[2];
            }
        }

        private float GetDefaultValue(int index)
        {
            if (_defaultValues != null
                && index < _defaultValues.Count) return _defaultValues[index];

            return _defaultValue;
        }
    }

    public class AngleFrameProcessor : FrameProcessor<float>
    {
        private List<float> _defaultValues = null;
        private float _defaultValue = 0;
        public AngleFrameProcessor(
            AEAsAnimationLayerPropertyData<float> data,
            float defaultValue = 0)
        {
            _defaultValue = defaultValue;
            base.SetUp(data);
        }

        public AngleFrameProcessor(
            AEAsAnimationLayerPropertyData<float> data,
            List<float> defaultValues)
        {
            _defaultValues = defaultValues;
            base.SetUp(data);
        }

        public float angle
        {
            get
            {
                if (isInvalid
                    || _data.maxFrame < 0) return GetDefaultValue(0);
                return _data.elements[_currentFrame].args[0];
            }
        }

        private float GetDefaultValue(int index)
        {
            if (_defaultValues != null
                && index < _defaultValues.Count) return _defaultValues[index];

            return _defaultValue;
        }
    }

    public class OpacityFrameProcessor : FrameProcessor<float>
    {
        private List<float> _defaultValues = null;
        private float _defaultValue = 0;
        public OpacityFrameProcessor(
            AEAsAnimationLayerPropertyData<float> data,
            float defaultValue = 0)
        {
            _defaultValue = defaultValue;
            base.SetUp(data);
        }

        public OpacityFrameProcessor(
            AEAsAnimationLayerPropertyData<float> data,
            List<float> defaultValues)
        {
            _defaultValues = defaultValues;
            base.SetUp(data);
        }

        public float opacity
        {
            get
            {
                if (isInvalid
                    || _data.maxFrame < 0) return GetDefaultValue(0);
                return _data.elements[_currentFrame].args[0];
            }
        }

        private float GetDefaultValue(int index)
        {
            if (_defaultValues != null
                && index < _defaultValues.Count) return _defaultValues[index];

            return _defaultValue;
        }
    }

    public class ColorFrameProcessor : FrameProcessor<float>
    {
        private List<float> _defaultValues = null;
        private float _defaultValue = 0;
        public ColorFrameProcessor(
            AEAsAnimationLayerPropertyData<float> data,
            float defaultValue = 0)
        {
            _defaultValue = defaultValue;
            base.SetUp(data);
        }

        public ColorFrameProcessor(
            AEAsAnimationLayerPropertyData<float> data,
            List<float> defaultValues)
        {
            _defaultValues = defaultValues;
            base.SetUp(data);
        }

        public float r
        {
            get
            {
                if (isInvalid
                    || _data.maxFrame < 0) return GetDefaultValue(0);
                return _data.elements[_currentFrame].args[0];
            }
        }
        public float g
        {
            get
            {
                if (isInvalid
                    || _data.maxFrame < 0) return GetDefaultValue(1);
                return _data.elements[_currentFrame].args[1];
            }
        }
        public float b
        {
            get
            {
                if (isInvalid
                    || _data.maxFrame < 0) return GetDefaultValue(2);
                return _data.elements[_currentFrame].args[2];
            }
        }

        private float GetDefaultValue(int index)
        {
            if (_defaultValues != null
                && index < _defaultValues.Count) return _defaultValues[index];

            return _defaultValue;
        }
    }




    public class FrameProcessor<T>
    {
        protected AEAsAnimationLayerPropertyData<T> _data;
        protected int _currentFrame;
        protected int _loopStartFrame = -1;
        protected int _loopEndFrame = -1;
        protected int _periodFrame = -1;

        protected void SetUp(AEAsAnimationLayerPropertyData<T> data)
        {
            _data = data;
        }

        public void SetPeriod(int periodFrame)
        {
            _periodFrame = periodFrame;
        }

        public void GoNextFrame()
        {
            GoToFrame(_currentFrame + 1);
        }

        public void GoToFrame(int frame, int periodFrame = -1)
        {
            if (isInvalid) return;

            if (periodFrame >= 0)
            {
                _periodFrame = periodFrame;
            }

            var nextFrame = frame;
            if (_data.expression != null && _data.expression.hasLoopOut)
            {
                var loopDuration = _data.endKeyFrame - _data.startKeyFrame;
                if (loopDuration > 0)
                {
                    while (nextFrame > _data.endKeyFrame)
                    {
                        nextFrame -= loopDuration;
                    }
                    if (nextFrame < 0) nextFrame = 0;
                }
            }

            var maxFrame = _loopEndFrame >= 0 ? _loopEndFrame : _data.maxFrame;
            if (_periodFrame >= 0 && _periodFrame < maxFrame)
            {
                maxFrame = _periodFrame;
            }
            if (nextFrame <= maxFrame)
            {
                _currentFrame = nextFrame;
            }
            else
            {
                bool isLooping = _loopStartFrame >= 0;
                if (isLooping)
                {
                    _currentFrame = _loopStartFrame;
                }
                else
                {
                    _currentFrame = maxFrame;
                }
            }
        }

        public void SetLoop()
        {
            _loopStartFrame = 0;
        }

        public void SetLoop(
            int from,
            int to)
        {
            _loopStartFrame = from;
            _loopEndFrame = to;
        }

        public void Restart()
        {
            _currentFrame = 0;
        }

        public bool isInvalid
        {
            get
            {
                return _data == null;
            }
        }

        public int currentFrame
        {
            get
            {
                return _currentFrame;
            }
        }
    }


    public class MaskShapeFrameProcessor
    {
        protected AEAsAnimationLayerShapePropertyData _data;
        protected int _currentFrame;
        protected int _loopStartFrame = -1;
        protected int _loopEndFrame = -1;
        protected int _periodFrame = -1;

        public MaskShapeFrameProcessor(AEAsAnimationLayerShapePropertyData data)
        {
            _data = data;
        }

        public void SetPeriod(int periodFrame)
        {
            _periodFrame = periodFrame;
        }
        
        public void GoNextFrame()
        {
            GoToFrame(_currentFrame + 1);
        }

        public void GoToFrame(int frame, int periodFrame = -1)
        {
            if (isInvalid) return;

            if (periodFrame >= 0)
            {
                _periodFrame = periodFrame;
            }

            var nextFrame = frame;
            if (_data.expression != null && _data.expression.hasLoopOut)
            {
                var loopDuration = _data.endKeyFrame - _data.startKeyFrame;
                if (loopDuration > 0)
                {
                    while (nextFrame > _data.endKeyFrame)
                    {
                        nextFrame -= loopDuration;
                    }
                    if (nextFrame < 0) nextFrame = 0;
                }
            }

            var maxFrame = _loopEndFrame >= 0 ? _loopEndFrame : _data.maxFrame;
            if (_periodFrame >= 0 && _periodFrame < maxFrame)
            {
                maxFrame = _periodFrame;
            }
            if (nextFrame <= maxFrame)
            {
                _currentFrame = nextFrame;
            }
            else
            {
                bool isLooping = _loopStartFrame >= 0;
                if (isLooping)
                {
                    _currentFrame = _loopStartFrame;
                }
                else
                {
                    _currentFrame = maxFrame;
                }
            }
        }

        public void SetLoop()
        {
            _loopStartFrame = 0;
        }

        public void SetLoop(
            int from,
            int to)
        {
            _loopStartFrame = from;
            _loopEndFrame = to;
        }

        public void Restart()
        {
            _currentFrame = 0;
        }

        public bool isInvalid
        {
            get
            {
                return _data == null;
            }
        }

        public int currentFrame
        {
            get
            {
                return _currentFrame;
            }
        }

        public List<AEAsAnimationLayerShapeBezierData> shape
        {
            get
            {
                return _data.points[_currentFrame].args;
            }
        }
    }
}