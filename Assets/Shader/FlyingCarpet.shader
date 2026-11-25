Shader "Custom/Distorted/FlyingCarpet" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _Color ("Color (RGBA)", Color) = (1,1,1,1)
 _Frequency ("Frequency", Range(0.01,5)) = 4
}
SubShader {
 Pass {
  Cull Off
  // Port of the original GLES program: triangle-wave vertex wobble driven by _Time.x, _Frequency and
  // the vertex colour's red channel, then the clip-space bend. Textured, tinted by _Color.
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  float4 _MainTex_ST;
  fixed4 _Color;
  float _Frequency;
  float4 _Distort; // global, as in the original

  struct appdata { float4 vertex : POSITION; float4 color : COLOR; float4 texcoord : TEXCOORD0; };
  struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

  v2f vert(appdata v) {
    v2f o;
    float intensity = v.color.x;
    float3 p2 = float3(v.vertex.x, v.vertex.z, 0.0);
    float3 t = abs(fract((((_Time.x * float3(50.0, 100.0, 0.0)) * _Frequency) - dot(p2, float3(1.0, 1.0, 1.0))) / 9.42477 + 0.5) * 2.0 - 1.0);
    float3 off = float3(0.0, 0.0, 0.0);
    off.y = intensity * ((((t * t) * (3.0 - 2.0 * t)) * 2.0 - 1.0).y) * 0.3;
    float4 world = float4(v.vertex.xyz + off, v.vertex.w);
    o.pos = UnityObjectToClipPos(world);
    o.pos.x = o.pos.x + (o.pos.z * o.pos.z) * _Distort.x;
    o.pos.y = o.pos.y + (o.pos.z * o.pos.z) * _Distort.y;
    o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
    return o;
  }

  fixed4 frag(v2f i) : SV_Target { return tex2D(_MainTex, i.uv) * _Color; }
  ENDCG
 }
}
}
