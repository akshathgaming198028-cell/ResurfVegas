Shader "Unlit/Premultiplied Colored" {
Properties {
 _MainTex ("Base (RGB), Alpha (A)", 2D) = "black" { }
}
SubShader {
 LOD 200
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Blend One OneMinusSrcAlpha
  ColorMask RGB
  Offset -1, -1
  GpuProgramID 6745
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
 LOD 100
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Blend One OneMinusSrcAlpha
  ColorMask RGB
  Offset -1, -1
  GpuProgramID 71517
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
}
t
8
,m
#version 100
#ifdef VERTEX
attribute vec4 _glesVertex;
_Color
/MultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
varying'
vec2 xlv_TEXCOORD0"
rmediumps
$
COLOR;
void main ()
{
  gl_Posit
>= (v
( *
J);
 t
/ =
4.xy*
z
) =
}
#endifd
FRAGMENT
sampler2D _MainTex
B3low&
tmpvar_1
 = texture2D (
,v
");H
FragData[0]K
D
*
)
C
|
.
8
/j
'
O
6_ST
0
	=
2
2 = clamp (
, 0.0, 1.0)B
B
Y
3.
P3.w =C
Xxyz =
	^
/((
( *`
i.xy) +
Ozw);%
)
$
V
(
^
P
