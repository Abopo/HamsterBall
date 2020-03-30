Shader "SmkGames/ColorReplacement"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}


		[HideInInspector] _color("Color", Color) = (1.0,1.0,1.0,1.0)

		_OriginalColor1("Original Color 1", Color) = (1.0,0,0,1.0)
		_ColorReplacement1("Replacement Color 1", Color) = (1.0,0,0,1.0)
		_Threshold1("Threshold", Range(0,1)) = 0.1
		_OriginalColor2("Original Color 2", Color) = (0,1.0,0,1.0)
		_ColorReplacement2("Replacement Color 2", Color) = (0,1.0,0,1.0)
		_Threshold2("Threshold", Range(0,1)) = 0.1
		_OriginalColor3("Original Color 3", Color) = (0,0,1.0,1.0)
		_ColorReplacement3("Replacement Color 3", Color) = (0,0,1.0,1.0)
		_Threshold3("Threshold", Range(0,1)) = 0.1
		_OriginalColor4("Original Color 4", Color) = (1.0,1.0,0,1.0)
		_ColorReplacement4("Replacement Color 4", Color) = (1.0,1.0,0,1.0)
		_Threshold4("Threshold", Range(0,1)) = 0.1
		_OriginalColor5("Original Color 5", Color) = (1.0,0,1.0,1.0)
		_ColorReplacement5("Replacement Color 5", Color) = (1.0,0,1.0,1.0)
		_Threshold5("Threshold", Range(0,1)) = 0.1
		_OriginalColor6("Original Color 6", Color) = (0,1.0,1.0,1.0)
		_ColorReplacement6("Replacement Color 6", Color) = (0,1.0,1.0,1.0)
		_Threshold6("Threshold", Range(0,1)) = 0.1
		_OriginalColor7("Original Color 7", Color) = (1.0,1.0,1.0,1.0)
		_ColorReplacement7("Replacement Color 7", Color) = (1.0,1.0,1.0,1.0)
		_Threshold7("Threshold", Range(0,1)) = 0.1
		_OriginalColor8("Original Color 8", Color) = (1.0,1.0,1.0,1.0)
		_ColorReplacement8("Replacement Color 8", Color) = (1.0,1.0,1.0,1.0)
		_Threshold8("Threshold", Range(0,1)) = 0.1
		_OriginalColor9("Original Color 9", Color) = (1.0,1.0,1.0,1.0)
		_ColorReplacement9("Replacement Color 9", Color) = (1.0,1.0,1.0,1.0)
		_Threshold9("Threshold", Range(0,1)) = 0.1
		_OriginalColor10("Original Color 10", Color) = (1.0,1.0,1.0,1.0)
		_ColorReplacement10("Replacement Color 10", Color) = (1.0,1.0,1.0,1.0)
		_Threshold10("Threshold", Range(0,1)) = 0.1

	}

		SubShader
		{
	Tags {"Queue" = "Transparent"} ZWrite Off  Blend SrcAlpha OneMinusSrcAlpha
			LOD 100

			// disable back culling
			Cull Off

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"


				uniform sampler2D _MainTex;
				uniform float4 _MainTex_ST;
				fixed4 _color;

				//MainColors
				uniform float4 _OriginalColor1;
				uniform float4 _OriginalColor2;
				uniform float4 _OriginalColor3;
				uniform float4 _OriginalColor4;
				uniform float4 _OriginalColor5;
				uniform float4 _OriginalColor6;
				uniform float4 _OriginalColor7;
				uniform float4 _OriginalColor8;
				uniform float4 _OriginalColor9;
				uniform float4 _OriginalColor10;

				//Colors Replacement
				uniform float4 _ColorReplacement1;
				uniform float4 _ColorReplacement2;
				uniform float4 _ColorReplacement3;
				uniform float4 _ColorReplacement4;
				uniform float4 _ColorReplacement5;
				uniform float4 _ColorReplacement6;
				uniform float4 _ColorReplacement7;
				uniform float4 _ColorReplacement8;
				uniform float4 _ColorReplacement9;
				uniform float4 _ColorReplacement10;

				uniform float _Threshold1;
				uniform float _Threshold2;
				uniform float _Threshold3;
				uniform float _Threshold4;
				uniform float _Threshold5;
				uniform float _Threshold6;
				uniform float _Threshold7;
				uniform float _Threshold8;
				uniform float _Threshold9;
				uniform float _Threshold10;

				struct appdata {
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f {
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
				};



				v2f vert(appdata v) {
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					return o;
				}

				half dis(float4 c) {
				half result = (pow(_color.r - c.r,2.0) + pow(_color.g - c.g,2.0) + pow(_color.b - c.b,2.0));
				return result;
				}

				fixed4 frag(v2f i) : Color
				{
					 _color = tex2D(_MainTex, i.uv);

					 if (_color.a <= 0.15) {
						return half4(0,0,0,0);
					}

					 float4 transparent = float4(0,0,0,0);
					 //_color = all(_color == _OriginalColor1) ? _ColorReplacement1 : _color;
					 _color = (distance(_color.rgb, _OriginalColor1.rgb) < _Threshold1) ? _ColorReplacement1 : _color;
					 //_color = all(_color == _OriginalColor2) ? _ColorReplacement2 : _color;
					 _color = (distance(_color.rgb, _OriginalColor2.rgb) < _Threshold2) ? _ColorReplacement2 : _color;
					 //_color = all(_color == _OriginalColor3) ? _ColorReplacement3 : _color;
					 _color = (distance(_color.rgb, _OriginalColor3.rgb) < _Threshold3) ? _ColorReplacement3 : _color;
					 //_color = all(_color == _OriginalColor4) ? _ColorReplacement4 : _color;
					 _color = (distance(_color.rgb, _OriginalColor4.rgb) < _Threshold4) ? _ColorReplacement4 : _color;
					 //_color = all(_color == _OriginalColor5) ? _ColorReplacement5 : _color;
					 _color = (distance(_color.rgb, _OriginalColor5.rgb) < _Threshold5) ? _ColorReplacement5 : _color;
					 //_color = all(_color == _OriginalColor6) ? _ColorReplacement6 : _color;
					 _color = (distance(_color.rgb, _OriginalColor6.rgb) < _Threshold6) ? _ColorReplacement6 : _color;
					 //_color = all(_color == _OriginalColor7) ? _ColorReplacement7 : _color;
					 _color = (distance(_color.rgb, _OriginalColor7.rgb) < _Threshold7) ? _ColorReplacement7 : _color;
					 //
					 _color = (distance(_color.rgb, _OriginalColor8.rgb) < _Threshold8) ? _ColorReplacement8 : _color;
					 //
					 _color = (distance(_color.rgb, _OriginalColor9.rgb) < _Threshold9) ? _ColorReplacement9 : _color;
					 //
					 _color = (distance(_color.rgb, _OriginalColor10.rgb) < _Threshold10) ? _ColorReplacement10 : _color;


					 return _color;
				}
				ENDCG
			}
		}
}