Shader "Custom/Sky" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" { }
	_Color ("Color Tint", Color) = (1,1,1,1)
	_MainColor ("Main Color", Color) = (1,1,1,0)
	_Distort ("Distort", Color) = (0,0,0,0)
	_GradientFade ("Fade", Float) = 1.5
	_GradientOffset ("Gradient offset", Float) = 0
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

		// Set globally from ThemeManager.cs (SetShaderStates).
		float4 _SkyGradientTopColor;
		float4 _SkyGradientBottomColor;
		float _SkyGradientOffset;

		struct appdata { float4 vertex : POSITION; };
		struct v2f { float4 pos : SV_POSITION; fixed4 col : COLOR0; };

		// Exact transcription of the original Custom/Sky GLES program:
		// gradient from bottom to top colour driven by the clip-space screen Y.
		v2f vert(appdata v) {
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			float screenY = o.pos.y / o.pos.w;
			float g = saturate((screenY - _SkyGradientOffset) / (1.0 - _SkyGradientOffset));
			o.col = fixed4(lerp(_SkyGradientBottomColor.rgb, _SkyGradientTopColor.rgb, g), 1);
			return o;
		}

		fixed4 frag(v2f i) : SV_Target {
			return i.col;
		}
		ENDCG
	}
	}
}
