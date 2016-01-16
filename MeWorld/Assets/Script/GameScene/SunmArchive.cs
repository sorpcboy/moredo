using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using System;

public class SunmArchive
{
    private string XmlDir_Path = Application.dataPath + "/XmlDir";
    private XmlDocument _xmlDoc = new XmlDocument();
    public List<SunmArchiveBean> dataSetList = null;

    #region XmlSave_Path
    private string XmlSave_Path;
    private string XmlSave_root = "Archive";
    #endregion

    public SunmArchive(string _XmlDir_Path, string _XmlSave_Path)
    {
        this.XmlDir_Path = _XmlDir_Path;
        this.XmlSave_Path = _XmlDir_Path + _XmlSave_Path;
        Init();
    }

    public SunmArchive(string _XmlDir_File)
    {
        this.XmlSave_Path = _XmlDir_File;
    }

    /// <summary>
    /// Init this instance.检测文件是否存在
    /// </summary>
    void Init()
    {
        if (!Directory.Exists(XmlDir_Path))
        {
            Directory.CreateDirectory(XmlDir_Path);
        }
        if (File.Exists(XmlSave_Path))
        {
            File.Delete(XmlSave_Path);
        }

        CheckAndCreateXml(XmlSave_Path, XmlSave_root);
    }

    /// <summary>
    /// Checks the and create xml.判断xml是否存在，不存在就创建一个，并同时创建根节点
    /// </summary>
    /// <param name="_name">_name.</param>
    /// <param name="_root">_root.</param>
    void CheckAndCreateXml(string _name, string _root)
    {
        if (!File.Exists(_name))
        {
            //File.Create(_name);
            XmlDocument tempDoc = new XmlDocument();
            //tempDoc.Load(_name);
            XmlElement rootNode = tempDoc.CreateElement(_root);
            tempDoc.AppendChild(rootNode);
            tempDoc.Save(_name);
        }
    }

    /// <summary>
    /// Adds the role.向xml文件中添加一个角色或者添加一个存档
    /// </summary>
    /// <param name="_roleData">_role data.</param>
    /// <param name="r_and_s">R_and_s.</param>
    public void AddRole(SunmArchiveBean _roleData)
    {
        string newNodeName = "";
        newNodeName = "Item";
        _xmlDoc.Load(XmlSave_Path);

        XmlElement newNode = _xmlDoc.CreateElement(newNodeName);
        newNode.SetAttribute("id", new Vector3(_roleData.x, _roleData.y, _roleData.z) + "");
        _xmlDoc.DocumentElement.AppendChild(newNode);

        XmlElement matarialId = _xmlDoc.CreateElement("matarialId");
        matarialId.InnerText = _roleData.matarialId + "";
        XmlElement archiveModel = _xmlDoc.CreateElement("archiveModel");
        archiveModel.InnerText = _roleData.archiveModel;
        XmlElement xP = _xmlDoc.CreateElement("xP");
        xP.InnerText = _roleData.x + "";
        XmlElement yP = _xmlDoc.CreateElement("yP");
        yP.InnerText = _roleData.y + "";
        XmlElement zP = _xmlDoc.CreateElement("zP");
        zP.InnerText = _roleData.z + "";

        newNode.AppendChild(matarialId);
        newNode.AppendChild(xP);
        newNode.AppendChild(yP);
        newNode.AppendChild(zP);
        newNode.AppendChild(archiveModel);

        _xmlDoc.Save(XmlSave_Path);
    }

    /// <summary>
    /// Clears the xml file.清空整个xml文件
    /// </summary>
    /// <param name="r_and_s">R_and_s.</param>
    public void ClearXmlFile()
    {
        _xmlDoc.Load(XmlSave_Path);
        _xmlDoc.DocumentElement.RemoveAll();
        _xmlDoc.Save(XmlSave_Path);
    }

    /// <summary>
    /// Res the fresh xml identifier.当xml中有节点被删除的时候从新排下节点的id。
    /// </summary>
    /// <param name="r_and_s">R_and_s.</param>
    public void ReFreshXmlId()
    {
        _xmlDoc.Load(XmlSave_Path);
        if (_xmlDoc.DocumentElement.HasChildNodes)
        {
            XmlNodeList _chilidlist = _xmlDoc.DocumentElement.ChildNodes;
            int i = 0;
            foreach (XmlNode xn in _chilidlist)
            {
                XmlElement xe = (XmlElement)xn;
                xe.Attributes[0].InnerText = (i++).ToString();
            }
        }
        _xmlDoc.Save(XmlSave_Path);
    }

    /// <summary>
    /// Gets the data from xml.先打开xml然后读取xml里面的内容
    /// </summary>
    /// <returns>The data from xml.</returns>
    /// <param name="r_and_s">R_and_s.</param>
    public List<SunmArchiveBean> GetDataFromXml()
    {
        _xmlDoc.Load(XmlSave_Path);
        if (null == dataSetList)
        {
            dataSetList = new List<SunmArchiveBean>();
        }
        if (_xmlDoc.DocumentElement.HasChildNodes)
        {
            dataSetList.Clear();
            XmlNodeList _chilidlist = _xmlDoc.DocumentElement.ChildNodes;
            foreach (XmlNode xn in _chilidlist)
            {
                XmlElement xe = (XmlElement)xn;
                SunmArchiveBean xds = new SunmArchiveBean(  Convert.ToByte(xe.ChildNodes[0].InnerText),
                                                            Convert.ToSingle(xe.ChildNodes[1].InnerText),
                                                            Convert.ToSingle(xe.ChildNodes[2].InnerText),
                                                            Convert.ToSingle(xe.ChildNodes[3].InnerText),
                                                            xe.ChildNodes[4].InnerText);
                xds.id = xe.Attributes[0].InnerText;
                dataSetList.Add(xds);
            }
        }
        _xmlDoc.Save(XmlSave_Path);
        return dataSetList;
    }

    /// <summary>
    /// Removes the item by identifier.通过id属性找到节点，然后删除
    /// </summary>
    /// <param name="id">Identifier.</param>
    /// <param name="r_and_s">R_and_s.</param>
    public void RemoveItemById(string _vector3)
    {
        _xmlDoc.Load(XmlSave_Path);
        if (_xmlDoc.DocumentElement.HasChildNodes)
        {
            XmlNodeList _chilidlist = _xmlDoc.DocumentElement.ChildNodes;
            for (int i = 0; i < _chilidlist.Count; ++i)
            {
                if (_chilidlist[i].Attributes[0].InnerText.Equals(_vector3))
                {
                    _xmlDoc.DocumentElement.RemoveChild(_chilidlist[i]);
                    break;
                }
            }
        }
        _xmlDoc.Save(XmlSave_Path);
        //ReFreshXmlId(r_and_s);
    }
}
