Shader "Hidden/InternalClear" {
SubShader { 
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  ColorMask 0
  GpuProgramID 34072
Program "vp" {
SubProgram "gles " {
GpuProgramIndex 0
}
