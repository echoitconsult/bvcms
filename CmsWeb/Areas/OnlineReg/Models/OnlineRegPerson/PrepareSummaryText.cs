﻿using System.Linq;
using System.Text;
using CmsData.Registration;
using UtilityExtensions;
using CmsData;

namespace CmsWeb.Models
{
    public partial class OnlineRegPersonModel
    {
        public string PrepareSummaryText(Transaction ti)
        {
            if (RecordFamilyAttendance())
                return SummarizeFamilyAttendance();

            var om = GetOrgMember();
            var sb = StartSummary();
            SummarizePayment(ti, om, sb);
            SummarizeSupportMissionTrip(sb);
            SummarizeAnswers(sb, om);
            return FinishSummary(sb);
        }

        private static void SummarizePayment(Transaction ti, OrganizationMember om, StringBuilder sb)
        {
            if ((ti.Amt ?? 0) == 0 || om == null) 
                return;

            var ts = om.TransactionSummary(DbUtil.Db);
            if (ts != null)
                sb.AppendFormat(@"
<tr><td colspan='2'> 
<table cellpadding=4>
    <tr>
        <td>Registrant Fee</td>
        <td>Amount Paid</td>
        <td>Amount Due</td>
    </tr>
    <tr>
        <td align='right'>{0}</td>
        <td align='right'>{1}</td>
        <td align='right'>{2}</td>
    </tr>
</table>
</td></tr>
    ", ts.IndAmt.ToString2("c"),
                    om.TotalPaid(DbUtil.Db).ToString("c"),
                    om.AmountDue(DbUtil.Db).ToString("c"));
        }

        private StringBuilder StartSummary()
        {
            var sb = new StringBuilder();
            sb.Append("<table>");
            sb.AppendFormat("<tr><td width='50%'>Org:</td><td width='50%'>{0}</td></tr>\n", org.OrganizationName);
            sb.AppendFormat("<tr><td>First:</td><td>{0}</td></tr>\n", person.PreferredName);
            sb.AppendFormat("<tr><td>Last:</td><td>{0}</td></tr>\n", person.LastName);
            return sb;
        }
        private static string FinishSummary(StringBuilder sb)
        {
            sb.AppendLine("</table>");
            return sb.ToString();
        }


        private void SummarizeSupportMissionTrip(StringBuilder sb)
        {
            if (!Parent.SupportMissionTrip)
                return;
            var goer = DbUtil.Db.LoadPersonById(MissionTripGoerId ?? 0);
            if (goer != null)
                sb.AppendFormat("<tr><td>Support Mission Trip for:</td><td>{0}</td></tr>\n", goer.Name);
            if (MissionTripSupportGeneral > 0)
                sb.Append("<tr><td>Support Mission Trip:</td><td>Any other participiants</td></tr>\n");
        }

        private string SummarizeFamilyAttendance()
        {
            var sb = StartSummary();
            foreach (var m in FamilyAttend.Where(m => m.Attend))
                if (m.PeopleId != null)
                    sb.Append("<tr><td colspan=\"2\">{0}{1}</td></tr>\n"
                        .Fmt(m.Name, (m.Age.HasValue ? " ({0})".Fmt(m.Age) : "")));
                else
                {
                    sb.Append("<tr><td colspan=\"2\">{0}{1}".Fmt(m.Name, (m.Age.HasValue ? " ({0})".Fmt(m.Age) : "")));
                    if (m.Email.HasValue())
                        sb.Append(", {0}".Fmt(m.Email));
                    if (m.Birthday.HasValue())
                        sb.Append(", {0}".Fmt(m.Birthday));
                    if (m.MaritalId.HasValue)
                        sb.Append(", {0}".Fmt(m.Marital));
                    if (m.GenderId.HasValue)
                        sb.Append(", {0}".Fmt(m.Gender));
                    sb.Append("</td></tr>\n");
                }
            return FinishSummary(sb);
        }

