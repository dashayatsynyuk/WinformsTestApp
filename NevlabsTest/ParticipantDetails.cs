using NevlabsTest.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NevlabsTest
{
    public partial class ParticipantDetails : Form
    {
        private Form1 mainForm;
        private Questionnaire participant;

        public ParticipantDetails()
        {
            InitializeComponent();
        }

        public ParticipantDetails(Form1 main):this()
        {
            mainForm = main;
            participant = new Questionnaire();
        }

        public ParticipantDetails(Form1 main, Questionnaire participant) : this()
        {
            mainForm = main;
            this.participant = participant;
            nameTb.Text = participant.Name;
            birthDateTb.Value = participant.BirthDate;
            emailTb.Text = participant.Email;
            phoneTb.Text = participant.Phone;
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Method for saving and updating records in db
        private void saveBtn_Click(object sender, EventArgs e)
        {
            Questionnaire _participant = new Questionnaire();
            _participant.Name = nameTb.Text;
            _participant.BirthDate = birthDateTb.Value;
            _participant.Email = emailTb.Text;
            _participant.Phone = phoneTb.Text;
            bool result = false;
            if (participant.QuestionnaireId != 0)
            {
                _participant.QuestionnaireId = participant.QuestionnaireId;
                result = UpdateParticipantDetails(_participant);
            }
            else
            {
                result = SaveParticipantDetails(_participant);  
            }
            mainForm.ShowStatus(result, "Save");
            mainForm.DisplayData();
            this.Close();
        }

        //Method for saving
        public bool SaveParticipantDetails(Questionnaire Participant)
        {
            bool result = false;
            using (NevlabsTestEntities _entity = new NevlabsTestEntities())
            {
                _entity.Questionnaire.Add(Participant);
                _entity.SaveChanges();
                result = true;
            }
            return result;
        }

        //Methoid for updating
        public bool UpdateParticipantDetails(Questionnaire Participant)   
        {
            bool result = false;
            using (NevlabsTestEntities _entity = new NevlabsTestEntities())
            {
                Questionnaire participantUpdate = _entity.Questionnaire.Where(x => x.QuestionnaireId == Participant.QuestionnaireId).Select(x => x).FirstOrDefault();
                participantUpdate.Name = Participant.Name.Trim();
                participantUpdate.BirthDate = Participant.BirthDate;
                participantUpdate.Email = Participant.Email.Trim();
                participantUpdate.Phone = Participant.Phone.Trim();
                _entity.SaveChanges();
                result = true;
            }
            return result;
        }
    }
}
