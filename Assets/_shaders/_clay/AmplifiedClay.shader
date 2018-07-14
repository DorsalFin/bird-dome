// Upgrade NOTE: upgraded instancing buffer 'AmplifiedClay' to new syntax.

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "AmplifiedClay"
{
	Properties
	{
		_claynormal("clay normal", 2D) = "bump" {}
		_AnimSpeed("AnimSpeed", Range( 0 , 5)) = 0
		_VertexAnimScale("VertexAnimScale", Range( 0 , 0.1)) = 0
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_Color0("Color 0", Color) = (0.6313726,0.8666667,0.9607843,1)
		_Wiggle("Wiggle", Int) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Background+0" "IgnoreProjector" = "True" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#pragma target 4.0
		#pragma multi_compile_instancing
		#pragma surface surf Standard keepalpha noshadow exclude_path:deferred vertex:vertexDataFunc 
		struct Input
		{
			float3 worldNormal;
			INTERNAL_DATA
			float2 uv_texcoord;
		};

		uniform float _AnimSpeed;
		uniform float _VertexAnimScale;
		uniform sampler2D _claynormal;
		uniform float4 _claynormal_ST;
		uniform float4 _Color0;
		uniform float _Metallic;
		uniform float _Smoothness;

		UNITY_INSTANCING_BUFFER_START(AmplifiedClay)
			UNITY_DEFINE_INSTANCED_PROP(int, _Wiggle)
#define _Wiggle_arr AmplifiedClay
		UNITY_INSTANCING_BUFFER_END(AmplifiedClay)


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


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertexNormal = v.normal.xyz;
			int _Wiggle_Instance = UNITY_ACCESS_INSTANCED_PROP(_Wiggle_arr, _Wiggle);
			float3 temp_output_70_0 = ( ase_vertexNormal + ( floor( ( _Time.y * 10.0 ) ) * ( _AnimSpeed * _Wiggle_Instance ) ) );
			float simplePerlin2D71 = snoise( temp_output_70_0.xy );
			float temp_output_113_0 = ( _Wiggle_Instance * _VertexAnimScale );
			float3 temp_cast_1 = ((( temp_output_113_0 * 0.0 ) + (simplePerlin2D71 - 0.0) * (temp_output_113_0 - ( temp_output_113_0 * 0.0 )) / (1.0 - 0.0))).xxx;
			v.vertex.xyz += temp_cast_1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			int _Wiggle_Instance = UNITY_ACCESS_INSTANCED_PROP(_Wiggle_arr, _Wiggle);
			float3 temp_output_70_0 = ( ase_vertexNormal + ( floor( ( _Time.y * 10.0 ) ) * ( _AnimSpeed * _Wiggle_Instance ) ) );
			float simplePerlin2D88 = snoise( temp_output_70_0.xy );
			float2 uv_claynormal = i.uv_texcoord * _claynormal_ST.xy + _claynormal_ST.zw;
			o.Normal = UnpackScaleNormal( tex2D( _claynormal, uv_claynormal ) ,simplePerlin2D88 );
			o.Albedo = _Color0.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15401
509;139;1366;848;1258.139;90.82626;1.423203;True;False
Node;AmplifyShaderEditor.CommentaryNode;101;-1055.402,-13.54938;Float;False;1047.161;541.5779;(every x seconds);8;69;77;79;70;66;68;76;67;Timing;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;79;-1003.322,271.8852;Float;False;Constant;_Float0;Float 0;7;0;Create;True;0;0;False;0;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;69;-1014.611,74.55193;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;-788.6908,180.7711;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;112;-216.2453,737.803;Float;False;InstancedProperty;_Wiggle;Wiggle;7;0;Create;True;0;0;False;0;0;1;0;1;INT;0
Node;AmplifyShaderEditor.RangedFloatNode;67;-743.8474,425.6344;Float;False;Property;_AnimSpeed;AnimSpeed;2;0;Create;True;0;0;False;0;0;5;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;104;71.71328,507.8676;Float;False;982.2985;509.2713;(random vertex displacement);5;74;72;113;71;75;Vertex;1,1,1,1;0;0
Node;AmplifyShaderEditor.FloorOpNode;76;-614.3818,233.4586;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;114;-430.5813,522.309;Float;False;2;2;0;FLOAT;0;False;1;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;72;103.8889,895.0003;Float;False;Property;_VertexAnimScale;VertexAnimScale;3;0;Create;True;0;0;False;0;0;0.01;0;0.1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;-386.8635,314.151;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;66;-455.3021,67.21877;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;70;-139.374,209.0224;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;103;264.5298,-61.44882;Float;False;700.5166;335.3609;(uv manipulation);2;88;60;Normals;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;113;407.6346,809.0534;Float;False;2;2;0;INT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;75;607.9855,757.7678;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;88;331.038,49.64085;Float;False;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;71;546.3547,587.6379;Float;False;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;1;1989.584,157.7558;Float;False;313;505;;1;21;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TFHCRemapNode;74;819.1904,755.5175;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;89;1671.075,423.0801;Float;False;Property;_Smoothness;Smoothness;5;0;Create;True;0;0;False;0;0;0.5647059;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;111;1638.499,10.79315;Float;False;Property;_Color0;Color 0;6;0;Create;True;0;0;False;0;0.6313726,0.8666667,0.9607843,1;0.6313726,0.8666667,0.9607843,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;60;602.9556,23.54635;Float;True;Property;_claynormal;clay normal;1;0;Create;True;0;0;False;0;2499d375cb4b2bb488b6b2f1ed7cb69c;2499d375cb4b2bb488b6b2f1ed7cb69c;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;90;1667.229,323.3002;Float;False;Property;_Metallic;Metallic;4;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;21;2043.263,216.7907;Float;False;True;4;Float;ASEMaterialInspector;0;0;Standard;AmplifiedClay;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Opaque;;Background;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;-1;False;-1;-1;False;-1;0;False;0.01;0.2352941,0.1764706,0.509804,1;VertexScale;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;109;1170.039,78.60989;Float;False;310;732;only do these if a property is true (if the model has moved/rotated/animated this frame);0;Condition;1,1,1,1;0;0
WireConnection;77;0;69;2
WireConnection;77;1;79;0
WireConnection;76;0;77;0
WireConnection;114;0;67;0
WireConnection;114;1;112;0
WireConnection;68;0;76;0
WireConnection;68;1;114;0
WireConnection;70;0;66;0
WireConnection;70;1;68;0
WireConnection;113;0;112;0
WireConnection;113;1;72;0
WireConnection;75;0;113;0
WireConnection;88;0;70;0
WireConnection;71;0;70;0
WireConnection;74;0;71;0
WireConnection;74;3;75;0
WireConnection;74;4;113;0
WireConnection;60;5;88;0
WireConnection;21;0;111;0
WireConnection;21;1;60;0
WireConnection;21;3;90;0
WireConnection;21;4;89;0
WireConnection;21;11;74;0
ASEEND*/
//CHKSM=E23A1BC722206A5DC916D5A5FD13A69A7C0479A2