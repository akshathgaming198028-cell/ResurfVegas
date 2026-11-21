Shader "Hidden/InternalSplashShadowCaster" {
SubShader { 
 Pass {
  ZTest Always
  Cull Off
  Blend One One
  GpuProgramID 60892
Program "vp" {
SubProgram "gles " {
GpuProgramIndex 0
}
