using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace KULUMBA_Francis_TDK
{
    class Program
    {
        private static int index = 0;
        private static MyImage Test = new MyImage("Test.bmp");

        private static string drawMenu(List<string> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (i == index)
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.White;

                    Console.WriteLine(items[i]);
                }
                else
                {
                    Console.WriteLine(items[i]);
                }
                Console.ResetColor();
            }

            ConsoleKeyInfo ckey = Console.ReadKey();

            if (ckey.Key == ConsoleKey.DownArrow)
            {
                if (index == items.Count - 1)
                {
                    //index = 0; //Remove the comment to return to the topmost item in the list
                }
                else { index++; }
            }
            else if (ckey.Key == ConsoleKey.UpArrow)
            {
                if (index <= 0)
                {
                    //index = menuItem.Count - 1; //Remove the comment to return to the item in the bottom of the list
                }
                else { index--; }
            }
            else if (ckey.Key == ConsoleKey.Enter)
            {
                return items[index];
            }
            else
            {
                return "";
            }

            Console.Clear();
            return "";
        }

        static void Afficher(string path)
        {
            var filePath = path;
            ProcessStartInfo Info = new ProcessStartInfo()
            {
            FileName = "mspaint.exe",
            WindowStyle = ProcessWindowStyle.Maximized,
            Arguments = filePath
            };
            Process.Start(Info);
        }

        static void Main(string[] args)
        {
            List<string> menuItems = new List<string>() {
                "Traitement photo",
                "Creation fractale",
                "Steganographie",
                "Exit"
            };

            Console.CursorVisible = false;
            while (true)
            {
                Console.Clear();
                string selectedMenuItem = drawMenu(menuItems);
                switch (selectedMenuItem)
                {
                    case "Traitement photo":
                        Console.Clear();
                        Console.Write("Entrez le nom du fichier sur lequel vous souhaitez travailler \n >");
                        string fichier = Console.ReadLine();
                        MyImage picture = new MyImage(fichier);
                        Afficher(fichier);
                        List<string> menuItems1 = new List<string>() {
                        "Proportion",
                        "Filtre",
                        "Histogramme",
                        "Exit"
                        };
                        while (true)
                        {
                            Console.Clear();
                            string selectedMenuItem1 = drawMenu(menuItems1);
                            switch (selectedMenuItem1)
                            {
                                case "Proportion":
                                    Console.Clear();
                                    List<string> menuItems11 = new List<string>() {
                                    "Retour",
                                    "Rotation 90",
                                    "Rotation 180",
                                    "Rotation 270",
                                    "Effet mirroir",
                                    "Agrandir",
                                    "Retrecir"
                                    };
                                    bool retour = false;
                                    while(retour == false)
                                    {
                                        Console.Clear();
                                        string selectedMenuItem11 = drawMenu(menuItems11);
                                        switch(selectedMenuItem11)
                                        {
                                            case "Retour":
                                                retour = true;
                                                break;

                                            case "Rotation 90":
                                                picture.Rotation90();
                                                Afficher("sortie.bmp");
                                                break;

                                            case "Rotation 180":
                                                picture.Rotation180();
                                                Afficher("sortie.bmp");
                                                break;

                                            case "Rotation 270":
                                                picture.Rotation270();
                                                Afficher("sortie.bmp");
                                                break;

                                            case "Effet mirroir":
                                                picture.EffetMirroir();
                                                Afficher("sortie.bmp");
                                                break;

                                            case "Agrandir":
                                                Console.Write("Entrez un facteur d'agrandissement ENTIER \n >");
                                                picture.Agrandir(Convert.ToInt32(Console.ReadLine()));
                                                Afficher("sortie.bmp");
                                                break;

                                            case "Retrecir":
                                                Console.Write("Entrez le facteur de rétrecissement ENTIER et MULTIPLE DE 2 \n >");
                                                picture.Retrecir(Convert.ToInt32(Console.ReadLine()));
                                                Afficher("sortie.bmp");
                                                break;

                                        }
                                    }
                                    break;

                                case "Filtre":
                                    Console.Clear();
                                    List<string> menuItems12 = new List<string>() {
                                        "Retour",
                                        "Detection de bord",
                                        "Renforcement des bords",
                                        "Flou",
                                        "Repoussage",
                                        "Contraste +",
                                        "Niveau de gris",
                                        "Noir et blanc",
                                        "Dessin",
                                    };
                                    bool retour2 = false;
                                    while(retour2 == false)
                                    {
                                        Console.Clear();
                                        string selectedMenuItems12 = drawMenu(menuItems12);
                                        switch(selectedMenuItems12)
                                        {
                                            case "Retour":
                                                retour2 = true;
                                                break;

                                            case "Detection de bord":
                                                picture.DetectBord();
                                                Afficher("sortie.bmp");
                                                break;

                                            case "Renforcement des bords":
                                                picture.StrengthBord();
                                                Afficher("sortie.bmp");
                                                break;

                                            case "Flou":
                                                picture.Blur();
                                                Afficher("sortie.bmp");
                                                break;

                                            case "Repoussage":
                                                picture.Repulse();
                                                Afficher("sortie.bmp");
                                                break;

                                            case "Contraste +":
                                                picture.StrengthContrast();
                                                Afficher("sortie.bmp");
                                                break;

                                            case "Niveau de gris":
                                                picture.ToGrey();
                                                Afficher("sortie.bmp");
                                                break;

                                            case "Noir et blanc":
                                                picture.ToBlackAndWhite();
                                                Afficher("sortie.bmp");
                                                break;

                                            case "Dessin":
                                                picture.ToGrey();
                                                picture.ToInverse();
                                                MyImage picture2 = new MyImage("sortie2.bmp");
                                                picture.Blur();
                                                picture.ToDrawing(picture2);
                                                picture.ToGrey();
                                                Afficher("sortie.bmp");
                                                break;
                                        }
                                    }
                                    break;

                                case "Histogramme":
                                    MyImage Histo = new MyImage(fichier);
                                    Histo.Histogramme();
                                    Afficher("histogramme.bmp");
                                    break;

                                case "Exit":
                                    Environment.Exit(0);
                                    break;
                            }


                        }
                        break;

                    case "Creation fractale":
                        MyImage oui = new MyImage("Test.bmp");
                        oui.Mandelbrot();
                        Afficher("fractale.bmp");
                        break;

                    case "Steganographie":
                        //Console.Clear();
                        //MyImage Fractale = new MyImage("lena.bmp");
                        //MyImage Fractale2 = new MyImage("coco.bmp");
                        //Fractale.Stéganographie(Fractale2.image);
                        //Afficher("sortie.bmp");
                        MyImage sortie = new MyImage("stego.bmp");
                        sortie.Décoder_Stéganographie2();
                        Afficher("sortie.bmp");
                        break;

                    case "Exit":
                        Environment.Exit(0);
                        break;
                }
            }
        }
    }
}
