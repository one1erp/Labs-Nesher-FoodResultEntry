using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Common;
using DAL;
using LSExtensionWindowLib;
using LSSERVICEPROVIDERLib;
using MSXML;
using One1.Controls;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using XmlService;

namespace FoodResultEntry
{
    //14369013

    [ComVisible(true)]
    [ProgId("FoodResultEntry.FoodResultEntry")]
    public partial class FoodResultEntry : UserControl, IExtensionWindow
    {



        #region Ctor

        public FoodResultEntry()
        {
            InitializeComponent();
            BackColor = Color.FromName("Control");
            gridResults.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
        }

        #endregion

        #region private members

        private Result _currentResult;
        private INautilusDBConnection _ntlsCon;
        private IExtensionWindowSite2 _ntlsSite;
        private INautilusProcessXML _processXml;
        private string clientName;

        //     private BindingList<Result> _resultsToSet;
        private BindingList<ResultToAdd> _ResultToAdd;
        private IDataLayer dal;
        //ספי אמר שהערך הזה ייכנס כאשר המשתמש יסמן גדול מערכי מקסימום




        #endregion

        #region Implementation of IExtensionWindow

        public bool CloseQuery()
        {
            if (dal != null)
                dal.Close();
            return true;
        }

        public void Internationalise()
        {
        }

        public void SetSite(object site)
        {
          //  Logger.WriteLogFile("SetSite", false);

            _ntlsSite = (IExtensionWindowSite2)site;
            _ntlsSite.SetWindowInternalName("הזנת תוצאה");
            _ntlsSite.SetWindowRegistryName("הזנת תוצאה");
            _ntlsSite.SetWindowTitle("הזנת תוצאה");
        }

        public bool DEBUG = false;

        public void PreDisplay()
        {

          //  Logger.WriteLogFile("1", false);
            Utils.CreateConstring(_ntlsCon);
          //  Logger.WriteLogFile("12", false);
            if (DEBUG)
            {
              //  Logger.WriteLogFile("mock", false);
                dal = new MockDataLayer();
            }
            else
            {
                dal = new DataLayer();
              //  Logger.WriteLogFile("132", false);

            }
            _ResultToAdd = new BindingList<ResultToAdd>();
            _ResultToAdd.RaiseListChangedEvents = true;
            gridResults.DataSource = _ResultToAdd;
          //  Logger.WriteLogFile("4", false);

            dal.Connect();
          //  Logger.WriteLogFile("5", false);

            InitialMode();
            //בכדי שנאוטילוס יעשה פוקוס צריך להפעיל טיימר
            timerFocus.Start();
          //  Logger.WriteLogFile("6", false);

            //רישום של כל RADIO BUTTON
            IEnumerable<RadRadioButton> radioBtnList = GetRadioBtnList();
            foreach (RadRadioButton radRadioButton in radioBtnList)
            {
                radRadioButton.ToggleStateChanged += radRadioButton_ToggleStateChanged;

            }

            SetDefaultRadioBtn();
        }

        /// <summary>
        /// //השמה של -1 כברירת מחדל
        /// </summary>
        private void SetDefaultRadioBtn()
        {
          //  Logger.WriteLogFile("SetSite", false);

            InitRadioBtnList();

            foreach (RadRadioButton radioButton in GetRadioBtnList())
            {
                if (radioButton.Text == "1-")
                {
                    radioButton.IsChecked = true;
                    return;
                }
            }


        }

        public WindowButtonsType GetButtons()
        {
            return WindowButtonsType.windowButtonsNone;
        }

        public bool SaveData()
        {
            return false; //???
        }

        public void SetServiceProvider(object serviceProvider)
        {
          //  Logger.WriteLogFile("SetServiceProvider", false);

            sp = serviceProvider as NautilusServiceProvider;
            _processXml = Utils.GetXmlProcessor(sp);
            _ntlsCon = Utils.GetNtlsCon(sp);
          //  Logger.WriteLogFile("SetServiceProvider2", false);

        }

        public void SetParameters(string parameters)
        {
            txtResultID.Select();

        }


        public void Setup()
        {
          //  Logger.WriteLogFile("Setup", false);

        }

        public WindowRefreshType DataChange()
        {
            return WindowRefreshType.windowRefreshNone;
        }

        public WindowRefreshType ViewRefresh()
        {
            return WindowRefreshType.windowRefreshNone;
        }

        public void refresh()
        {
        }

        public void SaveSettings(int hKey)
        {
        }

        public void RestoreSettings(int hKey)
        {
        }

        #endregion

        #region Common

        #region  events

