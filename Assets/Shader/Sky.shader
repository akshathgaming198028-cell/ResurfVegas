Shader "Custom/Sky" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _Color ("Color Tint", Color) = (1,1,1,1)
 _MainColor ("Main Color", Color) = (1,1,1,0)
 _Distort ("Distort", Color) = (0,0,0,0)
}
		SubShader {
		Tags { "Queue"="Background" "RenderType"="Background" }
		LOD 100
		ZWrite Off
		Cull Off
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			float4 _SkyGradientTopColor;
			float4 _SkyGradientBottomColor;
			float _SkyGradientOffset;
			struct appdata { float4 vertex : POSITION; };
			struct v2f { float4 pos : SV_POSITION; float dirY : TEXCOORD0; };
			v2f vert(appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				float3 wp = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.dirY = normalize(wp - _WorldSpaceCameraPos).y;
				return o;
			}
			fixed4 frag(v2f i) : SV_Target {
				float g = saturate(i.dirY * 1.5 + _SkyGradientOffset);
				return fixed4(lerp(_SkyGradientBottomColor.rgb, _SkyGradientTopColor.rgb, g), 1);
			}
			ENDCG
		}
	}
}
