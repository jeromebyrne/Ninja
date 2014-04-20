//#################################################################################################
//
//
// Same as single texture shader except this shader mixes two textures by a given amount.
//
//
//#################################################################################################

//#################################################################################################
// Uniform variables
//#################################################################################################

/// Transform matrix: converts from object space to the cameras screen space

uniform extern float4x4 WorldViewProjection;

/// 1st Texture to draw

uniform extern texture Texture1;

/// 2nd Texture to draw

uniform extern texture Texture2;

// Blend amount. 0 = full texture 1 , 1 = full texture 2

uniform extern float TextureMix;

//#################################################################################################
// Samplers
//#################################################################################################

sampler Sampler_Texture1 = sampler_state
{
	texture	= <Texture1>;
};	

sampler Sampler_Texture2 = sampler_state
{
	texture	= <Texture2>;
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
	float4	color		: COLOR0;
};

struct VertexShaderOutput
{
	float4	position	: POSITION;	
	float2	texcoord	: TEXCOORD0;
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
	
	output.texcoord = input.texcoord;
	output.color	= input.color;
	
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
	
	// Grab the two tex samples:
	
	float4 c1 = tex2D( Sampler_Texture1 , input.texcoord );
	float4 c2 = tex2D( Sampler_Texture2 , input.texcoord );
	
	// Multilpy by blend factors:
	
	c1 *= 1.0f - TextureMix; c2 *= TextureMix;
	
	// Mix:
	
	float4 c3 = c1 + c2;

	// Save output texture mapped pixel:
	
	output.color = c3 * input.color;
	
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