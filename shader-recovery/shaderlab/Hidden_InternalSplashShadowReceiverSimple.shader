Shader "Hidden/InternalSplashShadowReceiverSimple" {
SubShader { 
 Pass {
  Cull Off
  GpuProgramID 38741
Program "vp" {
SubProgram "gles " {
GpuProgramIndex 0
}
