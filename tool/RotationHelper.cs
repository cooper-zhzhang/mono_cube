using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using Microsoft.Xna.Framework;


namespace tool
{
    public class RotationHelper
    {
        // 面标识符常量
        public const char FACE_UP = 'U';
        public const char FACE_DOWN = 'D';
        public const char FACE_LEFT = 'L';
        public const char FACE_RIGHT = 'R';
        public const char FACE_FRONT = 'F';
        public const char FACE_BACK = 'B';

        // 面索引常量（用于数组索引）
        public const int INDEX_FRONT = 0;  // 前(Z正)
        public const int INDEX_BACK = 1;   // 后(Z负)
        public const int INDEX_UP = 2;     // 上(Y正)
        public const int INDEX_DOWN = 3;   // 下(Y负)
        public const int INDEX_RIGHT = 4;  // 右(X正)
        public const int INDEX_LEFT = 5;   // 左(X负)

        // 旋转指令字符串常量
        public const string CMD_UP = "U";
        public const string CMD_UP_PRIME = "U'";
        public const string CMD_DOWN = "D";
        public const string CMD_DOWN_PRIME = "D'";
        public const string CMD_LEFT = "L";
        public const string CMD_LEFT_PRIME = "L'";
        public const string CMD_RIGHT = "R";
        public const string CMD_RIGHT_PRIME = "R'";
        public const string CMD_FRONT = "F";
        public const string CMD_FRONT_PRIME = "F'";
        public const string CMD_BACK = "B";
        public const string CMD_BACK_PRIME = "B'";
        public const string CMD_UP_2 = "U2";
        public const string CMD_DOWN_2 = "D2";
        public const string CMD_LEFT_2 = "L2";
        public const string CMD_RIGHT_2 = "R2";
        public const string CMD_FRONT_2 = "F2";
        public const string CMD_BACK_2 = "B2";

        // 创建旋转矩阵
        public static Matrix CreateRotationMatrix(char face, float angle)
        {
            switch (face)
            {
                case FACE_UP: // 最上层面绕Y轴旋转
                    return Matrix.CreateRotationY(-angle);
                case FACE_DOWN: // 最下层面绕Y轴旋转
                    return Matrix.CreateRotationY(angle);
                case FACE_LEFT: // 最左层面绕X轴旋转
                    return Matrix.CreateRotationX(angle);
                case FACE_RIGHT: // 最右层面绕X轴旋转
                    return Matrix.CreateRotationX(-angle);
                case FACE_FRONT: // 最前层面绕Z轴旋转
                    return Matrix.CreateRotationZ(-angle);
                case FACE_BACK: // 最后层面绕Z轴旋转
                    return Matrix.CreateRotationZ(angle);
                default:
                    return Matrix.Identity;
            }
        }

// 正值表示顺时针，负值表示逆时针
        public static float GetRotationAngle(string cmd)
        {
            switch (cmd)
            {
                case CMD_UP: // 最上层面绕Y轴旋转
                    return -MathHelper.PiOver2;
                case CMD_UP_PRIME: // 最上层面绕Y轴旋转
                    return MathHelper.PiOver2;
                case CMD_DOWN: // 最下层面绕Y轴旋转
                    return MathHelper.PiOver2;
                case CMD_DOWN_PRIME: // 最下层面绕Y轴旋转
                    return -MathHelper.PiOver2;
                case CMD_LEFT: // 最左层面绕X轴旋转
                    return MathHelper.PiOver2;
                case CMD_LEFT_PRIME: // 最左层面绕X轴旋转
                    return -MathHelper.PiOver2;
                case CMD_RIGHT: // 最右层面绕X轴旋转
                    return -MathHelper.PiOver2;
                case CMD_RIGHT_PRIME: // 最右层面绕X轴旋转
                    return MathHelper.PiOver2;
                case CMD_FRONT: // 最前层面绕Z轴旋转
                    return -MathHelper.PiOver2;
                case CMD_FRONT_PRIME: // 最前层面绕Z轴旋转
                    return MathHelper.PiOver2;
                case CMD_BACK: // 最后层面绕Z轴旋转
                    return MathHelper.PiOver2;
                case CMD_BACK_PRIME: // 最后层面绕Z轴旋转
                    return -MathHelper.PiOver2;
                case CMD_UP_2: // 最上层面绕Y轴旋转
                    return -MathHelper.Pi;
                case CMD_DOWN_2: // 最下层面绕Y轴旋转
                    return MathHelper.Pi;
                case CMD_LEFT_2: // 最左层面绕X轴旋转
                    return MathHelper.Pi;
                case CMD_RIGHT_2: // 最右层面绕X轴旋转
                    return -MathHelper.Pi;
                case CMD_FRONT_2: // 最前层面绕Z轴旋转
                    return -MathHelper.Pi;
                case CMD_BACK_2: // 最后层面绕Z轴旋转
                    return MathHelper.Pi;
                default:
                    return 0;
            }
        }


        public static Matrix CreateRotationMatrix(string face, float angle)
        {
            switch (face)
            {
                case CMD_UP: // 最上层面绕Y轴旋转
                case CMD_UP_2:
                    return Matrix.CreateRotationY(-angle);
                case CMD_UP_PRIME: // 最上层面绕Y轴旋转
                    return Matrix.CreateRotationY(angle);
                case CMD_DOWN: // 最下层面绕Y轴旋转
                case CMD_DOWN_2:
                    return Matrix.CreateRotationY(angle);
                case CMD_DOWN_PRIME:
                    return Matrix.CreateRotationY(-angle);
                case CMD_LEFT: // 最左层面绕X轴旋转
                case CMD_LEFT_2:
                    return Matrix.CreateRotationX(angle);
                case CMD_LEFT_PRIME:
                    return Matrix.CreateRotationX(-angle);
                case CMD_RIGHT: // 最右层面绕X轴旋转
                case CMD_RIGHT_2:
                    return Matrix.CreateRotationX(-angle);
                case CMD_RIGHT_PRIME: // 最右层面绕X轴旋转
                    return Matrix.CreateRotationX(angle);
                case CMD_FRONT: // 最前层面绕Z轴旋转
                case CMD_FRONT_2:
                    return Matrix.CreateRotationZ(-angle);
                case CMD_FRONT_PRIME: // 最前层面绕Z轴旋转
                    return Matrix.CreateRotationZ(angle);
                case CMD_BACK: // 最后层面绕Z轴旋转
                case CMD_BACK_2:
                    return Matrix.CreateRotationZ(angle);
                case CMD_BACK_PRIME: // 最后层面绕Z轴旋转
                    return Matrix.CreateRotationZ(-angle);
                default:
                    return Matrix.Identity;
            }
        }


    }
}