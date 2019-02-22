using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Globalization;
using Teigha.Runtime;
using Teigha.Geometry;
using Teigha.DatabaseServices;

namespace CAS.myCAD
{
    public partial class MyUtilities
    {
        public ErrorStatus ConvertToDouble(string String, ref double dZahl, int? Zähler)
        {
            ErrorStatus eStatus = ErrorStatus.OutOfRange;

            if (!(String == String.Empty || String == "\r"))
            {
                string sZahl = "";

                //',' gegen '.' tauschen
                String = String.Replace(',', '.');

                //Vorzeichen berücksichtigen
                try
                {
                    if (String[0] == '-')
                        sZahl = "-";
                }

                catch { }

                //alphazeichen aus String entfernen
                for (int i = 0; i < String.Length; i++)
                {
                    if ((String[i] >= '0') && (String[i] <= '9')
                          || String[i] == '.'
                          || String[i] == ',')

                        sZahl += String[i];
                }

                int test = String.Length - sZahl.Length;

                try
                {
                    if (sZahl != String.Empty)
                    {
                        dZahl = Convert.ToDouble(sZahl, CultureInfo.InvariantCulture);
                        eStatus = ErrorStatus.OK;
                    }
                }

#pragma warning disable CS0168 // Die Variable "e" ist deklariert, wird aber nie verwendet.
                catch (System.FormatException e)
#pragma warning restore CS0168 // Die Variable "e" ist deklariert, wird aber nie verwendet.
                {
                    System.Windows.Forms.MessageBox.Show(sZahl + " konnte nicht konvertiert werden. (Zeile " + Zähler.ToString() + ")");
                }
            }

            //MP ohne Höhe
            else
            {
                eStatus = ErrorStatus.NullExtents;
            }

            return eStatus;
        }

        /// <summary>
        /// Anzahl der gültigen Nachkommastellen ermitteln
        /// </summary>
        /// <param name="Wert"></param>
        /// <returns></returns>
        //public int Precision(string Wert)
        //{
        //    try
        //    {
        //        Wert = Wert.Substring(Wert.LastIndexOf('.'));
        //    }
        //    catch { }

        //    if (Wert[0] == '.')
        //        Wert = Wert.Substring(1);

        //    return Wert.Length;
        //}

        ///<summary>
        ///Richtungswinkel
        ///</summary>
        public double RiWi(Point2d pt1, Point2d pt2)
        {
            double dRiWi = 0;
            double dx = pt2.X - pt1.X;
            double dy = pt2.Y - pt1.Y;
            double dPhi = 0;

            if (dx != 0)
            {
                dPhi = Math.Abs(Math.Atan(dx / dy));

                // 1. Quadrant
                if ((dx >= 0) && (dy >= 0))
                    dRiWi = dPhi;

                //2. Quadrant
                if ((dx >= 0) && (dy <= 0))
                    dRiWi = Math.PI - dPhi;

                //3. Quadrant
                if ((dx <= 0) && (dy <= 0))
                    dRiWi = dPhi + Math.PI;


                //4. Quadrant
                if ((dx <= 0) && (dy >= 0))
                    dRiWi = -dPhi;

            }
            else
                if (dy >= 0)
                dPhi = 0;
            else
                dPhi = Math.PI;



            return dRiWi;
        }

        ///<summary>
        ///Berechnung der Sehnenlänge
        ///</summary>
        public double CalcSehne(double dRadius, double dStich)
        {
            double dSehnenlänge = 2 * Math.Sqrt((dRadius * dRadius) - (dRadius - dStich) * (dRadius - dStich));

            return dSehnenlänge;
        }

