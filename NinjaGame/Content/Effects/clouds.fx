//#################################################################################################
//
//
// Shader for making clouds wisp and distort.
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

//#################################################################################################
// Constants
//#################################################################################################

// How much overall position affects the distortion 

const float POSITION_IMPORTANCE = 0.02;

// How important the Y position of the pixel is when affecting distortion

const float Y_POSITION_IMPORTANCE = 2.0;

// How important the color of the pixel when affecting distortion

const float COLOR_IMPORTANCE = 10.5;

// How much distortion takes place

const float DISTORTION_SCALE = 0.035;

//#################################################################################################
// Samplers
//#################################################################################################

sampler Sampler_Texture = sampler_state
{
	texture = <Texture>;
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
	float2	texcoord	: TEXCOORD0;
	float2  wposition	: TEXCOORD1;
	float4	color		: COLOR0;
};

struct VertexShaderOutput
{
	float4	position	: POSITION;	
	float2	texcoord	: TEXCOORD0;
	float2  wposition	: TEXCOORD1;
	float4	color		: COLOR0;	
};

struct VertexShaderInput
{
	float4	position	: POSITION0;
	float2	texcoord	: TEXCOORD0;
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
	
	output.position = mul( input.position , WorldViewProjection );
	
	// Save texture coordinate and color:
	
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
	
	// Sample the cloud texture
	
	float4 tex_sample = tex2D( Sampler_Texture , input.texcoord );
	
	// Figure out a direction to distort the cloud in:
	
	float x = cos( tex_sample.r * COLOR_IMPORTANCE + ( input.wposition.x + input.wposition.y * Y_POSITION_IMPORTANCE ) * POSITION_IMPORTANCE );
	float y = sin( tex_sample.r * COLOR_IMPORTANCE + ( input.wposition.x + input.wposition.y * Y_POSITION_IMPORTANCE ) * POSITION_IMPORTANCE );
	
	float2 dir = float2( x , y );
	
	// Normalise the direction
	
	dir = normalize(dir);
	
	// Scale by distortion amount and alpha:
	
	dir *= DISTORTION_SCALE * tex_sample.a;
	
	// Calculate new texture coordinate:
	
	float2 tc = dir + input.texcoord.xy;

	// Grab output color with distortion taken into account:
	
	output.color = tex2D( Sampler_Texture , tc ) * input.color;
	
	// Reduce color intensity with alpha
	
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