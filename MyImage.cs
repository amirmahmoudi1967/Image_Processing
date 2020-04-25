using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;



namespace KULUMBA_Francis_TDK
{
    class MyImage
    {
        public string type;
        int taille;
        int Offset;
        int tailleHeader;
        int largeur; int hauteur;
        int profondeurCouleurs;
        int tailleRAW;
        int resolutionHorizontale;
        int resolutionVerticale;
        public Pixel[,] image;

        #region Constructeur
        public MyImage(string filename)
        {
            byte[] MyFile;
            if (filename[filename.Length - 1].CompareTo('p') == 0)
            {
                MyFile = ReadByte(filename);
                type = "bmp";
            }
            else
            {
                MyFile = ReadFile(filename);
                type = "csv";
            }

            taille = Convertir_Endian_To_Int(MyFile[2], MyFile[3], MyFile[4], MyFile[5]);
            Offset = Convertir_Endian_To_Int(MyFile[10], MyFile[11], MyFile[12], MyFile[13]);
            tailleHeader = Convertir_Endian_To_Int(MyFile[14], MyFile[15], MyFile[16], MyFile[17]);
            largeur = Convertir_Endian_To_Int(MyFile[18], MyFile[19], MyFile[20], MyFile[21]);
            hauteur = Convertir_Endian_To_Int(MyFile[22], MyFile[23], MyFile[24], MyFile[25]);
            profondeurCouleurs = Convertir_Endian_To_Int(MyFile[28], MyFile[29], 0, 0);
            tailleRAW = Convertir_Endian_To_Int(MyFile[34], MyFile[35], MyFile[36], MyFile[37]);
            resolutionHorizontale = Convertir_Endian_To_Int(MyFile[38], MyFile[39], MyFile[40], MyFile[41]);
            resolutionVerticale = Convertir_Endian_To_Int(MyFile[42], MyFile[43], MyFile[44], MyFile[45]);
            image = Image(MyFile, largeur, hauteur, Offset);

        }
        #endregion Constructeur

        #region Lecture fichier
        static byte[] ReadByte(string filename)
        {
            try
            {
                StreamReader str = null;
                str = new StreamReader(filename);
                byte[] MyFile = File.ReadAllBytes(filename);
                return MyFile;
            }
            catch (FileNotFoundException f)
            {
                Console.WriteLine("Le fichier n'existe pas ");
                Console.WriteLine(f.Message);
                return null;
            }
            finally { Console.WriteLine("Sortie du try"); };
        }

        public byte[] ReadFile(string filename)
        {
            // Création d'une variable de type StreamReader
            StreamReader flux = new StreamReader(filename);
            string line;
            try
            {
                byte[] HEADER = new byte[40];
                line = flux.ReadLine();
                {
                    string[] string_HEADER = line.Split(';');
                    for (int i = 0; i < string_HEADER.Length; i++)
                    {
                        if (string_HEADER[i].Length >= 1) HEADER[i] = Convert.ToByte(string_HEADER[i]);
                    }
                }
                byte[] MyFile = new byte[Convertir_Endian_To_Int(HEADER[2], HEADER[3], HEADER[4], HEADER[5])];
                int j = 0;

                string[] Temporaire;
                string[] lignes = File.ReadAllLines(filename);
                foreach (string ligne in lignes)
                {
                    Temporaire = ligne.Split(';');
                    int index = 0;
                    for (int i = 0; i < Temporaire.Length; i++)
                    {
                        if (Temporaire[i].Length >= 1)
                        {
                            MyFile[j + i] = Convert.ToByte(Temporaire[i]);
                            index++;
                        }
                    }
                    j += index;
                }
                return MyFile;
            }
            catch (FileNotFoundException e)
            { Console.WriteLine(e.Message); return null; }
            catch (IOException e)
            { Console.WriteLine(e.Message); return null; }
            catch (Exception e)
            {
                // Récupération des erreurs
                Console.WriteLine(e.Message);
                return null;
            }
            finally
            {   // Fermeture du flux dans tous les cas
                if (flux != null) { flux.Close(); }
            }
        }


