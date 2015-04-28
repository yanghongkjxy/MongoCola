﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Common.Logic;
using Common.UI;
using MongoDB.Bson;
using MongoUtility.Core;
using ResourceLib.Utility;

namespace MongoCola
{
    public partial class frmEvalJS : Form
    {
        public frmEvalJS()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmevalJS_Load(object sender, EventArgs e)
        {
            if (!GUIConfig.IsUseDefaultLanguage)
            {
                Text = GUIConfig.GetText(TextType.EvalJS_Title);
                ctlEval.Title = GUIConfig.GetText(TextType.EvalJS_Method);
                lblParm.Text = GUIConfig.GetText(TextType.EvalJS_Parameter);
                cmdEval.Text = GUIConfig.GetText(TextType.EvalJS_Run);
            }
            ctlEval.Context = "function eval(){" + Environment.NewLine;
            ctlEval.Context += "    var i = 0;" + Environment.NewLine;
            ctlEval.Context += "    i++;" + Environment.NewLine;
            ctlEval.Context += "    return i;" + Environment.NewLine;
            ctlEval.Context += "}" + Environment.NewLine;
        }

        /// <summary>
        ///     eval Javascript
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdEval_Click(object sender, EventArgs e)
        {
            var mongoDB = RuntimeMongoDBContext.GetCurrentDataBase();
            var js = new BsonJavaScript(ctlEval.Context);
            var Params = new List<Object>();
            if (txtParm.Text != string.Empty)
            {
                foreach (var parm in txtParm.Text.Split(",".ToCharArray()))
                {
                    if (parm.StartsWith("'") & parm.EndsWith("'"))
                    {
                        Params.Add(parm.Trim("'".ToCharArray()));
                    }
                    else
                    {
                        try
                        {
                            var isNuberic = true;
                            for (var i = 0; i < parm.Length; i++)
                            {
                                if (!char.IsNumber(parm, i))
                                {
                                    isNuberic = false;
                                }
                            }
                            if (isNuberic)
                            {
                                Params.Add(Convert.ToInt16(parm));
                            }
                        }
                        catch (Exception ex)
                        {
                            Utility.ExceptionDeal(ex, "Exception", "Parameter Exception");
                        }
                    }
                }
            }
            try
            {
                var result = mongoDB.Eval(js, Params.ToArray());
                MyMessageBox.ShowMessage("Result", "Result",
                    result.ToJson(MongoUtility.Basic.Utility.JsonWriterSettings), true);
            }
            catch (Exception ex)
            {
                Utility.ExceptionDeal(ex, "Exception", "Result");
            }
        }
    }
}