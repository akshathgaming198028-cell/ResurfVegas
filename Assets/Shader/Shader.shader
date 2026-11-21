Shader "Standard" {
Properties {
 _Color ("Color", Color) = (1,1,1,1)
 _MainTex ("Albedo", 2D) = "white" { }
 _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
 _Glossiness ("Smoothness", Range(0,1)) = 0.5
[Gamma]  _Metallic ("Metallic", Range(0,1)) = 0
 _MetallicGlossMap ("Metallic", 2D) = "white" { }
 _BumpScale ("Scale", Float) = 1
 _BumpMap ("Normal Map", 2D) = "bump" { }
 _Parallax ("Height Scale", Range(0.005,0.08)) = 0.02
 _ParallaxMap ("Height Map", 2D) = "black" { }
 _OcclusionStrength ("Strength", Range(0,1)) = 1
 _OcclusionMap ("Occlusion", 2D) = "white" { }
 _EmissionColor ("Color", Color) = (0,0,0,1)
 _EmissionMap ("Emission", 2D) = "white" { }
 _DetailMask ("Detail Mask", 2D) = "white" { }
 _DetailAlbedoMap ("Detail Albedo x2", 2D) = "grey" { }
 _DetailNormalMapScale ("Scale", Float) = 1
 _DetailNormalMap ("Normal Map", 2D) = "bump" { }
[Enum(UV0,0,UV1,1)]  _UVSec ("UV Set for secondary textures", Float) = 0
[HideInInspector]  _Mode ("__mode", Float) = 0
[HideInInspector]  _SrcBlend ("__src", Float) = 1
[HideInInspector]  _DstBlend ("__dst", Float) = 0
[HideInInspector]  _ZWrite ("__zw", Float) = 1
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
			struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; fixed4 color : COLOR; };
			v2f vert(appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.color = v.color;
				return o;
			}
			fixed4 frag(v2f i) : SV_Target {
				return tex2D(_MainTex, i.uv) * _Color * i.color;
			}
			ENDCG
		}
	}
}
