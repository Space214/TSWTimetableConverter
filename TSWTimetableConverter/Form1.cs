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

namespace TSWTimetableConverter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Convert_B_Click(object sender, EventArgs e)
        {
            string Convert = ToConvert_TB.Text;
            Convert = Convert.Replace("\r", "");
            List<string> SplitConvert = Convert.Split('\n').ToList<string>();
            List<string> FinalList = new List<string>();

            while (SplitConvert.ToArray().Length > 1)
            {
                // This could be a switch statment, but I'm lazy and this works just as well for this application
                if (SplitConvert[0].Contains("Class=/Script/TS2PrototypeEditor.EdNode_Formation"))
                {
                    foreach (string str in ConvertFormation(SplitConvert))
                    {
                        FinalList.Add(str);
                    }
                }
                if (SplitConvert[0].Contains("Class=/Script/TS2PrototypeEditor.EdNode_Service"))
                {
                    foreach (string str in ConvertService(SplitConvert))
                    {
                        FinalList.Add(str);
                    }
                }
                if (SplitConvert[0].Contains("Class=/Script/TS2PrototypeEditor.EdNode_GoTo"))
                {
                    foreach (string str in ConvertGoTo(SplitConvert))
                    {
                        FinalList.Add(str);
                    }
                }
                if (SplitConvert[0].Contains("Class=/Script/TS2PrototypeEditor.EdNode_Couple"))
                {
                    foreach (string str in ConvertCouple(SplitConvert))
                    {
                        FinalList.Add(str);
                    }
                }
                if (SplitConvert[0].Contains("Class=/Script/TS2PrototypeEditor.EdNode_LoadUnload"))
                {
                    foreach (string str in ConvertLoadUnload(SplitConvert))
                    {
                        FinalList.Add(str);
                    }
                }
                if (SplitConvert[0].Contains("Class=/Script/TS2PrototypeEditor.EdNode_Uncouple"))
                {
                    foreach (string str in ConvertUncouple(SplitConvert))
                    {
                        FinalList.Add(str);
                    }
                }
                if (SplitConvert[0].Contains("Class=/Script/TS2PrototypeEditor.EdNode_EndService"))
                {
                    foreach (string str in ConvertEndService(SplitConvert))
                    {
                        FinalList.Add(str);
                    }
                }

                SplitConvert.RemoveAt(0);
            }

            foreach (string str in SplitConvert)
            {
                FinalList.Add(str);
            }

            string ReturnText = "";
            foreach (string str in FinalList)
            {
                if (str != "")
                {
                    ReturnText = ReturnText + '\r' + '\n' + " " + '\r' + '\n' + str;
                }
            }

            Converted_TB.Text = ReturnText;
        }

        private List<string> ConvertFormation(List<string> ToConvert)
        {
            int EndObjects = 3;
            int LinesPassed = 0;
            List<string> ConvertedLines = new List<string>();

            foreach (string str in ToConvert)
            {
                if (EndObjects < 1)
                {
                    ConvertedLines.Add("End Object");
                    break;
                }
                if (str.Contains("End Object"))
                {
                    EndObjects--;
                }
                if (str.Contains("Class=/Script/TS2PrototypeEditor.EdNode_Formation"))
                {
                    string FormationName = str.Substring(69).Replace("\"", "").Replace("EdNode", "RouteTimetableGraphNode");
                    ConvertedLines.Add($"Begin Object Class=/Script/TS2PrototypeEditor.RouteTimetableGraphNode_Formation Name=\"{FormationName}\"");
                }
                if (str.Contains("FormationData"))
                {
                    ConvertedLines.Add(str.Replace("FormationData", "Formation"));
                }
                if (str.Contains("Instruction") || str.Contains("NodePos") || str.Contains("NodeGuid"))
                {
                    ConvertedLines.Add(str);
                }
                if (str.Contains("CustomProperties Pin"))
                {
                    ConvertedLines.Add(str.Replace("NoService", "Formation").Replace("LinkedTo=(EdNode", "LinkedTo=(RouteTimetableGraphNode"));
                }

                LinesPassed++;
            }

            ToConvert.RemoveRange(0, LinesPassed - 1);

            return ConvertedLines;
        }

        private List<string> ConvertService(List<string> ToConvert)
        {
            int EndObjects = 3;
            int LinesPassed = 0;
            int PinNum = 0;
            List<string> ConvertedLines = new List<string>();

            foreach (string str in ToConvert)
            {
                if (EndObjects < 1)
                {
                    ConvertedLines.Add("End Object");
                    break;
                }
                if (str.Contains("End Object"))
                {
                    EndObjects--;
                }
                if (str.Contains("Class=/Script/TS2PrototypeEditor.EdNode_Service"))
                {
                    string ServiceName = str.Substring(67).Replace("\"", "").Replace("EdNode", "RouteTimetableGraphNode");
                    ConvertedLines.Add($"Begin Object Class=/Script/TS2PrototypeEditor.RouteTimetableGraphNode_Service Name=\"{ServiceName}\"");
                }
                if (str.Contains("ServiceData"))
                {
                    ConvertedLines.Add(str.Replace("ServiceData", "Service"));
                }
                if (str.Contains("NodePos") || str.Contains("NodeGuid"))
                {
                    ConvertedLines.Add(str);
                }
                if (str.Contains("CustomProperties Pin"))
                {
                    // we're just going to assume this came after the service name is defined... WHICH IT SHOULD BE
                    if (PinNum < 1)
                    {
                        ConvertedLines.Add(str.Replace("PinType.PinCategory=\"NoService\"", "PinName=\"Formation\",PinType.PinCategory=\"Formation\"").Replace("LinkedTo=(EdNode", "LinkedTo=(RouteTimetableGraphNode"));
                        PinNum++;
                    }
                    else
                    {
                        ConvertedLines.Add(str.Replace("Direction=\"EGPD_Output\",PinType.PinCategory=\"Service\"", "PinName=\"Service\",Direction=\"EGPD_Output\",PinType.PinCategory=\"Service\"").Replace("LinkedTo=(EdNode", "LinkedTo=(RouteTimetableGraphNode"));
                    }
                }

                LinesPassed++;
            }

            ToConvert.RemoveRange(0, LinesPassed - 1);

            return ConvertedLines;
        }

        private List<string> ConvertGoTo(List<string> ToConvert)
        {
            int EndObjects = 3;
            int LinesPassed = 0;
            int PinNum = 0;
            List<string> ConvertedLines = new List<string>();

            foreach (string str in ToConvert)
            {
                if (EndObjects < 1)
                {
                    // Adds new pin
                    // ConvertedLines.Add("CustomProperties Pin (PinId=755200B44C9E2884A2ED728C21D09CEA,PinName=\"Instruction Reference\",Direction=\"EGPD_Output\",PinType.PinCategory=\"Instruction\",PinType.PinSubCategory=\"\",PinType.PinSubCategoryObject=ScriptStruct'\" / Script / TS2Prototype.RouteTimetableServiceInstructionReference\"',PinType.PinSubCategoryMemberReference=(),PinType.PinValueType=(),PinType.ContainerType=None,PinType.bIsReference=True,PinType.bIsConst=False,PinType.bIsWeakPointer=False,PinType.bIsUObjectWrapper=True,PersistentGuid=00000000000000000000000000000000,bHidden=False,bNotConnectable=False,bDefaultValueIsReadOnly=False,bDefaultValueIsIgnored=False,bAdvancedView=False,bOrphanedPin=False,)");
                    ConvertedLines.Add("End Object");
                    break;
                }
                if (str.Contains("End Object"))
                {
                    EndObjects--;
                }
                if (str.Contains("Class=/Script/TS2PrototypeEditor.EdNode_GoTo"))
                {
                    string ServiceName = str.Substring(64).Replace("\"", "").Replace("EdNode", "RouteTimetableGraphNode");
                    ConvertedLines.Add($"Begin Object Class=/Script/TS2PrototypeEditor.RouteTimetableGraphNode_InstructionGoTo Name=\"{ServiceName}\"");
                }
                if (str.Contains("InstructionData"))
                {
                    ConvertedLines.Add(str.Replace("InstructionData", "Instruction"));
                }
                if (str.Contains("NodePos") || str.Contains("NodeGuid"))
                {
                    ConvertedLines.Add(str);
                }
                if (str.Contains("CustomProperties Pin"))
                {
                    if (PinNum < 1)
                    {
                        ConvertedLines.Add(str.Replace("PinType.PinCategory=\"Service\"", "PinName=\"Service In\",PinType.PinCategory=\"Service\"").Replace("LinkedTo=(EdNode", "LinkedTo=(RouteTimetableGraphNode"));
                        // Adds new pin
                        // ConvertedLines.Add("CustomProperties Pin (PinId=538EAD564470757704396B98EFD1FF60,PinName=\"Instruction Dependancies\",PinType.PinCategory=\"Instruction\",PinType.PinSubCategory=\"\",PinType.PinSubCategoryObject=ScriptStruct'\" / Script / TS2Prototype.RouteTimetableServiceInstructionReference\"',PinType.PinSubCategoryMemberReference=(),PinType.PinValueType=(),PinType.ContainerType=Array,PinType.bIsReference=False,PinType.bIsConst=False,PinType.bIsWeakPointer=False,PinType.bIsUObjectWrapper=True,PersistentGuid=00000000000000000000000000000000,bHidden=False,bNotConnectable=False,bDefaultValueIsReadOnly=False,bDefaultValueIsIgnored=False,bAdvancedView=False,bOrphanedPin=False,)");
                        PinNum++;
                    }
                    else
                    {
                        ConvertedLines.Add(str.Replace("PinType.PinCategory=\"Service\"", "PinName=\"Service Out\",PinType.PinCategory=\"Service\"").Replace("LinkedTo=(EdNode", "LinkedTo=(RouteTimetableGraphNode"));
                    }
                }

                LinesPassed++;
            }

            ToConvert.RemoveRange(0, LinesPassed - 1);

            return ConvertedLines;
        }

        private List<string> ConvertCouple(List<string> ToConvert)
        {
            int EndObjects = 3;
            int LinesPassed = 0;
            int PinNum = 0;
            List<string> ConvertedLines = new List<string>();

            foreach (string str in ToConvert)
            {
                if (EndObjects < 1)
                {
                    ConvertedLines.Add("End Object");
                    break;
                }
                if (str.Contains("End Object"))
                {
                    EndObjects--;
                }
                if (str.Contains("Class=/Script/TS2PrototypeEditor.EdNode_Couple"))
                {
                    string ServiceName = str.Substring(64).Replace("\"", "").Replace("EdNode", "RouteTimetableGraphNode");
                    ConvertedLines.Add($"Begin Object Class=/Script/TS2PrototypeEditor.RouteTimetableGraphNode_InstructionCouple Name=\"{ServiceName}\"");
                }
                if (str.Contains("InstructionData"))
                {
                    ConvertedLines.Add(str.Replace("InstructionData", "Instruction"));
                }
                if (str.Contains("NodePos") || str.Contains("NodeGuid"))
                {
                    ConvertedLines.Add(str);
                }
                if (str.Contains("CustomProperties Pin"))
                {
                    if (PinNum == 2)
                    {
                        ConvertedLines.Add(str.Replace("Direction=\"EGPD_Output\",PinType.PinCategory=\"Service\"", "PinName=\"Service Out\",Direction=\"EGPD_Output\",PinType.PinCategory=\"Service\"").Replace("LinkedTo=(EdNode", "LinkedTo=(RouteTimetableGraphNode"));
                        PinNum++;
                    }
                    if (PinNum == 1)
                    {

                        ConvertedLines.Add(str.Replace("PinType.PinCategory=\"NoService\"", "PinName=\"Formation\",PinType.PinCategory=\"Formation\"").Replace("LinkedTo=(EdNode", "LinkedTo=(RouteTimetableGraphNode"));
                        PinNum++;
                    }
                    if (PinNum == 0)
                    {
                        ConvertedLines.Add(str.Replace("PinType.PinCategory=\"Service\"", "PinName=\"Service In\",PinType.PinCategory=\"Service\"").Replace("LinkedTo=(EdNode", "LinkedTo=(RouteTimetableGraphNode"));
                        PinNum++;
                    }
                }

                LinesPassed++;
            }

            ToConvert.RemoveRange(0, LinesPassed - 1);

            return ConvertedLines;
        }

        private List<string> ConvertLoadUnload(List<string> ToConvert)
        {
            int EndObjects = 3;
            int LinesPassed = 0;
            int PinNum = 0;
            List<string> ConvertedLines = new List<string>();

            foreach (string str in ToConvert)
            {
                if (EndObjects < 1)
                {
                    //ConvertedLines.Add("CustomProperties Pin (PinId=755200B44C9E2884A2ED728C21D09CEA,PinName=\"Instruction Reference\",Direction=\"EGPD_Output\",PinType.PinCategory=\"Instruction\",PinType.PinSubCategory=\"\",PinType.PinSubCategoryObject=ScriptStruct'\" / Script / TS2Prototype.RouteTimetableServiceInstructionReference\"',PinType.PinSubCategoryMemberReference=(),PinType.PinValueType=(),PinType.ContainerType=None,PinType.bIsReference=True,PinType.bIsConst=False,PinType.bIsWeakPointer=False,PinType.bIsUObjectWrapper=True,PersistentGuid=00000000000000000000000000000000,bHidden=False,bNotConnectable=False,bDefaultValueIsReadOnly=False,bDefaultValueIsIgnored=False,bAdvancedView=False,bOrphanedPin=False,)");
                    ConvertedLines.Add("End Object");
                    break;
                }
                if (str.Contains("End Object"))
                {
                    EndObjects--;
                }
                if (str.Contains("Class=/Script/TS2PrototypeEditor.EdNode_LoadUnload"))
                {
                    string ServiceName = str.Substring(69).Replace("\"", "").Replace("EdNode", "RouteTimetableGraphNode");
                    ConvertedLines.Add($"Begin Object Class=/Script/TS2PrototypeEditor.RouteTimetableGraphNode_InstructionLoadUnload Name=\"{ServiceName}\"");
                }
                if (str.Contains("InstructionData"))
                {
                    ConvertedLines.Add(str.Replace("InstructionData", "Instruction"));
                }
                if (str.Contains("NodePos") || str.Contains("NodeGuid"))
                {
                    ConvertedLines.Add(str);
                }
                if (str.Contains("CustomProperties Pin"))
                {
                    if (PinNum < 1)
                    {
                        ConvertedLines.Add(str.Replace("PinType.PinCategory=\"Service\"", "PinName=\"Service In\",PinType.PinCategory=\"Service\"").Replace("LinkedTo=(EdNode", "LinkedTo=(RouteTimetableGraphNode"));
                        //ConvertedLines.Add("CustomProperties Pin (PinId=538EAD564470757704396B98EFD1FF60,PinName=\"Instruction Dependancies\",PinType.PinCategory=\"Instruction\",PinType.PinSubCategory=\"\",PinType.PinSubCategoryObject=ScriptStruct'\" / Script / TS2Prototype.RouteTimetableServiceInstructionReference\"',PinType.PinSubCategoryMemberReference=(),PinType.PinValueType=(),PinType.ContainerType=Array,PinType.bIsReference=False,PinType.bIsConst=False,PinType.bIsWeakPointer=False,PinType.bIsUObjectWrapper=True,PersistentGuid=00000000000000000000000000000000,bHidden=False,bNotConnectable=False,bDefaultValueIsReadOnly=False,bDefaultValueIsIgnored=False,bAdvancedView=False,bOrphanedPin=False,)");
                        PinNum++;
                    }
                    else
                    {
                        ConvertedLines.Add(str.Replace("Direction=\"EGPD_Output\",PinType.PinCategory=\"Service\"", "PinName=\"Service Out\",Direction=\"EGPD_Output\",PinType.PinCategory=\"Service\"").Replace("LinkedTo=(EdNode", "LinkedTo=(RouteTimetableGraphNode"));
                    }
                }

                LinesPassed++;
            }

            ToConvert.RemoveRange(0, LinesPassed - 1);

            return ConvertedLines;
        }

        private List<string> ConvertUncouple(List<string> ToConvert)
        {
            int EndObjects = 3;
            int LinesPassed = 0;
            int PinNum = 0;
            List<string> ConvertedLines = new List<string>();
            string LinkedToFirst = "";

            foreach (string str in ToConvert)
            {
                if (EndObjects < 1)
                {
                    //ConvertedLines.Add("CustomProperties Pin (PinId=755200B44C9E2884A2ED728C21D09CEA,PinName=\"Instruction Reference\",Direction=\"EGPD_Output\",PinType.PinCategory=\"Instruction\",PinType.PinSubCategory=\"\",PinType.PinSubCategoryObject=ScriptStruct'\" / Script / TS2Prototype.RouteTimetableServiceInstructionReference\"',PinType.PinSubCategoryMemberReference=(),PinType.PinValueType=(),PinType.ContainerType=None,PinType.bIsReference=True,PinType.bIsConst=False,PinType.bIsWeakPointer=False,PinType.bIsUObjectWrapper=True,PersistentGuid=00000000000000000000000000000000,bHidden=False,bNotConnectable=False,bDefaultValueIsReadOnly=False,bDefaultValueIsIgnored=False,bAdvancedView=False,bOrphanedPin=False,)");
                    ConvertedLines.Add("End Object");
                    break;
                }
                if (str.Contains("End Object"))
                {
                    EndObjects--;
                }
                if (str.Contains("Class=/Script/TS2PrototypeEditor.EdNode_Uncouple"))
                {
                    string ServiceName = str.Substring(68).Replace("\"", "").Replace("EdNode", "RouteTimetableGraphNode");
                    ConvertedLines.Add($"Begin Object Class=/Script/TS2PrototypeEditor.RouteTimetableGraphNode_InstructionUncouple Name=\"{ServiceName}\"");
                }
                if (str.Contains("InstructionData"))
                {
                    ConvertedLines.Add(str.Replace("InstructionData", "Instruction"));
                }
                if (str.Contains("NodePos") || str.Contains("NodeGuid"))
                {
                    ConvertedLines.Add(str);
                }
                if (str.Contains("CustomProperties Pin"))
                {
                    if (PinNum == 2)
                    {
                        ConvertedLines.Add(str.Replace("PinType.PinCategory=\"NoService\"", "PinName=\"Formation\",Direction=\"EGPD_Output\",PinType.PinCategory=\"Formation\"").Replace(LinkedToFirst, "").Replace("LinkedTo=(EdNode", "LinkedTo=(RouteTimetableGraphNode"));
                        PinNum++;
                    }
                    if (PinNum == 1)
                    {

                        ConvertedLines.Add(str.Replace("PinType.PinCategory=\"Service\"", "PinName=\"Service Out\",Direction=\"EGPD_Output\",PinType.PinCategory=\"Service\"").Replace(LinkedToFirst, "").Replace("LinkedTo=(EdNode", "LinkedTo=(RouteTimetableGraphNode"));
                        PinNum++;
                    }
                    if (PinNum == 0)
                    {
                        ConvertedLines.Add(str.Replace("PinType.PinCategory=\"Service\"", "PinName=\"Service In\",PinType.PinCategory=\"Service\"").Replace("LinkedTo=(EdNode", "LinkedTo=(RouteTimetableGraphNode"));
                        //ConvertedLines.Add("CustomProperties Pin (PinId=538EAD564470757704396B98EFD1FF60,PinName=\"Instruction Dependancies\",PinType.PinCategory=\"Instruction\",PinType.PinSubCategory=\"\",PinType.PinSubCategoryObject=ScriptStruct'\" / Script / TS2Prototype.RouteTimetableServiceInstructionReference\"',PinType.PinSubCategoryMemberReference=(),PinType.PinValueType=(),PinType.ContainerType=Array,PinType.bIsReference=False,PinType.bIsConst=False,PinType.bIsWeakPointer=False,PinType.bIsUObjectWrapper=True,PersistentGuid=00000000000000000000000000000000,bHidden=False,bNotConnectable=False,bDefaultValueIsReadOnly=False,bDefaultValueIsIgnored=False,bAdvancedView=False,bOrphanedPin=False,)");

                        LinkedToFirst = str.Substring(str.IndexOf("LinkedTo=(") + 10, str.Substring(str.IndexOf("LinkedTo=(") + 10).IndexOf(" "));

                        PinNum++;
                    }
                }

                LinesPassed++;
            }

            ToConvert.RemoveRange(0, LinesPassed - 1);

            return ConvertedLines;
        }

        private List<string> ConvertEndService(List<string> ToConvert)
        {
            int EndObjects = 3;
            int LinesPassed = 0;
            int PinNum = 0;
            List<string> ConvertedLines = new List<string>();

            foreach (string str in ToConvert)
            {
                if (EndObjects < 1)
                {
                    ConvertedLines.Add("End Object");
                    break;
                }
                if (str.Contains("End Object"))
                {
                    EndObjects--;
                }
                if (str.Contains("Class=/Script/TS2PrototypeEditor.EdNode_EndService"))
                {
                    string ServiceName = str.Substring(70).Replace("\"", "").Replace("EdNode", "RouteTimetableGraphNode");
                    ConvertedLines.Add($"Begin Object Class=/Script/TS2PrototypeEditor.RouteTimetableGraphNode_EndService Name=\"{ServiceName}\"");
                }
                if (str.Contains("NodePos") || str.Contains("NodeGuid"))
                {
                    ConvertedLines.Add(str);
                }
                if (str.Contains("CustomProperties Pin"))
                {
                    if (PinNum < 1)
                    {
                        ConvertedLines.Add(str.Replace("PinType.PinCategory=\"Service\"", "PinName=\"Service\",PinType.PinCategory=\"Service\"").Replace("LinkedTo=(EdNode", "LinkedTo=(RouteTimetableGraphNode"));
                        PinNum++;
                    }
                    else
                    {
                        ConvertedLines.Add(str.Replace("Direction=\"EGPD_Output\",PinType.PinCategory=\"NoService\"", "PinName=\"Formation\",Direction=\"EGPD_Output\",PinType.PinCategory=\"Formation\"").Replace("LinkedTo=(EdNode", "LinkedTo=(RouteTimetableGraphNode"));
                    }
                }

                LinesPassed++;
            }

            ToConvert.RemoveRange(0, LinesPassed - 1);

            return ConvertedLines;
        }

        private void ConvertFile_B_Click(object sender, EventArgs e)
        {
            string OpenFile = File.ReadAllText(FilePath_TB.Text+ "/timetable.txt");

            OpenFile = OpenFile.Replace("\r", "");
            List<string> SplitConvert = OpenFile.Split('\n').ToList<string>();
            List<string> FinalList = new List<string>();

            while (SplitConvert.ToArray().Length > 1)
            {
                // This could be a switch statment, but I'm lazy and this works just as well for this application
                if (SplitConvert[0].Contains("Class=/Script/TS2PrototypeEditor.EdNode_Formation"))
                {
                    foreach (string str in ConvertFormation(SplitConvert))
                    {
                        FinalList.Add(str);
                    }
                }
                if (SplitConvert[0].Contains("Class=/Script/TS2PrototypeEditor.EdNode_Service"))
                {
                    foreach (string str in ConvertService(SplitConvert))
                    {
                        FinalList.Add(str);
                    }
                }
                if (SplitConvert[0].Contains("Class=/Script/TS2PrototypeEditor.EdNode_GoTo"))
                {
                    foreach (string str in ConvertGoTo(SplitConvert))
                    {
                        FinalList.Add(str);
                    }
                }
                if (SplitConvert[0].Contains("Class=/Script/TS2PrototypeEditor.EdNode_Couple"))
                {
                    foreach (string str in ConvertCouple(SplitConvert))
                    {
                        FinalList.Add(str);
                    }
                }
                if (SplitConvert[0].Contains("Class=/Script/TS2PrototypeEditor.EdNode_LoadUnload"))
                {
                    foreach (string str in ConvertLoadUnload(SplitConvert))
                    {
                        FinalList.Add(str);
                    }
                }
                if (SplitConvert[0].Contains("Class=/Script/TS2PrototypeEditor.EdNode_Uncouple"))
                {
                    foreach (string str in ConvertUncouple(SplitConvert))
                    {
                        FinalList.Add(str);
                    }
                }
                if (SplitConvert[0].Contains("Class=/Script/TS2PrototypeEditor.EdNode_EndService"))
                {
                    foreach (string str in ConvertEndService(SplitConvert))
                    {
                        FinalList.Add(str);
                    }
                }

                SplitConvert.RemoveAt(0);
            }

            foreach (string str in SplitConvert)
            {
                FinalList.Add(str);
            }

            string ReturnText = "";
            foreach (string str in FinalList)
            {
                if (str != "")
                {
                    ReturnText = ReturnText + '\r' + '\n' + " " + '\r' + '\n' + str;
                }
            }

            File.WriteAllText(FilePath_TB.Text + "/ConvertedTimetable.txt", ReturnText);
        }
    }
}
