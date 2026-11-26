Shader "Custom/Distorted/Alpha Blended" {
Properties {
 _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
 _MainTex ("Particle Texture", 2D) = "white" { }
 _InvFade ("Soft Particles Factor", Range(0.01,3)) = 1
 // Not declared in the original ShaderLab; declared here so the saved value
 // in particle_fog.mat (-0.0005, -0.0003) stays bound to the uniform.
 _Distort ("Distort", Vector) = (0,0,0,0)
}
SubShader {
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  ColorMask RGB

  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #pragma multi_compile_particles
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  float4 _MainTex_ST;
  fixed4 _TintColor;
  float _InvFade;
  float4 _Distort;

  // Set globally by ThemeManager.cs (Shader.SetGlobalColor / SetGlobalFloat).
  fixed4 _SkyGradientTopColor;
  fixed4 _SkyGradientBottomColor;
  fixed4 _FogSilhouetteColor;
  fixed _SkyGradientOffset;

#if defined(SOFTPARTICLES_ON)
  sampler2D_float _CameraDepthTexture;   // _ZBufferParams is a Unity built-in
#endif

  struct appdata {
    float4 vertex : POSITION;
    fixed4 color : COLOR;
    float2 texcoord : TEXCOORD0;
  };

  struct v2f {
    float4 pos : SV_POSITION;
    fixed4 color : COLOR;          // xlv_COLOR
    float2 uv : TEXCOORD0;         // xlv_TEXCOORD0
    fixed4 fog : TEXCOORD1;        // xlv_TEXCOORD1: rgb = sky/silhouette fog colour, a = fog factor
#if defined(SOFTPARTICLES_ON)
    float4 projPos : TEXCOORD2;    // xlv_TEXCOORD2: xy/w = screen uv, z = eye depth, w = screen w
#endif
  };

  v2f vert(appdata v) {
    v2f o;

    // Clip-space bend (same as original): offset x/y by z*z*_Distort, z being clip-space z.
    float4 clip = mul(UNITY_MATRIX_MVP, v.vertex);
    clip.x += clip.z * clip.z * _Distort.x;
    clip.y += clip.z * clip.z * _Distort.y;
    o.pos = clip;

    o.color = v.color;
    o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

    // Fog, evaluated per vertex from the bent clip position (as in the original).
    float skyT = saturate((clip.y / clip.w - _SkyGradientOffset) / (1.0 - _SkyGradientOffset));
    float3 fogRGB = lerp(_SkyGradientBottomColor.rgb, _SkyGradientTopColor.rgb, skyT);
    fogRGB = lerp(fogRGB, _FogSilhouetteColor.rgb, saturate((1100.0 - clip.w) / 300.0));
    float fogFactor = saturate((800.0 - clip.w) / 500.0);
    o.fog = fixed4(fogRGB, fogFactor);

#if defined(SOFTPARTICLES_ON)
    // Screen position for the depth lookup: same as ComputeScreenPos with _ProjectionParams.x flip.
    float4 sp;
    sp.xy = float2(clip.x, clip.y * _ProjectionParams.x) * 0.5 + clip.w * 0.5;
    sp.zw = clip.zw;
    o.projPos = float4(sp.x, sp.y, -mul(UNITY_MATRIX_MV, v.vertex).z, sp.w);
#endif
    return o;
  }

  fixed4 frag(v2f i) : SV_Target {
    fixed4 vc = i.color;

#if defined(SOFTPARTICLES_ON)
    // Soft-particle fade against the camera depth texture (original SOFTPARTICLES_ON path).
    float rawDepth = tex2Dproj(_CameraDepthTexture, i.projPos).r;
    float sceneZ = 1.0 / (_ZBufferParams.z * rawDepth + _ZBufferParams.w);
    vc.a *= saturate(_InvFade * (sceneZ - i.projPos.z));
#endif

    // 2x tint and texture, applied to all four channels as in the original.
    fixed4 tint = (2.0 * vc) * (_TintColor * tex2D(_MainTex, i.uv));

    // Fog: mix from fog colour (factor 0 = far) to particle colour (factor 1 = near).
    return fixed4(lerp(i.fog.rgb, tint.rgb, i.fog.a), tint.a);
  }
  ENDCG
 }
}
}
