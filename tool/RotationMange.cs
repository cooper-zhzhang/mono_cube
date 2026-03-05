using System.Reflection.Metadata.Ecma335;
using Microsoft.Xna.Framework;


namespace tool
{
    public class RotationHelper
    {
        // 创建旋转矩阵
        public static Matrix CreateRotationMatrix(char face, float angle)
        {
            switch (face)
            {
                case 'u': // 最上层面绕Y轴旋转
                    return Matrix.CreateRotationY(-angle);
                case 'd': // 最下层面绕Y轴旋转
                    return Matrix.CreateRotationY(angle);
                case 'l': // 最左层面绕X轴旋转
                    return Matrix.CreateRotationX(angle);
                case 'r': // 最右层面绕X轴旋转
                    return Matrix.CreateRotationX(-angle);
                case 'f': // 最前层面绕Z轴旋转
                    return Matrix.CreateRotationZ(-angle);
                case 'b': // 最后层面绕Z轴旋转
                    return Matrix.CreateRotationZ(angle);
                default:
                    return Matrix.Identity;
            }
        }


        public static float GetRotationAngle(string face)
        {
            switch (face)
            {
                case "u": // 最上层面绕Y轴旋转
                    return -MathHelper.PiOver2;
                case "u'": // 最上层面绕Y轴旋转
                    return MathHelper.PiOver2;
                case "d": // 最下层面绕Y轴旋转
                    return MathHelper.PiOver2;
                case "d'": // 最下层面绕Y轴旋转
                    return -MathHelper.PiOver2;
                case "l": // 最左层面绕X轴旋转
                    return MathHelper.PiOver2;
                case "l'": // 最左层面绕X轴旋转
                    return -MathHelper.PiOver2;
                case "r": // 最右层面绕X轴旋转
                    return -MathHelper.PiOver2;
                case "r'": // 最右层面绕X轴旋转
                    return MathHelper.PiOver2;
                case "f": // 最前层面绕Z轴旋转
                    return -MathHelper.PiOver2;
                case "f'": // 最前层面绕Z轴旋转
                    return MathHelper.PiOver2;
                case "b": // 最后层面绕Z轴旋转
                    return MathHelper.PiOver2;
                case "b'": // 最后层面绕Z轴旋转
                    return -MathHelper.PiOver2;
                case "u2": // 最上层面绕Y轴旋转
                    return -MathHelper.Pi;
                case "d2": // 最下层面绕Y轴旋转
                    return MathHelper.Pi;
                case "l2": // 最左层面绕X轴旋转
                    return MathHelper.Pi;
                case "r2": // 最右层面绕X轴旋转
                    return -MathHelper.Pi;
                case "f2": // 最前层面绕Z轴旋转
                    return -MathHelper.Pi;
                case "b2": // 最后层面绕Z轴旋转
                    return MathHelper.Pi;
                default:
                    return 0;
            }
        }


        public static Matrix CreateRotationMatrix(string face, float angle)
        {
            switch (face)
            {
                case "u": // 最上层面绕Y轴旋转
                    return Matrix.CreateRotationY(-angle);
                case "d": // 最下层面绕Y轴旋转
                    return Matrix.CreateRotationY(angle);
                case "l": // 最左层面绕X轴旋转
                    return Matrix.CreateRotationX(angle);
                case "r": // 最右层面绕X轴旋转
                    return Matrix.CreateRotationX(-angle);
                case "f": // 最前层面绕Z轴旋转
                    return Matrix.CreateRotationZ(-angle);
                case "b": // 最后层面绕Z轴旋转
                    return Matrix.CreateRotationZ(angle);
                default:
                    return Matrix.Identity;
            }
        }


    }
}