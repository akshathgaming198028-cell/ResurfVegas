Shader "Hidden/Internal-Flare" {
SubShader { 
 Tags { "RenderType"="Overlay" }
 Pass {
  Tags { "RenderType"="Overlay" }
  ZTest Always
  ZWrite Off
  Cull Off
  Blend One One
  ColorMask RGB
  GpuProgramID 33804
Program "vp" {
SubProgram "gles " {
GpuProgramIndex 0
}
