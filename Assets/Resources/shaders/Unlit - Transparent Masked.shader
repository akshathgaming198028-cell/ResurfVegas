Shader "Unlit/Transparent Masked" {
Properties {
 _MainTex ("Base (RGB), Alpha (A)", 2D) = "black" { }
 _Mask ("Alpha (A)", 2D) = "white" { }
}
SubShader {
 LOD 200
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  Offset -1, -1

  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  sampler2D _Mask;

  struct appdata {
    float4 vertex : POSITION;
    float2 texcoord : TEXCOORD0;
    float2 texcoord1 : TEXCOORD1;
    fixed4 color : COLOR;
  };

  struct v2f {
    float4 pos : SV_POSITION;
    float2 uv : TEXCOORD0;
    float2 uv1 : TEXCOORD1;
    fixed4 color : COLOR;
  };

  v2f vert(appdata v) {
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = v.texcoord;
    o.uv1 = v.texcoord1;
    o.color = v.color;
    return o;
  }

  fixed4 frag(v2f i) : SV_Target {
    fixed4 c = tex2D(_MainTex, i.uv) * i.color;
    c.a = c.a * tex2D(_Mask, i.uv1).a;
    return c;
  }
  ENDCG
 }
}
}
