Shader "Hidden/Unlit/Premultiplied Colored 1" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _Color ("Color Tint", Color) = (1,1,1,1)
 _MainColor ("Main Color", Color) = (1,1,1,0)
 _Distort ("Distort", Color) = (0,0,0,0)
 _ClipRange0 ("Clip Range", Vector) = (0,0,100000,100000)
 _ClipArgs0 ("Clip Args", Vector) = (1,1,0,0)
}
		SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 100
		ZWrite Off
		Cull Off
		Blend One OneMinusSrcAlpha
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
			struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; fixed4 color : COLOR; float2 wxy : TEXCOORD1; };
			float4 _ClipRange0;
			float4 _ClipArgs0;

			v2f vert(appdata v) {
				v2f o;
				float4 wp = mul(unity_ObjectToWorld, v.vertex);
				o.wxy = wp.xy;
				float3 rel = wp.xyz - _WorldSpaceCameraPos;
				float dd = rel.z * rel.z;
				wp.x += _Distort.x * dd;
				wp.y += _Distort.y * dd;
				o.pos = mul(UNITY_MATRIX_VP, wp);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.color = v.color;
				return o;
			}
			fixed4 frag(v2f i) : SV_Target { fixed4 c = tex2D(_MainTex, i.uv) * _Color * i.color; float2 cp = (i.wxy - _ClipRange0.xy) / _ClipRange0.zw; float2 cf = (float2(1,1) - abs(cp)) * _ClipArgs0.xy; float clipA = saturate(min(cf.x, cf.y)); c *= clipA; return c; }
			ENDCG
		}
	}
}