        static Pixel[,] Image(byte[] MyFile, int largeur, int hauteur, int offset)
        {
            Pixel[,] image = new Pixel[hauteur, largeur];
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    image[i, j] = new Pixel(MyFile[offset], MyFile[offset + 1], MyFile[offset + 2]);
                    offset += 3;
                }
            }
            return image;
        }
        #endregion Lecture fichier

        #region utilitaire
		/// <summary>
		/// sert à modifier la largeur et la hauteur d'un fichier bmp après y avoir fait des modifications
		/// </summary>
		/// <param name="picture"></param> correspond à la nouvelle matrice de pixel
        public void ChangerProprietees(Pixel[,] picture)
        {
            taille -= image.GetLength(0) * image.GetLength(1);
            largeur = picture.GetLength(1);
            hauteur = picture.GetLength(0);
            taille += largeur * hauteur * 9;
            image = picture;
        }
		/// <summary>
		/// Applique une matrice de convolution 3*3 à n'importe quelle matrice image
		/// C'est un filtre passe haut et passe bas, il met à 0 quand la valeur est négative ou à 255 quand elle est > 255)
		/// </summary>
		/// <param name="mat_conv"></param> correspond à la matrice de convolution
		/// <param name="ligne"></param> désigne la ligne de l'image sur laquelle on travaille
		/// <param name="colonne"></param> désigne la colonne de l'image sur laquelle on travaille
		/// <returns></returns>
		public Pixel Convolution(int[,] mat_conv, int ligne, int colonne)
        {
            //Applique une matrice de convolution 3*3 à n'importe quelle matrice image
            int valeurR = 0;
            int valeurG = 0;
            int valeurB = 0;

            int x = -1;
            int y = -1;

            for (int i = 0; i < mat_conv.GetLength(0); i++)
            {
                y = -1;
                for (int j = 0; j < mat_conv.GetLength(1); j++)
                {
                    if (ligne + x >= 0 && ligne + x < image.GetLength(0) && colonne + y >= 0 && colonne + y < image.GetLength(1)) //On vérifie que l'on ne dépasse pas des limites du tableau 
                    {
                        valeurR += image[ligne + x, colonne + y].R * mat_conv[i, j];
                        valeurG += image[ligne + x, colonne + y].G * mat_conv[i, j];
                        valeurB += image[ligne + x, colonne + y].B * mat_conv[i, j];
                    }
                    y++;
                }
                x++;
            }
            //On vérifie que la couleur du pixel peut s'écrire sur 2 octets
            if (valeurR < 0) valeurR = 0;
            if (valeurR > 255) valeurR = 255;

            if (valeurG < 0) valeurG = 0;
            if (valeurG > 255) valeurG = 255;

            if (valeurB < 0) valeurB = 0;
            if (valeurB > 255) valeurB = 255;

            return new Pixel(valeurR, valeurG, valeurB);
        }
		/// <summary>
		/// Au lieu de faire un filtre passe haut  et passe bas(mettre à 0 quand la valeur est négative ou à 255 quand elle est > 255)
		/// on fait la moyenne des coefficients de la matrice de convolution
		/// </summary>
		/// <param name="mat_conv"></param>correspond à la matrice de convolution
		/// <param name="ligne"></param>désigne la ligne de l'image sur laquelle on travaille
		/// <param name="colonne"></param>désigne la colonne de l'image sur laquelle on travaille
		/// <returns></returns>
		public Pixel ConvolutionNormalisee(int[,] mat_conv, int ligne, int colonne)
        {
            int valeurR = 0;
            int valeurG = 0;
            int valeurB = 0;
            int somme = 0;

            int x = -1;
            int y = -1;

            for (int i = 0; i < mat_conv.GetLength(0); i++)
            {
                y = -1;
                for (int j = 0; j < mat_conv.GetLength(1); j++)
                {
                    if (ligne + x >= 0 && ligne + x < image.GetLength(0) && colonne + y >= 0 && colonne + y < image.GetLength(1))
                    {
                        valeurR += image[ligne + x, colonne + y].R * mat_conv[i, j];
                        valeurG += image[ligne + x, colonne + y].G * mat_conv[i, j];
                        valeurB += image[ligne + x, colonne + y].B * mat_conv[i, j];
                    }
                    somme += Math.Abs(mat_conv[i, j]);
                    y++;
                }
                x++;
            }
            valeurR /= somme;
            valeurG /= somme;
            valeurB /= somme;

            //Même méthode que la fonction précédente mais au lieu d'appliquer un filtre passe/haut on normalise
            return new Pixel(valeurR, valeurG, valeurB);
        }
		/// <summary>
		/// Sert à faire la moyenne des pixels compris dans la sous matrice d'étude
		/// </summary>
		/// <param name="a"></param> correspond de la matrice quand elle est rétréci
		/// <param name="x"></param> correspond au coefficent en ligne pour trouver la case de la matrice
		/// <param name="y"></param> correspond au coefficent en colonne pour trouver la case de la matrice
		/// <returns></returns>
		public Pixel MoyennePixel(int a, int x, int y)
        {
            int valeurR = 0;
            int valeurG = 0;
            int valeurB = 0;

            for (int i = 0; i < a; i++)
                for (int j = 0; j < a; j++)
                {
                    valeurR += image[i + x, j + y].R;
                    valeurG += image[i + x, j + y].G;
                    valeurB += image[i + x, j + y].B;
                }

            return new Pixel(valeurR / (a * a), valeurG / (a * a), valeurB / (a * a));
        }
        #endregion utilitaire

        #region Conversions
        public int Convertir_Endian_To_Int(int a, int b, int c, int d)
        {
            return a + b * (int)Math.Pow(2, 8) + c * (int)Math.Pow(2, 16) + d * (int)Math.Pow(2, 24);
        }

        public int[] ConvertirInt_ToBinaire(int valeur)
        {
            //Sur 4 octets
            int[] tableaubinaire = new int[32];
            int valoctets = valeur;
            int i = 0;
            while (valoctets > 0)
            {
                int reste = valoctets % 2;
                tableaubinaire[i] = reste;
                i++;
                valoctets = (valoctets - reste) / 2;
            }
            return tableaubinaire;
        }

        public int[] ConvertirInt_ToBinaireV2(int valeur)
        {
            //Sur un octet
            int[] tableaubinaire = new int[8];
            int valoctets = valeur;
            int i = 0;
            while (valoctets > 0)
            {
                int reste = valoctets % 2;
                tableaubinaire[i] = reste;
                i++;
                valoctets = (valoctets - reste) / 2;
            }
            return tableaubinaire;
        }

        public byte[] ConvertirBinaire_ToEndian(int[] tableaubinaire, byte[] tabEndian, int i = 0, int j = 0, int compteur = 0)
        {
            int valeur = 0;
            if (i != 32)
            {
                while (j <= 7)
                {
                    valeur += tableaubinaire[i + j] * (int)Math.Pow(2, j);
                    j++;
                }
                tabEndian[compteur] = Convert.ToByte(valeur);
            }
            if (i == 32) return tabEndian;
            else return ConvertirBinaire_ToEndian(tableaubinaire, tabEndian, i += 8, 0, compteur += 1);
        }

        public byte[] Convertir_Int_To_Endian(int valeur)
        {
            byte[] tabEndian = new byte[4];
            int[] tabBinaire = ConvertirInt_ToBinaire(valeur);
            byte[] tabeEndian = ConvertirBinaire_ToEndian(tabBinaire, tabEndian);
            return tabEndian;
        }

        public int Convertir_Binaire_To_Int(int[] tab)
        {
            int puissance = 7;
            int valeur = 0;
            for (int i = 0; i < tab.Length; i++)
            {
                valeur += Convert.ToInt32(tab[i] * Math.Pow(2, puissance));
                puissance--;
            }
            return valeur;
        }

        public void FromImage_ToFIle(string path)
        {
            byte[] Octets = new byte[taille];

            Octets[0] = 66;
            Octets[1] = 77;
            Octets[26] = 1;

            for (int i = 6; i < Offset; i++)
            {
                if (i <= 9 || i == 27 || i >= 30 && i <= 33 || i >= 46)
                    Octets[i] = 0;
            }

            int j = 2;
            while (j < Octets.Length)
            {
                if (j <= 5)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Octets[j] = Convertir_Int_To_Endian(taille)[k];
                        j++;
                    }
                }

                if (j >= 10 && j <= 13)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Octets[j] = Convertir_Int_To_Endian(Offset)[k];
                        j++;
                    }
                }

                if (j >= 14 && j <= 17)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Octets[j] = Convertir_Int_To_Endian(tailleHeader)[k];
                        j++;
                    }
                }

                if (j >= 18 && j <= 21)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Octets[j] = Convertir_Int_To_Endian(largeur)[k];
                        j++;
                    }
                }

                if (j >= 22 && j <= 25)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Octets[j] = Convertir_Int_To_Endian(hauteur)[k];
                        j++;
                    }
                }

                if (j == 28 || j == 29)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        Octets[j] = Convertir_Int_To_Endian(profondeurCouleurs)[k];
                        j++;
                    }
                }

                if (j >= 34 && j <= 37)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Octets[j] = Convertir_Int_To_Endian(tailleRAW)[k];
                        j++;
                    }
                }

                if (j >= 38 && j <= 41)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Octets[j] = Convertir_Int_To_Endian(resolutionHorizontale)[k];
                        j++;
                    }
                }

                if (j >= 42 && j <= 45)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Octets[j] = Convertir_Int_To_Endian(resolutionVerticale)[k];
                        j++;
                    }
                }
                j++;
            }

            int x = Offset;
            for (int a = 0; a < image.GetLength(0); a++)
            {
                for (int b = 0; b < image.GetLength(1); b++)
                {
                    Octets[x] = Convertir_Int_To_Endian(image[a, b].R)[0];
                    x++;
                    Octets[x] = Convertir_Int_To_Endian(image[a, b].G)[0];
                    x++;
                    Octets[x] = Convertir_Int_To_Endian(image[a, b].B)[0];
                    x++;
                }
            }

            File.WriteAllBytes(path, Octets);
        }
		#endregion Conversions

		#region Proportions
		/// <summary>
		/// On effectue une rotation 90° dans le sens direct
		/// on créé une matrice qui a pour dimension [nbr colonne de l'ancienne matrice ,nbr ligne de l'ancienne matrice]
		/// le principe revient à faire une succession d'inversions de lignes puis de colonnes
		/// </summary>
		public void Rotation90()
        {
            Pixel[,] newMatrix = new Pixel[image.GetLength(1), image.GetLength(0)];
            int newColumn, newRow = 0;
            for (int oldColumn = image.GetLength(1) - 1; oldColumn >= 0; oldColumn--)
            {
                newColumn = 0;
                for (int oldRow = 0; oldRow < image.GetLength(0); oldRow++)
                {
                    newMatrix[newRow, newColumn] = image[oldRow, oldColumn];
                    newColumn++;
                }
                newRow++;
            }
            image = newMatrix;
            ChangerProprietees(newMatrix);
            EffetMirroir();
            FromImage_ToFIle(@"sortie.bmp");
        }
		/// <summary>
		/// On effectue une rotation 90° dans le sens indirect
		/// on créé une matrice qui a pour dimension [nbr colonne de l'ancienne matrice ,nbr ligne de l'ancienne matrice]
		/// le principe revient à faire une succession d'inversions de colonnes puis de lignes
		/// </summary>
		public void Rotation90B()
        {
            Pixel[,] newMatrix = new Pixel[image.GetLength(1), image.GetLength(0)];
            int newColumn, newRow = 0;
            for (int oldColumn = image.GetLength(1) - 1; oldColumn >= 0; oldColumn--)
            {
                newColumn = 0;
                for (int oldRow = 0; oldRow < image.GetLength(0); oldRow++)
                {
                    newMatrix[newRow, newColumn] = image[oldRow, oldColumn];
                    newColumn++;
                }
                newRow++;
            }
            image = newMatrix;
            ChangerProprietees(newMatrix);
            FromImage_ToFIle(@"sortie.bmp");
        }
		/// <summary>
		/// On effectue une rotation 180° dans le sens direct
		/// on créé une matrice qui a les même dimensions que la première
		/// le principe revient à échanger les valeurs stockés en indice [ligneDebut,i] et [ligneFin,i] en incrémentant à chaque tour le i et
		/// et ligneDebut en décrementant ligneFin
		/// </summary>
		public void Rotation180()
        {
            int ligneDebut = 0;
            int ligneFin = image.GetLength(0) - 1;
            int colonneDebut = 0;
            int colonneFin = image.GetLength(1) - 1;
            Pixel temporaire = new Pixel(0, 0, 0);

            while (ligneDebut < ligneFin)
            {
                for (int i = 0; i < image.GetLength(1); i++)
                {
                    temporaire = image[ligneDebut, i];
                    image[ligneDebut, i] = image[ligneFin, i];
                    image[ligneFin, i] = temporaire;
                }
                ligneDebut++;
                ligneFin--;
            }

            while (colonneDebut < colonneFin)
            {
                for (int i = 0; i < image.GetLength(0); i++)
                {
                    temporaire = image[i, colonneDebut];
                    image[i, colonneDebut] = image[i, colonneFin];
                    image[i, colonneFin] = temporaire;
                }
                colonneDebut++;
                colonneFin--;
            }
            FromImage_ToFIle(@"sortie.bmp");
        }
		/// <summary>
		/// la rotation 270 dans le sens direct correspond à une rotation 90° dans le sens direct puis une rotation 180° dans le sens direct 
		/// </summary>
		public void Rotation270()//Sens trigo
        {
            Rotation180();
            Rotation90();
            FromImage_ToFIle(@"sortie.bmp");
        }
		/// <summary>
		/// on créé une matrice qui a les même dimensions que la première
		/// le principe revient à échanger les valeurs stockés en indice [i,colonneDebut] et [i,colonneFin] (l'échange
		/// s'effectue avec l'utilisation d'une valuer temporelle pour éviter d'effacer la valuer [i,colonneDebut])
		/// en incrémentant à chaque tour le i et colonneDebut et en décrementant colonneFin
		/// l'effet mirroir s'effectue en échangeant 
		/// </summary>
		public void EffetMirroir()
        {
            int colonneDebut = 0;
            int colonneFin = image.GetLength(1) - 1;
            Pixel temporaire = new Pixel(0, 0, 0);

            while (colonneDebut < colonneFin)
            {
                for (int i = 0; i < image.GetLength(0); i++)
                {
                    temporaire = image[i, colonneDebut];
                    image[i, colonneDebut] = image[i, colonneFin];
                    image[i, colonneFin] = temporaire;
                }
                colonneDebut++;
                colonneFin--;
            }
            FromImage_ToFIle(@"sortie.bmp");
        }
		/// <summary>
		/// en agrandissant par le facteur "a" le pixel devient une matrice [a,a]
		/// </summary>
		/// <param name="a"></param> ce paramètre désigne le coefficient d'agrandissement qui est un entier >= à 1
        public void Agrandir(int a)
        {
            //Ne fonctionne qu'avec des facteur d'agrandissement entiers, supérieurs ou égaux à 1
            Pixel[,] picture = new Pixel[hauteur * a, largeur * a];

            int ligne = 0;
            int colonne = 0;

            for (int i = 0; i < picture.GetLength(0); i += a)
            {
                for (int j = 0; j < picture.GetLength(1); j += a)
                {

                    for (int x = 0; x < a; x++)
                        for (int y = 0; y < a; y++) picture[i + x, j + y] = image[ligne, colonne];

                    if (colonne == image.GetLength(1) - 1)
                    {
                        colonne = 0;
                        ligne++;
                    }
                    else colonne++;
                }
            }

            ChangerProprietees(picture);
            FromImage_ToFIle(@"sortie.bmp");
        }
		/// <summary>
		/// en retrécissant par le facteur "a" la matrice [a,a] devient un seul pixel qui aura de R,G et B les valeurs moyennes
		/// pour les pixels compris dans la matrice [a,a]
		/// </summary>
		/// <param name="a"></param> ce paramètre désigne le coefficient d'agrandissement qui est un entier >= à 1
		public void Retrecir(int a)
        {
            //Ne fonctionne qu'avec des facteurs de retrecissement multiples de 2
            Pixel[,] picture = new Pixel[hauteur / a, largeur / a];
            int ligne = 0;
            int colonne = 0;

            for (int i = 0; i < image.GetLength(0); i += a)
                for (int j = 0; j < image.GetLength(1); j += a)
                {
                    picture[ligne, colonne] = MoyennePixel(a, i, j);
                    if (colonne == picture.GetLength(1) - 1)
                    {
                        colonne = 0;
                        ligne++;
                    }
                    else colonne++;
                }

            ChangerProprietees(picture);
            FromImage_ToFIle(@"sortie.bmp");
        }
        #endregion Proportions

        #region filtre
        public void DetectBord()
        {
            Pixel[,] picture = new Pixel[image.GetLength(0), image.GetLength(1)];
            int[,] mat_conv = new int[,] { { 0, 1, 0 }, { 1, -4, 1 }, { 0, 1, 0 } };

            for (int i = 0; i < image.GetLength(0); i++)
                for (int j = 0; j < image.GetLength(1); j++)
                    picture[i, j] = Convolution(mat_conv, i, j);

            image = picture;
            FromImage_ToFIle(@"Sortie.bmp");
        }

        public void StrengthBord()
        {
            Pixel[,] picture = new Pixel[image.GetLength(0), image.GetLength(1)];
            int[,] mat_conv = new int[,] { { 0, 0, 0 }, { -1, 1, 0 }, { 0, 0, 0 } };

            for (int i = 0; i < image.GetLength(0); i++)
                for (int j = 0; j < image.GetLength(1); j++)
                    picture[i, j] = Convolution(mat_conv, i, j);

            image = picture;
            FromImage_ToFIle(@"Sortie.bmp");
        }

        public void Blur()
        {
            Pixel[,] picture = new Pixel[image.GetLength(0), image.GetLength(1)];
            int[,] mat_conv = new int[,] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };

            for (int i = 0; i < image.GetLength(0); i++)
                for (int j = 0; j < image.GetLength(1); j++)
                    picture[i, j] = ConvolutionNormalisee(mat_conv, i, j);

            image = picture;
            FromImage_ToFIle(@"Sortie.bmp");
        }

        public void Repulse()
        {
            Pixel[,] picture = new Pixel[image.GetLength(0), image.GetLength(1)];
            int[,] mat_conv = new int[,] { { -2, -1, 0 }, { -1, 1, 1 }, { 0, 1, 2 } };

            for (int i = 0; i < image.GetLength(0); i++)
                for (int j = 0; j < image.GetLength(1); j++)
                    picture[i, j] = Convolution(mat_conv, i, j);

            image = picture;
            FromImage_ToFIle(@"Sortie.bmp");
        }

        public void StrengthContrast()
        {
            Pixel[,] picture = new Pixel[image.GetLength(0), image.GetLength(1)];
            int[,] mat_conv = new int[,] { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } };

            for (int i = 0; i < image.GetLength(0); i++)
                for (int j = 0; j < image.GetLength(1); j++)
                    picture[i, j] = Convolution(mat_conv, i, j);

            image = picture;
            FromImage_ToFIle(@"Sortie.bmp");
        }

        public void ToGrey()
        {
            for (int i = 0; i < hauteur; i++)
                for (int j = 0; j < largeur; j++)
                    image[i, j] = new Pixel(image[i, j].Moyenne(), image[i, j].Moyenne(), image[i, j].Moyenne());

            FromImage_ToFIle("sortie.bmp");
            FromImage_ToFIle("sortie2.bmp");
        }

        public void ToBlackAndWhite()
        {
            for (int i = 0; i < hauteur; i++)
                for (int j = 0; j < largeur; j++)
                    image[i, j] = new Pixel(image[i, j].Moyenne() > 127 ? 255 : 0, image[i, j].Moyenne() > 127 ? 255 : 0, image[i, j].Moyenne() > 127 ? 255 : 0);

            FromImage_ToFIle("sortie.bmp");
        }

        public void ToInverse()
        {
            Pixel[,] picture = new Pixel[hauteur, largeur];

            for (int i = 0; i < hauteur; i++)
                for (int j = 0; j < largeur; j++)
                {
                    picture[i, j] = new Pixel(255 - image[i, j].R, 255 - image[i, j].G, 255 - image[i, j].B);
                }

            ChangerProprietees(picture);
            FromImage_ToFIle("sortie.bmp");
        }

        public void ToDrawing(MyImage picture2)
        {
            Pixel[,] picture = new Pixel[hauteur, largeur];

            for (int i = 0; i < hauteur; i++)
                for (int j = 0; j < largeur; j++)
                {
                    int r = image[i, j].R * 255 / ((255 - picture2.image[i, j].R) + 1);
                    if (r > 255) r = 255;
                    int g = (image[i, j].G * 255) / ((255 - picture2.image[i, j].G) + 1);
                    if (g > 255) g = 255;
                    int b = (image[i, j].B * 255) / ((255 - picture2.image[i, j].B) + 1);
                    if (b > 255) b = 255;

                    picture[i, j] = new Pixel(r, g, b);
                }

            ChangerProprietees(picture);
            FromImage_ToFIle("sortie.bmp");
        }
        #endregion filtre

        #region Histogramme
        /// <summary>
        /// Création des tableaux qui compte les nombres de couleurs Rouge,Vert et Bleu
        /// on parcourt la matrice en incrémentant les tableaux R,G et B pour éviter les problèmes d'affichage pour l'histogramme
        /// On initialise les valeurs maximales de R,G et B
        /// On créé une image de largeur 255*3 car on a 3 valeurs (R,G et B) et 255*4 en longueur pour avoir une dimesion de plus en longueur
        /// On initialise les valeurs de R,G et B à 0 pour l'image picture
        /// ensuite on parcours la matrice en incrémentant par 3 car on va les séparer en 3 (on évite de passer sur sur le même pixel plusieurs fois
        /// On divise l'index par 3 pour qu'il passe par tout les pixels
        /// On regarde la nuance de R,G et B  en effectuant un ratio entre le nombre de R,G et B dans ma matrice par rapport au max de R,G et Bs
        /// </summary>
        public void Histogramme()
        {
            int[] NombreR = new int[256];
            int[] NombreG = new int[256];
            int[] NombreB = new int[256];
            for (int k = 0; k < image.GetLength(0); k++)
            {
                for (int l = 0; l < image.GetLength(1); l++)
                {
                    NombreR[Convert.ToInt32(image[k, l].R)]++;
                    NombreG[Convert.ToInt32(image[k, l].G)]++;
                    NombreB[Convert.ToInt32(image[k, l].B)]++;
                }
            }
            int ValeurMaxR = NombreR.Max();
            int ValeurMaxG = NombreG.Max();
            int ValeurMaxB = NombreB.Max();
            Pixel[,] picture = new Pixel[255 * 3, 255 * 4];
            for (int k = 0; k < picture.GetLength(0); k++)
            {
                for (int l = 0; l < picture.GetLength(1); l++)
                {
                    picture[k, l] = new Pixel(0, 0, 0);
                }
            }
            for (int k = 0; k < picture.GetLength(0) - 1; k += 3)
            {
                for (int l = 0; l < 3; l++)
                {
                    int nuanceR = 1000 * NombreR[k / 3] / ValeurMaxR;
                    int nuanceG = 1000 * NombreG[k / 3] / ValeurMaxG;
                    int nuanceB = 1000 * NombreB[k / 3] / ValeurMaxB;
                    for (int m = 0; m < nuanceR; m++)
                    {
                        picture[k + l, m].R = 255;
                    }
                    for (int m = 0; m < nuanceG; m++)
                    {
                        picture[k + l, m].G = 255;
                    }
                    for (int m = 0; m < nuanceB; m++)
                    {
                        picture[k + l, m].B = 255;
                    }
                }
            }
            ChangerProprietees(picture);
            FromImage_ToFIle(@"histogramme.bmp");
        }
        #endregion Histogramme

        #region Fractale
        public void Mandelbrot()
        {
            Pixel[,] picture = new Pixel[1000, 1000];
            ChangerProprietees(picture);
            for (int x = 0; x < largeur; x++)
            {
                for (int y = 0; y < hauteur; y++)
                {
                    double a = (double)(x - (largeur / 2)) / (double)(largeur / 4);
                    double b = (double)(y - (hauteur / 2)) / (double)(hauteur / 4);
                    Complex c = new Complex(a, b);
                    Complex z = new Complex(0, 0);

                    int i = 0;
                    while (i < 2000)
                    {
                        i++;
                        z.JCarre();
                        z.JAddtion(c);
                        if (z.JModule() > 2) break;
                    }

                    if (i < 2000) image[x, y] = new Pixel(0, 0, 0);
                    else image[x, y] = new Pixel(0, 0, 255);
                }
            }

            FromImage_ToFIle("fractale.bmp");
        }
        #endregion Fractale

		#region Stéganographie
		/// <summary>
		/// On parcourt la totalité de la matrice dans laquelle on va cacher l'autre image
		/// quand les indexs sont plus grands que les dimensions de l'image caché on les valeurs de R,G et B de l'image caché = 0
		/// à chaque on relève les valeurs R,G et B pour les 2 images que l'on converti en string puis en tableau de int
		/// on prend les 4 premiers bit de R,G et B pour le pixel étudié de chaque image
		/// on créé une nouvelle valeur de R,G et B pour notre stéganograophie que l'on repasse en valeur entre 0 et 255
		/// les 4 premiers bit de la stéganographie sont les 4 premiers bit du pixel de l'image dans laquelle on cache
		/// les 4 derniers bit de la stéganographie sont les 4 premiers bit du pixel de l'image caché
		/// </summary>
		/// <param name="imagecaché"></param> correspond à l'image que l'on souhaite caché
		public void Stéganographie(Pixel[,] imagecaché)

        {

            int[] newValeurR = new int[8];

            int[] newValeurG = new int[8];

            int[] newValeurB = new int[8];

            Pixel[,] picture = new Pixel[image.GetLength(0), image.GetLength(1)];

            for (int k = 0; k < image.GetLength(0); k++)

            {
                for (int l = 0; l < image.GetLength(1); l++)
                {
                    string Rbinaire2, Gbinaire2, Bbinaire2;
                    string Rbinaire = Convert.ToString(image[k, l].R, 2).PadLeft(8, '0');

                    string Gbinaire = Convert.ToString(image[k, l].G, 2).PadLeft(8, '0');

                    string Bbinaire = Convert.ToString(image[k, l].B, 2).PadLeft(8, '0');
                    if (k < imagecaché.GetLength(0) && l < imagecaché.GetLength(1))
                    {
                        Rbinaire2 = Convert.ToString(imagecaché[k, l].R, 2).PadLeft(8, '0');

                        Gbinaire2 = Convert.ToString(imagecaché[k, l].G, 2).PadLeft(8, '0');

                        Bbinaire2 = Convert.ToString(imagecaché[k, l].B, 2).PadLeft(8, '0');
                    }
                    else
                    {
                        Rbinaire2 = "00000000";

                        Gbinaire2 = "00000000";

                        Bbinaire2 = "00000000";
                    }

                    for (int i = 0; i < 4; i++)

                    {

                        newValeurR[i] = Convert.ToInt32(Rbinaire[i]);

                        newValeurG[i] = Convert.ToInt32(Gbinaire[i]);

                        newValeurB[i] = Convert.ToInt32(Bbinaire[i]);

                    }

                    for (int i = 0; i < 4; i++)

                    {

                        newValeurR[i + 4] = Convert.ToInt32(Rbinaire2[i]);

                        newValeurG[i + 4] = Convert.ToInt32(Gbinaire2[i]);

                        newValeurB[i + 4] = Convert.ToInt32(Bbinaire2[i]);

                    }

                    int valeurR = 0;

                    int valeurG = 0;

                    int valeurB = 0;

                    double a = 7;

                    for (int i = 0; i < 8; i++)
                    {
                        valeurR = Convert.ToInt32(valeurR + newValeurR[i] * Math.Pow(2, a));

                        valeurG = Convert.ToInt32(valeurG + newValeurG[i] * Math.Pow(2, a));

                        valeurB = Convert.ToInt32(valeurB + newValeurB[i] * Math.Pow(2, a));

                        a--;
                    }
                    picture[k, l] = new Pixel(valeurR, valeurG, valeurB);
                }
            }

            ChangerProprietees(picture);

            FromImage_ToFIle(@"sortie.bmp");

        }

        #endregion Stéganographie

        #region Décoder Stéganographie
		/// <summary>
		/// On récupere les valeurs de RGB de l'image stéganographié
		/// ON parcourt tte l'image on prend à chaque fois les 4 derniers bit de RGB pour chaque pixel
		/// ainsi les valeurs de RGB des pixels l'image caché correpondent aux 4 derniers bit RGB des pixels de l'image stéganographié suivi de 4 zéros
		/// On converti ce résultat en valeur entre 0 et 255 et on obtient l'image
		/// </summary>
        public void Décoder_Stéganographie2()

        {

            int[] newValeurR2 = new int[8];

            int[] newValeurG2 = new int[8];

            int[] newValeurB2 = new int[8];

            Pixel[,] picture2 = new Pixel[image.GetLength(0), image.GetLength(1)];

            for (int k = 0; k < image.GetLength(0); k++)

            {

                for (int l = 0; l < image.GetLength(1); l++)

                {

                    string Rbinaire = Convert.ToString(image[k, l].R, 2).PadLeft(8, '0');

                    string Gbinaire = Convert.ToString(image[k, l].G, 2).PadLeft(8, '0');

                    string Bbinaire = Convert.ToString(image[k, l].B, 2).PadLeft(8, '0');

                    for (int i = 0; i < 4; i++)

                    {

                        newValeurR2[i] = Convert.ToInt32(Rbinaire[i + 4]);

                        newValeurG2[i] = Convert.ToInt32(Gbinaire[i + 4]);

                        newValeurB2[i] = Convert.ToInt32(Bbinaire[i + 4]);

                    }

                    for (int i = 4; i < 8; i++)

                    {

                        newValeurR2[i] = 0;

                        newValeurG2[i] = 0;

                        newValeurB2[i] = 0;

                    }

                    int valeurR2 = 0;

                    int valeurG2 = 0;

                    int valeurB2 = 0;

                    double a = 7;

                    for (int i = 0; i < 8; i++)

                    {

                        valeurR2 = Convert.ToInt32(valeurR2 + newValeurR2[i] * Math.Pow(2, a));

                        valeurG2 = Convert.ToInt32(valeurG2 + newValeurG2[i] * Math.Pow(2, a));

                        valeurB2 = Convert.ToInt32(valeurB2 + newValeurB2[i] * Math.Pow(2, a));

                        a--;

                    }

                    picture2[k, l] = new Pixel(valeurR2, valeurG2, valeurB2);

                }

            }

            taille -= image.GetLength(0) * image.GetLength(1);

            largeur = picture2.GetLength(1);

            hauteur = picture2.GetLength(0);

            taille += hauteur * largeur * 9;

            image = picture2;

            FromImage_ToFIle(@"sortie.bmp");

        }

        #endregion Décoder Stéganographie
    }
}
        