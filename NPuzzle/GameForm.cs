/* 
 * NPuzzle
 * Revision History
 *      Almas Khan: 2016.10.25: Created
 *      Almas Khan: 2016.10.28: coded
 *      Almas Khan: 2016.10.28: Debugged
 *      
 */


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace NPuzzle
{
    /// <summary>
    /// A class that shows NPuzzle Game
    /// </summary>
    public partial class GameForm : Form
    {
        //Constants declaration
        const int tileHeight = 50;
        const int tileWidth = 50;
        //Global variables
        int rows;
        int columns;
        int counter = 0;
        int arraySize;
        int[,] tiles;
        Tile free;
        int startX = 50;
        int startY = 50;
        int[] random;
        Tile tile;

        /// <summary>
        /// The constructor of the class
        /// </summary>

        public GameForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Event handler will generate the puzzle according to the input of rows and columns
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            //Method called to clear the panel remove all buttons and set counter to zero
            clearPanel();
            try
            {
                rows = Convert.ToInt32(txtRows.Text);
                columns = Convert.ToInt32(txtColumns.Text);
                if (rows.ToString() == "" && columns.ToString() == "")
                {
                    throw new Exception("Please enter row and column value to generate game");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " +ex.Message);
            }
            tiles = new int[rows, columns];
            arraySize = tiles.Length;
            random = new int[arraySize];
            //Method called to generate random numbers
            random = generateUniqueRandom(1, arraySize);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    tile = new Tile();
                    tile.Left = startX;
                    tile.Top = startY;
                    tile.Height = tileHeight;
                    tile.Width = tileWidth;
                    tile.Font = new Font("Georgia", 16);
                    tile.BackColor = Color.White;
                    if (random[counter] == arraySize)
                    {
                        tile.Visible = false;
                        tiles[i, j] = 0;
                        tile.Text = 0.ToString();
                        free = tile;
                    }
                   else
                   {
                        tile.Text = random[counter].ToString();
                        tiles[i, j] = random[counter];
                    }
                    displayPanel.Controls.Add(tile);
                    tile.Click += new EventHandler(tileClickEvent);

                    startX += tileWidth;
                    counter++;
                }
                startX = tileWidth;
                startY += tileHeight;
            }
        }

        /// <summary>
        /// Event handler called when tile is clicked, check its text if text is something then other methods will be called
        /// to swap clicked til with the free tile and will check game end
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tileClickEvent(object sender, EventArgs e)
        {
            Tile clickButton = sender as Tile;
            if (clickButton.Text != "")
            {
                Tile tile = (Tile)sender;
                swapTiles(tile);
                checkGameEnd();
            }
        }
        /// <summary>
        /// Method is called to check game end condition. Current positions of tiles will compare with the exact positions
        /// If it matches then game ends and panel will be cleared also textboxes will be cleared
        /// </summary>
        private void checkGameEnd()
        {
            string correctPositions = "";
            string currentPositions = "";
            int value = 1;
            int[,] correctArray = new int[rows,columns];
            arraySize = rows * columns;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    currentPositions += tiles[i, j];
                    if (value == arraySize)
                    {
                        correctArray[i, j] = 0;
                        correctPositions += correctArray[i, j];
                    }
                    else
                    {
                        correctArray[i, j] = value;
                        correctPositions += correctArray[i, j];
                    }
                    value++;
                }
            }
            
            if (currentPositions == correctPositions)
            {
                MessageBox.Show("Game Ends!!");
                clearPanel();
                txtColumns.Text = "";
                txtRows.Text = "";
            }
          }

        /// <summary>
        /// Method will swap the positions of clicked tile with the free tile
        /// </summary>
        /// <param name="tile">Selected tile</param>
        private void swapTiles(Tile tile)
        {
            const int startIndex = 2;
            int tempLeft;
            int tempTop;
            string i;
            string j;
            int tileRow;
            int tileColumn;
            int freeRow;
            int freeColumn;

            if ((tile.Left == free.Left - tileWidth || tile.Left == free.Left + tileWidth)
                && (tile.Top == free.Top)
                || (tile.Top == free.Top - tileHeight || tile.Top == free.Top + tileHeight)
                && (tile.Left == free.Left))
            {
                tempLeft = tile.Left;
                tempTop = tile.Top;
                tile.Left = free.Left;
                tile.Top = free.Top;
                free.Left = tempLeft;
                free.Top = tempTop;

                i = indexGetter(Convert.ToInt32(tile.Text));
                tileRow = Convert.ToInt32(i.Substring(0, 1));
                tileColumn = Convert.ToInt32(i.Substring(startIndex, 1));
                j = indexGetter(Convert.ToInt32(free.Text));
                freeRow = Convert.ToInt32(j.Substring(0, 1));
                freeColumn = Convert.ToInt32(j.Substring(startIndex, 1));
                positionSwap(ref tiles[tileRow, tileColumn], ref tiles[freeRow, freeColumn]);
            }
        }
        /// <summary>
        /// Methos will update the array according to swapped tiles
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void positionSwap(ref int x, ref int y)
        {
            int tempVar = x;
            x = y;
            y = tempVar;
        }

        /// <summary>
        /// Method will find index of the clicked tile
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private string indexGetter(int x)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                { 
                    if (tiles[i,j] == x)
                    {
                        return i+","+j;
                    }
                }
            }
            return "empty";
        }

        /// <summary>
        /// Method will generate unique random numbers to assign the puzzles with those generated numbers
        /// </summary>
        /// <param name="min">Min number that is 1</param>
        /// <param name="max">Max number that is size of the array</param>
        /// <returns></returns>
        private int[] generateUniqueRandom(int min, int max)
        {
            int n = max - min + 1;
            int[] result = new int[n];
            List<int> tiles = new List<int>();

            for (int i = min; i <= max; i++)
            {
                tiles.Add(i);
            }
            Random r = new Random();
            int j = 0;
            while (tiles.Count > 0)
            {
                int index = r.Next(tiles.Count); 
                result[j++] = tiles[index];
                tiles.RemoveAt(index);
            }
            return result;
        }

        /// <summary>
        /// Method will remove all the controls from the panel and set counter to zero
        /// </summary>
        private void clearPanel()
        {
            counter = 0;
            startX = tileHeight;
            startY = tileWidth;
            displayPanel.Controls.Clear();
        }

        /// <summary>
        /// Method will be called to read all the numbers from the loaded file and will generate tiles according to that
        /// </summary>
        /// <param name="filename">Name of the file</param>
        private void doLoad(string filename)
        {
            int value;
            clearPanel();
            StreamReader reader = new StreamReader(filename);
            rows = Convert.ToInt32(reader.ReadLine());
            columns = Convert.ToInt32(reader.ReadLine());
            txtRows.Text = rows.ToString();
            txtColumns.Text = columns.ToString();
            tiles = new int[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    tile = new Tile();
                    tile.Left = startX;
                    tile.Top = startY;
                    tile.Height = tileHeight;
                    tile.Width = tileWidth;
                    tile.Font = new Font("Georgia", 16);
                    tile.BackColor = Color.White;
                    value = Convert.ToInt32(reader.ReadLine());
                    if (value == 0)
                    {
                        tile.Visible = false;
                        tiles[i, j] = 0;
                        tile.Text = 0.ToString();
                        free = tile;
                    }
                    else
                    {
                        tile.Text = value.ToString();
                        tiles[i, j] = value;
                    }
                    displayPanel.Controls.Add(tile);
                    tile.Click += new EventHandler(tileClickEvent);
                    startX += tileHeight;
                    counter++;
                }
                startX = tileWidth;
                startY += tileWidth;
            }
            reader.Close();          
        }
        /// <summary>
        /// Event handler will be called to get exit fron the game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Method will write all values from the tiles array to the file
        /// </summary>
        /// <param name="filename">Name of the file</param>
        /// <param name="rowValue">Number of rows</param>
        /// <param name="colValue">Number of columns</param>
        private void doSave(string filename, int rowValue, int colValue)
        {

            StreamWriter writer = new StreamWriter(filename);
            writer.WriteLine(rowValue);
            writer.WriteLine(colValue);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    writer.WriteLine(tiles[i, j].ToString());
                }
            }
            writer.Close();
        }
        /// <summary>
        /// Method will save the current positions of the tiles based on their text in the text file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int rowValue = Convert.ToInt32(txtRows.Text);
            int colValue = Convert.ToInt32(txtColumns.Text);
            DialogResult result = dlgSave.ShowDialog();
            dlgSave.InitialDirectory = @"C:\";
            switch (result)
            {
                case DialogResult.Abort:
                    break;
                case DialogResult.Cancel:
                    break;
                case DialogResult.Ignore:
                    break;
                case DialogResult.No:
                    break;
                case DialogResult.None:
                    break;
                case DialogResult.OK:
                    try
                    {
                        string filename = dlgSave.FileName;
                        doSave(filename, rowValue,colValue);
                        MessageBox.Show("Your file is saved");
                    }
                    catch (Exception)
                    {

                        MessageBox.Show("Error in file save");
                    }

                    break;
                case DialogResult.Retry:
                    break;
                case DialogResult.Yes:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Method will load the selected file and call doLoad method to make tiles
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = dlgOpen.ShowDialog();
            dlgOpen.InitialDirectory = @"C:\";
            switch (result)
            {
                case DialogResult.Abort:
                    break;
                case DialogResult.Cancel:
                    break;
                case DialogResult.Ignore:
                    break;
                case DialogResult.No:
                    break;
                case DialogResult.None:
                    break;
                case DialogResult.OK:
                    try
                    {
                        string filename = dlgOpen.FileName;
                        doLoad(filename);
                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show("Error in file load"+ ex.Message);
                    }
                    break;
                case DialogResult.Retry:
                    break;
                case DialogResult.Yes:
                    break;
                default:
                    break;
            }
        }
    }
}

    

