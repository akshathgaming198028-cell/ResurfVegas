Shader "Hidden/Unlit/Transparent Masked 2" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _Color ("Color Tint", Color) = (1,1,1,1)
 _MainColor ("Main Color", Color) = (1,1,1,0)
 _Distort ("Distort", Color) = (0,0,0,0)
}
		SubShader {
		Tags { "Queue"="AlphaTest" "RenderType"="TransparentCutout" }
		LOD 100
		ZWrite On
		Cull Off
		Offset -1, -1
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			float4 _MainColor;
			float4 _Distort;
			struct appdata { float4 vertex : POSITION; float2 texcoord : TEXCOORD0; fixed4 color : COLOR; };
			struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; fixed4 color : COLOR; };

			v2f vert(appdata v) {
				v2f o;
				float4 wp = mul(unity_ObjectToWorld, v.vertex);
				float3 rel = wp.xyz - _WorldSpaceCameraPos;
				float dd = rel.z * rel.z;
				wp.x += _Distort.x * dd * 0.35;
				wp.y += _Distort.y * dd;
				o.pos = mul(UNITY_MATRIX_VP, wp);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.color = v.color;
				return o;
			}
			fixed4 frag(v2f i) : SV_Target { fixed4 t = tex2D(_MainTex, i.uv) * _Color * i.color; clip(t.a - 0.5); return fixed4(t.rgb, 1); }
			ENDCG
		}
	}
}
