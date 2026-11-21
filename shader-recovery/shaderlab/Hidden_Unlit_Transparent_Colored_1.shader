Shader "Hidden/Unlit/Transparent Colored 1" {
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
  Blend SrcAlpha OneMinusSrcAlpha
  ColorMask RGB
  Offset -1, -1
  GpuProgramID 8907
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
2
8
,k
#version 100
#ifdef VERTEX
attribute vec4 _glesVertex;
_Color
&MultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp'
O
ClipRange0;
varying medium"
xlv_COLOR
@
2
TEXCOORD0"
1;
void main ()
{
  gl_Posit6
>= (
( *,
F);
) =,
/ =/
<.xy*
1z
(f
X.xy *
i.zw) +
xy);
}
#endif
FRAGMENTn
sampler2D _MainTex
2[
OArgs
e	m
Pcol_1'
A
tmpvar_2
9
(1.0, 1.0) - abs(o
G)) *
3lowm
M
3Y
3 = texture2D (:
,
,);
?
4K
4
W
*H
>
e.xyz =1
Tfloat
5Y
5 = clamp (mind
E2.x,
d.y), 0
q
w
U4.w *U
");
FragData[0] =
o
#
x
.
8
/j
5
6_ST
v
P
0Q
/
1
1
4
"
P
) =B
1
S
91.0
zB
%
A
/((C
( *`
) +
Ozw);
P
%
V
(
m
0
^
P
