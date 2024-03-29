﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using kMaxMin.Model;

namespace kMeans.ViewModel
{
    public class WindowViewModel: BaseViewModel
    {

        public WindowViewModel()
        {
            IsEnableToPress = true;
            ExecuteTask = new Command(async () =>
            {
                if (!int.TryParse(PointsCount, out pointsCountNum))
                { 
                    return;
                }
                IsEnableToPress = false;
                IsWorking = true;
                ClassModel result = null;
                var model = new PointsModel(pointsCountNum);
                await Task.Run(() =>
                {
                    result = model.GeneratePoints();
                });
                //ImageSource = await Task.FromResult(Draw(result.Item1));
                Tuple<List<ClassModel>, bool> redrawResult = null;
                List<Points> allPointses = new List<Points>(result.ClassPoints);
                var classList = new List<ClassModel>();
                classList.Add(result);
                while (true)
                {
                    await Task.Run(() =>
                    {
                        redrawResult = model.GenerateClasses(classList, allPointses);
                    });
                    
                    if (!redrawResult.Item2)
                    {
                        break;
                    } 
                }
                ImageSource = await Task.FromResult(Draw(redrawResult.Item1));
                IsWorking = false;
                IsEnableToPress = true;
            });
        }



        private string classCount;

        public string ClassCount
        {
            get { return classCount; }
            set
            {
                classCount = value;
                OnPropertyChanged();
            }
        }


        private string pointsCount;

        public string PointsCount
        {
            get
            {
                return pointsCount;
            }
            set
            {
                pointsCount = value;
                OnPropertyChanged();
            }
        }


        private string errorString;

        public string ErrorString
        {
            get
            {
                return errorString;
            }
            set
            {
                errorString = value;
                OnPropertyChanged();
            }
        }

        private bool isAnError;

        public bool IsAnError
        {
            get
            {
                return isAnError;
            }
            set
            {
                isAnError = value;
                OnPropertyChanged();
            }
        }

        private ImageSource image;
        public ImageSource ImageSource
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
                OnPropertyChanged();
            }
        }


        public ICommand ExecuteTask { get; private set; }


        private int classCountNum;
        private int pointsCountNum;

        private bool isWorking;

        public bool IsWorking
        {
            get
            {
                return isWorking;
                
            }
            set
            {
                isWorking = value;
                OnPropertyChanged();
            }
        }

        private bool isEnableToPress;

        public bool IsEnableToPress
        {
            get
            {
                return isEnableToPress; 
            }
            set
            {
                isEnableToPress = value;
                OnPropertyChanged();
            }
        }

        private bool ConvertCheckingWrite()
        {
            if ((CheckWriteValue(ClassCount, out classCountNum, "Error input at class count field")) &&
                CheckWriteValue(PointsCount, out pointsCountNum, "Error input at points count field"))
            {
                 return true;
            }
            return false;
        }


        private bool CheckWriteValue(string fieldValue,out int fieldNumValue, string errorMessage)
        {
            if (!int.TryParse(fieldValue, out fieldNumValue))
            {
                ErrorString = errorMessage;
                IsAnError = true;
                return false;
            }
            return true;
        }


        private ImageSource Draw(List<ClassModel> list)
        {
            RenderTargetBitmap bitmap = new RenderTargetBitmap(770, 570, 90, 90, PixelFormats.Default);
            DrawingVisual drawing = new DrawingVisual();
            DrawingContext drawingContext = drawing.RenderOpen();
            foreach (var currentClass in list)
            {
                var brush = new SolidColorBrush(currentClass.ClassColor);
                var pen = new Pen(brush, 3);
                pen.Freeze();
                brush.Freeze();
                DrawPoints(currentClass.ClassPoints, pen, brush, drawingContext, currentClass.CentralPoints);
            }
            drawingContext.Close();
            bitmap.Clear();
            bitmap.Render(drawing);
            return bitmap;
        }


        private void DrawPoints(List<Points> points, Pen pen, Brush brush, DrawingContext context, Points center)
        {
            foreach (var point in points)
            {
                context.DrawEllipse(brush, pen, new Point(point.XPoint, point.YPoint), 3, 3 );
            }
            context.DrawEllipse(new SolidColorBrush(), new Pen(new SolidColorBrush(Colors.Black) ,7), new Point(center.XPoint, center.YPoint), 7, 7);
        }
    }
}