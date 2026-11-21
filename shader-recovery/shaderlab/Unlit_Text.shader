Shader "Unlit/Text" {
Properties {
 _MainTex ("Alpha (A)", 2D) = "white" { }
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
  GpuProgramID 33378
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
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZTest Always
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  GpuProgramID 70832
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
,
#version 100
#ifdef VERTEX
attribute vec4 _glesVertex;
_Color
7MultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
varying mediumpQ
xlv_COLOR
G
Avec2
TEXCOORD0;
void main ()
{
  gl_Posit
>= (v
( *
F);
 r
) =
 n
/ =
.xy;
}
#endifd
FRAGMENT
sampler2D _MainTex
B	K
Pcol_1
f.xyz =`
3low0
tmpvar_2
 = texture2D (
,
)e
ww
d
5w *K
B.w);
FragData[0] =
E
#
.
/t
'
O
6_ST
0
"
,1;$
vclamp (
, 0.0, 1.0)+
91 =B
Y
3.
3
91.0
f
1
/((
( *`
i.xy) +
z
5
)
$
J
0Q
/.w
8
P
