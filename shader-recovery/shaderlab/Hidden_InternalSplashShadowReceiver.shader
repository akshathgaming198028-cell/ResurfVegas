Shader "Hidden/InternalSplashShadowReceiver" {
SubShader { 
 Pass {
  Cull Off
  GpuProgramID 50027
Program "vp" {
SubProgram "gles " {
GpuProgramIndex 0
}
