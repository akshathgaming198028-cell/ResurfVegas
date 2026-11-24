Shader "Custom/Distorted/Unlit with overlay and Reflection" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _Color ("Color Tint", Color) = (0,0,0,0)
 _ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
 _Cube ("Reflection Cubemap", CUBE) = "_Skybox" { }
}
SubShader {
 Pass {
  // Port of the original GLES program (default render states, sky/fog overlay, cube reflection).
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  float4 _MainTex_ST;
  fixed4 _Color;
  fixed4 _ReflectColor;
  samplerCUBE _Cube;

  // Globals set by ThemeManager.SetShaderStates (not material properties, as in the original).
  float4 _Distort;
  float4 _SkyGradientTopColor;
  float4 _SkyGradientBottomColor;
  float4 _FogSilhouetteColor;
  float _SkyGradientOffset;

  // Sky gradient + fog silhouette overlay, identical to the original (applied after the clip curve).
  fixed4 SkyFogOverlay(float4 p) {
    float screenPosY = p.y / p.w;
    float3 sky = lerp(_SkyGradientBottomColor.rgb, _SkyGradientTopColor.rgb,
                      saturate((screenPosY - _SkyGradientOffset) / (1.0 - _SkyGradientOffset)));
    sky = lerp(sky, _FogSilhouetteColor.rgb, saturate((1100.0 - p.w) / 300.0));
    return fixed4(sky, saturate((800.0 - p.w) / 500.0));
  }
  fixed4 ApplySkyFog(fixed4 c, fixed4 fog) {
    fixed4 o; o.a = c.a; o.rgb = lerp(fog.rgb, c.rgb, fog.a); return o;
  }

  struct appdata { float4 vertex : POSITION; float3 normal : NORMAL; float4 texcoord : TEXCOORD0; };
  struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD2; float3 nw : TEXCOORD0; float3 vw : TEXCOORD1; fixed4 fog : TEXCOORD3; };

  v2f vert(appdata v) {
    v2f o;
    o.vw = mul(unity_ObjectToWorld, v.vertex).xyz - _WorldSpaceCameraPos;
    o.nw = mul(float4(v.normal, 0), unity_WorldToObject).xyz;

    o.pos = UnityObjectToClipPos(v.vertex);
    o.pos.x = o.pos.x + (o.pos.z * o.pos.z) * _Distort.x;
    o.pos.y = o.pos.y + (o.pos.z * o.pos.z) * _Distort.y;

    o.fog = SkyFogOverlay(o.pos);
    o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
    return o;
  }

  fixed4 frag(v2f i) : SV_Target {
    float3 V = normalize(i.vw);
    float3 N = normalize(i.nw);
    fixed4 t = tex2D(_MainTex, i.uv);
    float3 R = V - 2.0 * dot(N, V) * N;
    fixed4 refl = texCUBE(_Cube, R) * _ReflectColor * (1.0 - saturate(-dot(V, N))) * 0.495 * ((t.x + t.z) + t.y);
    fixed4 c = t * _Color + refl;
    return ApplySkyFog(c, i.fog);
  }
  ENDCG
 }
}
}
