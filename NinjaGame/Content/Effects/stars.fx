//#################################################################################################
//
//
// A simple shader used for rendering stars. This shader wraps the stars around the screen.
//
//
//#################################################################################################

//#################################################################################################
// Uniform variables
//#################################################################################################

/// Texture to draw

uniform extern texture Texture;

// Size of the screen (x&y) and position of the camera (z&w) in the world:

uniform extern float4 ScreenSizeAndCameraPosition;

// How much camera scrolling affects scrolling of the stars

uniform extern float ParallaxScale;

//#################################################################################################
// Samplers
//#################################################################################################

sampler Sampler_Texture = sampler_state
{
	texture	= <Texture>;
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
	float3	position	: POSITION0;
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
	
	// Set output position:
	
	output.position.xy  = input.position.xy;
	output.position.z	= 0.5;
	output.position.w	= 1;
	
	// Scale size of the star according to screen size:
	
	output.position.x /= ScreenSizeAndCameraPosition.x;
	output.position.y /= ScreenSizeAndCameraPosition.y;	
	
	// Get size of the star in normalised screen coords:
	
	float sx = input.position.z / ScreenSizeAndCameraPosition.x;
	float sy = input.position.z / ScreenSizeAndCameraPosition.y;
	
	// Figure out how much to translate the star in normalised screen coords:
	
	float tx = ScreenSizeAndCameraPosition.z * ParallaxScale;
	float ty = ScreenSizeAndCameraPosition.w * ParallaxScale;
	
	tx /= ScreenSizeAndCameraPosition.x;
	ty /= ScreenSizeAndCameraPosition.y;
	
	// Clamp position coords in a 0-1 range but including size of star:
	
	float pxc = fmod( input.texcoord.z + tx , 1.0 + sx );
	float pyc = fmod( input.texcoord.w + ty , 1.0 + sy );
	
	if ( pxc < 0 ) pxc = 1.0 + pxc;
	if ( pyc < 0 ) pyc = 1.0 + pyc;
	
	// Now bring into negative -1 to 1 range (plus star size)
	
	pxc = pxc * 2 - ( 1.0 + sx );
	pyc = pyc * 2 - ( 1.0 + sy );
	
	// Calculate final position of the vertex:
	
	output.position.x += pxc;
	output.position.y += pyc;
	
	// Save rest of the output:
	
	output.texcoord.xy	= input.texcoord.xy;
	output.color		= input.color;
	
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

	// Save output texture mapped pixel:
	
	output.color = tex2D( Sampler_Texture , input.texcoord ) * input.color;
	
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