Shader "Hidden/Unlit/Transparent Colored 3" {
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
  GpuProgramID 40945
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
"
8
,
,
#version 100
#ifdef VERTEX
attribute vec4 _glesVertex;
_Color
&MultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp'
O
ClipRange0
1
_Args1?
2?
72;
	tmpvar_1;
varying medium
xlv_COLOR
=
2
TEXCOORD0"
4"
1D
2;
void main ()
{
.xy = ((
*X
i.zw) +
hxy);
 q
Pret_2
..xV
' *m
i.w) - !
y!
6z))O
yO
z
O
wO
!zwS
_
1
1
3
3
2
2
3
2
2
gl_Posit
>= (T
( *
4
) =
/ =
<.xy*
71 =
2
3
2
2
}
#endif
FRAGMENT
sampler2D _MainTex
0
&
[col_1
factor_2
3/
3
(1.0, 1.0
Iabs(
)
*
l
I
*zwI
Tfloat
4
P4 = m
5.x,
B.y),#
/y)
2
2
:low
5
5
exture2D (
,
,);
?
6K
56 =
65 *}
>
F.xyz
6
O
7Y
7 = clampQ
4B
4, 0.
w
U6.w *e
FragData[0] =
C
.
8
/j
G
6_ST6
0
T
1
2F
2F
$
P
4
U
91.0
z
0
0|
( *`
) +
/zww
%
V
(
0
^
P
