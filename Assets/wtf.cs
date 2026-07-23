using System.IO;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class wtf : MonoBehaviour
{
    private Texture2D texture;
    public float scale = 0.11f;
    public float scale2 = 0.5f;
    public float cutoff = 0.5f;

    public float a = 3;
    public float b = 2;
    public float c = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int width = 100;
        int height = 100;
        texture = new Texture2D(width, height);
    }

    // Update is called once per frame
    void Update()
    {
        int width = 100;
        int height = 100;

        float[] map = new float[width * height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xDist = x - 50;
                float yDist = (y - 50) * 1.3f;

                float dist = Mathf.Sqrt(xDist * xDist + yDist * yDist);
                float distMod = Mathf.Pow(dist / 50, a) * b - c;

                map[x + height * y] =  Mathf.PerlinNoise(x * scale, y * scale); // + distMod;
                map[x + height * y] += Mathf.PerlinNoise((x + 999999) * scale2, (y + 333333) * scale2);
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x + height * y] > cutoff)
                {
                    texture.SetPixel(x, y, Color.rosyBrown);
                    if (y < height - 1 && map[x + height * (y + 1)] < cutoff)
                    {
                        texture.SetPixel(x, y, Color.red);
                    }
                    else if (y > 0 && map[x + height * (y - 1)] < cutoff)
                    {
                        texture.SetPixel(x, y, Color.red);
                    }

                }
                else
                {
                    texture.SetPixel(x, y, Color.black);
                }
                //texture.SetPixel(x, y, new Color(map[x + height * y], 0f, 0f));

                //texture.SetPixel(x, y, new Color(0, Mathf.Pow(dist / 50, 3), 0));
            }
        }

        texture.Apply();

        RawImage image = GetComponent<RawImage>();
        image.texture = (Texture)texture;
    }
}
