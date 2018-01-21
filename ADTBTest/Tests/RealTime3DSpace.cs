using System;
using System.Threading;

using System.Diagnostics;

using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Windows;
using Color = SharpDX.Color;
using ADTB;

namespace ADTBTest
{
    public class RealTime3DSpace : TestClass
    {
        private RenderForm form;
        private bool formLoaded = false;

        const int lockedFPS = 30;
        const int smoothLevel = 10;

        delegate void CloseFormCallback();

        private void CloseForm()
        {
            if (form.InvokeRequired)
            {
                CloseFormCallback d = new CloseFormCallback(CloseForm);
                form.Invoke(d, new object[] { });
            }
            else form.Close();
        }

        [STAThread]
        public override void Run(object none)
        {
            Init();

            Console.WriteLine("Нажмите Esc для выхода. . .\n");
            var formTrhead = new Thread(p =>
            {

                form = new RenderForm("Rotation Test");

                // Creates the Device
                var direct3D = new Direct3D();
                var device = new Device(direct3D, 0, DeviceType.Hardware, form.Handle, CreateFlags.HardwareVertexProcessing, new PresentParameters(form.ClientSize.Width, form.ClientSize.Height));

                // Creates the VertexBuffer
                var vertices = new VertexBuffer(device, Utilities.SizeOf<Vector4>() * 2 * 36, Usage.WriteOnly, VertexFormat.None, Pool.Managed);
                vertices.Lock(0, 0, LockFlags.None).WriteRange(
                    new[] 
                {
                    new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f), // Front
                    new Vector4(-1.0f,  1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                    new Vector4( 1.0f,  1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                    new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                    new Vector4( 1.0f,  1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                    new Vector4( 1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                    new Vector4(-1.0f, -1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f), // BACK
                    new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                    new Vector4(-1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                    new Vector4(-1.0f, -1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                    new Vector4( 1.0f, -1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                    new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                    new Vector4(-1.0f, 1.0f, -1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f), // Top
                    new Vector4(-1.0f, 1.0f,  1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                    new Vector4( 1.0f, 1.0f,  1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                    new Vector4(-1.0f, 1.0f, -1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                    new Vector4( 1.0f, 1.0f,  1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                    new Vector4( 1.0f, 1.0f, -1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                    new Vector4(-1.0f,-1.0f, -1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f), // Bottom
                    new Vector4( 1.0f,-1.0f,  1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                    new Vector4(-1.0f,-1.0f,  1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                    new Vector4(-1.0f,-1.0f, -1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                    new Vector4( 1.0f,-1.0f, -1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                    new Vector4( 1.0f,-1.0f,  1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                    new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f), // Left
                    new Vector4(-1.0f, -1.0f,  1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                    new Vector4(-1.0f,  1.0f,  1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                    new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                    new Vector4(-1.0f,  1.0f,  1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                    new Vector4(-1.0f,  1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                    new Vector4( 1.0f, -1.0f, -1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f), // Right
                    new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                    new Vector4( 1.0f, -1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                    new Vector4( 1.0f, -1.0f, -1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                    new Vector4( 1.0f,  1.0f, -1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                    new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                });
                vertices.Unlock();

                // Compiles the effect
                var effect = Effect.FromFile(device, "MiniCube.fx", ShaderFlags.None);

                // Allocate Vertex Elements
                var vertexElems = new[] {
                    new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Position, 0),
                    new VertexElement(0, 16, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Color, 0),
                    VertexElement.VertexDeclarationEnd
                };

                // Creates and sets the Vertex Declaration
                var vertexDecl = new VertexDeclaration(device, vertexElems);
                device.SetStreamSource(0, vertices, 0, Utilities.SizeOf<Vector4>() * 2);
                device.VertexDeclaration = vertexDecl;

                // Get the technique
                var technique = effect.GetTechnique(0);
                var pass = effect.GetPass(technique, 0);

                // Prepare matrices
                var view = Matrix.LookAtLH(new Vector3(-10, 10, -10), new Vector3(0, 0, 0), Vector3.UnitY);
                var proj = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, form.ClientSize.Width / (float)form.ClientSize.Height, 0.1f, 100.0f);
                var viewProj = Matrix.Multiply(view, proj);

                // Use clock
                var clock = new Stopwatch();
                clock.Start();
                ADTBSmoothResultStack smoothResultStack = new ADTBSmoothResultStack(smoothLevel);
                formLoaded = true;

                form.FormClosed += (s, a) =>
                {
                    form = null;
                };

                RenderLoop.Run(form, () =>
                {
                    var time = clock.ElapsedMilliseconds / 1000.0f;

                    var data = arduino.GetNext();
                    if(data.Status != TransferStatus.OK)
                    {
                        Console.WriteLine("Ошибка получения данных. Статус {0}", data.Status);
                        arduino.Disconnect();
                        Error();
                    }

                    //var normData = data.Normalize(ADTB.ADTBDataPacket.AngleFormat.Degrees, ADTB.ADTBDataPacket.TempFormat.Celsius);
                    smoothResultStack.Push(data.Normalize());
                    var normData = smoothResultStack.Current();

                    device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                    device.BeginScene();

                    effect.Technique = technique;
                    effect.Begin();
                    effect.BeginPass(0);

                    var worldViewProj = Matrix.RotationYawPitchRoll(0, - normData.Roll * 0.0174533f, normData.Pitch * 0.0174533f)
                                        * viewProj; 

                    effect.SetValue("worldViewProj", worldViewProj);

                    device.DrawPrimitives(PrimitiveType.TriangleList, 0, 12);
                    
                    //Draw axis and grid
                    Line line = new Line(device)
                    {
                        Width = 2
                    };
                    line.Begin();
                    ColorBGRA lineColor = new ColorBGRA(50, 50, 50, 255);
                    for (int i = -5; i <= 5; i++)
                        for (int j = -5; j <= 5; j++)
                        {
                            line.DrawTransform(new Vector3[] { new Vector3(-i, 0, -j), new Vector3(i, 0, -j) }, worldViewProj, lineColor);
                            line.DrawTransform(new Vector3[] { new Vector3(-i, 0, -j), new Vector3(-i, 0, j) }, worldViewProj, lineColor);
                        }
                    lineColor = new ColorBGRA(255, 255, 0, 255);
                    line.DrawTransform(new Vector3[] { new Vector3(0,0,-5), new Vector3(0,0,5) }, worldViewProj, lineColor);
                    lineColor = new ColorBGRA(0, 255, 0, 255);
                    line.DrawTransform(new Vector3[] { new Vector3(0, -5, 0), new Vector3(0, 5, 0) }, worldViewProj, lineColor);
                    lineColor = new ColorBGRA(0, 0, 255, 255);
                    line.DrawTransform(new Vector3[] { new Vector3(-5, 0, 0), new Vector3(5, 0, 0) }, worldViewProj, lineColor);
                    line.End();
                    line.Dispose();

                    effect.EndPass();
                    effect.End();

                    device.EndScene();
                    device.Present();
                });

                effect.Dispose();
                vertices.Dispose();
                device.Dispose();
                direct3D.Dispose();
            });

            formTrhead.Start();
            while (!formLoaded) Thread.Sleep(10);
            while (form != null)
            {
                if (Console.KeyAvailable)
                    if (Console.ReadKey().Key == ConsoleKey.Escape)
                    {
                        CloseForm();
                        formTrhead.Abort();
                        arduino.Disconnect();
                        return;
                    }
            }
            arduino.Disconnect();
        }

        private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            CloseForm();
        }
    }
}
