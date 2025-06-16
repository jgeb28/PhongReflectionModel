using Avalonia;
using Avalonia.Input;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Interactivity;
using System;
using System.Numerics;

namespace PhongModel
{
    public partial class MainWindow : Window
    {
        private readonly int width = 800;  
        private readonly int height = 800;
        private readonly PhongRenderer renderer;
        private Vector3 currentLightPosition;

        private float lightRadius = 300f;
        private float lightTheta = 800f; // equator angle
        private float lightPhi = MathF.PI / 2; // start at equator (90 degrees)

        private const float AngleStep = 0.1f; // radians
        private const float RadiusStep = 10f;

        public MainWindow()
        {
            InitializeComponent();

            currentLightPosition = new Vector3(
                lightRadius * MathF.Sin(lightPhi) * MathF.Cos(lightTheta),
                lightRadius * MathF.Cos(lightPhi),
                lightRadius * MathF.Sin(lightPhi) * MathF.Sin(lightTheta) );
            renderer = new PhongRenderer(width, height, currentLightPosition );

            MatteRadio.IsCheckedChanged += MaterialRadio_Checked;
            WoodRadio.IsCheckedChanged += MaterialRadio_Checked;
            PlasticRadio.IsCheckedChanged += MaterialRadio_Checked;
            MetallicRadio.IsCheckedChanged += MaterialRadio_Checked;


            Render();

            this.KeyDown += MainWindow_KeyDown;

            this.Focus();
        }

        private void MaterialRadio_Checked(object? sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.IsChecked == true)
            {
                switch (rb.Name)
                {
                    case "MatteRadio":
                        renderer.Shininess = 2f;     // Matte (dull)
                        renderer.AmbientStrength = 0.1f;
                        renderer.DiffuseStrength = 0.8f;
                        renderer.SpecularStrength = 0.0f;
                        break;
                    case "WoodRadio":
                        renderer.Shininess = 8f;     // Matte (dull)
                        renderer.AmbientStrength = 0.1f;
                        renderer.DiffuseStrength = 0.7f;
                        renderer.SpecularStrength = 0.1f;
                        break;
                    case "PlasticRadio":
                        renderer.Shininess = 32f;     // Plastic
                        renderer.AmbientStrength = 0.1f;
                        renderer.DiffuseStrength = 0.6f;
                        renderer.SpecularStrength = 0.6f;
                        break;
                    case "MetallicRadio":
                        renderer.Shininess = 128f;     // Plastic
                        renderer.AmbientStrength = 0.05f;
                        renderer.DiffuseStrength = 0.4f;
                        renderer.SpecularStrength = 1.0f;
                        break;
                }

                Render();
            }

        }


        private void Render()
        {
            renderer.LightPosition = currentLightPosition;
            var bmp = renderer.Render(); 
            OutputImage.Source = bmp;
        }

        private void MainWindow_KeyDown(object? sender, KeyEventArgs e)
        {
             switch (e.Key)
            {
                case Key.A:
                    lightTheta -= AngleStep; // rotate left
                    if (lightTheta < 0) lightTheta += 2 * MathF.PI;
                    break;
                case Key.D:
                    lightTheta += AngleStep; // rotate right
                    if (lightTheta > 2 * MathF.PI) lightTheta -= 2 * MathF.PI;
                    break;
                case Key.W:
                    lightPhi -= AngleStep; // rotate up
                    if (lightPhi > 2 * MathF.PI) lightPhi += 2 * MathF.PI;
                    break;
                case Key.S:
                    lightPhi += AngleStep; // rotate down
                    if (lightPhi > 2 * MathF.PI) lightPhi -= 2 * MathF.PI;
                    break;
                case Key.E:
                    lightRadius += RadiusStep; // move away from center
                    break;
                case Key.Q:
                    lightRadius -= RadiusStep; // move toward center
                    if (lightRadius < 10f) lightRadius = 10f; // prevent inside sphere
                    break;
            }
            currentLightPosition = new Vector3(
                lightRadius * MathF.Sin(lightPhi) * MathF.Cos(lightTheta),
                lightRadius * MathF.Cos(lightPhi),
                lightRadius * MathF.Sin(lightPhi) * MathF.Sin(lightTheta)
            );

            Render();
        }

    }
}
