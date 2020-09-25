// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "WaterUnder"
{
	Properties
	{
		_SpecColor("Specular Color",Color)=(1,1,1,1)
		_HeightMask("HeightMask", 2D) = "white" {}
		_Scale("Scale", Range( 0 , 16)) = 36.39118
		[Normal]_Texture0("Texture 0", 2D) = "bump" {}
		_ColorFirst("Color First", Color) = (0.7216981,0.861698,1,0)
		_ColorSecond("ColorSecond", Color) = (0.2214756,0.3694088,0.745283,0)
		_FoamExponent("FoamExponent", Range( 0 , 8)) = 3
		_FoamColor("FoamColor", Color) = (1,1,1,0)
		_EmmisionPower("EmmisionPower", Range( 0 , 5)) = 0.57
		_Opacity("Opacity", Range( 0 , 1)) = 0.58
		_Distortion("Distortion", Range( 0 , 0.0001)) = 0.0001
		[Header( Beach color by Vertex Color Red Chanel)]_BeachColor("BeachColor", Color) = (0.8207547,0.8207547,0.8207547,0)
		[Toggle]_UseVertexColorMask("UseVertexColorMask", Float) = 1
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Front
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float3 worldPos;
			float4 vertexColor : COLOR;
		};

		uniform float _UseVertexColorMask;
		uniform sampler2D _HeightMask;
		uniform float _Scale;
		uniform float _Distortion;
		uniform float _FoamExponent;
		uniform float4 _FoamColor;
		uniform float4 _ColorFirst;
		uniform float4 _ColorSecond;
		uniform float _EmmisionPower;
		uniform sampler2D _Texture0;
		uniform float4 _BeachColor;
		uniform float _Opacity;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		void surf( Input i , inout SurfaceOutput o )
		{
			float4 _Speed = float4(0.1,-0.1,-0.05,0.07);
			float2 appendResult16 = (float2(_Speed.x , _Speed.y));
			float3 ase_worldPos = i.worldPos;
			float2 appendResult13 = (float2(ase_worldPos.x , ase_worldPos.z));
			float2 panner12 = ( 0.3 * _Time.y * appendResult16 + ( appendResult13 * _Scale ));
			float2 temp_output_9_0 = ( appendResult13 * 0.33 );
			float2 panner50 = ( 0.21 * _Time.y * float2( 0.16,-0.23 ) + ( float2( 0.86,0.91 ) * temp_output_9_0 ));
			float simplePerlin2D47 = snoise( panner50 );
			float2 lerpResult55 = lerp( panner12 , ( panner12 * simplePerlin2D47 ) , _Distortion);
			float2 appendResult17 = (float2(_Speed.z , _Speed.w));
			float2 panner11 = ( 0.3 * _Time.y * appendResult17 + temp_output_9_0);
			float2 lerpResult54 = lerp( panner11 , ( panner11 * simplePerlin2D47 ) , _Distortion);
			float temp_output_20_0 = ( ( tex2D( _HeightMask, lerpResult55 ).r * 2.0 ) * ( tex2D( _HeightMask, lerpResult54 ).r * 2.0 ) );
			float clampResult36 = clamp( temp_output_20_0 , 0.0 , 1.0 );
			float4 lerpResult35 = lerp( _ColorFirst , _ColorSecond , clampResult36);
			float temp_output_51_0 = (0.2 + (simplePerlin2D47 - -1.0) * (1.0 - 0.2) / (1.0 - -1.0));
			float3 tex2DNode30 = UnpackScaleNormal( tex2D( _Texture0, lerpResult55 ), temp_output_51_0 );
			float3 appendResult42 = (float3(tex2DNode30.r , 0.0 , tex2DNode30.b));
			float3 tex2DNode31 = UnpackScaleNormal( tex2D( _Texture0, lerpResult54 ), temp_output_51_0 );
			float dotResult69 = dot( appendResult42 , BlendNormals( tex2DNode30 , tex2DNode31 ) );
			float clampResult73 = clamp( pow( dotResult69 , 16.0 ) , 0.8 , 1.0 );
			float4 temp_output_63_0 = ( ( ( pow( temp_output_20_0 , _FoamExponent ) * _FoamColor ) + lerpResult35 ) * _EmmisionPower * clampResult73 );
			float4 blendOpSrc81 = temp_output_63_0;
			float4 blendOpDest81 = ( _BeachColor * i.vertexColor.r );
			o.Emission = lerp(temp_output_63_0,( saturate( ( 1.0 - ( 1.0 - blendOpSrc81 ) * ( 1.0 - blendOpDest81 ) ) )),_UseVertexColorMask).rgb;
			float lerpResult79 = lerp( _Opacity , _BeachColor.a , i.vertexColor.r);
			o.Gloss = lerpResult79;
			o.Alpha = lerpResult79;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf BlinnPhong alpha:fade keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float3 worldPos : TEXCOORD1;
				half4 color : COLOR0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.color = v.color;
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.vertexColor = IN.color;
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16700
160;533;1906;1004;-3221.539;1404.599;1;True;False
Node;AmplifyShaderEditor.WorldPosInputsNode;4;-1492,-508;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;13;-1210.91,-528.5557;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-1045,-1025;Float;False;Constant;_Float0;Float 0;2;0;Create;True;0;0;False;0;0.33;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-847,-969;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-1032.518,-1314.132;Float;False;2;2;0;FLOAT2;0.86,0.91;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;14;-1444.801,-1546.854;Float;False;Constant;_Speed;Speed;2;0;Create;True;0;0;False;0;0.1,-0.1,-0.05,0.07;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;7;-1328,-804.2279;Float;False;Property;_Scale;Scale;2;0;Create;True;0;0;False;0;36.39118;2.16;0;16;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;50;-293.3405,-1804.818;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.16,-0.23;False;1;FLOAT;0.21;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;-834.4883,-615.4535;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;17;-1015.398,-1529.216;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;16;-924.1202,-1318.466;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;47;68.60349,-1788.503;Float;False;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;11;-573.0132,-1537.421;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-1,-0.5;False;1;FLOAT;0.3;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;12;-546.6173,-1140.457;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.5,0.15;False;1;FLOAT;0.3;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;136.3821,-1494.544;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;57;-966.9506,-47.34157;Float;False;Property;_Distortion;Distortion;11;0;Create;True;0;0;False;0;0.0001;3.5E-06;0;0.0001;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;-78.92259,-983.7983;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;54;-413.9487,-592.4609;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;55;449.4396,-1263.503;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;2;-91.1467,-588.0665;Float;True;Property;_HeightMask;HeightMask;1;0;Create;True;0;0;False;0;700fe5f700bcdd44ea2e37faeedfbb35;700fe5f700bcdd44ea2e37faeedfbb35;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SamplerNode;1;262.6088,-681.8882;Float;True;Property;_water;water;0;0;Create;True;0;0;False;0;700fe5f700bcdd44ea2e37faeedfbb35;700fe5f700bcdd44ea2e37faeedfbb35;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;262.9093,-864.2871;Float;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;29;-136.3981,-196.0259;Float;True;Property;_Texture0;Texture 0;3;1;[Normal];Create;True;0;0;True;0;a4971fc01be23a245b0e18a3e21d979c;a4971fc01be23a245b0e18a3e21d979c;True;bump;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TFHCRemapNode;51;315.4592,-1805.48;Float;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0.2;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;677.5963,-917.6184;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;31;312.6094,-115.7723;Float;True;Property;_TextureSample2;Texture Sample 2;3;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;678.5962,-675.6184;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;30;248.5193,-440.261;Float;True;Property;_TextureSample1;Texture Sample 1;3;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BlendNormalsNode;32;1109.503,30.29892;Float;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;41;990.1318,-1395.02;Float;False;Property;_FoamExponent;FoamExponent;6;0;Create;True;0;0;False;0;3;3.18;0;8;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;1020.994,-676.2452;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;42;1191.907,-349.3022;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;33;1353.132,-865.0204;Float;False;Property;_ColorFirst;Color First;4;0;Create;True;0;0;False;0;0.7216981,0.861698,1,0;0,0.3151596,0.4705882,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;34;1339.132,-653.0204;Float;False;Property;_ColorSecond;ColorSecond;5;0;Create;True;0;0;False;0;0.2214756,0.3694088,0.745283,0;0.4575472,0.7490385,1,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;36;2757.565,-447.033;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;69;1388.656,-262.213;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;37;1320.132,-1341.02;Float;False;2;0;FLOAT;0;False;1;FLOAT;4;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;38;1628.132,-1207.02;Float;False;Property;_FoamColor;FoamColor;7;0;Create;True;0;0;False;0;1,1,1,0;0.1792453,0.1792453,0.1792453,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;35;1782.132,-980.0204;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;70;3327.637,-1080.413;Float;False;2;0;FLOAT;0;False;1;FLOAT;16;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;1838.132,-1385.02;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;76;3900.046,-1348.034;Float;False;Property;_BeachColor;BeachColor;12;0;Create;True;0;0;False;1;Header( Beach color by Vertex Color Red Chanel);0.8207547,0.8207547,0.8207547,0;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;75;3931.046,-1120.034;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;64;3118.399,-801.8187;Float;False;Property;_EmmisionPower;EmmisionPower;9;0;Create;True;0;0;False;0;0.57;0.75;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;40;3018.115,-619.7989;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;73;3667.25,-1087.048;Float;False;3;0;FLOAT;0;False;1;FLOAT;0.8;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;4253.046,-1278.034;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;3932.361,-946.0099;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;62;3894.208,-524.5475;Float;False;Property;_Opacity;Opacity;10;0;Create;True;0;0;False;0;0.58;0.503;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;81;4490.169,-1140.312;Float;False;Screen;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;79;4224.569,-809.1108;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;56;-58.63953,-1504.93;Float;False;Property;_Float1;Float 1;8;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;44;988.2571,-189.8444;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ToggleSwitchNode;80;4596.369,-926.1114;Float;False;Property;_UseVertexColorMask;UseVertexColorMask;13;0;Create;True;0;0;False;0;1;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;66;4970.81,-942.2935;Float;False;True;2;Float;ASEMaterialInspector;0;0;BlinnPhong;WaterUnder;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Front;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;0;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;13;0;4;1
WireConnection;13;1;4;3
WireConnection;9;0;13;0
WireConnection;9;1;10;0
WireConnection;48;1;9;0
WireConnection;50;0;48;0
WireConnection;74;0;13;0
WireConnection;74;1;7;0
WireConnection;17;0;14;3
WireConnection;17;1;14;4
WireConnection;16;0;14;1
WireConnection;16;1;14;2
WireConnection;47;0;50;0
WireConnection;11;0;9;0
WireConnection;11;2;17;0
WireConnection;12;0;74;0
WireConnection;12;2;16;0
WireConnection;52;0;11;0
WireConnection;52;1;47;0
WireConnection;53;0;12;0
WireConnection;53;1;47;0
WireConnection;54;0;11;0
WireConnection;54;1;52;0
WireConnection;54;2;57;0
WireConnection;55;0;12;0
WireConnection;55;1;53;0
WireConnection;55;2;57;0
WireConnection;1;0;2;0
WireConnection;1;1;54;0
WireConnection;3;0;2;0
WireConnection;3;1;55;0
WireConnection;51;0;47;0
WireConnection;18;0;3;1
WireConnection;31;0;29;0
WireConnection;31;1;54;0
WireConnection;31;5;51;0
WireConnection;19;0;1;1
WireConnection;30;0;29;0
WireConnection;30;1;55;0
WireConnection;30;5;51;0
WireConnection;32;0;30;0
WireConnection;32;1;31;0
WireConnection;20;0;18;0
WireConnection;20;1;19;0
WireConnection;42;0;30;1
WireConnection;42;2;30;3
WireConnection;36;0;20;0
WireConnection;69;0;42;0
WireConnection;69;1;32;0
WireConnection;37;0;20;0
WireConnection;37;1;41;0
WireConnection;35;0;33;0
WireConnection;35;1;34;0
WireConnection;35;2;36;0
WireConnection;70;0;69;0
WireConnection;39;0;37;0
WireConnection;39;1;38;0
WireConnection;40;0;39;0
WireConnection;40;1;35;0
WireConnection;73;0;70;0
WireConnection;77;0;76;0
WireConnection;77;1;75;1
WireConnection;63;0;40;0
WireConnection;63;1;64;0
WireConnection;63;2;73;0
WireConnection;81;0;63;0
WireConnection;81;1;77;0
WireConnection;79;0;62;0
WireConnection;79;1;76;4
WireConnection;79;2;75;1
WireConnection;44;0;31;1
WireConnection;44;2;31;3
WireConnection;80;0;63;0
WireConnection;80;1;81;0
WireConnection;66;2;80;0
WireConnection;66;4;79;0
WireConnection;66;9;79;0
ASEEND*/
//CHKSM=8579A75394F9D99FE191868427570AECBA00D405