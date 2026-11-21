Shader "Bend/UV Scroll (Screen)/Simple" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _ScrollSpeed ("Scroll speed (xy)", Vector) = (1,1,0,0)
}
SubShader {
 Tags { "QUEUE"="Geometry" }
 Pass {
  Tags { "QUEUE"="Geometry" }
  GpuProgramID 39299
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
 Pass {
  GpuProgramID 128372
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