        private void WindowExtension_Load(object sender, EventArgs e)
        {
            InitialMode();
        }







        private void btnClose_Click(object sender, EventArgs e)
        {

            if (gridResults.RowCount > 0)
            {
                DialogResult dialogResult = MessageBox.Show("האם אתה בטוח שברצונך לצאת ממסך זה ללא אישור? ", "יציאה",
                    MessageBoxButtons.YesNoCancel);
                if (dialogResult == DialogResult.Yes)
                {

                    _ntlsSite.CloseWindow();
                }
            }
            else
            {

                _ntlsSite.CloseWindow();
            }



        }

        #region New region




        private void BadValue(RadTextBox senderTxtResult, string errorMessage)
        {
            //  EnableButtons(false);
            string message = errorMessage;
            //   lblError.Text = message;
            //   d_lblError.Text = message;
            System.Media.SystemSounds.Beep.Play();
            System.Media.SystemSounds.Beep.Play();
            System.Media.SystemSounds.Beep.Play();
            System.Media.SystemSounds.Beep.Play();
            One1.Controls.CustomMessageBox
                .Show(errorMessage);
            senderTxtResult.Focus();

            senderTxtResult.Text = string.Empty;



        }

        #endregion



        #endregion













        #endregion

        #region Panel without dilution






        #endregion

        #region DilutinPanel


        private NautilusServiceProvider sp;

        private void InitialMode()
        {
            dataGridView1.Rows.Clear();
            txtResultID.Text = string.Empty;
            txtResultID.Enabled = true;
            txtResultID.Focus();
            txtResultValue.Text = string.Empty;
            txtResultValue.Enabled = false;

            d_btnClose.Enabled = true;
            d_lblError.Text = "";





            panelDilution.Enabled = true;
            txtResultID.Select();
            txtResultID.Focus();

        }

        #endregion


        private void SingleResultEntry_Resize(object sender, EventArgs e)
        {
            lblHeader.Location = new Point(panelDilution.Width / 2 - lblHeader.Width / 2, lblHeader.Location.Y);
            panelDilution.Location = new Point(Width / 2 - panelDilution.Width / 2, panelDilution.Location.Y);
        }


        private void timerFocus_Tick(object sender, EventArgs e)
        {
            txtResultID.Focus();
            timerFocus.Stop();

        }


        private void btnSaveResults_Click(object sender, EventArgs e)
        {


            ResultEntry();
        }


        #region key down

        private void txtResultID_KeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                
                var senderTxtResult = (RadTextBox)sender;
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {

                    var value = senderTxtResult.Text.Trim();
                    int resultId;
                    if ((value.StartsWith("R", StringComparison.OrdinalIgnoreCase) || value.StartsWith("ר"))
                        && int.TryParse(value.Replace(value[0], ' ').Trim(), out resultId))
                    {
                        //    EnableButtons(false);
                        _currentResult = dal.IsGoodResultForEntry(resultId, 'C', 'V', 'U', 'S', 'P');
                        if (_currentResult == null)
                        {
                            BadValue(senderTxtResult, "מספר בדיקה לא קיים, או אינו בסטטוס הנכון.");
                            return;
                        }
                        if (_ResultToAdd.SingleOrDefault(x => x.ResultId == _currentResult.ResultId) != null)
                        {
                            BadValue(senderTxtResult, "תוצאה זו כבר מופיעה בטבלה.");
                            return;
                        }
                        //     Debugger.Launch();
                        try
                        {
                            clientName = _currentResult.Test.Aliquot.Sample.Sdg.Client.Name;
                            lblClientName.Text = clientName;
                            lblDesc.Text = _currentResult.Test.Aliquot.Sample.Description;

                     

                            AddToReportedList();

                        }
                        catch (Exception e1)
                        {
                          //  Logger.WriteLogFile(e1);
                            clientName = "";
                        }


                        senderTxtResult.Enabled = false;

                        lblDuplicates.Text = GetDuplicates().ToString();

                        d_lblError.Text = "";
                        txtResultValue.Enabled = true;
                        txtResultValue.Text = "";
                        txtResultValue.Select();
                        txtResultValue.Focus();

                    }
                    else
                    {
                        BadValue(senderTxtResult, "מספר בדיקה אינו מתאים.");
                    }
                }
            }


