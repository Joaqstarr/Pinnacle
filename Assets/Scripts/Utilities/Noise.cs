using UnityEngine;

namespace Utilities
{
    public class Noise
    {
        public static Texture2D GetNoiseMap(int width, int height, float scale)
        {
            Texture2D noiseMapTexture = new Texture2D(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float noiseValue = Mathf.PerlinNoise((float)x / width * scale, (float)y / height * scale);
                    
                    noiseMapTexture.SetPixel(x, y, new Color(0, noiseValue, 0));
                }
            }
            
            noiseMapTexture.Apply();
            return noiseMapTexture;
        }
        
        
    }
}