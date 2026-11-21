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
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			float4 _MainColor;
			float4 _Distort;
			struct appdata { float4 vertex : POSITION; float2 texcoord : TEXCOORD0; fixed4 color : COLOR; };
			struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; fixed4 color : COLOR; };
			float4 _SkyGradientTopColor;
			float4 _SkyGradientBottomColor;
			float _SkyGradientOffset;

			v2f vert(appdata v) {
				v2f o;
				float4 wp = mul(unity_ObjectToWorld, v.vertex);
				float3 rel = wp.xyz - _WorldSpaceCameraPos;
				float dd = rel.z * rel.z;
				wp.x += _Distort.x * dd;
				wp.y += _Distort.y * dd;
				o.pos = mul(UNITY_MATRIX_VP, wp);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.color = v.color;
				return o;
			}
			fixed4 frag(v2f i) : SV_Target { float g = saturate(i.uv.y + _SkyGradientOffset); fixed4 grad = lerp(_SkyGradientBottomColor, _SkyGradientTopColor, g); fixed4 t = tex2D(_MainTex, i.uv); return fixed4(lerp(grad.rgb, t.rgb * grad.rgb, t.a), 1); }
			ENDCG
		}
	}
}
