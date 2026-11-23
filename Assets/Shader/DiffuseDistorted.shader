Shader "Custom/Distorted/Unlit" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" { }
	_Color ("Color Tint", Color) = (1,1,1,1)
	_MainColor ("Main Color", Color) = (1,1,1,0)
	_Distort ("Distort", Color) = (0,0,0,0)
	_FogStart ("Fog start distance", Float) = 120
	_FogEnd ("Fog end distance", Float) = 360
	_FogStrength ("Fog strength", Range(0,1)) = 0
}
		SubShader {
		Tags { "Queue"="Geometry" "RenderType"="Opaque" }
		LOD 200
		Pass {
			Tags { "LightMode"="ForwardBase" }
			ZWrite On
			Cull Back
			Offset -1, -1
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			float4 _Distort;

			// Set globally from ThemeManager.cs: the sky gradient the fog blends into.
			float4 _SkyGradientTopColor;
			float4 _SkyGradientBottomColor;
			float _SkyGradientOffset;
			float4 _FogSilhouetteColor;

			float _FogStart;
			float _FogEnd;
			float _FogStrength;

			struct appdata { float4 vertex : POSITION; float3 normal : NORMAL; float2 texcoord : TEXCOORD0; };
			struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; float3 n : TEXCOORD1; float3 fogCol : TEXCOORD2; float fogAmt : TEXCOORD3; };

			v2f vert(appdata v) {
				v2f o;
				float4 wp = mul(unity_ObjectToWorld, v.vertex);
				float3 rel = wp.xyz - _WorldSpaceCameraPos;
				float dd = rel.z * rel.z;
				wp.x += _Distort.x * dd;
				wp.y += _Distort.y * dd;
				o.pos = mul(UNITY_MATRIX_VP, wp);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.n = UnityObjectToWorldNormal(v.normal);

				// Distance haze in the original purple fog silhouette colour, not the orange sky bottom.
				float3 toV = wp.xyz - _WorldSpaceCameraPos;
				float dist = length(toV);
				float dirY = normalize(toV).y;
				o.fogCol = _FogSilhouetteColor.rgb;
				o.fogAmt = saturate((dist - _FogStart) / max(_FogEnd - _FogStart, 0.001)) * _FogStrength;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target {
				fixed4 t = tex2D(_MainTex, i.uv) * _Color;
				float3 n = normalize(i.n);
				float ndl = dot(n, _WorldSpaceLightPos0.xyz) * 0.5 + 0.5;
				fixed3 light = 0.8 + 0.2 * ndl;
				fixed3 lit = t.rgb * light;
				return fixed4(lerp(lit, i.fogCol, i.fogAmt), 1);
			}
			ENDCG
		}
	}
}
