using System;
using System.Diagnostics;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace PRI_116_KP_Space_Martynov_v0._0._1
{
    class SolarSystem
    {
        /// <summary>
        /// Координаты положения объектов
        /// </summary>
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public bool orbits { get; set; }
        /// <summary>
        /// Вид сверху
        /// </summary>
        public bool viewFromAbove { get; set; }
        public bool playColorsFract { get; set; }
        private double sizeSun { get; set; }
        public SolarSystem(bool orbits, bool viewFromAbove, double sizeSun)
        {
            this.orbits = orbits;
            this.viewFromAbove = viewFromAbove;
            this.playColorsFract = false;
            this.sizeSun = sizeSun;
        }

        /// <summary>
        /// Координаты для положения камеры
        /// </summary>
        public struct CoordsPositions
        {
            public double X;
            public double Y;
            public double Z;
        }
        //Позиции просмотра системы
        // x - в минус вправо, y - в минус вверх, z - в минус назад
        public CoordsPositions[] position = new[]{
            new CoordsPositions{X = -1.4, Y = -2.2, Z = -15}, //пленеты земной группы
            new CoordsPositions{X = -2.73, Y = -6.5, Z = -26.91}, //пленеты гиганты
            new CoordsPositions{X = -3.08, Y = -6.1, Z = -29.98}, //камета Галлея

            new CoordsPositions{X = -6.62, Y = 0.1, Z = -1.18},  //Меркурий
            new CoordsPositions{X = -8.19, Y = 0.1, Z = -1},  //Венера
            new CoordsPositions{X = -9.71, Y = 0.1, Z = -1},  //Земля
            new CoordsPositions{X = -10.72, Y = 0.12, Z = -1.18},//Марс
            new CoordsPositions{X = -13.51, Y = 0.1, Z = -2.71}, //Юпитер
            new CoordsPositions{X = -15.62, Y = 0.1, Z = -2.58}, //Стурн
            new CoordsPositions{X = -17.12, Y = 0.1, Z = -2.58}, //Уран
            new CoordsPositions{X = -19.20, Y = 0.3, Z = -1.58}, //Нептун                    
            new CoordsPositions{X = 0, Y = 0, Z = 0}, //Центр
            new CoordsPositions{X = 0.65, Y = 0.359, Z = 6.67}, //Фрактал 
        };
        /// <summary>
        /// Переменная для задания текстур
        /// </summary>
        private Bitmap[] pictures = new Bitmap[11];
        public Bitmap[] Pictures
        {
            get
            {
                return pictures;
            }
            set
            {
                pictures = value;
            }
        }

        /// <summary>
        /// Отрисовка космического пространства
        /// </summary>
        /// <param name="pic"></param>
        /// <param name="speed_os"></param>
        public void DrawSpace(Bitmap pic, double speed_os)
        {
            GLTexture.LoadTexture(pic);
            GL.Enable(EnableCap.Texture2D);
            Sphere(32, 10, 10, 0, 0, 0, speed_os, true); //z меньше вперед
            GL.Disable(EnableCap.Texture2D);
        }

        public void DrawSun(Bitmap pic, double Xs, double Ys, double Zs, double speed_os)
        {
            GLTexture.LoadTexture(pic);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Light0);
            GL.PushMatrix();
            GL.Rotate(speed_os, 1.0, 1.0, 1.0);
            Sphere(sizeSun, 25, 25, Xs, Ys, Zs, speed_os);
            GL.PopMatrix();
            GL.Disable(EnableCap.Light0);
            GL.Disable(EnableCap.Texture2D);
        }
        /// <summary>
        /// моделируем планеты
        /// </summary>
        /// <param name="pic">текстура</param>
        /// <param name="speed">скорость</param>
        /// <param name="remoteness">удаленность</param>
        /// <param name="radius">радиус</param>
        /// <param name="orbits"></param>
        /// <param name="position">горизонально ровная позиция орбит планет</param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        public CoordsPositions DrawPlanet(Bitmap pic, double speed, double rotateSpeed, double remoteness = 1.5, double radius = 0.2, double position = 238, int nx = 20, int ny = 20)
        {
            GLTexture.LoadTexture(pic);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Light0); //освещение объектов
            GL.PushMatrix();
            if(!viewFromAbove)
                GL.Rotate(position, 1.0, 1.0, 1.0);
            CoordsPositions coord = Sphere(radius, nx, ny, Math.Sin(speed) * remoteness, Math.Cos(speed) * remoteness, 0, rotateSpeed, true);

            GL.Begin(PrimitiveType.LineLoop);
            //GL.Vertex2(0.0f, 0.0f); //начальная позиция
            //Рисуем орбиты планет вокруг солнца
            if (orbits)
            {
                float a = 0f;
                for (int i = 0; i <= 50; i++)
                {
                    a = (float)i / 50.0f * 3.1415f * 2.0f;
                    GL.Vertex2(Math.Sin(a) * remoteness, Math.Cos(a) * remoteness);
                }
            }
            GL.End();
            GL.PopMatrix();
            GL.Disable(EnableCap.Light0);
            GL.Disable(EnableCap.Texture2D);
            return coord;
        }

        /// <summary>
        /// Отрисовка сферы
        /// </summary>
        /// <param name="r">радиус</param>
        /// <param name="nx">Количество граней по горизонтали</param>
        /// <param name="ny">Количество граней по вертикали</param>
        /// <param name="sx">положение по координате X</param>
        /// <param name="sy">положение по координате Y</param>
        /// <param name="sz">положение по координате Z</param>
        /// <param name="speed">Скорость вращения вокруг своей оси</param>
        /// <param name="rotate_texture">вращать текстуру</param>
        public  CoordsPositions Sphere(double r, int nx, int ny, double sx, double sy, double sz, double speed, bool rotate_texture = true)
        {
            int ix, iy;
            double x, y, z, tex_x, tex_y;

            for (iy = 0; iy < ny; ++iy)
            {
                tex_y = (double)iy / (double)ny;

                //начинаем рисовать четырехугольник
                GL.Begin(PrimitiveType.QuadStrip);
                for (ix = 0; ix <= nx; ++ix)
                {
                    //вращение вокруг своей оси
                    tex_x = (double)ix / (double)nx + (rotate_texture ? speed - Math.Floor(speed) : 0);
                    x = r * Math.Sin(iy * Math.PI / ny) * Math.Cos(2 * ix * Math.PI / nx) + sx;
                    y = r * Math.Sin(iy * Math.PI / ny) * Math.Sin(2 * ix * Math.PI / nx) + sy;
                    z = r * Math.Cos(iy * Math.PI / ny) + sz;
                    GL.Normal3(x, y, z);//нормаль направлена от центра
                    GL.TexCoord2(tex_x, tex_y); //задаем координаты текстуры для вращения вокруг своей оси
                    GL.Vertex3(x, y, z); //Задаем координаты точек

                    //задаем координаты перемещения планет
                    x = r * Math.Sin((iy + 1) * Math.PI / ny) * Math.Cos(2 * ix * Math.PI / nx) + sx;
                    y = r * Math.Sin((iy + 1) * Math.PI / ny) * Math.Sin(2 * ix * Math.PI / nx) + sy;
                    z = r * Math.Cos((iy + 1) * Math.PI / ny) + sz;
                    GL.Normal3(x, y, z);
                    GL.TexCoord2(tex_x, tex_y + 1.0 / (double)ny);
                    GL.Vertex3(x, y, z);
                    if (iy == 1 && ix == 1)
                        X = -x; Y = -y; Z = -z;
                }
                GL.End();
            }
            CoordsPositions coord = new CoordsPositions { X = X, Y = Y, Z = Z };
            return coord;
        }

        /// <summary>
        /// моделируем комету
        /// </summary>
        /// <param name="pic">текстура</param>
        /// <param name="speed">скорость</param>
        /// <param name="remoteness">удаленность</param>
        /// <param name="radius">радиус</param>
        /// <param name="orbits"></param>
        /// <param name="position">горизонально ровная позиция орбит планет</param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        public void DrawComet(Bitmap pic, double speed, double remoteness = 1.5, double radius = 0.2, double position = 250, int nx = 5, int ny = 5)
        {
            GLTexture.LoadTexture(pic);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Light0); //освещение объектов
            GL.PushMatrix();
            GL.Rotate(position, 1.0, 1.0, 1.0);
            СometRun(radius, nx, ny, Math.Sin(speed) * remoteness, Math.Cos(speed) * remoteness, 0, speed, true);

            GL.Begin(PrimitiveType.LineLoop);
            //GL.Vertex2(0.0f, 0.0f); 
            //Рисуем орбиту каметы
            if (orbits)
            {
                float a = 0f;
                for (int i = 0; i <= 50; i++)
                {
                    a = (float)i / 50.0f * 3.1415f * 2.0f;
                    GL.Vertex2(Math.Sin(a) * remoteness, Math.Cos(a) * remoteness);
                }
            }
            GL.End();
            GL.PopMatrix();
            GL.Disable(EnableCap.Light0);
            GL.Disable(EnableCap.Texture2D);
        }
        /// <summary>
        /// Отрисовка каметы
        /// </summary>
        /// <param name="r">радиус</param>
        /// <param name="nx">Количество граней по горизонтали</param>
        /// <param name="ny">Количество граней по вертикали</param>
        /// <param name="sx"></param>
        /// <param name="sy"></param>
        /// <param name="sz"></param>
        /// <param name="speed"></param>
        /// <param name="rotate_texture"></param>
        public static void СometRun(double r, int nx, int ny, double sx, double sy, double sz, double speed, bool rotate_texture = false)
        {
            int ix, iy;
            double x, y, z, tex_x, tex_y;

            for (iy = 0; iy < ny; ++iy)
            {
                tex_y = (double)iy / (double)ny;

                //начинаем рисовать четырехугольник
                GL.Begin(PrimitiveType.QuadStrip);
                for (ix = 0; ix <= nx; ++ix)
                {
                    tex_x = (double)ix / (double)nx + (rotate_texture ? speed - Math.Floor(speed) : 0);
                    x = r * Math.Sin(iy * Math.PI / ny) * Math.Cos(2 * ix * Math.PI / nx) + sx;
                    y = r * Math.Sin(iy * Math.PI / ny) * Math.Sin(ix * Math.PI / nx) + sy;
                    z = r * Math.Cos(iy * Math.PI / ny) + sz;
                    GL.Normal3(x, y, z);//нормаль направлена от центра
                    GL.TexCoord2(tex_x, tex_y); //задаем координаты текстуры
                    GL.Vertex3(x, y, z); //Задаем координаты точек

                    x = r * Math.Sin((iy + 1) * Math.PI / ny) * Math.Cos(2 * ix * Math.PI / nx) + sx;
                    y = r * Math.Sin((iy + 1) * Math.PI / ny) * Math.Sin(ix * Math.PI / nx) + sy;
                    z = r * Math.Cos((iy + 1) * Math.PI / ny) + sz;
                    GL.Normal3(x, y, z);
                    GL.TexCoord2(tex_x, tex_y + 3.0 / (double)ny);
                    GL.Vertex3(x, y, z);
                }
                GL.End();
            }
        }
        /// <summary>
        /// Отрисовка фрактала "Дерево Пифагора"
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="radius"></param>
        /// <param name="position"></param>
        /// <param name="remoteness"></param>
        public void DrawPythagorasTree(double x, double y, double z, double radius = 0.317, double position = 238, double remoteness = 1.5)
        {
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Light0); //освещение объектов
            GL.PushMatrix();
            if (!viewFromAbove)
                GL.Rotate(position, 1.0, 1.0, 1.0);
            PythagorasTree(x, y, z, radius,  22);

            GL.PopMatrix();
            GL.Disable(EnableCap.Light0);
            GL.Disable(EnableCap.Texture2D);
        }
        /// <summary>
        /// Рекурсивная функция фрактала
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="l"></param>
        /// <param name="a"></param>
        public void PythagorasTree(double x, double y, double z, double l, double a)
        {
            if (l > 0.001)
            {
                Rect(x, y, z, l, a);

                PythagorasTree(
                    x - l * Math.Sin(a),
                    y - l * Math.Cos(a),
                    z, l / Math.Sqrt(2),
                    a + Math.PI / 4
                );

                PythagorasTree(
                    x - l * Math.Sin(a) + l / Math.Sqrt(2) * Math.Cos(a + Math.PI / 4),
                    y - l * Math.Cos(a) - l / Math.Sqrt(2) * Math.Sin(a + Math.PI / 4),
                    z, l / Math.Sqrt(2),
                    a - Math.PI / 4
                );
            }
        }

        public void Rect(double x1, double y1, double z, double l, double a1)
        {
            if (playColorsFract) {
                Random r = new Random();
                GL.Color3((float)r.Next(0, 251) * 0.01, (float)r.Next(0, 251) * 0.01, (float)r.Next(0, 251) * 0.01);
            }
            // формирование изображения      
            double x2 = x1 + (l * Math.Cos(a1));
            double y2 = y1 - (l * Math.Sin(a1));

            double x3 = x1 + (l * Math.Sqrt(2) * Math.Cos(a1 + Math.PI / 4));
            double y3 = y1 - (l * Math.Sqrt(2) * Math.Sin(a1 + Math.PI / 4));

            double x4 = x1 + (l * Math.Cos(a1 + Math.PI / 2));
            double y4 = y1 - (l * Math.Sin(a1 + Math.PI / 2));

            GL.Begin(PrimitiveType.LineLoop);
            GL.Vertex3(x1, y1, z); GL.Vertex3(x2, y2, z);
            GL.Vertex3(x3, y3, z); GL.Vertex3(x4, y4, z);
            GL.End();
            GL.Color3(1f, 1f, 1f);
        }
        public void PlayColors()
        {
            Random r = new Random();
            GL.Color3((float)r.Next(0, 251) * 0.01, (float)r.Next(0, 251) * 0.01, (float)r.Next(0, 251) * 0.01);
        }


        // VBO - Vertex Buffer Object. Средство для работы с массивами, содержащими свойства вершин
        private uint VBOHandle;
        private static int MaxParticleCount = 1000;
        private int VisibleParticleCount;
        // Создаем VBO
        private VertexC4ubV3f[] VBO = new VertexC4ubV3f[MaxParticleCount];
        // Структура свойств частиц (Direction и Age)
        ParticleAttribut[] ParticleAttributes = new ParticleAttribut[MaxParticleCount];
        private int SpeedParticles = 15;

        // Структура, употребляемая при выводе вершин
        struct VertexC4ubV3f
        {
            public byte R, G, B, A;
            public Vector3 Position;
            public static int SizeInBytes = 16;
        }
        // Структура, употребляемая для обновления сцены
        struct ParticleAttribut
        {
            public Vector3 Direction;
            public uint Age;
            // Далее можно задать Rotation, Radius и прочие свойства частиц
        }


        public void OnLoadExplosion()
        {
            GL.PointSize(25f); // Размер частиц
            GL.Enable(EnableCap.PointSmooth); // Точка округлой формы
            GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest); // Наилучшее сглаживание
            // Статус VBO. Работаем с массивами цвета и вершин
            GL.EnableClientState(ArrayCap.ColorArray);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.GenBuffers(1, out VBOHandle); // Номер обработчика VBO
            // Можно выполнить привязку здесь, поскольку имеется только 1 VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOHandle);
            GL.ColorPointer(4, ColorPointerType.UnsignedByte, VertexC4ubV3f.SizeInBytes, (IntPtr)0);
            GL.VertexPointer(3, VertexPointerType.Float, VertexC4ubV3f.SizeInBytes, (IntPtr)(4 * sizeof(byte)));
            Random rnd = new Random();
            Vector3 temp = Vector3.Zero;
            // Генерация частиц
            for (uint i = 0; i < MaxParticleCount; i++)
            {
                // Цвет частицы i
                VBO[i].R = (byte)rnd.Next(100, 256);
                VBO[i].G = (byte)rnd.Next(50, 240);
                if (VBO[i].G > VBO[i].R) VBO[i].G = (byte)20;
                VBO[i].B = (byte)rnd.Next(0, 20);
                VBO[i].A = (byte)rnd.Next(0, 256); // Не используется
                // Координаты частицы i
                VBO[i].Position = Vector3.Zero; // Все частицы появляются в начале координат
                // Генерация вектора направления в диапазоне [-0.25f...+0.25f]
                temp.X = (float)((rnd.NextDouble() - 0.5) * 0.02f);
                temp.Y = (float)((rnd.NextDouble() - 0.5) * 0.02f);
                temp.Z = (float)((rnd.NextDouble() - 0.5) * 0.02f);
                ParticleAttributes[i].Direction = temp; // Вектор направления
                ParticleAttributes[i].Age = 0; // Начальный возраст частицы
            }
            // Число родившихся частиц
            VisibleParticleCount = 0;
        }
        public void RenderExplosion()
        {
            GL.PushMatrix();
            // Перемещаем частицы по оси Z
            GL.Translate(0f, 0f, 0f);
            // Сообщаем OpenGL, что можно пренебречь прежним VBO и предоставить память для нового VBO-буфера
            // Без этого GL будет ждать завершения вывода старого VBO
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(VertexC4ubV3f.SizeInBytes * MaxParticleCount), IntPtr.Zero, BufferUsageHint.StreamDraw);
            // Заполняем вновь выделенный буфер
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(VertexC4ubV3f.SizeInBytes * MaxParticleCount), VBO, BufferUsageHint.StreamDraw);
            // Выводим только уже родившиеся частицы
            GL.DrawArrays(PrimitiveType.Points, MaxParticleCount - VisibleParticleCount, VisibleParticleCount);

            GL.PopMatrix();
        }

        public void UpdateFrameExplosion()
        {
            // Обновляем частицы. При использовании Physics SDK частота обновления существенно выше числа кадров в секунду
            // Поэтому будут лишние циклы копирования в VBO
            if (VisibleParticleCount < MaxParticleCount) VisibleParticleCount++;
            Vector3 temp;
            for (int i = MaxParticleCount - VisibleParticleCount; i < MaxParticleCount; i++)
            {
                if (ParticleAttributes[i].Age >= MaxParticleCount-300)
                {
                    // Приводим непоявившиеся частицы в начальное состояние
                    ParticleAttributes[i].Age = 40;
                    VBO[i].Position = Vector3.Zero;
                    //return 0;
                }
                else
                {
                    // Эти частицы уже в сцене. Увеличиваем возраст частицы
                    ParticleAttributes[i].Age += (uint)Math.Max(ParticleAttributes[i].Direction.LengthFast * 30, 10);
                    // Получаем вектор temp, умножая вектор направления на время события
                    Vector3.Multiply(ref ParticleAttributes[i].Direction, (float)SpeedParticles, out temp);
                    // Изменяем координаты частицы, складывая прежние координаты с вектором temp
                    Vector3.Add(ref VBO[i].Position, ref temp, out VBO[i].Position);
                }
            }
            //return 0;
        }

        public void SmollSizeSun()
        {
            sizeSun = 1;
        }
        public void BigSizeSun()
        {
            sizeSun = 3.5;
        }
    }
}
