using NevlabsTest.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NevlabsTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            DisplayData();
            questionnaireInfoGrid.Columns["QuestionnaireId"].Visible = false;
        }

        //Method for displaying data in dataGridView
        public void DisplayData()   
        {
            using (NevlabsTestEntities _entity = new NevlabsTestEntities())
            {
                List<QuestionnaireInfo> _questionaireList = new List<QuestionnaireInfo>();
                _questionaireList = _entity.Questionnaire.Select(x => new QuestionnaireInfo
                {
                    QuestionnaireId = x.QuestionnaireId,
                    Name = x.Name.Trim(),
                    BirthDate = x.BirthDate,
                    Email = x.Email.Trim(),
                    Phone = x.Phone.Trim()
                }).ToList();
                questionnaireInfoGrid.DataSource = _questionaireList;
            }
        }

        //Event for adding records
        private void addBtn_Click(object sender, EventArgs e)
        {
            ParticipantDetails participantForm = new ParticipantDetails(this);
            participantForm.Show();
        }

        //Event for importing data to db(async)
        private void importBtn_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(new ThreadStart(ImportData));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            DisplayData();
        }

        //Method for import of data
        public void ImportData()
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                List<string> inputFile = new List<string>();
                using (StreamReader sr = new StreamReader(openFileDialog1.OpenFile(), Encoding.Default))
                {
                    string file = sr.ReadToEnd();
                    inputFile = new List<string>(file.Split('\n'));
                }
                List<Questionnaire> participantList = new List<Questionnaire>();
                for (int i = 1; i < inputFile.Count - 1; i++)
                {
                    string participant = inputFile[i];
                    Questionnaire questionnaire = new Questionnaire();
                    string[] data = participant.Split('\t');
                    questionnaire.Name = data[0];
                    questionnaire.BirthDate = Convert.ToDateTime(data[1]);
                    questionnaire.Email = data[2];
                    questionnaire.Phone = data[3];
                    participantList.Add(questionnaire);
                }
                bool resultSave = false;
                using (NevlabsTestEntities _entity = new NevlabsTestEntities())
                {
                    _entity.Questionnaire.AddRange(participantList);
                    _entity.SaveChanges();
                    resultSave = true;
                }
                ShowStatus(resultSave, "Import");
            }
        }

        //Additional method for status of actions
        public void ShowStatus(bool result, string Action)  
        {
            if (result)
            {
                if (Action.ToUpper() == "SAVE")
                {
                    MessageBox.Show("Saved Successfully!..", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (Action.ToUpper() == "UPDATE")
                {
                    MessageBox.Show("Updated Successfully!..", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (Action.ToUpper() == "IMPORT")
                {
                    MessageBox.Show("Imported Successfully!..", "Import", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (Action.ToUpper() == "EXPORT")
                {
                    MessageBox.Show("Exported Successfully!..", "Import", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Deleted Successfully!..", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Something went wrong!. Please try again!..", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //Event for updating records
        private void updateBtn_Click(object sender, EventArgs e)
        {
            Questionnaire selectedParticipant = new Questionnaire();
            if(questionnaireInfoGrid.Rows.Count>0)
            {
                if (questionnaireInfoGrid.SelectedRows.Count > 0)
                {
                    foreach (DataGridViewRow row in questionnaireInfoGrid.SelectedRows)  
                    {
                        selectedParticipant.QuestionnaireId = Convert.ToInt32(row.Cells[0].Value);
                        selectedParticipant.Name = row.Cells[1].Value.ToString();
                        selectedParticipant.BirthDate = Convert.ToDateTime(row.Cells[2].Value.ToString());
                        selectedParticipant.Email = row.Cells[3].Value.ToString();
                        selectedParticipant.Phone = row.Cells[4].Value.ToString();
                    }
                }
            }
            ParticipantDetails participantForm = new ParticipantDetails(this, selectedParticipant);
            participantForm.Show();
        }

        //Event for deleting records
        private void deleteBtn_Click(object sender, EventArgs e)
        {
            Questionnaire selectedParticipant = new Questionnaire();
            int participantId = 0;
            if (questionnaireInfoGrid.Rows.Count > 0)
            {
                if (questionnaireInfoGrid.SelectedRows.Count > 0)
                {
                    foreach (DataGridViewRow row in questionnaireInfoGrid.SelectedRows) 
                    {
                        participantId = Convert.ToInt32(row.Cells[0].Value);
                    }
                }
            }
            bool result = false;
            if (participantId > 0)
            {
                result = DeleteStudentDetails(participantId);
            }
            ShowStatus(result, "Delete");
            DisplayData();
        }

        //Method for deleting records
        public bool DeleteStudentDetails(int id)  
        {
            bool result = false;
            using (NevlabsTestEntities _entity = new NevlabsTestEntities())
            {
                Questionnaire participant = _entity.Questionnaire.Where(x => x.QuestionnaireId == id).Select(x => x).FirstOrDefault();
                _entity.Questionnaire.Remove(participant);
                _entity.SaveChanges();
                result = true;
            }
            return result;
        }

        //Method for export of data to CSV file with tabbed delimeter
        private void exportBtn_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = @"C:\";
            saveFileDialog1.Title = "Save text Files";
            saveFileDialog1.DefaultExt = "csv";
            saveFileDialog1.Filter = "Text files (*.csv)|*.csv";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            DialogResult result = saveFileDialog1.ShowDialog();
            bool resultExport = false;
            if (result == DialogResult.OK)
            {

                List<QuestionnaireInfo> _questionaireList;
                using (NevlabsTestEntities _entity = new NevlabsTestEntities())
                {
                    _questionaireList = new List<QuestionnaireInfo>();
                    _questionaireList = _entity.Questionnaire.Select(x => new QuestionnaireInfo
                    {
                        QuestionnaireId = x.QuestionnaireId,
                        Name = x.Name.Trim(),
                        BirthDate = x.BirthDate,
                        Email = x.Email.Trim(),
                        Phone = x.Phone.Trim()
                    }).ToList();
                    questionnaireInfoGrid.DataSource = _questionaireList;
                }

                StringBuilder newLine = new StringBuilder();
                newLine.AppendLine(String.Format("{0}\t{1}\t{2}\t{3}", "ФИО", "Дата рождения", "Email", "Телефон"));
                foreach (QuestionnaireInfo item in _questionaireList)
                {
                    newLine.AppendLine(String.Format("{0}\t{1}\t{2}\t{3}", item.Name, item.BirthDate.ToString(), item.Email, item.Phone));
                }

                using (StreamWriter sr = new StreamWriter(saveFileDialog1.OpenFile(), Encoding.Default))
                {
                    sr.Write(newLine.ToString());
                }
                resultExport = true;
            }
            ShowStatus(resultExport, "Export");
        }
    }
}
