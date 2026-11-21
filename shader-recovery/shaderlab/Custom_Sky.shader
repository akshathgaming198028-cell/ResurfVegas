Shader "Custom/Sky" {
Properties {
 _GradientFade ("Fade", Float) = 1
 _GradientOffset ("Gradient offset", Float) = 0
}
SubShader {
 Tags { "QUEUE"="Background+2000" "RenderType"="Opaque" }
 Pass {
  Tags { "QUEUE"="Background+2000" "RenderType"="Opaque" }
  ZWrite Off
  GpuProgramID 62636
Program "vp" {
SubProgram "gles " {
GpuProgramIndex 0
}
}
Program "fp" {
SubProgram "gles " {
GpuProgramIndex 1
}
}
 }
}
SubShader {
 Tags { "QUEUE"="Background" "RenderType"="Opaque" }
 Pass {
  Tags { "QUEUE"="Background" "RenderType"="Opaque" }
  ZWrite Off
  Cull Front
  GpuProgramID 778030
Program "vp" {
SubProgram "gles " {
GpuProgramIndex 2
}
}
Program "fp" {
SubProgram "gles " {
GpuProgramIndex 1
}
}
 }
}
Fallback "Diffuse"
}
