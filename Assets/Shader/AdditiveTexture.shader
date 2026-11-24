Shader "Custom/Additive Texture" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
}
SubShader {
 Tags { "QUEUE"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" }
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  // Port of the original GLES program: out = tex * vertexColor. No bend.
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  float4 _MainTex_ST;

  struct appdata { float4 vertex : POSITION; float4 texcoord : TEXCOORD0; float4 color : COLOR; };
  struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; fixed4 color : COLOR; };

  v2f vert(appdata v) {
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.color = v.color;
    o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
    return o;
  }

  fixed4 frag(v2f i) : SV_Target { return tex2D(_MainTex, i.uv) * i.color; }
  ENDCG
 }
}
}