            catch (Exception ex)
            {
              //  Logger.WriteLogFile(ex);
            }

        }
        private void AddToReportedList()
        {
            foreach (Aliquot aliquot in _currentResult.Test.Aliquot.Sample.Aliqouts)
            {
                foreach (Test test in aliquot.Tests)
                {
                //הוא אמר שיכול להיות יותר מאחד שמסומן ובמקרה זה להביא את הראשון
                    Result reportedResult = (from result in test.Results 
                                             where result.REPORTED == "T" && result.Status != "X"
                                             orderby result.ResultId ascending
                                             select result).FirstOrDefault();
                    if (reportedResult != null)
                    {
                      dataGridView1.Rows.Add(aliquot.ShortName,reportedResult.FormattedResult);
                     
                    }
                }
            }
        }



        

        private void txtResultValue_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                int temp;
                if (!int.TryParse(txtResultValue.Text, out temp))
                {
                    CustomMessageBox.Show("ערך לא חוקי, אנא הכנס מספר.", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txtResultValue.Text = "";
                    txtResultValue.Focus();
                }
                try
                {

                    Sdg currentSdg = _currentResult.Test.Aliquot.Sample.Sdg;
                    Sample currentSample = _currentResult.Test.Aliquot.Sample;
                    var checkedRadioButton = GetRadioBtnList().Where(x => x.IsChecked).FirstOrDefault();
                    if (checkedRadioButton != null)
                    {
                        var red = new ResultToAdd
                                  {
                                      ResultId = _currentResult.ResultId,
                                      TestId = _currentResult.TestId,
                                      ResultTemplateId = _currentResult.ResultTemplateId,
                                      ClientName = currentSdg.Client.Name,
                                      ResultValue = int.Parse(txtResultValue.Text),
                                      ResultName = _currentResult.Name,
                                      SampleDescription = currentSample.Description,
                                      Description = currentSample.Description,
                                      SdgName = currentSample.Name,
                                      MaxValueIncrease = false,
                                      Dilution = int.Parse(checkedRadioButton.Text[0].ToString()),
                                      Duplicates = GetDuplicates(true)
                                  };
                        _ResultToAdd.Add(red);
                    }


                    d_lblError.Text = "";
                    panelDilution.Enabled = false;

                    //תמיד זה היה קורה שהטיימר נגמר מכיוון שספי אמר להוריד את הטיימר קראתי לפונקציה
                    System.Media.SystemSounds.Beep.Play();
                    InitialMode();

                    d_lblError.Text = "";
                    SetDefaultRadioBtn();


                }
                catch (Exception exception)
                {

                  //  Logger.WriteLogFile(exception);
                    CustomMessageBox.Show("לא ניתן להזין את התוצאה המבוקשת - אנא פנה לתמיכה.");
                }
                finally
                {
                }



            }

        }

        #endregion

        #region private methods

        private void ResultEntry()
        {

            var objDom = new DOMDocument();
            IXMLDOMElement objResultRequest = objDom.createElement("lims-request");
            objDom.appendChild(objResultRequest);
            foreach (var resultToAdd in _ResultToAdd)
            {
                dal.UpdateDilution(resultToAdd.Dilution, resultToAdd.ResultId);

                IXMLDOMElement objResultRequest2 = objDom.createElement("result-request");
                //gets entity name
                string resultId = resultToAdd.ResultId.ToString();
                string testId = resultToAdd.TestId.ToString();
                var resultValue = resultToAdd.CalculateValueToSet();

                IXMLDOMElement objLoad = objDom.createElement("load");
                objLoad.setAttribute("entity", "TEST");

                objLoad.setAttribute("id", testId);
                objLoad.setAttribute("mode", "entry");

                objResultRequest2.appendChild(objLoad);
                objResultRequest.appendChild(objResultRequest2);

                IXMLDOMElement objResultEntryElem = ObjResultEntryElem(objDom, resultToAdd.ResultValue.ToString(), resultId);
                objLoad.appendChild(objResultEntryElem);




            }
            var res = new DOMDocument();
            _processXml.ProcessXMLWithResponse(objDom, res);
            // For testing
            if (false)
            {


                var p = @"C:\temp\";
                objDom.save(p + "docResultEntry.xml");
                res.save(p + "resResultEntry.xml");
            }

            //get errors
            var answer = res.getElementsByTagName("errors");


            //MessageBox.Show(answer.ToString());
            if (answer.length < 1)
            {
                CustomMessageBox.Show("התוצאות הוזנו בהצלחה");

            }
            else
            {
                string msg = "Error in ";
                for (int i = 0; i < answer.length; i++)
                {
                    var tttt = answer[i].parentNode;
                    msg += "\n" + "Result id " + tttt.attributes[0].text;
                    //_ResultToAdd.Where(x => x.ResultId == int.Parse(tttt.attributes[0].text)).FirstOrDefault().ResultName;
                    msg += "\n" + answer[i].xml;

                }

                CustomMessageBox.Show(msg);
            }
            gridResults.Rows.Clear();
            btnClean_Click_1(null, null);
            lblDuplicates.Text = string.Empty;
            lblClientName.Text = string.Empty;
            lblDesc.Text = string.Empty;
            //lbReportedResult.Items.Clear();
            dataGridView1.Rows.Clear();
        }

        private IXMLDOMElement ObjResultEntryElem(DOMDocument objDom, string value, string resultId)
        {
            IXMLDOMElement objResultEntryElem = objDom.createElement("result-entry");
            objResultEntryElem.setAttribute("result-id", resultId);
            objResultEntryElem.setAttribute("dilution_factor", value);

            objResultEntryElem.setAttribute("original-result", value);



            return objResultEntryElem;
        }

        private int GetDuplicates(bool addToOld = false)
        {
            var dup = (from result in _ResultToAdd
                       where result.TestId == _currentResult.TestId
                             && result.ResultTemplateId == _currentResult.ResultTemplateId
                       select result);
            if (dup.Count() < 1)
            {
                return 0;
            }
            else
            {
                if (addToOld)
                {
                    foreach (ResultToAdd resultToAdd in dup)
                    {
                        ++resultToAdd.Duplicates;
                    }
                }
                return dup.Count();
            }


        }

        #endregion

        private void btnClean_Click_1(object sender, EventArgs e)
        {
            txtResultID.Enabled = true;
            txtResultValue.Enabled = false;
            txtResultValue.Text = string.Empty;
            txtResultID.Text = string.Empty;
            txtResultID.Focus();
        }

        private void OnRowFormatting(object sender, RowFormattingEventArgs e)
        {


            if (e.RowElement.IsSelected)
            {
                e.RowElement.GradientStyle = GradientStyles.Solid;
                e.RowElement.BackColor = Color.Aquamarine;
                e.RowElement.DrawFill = true;



            }
            else if (e.RowElement.Data.DataBoundItem as ResultToAdd != null &&
                     (e.RowElement.Data.DataBoundItem as ResultToAdd).Duplicates > 0) //
            {

                e.RowElement.GradientStyle = GradientStyles.Solid;
                e.RowElement.BackColor = Color.LightSeaGreen;
                e.RowElement.DrawFill = true;

            }


            else
            {
                e.RowElement.ResetValue(LightVisualElement.DrawFillProperty, ValueResetFlags.Local);
                e.RowElement.ResetValue(LightVisualElement.BackColorProperty, ValueResetFlags.Local);
                e.RowElement.ResetValue(LightVisualElement.GradientStyleProperty, ValueResetFlags.Local);
            }

        }

        #region radio button

        /// <summary>
        /// Init radio buttons mode
        /// </summary>
        private void InitRadioBtnList()
        {

            //Set all radioButtons false
            GetRadioBtnList().Foreach(x => x.IsChecked = false);
        }

        private IEnumerable<RadRadioButton> GetRadioBtnList()
        {

            //Do it radioButton list
            List<RadRadioButton> rbl = d_groupBox1.Controls.OfType<RadRadioButton>().ToList();
            return rbl;
        }

        private void radRadioButton_ToggleStateChanged(object sender, StateChangedEventArgs args)
        {
            //t//ry
            // {



            //    var radioButton = (RadRadioButton)sender;
            //    if (radioButton.IsChecked)
            //    {
            //        //The tag is defined in design
            //        int tag = int.Parse(radioButton.Tag.ToString());
            //        if (tag > 0)
            //        {
            //            decimal value = ( int.Parse(txtResultValue.Text / oldTag)) * tag;
            //            oldTag = tag;
            //            d_spinResultValue.Value = value;
            //            d_groupBox1.SetRedBorder();
            //            ;
            //        }
            //        d_btnOk.Enabled = true;
            //        d_btnReject.Enabled = true;
            //        d_spinResultValue.Enabled = false;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Logger.WriteLogFile(ex);
            //    One1.Controls.CustomMessageBox.Show("error");
            //    throw;
            //}

        #endregion


        }

        private void d_groupBox1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var rrr = GetRadioBtnList().Where(x => x.IsChecked).FirstOrDefault().Text[0];
            var b = int.Parse(rrr.ToString());
            MessageBox.Show(b.ToString());
        }

        private void txtResultID_TextChanged(object sender, EventArgs e)
        {

        }

        private void TopPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtResultValue_TextChanged(object sender, EventArgs e)
        {

        }

    }
}