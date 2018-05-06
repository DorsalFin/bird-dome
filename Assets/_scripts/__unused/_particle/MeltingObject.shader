// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/MeltingObject" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		[HDR]_EmissionColor("Emission color", color) = (1,1,1,1)
		// _EmissionStrength ("Emission multiplier", float) = 1
		_EmissionMask("Emission mask", 2D) = "white" {}
		_EmissionMaskStrength ("Emission mask strength", float) = 1
		_EdgeRadius ("Edge radius", float) = 1
		[HDR]_EdgeColor("Edge color", color  ) = (1,1,1,1)
		[Space(20)]
		_CutoffMask("Cutoff mask", 2D) = "white" {}
		_CutoffThreshold("Cutoff threshold", float) = 1
		_CutoffMaskSpeed("Cutoff mask speed", vector) = (1,1,1,1)
		[Space(20)]
		_Strength ("Bulge Strength", float) = 1

		[Space(20)]
		_OffsetStrength("Offset strength", float) = 1
		_NoiseTex ("_NoiseTex", 2D) = "white" {}
		_Tiling ("Noise Tiling", vector) = (1,1,1,1)
		_NoiseSpeed ("_NoiseSpeed", vector) = (1,1,1,1)
		_CollapseStrength ("Collapse Strength", range(-5,5)) = 0.5
		_Gravity ("Gravity Strength", float) = -9.81

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Cull off
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows addshadow  vertex:vert nolightmap

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 5.0

		sampler2D _MainTex;
		sampler2D _EmissionMask;
		sampler2D _CutoffMask;

		struct Input {
			float2 uv_MainTex;
			float2 uv_EmissionMask;
			float2 uv_CutoffMask;
			fixed displacementStrength;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed _Strength;
		float3 _EmissionColor;
		float _EmissionStrength;
		fixed _EmissionMaskStrength;
		fixed _EdgeRadius;
		float4 _EdgeColor;

		fixed _CutoffThreshold;
		fixed2 _CutoffMaskSpeed;

		float3 _ParticlePositions[8];
		fixed _ParticleSizes[8];
		fixed _ParticleAlpha[8];
		fixed _ParticleRed[8];

		sampler2D _NoiseTex;
		fixed _CollapseStrength;
		fixed _OffsetStrength;
		half _Gravity;
		half4 _Tiling;
		half2 _NoiseSpeed;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		float distortionFalloff(float dist)
		{
			//Lots of magic numbers here
			//But afraid not. They are simply adjusting the sinewave
			//What I mean by that is it's purely artistic choice these numbers
			//I was changing them many times checking how it impacts the "wobbly" motion
			//They don't mean anything specific but I found them working well
			
			return (sin(dist*9+(_Time.w*6))+1)*0.5*dist +  sin(dist * 1.5f);
			//If this function seem too complicated try replacing it with this for starters:
			//return (sin(9+(_Time.w*6))+1)*0.5 + sin(1.5f);
		}


		/* There are two versions of this function
		Both are displacing the mesh
		But only one is storing how strongly a mesh is displaced (displacementStrength)
		This value is used later on to emit light based on this strength value*/
		float4 getNewVertPosition(float4 positionOS, float4 positionWS, float3 normals, inout fixed displacementStrength, float2 texcoords)
		{
			//I am sampling noise texture here
			//This is done per-vertex
			//These values are used to create some randomization in strength applied to each vertex
			float3 offset = tex2Dlod(_NoiseTex, float4((_Time.y * _NoiseSpeed) + _Tiling.xy + texcoords.xy * _Tiling.zw,0,0)) + _OffsetStrength;
			offset = clamp(offset,0,01);

			float3 collapseDir = normalize( 0-positionOS.xyz) * _CollapseStrength * offset;
			positionOS.xyz += collapseDir;
			positionOS.z += _Gravity * clamp(_CollapseStrength,0,_CollapseStrength) * distance(positionOS.xyz, 0) ;


			for (int i = 0; i < 8; i++)
			{
				//Create radial falloff which will be used to fade out displacement strength
				float dist = distance(positionWS, _ParticlePositions[i]);
				dist = 1- clamp(dist/_ParticleSizes[i],0,1); //TRANSFORM DISTANCE TO 0-1 SPACE; 0 IS FAR DISTANCE, 1 IS CLOSE
				dist = distortionFalloff(dist);// + sin(dist * 1.5f);
				dist *= _ParticleAlpha[i];
				displacementStrength += dist;
				float3 bulgeVector = mul(unity_WorldToObject, _ParticlePositions[i] - positionWS.xyz) + normals;
				positionOS.xyz +=  bulgeVector * _Strength * dist;
			}
			float4 wsPos = mul(unity_ObjectToWorld, positionOS);
			wsPos.y = clamp(wsPos.y, 0.01, 99999999);
			return mul(unity_WorldToObject, wsPos);
		} 

		float4 getNewVertPosition(float4 positionOS, float4 positionWS, float3 normals, float2 texcoords)
		{
			float3 offset = tex2Dlod(_NoiseTex, float4((_Time.y * _NoiseSpeed) + _Tiling.xy + texcoords.xy * _Tiling.zw,0,0)) + _OffsetStrength;
			offset = clamp(offset,0,01);

			float3 collapseDir = normalize( 0-positionOS.xyz) * _CollapseStrength * offset;
			positionOS.xyz += collapseDir;
			positionOS.z += _Gravity * clamp(_CollapseStrength,0,_CollapseStrength) * distance(positionOS.xyz, 0) ;

			for (int i = 0; i < 8; i++)
			{
				float dist = distance(positionWS, _ParticlePositions[i]);
				dist = 1- clamp(dist/_ParticleSizes[i],0,1); //TRANSFORM DISTANCE TO 0-1 SPACE; 0 IS FAR DISTANCE, 1 IS CLOSE
				dist = distortionFalloff(dist);// + sin(dist * 1.5f);
				dist *= _ParticleAlpha[i];
				float3 bulgeVector = mul(unity_WorldToObject, normalize(_ParticlePositions[i] - positionWS.xyz) ) + normals;
				positionOS.xyz +=  bulgeVector * _Strength * dist;
			}

			float4 wsPos = mul(unity_ObjectToWorld, positionOS);
			wsPos.y = clamp(wsPos.y, 0.01, 99999999);
			return mul(unity_WorldToObject, wsPos);
		} 


		/* Vertex shader here is responsible for displacing vertices of my meshes
		These meshes are very heavily subdivided in a 3d application
		This is because Unity Surface Shaders can't transfer data from VTX shader to Pixel shader and still use tesselation
		If you do that Unity will throw errors and shader won't compile
		
		Also, In vertex shader I check how much a vertex is displaced
		I save this value and I use it for emissive lighting in pixel shader
		The more it is displaced, the more it glows*/
		void vert (inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);

			float vertexOffset = 0.35;
			float4 bitangent = float4(cross(v.normal, v.tangent), 0);

			float4 vertexWS0 = mul(unity_ObjectToWorld, v.vertex);
			float4 vertexWS1 = mul(unity_ObjectToWorld, v.vertex + v.tangent * vertexOffset);
			float4 vertexWS2 = mul(unity_ObjectToWorld, v.vertex + bitangent * vertexOffset);


			
			float4 position = getNewVertPosition(v.vertex, vertexWS0, v.normal, o.displacementStrength, v.texcoord);
			float4 positionAndTangent = getNewVertPosition(v.vertex + v.tangent * vertexOffset, vertexWS1, v.normal, v.texcoord);
			float4 positionAndBitangent = getNewVertPosition(v.vertex + bitangent * vertexOffset, vertexWS2, v.normal, v.texcoord);

			/* OH MY GOD WHAT IS HAPPENNING HERE!
			Chill out. It is all explained here
			http://diary.conewars.com/vertex-displacement-shader/
			
			Because vertices are displaced, meaning that they have new positions
			Normals of the mesh need to be recalculated
			Otherwise cubemaps and shadowing will look very unnatural
			Article above explains one of the ways of recaltulating normals in a shader*/
			
			float4 newTangent = (positionAndTangent - position);
			float4 newBitangent = (positionAndBitangent - position);

			float3 newNormal = cross(newTangent, newBitangent).xyz;
			v.normal = newNormal;
			v.vertex = position;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed cutoffMask = tex2D(_CutoffMask, IN.uv_CutoffMask + (_Time.y * _CutoffMaskSpeed.xy)).r;

			if (cutoffMask < _CutoffThreshold)
			{
				discard;
			}


			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			fixed emissionMask = clamp(tex2D(_EmissionMask, IN.uv_EmissionMask ).r + _EmissionMaskStrength,0,1);
			float edgeGlow = 1-clamp(cutoffMask - _EdgeRadius,0,1);
			o.Emission = IN.displacementStrength * _EmissionColor * emissionMask +  (IN.displacementStrength *edgeGlow* _EdgeColor);
			o.Emission *= _EmissionStrength;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
