sampler2D implicitInputSampler : register(S0);
float4 targetColor : register(C0);

float4 main(float2 uv : TEXCOORD) : COLOR {
    float4 color = tex2D(implicitInputSampler, uv);
    return color * targetColor;
}
