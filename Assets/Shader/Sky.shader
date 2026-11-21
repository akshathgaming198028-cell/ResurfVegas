Shader "Custom/Sky" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" { }
	_Color ("Color Tint", Color) = (1,1,1,1)
	_MainColor ("Main Color", Color) = (1,1,1,0)
	_Distort ("Distort", Color) = (0,0,0,0)
	_GradientFade ("Fade", Float) = 1.5
	_GradientOffset ("Gradient offset", Float) = 0
	_HorizonColor ("Horizon band colour (measured)", Color) = (0.569,0.098,0.239,1)
	_HorizonWidth ("Horizon band width", Float) = 0.12
	_HorizonStrength ("Horizon band strength", Float) = 1
}
	SubShader {
	Tags { "QUEUE"="Background+2000" "RenderType"="Opaque" }
	LOD 100
	ZWrite Off
	Cull Off
	Pass {
		Tags { "QUEUE"="Background+2000" "RenderType"="Opaque" }
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"

		// Set globally from ThemeManager.cs (theme colours).
		float4 _SkyGradientTopColor;
		float4 _FogSilhouetteColor;
		float _SkyGradientOffset;
		float _GradientFade;
		float _GradientOffset;
		float4 _HorizonColor;
		float _HorizonWidth;
		float _HorizonStrength;

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
			// Measured from the original screenshot: deep purple at the top (theme top colour),
			// violet fog-silhouette colour below it, and a pink-red band at the horizon.
			float g = saturate(i.dirY * _GradientFade + _SkyGradientOffset + _GradientOffset);
			float3 col = lerp(_FogSilhouetteColor.rgb, _SkyGradientTopColor.rgb, g);
			float band = 1.0 - smoothstep(0.0, _HorizonWidth, abs(i.dirY));
			col = lerp(col, _HorizonColor.rgb, band * _HorizonStrength);
			return fixed4(col, 1);
		}
		ENDCG
	}
	}
}
