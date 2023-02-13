using System.Collections;
using System.Collections.Generic;


namespace AEAsAnimation
{
    public class AEAsAnimationLayerShapePropertyData : AEAsAnimationLayerPropertyData<AEAsAnimationLayerShapeBezierData>
    {
        public List<AEAsAnimationLayerShapePropertyElementData> points = new List<AEAsAnimationLayerShapePropertyElementData>();

        new public int maxFrame
        {
            get
            {
                return points.Count - 1;
            }
        }

        new public AEAsAnimationLayerShapePropertyData Parse(IList rawData)
        {
            var copy = new AEAsAnimationLayerShapePropertyDataParser().Parse(rawData);
            points = copy.points;
            markedKeyFrames = copy.markedKeyFrames;
            expression = copy.expression;
            return this;
        }


        new public static AEAsAnimationLayerShapePropertyData Empty()
        {
            return new AEAsAnimationLayerShapePropertyData
            {
                points = new List<AEAsAnimationLayerShapePropertyElementData>(),
                markedKeyFrames = new List<int>(),
                expression = new AVLayerExpressionData()
            };
        }
    }

    public class AEAsAnimationLayerPropertyData<T>
    {
        public List<AEAsAnimationLayerPropertyElementData<T>> elements = new List<AEAsAnimationLayerPropertyElementData<T>>();
        public List<int> markedKeyFrames;
        public AVLayerExpressionData expression;
        
        public int maxFrame
        {
            get
            {
                return elements.Count - 1;
            }
        }

        public int startKeyFrame
        {
            get
            {
                int startKeyFrame = -1;
                foreach (var keyFrame in markedKeyFrames)
                {
                    if (startKeyFrame == -1
                        || keyFrame < startKeyFrame)
                    {
                        startKeyFrame = keyFrame;
                    }
                }

                return startKeyFrame;
            }
        }

        public int endKeyFrame
        {
            get
            {
                int endKeyFrame = -1;
                foreach (var keyFrame in markedKeyFrames)
                {
                    if (endKeyFrame == -1
                        || keyFrame > endKeyFrame)
                    {
                        endKeyFrame = keyFrame;
                    }
                }

                return endKeyFrame;
            }
        }

        public AEAsAnimationLayerPropertyData<T> Parse(IList rawData)
        {
            var copy = new AEAsAnimationLayerPropertyDataParser().Parse<T>(rawData);
            elements = copy.elements;
            markedKeyFrames = copy.markedKeyFrames;
            expression = copy.expression;
            return this;
        }


        public static AEAsAnimationLayerPropertyData<T> Empty()
        {
            return new AEAsAnimationLayerPropertyData<T>
            {
                elements = new List<AEAsAnimationLayerPropertyElementData<T>>(),
                markedKeyFrames = new List<int>(),
                expression = new AVLayerExpressionData()
            };
        }
    }


    public class AEAsAnimationLayerShapePropertyElementData
    {
        public int frame;
        public List<AEAsAnimationLayerShapeBezierData> args = new List<AEAsAnimationLayerShapeBezierData>();
    }

    public class AEAsAnimationLayerPropertyElementData<T>
    {
        public int frame;
        public List<T> args = new List<T>();
    }

    public class AEAsAnimationLayerShapeBezierData
    {
        public List<float> origin;
        public List<float> inOffset;
        public List<float> outOffset;
    }

    public class AVLayerExpressionData
    {
        public string raw = "";

        public string LoopInTag = "loopIn(";
        public string LoopOutTag = "loopOut(";

        private bool _hasLoopIn = false;
        private bool _hasLoopOut = false;
        public bool hasLoopIn
        {
            get
            {
                return _hasLoopIn;
            }
        }

        public bool hasLoopOut
        {
            get
            {
                return _hasLoopOut;
            }
        }