        private void SummarizeAnswers(StringBuilder sb, OrganizationMember om)
        {
            var rr = person.RecRegs.Single();

            foreach (var ask in setting.AskItems)
            {
                switch (ask.Type)
                {
                    case "AskTickets":
                        sb.AppendFormat("<tr><td>Tickets:</td><td>{0}</td></tr>\n", om.Tickets);
                        break;
                    case "AskSize":
                        sb.AppendFormat("<tr><td>Shirt:</td><td>{0}</td></tr>\n", om.ShirtSize);
                        break;
                    case "AskEmContact":
                        sb.AppendFormat("<tr><td>Emerg Contact:</td><td>{0}</td></tr>\n", rr.Emcontact);
                        sb.AppendFormat("<tr><td>Emerg Phone:</td><td>{0}</td></tr>\n", rr.Emphone);
                        break;
                    case "AskDoctor":
                        sb.AppendFormat("<tr><td>Physician Name:</td><td>{0}</td></tr>\n", rr.Doctor);
                        sb.AppendFormat("<tr><td>Physician Phone:</td><td>{0}</td></tr>\n", rr.Docphone);
                        break;
                    case "AskInsurance":
                        sb.AppendFormat("<tr><td>Insurance Carrier:</td><td>{0}</td></tr>\n", rr.Insurance);
                        sb.AppendFormat("<tr><td>Insurance Policy:</td><td>{0}</td></tr>\n", rr.Policy);
                        break;
                    case "AskRequest":
                        sb.AppendFormat("<tr><td>{1}:</td><td>{0}</td></tr>\n", om.Request, ((AskRequest) ask).Label);
                        break;
                    case "AskHeader":
                        sb.AppendFormat("<tr><td colspan='2'><h4>{0}</h4></td></tr>\n", ((AskHeader) ask).Label);
                        break;
                    case "AskInstruction":
                        break;
                    case "AskAllergies":
                        sb.AppendFormat("<tr><td>Medical:</td><td>{0}</td></tr>\n", rr.MedicalDescription);
                        break;
                    case "AskTylenolEtc":
                        sb.AppendFormat("<tr><td>Tylenol?: {0},", tylenol == true ? "Yes" : tylenol == false ? "No" : "");
                        sb.AppendFormat(" Advil?: {0},", advil == true ? "Yes" : advil == false ? "No" : "");
                        sb.AppendFormat(" Robitussin?: {0},", robitussin == true ? "Yes" : robitussin == false ? "No" : "");
                        sb.AppendFormat(" Maalox?: {0}</td></tr>\n", maalox == true ? "Yes" : maalox == false ? "No" : "");
                        break;
                    case "AskChurch":
                        sb.AppendFormat("<tr><td>Member:</td><td>{0}</td></tr>\n", rr.Member);
                        sb.AppendFormat("<tr><td>OtherChurch:</td><td>{0}</td></tr>\n", rr.ActiveInAnotherChurch);
                        break;
                    case "AskParents":
                        sb.AppendFormat("<tr><td>Mother's name:</td><td>{0}</td></tr>\n", rr.Mname);
                        sb.AppendFormat("<tr><td>Father's name:</td><td>{0}</td></tr>\n", rr.Fname);
                        break;
                    case "AskCoaching":
                        sb.AppendFormat("<tr><td>Coaching:</td><td>{0}</td></tr>\n", rr.Coaching);
                        break;
                    case "AskSMS":
                        sb.AppendFormat("<tr><td>Receive Texts:</td><td>{0}</td></tr>\n", person.ReceiveSMS);
                        break;
                    case "AskDropdown":
                        SummarizeDropdownChoice(sb, ask);
                        break;
                    case "AskMenu":
                        SummarizeMenuChoices(sb, ask);
                        break;
                    case "AskCheckboxes":
                        SummarizeCheckboxChoices(sb, ask);
                        break;
                    case "AskYesNoQuestions":
                        SummarizeYesNoChoices(sb, ask);
                        break;
                    case "AskExtraQuestions":
                        SummarizeExtraAnswers(sb, ask);
                        break;
                    case "AskText":
                        SummarieTextAnswers(sb, ask);
                        break;
                    case "AskGradeOptions":
                        SummarizeGradeOption(sb, ask);
                        break;
                }
            }
            if (setting.AgeGroups.Count > 0)
                sb.AppendFormat("<tr><td>AgeGroup:</td><td>{0}</td></tr>\n", AgeGroup());
        }

