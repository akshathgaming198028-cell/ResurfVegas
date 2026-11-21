Shader "Hidden/InternalSplashShadowBlur" {
SubShader { 
 Pass {
  ZTest Always
  Cull Off
  GpuProgramID 58662
Program "vp" {
SubProgram "gles " {
GpuProgramIndex 0
}
