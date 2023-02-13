using System.Collections.Generic;

using UnityEngine;
using Unity.VectorGraphics;

namespace AEAsAnimation
{
    public class AEShapeComposer
    {
        private Scene _scene = null;
        private VectorUtils.TessellationOptions _option;

        private float _width;
        private float _height;

        public AEShapeComposer(
            List<AEAsAnimationLayerShapeBezierData> shape,
            float width,
            float height,
            bool inverted = false,
            float pixelOffset = 0,
            float masRadius = 1000
            )
        {
            _width = width;
            _height = height;

            var scene = new Scene();

            var segmentList = new List<BezierPathSegment>();

            var edges = new List<Vector2> {
                new Vector2(0, 0),
                new Vector2(0, -1 * (_height +  + 0.0001f)),
                new Vector2(_width + 0.0001f, -1 * (_height +  + 0.0001f)),
                new Vector2(_width + 0.0001f, 0)
            };

            if (inverted)
            {
                segmentList.Add(new BezierPathSegment
                {
                    P0 = edges[0],
                    P1 = edges[0],
                    P2 = edges[1]
                });
                segmentList.Add(new BezierPathSegment
                {
                    P0 = edges[1],
                    P1 = edges[1],
                    P2 = edges[2]
                });
                segmentList.Add(new BezierPathSegment
                {
                    P0 = edges[2],
                    P1 = edges[2],
                    P2 = edges[3]
                });
                segmentList.Add(new BezierPathSegment
                {
                    P0 = edges[3],
                    P1 = edges[3],
                    P2 = edges[0]
                });
                segmentList.Add(new BezierPathSegment
                {
                    P0 = edges[0],
                    P1 = edges[0],
                    P2 =
                        new Vector2(
                            shape[0].inOffset[0],
                            shape[0].inOffset[1] * -1) +
                        new Vector2(
                            shape[0].origin[0],
                            shape[0].origin[1] * -1)
                });
            }
            else
            {
                segmentList.Add(new BezierPathSegment
                {
                    P0 = edges[0],
                    P1 = edges[0],
                    P2 = edges[1]
                });
                segmentList.Add(new BezierPathSegment
                {
                    P0 = edges[1],
                    P1 = edges[1],
                    P2 = edges[2]
                });
                segmentList.Add(new BezierPathSegment
                {
                    P0 = edges[2],
                    P1 = edges[2],
                    P2 = edges[3]
                });
                segmentList.Add(new BezierPathSegment
                {
                    P0 = edges[3],
                    P1 = edges[3],
                    P2 = edges[0]
                });
                segmentList.Add(new BezierPathSegment
                {
                    P0 = edges[0],
                    P1 = edges[0],
                    P2 = edges[1]
                });
                segmentList.Add(new BezierPathSegment
                {
                    P0 = edges[1],
                    P1 = edges[1],
                    P2 = edges[2]
                });
                segmentList.Add(new BezierPathSegment
                {
                    P0 = edges[2],
                    P1 = edges[2],
                    P2 = edges[3]
                });
                segmentList.Add(new BezierPathSegment
                {
                    P0 = edges[3],
                    P1 = edges[3],
                    P2 = edges[0]
                });
                segmentList.Add(new BezierPathSegment
                {
                    P0 = edges[0],
                    P1 = edges[0],
                    P2 =
                        new Vector2(
                            shape[0].inOffset[0],
                            shape[0].inOffset[1] * -1) +
                        new Vector2(
                            shape[0].origin[0],
                            shape[0].origin[1] * -1)
                });
            }

            for (int i = 0; i < shape.Count; i++)
            {
                var segment = new BezierPathSegment();
                int nextIndex = i + 1 < shape.Count ? i + 1 : 0;

                segment.P0 = new Vector2(
                    shape[i].origin[0],
                    shape[i].origin[1] * -1);
                segment.P1 = new Vector2(
                    shape[i].outOffset[0],
                    shape[i].outOffset[1] * -1) + segment.P0;
                segment.P2 =
                    new Vector2(
                        shape[nextIndex].inOffset[0],
                        shape[nextIndex].inOffset[1] * -1) +
                    new Vector2(
                        shape[nextIndex].origin[0],
                        shape[nextIndex].origin[1] * -1);

                segmentList.Add(segment);
            }


            segmentList.Add(new BezierPathSegment
            {
                P0 = new Vector2(
                    shape[0].origin[0],
                    shape[0].origin[1] * -1),
                P1 = new Vector2(
                    shape[0].origin[0],
                    shape[0].origin[1] * -1),
                P2 = edges[0],
            });


            var Contours = new List<BezierContour> {
                new BezierContour()
                {
                    Segments = segmentList.ToArray(),
                    Closed = true
                }
            };
            var raw = new Shape()
            {
                Contours = Contours.ToArray(),
                Fill = new SolidFill
                {
                    Color = Color.white,
                    Mode = FillMode.OddEven
                },
                PathProps = new PathProperties()
                {
                    Corners = PathCorner.Round,
                    Head = PathEnding.Round,
                    Tail = PathEnding.Round,
                    Stroke = new Stroke()
                    {
                        Color = Color.white,
                        Fill = new SolidFill
                        {
                            Color = Color.white,
                            Mode = FillMode.NonZero
                        },
                        HalfThickness = pixelOffset
                    }
                }
            };

            _option = new VectorUtils.TessellationOptions()
            {
                StepDistance = masRadius,
                MaxCordDeviation = 0.05f,
                MaxTanAngleDeviation = 0.05f,
                SamplingStepSize = 0.01f
            };

            scene.Root = new SceneNode()
            {
                Shapes = new List<Shape> { raw }
            };
            _scene = scene;
        }


        public Sprite GetSprite()
        {
            var geoms = VectorUtils.TessellateScene(_scene, _option);
            return VectorUtils.BuildSprite(geoms, 100, VectorUtils.Alignment.SVGOrigin, Vector2.one / 2, 1);
        }



        public Vector2 size
        {
            get
            {
                return new Vector2(_width, _height);
            }
        }
    }
}