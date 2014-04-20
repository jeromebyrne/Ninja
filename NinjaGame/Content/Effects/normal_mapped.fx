//#################################################################################################
//
//
// Shader for drawing texture mapped, normal mapped and colored primitives. Also performs the 
// outline darkening that the black edged sprite shader does
//
//
//#################################################################################################

//#################################################################################################
// Uniform variables
//#################################################################################################

/// Transform matrix: converts from object space to the cameras screen space

uniform extern float4x4 WorldViewProjection;

/// Texture to draw

uniform extern texture Texture;

/// Normal map to use

uniform extern texture NormalMap;

/// Camera position

uniform extern float2 CameraPosition;

//#################################################################################################
// Samplers
//#################################################################################################

sampler Sampler_Texture = sampler_state
{
	texture	= <Texture>;
};

sampler Sampler_Normal = sampler_state
{
	texture	= <NormalMap>;
};	

//#################################################################################################
// Structures
//#################################################################################################

struct PixelShaderOutput
{
	float4	color		: COLOR0;
};

struct PixelShaderInput
{	
	float4	texcoord	: TEXCOORD0;
	float4	wposition	: TEXCOORD2;
	float4	color		: COLOR0;
};

struct VertexShaderOutput
{
	float4	position	: POSITION;	
	float4	texcoord	: TEXCOORD0;
	float4	wposition	: TEXCOORD2;
	float4	color		: COLOR0;
};

struct VertexShaderInput
{
	float4	position	: POSITION0;
	float4	texcoord	: TEXCOORD0;
	float4  color		: COLOR0;
};

//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
// Vertex shader
//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

VertexShaderOutput vertexShader( VertexShaderInput input )
{
	// Declare output 

	VertexShaderOutput output = (VertexShaderOutput)(0);
	
	// Transform vertex by the camera transform matrix and save:
	
	output.position		= mul( input.position , WorldViewProjection );	
	
	// Save texture coordinate, color and world position of the vertex:
	
	output.texcoord		= input.texcoord;
	output.color		= input.color;
	output.wposition	= input.position;
	
	// Return output
	
	return output;
}

//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
// Pixel shader
//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

PixelShaderOutput pixelShader( PixelShaderInput input )
{
	// Declare output 

	PixelShaderOutput output = (PixelShaderOutput)(0);
	
	// Grab the normal map pixel
	
	float4 normal = tex2D( Sampler_Normal , input.texcoord.xy );
	
	// Bring back into -1 to 1 range
	
	normal.xyz -= 0.5; normal.xyz = normalize(normal.xyz);
	
	// Get vector to the camera:
	
	float2 cam_vec = CameraPosition - input.wposition.xy;
	
	// Get distance to the camera:
	
	float dist = length( cam_vec ) + 0.00001;	
	
	// Normalise the direction to the camea
	
	cam_vec.x /= dist;
	cam_vec.y /= dist;
	
	// Turn the camera vector into a 3d vector to the camera. The extra z value helps determine the intensity of normal mapping. Higher values mean duller shading.
	
	float3 cam_vec_3d; 
	
	cam_vec_3d.x =   cam_vec.x;
	cam_vec_3d.y = - cam_vec.y;
	cam_vec_3d.z =   0.75;
	
	// Normalise the 3d vector to the camera
	
	cam_vec_3d = normalize(cam_vec_3d);
	
	// Do a dot with the normal: this will determine shading
	
	float dot = dot( cam_vec_3d , normal.xyz );
	
	// Begin dulling the shading as it nears the camera position. apply a quadratic falloff
	
	const float OUTER_DIST = 100;
	
	if ( dist < OUTER_DIST )
	{
		// Amount to of regular shading to use
		
		float t = sqrt( dist / OUTER_DIST );
		
		// Modify the shading by modifying the dot
	
		dot *= t; dot += 1.0 - t;
	}

	// Grab output texture mapped pixel:
	
	output.color = tex2D( Sampler_Texture , input.texcoord.xy ) * input.color;
	
	// Modify according to normal mapping
	
	output.color.xyz *= dot;
	
	// Begin blackening as alpha decreases
	
	output.color.xyz *= output.color.a;	
	
	// Return output:
	
	return output;
}

//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
// Shader techniques
//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

technique ShaderTechnique
{
	//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
	// Pass 1
	//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
	
	pass Pass1 
	{
		vertexShader	= compile vs_2_0 vertexShader();
		pixelShader		= compile ps_2_0 pixelShader();
	}	

};