Shader "Unlit/Transparent Masked" {
Properties {
 _MainTex ("Base (RGB), Alpha (A)", 2D) = "black" { }
 _Mask ("Alpha (A)", 2D) = "white" { }
}
SubShader {
 LOD 200
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  Offset -1, -1
  GpuProgramID 27282
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
  Blend SrcAlpha OneMinusSrcAlpha
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
$
8
\
,
#version 100
#ifdef VERTEX
attribute vec4 _glesVertex;
_Color
MultiTexCoord0$
"1;
uniform highp mat4 glstate_matrix_mvp;
varying'
vec2 xlv_TEXCOORD0"
1"
Blowp
!
COLOR;
void main ()
{
  gl_Posit7
>= (
( *-
J);
/ =
<.xy*
1*
1*
) =
}
#endif
FRAGMENTT
sampler2D _MainT
/skb
a
&
tmpvar_1
cmedium
ocol_2;,
3
texture2D (
,
6) *
_
) =T
j
4j
4-
	i
*skf
1Y
 .w
*D
7.w)S
gl_FragData[0]
1
#
.
8
j@
g
'
O
?_ST
0
#
2
i
vclamp (
, 0.0, 1.0
B
i
91.0
Xxyz =
	Q
0`
/((
( *`
i.xy) +
z
B
)
$
V
k
/0)
/
P
