using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using tool;
using MonoGameGum;
using Gum.Forms;
using Gum.Forms.Controls;
using Gum.Wireframe;

namespace cube_game_scene
{
    public class TitleScene : BaseScene
    {
        // Gum UI 组件
        private Button _freeCubeButton;
        private Button _buildSceneButton;

        private Button _MixSceneButton;
        
        public override string Name => "TitleScene";

        public TitleScene() : base(cube_game.Core.Graphics, Matrix.CreateLookAt(new Vector3(4, 5, 8), Vector3.Zero, Vector3.Up), Matrix.CreatePerspectiveFieldOfView(
            MathHelper.PiOver4,
            cube_game.Core.GraphicsDevice.Viewport.AspectRatio,
            0.1f,
            100f))
        {
            Debug.WriteLine("[TitleScene] 构造函数被调用");
        }

        public override void Initialize()
        {
            base.Initialize();
            
            // 清理 Gum 根容器，移除之前场景的 UI 元素
            GumService.Default.Root.Children.Clear();
            
            // 创建 Gum UI 组件
            CreateGumUI();
            
            Debug.WriteLine("[TitleScene] Gum 根容器已清理，UI 重新初始化");
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
            
            Debug.WriteLine($"[TitleScene] 屏幕尺寸: {screenWidth}x{screenHeight}");
            Debug.WriteLine($"[TitleScene] Gum Canvas 尺寸: {gumCanvasWidth}x{gumCanvasHeight}");
            
            // 创建自由魔方按钮 - 在 Gum Canvas 中居中显示
            _freeCubeButton = new Button();
            _freeCubeButton.X = gumCanvasWidth / 2 - 20; // Gum Canvas 中居中（160/4=40，40/2=20）
            _freeCubeButton.Y = gumCanvasHeight / 2 - 40; // 屏幕中央偏上，增加间距
            _freeCubeButton.Width = 40;  // 160/4=40
            _freeCubeButton.Height = 5; // 48/4=12
            _freeCubeButton.Text = "Free";
            _freeCubeButton.Anchor(Gum.Wireframe.Anchor.BottomLeft);
            _freeCubeButton.Click += (sender, args) =>
            {
                Debug.WriteLine("[TitleScene] 自由魔方按钮被点击");
                cube_game.Core.ChangeScene(new FreeCubeScene());
            };
            
            // 创建构建场景按钮 - 在第一个按钮下方，增加更多间距
            _buildSceneButton = new Button();
            _buildSceneButton.X = gumCanvasWidth / 2 - 20; // Gum Canvas 中居中
            _buildSceneButton.Y = gumCanvasHeight / 2 + 15;  // 第一个按钮下方，增加更多间距
            _buildSceneButton.Width = 40;  // 160/4=40
            _buildSceneButton.Height = 12; // 48/4=12
            _buildSceneButton.Text = "Build";
            _buildSceneButton.Click += (sender, args) =>
            {
                Debug.WriteLine("[TitleScene] 构建魔方按钮被点击");
                cube_game.Core.ChangeScene(new BuildScene());
            };

            _MixSceneButton = new Button();
            _MixSceneButton.X = gumCanvasWidth / 2 - 20; // Gum Canvas 中居中
            _MixSceneButton.Y = gumCanvasHeight / 2 + 30;  // 第二个按钮下方，增加更多间距
            _MixSceneButton.Width = 40;  // 160/4=40
            _MixSceneButton.Height = 12; // 48/4=12
            _MixSceneButton.Text = "Mix";
            _MixSceneButton.Anchor(Gum.Wireframe.Anchor.TopLeft);
            _MixSceneButton.Click += (sender, args) =>
            {
                Debug.WriteLine("[TitleScene] 混合魔方按钮被点击");
                cube_game.Core.ChangeScene(new MixScene());
            };
            
            // 将按钮添加到 Gum 根容器
            _freeCubeButton.AddToRoot();
            _buildSceneButton.AddToRoot();
            _MixSceneButton.AddToRoot();
            
            Debug.WriteLine($"[TitleScene] 按钮位置: 自由魔方({_freeCubeButton.X},{_freeCubeButton.Y}), 构建魔方({_buildSceneButton.X},{_buildSceneButton.Y}), 混合魔方({_MixSceneButton.X},{_MixSceneButton.Y})");
            Debug.WriteLine("[TitleScene] Gum UI 创建成功");
        }

        public override void Update(GameTime gameTime)
        {
            // 更新 Gum 系统
            GumService.Default.Update(gameTime);
            
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Debug.WriteLine("[TitleScene] Draw 开始");
            
            // 绘制背景
            Graphics.GraphicsDevice.Clear(Color.DarkSlateGray);
            Debug.WriteLine("[TitleScene] 背景已清除为深石板灰");

            // 先调用基类的 Draw 方法
            base.Draw(gameTime);
            
            // 渲染 Gum UI - 使用正确的渲染方法
            GumService.Default.Draw();
            
            Debug.WriteLine("[TitleScene] Gum UI 渲染完成");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // 清理 Gum 资源 - 从根容器中移除按钮
                if (_freeCubeButton != null)
                {
                    _freeCubeButton.RemoveFromRoot();
                }
                if (_buildSceneButton != null)
                {
                    _buildSceneButton.RemoveFromRoot();
                }
                if (_MixSceneButton != null)
                {
                    _MixSceneButton.RemoveFromRoot();
                }
            }
            
            base.Dispose(disposing);
        }
    }
}