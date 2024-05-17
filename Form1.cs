using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Net.Mime.MediaTypeNames;

namespace SCOI_lab_1
{
    public partial class Form1 : Form
    {
        private Bitmap initialImage; // Сохранение изначального изображения
        private Bitmap originalImage;
        private Bitmap NewImage;

        public Form1()
        {
            InitializeComponent();
            // Устанавливаем режим масштабирования изображения в PictureBox на Zoom
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            chart2.MouseClick += chart2_MouseClick;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Создаем новый экземпляр диалогового окна для выбора файла
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Устанавливаем фильтр для диалогового окна (только изображения)
            openFileDialog.Filter = "Image Files (*.bmp;*.jpg;*.jpeg;*.png;*.gif)|*.bmp;*.jpg;*.jpeg;*.png;*.gif|All files (*.*)|*.*";

            // Показываем диалоговое окно и проверяем результат выбора
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Создаем новое изображение из выбранного файла
                    originalImage = new Bitmap(openFileDialog.FileName);
                    initialImage = new Bitmap(originalImage); // Сохраняем копию загруженного изображения

                    // Загружаем выбранное изображение в PictureBox
                    pictureBox1.Image = originalImage;
                    NewImage = originalImage;
                }
                catch (Exception ex)
                {
                    // Выводим сообщение об ошибке, если не удалось загрузить изображение
                    MessageBox.Show("Ошибка загрузки изображения: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Увеличиваем яркость изображения на 50 единиц
            Bitmap transformedImage = IncreaseBrightness(NewImage, 50);

            // Отображаем преобразованное изображение в PictureBox
            pictureBox1.Image = transformedImage;
            NewImage = transformedImage;
        }


        private Bitmap ApplyTransformation(Bitmap originalImage)
        {
            // Создаем новый экземпляр изображения для обработки
            Bitmap transformedImage = new Bitmap(originalImage.Width, originalImage.Height);

            // Проходим по каждому пикселю изображения
            for (int y = 0; y < originalImage.Height; y++)
            {
                for (int x = 0; x < originalImage.Width; x++)
                {
                    // Получаем цвет пикселя
                    Color originalColor = originalImage.GetPixel(x, y);

                    // Выполняем преобразование цвета (например, изменение яркости)
                    // Например, увеличим значение яркости на 50 для каждого канала (RGB)
                    int newRed = Math.Min(255, originalColor.R + 50);
                    int newGreen = Math.Min(255, originalColor.G + 50);
                    int newBlue = Math.Min(255, originalColor.B + 50);

                    // Создаем новый цвет с измененными значениями каналов
                    Color transformedColor = Color.FromArgb(newRed, newGreen, newBlue);

                    // Устанавливаем новый цвет пикселя на изображении
                    transformedImage.SetPixel(x, y, transformedColor);
                }
            }

            // Возвращаем обработанное изображение
            return transformedImage;
        }


        private Bitmap IncreaseBrightness(Bitmap NewImage, int intensity)
        {
            // Создаем новый экземпляр изображения для обработки
            Bitmap transformedImage = new Bitmap(NewImage.Width, NewImage.Height);

            // Проходим по каждому пикселю изображения
            for (int y = 0; y < NewImage.Height; y++)
            {
                for (int x = 0; x < NewImage.Width; x++)
                {
                    // Получаем цвет пикселя
                    Color originalColor = NewImage.GetPixel(x, y);

                    // Увеличиваем интенсивность каждого канала (RGB) на указанное значение
                    int newRed = Math.Min(255, originalColor.R + intensity);
                    int newGreen = Math.Min(255, originalColor.G + intensity);
                    int newBlue = Math.Min(255, originalColor.B + intensity);

                    // Создаем новый цвет с измененными значениями каналов
                    Color transformedColor = Color.FromArgb(newRed, newGreen, newBlue);

                    // Устанавливаем новый цвет пикселя на изображении
                    transformedImage.SetPixel(x, y, transformedColor);
                }
            }

            // Возвращаем обработанное изображение
            return transformedImage;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Bitmap resultImage = new Bitmap(NewImage.Width, NewImage.Height);

            // Проходим по каждому пикселю изображения
            for (int y = 0; y < NewImage.Height; y++)
            {
                for (int x = 0; x < NewImage.Width; x++)
                {
                    // Получаем цвет пикселя
                    Color originalColor = NewImage.GetPixel(x, y);

                    // Увеличиваем контрастность путем умножения разницы между значением канала и средним значением на коэффициент
                    int averageIntensity = (originalColor.R + originalColor.G + originalColor.B) / 3;
                    int newRed = Clamp(averageIntensity + (originalColor.R - averageIntensity) * 2, 0, 255);
                    int newGreen = Clamp(averageIntensity + (originalColor.G - averageIntensity) * 2, 0, 255);
                    int newBlue = Clamp(averageIntensity + (originalColor.B - averageIntensity) * 2, 0, 255);

                    // Создаем новый цвет с измененными значениями каналов
                    Color newColor = Color.FromArgb(newRed, newGreen, newBlue);

                    // Устанавливаем новый цвет пикселя на изображении
                    resultImage.SetPixel(x, y, newColor);
                }
            }
            pictureBox1.Image = resultImage;
            NewImage = resultImage;
        }



        private int Clamp(int value, int min, int max)
        {
            return Math.Max(min, Math.Min(value, max));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            NewImage = new Bitmap(initialImage); // Восстанавливаем изначальное изображение из initialImage
            pictureBox1.Image = NewImage;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Увеличиваем насыщенность изображения на 0.5 
            Bitmap saturatedImage = IncreaseSaturation(NewImage, 0.5f);

            pictureBox1.Image = saturatedImage;
            NewImage = saturatedImage;
        }


        private Bitmap IncreaseSaturation(Bitmap NewImage, float saturationFactor)
        {
            // Создаем копию изображения
            Bitmap saturatedImage = new Bitmap(NewImage);

            // Проходим по каждому пикселю изображения
            for (int y = 0; y < NewImage.Height; y++)
            {
                for (int x = 0; x < NewImage.Width; x++)
                {
                    // Получаем цвет пикселя
                    Color color = NewImage.GetPixel(x, y);

                    // Преобразуем цвет в HSL
                    float h, s, l;
                    ColorToHSL(color, out h, out s, out l);

                    // Увеличиваем насыщенность
                    s *= saturationFactor;
                    s = Math.Min(1.0f, Math.Max(0.0f, s)); // Ограничиваем значение насыщенности от 0 до 1

                    // Преобразуем обратно в RGB
                    Color saturatedColor = HSLToColor(h, s, l);

                    // Устанавливаем новый цвет пикселя на изображении
                    saturatedImage.SetPixel(x, y, saturatedColor);
                }
            }

            return saturatedImage;
        }

        private void ColorToHSL(Color color, out float h, out float s, out float l)
        {
            float r = color.R / 255.0f;
            float g = color.G / 255.0f;
            float b = color.B / 255.0f;

            float max = Math.Max(r, Math.Max(g, b));
            float min = Math.Min(r, Math.Min(g, b));

            l = (max + min) / 2;

            if (max == min)
            {
                h = s = 0; // achromatic
            }
            else
            {
                float d = max - min;
                s = l > 0.5 ? d / (2 - max - min) : d / (max + min);
                switch (max)
                {
                    case float n when Math.Abs(n - r) < 0.0001:
                        h = (g - b) / d + (g < b ? 6 : 0);
                        break;
                    case float n when Math.Abs(n - g) < 0.0001:
                        h = (b - r) / d + 2;
                        break;
                    default:
                        h = (r - g) / d + 4;
                        break;
                }
                h /= 6;
            }
        }

        private Color HSLToColor(float h, float s, float l)
        {
            float r, g, b;

            if (s == 0)
            {
                r = g = b = l; // achromatic
            }
            else
            {
                float q = l < 0.5 ? l * (1 + s) : l + s - l * s;
                float p = 2 * l - q;
                r = HueToRGB(p, q, h + 1.0f / 3);
                g = HueToRGB(p, q, h);
                b = HueToRGB(p, q, h - 1.0f / 3);
            }

            return Color.FromArgb((int)(r * 255), (int)(g * 255), (int)(b * 255));
        }

        private float HueToRGB(float p, float q, float t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1.0f / 6) return p + (q - p) * 6 * t;
            if (t < 1.0f / 2) return q;
            if (t < 2.0f / 3) return p + (q - p) * (2.0f / 3 - t) * 6;
            return p;
        }