        ///<summary>
        ///Bogenkleinpunkte
        ///</summary>
        public List<Point3d> CalcBogenKleinpunkte3d(Point2d ptZentrum, double dRadius, Point3d ptAnfang, Point3d ptEnde, double dStich)
        {
            List<Point3d> lsPunkte = new List<Point3d>();
            Point2d ptAnfang2d = new Point2d(ptAnfang.X, ptAnfang.Y);
            Point2d ptEnde2d = new Point2d(ptEnde.X, ptEnde.Y);

            //erforderliche Sehnenlänge berechnen
            myCAD.MyUtilities objUtil = new myCAD.MyUtilities();
            double dSehne = objUtil.CalcSehne(dRadius, dStich);

            //Berechnung Bogenlänge
            double dAbstandSE = ptAnfang2d.GetDistanceTo(ptEnde2d);
            double dPhi = 2 * Math.Asin(dAbstandSE / (2 * dRadius));
            double dBL = dRadius * dPhi;

            //Berechnung der Bogenlänge für Kleinpunkte
            double dPhi1 = 0;

            dPhi1 = 2 * Math.Asin(dSehne / (2 * dRadius));
            double dBL1 = dRadius * dPhi1;

            //Vorzeichen für Phi1 festlegen (je nach Drehsinn)
            double dAlphaStart = objUtil.RiWi(ptZentrum, ptAnfang2d);
            double dAlphaEnde = objUtil.RiWi(ptZentrum, ptEnde2d);

            if (dAlphaStart > dAlphaEnde)
                dPhi1 = -dPhi1;

            //Richtungswinkel
            Vector2d v2dRiWi = ptZentrum.GetVectorTo(ptAnfang2d);
            double dRiWi = v2dRiWi.Angle;
            dRiWi = objUtil.RiWi(ptZentrum, ptAnfang2d);

            //solange Endpunkt nicht erreicht ist, Kleinpunkte berechnen
            double dStation = dBL1;
            double dWinkel = dRiWi;

            //dH für Höheninterpolation
            double dH = (ptEnde.Z - ptAnfang.Z) / dBL * dBL1;
            double dz = 0;

            while (dStation < dBL)
            {
                dWinkel += dPhi1;

                //dx, dy und dz berechnen
                double dx = dRadius * Math.Sin(dWinkel);
                double dy = dRadius * Math.Cos(dWinkel);
                dz += dH;

                //Kleinpunkt berechnen
                Point3d ptPunkt = new Point3d(ptZentrum.X + dx, ptZentrum.Y + dy, ptAnfang.Z + dz);

                //Punkt zu Liste hinzufügen
                lsPunkte.Add(ptPunkt);

                dStation += dBL1;
            }

            return lsPunkte;
        }

        public ErrorStatus GetTextStyleId(string Textstil, ref ObjectId TextstilId)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Transaction myT = db.TransactionManager.StartTransaction();

            ErrorStatus es = ErrorStatus.KeyNotFound;

            try
            {
                TextStyleTable tsTbl = (TextStyleTable)myT.GetObject(db.TextStyleTableId, OpenMode.ForRead, true, true);

                foreach (ObjectId objId in tsTbl)
                {
                    TextStyleTableRecord tsTblRec = new TextStyleTableRecord();
                    tsTblRec = (TextStyleTableRecord)myT.GetObject(objId, OpenMode.ForWrite);

                    if (Textstil == tsTblRec.Name)
                    {
                        TextstilId = objId;
                        //m_myT.Commit();
                        break;
                    }
                }

                es = ErrorStatus.OK;
            }

            finally
            {
                myT.Commit();
            }

            return es;
        }

        //icon to imagesource
        //  public static class IconUtilities
        //{
        //    [DllImport("gdi32.dll", SetLastError = true)]

        //    private static extern bool DeleteObject(IntPtr hObject);
        //    public static System.Windows.Media.ImageSource ToImageSource(this System.Drawing.Icon icon)
        //    {
        //        System.Drawing.Bitmap bitmap = icon.ToBitmap();
        //        IntPtr hBitmap = bitmap.GetHbitmap();
        //        ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(hBitmap,
        //                                             IntPtr.Zero, System.Windows.Int32Rect.Empty,
        //                                             BitmapSizeOptions.FromEmptyOptions());
        //        if (!DeleteObject(hBitmap))
        //        { throw new System.ComponentModel.Win32Exception(); }
        //        return wpfBitmap;
        //    }
        //}
    }
}
