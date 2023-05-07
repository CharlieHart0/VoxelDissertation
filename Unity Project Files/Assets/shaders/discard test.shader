Shader "Custom/HZB-OC Shader" {
    Properties{
        _CamDepthTex ("Camera Depth Texture", 2D) = "white" {}
    }

        SubShader{
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

                sampler2D _CamDepthTex;

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.pos = ComputeScreenPos(o.vertex);
                    return o;
                }

                float4 frag(v2f i) : SV_Target{
                    float fragDepth = Linear01Depth(i.vertex.z/ i.vertex.w); // the depth value of the fragment
                    float2 offset = float2(0,0);
                    float preCalculatedDepth = tex2D(_CamDepthTex, (i.pos.xy) / i.pos.w).r; // depth value of pre calculated render

                    float4 colr = float4(0,0,0,0);
                    return float4(preCalculatedDepth,0,fragDepth,0);
                    if(preCalculatedDepth == fragDepth){ // if pre calc depth is in front of this fragment
                       colr = float4(1,0,0,0);
                    }
                    else{
                        colr = float4(0,0,1,0);
                    }

                     return colr;
                   
                }
                ENDCG
            }
        }
        
}