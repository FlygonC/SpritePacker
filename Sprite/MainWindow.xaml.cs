using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using System.Windows.Shapes;
using Microsoft.Win32;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Sprite
{

    public class SpriteContainer : INotifyPropertyChanged
    {
        public BitmapImage imgsrc;
        private string _name;
        public string name
        {
            get { return _name; }
            set
            {
                if (this._name != value)
                {
                    _name = value;
                    notifyPropertyChanged("name");
                }
            }
        }
        public int width;
        public int height;
        public int locX = 0;
        public int locY = 0;
        
        SpriteContainer()
        {
            Console.WriteLine("butts");
        }
        public SpriteContainer(Uri a_uri)
        {
            imgsrc = new BitmapImage(a_uri);


            name = Path.GetFileNameWithoutExtension(a_uri.AbsolutePath);

            width = imgsrc.PixelWidth;
            height = imgsrc.PixelHeight;
        }

        public override string ToString()
        {
            return this.name;
        }

        public XElement ToXML( )
        {
            XElement element = new XElement( "Sprite" );
            element.Add(new XAttribute("Name", this.name));
            element.Add(new XAttribute("X", this.locX));
            element.Add(new XAttribute("Y", this.locY));
            element.Add(new XAttribute("Width", this.width));
            element.Add(new XAttribute("Height", this.height));

            return element;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void notifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class AnimationContainer : INotifyPropertyChanged
    {
        public ObservableCollection<SpriteContainer> sprites = new ObservableCollection<SpriteContainer>();
        private string _name;
        public string name
        {
            get { return _name; }
            set
            {
                if (this._name != value)
                {
                    _name = value;
                    notifyPropertyChanged("name");
                }
            }
        }
        public int tallest = 0;
        public int fullWidth = 0;
        public int locY = 0;

        public AnimationContainer()
        {

        }
        private void updateCheck()
        {
            if (sprites.Count > 0)
            {
                int tempWidth = 0;
                foreach (SpriteContainer i in this.sprites)
                {
                    tempWidth += i.width;
                    tallest = 0;
                    if (i.height > this.tallest)
                    {
                        this.tallest = i.height;
                    }
                }
                fullWidth = tempWidth;
            }
            else
            {
                this.fullWidth = 0;
                this.tallest = 0;
            }
        }

        public void addSprite(SpriteContainer a_add)
        {
            sprites.Add(a_add);
            fullWidth += a_add.width;
            if (a_add.height > tallest)
            {
                tallest = a_add.height;
            }
            updateCheck();
        }
        public void removeSprite(SpriteContainer a_remove)
        {
            if (a_remove != null)
            {
                if (sprites.Contains(a_remove))
                {
                    sprites.RemoveAt(sprites.IndexOf(a_remove));
                }
            }
            updateCheck();
        }

        public override string ToString()
        {
            return this.name;
        }

        public XElement ToXML()
        {
            XElement element = new XElement("Animation");
            element.Add(new XAttribute("Name", this.name));
            foreach (SpriteContainer i in sprites)
            {
                element.Add(i.ToXML());
            }

            return element;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void notifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //BitmapImage show;
        public ObservableCollection<AnimationContainer> animations;
        private int animNumber = 1;
        public int width = 0;
        public int height = 0;

        public MainWindow()
        {
            animations = new ObservableCollection<AnimationContainer>();
            InitializeComponent();

            animationList.ItemsSource = animations;

            AnimationContainer newAnimation = new AnimationContainer();
            newAnimation.name = "Animation0";
            animations.Add(newAnimation);

            spriteList.ItemsSource = animations[0].sprites;
        }

        private void updateCanvas()
        {
            //Sheet canvas
            canvas1.Children.Clear();
            int offsetX = 0;
            int offsetY = 0;
            int mostWide = 0;
            foreach (AnimationContainer i in animations)
            {
                i.locY = offsetY;
                offsetX = 0;
                foreach (SpriteContainer j in i.sprites)
                {
                    Image newImage = new Image();
                    newImage.Source = j.imgsrc;
                    Canvas.SetLeft(newImage, offsetX);
                    Canvas.SetTop(newImage, offsetY);
                    canvas1.Children.Add(newImage);
                    j.locX = offsetX;
                    j.locY = offsetY;
                    offsetX += j.width;
                }
                offsetY += i.tallest;
                if (offsetX > mostWide)
                {
                    mostWide = offsetX;
                }
            }
            canvas1.Width = mostWide;
            canvas1.Height = offsetY;

            //Animation canvas
            canvas2.Children.Clear();
            AnimationContainer animToDisplay = animationList.SelectedItem as AnimationContainer;

            offsetX = 0;
            offsetY = 0;
            if (animToDisplay != null)
            {
                foreach (SpriteContainer j in animToDisplay.sprites)
                {
                    Image newImage = new Image();
                    newImage.Source = j.imgsrc;
                    Canvas.SetLeft(newImage, offsetX);
                    Canvas.SetTop(newImage, 0);
                    canvas2.Children.Add(newImage);
                    offsetX += j.width;
                    if (offsetY < j.height)
                    {
                        offsetY = j.height;
                    }
                }
                canvas2.Width = offsetX;
                canvas2.Height = offsetY;
            }
        }
        private void updateInfo()
        {
            int tempX = 0;
            //int finalX = 0;
            int tempY = 0;
            int mostWide = 0;
            foreach (AnimationContainer j in animations)
            {
                tempX = 0;
                foreach (SpriteContainer i in j.sprites)
                {
                    tempX += i.width;
                }
                if (tempX > mostWide)
                {
                    mostWide = tempX;
                }
                tempY += j.tallest;
            }
            width = mostWide;
            height = tempY;

            sheetDimensions.Text = width.ToString() + " x " + height.ToString();

            //sheetWidth.Text = width.ToString() + " pixels";
            //sheetHeight.Text = height.ToString() + " pixels";

            AnimationContainer animTargeted = animationList.SelectedItem as AnimationContainer;
            if (animTargeted != null)
            {
                spriteList.ItemsSource = animTargeted.sprites;
                animName.Text = animTargeted.name;
                animWidth.Text = animTargeted.fullWidth.ToString() + " pixels";
                animHeight.Text = animTargeted.tallest.ToString() + " pixels";
                animLocY.Text = animTargeted.locY.ToString() + " pixels";
            }
            SpriteContainer spriteTargeted = spriteList.SelectedItem as SpriteContainer;
            if (spriteTargeted != null)
            {
                //spriteList.ItemsSource = animTargeted.sprites;
                spriteName.Text = spriteTargeted.name;
                spriteWidth.Text = spriteTargeted.width.ToString() + " pixels";
                spriteHeight.Text = spriteTargeted.height.ToString() + " pixels";
                spriteLocX.Text = spriteTargeted.locX.ToString() + " pixels";
                spriteLocY.Text = spriteTargeted.locY.ToString() + " pixels";
            }
            //updateCanvas();
        }

        private void AddSprite_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (animationList == null) { return; }
            AnimationContainer animTargeted = animationList.SelectedItem as AnimationContainer;

            if (animTargeted != null)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }
        private void AddSprite_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Multiselect = true;
            openFile.Filter = "Image Files (*.bmp;*.png;*.jpeg)|*.bmp;*.png;*.jpeg";

            AnimationContainer animToAddTo = animationList.SelectedItem as AnimationContainer;

            if (animToAddTo != null)
            {
                if (openFile.ShowDialog() == true)
                {
                    foreach (string ifile in openFile.FileNames)
                    {
                        SpriteContainer newSprite = new SpriteContainer(new Uri(ifile));
                        animToAddTo.addSprite(newSprite);

                        updateCanvas();
                        updateInfo();
                    }
                }
            }
        }

        private void RemoveSprite_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (animationList == null) { return; }
            AnimationContainer animTargeted = animationList.SelectedItem as AnimationContainer;

            if (spriteList == null) { return; }
            SpriteContainer spriteTargeted = spriteList.SelectedItem as SpriteContainer;

            if (spriteTargeted != null && animTargeted != null)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }
        private void RemoveSprite_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //spriteList.SelectedItem
            AnimationContainer animToRemoveFrom = animationList.SelectedItem as AnimationContainer;
            SpriteContainer spriteToRemove = spriteList.SelectedItem as SpriteContainer;
            if (spriteToRemove != null && animToRemoveFrom != null)
            {
                animToRemoveFrom.removeSprite(spriteToRemove);
            }
            updateCanvas();
            updateInfo();
        }

        private void CanAlways(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void AddAnimation_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AnimationContainer newAnimation = new AnimationContainer();
            newAnimation.name = "Animation" + animNumber;
            animations.Add(newAnimation);
            animNumber += 1;
        }

        private void RemoveAnimation_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (animationList == null) { return; }
            AnimationContainer animTargeted = animationList.SelectedItem as AnimationContainer;

            if (animTargeted != null /*&& animations.Count() > 1*/)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }
        private void RemoveAnimation_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AnimationContainer animToRemove = animationList.SelectedItem as AnimationContainer;
            if (animToRemove != null)
            {
                if (animations.Contains(animToRemove))
                {
                    animations.RemoveAt(animations.IndexOf(animToRemove));
                }
            }
            updateCanvas();
            updateInfo();
        }

        private void MoveSpriteUp_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (animationList == null) { return; }
            AnimationContainer animTargeted = animationList.SelectedItem as AnimationContainer;

            if (spriteList == null) { return; }
            SpriteContainer spriteTargeted = spriteList.SelectedItem as SpriteContainer;

            if (spriteTargeted != null && animTargeted != null && animTargeted.sprites.IndexOf(spriteTargeted) != 0)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }
        private void MoveSpriteUp_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AnimationContainer animTargeted = animationList.SelectedItem as AnimationContainer;
            SpriteContainer spriteTargeted = spriteList.SelectedItem as SpriteContainer;
            //SpriteContainer spriteToSwap = animTargeted.sprites[animTargeted.sprites.IndexOf(spriteTargeted) - 1];
            int targetedIndex = animTargeted.sprites.IndexOf(spriteTargeted);
            int toSwapIndex = targetedIndex - 1;
            SpriteContainer temp = spriteTargeted;

            animTargeted.sprites[targetedIndex] = animTargeted.sprites[toSwapIndex];
            animTargeted.sprites[toSwapIndex] = temp;

            spriteList.SelectedIndex = toSwapIndex;

            updateCanvas();
            updateInfo();
        }

        private void MoveAnimationUp_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (animationList == null) { return; }
            AnimationContainer animTargeted = animationList.SelectedItem as AnimationContainer;

            if (animTargeted != null && animations.IndexOf(animTargeted) != 0)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }
        private void MoveAnimationUp_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AnimationContainer animTargeted = animationList.SelectedItem as AnimationContainer;

            int targetedIndex = animations.IndexOf(animTargeted);
            int toSwapIndex = targetedIndex - 1;
            AnimationContainer temp = animTargeted;

            animations[targetedIndex] = animations[toSwapIndex];
            animations[toSwapIndex] = temp;

            animationList.SelectedIndex = toSwapIndex;

            updateCanvas();
            updateInfo();
        }

        private void RenameAnimation_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //if (animationList == null) { return; }
            AnimationContainer animTargeted = animationList.SelectedItem as AnimationContainer;

            RenameDialog inputDialog = new RenameDialog(animTargeted.name);
            if (inputDialog.ShowDialog() == true)
            {
                animTargeted.name = inputDialog.input;
            }
        }

        private void RenameSprite_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AnimationContainer animTargeted = animationList.SelectedItem as AnimationContainer;
            SpriteContainer spriteTargeted = spriteList.SelectedItem as SpriteContainer;

            RenameDialog inputDialog = new RenameDialog(spriteTargeted.name);
            if (inputDialog.ShowDialog() == true)
            {
                spriteTargeted.name = inputDialog.input;
            }
        }

        private void export_combineImages(object sender, RoutedEventArgs e)
        {
            WriteableBitmap export = BitmapFactory.New(width, height);
            int offsetX = 0;
            int offsetY = 0;
            foreach (AnimationContainer j in animations)
            {
                offsetX = 0;
                foreach (SpriteContainer i in j.sprites)
                {
                    Rect tempRect = new Rect(offsetX, offsetY, i.width, i.height);
                    export.Blit(tempRect, new WriteableBitmap(BitmapFactory.ConvertToPbgra32Format(i.imgsrc)), new Rect(0, 0, i.width, i.height));
                    offsetX += i.width;
                }
                offsetY += j.tallest;
            }

            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "PNG File (*.png)|*.png";
            if (saveFile.ShowDialog() == true)
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(export));

                FileStream fileToSave = new FileStream(saveFile.FileName, FileMode.Create);
                encoder.Save(fileToSave);
                fileToSave.Close();
            }
            
        }

        private void export_compileXML(object sender, RoutedEventArgs e)
        {
            XDocument document = new XDocument();
            XElement _base = new XElement("base");
            foreach (AnimationContainer i in animations)
            {
                _base.Add(i.ToXML());
            }
            document.Add(_base);
            
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "XML File (*.XML)|*.XML";
            if (saveFile.ShowDialog() == true)
            {
                document.Save(saveFile.FileName);
            }
        }

        private void animationList_MouseUp(object sender, MouseButtonEventArgs e)
        {
            updateCanvas();
            updateInfo();
            
        }

    }
}