Shader "Custom/HZB-OC Shader" {
    Properties{
    }

        SubShader{
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
            Pass {
                
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f {
                    float4 pos : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _CameraDepthTexture;

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.pos = ComputeScreenPos(o.vertex);
                    return o;
                }

                float4 frag(v2f i) : SV_Target{
                    float2 uv = i.pos.xy / i.pos.w;
                    // calculate depth from near plane to far plane at screen space position. might need to remove the linear01depth() if a non linear mapping will work better
                    float depth = Linear01Depth( 
                        UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, uv)));

                   
                return float4(depth, 0,0,0);
                }
                ENDCG
            }
        }
        
}