        private void chart1_Click(object sender, EventArgs e)
        {
            // Получаем гистограмму изображения
            int[] histogram = CalculateHistogram(NewImage);

            // Очищаем график перед отображением новой гистограммы
            chart1.Series[0].Points.Clear();

            // Добавляем точки на график для каждого значения яркости
            for (int i = 0; i < 256; i++)
            {
                chart1.Series[0].Points.AddXY(i, histogram[i]);
            }
        }

        private int[] CalculateHistogram(Bitmap image)
        {
            // Создаем массив для хранения гистограммы (256 значений)
            int[] histogram = new int[256];

            // Проходим по каждому пикселю изображения
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    // Получаем цвет пикселя
                    Color pixelColor = image.GetPixel(x, y);

                    // Вычисляем значение яркости пикселя
                    int brightness = (int)(0.299 * pixelColor.R + 0.587 * pixelColor.G + 0.114 * pixelColor.B);

                    // Увеличиваем соответствующий элемент гистограммы
                    histogram[brightness]++;
                }
            }

            return histogram;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // Отображаем гистограмму текущего изображения
            DisplayHistogram(NewImage);
        }

        private void DisplayHistogram(Bitmap image)
        {
            // Получаем гистограмму изображения
            int[] histogram = CalculateHistogram(image);

            // Очищаем график перед отображением новой гистограммы
            chart1.Series[0].Points.Clear();

            // Добавляем точки на график для каждого значения яркости
            for (int i = 0; i < 256; i++)
            {
                chart1.Series[0].Points.AddXY(i, histogram[i]);
            }
        }

        private void chart2_MouseClick(object sender, MouseEventArgs e)
        {
            // Получаем координаты клика относительно экрана
            Point screenCoordinates = chart2.PointToScreen(new Point(e.X, e.Y));

            // Конвертируем координаты в клиентские координаты графика
            Point clientCoordinates = chart2.PointToClient(screenCoordinates);

            // Получаем координаты клика
            double xValue = chart2.ChartAreas[0].AxisX.PixelPositionToValue(clientCoordinates.X);
            double yValue = chart2.ChartAreas[0].AxisY.PixelPositionToValue(clientCoordinates.Y);

            // Добавляем точку на график
            chart2.Series[0].Points.AddXY(xValue, yValue);
        }


       



        private void button7_Click(object sender, EventArgs e)
        {
            // Получаем координаты точек с графика
            List<double> xValues = chart2.Series[0].Points.Select(point => point.XValue).ToList();
            List<double> yValues = chart2.Series[0].Points.Select(point => point.YValues[0]).ToList();

            // Выполняем линейную интерполяцию между точками
            List<double> interpolatedXValues = Enumerable.Range(0, 256).Select(i => (double)i).ToList();
            List<double> interpolatedYValues = LinearInterpolation(xValues, yValues, interpolatedXValues);

            // Применяем интерполированные значения к изображению
            ApplyInterpolationToImage(interpolatedXValues, interpolatedYValues);
        }

        private List<double> LinearInterpolation(List<double> xValues, List<double> yValues, List<double> interpolatedXValues)
        {
            List<double> interpolatedYValues = new List<double>();

            for (int i = 0; i < interpolatedXValues.Count; i++)
            {
                double x = interpolatedXValues[i];

                // Находим ближайшие точки для интерполяции
                int index = xValues.BinarySearch(x);
                if (index < 0)
                {
                    index = ~index;
                    if (index == 0) index = 1;
                    else if (index == xValues.Count) index = xValues.Count - 1;
                }

                // Выполняем линейную интерполяцию между ближайшими точками
                double x1 = xValues[index - 1];
                double x2 = xValues[index];
                double y1 = yValues[index - 1];
                double y2 = yValues[index];

                double interpolatedY = y1 + (x - x1) * (y2 - y1) / (x2 - x1);
                interpolatedYValues.Add(interpolatedY);
            }

            return interpolatedYValues;
        }

        private void ApplyInterpolationToImage(List<double> xValues, List<double> yValues)
        {
            // Применяем интерполированные значения к изображению
            for (int y = 0; y < NewImage.Height; y++)
            {
                for (int x = 0; x < NewImage.Width; x++)
                {
                    // Получаем цвет пикселя
                    Color originalColor = NewImage.GetPixel(x, y);

                    // Вычисляем значение яркости пикселя
                    int brightness = (int)(0.299 * originalColor.R + 0.587 * originalColor.G + 0.114 * originalColor.B);

                    // Находим соответствующее значение яркости в интерполированных данных
                    double interpolatedBrightness = yValues[brightness];

                    // Ограничиваем значения яркости от 0 до 255
                    int newBrightness = Clamp((int)interpolatedBrightness, 0, 255);

                    // Создаем новый цвет с измененным значением яркости
                    Color newColor = Color.FromArgb(newBrightness, newBrightness, newBrightness);

                    // Устанавливаем новый цвет пикселя на изображении
                    NewImage.SetPixel(x, y, newColor);
                }
            }

            // Отображаем преобразованное изображение в PictureBox
            pictureBox1.Image = NewImage;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.Text)
            {
                case "Глобальный метод Гаврилова":
                    {
                        label1.Visible = false;
                        label2.Visible = false;
                        textBox1.Visible = false;
                        textBox2.Visible = false;
                        break;
                    }
                case "Глобальный метод Отсу":
                    {
                        label1.Visible = false;
                        label2.Visible = false;
                        textBox1.Visible = false;
                        textBox2.Visible = false;
                        break;
                    }
                case "Локальный метод Ниблека":
                    {
                        label1.Visible = true;
                        label2.Visible = true;
                        textBox1.Visible = true;
                        textBox1.Text = "-0,2";
                        textBox2.Visible = true;
                        textBox2.Text = "20";
                        break;
                    }
                case "Локальный метод Сауволы":
                    {
                        label1.Visible = true;
                        label2.Visible = true;
                        textBox1.Visible = true;
                        textBox1.Text = "0,2";
                        textBox2.Visible = true;
                        textBox2.Text = "20";
                        break;
                    }
                case "Локальный метод Вульфа":
                    {
                        label1.Visible = true;
                        label2.Visible = false;
                        textBox1.Visible = false;
                        textBox2.Visible = true;
                        textBox2.Text = "20";
                        break;
                    }
                case "Локальный метод Брэдли-Рота":
                    {
                        label1.Visible = true;
                        label2.Visible = true;
                        textBox1.Visible = true;
                        textBox2.Visible = true;
                        textBox1.Text = "0,15";
                        textBox2.Text = "20";
                        break;
                    }


            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {

        }
    }
}