        public AVLayerExpressionData Parse()
        {
            var loopInIndex = raw.IndexOf(LoopInTag);
            if (loopInIndex >= 0)
            {
                var subStr = raw.Substring(loopInIndex);
                var loopInIndexEnd = subStr.IndexOf(")");
                var argStrings = raw.Substring(
                    loopInIndex,
                    loopInIndexEnd - loopInIndex).Split(",".ToCharArray());
                _hasLoopIn = true;
            }

            var loopOutIndex = raw.IndexOf(LoopOutTag);
            if (loopOutIndex >= 0)
            {
                var subStr = raw.Substring(loopOutIndex);
                var loopOutIndexEnd = subStr.IndexOf(")");
                var argStrings = raw.Substring(
                    loopOutIndex,
                    loopOutIndexEnd - loopOutIndex).Split(",".ToCharArray());
                _hasLoopOut = true;
            }

            return this;
        }
    }


    public class AEAsAnimationLayerPropertyDataParser
    {
        public AEAsAnimationLayerPropertyData<T> Parse<T>(IList rawData)
        {
            var result = new AEAsAnimationLayerPropertyData<T>();

            var frameValues = (IList)rawData[0];
            List<AEAsAnimationLayerPropertyElementData<T>> elements = new List<AEAsAnimationLayerPropertyElementData<T>>();
            foreach (var frameValue in frameValues)
            {
                var valueBundle = (IList)frameValue;
                var frame = System.Convert.ToInt32(valueBundle[0]);
                var valueList = (IList)valueBundle[1];

                var args = new List<T>();
                foreach (var arg in valueList)
                {
                    args.Add(Convert<T>(arg));
                }

                elements.Add(new AEAsAnimationLayerPropertyElementData<T>
                {
                    frame = frame,
                    args = args
                });
            }
            result.elements = elements;

            var frames = new List<int>();
            foreach (var frame in (IList)rawData[1])
            {
                frames.Add(System.Convert.ToInt32(frame));
            }
            result.markedKeyFrames = frames;

            var expression = System.Convert.ToString(rawData[2]);
            result.expression = new AVLayerExpressionData
            {
                raw = expression
            }.Parse();

            return result;
        }

        public static T Convert<T>(object target)
        {
            if (typeof(T) == typeof(int)) return (T)(object)System.Convert.ToInt32(target);
            if (typeof(T) == typeof(float)) return (T)(object)System.Convert.ToSingle(target);
            if (typeof(T) == typeof(string)) return (T)(object)System.Convert.ToString(target);
            return (T)target;
        }
    }


    public class AEAsAnimationLayerShapePropertyDataParser
    {
        public AEAsAnimationLayerShapePropertyData Parse(IList rawData)
        {
            var result = new AEAsAnimationLayerShapePropertyData();

            var frameValues = (IList)rawData[0];
            List<AEAsAnimationLayerShapePropertyElementData> points = new List<AEAsAnimationLayerShapePropertyElementData>();
            foreach (var frameValue in frameValues)
            {
                var valueBundle = (IList)frameValue;
                var frame = System.Convert.ToInt32(valueBundle[0]);
                var valueList = (IList)valueBundle[1];

                var vertices = (IList)valueList[0];
                var inPoints = (IList)valueList[1];
                var outPoints = (IList)valueList[2];

                var args = new List<AEAsAnimationLayerShapeBezierData>();
                int vertexIndex = 0;
                foreach (IList vertex in vertices)
                {
                    var arg = new AEAsAnimationLayerShapeBezierData();

                    arg.origin = new List<float>
                    {
                        float.Parse((string)vertex[0]),
                        float.Parse((string)vertex[1])
                    };

                    var inPoint = (IList)inPoints[vertexIndex];
                    arg.inOffset = new List<float>
                    {
                        float.Parse((string)inPoint[0]),
                        float.Parse((string)inPoint[1])
                    };

                    var outPoint = (IList)outPoints[vertexIndex];
                    arg.outOffset = new List<float>
                    {
                        float.Parse((string)outPoint[0]),
                        float.Parse((string)outPoint[1])
                    };

                    args.Add(arg);

                    vertexIndex++;
                }

                points.Add(new AEAsAnimationLayerShapePropertyElementData
                {
                    frame = frame,
                    args = args
                });
            }
            result.points = points;

            var frames = new List<int>();
            foreach (var frame in (IList)rawData[1])
            {
                frames.Add(System.Convert.ToInt32(frame));
            }
            result.markedKeyFrames = frames;

            var expression = System.Convert.ToString(rawData[2]);
            result.expression = new AVLayerExpressionData
            {
                raw = expression
            }.Parse();

            return result;
        }
    }
}