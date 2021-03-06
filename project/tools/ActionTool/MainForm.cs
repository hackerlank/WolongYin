﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using ActionEditor;
using ActionTool.Properties;
using ProtoBuf;


namespace ActionTool
{
    public partial class MainForm : Form
    {
        public UnitActionSetupProto TblData = null;

        public ProtoSerializer ProtobufSerializer = new ProtoSerializer();

        public EditorSetupData EditorData = new EditorSetupData();

        public Dictionary<int, String> UnitIDNames = new Dictionary<int, String>();


        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _LoadEditorSetupFile("EditorSetup.xml");

            string url = Application.StartupPath + EditorData.TablePath + "\\" + Settings.Default.TblFileName;
            if (File.Exists(url))
            {
                TblData = _LoadProtoFile<UnitActionSetupProto>(url);                
            }
            else
            {
                
            }

            url = Application.StartupPath + EditorData.TablePath + "\\..\\..\\" + Settings.Default.RoleExcelFile;
            _LoadUnitDataFromExcel(url);

            _ResetView();
        }

        #region Load&Save
        private void _LoadEditorSetupFile(string filename)
        {
            FileStream stream = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(EditorSetupData));
                stream = File.OpenRead(filename);
                XmlReader reader = XmlReader.Create(stream);
                EditorData = (EditorSetupData)serializer.Deserialize(reader);
                stream.Close();
            }
            catch (InvalidOperationException ex)
            {
                if (stream != null)
                    stream.Close();

                MessageBox.Show("错误：" + ex.Message + "\n无法加载编辑器启动文件！！！", Settings.Default.EditorTitle);

                Application.Exit();
            }
        }

        private void _LoadUnitDataFromExcel(string fileName)
        {
            DataSet dataset = _LoadExcelData(fileName);
            DataTable tb1 = dataset.Tables[0];
            foreach (DataRow row in tb1.Rows)
            {
                int raceID = 0;
                if (int.TryParse(row[0].ToString(), out raceID))
                {
                    UnitIDNames[raceID] = row[1].ToString();
                }
            }
        }


        private DataSet _LoadExcelData(string fileName)
        {
            try
            {
                string strConn;
                strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1'";
                System.Data.OleDb.OleDbConnection OleConn = new System.Data.OleDb.OleDbConnection(strConn);
                OleConn.Open();
                String sql = "SELECT * FROM  [Sheet1$]";//可是更改Sheet名称，比如sheet2，等等   

                System.Data.OleDb.OleDbDataAdapter OleDaExcel = new System.Data.OleDb.OleDbDataAdapter(sql, OleConn);
                DataSet OleDsExcle = new DataSet();
                OleDaExcel.Fill(OleDsExcle, "Sheet1");
                OleConn.Close();
                return OleDsExcle;
            }
            catch (Exception err)
            {
                MessageBox.Show("数据绑定Excel失败!失败原因：" + err.Message, "提示信息",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }

        }

        private T _LoadProtoFile<T>(string filePath)
        {
            using (FileStream stream = File.OpenRead(filePath))
            {
                T t = System.Activator.CreateInstance<T>();
                t = (T)ProtobufSerializer.Deserialize(stream, null, t.GetType());

                stream.Close();
                stream.Dispose();

                return t;
            }
        }

        private void _SaveProtoFile<T>(string filePath, bool isDelete, T t)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                if (File.Exists(filePath) && isDelete)
                {
                    File.Delete(filePath);
                }

                ProtobufSerializer.Serialize(ms, t);

                using (FileStream stream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    BinaryWriter bw = new BinaryWriter(stream);
                    bw.Write(ms.ToArray());
                    bw.Flush();
                    bw.Close();
                    stream.Close();
                }
                ms.Close();
            }
        }
        #endregion

        private void _ResetView()
        {
            tv_roleId.Nodes.Clear();
            foreach (var proto in TblData.ProtoList)
            {
                _AddTreeView(proto);
            }
        }

        void _AddTreeView(UnitActionProto proto)
        {
            string skey = proto.roleID.ToString() + "(" + UnitIDNames[proto.roleID] + ")";
            TreeNode newNode = tv_roleId.Nodes.Add(skey);
            newNode.Tag = proto;

            foreach (var actionStateProto in proto.actions)
            {
                _AddTreeView(proto.roleID, actionStateProto);
            }
        }

        void _AddTreeView(int roleId, ActionStateProto proto)
        {
            TreeNode parentNode = _GetStateTreeNode(roleId);
            string skey = proto.stateID + "(" + proto.stateName + ")";
            TreeNode newNode = parentNode.Nodes.Add(skey);
            newNode.Tag = proto;
        }

        TreeNode _GetTreeNode(int roleId)
        {
            string skey = roleId.ToString() + "(" + UnitIDNames[roleId] + ")";
            if (!tv_roleId.Nodes.ContainsKey(skey))
                tv_roleId.Nodes.Add(skey);

            return tv_roleId.Nodes[skey];
        }

        TreeNode _GetStateTreeNode(int roleId)
        {
            TreeNode parentNode = _GetTreeNode(roleId);
            string skey = "States";
            if (!parentNode.Nodes.ContainsKey(skey))
                parentNode.Nodes.Add(skey);

            return parentNode.Nodes[skey];
        }

        TreeNode _GetDefTreeNode(int roleId)
        {
            TreeNode parentNode = _GetTreeNode(roleId);
            string skey = "HitDefinitions";
            if (!parentNode.Nodes.ContainsKey(skey))
                parentNode.Nodes.Add(skey);

            return parentNode.Nodes[skey];
        }
    }
}
