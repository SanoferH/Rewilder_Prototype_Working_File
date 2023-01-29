Shader "UIMetaballs/MetaballRendering"
{
    Properties
    {
        
    }
    SubShader
    {
        Lighting Off
        Cull Off

        Pass
        {
            CGPROGRAM
            
            #include "UnityCG.cginc"
            #include "UnityCustomRenderTexture.cginc"

            #pragma vertex InitCustomRenderTextureVertexShader
            #pragma fragment frag
            #pragma target 3.0

            // Shorter smoothstep usage
            #define S(a,b,c) smoothstep(a,b,c)

            // Metaball structure
            struct Metaball
            {
                float4 color;
                float2 position;
                float2 size;
                float blending;
                float4 roundness;
                float angle;
                int round;
                float outlineWidth;
            };
            
            // Buffer containing metaball informations
            StructuredBuffer<Metaball> MetaballBuffer;

            // Panel variables and etc
            int UICount, CorrectColoring;
            float Colorblending;
            float4 Resolution;
            float4 BGColor;
            float4 OutlineColor;

            // SDF Smooth Union
            float metaUnion( float d1, float d2, float k ) {
                float h = clamp( 0.5 + 0.5*(d2-d1)/k, 0.0, 1.0 );
                return lerp( d2, d1, h ) - k*h*(1.0-h); }
            
            // SDF Circle
            float metaCircle( float2 p, float r )
            {
                return length(p) - r;
            }

            // SDF Rounded Box
            float metaBox( in float2 p, in float2 b, in float4 r )
            {
                r.xy = (p.x>0.0)?r.xy : r.zw;
                r.x  = (p.y>0.0)?r.x  : r.y;
                float2 q = abs(p)-b+r.x;
                return min(max(q.x,q.y),0.0) + length(max(q,0.0)) - r.x;
            }

            // 2D Rotation
            float2 rotate(float2 uv, float angle)
            {
                float2x2 rot = float2x2(cos(angle), -sin(angle), sin(angle), cos(angle));

                return mul(rot, uv);
            }

            // Second loop definition for correct coloring
            #define SIZEDEFINITION float2 size = current.size / (Resolution*2); size.x *= Resolution.x/Resolution.y;
            #define ROUNDNESSDEFINITION float4 roundness = current.roundness*100; roundness /= Resolution.y;
            
            fixed4 frag (v2f_init_customrendertexture IN) : SV_Target
            {
                float2 uv = IN.texcoord.xy;
                uv -= 0.5;
                uv.x *= Resolution.x/Resolution.y;

                float SDFs = 1;
                float Outline = 1;
                float4 COLOR = 1, COLOR2 = 1;
                
                for(int j = 0; j < UICount; j++)
                {
                    Metaball current = MetaballBuffer[j];
                    
                    SIZEDEFINITION
                    float2 rotatedPosition = rotate(uv + current.position / Resolution.y, radians(current.angle));

                    ROUNDNESSDEFINITION
                    
                    float sdf = current.round ? metaCircle(rotatedPosition, size) : metaBox(rotatedPosition, size, roundness);
                    //sdf = lerp(metaBox(rotatedPosition, size, roundness), metaCircle(rotatedPosition, size), current.round);
                    //if(current.round == 1)
                    //    sdf = metaCircle(rotatedPosition, size);
                    //else
                    //    sdf = metaBox(rotatedPosition, size, roundness);

                    SDFs = metaUnion(SDFs, sdf, current.blending);
                    Outline = metaUnion(Outline, sdf-current.outlineWidth, current.blending);
                    
                    float lerpValue = saturate(sdf * Resolution.w);
                    COLOR = lerp(current.color, COLOR, lerpValue);
                }

                // Second loop for correct color merging
                if(CorrectColoring == 1)
                    for(int k = UICount; k > -1; k--)
                    {
                        Metaball current = MetaballBuffer[k];
                        
                        SIZEDEFINITION
                        float2 rotatedPosition = rotate(uv + current.position / Resolution.y, radians(current.angle));
                        ROUNDNESSDEFINITION

                        float sdf = current.round ? metaCircle(rotatedPosition, size) : metaBox(rotatedPosition, size, roundness);
                        
                        float lerpValue = saturate(sdf * Resolution.w);
                        COLOR2 = lerp(current.color, COLOR2, lerpValue);
                    }

                // float4 COLOR2 is the color from the correct coloring loop
                // int CorrectColoring (The boolean on the editor) is an int that activates and deactivates it
                float4 FINALColor = COLOR+COLOR2*CorrectColoring;

                // fwidth antialiasing ( resolution independant )
                float aaf = fwidth(SDFs);
                Outline = S(aaf+Resolution.z, 0, Outline);
                float FINALSDF = S(aaf+Resolution.z, 0, SDFs);

                // normal antialiasing ( resolution dependant
                //float FINALSDF = S(texel * (Resolution.z), 0.00, SDFs);

                // Lerp between outline, background color and SDFs
                float4 fColor = lerp(OutlineColor, FINALColor, FINALSDF);
                return lerp(BGColor, fColor, Outline);
            }
            
            ENDCG
        }
    }
}
