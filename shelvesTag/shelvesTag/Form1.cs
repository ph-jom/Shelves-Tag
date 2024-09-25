using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Drawing.Imaging;
using ZXing;
using ZXing.QrCode.Internal;
using static System.Windows.Forms.AxHost;
using System.IO;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using static System.Net.WebRequestMethods;
using System.Security.Cryptography;
using System.Drawing.Text;

namespace shelvesTag
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(@"C:\Program Files\RCS\shelvesTag\shelvesTag\ex_template.xml");

            XmlNode logo = xml.SelectSingleNode($"/template/layout/graphic[1]");
            XmlNode currency = xml.SelectSingleNode($"/template/layout/graphic[2]");
            XmlNode branchCode = xml.SelectSingleNode($"/template/layout/text[1]");
            XmlNode description = xml.SelectSingleNode($"/template/layout/text[2]");
            XmlNode price = xml.SelectSingleNode($"/template/layout/text[3]");
            XmlNode priceDecimal = xml.SelectSingleNode($"/template/layout/text[4]");
            XmlNode uom = xml.SelectSingleNode($"/template/layout/text[5]");
            XmlNode date = xml.SelectSingleNode($"/template/layout/text[6]");
            XmlNode itemCode = xml.SelectSingleNode($"/template/layout/barcode");
            
            string lFormat, lValue, cFormat, cValue, bValue, dValue, pValue, pDValue, uValue, dTValue, iCode;
            
            lFormat = logo.Attributes["format"].Value;
            lValue = logo.InnerText;
            cFormat = currency.Attributes["format"].Value;
            cValue = currency.InnerText;
            bValue = branchCode.InnerText;
            dValue = description.InnerText;
            pValue = price.InnerText;
            pDValue = priceDecimal.InnerText;
            uValue = uom.InnerText;
            dTValue = date.InnerText;
            iCode = itemCode.InnerText;

            // Format the date and time to include seconds
            string dTime = DateTime.Now.ToString("yyyyMMdd");

            gBarcode(iCode);

            //create a new bitmap image
            Bitmap bitmap = new Bitmap(393, 223);
            
            // To create a graphic object to draw on the bitmap
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                // To set the bacground color
                graphics.Clear(Color.White);

                // To draw the RCS LOGO
                Image img = Image.FromFile(@"C:\Program Files\RCS\shelvesTag\shelvesTag\" + lValue + "." + lFormat);
                                       //x, y, width, height
                graphics.DrawImage(img, 20, 20, 30, 20);

                using (Brush brush = Brushes.Black)
                    {
                        graphics.DrawString(dTValue, new Font("Akrobat ExtraBold",8), brush, new PointF(260, 5));
                        graphics.DrawString(bValue, new Font("Akrobat ExtraBold", 15), brush, new PointF(55, 19));
                        graphics.DrawString(cValue, new Font("Ariel", 22), brush, new PointF(135, 12));
                        Image generatedBarcode = Image.FromFile(@"C:\Program Files\RCS\shelvesTag\shelvesTag\barcode\" + iCode+".png");
                        graphics.DrawImage(generatedBarcode, 20,55,150,140);
                     
                        GraphicsState state = graphics.Save();
                        try 
                        {
                            graphics.ScaleTransform(0.77f, 1f);
                            graphics.DrawString(dValue, new Font("Akrobat ExtraBold", 16), brush, new PointF(20, 190));
                        }
                        finally
                        {
                            graphics.Restore(state);
                        }
                        graphics.DrawString(pDValue, new Font("Akrobat ExtraBold", 46), brush, new PointF(325, 75));
                        graphics.DrawString(uValue, new Font("Akrobat ExtraBold", 18), brush, new PointF(344, 150));
                        graphics.ScaleTransform(1f, 2f);
                        graphics.DrawString(pValue, new Font("Akrobat ExtraBold", 62, FontStyle.Regular), brush, new PointF(175, 3));
                    }
            }
            string path = Path.Combine(@"C:\Program Files\RCS\shelvesTag\shelvesTag", "generated");
            string wholepath = Path.Combine(path, $"{dTime}");

            Directory.CreateDirectory(path);
            if (Directory.Exists(path)) {
                Directory.CreateDirectory(wholepath);
            }
            string filepath = Path.Combine(wholepath, iCode+".jpg");

            Directory.CreateDirectory(@"C:\Program Files\RCS\shelvesTag\shelvesTag\generated\"+dTime);
            
            // To Save the made bitmap
            bitmap.Save(filepath);

            txtBox.Text = "Search XML File";

            if (!String.IsNullOrEmpty(filepath))
            {
                txtBoxAddLine($"Success generate {iCode}");
            }
            else {
            }
        }

        private void txtBoxAddLine(string txt) {
            txtBox.Text += $"\r\n{txt}";
        }
        static void gBarcode(string iCode)
{
            BarcodeWriter bWriter = new BarcodeWriter() { Format = BarcodeFormat.EAN_13 };
            bWriter.Options = new ZXing.Common.EncodingOptions{Width = 125, Height = 120};
            using (Bitmap bitmap = bWriter.Write(iCode)) 
            {
                bitmap.Save(@"C:\Program Files\RCS\shelvesTag\shelvesTag\barcode\" + iCode+".png", System.Drawing.Imaging.ImageFormat.Png);     
            }
        }
    }
}

