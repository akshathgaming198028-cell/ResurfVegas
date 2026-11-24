Shader "Hidden/Unlit/Text 3" {
Properties {
 _Color ("Color Tint", Color) = (1,1,1,1)
 _MainTex ("Alpha (A)", 2D) = "white" { }
}
		SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 100
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			struct appdata { float4 vertex : POSITION; float2 texcoord : TEXCOORD0; fixed4 color : COLOR; };
			float4 _ClipRange0; float4 _ClipArgs0;
			struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; fixed4 color : COLOR; float2 clipc : TEXCOORD1; };
			v2f vert(appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.color = v.color;
				o.clipc = v.vertex.xy * _ClipRange0.zw + _ClipRange0.xy;
				return o;
			}
			fixed4 frag(v2f i) : SV_Target {
				fixed a = tex2D(_MainTex, i.uv).a;
				float2 cf = (float2(1,1) - abs(i.clipc)) * _ClipArgs0.xy; float cA = saturate(min(cf.x, cf.y));
				return fixed4(i.color.rgb * _Color.rgb, a * i.color.a * _Color.a * cA);
			}
			ENDCG
		}
	}
}
