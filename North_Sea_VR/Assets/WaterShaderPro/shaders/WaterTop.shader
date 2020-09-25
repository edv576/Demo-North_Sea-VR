// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "WaterTop"
{
	Properties
	{
		[Header(Refraction)]
		_ChromaticAberration("Chromatic Aberration", Range( 0 , 0.3)) = 0.1
		_HeightMask("HeightMask", 2D) = "white" {}
		_Scale("Scale", Range( 0 , 16)) = 36.39118
		[Normal]_Texture0("Texture 0", 2D) = "bump" {}
		_ColorFirst("Color First", Color) = (0.7216981,0.861698,1,0)
		_ColorSecond("ColorSecond", Color) = (0.2214756,0.3694088,0.745283,0)
		_FoamExponent("FoamExponent", Range( 0 , 8)) = 3
		_FoamColor("FoamColor", Color) = (1,1,1,0)
		_EmmisionPower("EmmisionPower", Range( 0 , 2)) = 0.57
		_Opacity("Opacity", Range( 0 , 1)) = 0.58
		_Distortion("Distortion", Range( 0 , 0.0001)) = 0.0001
		_Smooth("Smooth", Range( 0 , 1)) = 0.84
		[Header( Beach color by Vertex Color Red Chanel)]_BeachColor("BeachColor", Color) = (0.8207547,0.8207547,0.8207547,0)
		[Toggle]_UseVertexColorMask("UseVertexColorMask", Float) = 1
		_RefractRemap("RefractRemap", Vector) = (0,0,0,0)
		_VertexColorExp("VertexColorExp", Range( 0 , 8)) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		GrabPass{ }
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma multi_compile _ALPHAPREMULTIPLY_ON
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float4 vertexColor : COLOR;
			float4 screenPos;
		};

		uniform sampler2D _Texture0;
		uniform float _Scale;
		uniform float _Distortion;
		uniform float _UseVertexColorMask;
		uniform sampler2D _HeightMask;
		uniform float _FoamExponent;
		uniform float4 _FoamColor;
		uniform float4 _ColorFirst;
		uniform float4 _ColorSecond;
		uniform float _EmmisionPower;
		uniform float4 _BeachColor;
		uniform float _VertexColorExp;
		uniform float _Smooth;
		uniform float _Opacity;
		uniform sampler2D _GrabTexture;
		uniform float _ChromaticAberration;
		uniform float2 _RefractRemap;


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


		inline float4 Refraction( Input i, SurfaceOutputStandard o, float indexOfRefraction, float chomaticAberration ) {
			float3 worldNormal = o.Normal;
			float4 screenPos = i.screenPos;
			#if UNITY_UV_STARTS_AT_TOP
				float scale = -1.0;
			#else
				float scale = 1.0;
			#endif
			float halfPosW = screenPos.w * 0.5;
			screenPos.y = ( screenPos.y - halfPosW ) * _ProjectionParams.x * scale + halfPosW;
			#if SHADER_API_D3D9 || SHADER_API_D3D11
				screenPos.w += 0.00000000001;
			#endif
			float2 projScreenPos = ( screenPos / screenPos.w ).xy;
			float3 worldViewDir = normalize( UnityWorldSpaceViewDir( i.worldPos ) );
			float3 refractionOffset = ( ( ( ( indexOfRefraction - 1.0 ) * mul( UNITY_MATRIX_V, float4( worldNormal, 0.0 ) ) ) * ( 1.0 / ( screenPos.z + 1.0 ) ) ) * ( 1.0 - dot( worldNormal, worldViewDir ) ) );
			float2 cameraRefraction = float2( refractionOffset.x, -( refractionOffset.y * _ProjectionParams.x ) );
			float4 redAlpha = tex2D( _GrabTexture, ( projScreenPos + cameraRefraction ) );
			float green = tex2D( _GrabTexture, ( projScreenPos + ( cameraRefraction * ( 1.0 - chomaticAberration ) ) ) ).g;
			float blue = tex2D( _GrabTexture, ( projScreenPos + ( cameraRefraction * ( 1.0 + chomaticAberration ) ) ) ).b;
			return float4( redAlpha.r, green, blue, redAlpha.a );
		}

		void RefractionF( Input i, SurfaceOutputStandard o, inout half4 color )
		{
			#ifdef UNITY_PASS_FORWARDBASE
			float temp_output_93_0 = ( 1.0 - pow( ( 1.0 - i.vertexColor.r ) , 1.8 ) );
			float3 ase_worldPos = i.worldPos;
			float2 appendResult13 = (float2(ase_worldPos.x , ase_worldPos.z));
			float2 temp_output_9_0 = ( appendResult13 * 0.33 );
			float2 panner50 = ( 0.21 * _Time.y * float2( 0.16,-0.23 ) + ( float2( 0.86,0.91 ) * temp_output_9_0 ));
			float simplePerlin2D47 = snoise( panner50 );
			float temp_output_51_0 = (0.2 + (simplePerlin2D47 - -1.0) * (1.0 - 0.2) / (1.0 - -1.0));
			float4 _Speed = float4(0.1,-0.1,-0.05,0.07);
			float2 appendResult16 = (float2(_Speed.x , _Speed.y));
			float2 panner12 = ( 0.3 * _Time.y * appendResult16 + ( _Scale * appendResult13 ));
			float2 lerpResult55 = lerp( panner12 , ( panner12 * simplePerlin2D47 ) , _Distortion);
			float3 tex2DNode30 = UnpackScaleNormal( tex2D( _Texture0, lerpResult55 ), temp_output_51_0 );
			float3 appendResult42 = (float3(tex2DNode30.r , 0.0 , tex2DNode30.b));
			float2 appendResult17 = (float2(_Speed.z , _Speed.w));
			float2 panner11 = ( 0.3 * _Time.y * appendResult17 + temp_output_9_0);
			float2 lerpResult54 = lerp( panner11 , ( panner11 * simplePerlin2D47 ) , _Distortion);
			float3 temp_output_32_0 = BlendNormals( tex2DNode30 , UnpackScaleNormal( tex2D( _Texture0, lerpResult54 ), temp_output_51_0 ) );
			float dotResult69 = dot( appendResult42 , temp_output_32_0 );
			float clampResult73 = clamp( pow( dotResult69 , 16.0 ) , 0.8 , 1.0 );
			color.rgb = color.rgb + Refraction( i, o, (_RefractRemap.x + (( temp_output_93_0 + clampResult73 ) - 0.0) * (_RefractRemap.y - _RefractRemap.x) / (1.0 - 0.0)), _ChromaticAberration ) * ( 1 - color.a );
			color.a = 1;
			#endif
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Normal = float3(0,0,1);
			float3 ase_worldPos = i.worldPos;
			float2 appendResult13 = (float2(ase_worldPos.x , ase_worldPos.z));
			float2 temp_output_9_0 = ( appendResult13 * 0.33 );
			float2 panner50 = ( 0.21 * _Time.y * float2( 0.16,-0.23 ) + ( float2( 0.86,0.91 ) * temp_output_9_0 ));
			float simplePerlin2D47 = snoise( panner50 );
			float temp_output_51_0 = (0.2 + (simplePerlin2D47 - -1.0) * (1.0 - 0.2) / (1.0 - -1.0));
			float4 _Speed = float4(0.1,-0.1,-0.05,0.07);
			float2 appendResult16 = (float2(_Speed.x , _Speed.y));
			float2 panner12 = ( 0.3 * _Time.y * appendResult16 + ( _Scale * appendResult13 ));
			float2 lerpResult55 = lerp( panner12 , ( panner12 * simplePerlin2D47 ) , _Distortion);
			float3 tex2DNode30 = UnpackScaleNormal( tex2D( _Texture0, lerpResult55 ), temp_output_51_0 );
			float2 appendResult17 = (float2(_Speed.z , _Speed.w));
			float2 panner11 = ( 0.3 * _Time.y * appendResult17 + temp_output_9_0);
			float2 lerpResult54 = lerp( panner11 , ( panner11 * simplePerlin2D47 ) , _Distortion);
			float3 temp_output_32_0 = BlendNormals( tex2DNode30 , UnpackScaleNormal( tex2D( _Texture0, lerpResult54 ), temp_output_51_0 ) );
			o.Normal = temp_output_32_0;
			float temp_output_20_0 = ( ( tex2D( _HeightMask, lerpResult55 ).r * 2.0 ) * ( tex2D( _HeightMask, lerpResult54 ).r * 2.0 ) );
			float clampResult36 = clamp( temp_output_20_0 , 0.0 , 1.0 );
			float4 lerpResult35 = lerp( _ColorFirst , _ColorSecond , clampResult36);
			float3 appendResult42 = (float3(tex2DNode30.r , 0.0 , tex2DNode30.b));
			float dotResult69 = dot( appendResult42 , temp_output_32_0 );
			float clampResult73 = clamp( pow( dotResult69 , 16.0 ) , 0.8 , 1.0 );
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float fresnelNdotV74 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode74 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV74, 5.0 ) );
			float temp_output_75_0 = sqrt( fresnelNode74 );
			float4 temp_output_63_0 = ( ( ( pow( temp_output_20_0 , _FoamExponent ) * _FoamColor ) + lerpResult35 ) * _EmmisionPower * clampResult73 * temp_output_75_0 );
			float4 blendOpSrc87 = temp_output_63_0;
			float4 blendOpDest87 = ( _BeachColor * pow( i.vertexColor.r , _VertexColorExp ) );
			float4 lerpResult99 = lerp( ( saturate( ( 1.0 - ( 1.0 - blendOpSrc87 ) * ( 1.0 - blendOpDest87 ) ) )) , temp_output_63_0 , 0.5);
			float4 temp_output_89_0 = lerp(temp_output_63_0,lerpResult99,_UseVertexColorMask);
			o.Albedo = temp_output_89_0.rgb;
			o.Emission = temp_output_89_0.rgb;
			o.Metallic = pow( fresnelNode74 , 1.0 );
			float clampResult82 = clamp( ( 1.0 - temp_output_75_0 ) , 0.6 , 1.0 );
			o.Smoothness = ( clampResult82 * _Smooth );
			float temp_output_93_0 = ( 1.0 - pow( ( 1.0 - i.vertexColor.r ) , 1.8 ) );
			float lerpResult88 = lerp( _Opacity , _BeachColor.a , temp_output_93_0);
			o.Alpha = lerpResult88;
			o.Normal = o.Normal + 0.00001 * i.screenPos * i.worldPos;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha finalcolor:RefractionF fullforwardshadows exclude_path:deferred 

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
				float4 screenPos : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
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
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
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
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				surfIN.screenPos = IN.screenPos;
				surfIN.vertexColor = IN.color;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
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
237;699;1906;1004;-4623.735;1619.728;1.630151;True;False
Node;AmplifyShaderEditor.WorldPosInputsNode;4;-1492,-508;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;10;-1281.988,-1004.536;Float;False;Constant;_Float0;Float 0;2;0;Create;True;0;0;False;0;0.33;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;13;-1210.91,-528.5557;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-847,-969;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;14;-1444.801,-1546.854;Float;False;Constant;_Speed;Speed;2;0;Create;True;0;0;False;0;0.1,-0.1,-0.05,0.07;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-1180.201,-1290.748;Float;False;2;2;0;FLOAT2;0.86,0.91;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-1328,-803;Float;False;Property;_Scale;Scale;3;0;Create;True;0;0;False;0;36.39118;0.5;0;16;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;17;-1015.398,-1529.216;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;50;-293.3405,-1804.818;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.16,-0.23;False;1;FLOAT;0.21;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;16;-924.1202,-1318.466;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;-865.5251,-574.8896;Float;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;11;-573.0132,-1537.421;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-1,-0.5;False;1;FLOAT;0.3;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;12;-546.6173,-1140.457;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.5,0.15;False;1;FLOAT;0.3;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;47;68.60349,-1788.503;Float;False;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;-78.92259,-983.7983;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;136.3821,-1494.544;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;57;-966.9506,-47.34157;Float;False;Property;_Distortion;Distortion;12;0;Create;True;0;0;False;0;0.0001;4.73E-05;0;0.0001;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;55;449.4396,-1263.503;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;2;-91.1467,-588.0665;Float;True;Property;_HeightMask;HeightMask;2;0;Create;True;0;0;False;0;700fe5f700bcdd44ea2e37faeedfbb35;700fe5f700bcdd44ea2e37faeedfbb35;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.LerpOp;54;-413.9487,-592.4609;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;29;-136.3981,-196.0259;Float;True;Property;_Texture0;Texture 0;4;1;[Normal];Create;True;0;0;True;0;a4971fc01be23a245b0e18a3e21d979c;a4971fc01be23a245b0e18a3e21d979c;True;bump;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TFHCRemapNode;51;315.4592,-1805.48;Float;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0.2;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;262.6088,-681.8882;Float;True;Property;_water;water;0;0;Create;True;0;0;False;0;700fe5f700bcdd44ea2e37faeedfbb35;700fe5f700bcdd44ea2e37faeedfbb35;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;262.9093,-864.2871;Float;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;31;312.6094,-115.7723;Float;True;Property;_TextureSample2;Texture Sample 2;3;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;678.5962,-675.6184;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;30;248.5193,-440.261;Float;True;Property;_TextureSample1;Texture Sample 1;3;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;677.5963,-917.6184;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;1020.994,-676.2452;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendNormalsNode;32;4920.702,-430.7155;Float;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;42;1191.907,-349.3022;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;41;990.1318,-1395.02;Float;False;Property;_FoamExponent;FoamExponent;7;0;Create;True;0;0;False;0;3;5.42;0;8;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;69;2056.298,-324.1592;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;33;1353.132,-865.0204;Float;False;Property;_ColorFirst;Color First;5;0;Create;True;0;0;False;0;0.7216981,0.861698,1,0;0.1614009,0.8773585,0.86766,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;38;1628.132,-1207.02;Float;False;Property;_FoamColor;FoamColor;8;0;Create;True;0;0;False;0;1,1,1,0;0.01481844,0.05100306,0.08490568,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;34;1339.132,-653.0204;Float;False;Property;_ColorSecond;ColorSecond;6;0;Create;True;0;0;False;0;0.2214756,0.3694088,0.745283,0;0.4575472,0.7490385,1,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;36;2757.565,-447.033;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;37;1320.132,-1341.02;Float;False;2;0;FLOAT;0;False;1;FLOAT;4;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;35;1782.132,-980.0204;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FresnelNode;74;3948.364,-1215.53;Float;False;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;1838.132,-1385.02;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;70;3327.637,-1080.413;Float;False;2;0;FLOAT;0;False;1;FLOAT;16;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;98;5222.001,-1187.738;Float;False;Property;_VertexColorExp;VertexColorExp;17;0;Create;True;0;0;False;0;0;1;0;8;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;84;5272.358,-920.4937;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;64;3002.119,-741.1511;Float;False;Property;_EmmisionPower;EmmisionPower;10;0;Create;True;0;0;False;0;0.57;0.097;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;97;5639.319,-1018.202;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;83;5323.382,-1503.809;Float;False;Property;_BeachColor;BeachColor;14;0;Create;True;0;0;False;1;Header( Beach color by Vertex Color Red Chanel);0.8207547,0.8207547,0.8207547,0;0.1096921,0.1603774,0.1583477,0.3372549;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;92;5737.637,89.7135;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;40;3018.115,-619.7989;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SqrtOpNode;75;4314.917,-1182.54;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;73;5882.528,-396.7229;Float;False;3;0;FLOAT;0;False;1;FLOAT;0.8;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;85;5676.382,-1433.809;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;90;5934.875,-41.59647;Float;False;2;0;FLOAT;0;False;1;FLOAT;1.8;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;5857.843,-1007.818;Float;False;4;4;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;93;6303.637,38.7135;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;80;4533.971,-1046.64;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;87;6303.113,-1529.198;Float;False;Screen;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;100;6811.397,-1013.312;Float;False;Constant;_Float2;Float 2;17;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;62;5339.672,-204.8325;Float;False;Property;_Opacity;Opacity;11;0;Create;True;0;0;False;0;0.58;0.618;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;96;6723.061,448.4705;Float;False;Property;_RefractRemap;RefractRemap;16;0;Create;True;0;0;False;0;0,0;0.958,1.015;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.ClampOpNode;82;4880.208,-760.0569;Float;False;3;0;FLOAT;0;False;1;FLOAT;0.6;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;78;4438.745,-692.4207;Float;False;Property;_Smooth;Smooth;13;0;Create;True;0;0;False;0;0.84;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;94;6623.72,-15.78839;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;99;6956.481,-1448.562;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;95;7086.439,354.3872;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.95;False;4;FLOAT;1.05;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;88;6556.887,-326.3414;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;81;5315.712,-559.2698;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;77;4769.835,-1219.778;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;89;6584.304,-929.1;Float;False;Property;_UseVertexColorMask;UseVertexColorMask;15;0;Create;True;0;0;False;0;1;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;56;-58.63953,-1504.93;Float;False;Property;_Float1;Float 1;9;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;66;7273.645,-433.5272;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;WaterTop;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;0;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;13;0;4;1
WireConnection;13;1;4;3
WireConnection;9;0;13;0
WireConnection;9;1;10;0
WireConnection;48;1;9;0
WireConnection;17;0;14;3
WireConnection;17;1;14;4
WireConnection;50;0;48;0
WireConnection;16;0;14;1
WireConnection;16;1;14;2
WireConnection;79;0;7;0
WireConnection;79;1;13;0
WireConnection;11;0;9;0
WireConnection;11;2;17;0
WireConnection;12;0;79;0
WireConnection;12;2;16;0
WireConnection;47;0;50;0
WireConnection;53;0;12;0
WireConnection;53;1;47;0
WireConnection;52;0;11;0
WireConnection;52;1;47;0
WireConnection;55;0;12;0
WireConnection;55;1;53;0
WireConnection;55;2;57;0
WireConnection;54;0;11;0
WireConnection;54;1;52;0
WireConnection;54;2;57;0
WireConnection;51;0;47;0
WireConnection;1;0;2;0
WireConnection;1;1;54;0
WireConnection;3;0;2;0
WireConnection;3;1;55;0
WireConnection;31;0;29;0
WireConnection;31;1;54;0
WireConnection;31;5;51;0
WireConnection;19;0;1;1
WireConnection;30;0;29;0
WireConnection;30;1;55;0
WireConnection;30;5;51;0
WireConnection;18;0;3;1
WireConnection;20;0;18;0
WireConnection;20;1;19;0
WireConnection;32;0;30;0
WireConnection;32;1;31;0
WireConnection;42;0;30;1
WireConnection;42;2;30;3
WireConnection;69;0;42;0
WireConnection;69;1;32;0
WireConnection;36;0;20;0
WireConnection;37;0;20;0
WireConnection;37;1;41;0
WireConnection;35;0;33;0
WireConnection;35;1;34;0
WireConnection;35;2;36;0
WireConnection;39;0;37;0
WireConnection;39;1;38;0
WireConnection;70;0;69;0
WireConnection;97;0;84;1
WireConnection;97;1;98;0
WireConnection;92;0;84;1
WireConnection;40;0;39;0
WireConnection;40;1;35;0
WireConnection;75;0;74;0
WireConnection;73;0;70;0
WireConnection;85;0;83;0
WireConnection;85;1;97;0
WireConnection;90;0;92;0
WireConnection;63;0;40;0
WireConnection;63;1;64;0
WireConnection;63;2;73;0
WireConnection;63;3;75;0
WireConnection;93;0;90;0
WireConnection;80;0;75;0
WireConnection;87;0;63;0
WireConnection;87;1;85;0
WireConnection;82;0;80;0
WireConnection;94;0;93;0
WireConnection;94;1;73;0
WireConnection;99;0;87;0
WireConnection;99;1;63;0
WireConnection;99;2;100;0
WireConnection;95;0;94;0
WireConnection;95;3;96;1
WireConnection;95;4;96;2
WireConnection;88;0;62;0
WireConnection;88;1;83;4
WireConnection;88;2;93;0
WireConnection;81;0;82;0
WireConnection;81;1;78;0
WireConnection;77;0;74;0
WireConnection;89;0;63;0
WireConnection;89;1;99;0
WireConnection;66;0;89;0
WireConnection;66;1;32;0
WireConnection;66;2;89;0
WireConnection;66;3;77;0
WireConnection;66;4;81;0
WireConnection;66;8;95;0
WireConnection;66;9;88;0
ASEEND*/
//CHKSM=36C49E1EDBAABC7F8A889D46639DC3C49BAEC29B