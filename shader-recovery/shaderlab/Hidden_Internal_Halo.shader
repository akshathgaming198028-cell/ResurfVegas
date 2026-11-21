Shader "Hidden/Internal-Halo" {
SubShader { 
 Tags { "RenderType"="Overlay" }
 Pass {
  Tags { "RenderType"="Overlay" }
  ZWrite Off
  Cull Off
  Blend OneMinusDstColor One
  ColorMask RGB
  GpuProgramID 31992
Program "vp" {
SubProgram "gles " {
GpuProgramIndex 0
}