        private void SummarizeDropdownChoice(StringBuilder sb, Ask ask)
        {
            sb.AppendFormat("<tr><td>{1}:</td><td>{0}</td></tr>\n",
                ((AskDropdown) ask).SmallGroupChoice(option).Description,
                Util.PickFirst(((AskDropdown) ask).Label, "Options"));
        }

        private void SummarizeGradeOption(StringBuilder sb, Ask ask)
        {
            sb.AppendFormat("<tr><td>GradeOption:</td><td>{0}</td></tr>\n",
                GradeOptions(ask).SingleOrDefault(s => s.Value == (gradeoption ?? "00")).Text);
        }

        private void SummarieTextAnswers(StringBuilder sb, Ask ask)
        {
            foreach (var a in Text[ask.UniqueId])
                if (a.Value.HasValue())
                    sb.AppendFormat("<tr><td>{0}:</td><td>{1}</td></tr>\n".Fmt(a.Key, a.Value));
        }

        private void SummarizeExtraAnswers(StringBuilder sb, Ask ask)
        {
            foreach (var a in ExtraQuestion[ask.UniqueId])
                if (a.Value.HasValue())
                    sb.AppendFormat("<tr><td>{0}:</td><td>{1}</td></tr>\n".Fmt(a.Key, a.Value));
        }

        private void SummarizeYesNoChoices(StringBuilder sb, Ask ask)
        {
            foreach (var a in ((AskYesNoQuestions) ask).list)
                if (YesNoQuestion.ContainsKey(a.SmallGroup))
                    sb.AppendFormat("<tr><td>{0}:</td><td>{1}</td></tr>\n".Fmt(a.Question,
                        YesNoQuestion[a.SmallGroup] == true ? "Yes" : "No"));
        }

        private void SummarizeCheckboxChoices(StringBuilder sb, Ask ask)
        {
            var askcb = (AskCheckboxes) ask;
            var menulabel = askcb.Label;
            foreach (var i in askcb.CheckboxItemsChosen(Checkbox))
            {
                string row;
                if (menulabel.HasValue())
                    sb.Append("<tr><td colspan='2'><br>{0}</td></tr>\n".Fmt(menulabel));
                if (i.Fee > 0)
                    row = "<tr><td></td><td>{0} (${1:N2})<br>({2})</td></tr>\n".Fmt(i.Description, i.Fee,
                        i.SmallGroup);
                else
                    row = "<tr><td></td><td>{0}<br>({1})</td></tr>\n".Fmt(i.Description, i.SmallGroup);
                sb.Append(row);
                menulabel = string.Empty;
            }
        }

        private void SummarizeMenuChoices(StringBuilder sb, Ask ask)
        {
            var menulabel = ((AskMenu) ask).Label;
            foreach (var i in ((AskMenu) ask).MenuItemsChosen(MenuItem[ask.UniqueId]))
            {
                string row;
                if (i.amt > 0)
                    row = "<tr><td>{0}</td><td>{1} {2} (at {3:N2})</td></tr>\n".Fmt(menulabel, i.number, i.desc,
                        i.amt);
                else
                    row = "<tr><td>{0}</td><td>{1} {2}</td></tr>\n".Fmt(menulabel, i.number, i.desc);
                sb.AppendFormat(row);
                menulabel = string.Empty;
            }
        }
    }
}