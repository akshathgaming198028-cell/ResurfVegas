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
 LOD 300
 Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardBase" "SHADOWSUPPORT"="true" "RenderType"="Opaque" "PerformanceChecks"="False" }
  ZWrite [_ZWrite]
  Blend [_SrcBlend] [_DstBlend]
  GpuProgramID 39223
Program "vp" {
}
Program "fp" {
}
 }
 Pass {
  Name "FORWARD_DELTA"
  Tags { "LIGHTMODE"="ForwardAdd" "SHADOWSUPPORT"="true" "RenderType"="Opaque" "PerformanceChecks"="False" }
  ZWrite Off
  Blend [_SrcBlend] One
  GpuProgramID 129312
Program "vp" {
}
Program "fp" {
}
 }
 Pass {
  Name "SHADOWCASTER"
  Tags { "LIGHTMODE"="SHADOWCASTER" "SHADOWSUPPORT"="true" "RenderType"="Opaque" "PerformanceChecks"="False" }
  GpuProgramID 147192
Program "vp" {
}
Program "fp" {
}
 }
}
SubShader {
 LOD 150
 Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardBase" "SHADOWSUPPORT"="true" "RenderType"="Opaque" "PerformanceChecks"="False" }
  ZWrite [_ZWrite]
  Blend [_SrcBlend] [_DstBlend]
  GpuProgramID 358221
Program "vp" {
SubProgram "gles " {
GpuProgramIndex 0
}
SubProgram "gles " {
GpuProgramIndex 1
}
SubProgram "gles " {
GpuProgramIndex 2
}
SubProgram "gles " {
GpuProgramIndex 3
}
SubProgram "gles " {
GpuProgramIndex 4
}
SubProgram "gles " {
GpuProgramIndex 5
}
SubProgram "gles " {
GpuProgramIndex 6
}
SubProgram "gles " {
GpuProgramIndex 7
}
SubProgram "gles " {
GpuProgramIndex 8
}
SubProgram "gles " {
GpuProgramIndex 9
}
SubProgram "gles " {
GpuProgramIndex 10
}
SubProgram "gles " {
GpuProgramIndex 11
}
}
Program "fp" {
SubProgram "gles " {
GpuProgramIndex 12
}
SubProgram "gles " {
GpuProgramIndex 13
}
SubProgram "gles " {
GpuProgramIndex 14
}
SubProgram "gles " {
GpuProgramIndex 15
}
SubProgram "gles " {
GpuProgramIndex 16
}
SubProgram "gles " {
GpuProgramIndex 17
}
}
 }
 Pass {
  Name "FORWARD_DELTA"
  Tags { "LIGHTMODE"="ForwardAdd" "SHADOWSUPPORT"="true" "RenderType"="Opaque" "PerformanceChecks"="False" }
  ZWrite Off
  Blend [_SrcBlend] One
  GpuProgramID 443686
Program "vp" {
SubProgram "gles " {
GpuProgramIndex 18
}
SubProgram "gles " {
GpuProgramIndex 19
}
SubProgram "gles " {
GpuProgramIndex 20
}
SubProgram "gles " {
GpuProgramIndex 21
}
SubProgram "gles " {
GpuProgramIndex 22
}
SubProgram "gles " {
GpuProgramIndex 23
}
SubProgram "gles " {
GpuProgramIndex 24
}
SubProgram "gles " {
GpuProgramIndex 25
}
SubProgram "gles " {
GpuProgramIndex 26
}
SubProgram "gles " {
GpuProgramIndex 27
}
SubProgram "gles " {
GpuProgramIndex 28
}
SubProgram "gles " {
GpuProgramIndex 29
}
SubProgram "gles " {
GpuProgramIndex 30
}
SubProgram "gles " {
GpuProgramIndex 31
}
SubProgram "gles " {
GpuProgramIndex 32
}
SubProgram "gles " {
GpuProgramIndex 33
}
SubProgram "gles " {
GpuProgramIndex 34
}
SubProgram "gles " {
GpuProgramIndex 35
}
SubProgram "gles " {
GpuProgramIndex 36
}
SubProgram "gles " {
GpuProgramIndex 37
}
SubProgram "gles " {
GpuProgramIndex 38
}
SubProgram "gles " {
GpuProgramIndex 39
}
SubProgram "gles " {
GpuProgramIndex 40
}
SubProgram "gles " {
GpuProgramIndex 41
}
SubProgram "gles " {
GpuProgramIndex 42
}
SubProgram "gles " {
GpuProgramIndex 43
}
}
Program "fp" {
SubProgram "gles " {
GpuProgramIndex 44
}
SubProgram "gles " {
GpuProgramIndex 45
}
SubProgram "gles " {
GpuProgramIndex 46
}
SubProgram "gles " {
GpuProgramIndex 47
}
SubProgram "gles " {
GpuProgramIndex 48
}
SubProgram "gles " {
GpuProgramIndex 49
}
SubProgram "gles " {
GpuProgramIndex 50
}
SubProgram "gles " {
GpuProgramIndex 51
}
SubProgram "gles " {
GpuProgramIndex 52
}
SubProgram "gles " {
GpuProgramIndex 53
}
SubProgram "gles " {
GpuProgramIndex 54
}
SubProgram "gles " {
GpuProgramIndex 55
}
SubProgram "gles " {
GpuProgramIndex 56
}
SubProgram "gles " {
GpuProgramIndex 57
}
SubProgram "gles " {
GpuProgramIndex 58
}
SubProgram "gles " {
GpuProgramIndex 59
}
SubProgram "gles " {
GpuProgramIndex 60
}
SubProgram "gles " {
GpuProgramIndex 61
}
SubProgram "gles " {
GpuProgramIndex 62
}
SubProgram "gles " {
GpuProgramIndex 63
}
SubProgram "gles " {
GpuProgramIndex 64
}
SubProgram "gles " {
GpuProgramIndex 65
}
SubProgram "gles " {
GpuProgramIndex 66
}
SubProgram "gles " {
GpuProgramIndex 67
}
SubProgram "gles " {
GpuProgramIndex 68
}
SubProgram "gles " {
GpuProgramIndex 69
}
}
 }
 Pass {
  Name "SHADOWCASTER"
  Tags { "LIGHTMODE"="SHADOWCASTER" "SHADOWSUPPORT"="true" "RenderType"="Opaque" "PerformanceChecks"="False" }
  GpuProgramID 488501
Program "vp" {
SubProgram "gles " {
GpuProgramIndex 70
}
SubProgram "gles " {
GpuProgramIndex 71
}
}
Program "fp" {
SubProgram "gles " {
GpuProgramIndex 72
}
SubProgram "gles " {
GpuProgramIndex 73
}
}
 }
}
Fallback "VertexLit"
}
