using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace PRI_116_KP_Space_Martynov_v0._0._1
{
    public partial class Form1 : Form
    {
        private double lastMouseX, lastAngleZ, lastMouseY, lastPosX, lastPosY;

        private static bool pressed = false, pause = false;
        private static bool cometHalleyRun = false, realSize = false, planetView = false;
        private static bool orbits = true, viewFromAbove = false, ruleSun = false, rulePlanets = false;
        private static bool rulePythagorasTree = false, ViewPythagorasTree = false;
        private static bool viewMercury = true, viewVenus = true, viewEarth = true, viewMars = true;
        private static bool viewJupiter = true, viewSaturn = true, viewUran = true, viewNeptun = true;
        private static double sizeSun = 3.5;
        private double AngleZ = -5.6; //в минус влево
        /// <summary>
        ////Иницилизация объекта солнечной системы
        /// </summary>
        SolarSystem solS = new SolarSystem(orbits, viewFromAbove, sizeSun);

        Timer timer = new Timer(), tmComet = new Timer(), tmEplos = new Timer(),
            tmMercury = new Timer(),  tmVenus = new Timer(), tmEarth = new Timer(), tmMars = new Timer(),
            tmJupiter = new Timer(), tmSaturn = new Timer(), tmUran = new Timer(), tmNeptun = new Timer();
        /// <summary>
        /// Скорости космоса и планет
        /// </summary>
        private double speedSpace = 0;
        private double speedSpaceStart = 0.03;
        private double totalSpeed = 0;
        private double totalSpeedStart = 0.03;
        private double rotateSpeed = 0;
        private double rotateSpeedStart = 0.03;
        const double Step = 0.2;
        const double AngleDl = 5;
        /// <summary>
        /// Координаты камеры
        /// </summary>
        SolarSystem.CoordsPositions PosCam;
        /// <summary>
        /// Координаты солнца
        /// </summary>
        SolarSystem.CoordsPositions PosSun;
        /// <summary>
        /// Координаты общей удаленности планет
        /// </summary>
        SolarSystem.CoordsPositions PosPln;
        /// <summary>
        /// Координаты управления Фракталом
        /// </summary>
        SolarSystem.CoordsPositions PosFract;

        //Stopwatch tmComet = new Stopwatch();
        //Stopwatch tmEplos = new Stopwatch();

        public Form1()
        {
            InitializeComponent();
            solS.Pictures[0] = new Bitmap("images/textures/space_2700x4320.jpg");
            solS.Pictures[1] = new Bitmap("images/textures/8k_sun.jpg");
            solS.Pictures[2] = new Bitmap("images/textures/2k_mercury.jpg");
            solS.Pictures[3] = new Bitmap("images/textures/2k_venus.jpg");
            solS.Pictures[4] = new Bitmap("images/textures/2k_earth.png");
            solS.Pictures[5] = new Bitmap("images/textures/2k_mars.jpg");
            solS.Pictures[6] = new Bitmap("images/textures/2k_jupiter.jpg");
            solS.Pictures[7] = new Bitmap("images/textures/2k_saturn.jpg");
            solS.Pictures[8] = new Bitmap("images/textures/2k_uran.png");
            solS.Pictures[9] = new Bitmap("images/textures/2k_neptune.jpg");
            solS.Pictures[10] = new Bitmap("images/textures/2k_mercury.jpg");
        }
        
        private void AnT_Paint(object sender, PaintEventArgs e)
        {
            // очистка буферов цвета и глубины
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.LoadIdentity();
            GL.Rotate(AngleZ, 0.0, 1.0, 0.0); // поворот изображения
            GL.Translate(PosCam.X, PosCam.Y, PosCam.Z); //переащаем на нужную позицию
            //рисуем сферу космоса
            solS.DrawSpace(solS.Pictures[0], speedSpace);
            //рисуем солнце
            if (chExplosion.Checked) 
                solS.RenderExplosion();
            solS.DrawSun(solS.Pictures[1], PosSun.X, PosSun.Y, PosSun.Z, speedSpace);
            //рисуем планеты
            if(viewMercury)//                        текстура,     скорость,         скоростьВращения    удаленность,       радиус
                solS.position[3] = solS.DrawPlanet(solS.Pictures[2], totalSpeed / 2,    rotateSpeed / 2,    5 + PosPln.X,  realSize ? 0.0114 : 0.114);
            if(viewVenus)
                solS.position[4] = solS.DrawPlanet(solS.Pictures[3], totalSpeed / 3.5,  rotateSpeed / 3.5,  6.5 + PosPln.X,    realSize ? 0.0295 : 0.295);
            if(viewEarth)
                solS.position[5] = solS.DrawPlanet(solS.Pictures[4], totalSpeed / 5,    rotateSpeed / 5,    8 + PosPln.X,  realSize ? 0.03 : 0.3);
            if(viewMars)
                solS.position[6] = solS.DrawPlanet(solS.Pictures[5], totalSpeed / 6.5,  rotateSpeed / 6.5,  9.1 + PosPln.X, realSize ? 0.0174 : 0.174);
            if(viewJupiter)
               solS.position[7] = solS.DrawPlanet(solS.Pictures[6], totalSpeed / 9,    rotateSpeed / 9,    12.1 + PosPln.X, realSize ? 0.1 : 1);
            if(viewSaturn)
               solS.position[8] = solS.DrawPlanet(solS.Pictures[7], totalSpeed / 12,   rotateSpeed / 12,   14.5 + PosPln.X, realSize ? 0.08 : 0.8);
            if(viewUran)
               solS.position[9] = solS.DrawPlanet(solS.Pictures[8], totalSpeed / 15.5, rotateSpeed / 15.5, 16.9 + PosPln.X, realSize ? 0.07 : 0.7);
            if(viewNeptun)
               solS.position[10] = solS.DrawPlanet(solS.Pictures[9], totalSpeed / 19,  rotateSpeed / 19,   19 + PosPln.X, realSize ? 0.05 : 0.5);
            if(cometHalleyRun) 
               solS.DrawComet(solS.Pictures[10], totalSpeed * 2, 21, 0.094);

            if (ViewPythagorasTree)
                solS.DrawPythagorasTree(PosFract.X, PosFract.Y, PosFract.Z);
            if (chColorsGlob.Checked)
                solS.PlayColors();

            //крупный план планет 
            if (planetView)
            {
                GL.Rotate(0, 1.0, 1.0, 1.0); // поворот изображения
                switch (cmbBoxPlanet.SelectedIndex)
                {
                    case 1: { PosCam = solS.position[3]; PosCam.Z = -0.7; break; }
                    case 2: { PosCam = solS.position[4]; PosCam.Z = -1; break; }
                    case 3: { PosCam = solS.position[5]; PosCam.Z = -1; break; }
                    case 4: { PosCam = solS.position[6]; PosCam.Z = -0.8; break; }
                    case 5: { PosCam = solS.position[7]; PosCam.Z = -2.2; break; }
                    case 6: { PosCam = solS.position[8]; PosCam.Z = -1.8; break; }
                    case 7: { PosCam = solS.position[9]; PosCam.Z = -1.8; break; }
                    case 8: { PosCam = solS.position[10]; PosCam.Z = -1.7; break; }
                }
            }

            GL.Flush();
            GL.Finish();
            AnT.SwapBuffers();
        }

        private void AnT_Load(object sender, EventArgs e)
        {
            GL.Enable(EnableCap.DepthTest);
            float[] light2_diffuse = { 0.9f, 0.9f, 1.0f }; //цвет появления
            float[] light2_position = { 0.0f, 5.0f, 30.0f, 0.0f };
            GL.Enable(EnableCap.Light2);
            GL.Light(LightName.Light2, LightParameter.Diffuse, light2_diffuse);
            GL.Light(LightName.Light2, LightParameter.Position, light2_position);
            GL.Light(LightName.Light2, LightParameter.ConstantAttenuation, 0.01f);
            GL.Light(LightName.Light2, LightParameter.LinearAttenuation, 0.000009f);
            GL.Light(LightName.Light2, LightParameter.QuadraticAttenuation, 0.00009f);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            //Задаем начальные позиции
            PosCam = solS.position[1]; //камера
            PosPln = solS.position[11]; //Общая удаленность планет
            PosFract = solS.position[12]; //Фрактал
            cmbBoxArea.SelectedIndex = 1; //позиция планеты-гиганты
            cmbBoxPlanet.SelectedIndex = 0; //планеты - невыбрано
            cmbRule.SelectedIndex = 0; //управление объектом - камера
            timer.Interval = 20;
            timer.Tick += movementObjects;
            timer.Enabled = true;
        }

        private void movementObjects(object sender, EventArgs e)
        {
            speedSpace += speedSpaceStart / 150; //Скорость вращения сферы космоса и солнца
            totalSpeed += totalSpeedStart;  //Общая скорость вращения планет
            rotateSpeed += rotateSpeedStart; //Общая скорость вращения планет вокруг своей оси
            if(chExplosion.Checked) solS.UpdateFrameExplosion();
            AnT.Invalidate();
        }

        private void AnT_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, AnT.Width, AnT.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity(); //иннициализация
            GL.Frustum(-0.5, 0.5, -0.5, 0.5, 0.5, 65);
            GL.MatrixMode(MatrixMode.Modelview);
            Matrix4 p = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Width / (float)Height, 0.1f, 50.0f);
            GL.LoadMatrix(ref p);
            GL.MatrixMode(MatrixMode.Modelview);
            Matrix4 mv = Matrix4.LookAt(Vector3.UnitZ, Vector3.Zero, Vector3.UnitY);
            GL.LoadMatrix(ref mv);
            AnT.Invalidate();
        }

        private void AnT_KeyDown(object sender, KeyEventArgs e)
        {
            if (!ruleSun && !rulePlanets && !rulePythagorasTree)
            {
                switch (e.KeyCode)
                {
                    case Keys.Q: AngleZ -= AngleDl; break;
                    case Keys.E: AngleZ += AngleDl; break;
                    case Keys.W:
                        PosCam.X -= Step * Math.Sin(AngleZ * Math.PI / 180);
                        PosCam.Z += Step * Math.Cos(AngleZ * Math.PI / 180);
                        break;
                    case Keys.S:
                        PosCam.X += Step * Math.Sin(AngleZ * Math.PI / 180);
                        PosCam.Z -= Step * Math.Cos(AngleZ * Math.PI / 180);
                        break;
                    case Keys.R: PosCam.Y -= Step; break;
                    case Keys.F: PosCam.Y += Step; break;
                    case Keys.A: PosCam.X += Step * Math.Cos(AngleZ * Math.PI / 180); break;
                    case Keys.D: PosCam.X -= Step * Math.Cos(AngleZ * Math.PI / 180); break;
                    case Keys.Space:
                        pause = !pause;
                        totalSpeedStart = !pause ? speedMovement.Value * 0.001 : 0;
                        pauseMotion.Image = Bitmap.FromFile(!pause ? "images/pause 64.png" : "images/play 64.png");
                        break;
                }

                lblX.Text = PosCam.X.ToString();
                lblY.Text = PosCam.Y.ToString();
                lblZ.Text = PosCam.Z.ToString();
                lblAnglZ.Text = AngleZ.ToString();
            }
            else if (ruleSun)
            {
                switch (e.KeyCode)
                {
                    case Keys.W:
                        PosSun.X += Step * Math.Sin(AngleZ * Math.PI / 180);
                        PosSun.Z -= Step * Math.Cos(AngleZ * Math.PI / 180);
                        break;
                    case Keys.S:
                        PosSun.X -= Step * Math.Sin(AngleZ * Math.PI / 180);
                        PosSun.Z += Step * Math.Cos(AngleZ * Math.PI / 180);
                        break;
                    case Keys.R: PosSun.Y += Step; break;
                    case Keys.F: PosSun.Y -= Step; break;
                    case Keys.A: PosSun.X -= Step * Math.Cos(AngleZ * Math.PI / 180); break;
                    case Keys.D: PosSun.X += Step * Math.Cos(AngleZ * Math.PI / 180); break;
                }
            }
            else if (rulePlanets)
            {
                switch (e.KeyCode)
                {
                    case Keys.A: PosPln.X -= Step * Math.Cos(AngleZ * Math.PI / 180); break;
                    case Keys.D: PosPln.X += Step * Math.Cos(AngleZ * Math.PI / 180); break;
                }
            }
            else if (rulePythagorasTree)
            {
                switch (e.KeyCode)
                {
                    case Keys.R: PosFract.Z += Step * Math.Cos(AngleZ * Math.PI / 180); break;
                    case Keys.F: PosFract.Z -= Step * Math.Cos(AngleZ * Math.PI / 180); break;
                    case Keys.W: PosFract.Y += Step; break;
                    case Keys.S: PosFract.Y -= Step; break;
                    case Keys.A: PosFract.X -= Step * Math.Cos(AngleZ * Math.PI / 180); break;
                    case Keys.D: PosFract.X += Step * Math.Cos(AngleZ * Math.PI / 180); break;
                }
            }

            AnT.Invalidate(); //перерисовываем сцену
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            AnT_KeyDown(sender, e);
        }
        private void AnT_MouseMove(object sender, MouseEventArgs e)
        {
            if (pressed)
            {
                cmbBoxArea.SelectedIndex = 1;
                PosCam.Z = 0.5 * (lastAngleZ + (e.Y - lastMouseX) / 5);
                PosCam.X = Step * (lastPosX - (e.Y - lastMouseY) / 50 * Math.Sin(AngleZ * Math.PI / 180));
                PosCam.X = 0.7 * (lastPosY + (e.X - lastMouseY) / 50 * Math.Cos(AngleZ * Math.PI / 180));
                AnT.Invalidate();
            }
        }

        private void AnT_MouseDown(object sender, MouseEventArgs e)
        {
            pressed = true;
            lastMouseX = e.X;
            lastMouseY = e.Y;
            lastPosX = PosCam.X;
            lastPosY = PosCam.Y;
            lastAngleZ = PosCam.Z;
        }

        private void AnT_MouseUp(object sender, MouseEventArgs e)
        {
            pressed = false;
        }
        
        private void cmbBoxArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            AngleZ = -5.6;
            PosCam = cmbBoxArea.SelectedIndex == 0 ? solS.position[0] : solS.position[1];
            if(cmbBoxArea.SelectedIndex == 0) 
                viewJupiter = viewSaturn = viewUran = false;
            else
                viewJupiter = viewSaturn = viewUran = true;
        }

        private void chFractal_CheckedChanged(object sender, EventArgs e)
        {
            ViewPythagorasTree = chFractal.Checked;
            chViewAbove.Checked = ViewPythagorasTree;
            PosCam = ViewPythagorasTree ? solS.position[0] : solS.position[1];
            cmbBoxArea.SelectedIndex = ViewPythagorasTree ? 0 : 1;
            PosCam.Z = ViewPythagorasTree ? -12 : solS.position[1].Z;
        }

        private void chPlayColors_CheckedChanged(object sender, EventArgs e)
        {
            solS.playColorsFract = chPlayColors.Checked;
            if (!chPlayColors.Checked) GL.Color3(1f, 1f, 1f);
        }

        private void chColorsGlob_CheckedChanged(object sender, EventArgs e)
        {
            if (!chColorsGlob.Checked) GL.Color3(1f, 1f, 1f);
        }

        private void checkViewAbove_CheckedChanged(object sender, EventArgs e)
        {
            solS.viewFromAbove = chViewAbove.Checked;
        }

        private void cmbRule_SelectedIndexChanged(object sender, EventArgs e)
        {
            ruleSun = cmbRule.SelectedIndex == 1 ? true : false;
            rulePlanets = cmbRule.SelectedIndex == 2 ? true : false;
            rulePythagorasTree = cmbRule.SelectedIndex == 3 ? true : false;
        }

        private void cmbBoxPlanet_SelectedIndexChanged(object sender, EventArgs e)
        {
            orbitsCheck.Checked = cmbBoxPlanet.SelectedIndex != 0 ? false : true;
            planetView = cmbBoxPlanet.SelectedIndex != 0 ? true : false;
            chViewAbove.Checked = cmbBoxPlanet.SelectedIndex != 0 ? true : false;

            //остановка движения планет при просмотре
            //pause = cmbBoxPlanet.SelectedIndex != 0 ? true : false;
            //pauseMotion.Image = Bitmap.FromFile(pause ? "images/play 64.png" : "images/pause 64.png");
            //totalSpeedStart = pause ? 0 : speedMovement.Value * 0.001;

            if (cmbBoxPlanet.SelectedIndex == 0)
                PosCam = solS.position[1];
        }

        private void orbitsCheck_CheckedChanged(object sender, EventArgs e)
        {
            solS.orbits = orbitsCheck.Checked;
        }

        private void cometHalley_Click(object sender, EventArgs e)
        {
            tmComet.Interval = 20000; //Запуск таймера на 20 секунд
            tmComet.Tick += hideCometHalley;
            tmComet.Enabled = true;

            cometHalleyRun = !cometHalleyRun;
            chViewAbove.Checked = cometHalleyRun ? true : false;
            cometHalley.Image = Bitmap.FromFile(cometHalleyRun ? "images/asteroid-48.png" : "images/pastel-64.png");
            PosCam = cometHalleyRun ? solS.position[2] : solS.position[1];
            AngleZ = cometHalleyRun ? -5 : -5.6; //в минус влево
        }

        private void hideCometHalley(object sender, EventArgs e)
        {
            cometHalleyRun = false;
            chViewAbove.Checked = cometHalleyRun ? true : false;
            cometHalley.Image = Bitmap.FromFile(cometHalleyRun ? "images/asteroid-48.png" : "images/pastel-64.png");
            PosCam = cometHalleyRun ? solS.position[2] : solS.position[1];
            AngleZ = cometHalleyRun ? -5 : -5.6; //в минус влево
            tmComet.Enabled = false;
            tmComet = new Timer();
        }

        private void chExplosion_CheckedChanged(object sender, EventArgs e)
        {
            if (chExplosion.Checked) {
                solS.SmollSizeSun();
                solS.OnLoadExplosion();
                tmEplos.Interval = 20000; //Запуск таймера на 20 секунд
                tmEplos.Tick += hideExplosion;
                tmEplos.Enabled = true;
                tmMercury.Interval = 1500; //Запуск таймера на 1.5 секунду
                tmMercury.Tick += hideMercury;
                tmMercury.Enabled = true;
                tmVenus.Interval = 2500; //Запуск таймера на 2.5 секунды
                tmVenus.Tick += hideVenus;
                tmVenus.Enabled = true;
                tmEarth.Interval = 3500; //Запуск таймера на 3.5 секунды
                tmEarth.Tick += hideEarth;
                tmEarth.Enabled = true;
                tmMars.Interval = 3800; //Запуск таймера на 3.8 секунды
                tmMars.Tick += hideMars;
                tmMars.Enabled = true;
                tmJupiter.Interval = 4500; //Запуск таймера на 4.5 секунды
                tmJupiter.Tick += hideJupiter;
                tmJupiter.Enabled = true;
                tmSaturn.Interval = 5200; //Запуск таймера на 5.2 секунды
                tmSaturn.Tick += hideSaturn;
                tmSaturn.Enabled = true;
                tmUran.Interval = 5500; //Запуск таймера на 5.5 секунды
                tmUran.Tick += hideUran;
                tmUran.Enabled = true;
                tmNeptun.Interval = 6000; //Запуск таймера на 6 секунды
                tmNeptun.Tick += hideNeptun;
                tmNeptun.Enabled = true;
            }
            else hideExplosion(sender, e);
        }

        private void hideMercury(object sender, EventArgs e) {
            viewMercury = tmMercury.Enabled = false;
            tmMercury = new Timer();
        }
        private void hideVenus(object sender, EventArgs e){
            viewVenus = tmVenus.Enabled = false;
            tmVenus = new Timer();
        }
        private void hideEarth(object sender, EventArgs e) {
            viewEarth = tmEarth.Enabled = false;
            tmEarth = new Timer();
        }
        private void hideMars(object sender, EventArgs e){
            viewMars = tmMars.Enabled = false;
            tmMars = new Timer();
        }
        private void hideJupiter(object sender, EventArgs e){
            viewJupiter = tmJupiter.Enabled = false;
            tmJupiter = new Timer();
        }
        private void hideSaturn(object sender, EventArgs e){
            viewSaturn = tmSaturn.Enabled = false;
            tmSaturn = new Timer();
        }
        private void hideUran(object sender, EventArgs e){
            viewUran = tmUran.Enabled = false;
            tmUran = new Timer();
        }
        private void hideNeptun(object sender, EventArgs e){
            viewNeptun = tmNeptun.Enabled = false;
            tmNeptun = new Timer();
        }

        private void hideExplosion(object sender, EventArgs e)
        {
            chExplosion.Checked = false;
            tmEplos.Enabled = false;
            tmEplos = new Timer();
            solS.BigSizeSun();
            viewMercury = viewVenus = viewEarth = viewMars = 
            viewJupiter = viewSaturn = viewUran = viewNeptun = true;
        }

        private void realSizeCheck_CheckedChanged(object sender, EventArgs e)
        {
            realSize = !realSize;
            orbitsCheck.Checked = realSize ? false : true;
        }

        private void pauseMotion_Click(object sender, EventArgs e)
        {
            totalSpeedStart = pause ? speedMovement.Value * 0.001 : 0;
            pauseMotion.Image = Bitmap.FromFile(pause ? "images/pause 64.png" : "images/play 64.png");
            pause = !pause;
        }


        private void инструкцияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aboutProgram.Visible = false;
            instructionBox.Visible = true;
        }
        private void closeBtn_Click(object sender, EventArgs e)
        {
            instructionBox.Visible = false;
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            instructionBox.Visible = false;
            aboutProgram.Visible = true;
        }

        private void closeAboutPr_Click(object sender, EventArgs e)
        {
            aboutProgram.Visible = false;
        }
        private void speedMovement_Scroll(object sender, EventArgs e)
        {
            totalSpeedStart = speedMovement.Value * 0.01;
        }

        private void планетыЗемнойГруппыMenuItem_Click(object sender, EventArgs e)
        {
            PosCam = solS.position[0]; AngleZ = -5.6;
        }
        private void планетыгигантыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PosCam = solS.position[1]; AngleZ = -5.6;
        }

        private void меркуриийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cmbBoxPlanet.SelectedIndex = 1;
        }

        private void венераToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cmbBoxPlanet.SelectedIndex = 2;
        }

        private void земляToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cmbBoxPlanet.SelectedIndex = 3;
        }

        private void марсToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cmbBoxPlanet.SelectedIndex = 4;
        }

        private void юпитерToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cmbBoxPlanet.SelectedIndex = 5;
        }

        private void сатурнToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cmbBoxPlanet.SelectedIndex = 6;
        }

        private void уранToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cmbBoxPlanet.SelectedIndex = 7;
        }

        private void нептунToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cmbBoxPlanet.SelectedIndex = 8;
        }

        private void выйтиИзКрупногоПланаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cmbBoxPlanet.SelectedIndex = 0;
        }
    }
}
