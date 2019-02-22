using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bricscad.Runtime;
using Bricscad.EditorInput;
using Teigha.Runtime;
using Teigha.DatabaseServices;
using Bricscad.ApplicationServices;

//alias
using _AcAp = Bricscad.ApplicationServices;
using _AcDb = Teigha.DatabaseServices;
using _AcEd = Bricscad.EditorInput;
using _AcGe = Teigha.Geometry;

namespace CAS.myCAD
{
    public partial class Blöcke
    {
        //Methods


        /// <summary>
        /// Blöcke mit Fenster wählen
        /// </summary>
        /// <returns></returns>
        public ErrorStatus SelectWindow()
        {
            ErrorStatus eStatus = ErrorStatus.KeyNotFound;
            SelectionSet ssRes = null;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            //Filter
            TypedValue[] values = new TypedValue[1] {
                new TypedValue((int)DxfCode.Start, "INSERT")};
            SelectionFilter selFilter = new SelectionFilter(values);

            PromptSelectionResult resSel = ed.GetSelection(selFilter);

            if (resSel.Status == PromptStatus.OK)
            {
                ssRes = resSel.Value;
                FillTable(ssRes);
                eStatus = ErrorStatus.OK;
            }

            return eStatus;
        }  

        /// <summary>
        /// Attribute schalten
        /// </summary>
        /// 
        public enum Mode
        {
            HeightOff = 1,
            HeightOn = 2,
        }

        public void SwitchAtt(Mode mode)
        {
            switch(mode)
            {
                case Mode.HeightOn:
                    foreach(Messpunkt MP in ls_MP)
                    {
                        MP.HeightVisible(true);
                    }

                    break;

                case Mode.HeightOff:
                    foreach (Messpunkt MP in ls_MP)
                    {
                        MP.HeightVisible(false);
                    }

                    break;

                default:
                    break;
            }
        }
    }
}