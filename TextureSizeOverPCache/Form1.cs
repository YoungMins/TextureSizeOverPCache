using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextureSizeOverPCache
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            OpenFileDialog opendlg = new OpenFileDialog();
            opendlg.Filter = "PNG File|*.png|Bitmap File|*.bmp|JPEG File|*.jpg|TIFF File|*.tif";

            if ( opendlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Image img = Image.FromFile(opendlg.FileName);
                    Size size = img.Size;

                    string pCacheStr = pCacheHeader(size.Width * size.Height);

                    using (Bitmap bitmap = new Bitmap(img))
                    {
                        for( int j=0; j< size.Height; j++)
                        {
                            for( int i=0; i<size.Width; i++)
                            {
                                float x = Remap(i, 0, size.Width, -0.5f, 0.5f);
                                float y = Remap(j, 0, size.Height, -0.5f, 0.5f);
                                float z = 0;

                                Color col = bitmap.GetPixel(i, j);
                                float r = Remap(col.R, 0, 255, 0, 1);
                                float g = Remap(col.G, 0, 255, 0, 1);
                                float b = Remap(col.B, 0, 255, 0, 1);
                                float a = Remap(col.A, 0, 255, 0, 1);
                                pCacheStr += 
                                    (x.ToString() + " " +
                                     y.ToString() + " " +
                                     z.ToString() + " " +
                                     r.ToString() + " " +
                                     g.ToString() + " " +
                                     b.ToString() + " " +
                                     a.ToString() + "\n");
                            }
                        }                        
                    }
                    SaveFileDialog saveDlg = new SaveFileDialog();
                    saveDlg.Filter = "point cache file | *.pCache";
                    if (saveDlg.ShowDialog() == DialogResult.OK)
                    {
                        Encoding utf8WithoutBom = new UTF8Encoding(false);
                        File.WriteAllText(saveDlg.FileName, pCacheStr, utf8WithoutBom);
                        MessageBox.Show("Success");
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private string pCacheHeader(int count)
        {
            string str = string.Empty;
            str = "pcache\n" +
            "format ascii 1.0\n" +
            "comment Make by YOUNGMIN KIM\n" +
            "elements " + count.ToString() + "\n" +
            "property float position.x\n" +
            "property float position.y\n" +
            "property float position.z\n" +
            "property float color.r\n" +
            "property float color.g\n" +
            "property float color.b\n" +
            "property float color.a\n" +
            "end_header\n";

            return str;
        }

        public float Remap(float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
    }
}
