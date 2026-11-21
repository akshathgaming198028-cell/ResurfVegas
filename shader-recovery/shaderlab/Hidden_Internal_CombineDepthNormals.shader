Shader "Hidden/Internal-CombineDepthNormals" {
SubShader { 
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 4591
Program "vp" {
SubProgram "gles " {
GpuProgramIndex 0
}
