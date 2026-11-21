Shader "Hidden/Unlit/Transparent Colored 2" {
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
  GpuProgramID 1496
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
8
,q
#version 100
#ifdef VERTEX
attribute vec4 _glesVertex;
_Color
&MultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp'
O
ClipRange0
1
wArgs1;
	tmpvar_1;
varying medium
xlv_COLOR
=
2
TEXCOORD0"
4"
d
oid main ()
{
.xy = ((l
*
i.zw) +
hxy);
Pret_2
..xV
' *
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
gl_Posit
>= (A
( *
4
) =
/ =
<.xy*
71 =<
}
#endifK
FRAGMENT
sampler2D _MainTex
0
e	m
[col_1
factor_2
,
3
3
(1.0, 1.0
Iabs(
)F
l
I
*zwI
R
:low
4
4
exture2D (
,N
,);
?
5K
5
W
*
>
O
F.xyz\
5
K
Tfloat
6Y
6 = clamp (min
h
5.x,t
Q.y),
=
dy)), 0E
w
U5.w *y
FragData[0] =
C
.
8
/j
[
6_ST
0
1
2Z
2Z
$
P
4
U
91.0
z
0z
/((
( *`
) +
/zw^
)
$
V
(
0
P
