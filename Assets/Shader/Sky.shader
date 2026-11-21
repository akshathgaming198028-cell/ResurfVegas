Shader "Custom/Sky" {
Properties {
 _Color ("Color Tint", Color) = (1,1,1,1)
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _GradientFade ("Fade", Float) = 1
 _GradientOffset ("Gradient offset", Float) = 0
}
		SubShader {
		Tags { "Queue"="Geometry" "RenderType"="Opaque" }
		LOD 100
		ZWrite On
		Cull Back

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			struct appdata { float4 vertex : POSITION; float2 texcoord : TEXCOORD0; fixed4 color : COLOR; };
			struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; fixed4 color : COLOR; };
			v2f vert(appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.color = v.color;
				return o;
			}
			fixed4 frag(v2f i) : SV_Target { fixed4 t = tex2D(_MainTex, i.uv); return fixed4(t.rgb * _Color.rgb * i.color.rgb, 1); }
			ENDCG
		}
	}
}
