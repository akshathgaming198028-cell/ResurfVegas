Shader "Mobile/Particles/Alpha Blended" {
Properties {
 _MainTex ("Particle Texture", 2D) = "white" { }
}
SubShader {
 Tags { "Queue"="Transparent" "IgnoreProjector"="true" "RenderType"="Transparent" }
 LOD 100
 Pass {
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha

  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  float4 _MainTex_ST;

  struct appdata {
    float4 vertex : POSITION;
    float2 texcoord : TEXCOORD0;
    fixed4 color : COLOR;
  };

  struct v2f {
    float4 pos : SV_POSITION;
    float2 uv : TEXCOORD0;
    fixed4 color : COLOR;
  };

  v2f vert(appdata v) {
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
    o.color = saturate(v.color);
    return o;
  }

  fixed4 frag(v2f i) : SV_Target {
    return tex2D(_MainTex, i.uv) * i.color;
  }
  ENDCG
 }
}
}
