using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia;
using System.Numerics;
using System;


public class PhongRenderer
{
    private int width, height;
    private byte[] pixelBuffer;

    public float AmbientStrength = 0.1f;
    public float DiffuseStrength = 0.8f;
    public float SpecularStrength = 0.1f;


    public Vector3 LightPosition;
    public Vector3 CameraPosition = new Vector3(0, 0, 400);
    public Vector3 ObjectColor = new Vector3(1f, 0.5f, 0f); // Orange
    public float Shininess = 8f;

    public PhongRenderer(int w, int h, Vector3 startPosition)
    {
        width = w;
        height = h;
        pixelBuffer = new byte[width * height * 4];
        LightPosition = startPosition;
    }

    public void DrawPhongReflectedSphere(Vector3 center, float radius)
    {
        Array.Clear(pixelBuffer, 0, pixelBuffer.Length);

        for (int y = -(int)radius; y <= radius; y++)
        {
            for (int x = -(int)radius; x <= radius; x++)
            {
                if (x * x + y * y <= radius * radius)
                {
                    float z = (float)Math.Sqrt(radius * radius - x * x - y * y);
                    Vector3 point = new Vector3(center.X + x, center.Y + y, center.Z + z);
                    Vector3 normal = Vector3.Normalize(new Vector3(x, y, z));

                    Vector3 color = PhongReflection(point, normal);

                    int px = (int)(point.X + width / 2);
                    int py = (int)(point.Y + height / 2);

                    if (px < 0 || px >= width || py < 0 || py >= height)
                        continue;

                    int index = (py * width + px) * 4;
                    pixelBuffer[index + 0] = (byte)(color.Z * 255); // Blue
                    pixelBuffer[index + 1] = (byte)(color.Y * 255); // Green
                    pixelBuffer[index + 2] = (byte)(color.X * 255); // Red
                    pixelBuffer[index + 3] = 255; // Alpha
                }
            }
        }
    }


    private Vector3 PhongReflection(Vector3 pos, Vector3 normal)
    {
        Vector3 lightDir = Vector3.Normalize(LightPosition - pos);
        Vector3 viewDir = Vector3.Normalize(CameraPosition - pos);
        Vector3 reflectDir = Vector3.Reflect(-lightDir, normal);

        float ambient = AmbientStrength;
        float diffuse = Math.Max(Vector3.Dot(normal, lightDir), 0f) * DiffuseStrength;
        float specular = 0f;

        if (diffuse > 0)
        {
            float specAngle = Math.Max(Vector3.Dot(viewDir, reflectDir), 0f);
            specular = (float)Math.Pow(specAngle, Shininess) * SpecularStrength;
        }

        float intensity = ambient + 0.5f * diffuse + 0.5f * specular;
        intensity = Math.Clamp(intensity, 0, 1);

        return ObjectColor * intensity;
    }

    public WriteableBitmap Render() {

        var wb = new WriteableBitmap(
            new PixelSize(width, height),
            new Avalonia.Vector(96, 96),
            PixelFormat.Bgra8888
        );
        DrawPhongReflectedSphere(new Vector3(0, 0, 0), 200);
        using (var fb = wb.Lock())
        {
            System.Runtime.InteropServices.Marshal.Copy(pixelBuffer, 0, fb.Address, pixelBuffer.Length);
        }
        return wb;
    }

}
