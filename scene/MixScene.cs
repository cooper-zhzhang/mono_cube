using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using tool;
using MonoGameGum;
using Gum.Forms;
using Gum.Forms.Controls;
using Gum.Wireframe;

namespace cube_game_scene
{
    public class MixScene : cube_game_scene.BaseScene
    {
        // Gum UI 组件
        private Panel _controlPanel;
        private Button _backButton;
        
        // 3D 渲染相关
        private VertexBuffer _vertexBuffer;

        private cube_obj.CubePiece _cubePiece;
        private BasicEffect _basicEffect;
        private float _rotation = 0f;
        private cube_obj.Cube _cube;

        public MixScene() : base(cube_game.Core.Graphics, Matrix.CreateLookAt(new Vector3(4, 5, 8), Vector3.Zero, Vector3.Up),  Matrix.CreatePerspectiveFieldOfView(
            MathHelper.PiOver4,
            cube_game.Core.GraphicsDevice.Viewport.AspectRatio,
            0.1f,
            100f))
        {
            Debug.WriteLine("[MixScene] 构造函数被调用");
            _cube = new cube_obj.Cube(Graphics.GraphicsDevice, "UUUUUUUUURRRRRRRRRFFFFFFFFFDDDDDDDDDLLLLLLLLLBBBBBBBBB", this);
            _cube.createCubeByStage();
        }

        public override string Name => "MixScene";

        public override void Initialize()
        {
            base.Initialize();
            
            // 清理 Gum 根容器，移除之前场景的 UI 元素
            GumService.Default.Root.Children.Clear();
            
            // 创建 Gum UI 组件
            CreateGumUI();
            
            // 初始化 3D 渲染
            Initialize3DRendering();
            
            Debug.WriteLine("[MixScene] Gum 根容器已清理，UI 重新初始化");
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        private void CreateGumUI()
        {
            // 获取屏幕尺寸（注意：Gum 使用 1/4 尺寸的 Canvas）
            int screenWidth = Graphics.GraphicsDevice.Viewport.Width;
            int screenHeight = Graphics.GraphicsDevice.Viewport.Height;
            
            // Gum 的 Canvas 尺寸是屏幕尺寸的 1/4
            float gumCanvasWidth = screenWidth / 4.0f;
            float gumCanvasHeight = screenHeight / 4.0f;
            
            Debug.WriteLine($"[MixScene] 屏幕尺寸: {screenWidth}x{screenHeight}");
            Debug.WriteLine($"[MixScene] Gum Canvas 尺寸: {gumCanvasWidth}x{gumCanvasHeight}");
            
            // 创建控制面板
            _controlPanel = new Panel();
            _controlPanel.X = gumCanvasWidth - 80; // 右侧
            _controlPanel.Y = 10; // 顶部
            _controlPanel.Width = 70;  // 面板宽度
            _controlPanel.Height = 50;  // 面板高度
            
            // 创建返回按钮 - 放置在面板内
            _backButton = new Button();
            _backButton.X = 15; // 面板内居中 (70-40)/2=15
            _backButton.Y = 10; // 面板顶部
            _backButton.Width = 40;  // 160/4=40
            _backButton.Height = 12; // 48/4=12
            _backButton.Text = "返回";
            _backButton.Click += (sender, args) =>
            {
                Debug.WriteLine("[MixScene] 返回按钮被点击");
                cube_game.Core.ChangeScene(new TitleScene());
            };
            
            // 将按钮添加到面板
            _controlPanel.AddChild(_backButton);
            
            // 将面板添加到 Gum 根容器
            _controlPanel.AddToRoot();
            
            Debug.WriteLine($"[MixScene] 控制面板位置: ({_controlPanel.X},{_controlPanel.Y})");
            Debug.WriteLine($"[MixScene] 返回按钮位置: ({_backButton.X},{_backButton.Y})");
            Debug.WriteLine("[MixScene] Gum UI 创建成功");
        }

        private void Initialize3DRendering()
        {
            // 创建基本效果
            _basicEffect = new BasicEffect(Graphics.GraphicsDevice);
            _basicEffect.VertexColorEnabled = true;
            _basicEffect.LightingEnabled = false;
            
            // 创建正方体的顶点数据
            CreateCubeVertices();
            
            Debug.WriteLine("[MixScene] 3D 渲染初始化完成");
        }

        private void CreateCubeVertices()
        {
            cube_obj.CubePiece cubePiece = new cube_obj.CubePiece(Graphics.GraphicsDevice, Vector3.Zero, new Color[]
            {
                Color.Red,     // 后面
                Color.Blue,    // 前面
                Color.Green,   // 左面
                Color.Yellow,  // 右面
                Color.Orange,  // 下面
                Color.Purple   // 上面
            });
            _cubePiece = cubePiece;

            Debug.WriteLine("[MixScene] 正方体顶点数据创建完成");
        }

        public override void Update(GameTime gameTime)
        {
            // 更新 Gum 系统
            GumService.Default.Update(gameTime);
            
            // 更新 3D 旋转
            _rotation += (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // 重置图形设备状态（确保与 FreeScene 一致）
            Graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Graphics.GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
            Graphics.GraphicsDevice.BlendState = BlendState.Opaque;


            base.Draw(gameTime);

            // 使用与 FreeScene 相同的图形设备清除
            Graphics.GraphicsDevice.Clear(Color.Gray);
            _cube.Draw(gameTime);

            // // 渲染 3D 正方体
            // Draw3DCube();
            
            // 渲染 Gum UI
            GumService.Default.Draw();
            
            Debug.WriteLine("[MixScene] 混合界面渲染完成");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // 清理 Gum 资源
                if (_backButton != null)
                {
                    _backButton.RemoveFromRoot();
                }
                
                // 清理 3D 资源
                _vertexBuffer?.Dispose();
                _basicEffect?.Dispose();
                
                Debug.WriteLine("[MixScene] 资源清理完成");
            }
            
            base.Dispose(disposing);
        }
    }
}